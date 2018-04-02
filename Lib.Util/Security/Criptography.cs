#region Usings
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Lib.Util.Security
{
    public class Criptography
    {
        #region Public Static Methods

        /// <summary>
        /// Encrypt a string into a SHA256 hash
        /// </summary>
        /// <param name="stringToEncrypt">String to encrypt</param>
        /// <returns></returns>
        public static string Encrypt(string stringToEncrypt)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(stringToEncrypt));

                foreach (var b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        #endregion
    }
}
