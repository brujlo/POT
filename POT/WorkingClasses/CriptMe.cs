using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POT.WorkingClasses
{
    class CriptMe
    {
        public String Cript(String value)
        {
            byte[] toHash = Encoding.UTF8.GetBytes(value);

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(toHash);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
