using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Serialization_And_MySQL_Connection_Dispatcher_Controller
    {
        private static Serialization_And_MySQL_Connection_Dispatcher Serialization_And_MySQL_Connection_Dispatcher = new Serialization_And_MySQL_Connection_Dispatcher();

        public static async Task<byte[]> Serialise_Server_Payload_Dispatcher(string content)
        {
            return await Serialization_And_MySQL_Connection_Dispatcher.Dispatcher(content);
        }

        public static async Task<Client_WSDL_Payload> Deserialise_Client_Payload_Dispatcher(byte[] payload)
        {
            return await Serialization_And_MySQL_Connection_Dispatcher.Dispatcher(payload);
        }

        public static async Task<Tuple<bool, string>> Initiate_MySql_Connection_Dispatcher<Password__Or__Binary_Content>(string email__or__log_in_session_key, Password__Or__Binary_Content password__or__binary_content, string function)
        {
            return await Serialization_And_MySQL_Connection_Dispatcher.Dispatcher<Password__Or__Binary_Content>(email__or__log_in_session_key, password__or__binary_content, function);
        }
    }
}
