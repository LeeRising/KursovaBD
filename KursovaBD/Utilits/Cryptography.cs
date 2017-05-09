using System.Security.Cryptography;
using System.Text;

namespace KursovaBD
{
    public class Cryptography
    {
        internal static string getHashSha256(string password)
        {
            using (SHA256Managed crypt = new SHA256Managed())
            {
                StringBuilder hash = new StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
                return hash.ToString();
            }  
        }
    }
}
