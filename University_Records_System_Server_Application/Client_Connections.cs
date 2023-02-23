using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Client_Connections
    {
        protected sealed class Payload_Serialisation_and_Deserialisation_Mitigator:Payload_Serialisation_and_Deserialisation
        {
            internal static async Task<Client_WSDL_Payload> Deserialise_Client_Payload_Initiator(byte[] payload)
            {
                return await Deserialise_Client_Payload(payload);
            }

            internal static async Task<byte[]> Serialise_Server_Payload_Initiator(string payload)
            {
                return await Serialise_Server_Payload(payload);
            }
        }


        protected sealed class MySql_Connection_Initiator_Mitigator:MySql_Connection_Initiator
        {
            internal static async Task<string> Initiate_MySql_Connection_Initiator<Password__Or__Binary_Content>(string email__or__log_in_session_key, Password__Or__Binary_Content password__or__binary_content, string function)
            {
                return await Initiate_MySql_Connection<Password__Or__Binary_Content>(email__or__log_in_session_key, password__or__binary_content, function);
            }
        }






        protected async static Task<Server_WSDL_Payload> Operation_Selection(System.Net.Sockets.Socket client)
        {
            byte[] response = Encoding.UTF8.GetBytes("OK");
            Server_WSDL_Payload server_payload = new Server_WSDL_Payload();



            System.Net.Sockets.NetworkStream client_stream = new System.Net.Sockets.NetworkStream(client);

            try
            {
                byte[] payload_length = new byte[1024];
                await client_stream.ReadAsync(payload_length);





                await client_stream.WriteAsync(response, 0, response.Length);





                byte[] client_payload = new byte[BitConverter.ToInt32(payload_length, 0)];
                await client_stream.ReadAsync(client_payload, 0, client_payload.Length);
               


                
                Client_WSDL_Payload deserialised_client_payload = await Payload_Serialisation_and_Deserialisation_Mitigator.Deserialise_Client_Payload_Initiator(client_payload);



                string serialised_payload_content = await MySql_Connection_Initiator_Mitigator.Initiate_MySql_Connection_Initiator<string>(deserialised_client_payload.email__or__log_in_session_key, deserialised_client_payload.password__or__binary_content, deserialised_client_payload.function);




                byte[] serialised_payload = await Payload_Serialisation_and_Deserialisation_Mitigator.Serialise_Server_Payload_Initiator(serialised_payload_content);




                byte[] server_payload_length = BitConverter.GetBytes(serialised_payload.Length);
                await client_stream.WriteAsync(server_payload_length, 0, server_payload_length.Length);




                byte[] client_response = new byte[response.Length];
                await client_stream.ReadAsync(client_response);




                await client_stream.WriteAsync(serialised_payload, 0, serialised_payload.Length);




                
            }
            catch
            {
                if(client_stream != null)
                {
                    client_stream.Close();
                }

                if(client != null)
                {
                    client.Close();
                }
            }
            finally
            {
                if (client_stream != null)
                {
                    client_stream.Close();
                    client_stream.Dispose();
                }

                if (client != null)
                {
                    client.Close();
                    client.Dispose();
                }
            }

            return server_payload;
        }
    }
}
