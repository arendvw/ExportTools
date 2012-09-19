using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Elephant
{
    class Tools
    {
        public static string getHash(Object input)
        {
            // generate a unique id for an arbitrairy object
            MemoryStream memoryStream = new MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, input);
            System.Security.Cryptography.SHA1CryptoServiceProvider sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(memoryStream.ToArray());
            string str = Convert.ToBase64String(sha.Hash);
            return str;
        }
    }
}
