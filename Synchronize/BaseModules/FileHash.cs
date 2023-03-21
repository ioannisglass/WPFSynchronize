using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Synchronize.BaseModules
{
    class FileHash
    {
        private static string GetHashString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        static public string checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return GetHashString(md5.ComputeHash(stream));
                }
            }
        }
        static public string checkSHA1(string filename)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return GetHashString(sha1.ComputeHash(stream));
                }
            }
        }
    }
}
