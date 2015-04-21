using System.Linq;
using System.Text;

namespace Common
{
    public static class Helpers
    {
        #region Constants
        private const string Salt = "47DAB234-3CA8-42A4-A5B4-DA83D4228B1D";
        #endregion

        public static string HashPassword(string password, string salt = null)
        {
            byte[] bytes = null;
            if (string.IsNullOrEmpty(salt))
                salt = Salt;

            using (var shaProvider = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                bytes = shaProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password + salt));
            }

            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }
    }
}
