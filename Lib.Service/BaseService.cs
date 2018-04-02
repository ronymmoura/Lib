#region Usings
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#endregion

namespace Lib.Service
{
    public class BaseService<TEntity>
    {
        #region Fields

        readonly WebApiOptions WebApiOptions;

        #endregion

        #region Properties

        public string Token { get; private set; }

        public bool UsesToken { get; private set; }

        #endregion

        #region Constructors

        public BaseService(WebApiOptions apiOptions)
        {
            WebApiOptions = apiOptions;
        }

        public BaseService(WebApiOptions apiOptions, string token)
        {
            WebApiOptions = apiOptions;
            Token = token;
            UsesToken = true;
        }

        #endregion

        #region Public Methods

        #region Get

        protected async Task<TEntity> ExecuteGet(string path)
        {
            return await ExecuteGet<TEntity>(path);
        }

        protected async Task<TEntity2> ExecuteGet<TEntity2>(string path)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var response = await client.GetAsync(WebApiOptions.Url + path);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<TEntity2>(stringResponse);

                    return responseObject;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected async Task<List<TEntity>> ExecuteGetList(string path)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var response = await client.GetAsync(WebApiOptions.Url + path);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<List<TEntity>>(stringResponse);

                    return responseObject;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected TEntity ExecuteGetSync(string path)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var response = client.GetAsync(WebApiOptions.Url + path).Result;
                    response.EnsureSuccessStatusCode();

                    var stringResponse = response.Content.ReadAsStringAsync().Result;
                    var responseObject = JsonConvert.DeserializeObject<TEntity>(stringResponse);

                    return responseObject;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Post

        protected async Task<TEntity> ExecutePost(string path, TEntity content)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var httpContent = new StringContent(JsonConvert.SerializeObject(content));
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync(WebApiOptions.Url + path, httpContent);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TEntity>(stringResponse);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected async Task<TEntity> ExecutePost(string path, List<KeyValuePair<string, string>> paramList)
        {
            return await ExecutePost<TEntity>(path, paramList);
        }

        protected async Task<TEntity2> ExecutePost<TEntity2>(string path, List<KeyValuePair<string, string>> paramList, bool isDynamic = false)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var jsonResult = new StringBuilder();

                    for (int i = 0; i < paramList.Count; i++)
                    {
                        jsonResult.Append($"{paramList[i].Key}={paramList[i].Value}");

                        if (i < (paramList.Count - 1))
                            jsonResult.Append("&");
                    }

                    var httpContent = new StringContent(jsonResult.ToString());
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    var response = await client.PostAsync(WebApiOptions.Url + path, httpContent);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();

                    if (isDynamic)
                    {
                        var converter = new ExpandoObjectConverter();
                        dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(stringResponse, converter);
                        return result;
                    }
                    else
                        return JsonConvert.DeserializeObject<TEntity2>(stringResponse);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected TEntity2 ExecutePostSync<TEntity2>(string path, List<KeyValuePair<string, string>> paramList, bool isDynamic = false)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var jsonResult = new StringBuilder();

                    for (int i = 0; i < paramList.Count; i++)
                    {
                        jsonResult.Append($"{paramList[i].Key}={paramList[i].Value}");

                        if (i < (paramList.Count - 1))
                            jsonResult.Append("&");
                    }

                    var httpContent = new StringContent(jsonResult.ToString());
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    var response = client.PostAsync(WebApiOptions.Url + path, httpContent).Result;
                    response.EnsureSuccessStatusCode();

                    var stringResponse = response.Content.ReadAsStringAsync().Result;

                    if (isDynamic)
                    {
                        var converter = new ExpandoObjectConverter();
                        dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(stringResponse, converter);
                        return result;
                    }

                    return JsonConvert.DeserializeObject<TEntity2>(stringResponse);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Put

        protected async Task<TEntity> ExecutePut(string path, TEntity content)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var httpContent = new StringContent(JsonConvert.SerializeObject(content));
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PutAsync(WebApiOptions.Url + path, httpContent);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TEntity>(stringResponse);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Delete

        protected async Task ExecuteDelete(string path)
        {
            using (var client = CreateClient())
            {
                try
                {
                    var response = await client.DeleteAsync(WebApiOptions.Url + path);
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.ConnectionClose = true;

            if (UsesToken)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            return client;
        }

        #endregion
    }
}
