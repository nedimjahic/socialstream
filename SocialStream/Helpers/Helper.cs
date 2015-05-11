using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SocialStream.Helpers
{
    public static class Hash
    {
        public static string getSHA1String(string text)
        {
            SHA1 sha1 = SHA1.Create();

            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(text));

            //create new instance of StringBuilder to save hashed data
            StringBuilder hashedValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                hashedValue.Append(hashData[i].ToString());
            }

            return hashedValue.ToString();
        }
    }
}