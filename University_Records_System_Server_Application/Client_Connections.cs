using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Client_Connections:Server_Variables
    {

        private static System.Diagnostics.Stopwatch speed_checkup = new System.Diagnostics.Stopwatch();




        protected async static Task<Server_WSDL_Payload> Operation_Selection(System.Net.Sockets.Socket client)
        {

            byte[] response = Encoding.UTF8.GetBytes("OK");
            byte[] is_binary_file = new byte[1024];


            client.ReceiveBufferSize = 32768;
            client.SendBufferSize = 32768;

            client.ReceiveTimeout = 1000;
            client.SendTimeout = 1000;


            Server_WSDL_Payload server_payload = new Server_WSDL_Payload();



            System.Net.Sockets.NetworkStream client_stream = new System.Net.Sockets.NetworkStream(client);

            
            System.Net.Security.SslStream secure_client_stream = new System.Net.Security.SslStream(client_stream, false, null, null);

            try
            {

                try
                {
                    secure_client_stream.AuthenticateAsServer(server_certificate, false, System.Security.Authentication.SslProtocols.Tls11, true);


                    int bytes_per_second = await Rount_Trip_Time_Calculator(secure_client_stream);


                    await Calculate_Connection_Timeout(client, is_binary_file.Length, bytes_per_second);
                    await secure_client_stream.ReadAsync(is_binary_file);



                    await Calculate_Connection_Timeout(client, response.Length, bytes_per_second);
                    await secure_client_stream.WriteAsync(response);




                    byte[] payload_length = new byte[1024];
                    await Calculate_Connection_Timeout(client, payload_length.Length, bytes_per_second);
                    await secure_client_stream.ReadAsync(payload_length);




                    await Calculate_Connection_Timeout(client, response.Length, bytes_per_second);
                    await secure_client_stream.WriteAsync(response, 0, response.Length);






                    byte[] client_payload = new byte[BitConverter.ToInt32(payload_length, 0)];
                    await Calculate_Connection_Timeout(client, client_payload.Length, bytes_per_second);


                    switch (BitConverter.ToInt32(is_binary_file, 0))
                    {
                        case 1:
                            await Task.Run(() =>
                            {
                                for (int index = 0; index < client_payload.Length; index++)
                                {
                                    client_payload[index] = (byte)secure_client_stream.ReadByte();
                                }
                            });
                            break;

                        case 0:
                            await secure_client_stream.ReadAsync(client_payload, 0, client_payload.Length);
                            break;
                    }





                    
                    Client_WSDL_Payload deserialised_client_payload = await Serialization_And_MySQL_Connection_Dispatcher_Controller.Deserialise_Client_Payload_Dispatcher(client_payload);




                    Tuple<bool, string> mysql_extracted_data_and_type = await Serialization_And_MySQL_Connection_Dispatcher_Controller.Initiate_MySql_Connection_Dispatcher<string>(deserialised_client_payload.email__or__log_in_session_key, deserialised_client_payload.password__or__binary_content, deserialised_client_payload.function);
                    bool payload_type_is_binary_file = mysql_extracted_data_and_type.Item1;
                    string serialised_payload_content = mysql_extracted_data_and_type.Item2;



                    byte[] serialised_payload = await Serialization_And_MySQL_Connection_Dispatcher_Controller.Serialise_Server_Payload_Dispatcher(serialised_payload_content);




                    byte[] server_payload_length = BitConverter.GetBytes(serialised_payload.Length);
                    await Calculate_Connection_Timeout(client, server_payload_length.Length, bytes_per_second);
                    await secure_client_stream.WriteAsync(server_payload_length, 0, server_payload_length.Length);




                    byte[] client_response = new byte[response.Length];
                    await Calculate_Connection_Timeout(client, client_response.Length, bytes_per_second);
                    await secure_client_stream.ReadAsync(client_response);




                    await Calculate_Connection_Timeout(client, serialised_payload.Length, bytes_per_second);

                    switch (payload_type_is_binary_file)
                    {
                        case true:
                            await Task.Run(() =>
                            {
                                for (int index = 0; index < serialised_payload.Length; index++)
                                {
                                    secure_client_stream.WriteByte(serialised_payload[index]);
                                }
                            });
                            break;

                        case false:
                            await secure_client_stream.WriteAsync(serialised_payload, 0, serialised_payload.Length);
                            break;
                    }





                }
                catch (Exception E)
                {
                    await Server_Error_Logs(E, "Operation_Selection");
                    if (secure_client_stream != null)
                    {
                        secure_client_stream.Close();
                    }
                }
                finally
                {
                  
                    if (secure_client_stream != null)
                    {
                        secure_client_stream.Close();
                        secure_client_stream.Dispose();
                    }
                }
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Operation_Selection");
                if (client_stream != null)
                {
                    client_stream.Close();
                }

                if (client != null)
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


        private static async Task<int> Rount_Trip_Time_Calculator(System.Net.Security.SslStream secure_client_stream)
        {
            long time = 0;


            byte[] buffer = new byte[1500];

            int count = 0;


        RTT_Input:

            speed_checkup.Start();
            await secure_client_stream.WriteAsync(buffer, 0, buffer.Length);
            speed_checkup.Stop();


            time += speed_checkup.ElapsedMilliseconds;
            speed_checkup.Reset();


            speed_checkup.Start();
            await secure_client_stream.ReadAsync(buffer, 0, buffer.Length);
            speed_checkup.Stop();


            time += speed_checkup.ElapsedMilliseconds;
            speed_checkup.Reset();

            if(count < 10)
            {
                count++;
                goto RTT_Input;
            }

            if (time < 1)
            {
                time = 1;
            }

            time = 24 * (time / 10) * 125000;

            speed_checkup.Reset();

            if (time < 1)
            {
                time = 1;
            }

            return (int)time;

        }



        private static Task<int> Calculate_Connection_Timeout(System.Net.Sockets.Socket client, int payload_size, int bytes_per_second)
        {
            int seconds = 1000 * (payload_size / bytes_per_second) + 1000;

            if (seconds > 1000)
            {
                client.SendTimeout = seconds;
                client.ReceiveTimeout = seconds;
            }
            else
            {
                client.SendTimeout = 1000;
                client.ReceiveTimeout = 1000;
            }


            return Task.FromResult(0);
        }

    }
}
