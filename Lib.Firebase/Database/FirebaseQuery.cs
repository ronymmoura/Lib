using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Lib.Firebase.Http;
using Newtonsoft.Json;

namespace Lib.Firebase.Database
{
    /// <summary>
    /// Represents a firebase query. 
    /// </summary>
    public abstract class FirebaseQuery : IFirebaseQuery, IDisposable
    {
        protected readonly FirebaseQuery Parent;

        HttpClient client;

        /// <summary> 
        /// Initializes a new instance of the <see cref="FirebaseQuery"/> class.
        /// </summary>
        /// <param name="parent"> The parent of this query. </param>
        /// <param name="client"> The owning client. </param>
        protected FirebaseQuery(FirebaseQuery parent, FirebaseClient client)
        {
            Client = client;
            Parent = parent;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        public FirebaseClient Client { get; }

        /// <summary>
        /// Queries the firebase server once returning collection of items.
        /// </summary>
        /// <typeparam name="T"> Type of elements. </typeparam>
        /// <returns> Collection of <see cref="FirebaseObject{T}"/> holding the entities returned by server. </returns>
        public async Task<IReadOnlyCollection<FirebaseObject<T>>> OnceAsync<T>()
        {
            var path = await BuildUrlAsync().ConfigureAwait(false);

            using (var client = new HttpClient())
            {
                return await client.GetObjectCollectionAsync<T>(path).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Assumes given query is pointing to a single object of type <typeparamref name="T"/> and retrieves it.
        /// </summary>
        /// <typeparam name="T"> Type of elements. </typeparam>
        /// <returns> Single object of type <typeparamref name="T"/>. </returns>
        public async Task<T> OnceSingleAsync<T>()
        {
            var path = await BuildUrlAsync().ConfigureAwait(false);

            using (var client = new HttpClient())
            {
                var data = await client.GetStringAsync(path).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        /// <summary>
        /// Starts observing this query watching for changes real time sent by the server.
        /// </summary>
        /// <typeparam name="T"> Type of elements. </typeparam>
        /// <param name="elementRoot"> Optional custom root element of received json items. </param>
        /// <returns> Observable stream of <see cref="FirebaseEvent{T}"/>. </returns>
        public IObservable<FirebaseEvent<T>> AsObservable<T>(string elementRoot = "")
        {
            return Observable.Create<FirebaseEvent<T>>(observer => new FirebaseSubscription<T>(observer, this, elementRoot, new FirebaseCache<T>()).Run());
        }

        /// <summary>
        /// Builds the actual URL of this query.
        /// </summary>
        /// <returns> The <see cref="string"/>. </returns>
        public async Task<string> BuildUrlAsync()
        {
            // if token factory is present on the parent then use it to generate auth token
            if (Client.AuthTokenAsyncFactory != null)
            {
                var token = await Client.AuthTokenAsyncFactory().ConfigureAwait(false);
                return WithAuth(token).BuildUrl(null);
            }

            return BuildUrl(null);
        }

        /// <summary>
        /// Posts given object to repository.
        /// </summary>
        /// <param name="obj"> The object. </param> 
        /// <param name="generateKeyOffline"> Specifies whether the key should be generated offline instead of online. </param> 
        /// <typeparam name="T"> Type of <see cref="obj"/> </typeparam>
        /// <returns> Resulting firebase object with populated key. </returns>
        public async Task<FirebaseObject<T>> PostAsync<T>(T obj, bool generateKeyOffline = true)
        {
            // post generates a new key server-side, while put can be used with an already generated local key
            if (generateKeyOffline)
            {
                var key = FirebaseKeyGenerator.Next();
                await new ChildQuery(this, () => key, Client).PutAsync(obj).ConfigureAwait(false);

                return new FirebaseObject<T>(key, obj);
            }
            else
            {
                var c = GetClient();
                var data = await SendAsync(c, obj, HttpMethod.Post).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<PostResult>(data);

                return new FirebaseObject<T>(result.Name, obj);
            }
        }

        /// <summary>
        /// Patches data at given location instead of overwriting them. 
        /// </summary> 
        /// <param name="obj"> The object. </param>  
        /// <typeparam name="T"> Type of <see cref="obj"/> </typeparam>
        /// <returns> The <see cref="Task"/>. </returns>
        public async Task PatchAsync<T>(T obj)
        {
            var c = GetClient();

            await SendAsync(c, obj, new HttpMethod("PATCH")).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets or overwrites data at given location. 
        /// </summary> 
        /// <param name="obj"> The object. </param>  
        /// <typeparam name="T"> Type of <see cref="obj"/> </typeparam>
        /// <returns> The <see cref="Task"/>. </returns>
        public async Task PutAsync<T>(T obj)
        {
            var c = GetClient();

            await SendAsync(c, obj, HttpMethod.Put).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes data from given location.
        /// </summary>
        /// <returns> The <see cref="Task"/>. </returns>
        public async Task DeleteAsync()
        {
            var c = GetClient();
            var url = await BuildUrlAsync().ConfigureAwait(false);
            var result = await c.DeleteAsync(url).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Disposes this instance.  
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
        }

        /// <summary>
        /// Build the url segment of this child.
        /// </summary>
        /// <param name="child"> The child of this query. </param>
        /// <returns> The <see cref="string"/>. </returns>
        protected abstract string BuildUrlSegment(FirebaseQuery child);

        private string BuildUrl(FirebaseQuery child)
        {
            var url = BuildUrlSegment(child);

            if (Parent != null)
            {
                url = Parent.BuildUrl(this) + url;
            }

            return url;
        }

        private HttpClient GetClient()
        {
            if (client == null)
            {
                client = new HttpClient();
            }

            return client;
        }

        private async Task<string> SendAsync<T>(HttpClient client, T obj, HttpMethod method)
        {
            var url = await BuildUrlAsync().ConfigureAwait(false);
            var message = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(obj))
            };

            var result = await client.SendAsync(message).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();

            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
