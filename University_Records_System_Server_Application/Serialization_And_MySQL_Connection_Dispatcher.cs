using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{

    internal partial class Serialization_And_MySQL_Connection_Dispatcher
    {
        private static Payload_Serialisation_and_Deserialisation Payload_Serialisation_And_Deserialisation = new Payload_Serialisation_and_Deserialisation();
        private static MySql_Connection_Initiator MySql_Connection_Initiator = new MySql_Connection_Initiator();




        //DISPATCHER USED FOR DATA SERIALISATION
        public static async Task<byte[]> Dispatcher(string content)
        {
            return await Payload_Serialisation_And_Deserialisation.Serialise_Server_Payload(content);
        }


        // DISPATCHER USED FOR DATA DE-SERIALISATION
        public static async Task<Client_WSDL_Payload> Dispatcher(byte[] payload)
        {
            return await Payload_Serialisation_And_Deserialisation.Deserialise_Client_Payload(payload);
        }


        // DISPATCHER USED FOR MYSQL CONNECTION INITIATION
        public static async Task<string> Dispatcher<Password__Or__Binary_Content>(string email__or__log_in_session_key, Password__Or__Binary_Content password__or__binary_content, string function)
        {
            return await MySql_Connection_Initiator.Initiate_MySql_Connection(email__or__log_in_session_key, password__or__binary_content, function);
        }
    }
}
