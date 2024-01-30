using System.Security.Cryptography;
using System.Text;

namespace NachaFileParser
{
    public class Utility
    {
        public static string GetGuidHash(string batchId, int lineNumber, string entryDetailsId)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(batchId + lineNumber + entryDetailsId);
                var hashBytes = md5.ComputeHash(inputBytes);
                var guid = new Guid(hashBytes);
                return guid.ToString();
            }
        }
    }
}