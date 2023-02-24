﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Cryptographic_Functions
    {
        private static System.Random random = new Random();
        private static string[] character_list = new string[] { "`", "¬", "¦", "!", "\"", "£", "$", "%", "^", "&", "*", "(", ")", "-", "_", "+", "=", "[", "{", "]", "}", ";", ":", "@", "'", "~", "#", "€", ",", "<", ".", ">", "?", "/", "\\", "|"};
        private static string[] lowercase_letters = new string[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};


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

        protected static Task<byte[]> Content_Hasher(string content)
        {
            byte[] Content_Hashing_Result = new byte[] { };
            System.Security.Cryptography.HashAlgorithm content_hasher = System.Security.Cryptography.SHA256.Create();

            try
            {
                Content_Hashing_Result = content_hasher.ComputeHash(Encoding.UTF8.GetBytes(content));
            }
            catch { }
            finally
            {
                if (content_hasher != null)
                {
                    content_hasher.Dispose();
                }
            }

            return Task.FromResult(Content_Hashing_Result);
        }

    }
}
