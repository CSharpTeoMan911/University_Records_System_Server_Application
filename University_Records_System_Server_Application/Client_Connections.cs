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
        private static readonly byte[] response = Encoding.UTF8.GetBytes("OK");




        // PROCESS CLIENT CONNECTIONS
        protected async static Task<bool> Operation_Selection(System.Net.Sockets.Socket client)
        {



            // SET THE RECEIVE BUFFER AND THE SEND BUFFER TO HAVE A SIZE OF 32.768 Kilobytes
            client.ReceiveBufferSize = 32768;
            client.SendBufferSize = 32768;


            // SET THE RECEIVE AND SEND TIMOUT OF THE SOCKET TO 1 SECOND
            client.ReceiveTimeout = 1000;
            client.SendTimeout = 1000;



            // "NetworkStream" OBJECT WHICH WRAPS THE CLIENT CONNECTION "Socket" OBJECT
            System.Net.Sockets.NetworkStream client_stream = new System.Net.Sockets.NetworkStream(client);


            // "SslStream" OBJECT WHICH WRAPS THE "NetworkStream" OBJECT TO ESTABLISH A SECURE TLS1.2 CONNECTION BETWEEN THE SERVER AND THE CLIENT
            System.Net.Security.SslStream secure_client_stream = new System.Net.Security.SslStream(client_stream, false, null, null);


            try
            {

                try
                {
                    // AUTHENTICATE THE SERVER AS THE SERVER OF THIS CONNECTION, SET THE ENCRYPTION PROTOCOL AS TLS1.2, AND PASS THE PUBLIC KEY OF THE SERVER TO THE CLIENT
                    secure_client_stream.AuthenticateAsServer(server_certificate, false, System.Security.Authentication.SslProtocols.Tls12, true);







                    // CALCULATE THE ROUND TRIP TIME (RTT) OF THE "SslStream" CONNECTION TO CALCULATE THE SPEED OF THE CONNECTION
                    int bytes_per_second = await Rount_Trip_Time_Calculator(secure_client_stream);







                    // BUFFER THAT HOLDS THE PAYLOAD'S LENGTH TO BE SENT BY THE CLIENT
                    byte[] payload_length = new byte[1024];

                    // CALCULATE THE TIMEOUT IN WHICH THE CONNECTION SHOULD BE DROPPED IF 1024 BYTES ARE NOT RECEIVED FROM THE CLIENT ACCORDING WITH THE CONNECTION SPEED
                    await Calculate_Connection_Timeout(client, payload_length.Length, bytes_per_second);

                    // READ THE PAYLOAD'S LENGTH TO BE SENT BY THE CLIENT IN THE BUFFER 
                    await secure_client_stream.ReadAsync(payload_length);








                    // CALCULATE THE TIMEOUT IN WHICH THE CONNECTION SHOULD BE DROPPED IF THE "SYN-ACK" MESSAGE SENT TO CLIENT IS NOT SENT ACCORDING WITH THE CONNECTION SPEED
                    await Calculate_Connection_Timeout(client, response.Length, bytes_per_second);
                    await secure_client_stream.WriteAsync(response, 0, response.Length);








                    // BUFFER THAT HOLDS THE CLIENT PAYLOAD TO BE RECEIVED
                    byte[] client_payload = new byte[BitConverter.ToInt32(payload_length, 0)];

                    // CALCULATE THE TIMEOUT IN WHICH THE CONNECTION SHOULD BE DROPPED IF THE CLIENT PAYLOAD IS NOT RECEIVED FROM THE CLIENT ACCORDING WITH THE CONNECTION SPEED
                    await Calculate_Connection_Timeout(client, client_payload.Length, bytes_per_second);


                    // READ RECURSIVELY THE PAYLOAD SENT BY THE CLIENT UNTIL THE NUMBER OF BYTES RECEIVED ARE THE SAME AS THE CLIENT PAYLOAD'S SIZE
                    int total_bytes_read = 0;

                    while (total_bytes_read < client_payload.Length)
                    {
                        total_bytes_read += await secure_client_stream.ReadAsync(client_payload, total_bytes_read, client_payload.Length - total_bytes_read);
                    }







                    // "Client_WSDL_Payload" OBJECT WHICH HOLDS THE RETURN VALUE PF THE "Deserialise_Client_Payload_Dispatcher()" METHOD
                    Client_WSDL_Payload deserialised_client_payload = await Serialization_And_MySQL_Connection_Dispatcher_Controller.Deserialise_Client_Payload_Dispatcher(client_payload);



                    // "serialised_payload_content" OBJECT WHICH HOLDS THE RESULT OF THE OPERTION REQUESTED TO BE PERFORMED BY THE CLIENT
                    string serialised_payload_content = await Serialization_And_MySQL_Connection_Dispatcher_Controller.Initiate_MySql_Connection_Dispatcher<string>(deserialised_client_payload.email__or__log_in_session_key, deserialised_client_payload.password__or__binary_content, deserialised_client_payload.function);


                    // "serialised_payload" OBJECT THAT HOLDS THE SERIALISED PAYLOAD TO BE SENT TO THE CLIENT
                    byte[] serialised_payload = await Serialization_And_MySQL_Connection_Dispatcher_Controller.Serialise_Server_Payload_Dispatcher(serialised_payload_content);






                    // BUFFER THAT HOLDS THE VALUE REGARDING THE LENGTH OF THE PAYLOAD TO BE SENT BY THE SERVER TO THE CLIENT
                    byte[] server_payload_length = BitConverter.GetBytes(serialised_payload.Length);

                    // CALCULATE THE TIMEOUT IN WHICH THE CONNECTION SHOULD BE DROPPED IF THE MESSAGE CONTAING THE LENGTH OF THE PAYLOAD TO BE SENT TO CLIENT IS NOT SENT ACCORDING WITH THE CONNECTION SPEED
                    await Calculate_Connection_Timeout(client, server_payload_length.Length, bytes_per_second);

                    // SEND THE LENGTH OF THE PAYLOAD TO BE SENT BY THE SERVER TO THE CLIENT
                    await secure_client_stream.WriteAsync(server_payload_length, 0, server_payload_length.Length);







                    // BUFFER THAT HOLDS THE "SYN-ACK" MESSAGE OF THE CLIENT
                    byte[] client_response = new byte[response.Length];

                    // CALCULATE THE TIMEOUT IN WHICH THE CONNECTION SHOULD BE DROPPED IF THE "SYN-ACK" MESSAGE IS NOT RECEIVED FROM THE CLIENT ACCORDING WITH THE CONNECTION SPEED
                    await Calculate_Connection_Timeout(client, client_response.Length, bytes_per_second);

                    // READ THE "SYN-ACK" MESSAGE SENT BY THE CLIENT
                    await secure_client_stream.ReadAsync(client_response);






                    // CALCULATE THE TIMEOUT IN WHICH THE CONNECTION SHOULD BE DROPPED IF PAYLOAD TO BE SENT TO CLIENT IS NOT SENT ACCORDING WITH THE CONNECTION SPEED
                    await Calculate_Connection_Timeout(client, serialised_payload.Length, bytes_per_second);

                    // SEND THE SERVER PAYLOAD TO THE CLIENT
                    await secure_client_stream.WriteAsync(serialised_payload, 0, serialised_payload.Length);





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




            return true;
        }





        // CALCULATOR THAT CALCULATES THE TIME TOOK TO TRANSMIT AND RECEIVE A PACKET OF A SET SIZE OVER THE MAIN CONNECTION.
        // THE PACKET TRANSMISSION PROCEDURE IS REPEATED 10 TIMES OVER THE CONNECTION AND THE MEDIAN VALUE OF THE TIME
        // IS CALCULATED. THIS MUST BE DONE IN ORDER TO ADJUST THE MAIN CONNECTION'S SOCKET RECEIVE AND SEND TIMEOUT
        // IN ACCORDACE WITH THE SIZE OF THE PACKET TO BE RECEIVED OR SENT RESPECTIVELY.
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





        // CALCULATE THE MAIN CONNECTION'S SEND OR RECEIVE TIMEOUT BASED ON THE RTT CALCULATED VALUE
        // IN ACCORDANCE WITH THE PACKET SIZE TO BE SENT OR RECEIVED.
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
