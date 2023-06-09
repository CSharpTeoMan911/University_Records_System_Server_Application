using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Cryptographic_Functions:Server_Functions
    {
        private static System.Random random = new Random();
        private static string[] character_list = new string[] { "`", "¬", "¦", "!", "\"", "£", "$", "%", "^", "&", "*", "(", ")", "-", "_", "+", "=", "[", "{", "]", "}", ";", ":", "@", "'", "~", "#", "€", ",", "<", ".", ">", "?", "/", "\\", "|"};
        private static string[] lowercase_letters = new string[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};




        // METHOD THAT CREATES A 30 CHARACTERS RANDOM KEY 
        protected static Task<string> Create_Random_Key()
        {
            string random_key = String.Empty;


            for(int index = 0; index < 30; index++)
            {
                switch(random.Next(0, 3))
                {
                    case 0:
                        random_key += lowercase_letters[random.Next(0, lowercase_letters.Length)];
                        break;

                    case 1:
                        random_key += lowercase_letters[random.Next(0, lowercase_letters.Length)].ToUpper();
                        break;

                    case 2:
                        random_key += character_list[random.Next(0, character_list.Length)];
                        break;
                }
            }

            return Task.FromResult(random_key);
        }


        // METHOD THAT HASHES CONTENT USING THE SHA256 HASHING ALGORITHM
        protected static async Task<byte[]> Content_Hasher(string content)
        {
            byte[] Content_Hashing_Result = new byte[] { };
            System.Security.Cryptography.HashAlgorithm content_hasher = System.Security.Cryptography.SHA256.Create();

            try
            {
                Content_Hashing_Result = content_hasher.ComputeHash(Encoding.UTF8.GetBytes(content));
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Serialise_Server_Payload");
            }
            finally
            {
                if (content_hasher != null)
                {
                    content_hasher.Dispose();
                }
            }

            return Content_Hashing_Result;
        }

    }
}
