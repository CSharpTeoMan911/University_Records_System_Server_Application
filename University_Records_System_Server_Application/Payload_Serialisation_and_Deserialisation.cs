using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Payload_Serialisation_and_Deserialisation
    {
        protected static Task<byte[]> Serialise_Server_Payload()
        {
            return Task.FromResult(new byte[] { });
        }


        protected static Task<Client_WSDL_Payload> Deserialise_Client_Payload(byte[] payload)
        {
            Client_WSDL_Payload client_payload = new Client_WSDL_Payload();

            System.IO.TextReader payload_stream = new System.IO.StringReader(Encoding.UTF8.GetString(payload));

            try
            {
               
                System.Xml.Serialization.XmlSerializer payload_deserialiser = new System.Xml.Serialization.XmlSerializer(client_payload.GetType());
                client_payload = (Client_WSDL_Payload)payload_deserialiser?.Deserialize(payload_stream);

                client_payload.email__or__log_in_session_key = Encoding.UTF8.GetString(Convert.FromBase64String(client_payload.email__or__log_in_session_key));
                client_payload.password__or__binary_content = Encoding.UTF8.GetString(Convert.FromBase64String(client_payload.password__or__binary_content));
                client_payload.function = Encoding.UTF8.GetString(Convert.FromBase64String(client_payload.function));

            }
            catch(Exception E)
            {
                if (payload_stream != null)
                {
                    payload_stream.Close();
                }
            }
            finally
            {
                if (payload_stream != null)
                {
                    payload_stream.Close();
                    payload_stream.Dispose();
                }
            }


            return Task.FromResult(client_payload);
        }



        protected static async Task<byte[]> Serialise_Server_Payload(string content)
        {

            byte[] serialised_payload = Encoding.UTF8.GetBytes("FAILED");

            System.IO.MemoryStream payload_stream = new System.IO.MemoryStream();

            Server_WSDL_Payload payload = new Server_WSDL_Payload();
            payload.response = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

            try
            {
                System.Xml.Serialization.XmlSerializer payload_serialiser = new System.Xml.Serialization.XmlSerializer(payload.GetType());
                payload_serialiser.Serialize(payload_stream, payload);

                serialised_payload = payload_stream.ToArray();
                await payload_stream.FlushAsync();
            }
            catch
            {
                if(payload_stream != null)
                {
                    payload_stream.Close();
                }
            }
            finally
            {
                if (payload_stream != null)
                {
                    payload_stream.Close();
                    payload_stream.Dispose();
                }
            }

            return serialised_payload;
        }
    }
}
