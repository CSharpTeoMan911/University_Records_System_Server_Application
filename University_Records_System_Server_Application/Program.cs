using System;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Program:Client_Connections
    {
        private static string Selected_Menu = "Main Menu";
        private static System.Net.Sockets.Socket? server_socket;

        private sealed class Server_Variables_Mitigator:Server_Variables
        {
            internal static Task<short> Get_If_Server_Is_On_Or_Off()
            {
                return Task.FromResult(On_Off);
            }

            internal static Task<bool> Set_If_Server_Is_On_Or_Off(short on_off)
            {
                On_Off = on_off;
                return Task.FromResult(true);
            }
        }

        static void Main()
        {
            Server_Initiation();
        }


        private static async void Server_Initiation()
        {
            string output = String.Empty;

            if (Selected_Menu == "Main Menu")
            {
                output = Server_GUI.Main_Menu();
            }


            if (output == "Start")
            {
                await Server_Operation();
                Server_Initiation();
            }
            else if (output == "Stop")
            {
                if (server_socket != null)
                {
                    server_socket.Close();
                    server_socket.Dispose();
                }

                Server_Initiation();
            }
            else if (output == "Options menu")
            {

            }
            else if (output == "Exit")
            {
                Environment.Exit(0);
            }
            else
            {
                Server_Initiation();
            }
        }


        private static async Task<bool> Server_Operation()
        {
            try
            {
                server_socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                server_socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 1024));
                server_socket.Listen();


                System.Threading.Thread Server_Operation_Thread = new System.Threading.Thread(async () =>
                {
                    short Server_On_or_Off = await Server_Variables_Mitigator.Get_If_Server_Is_On_Or_Off();

                    while (Server_On_or_Off == 1)
                    {
                        Server_On_or_Off = await Server_Variables_Mitigator.Get_If_Server_Is_On_Or_Off();

                        if (server_socket != null)
                        {
                            try
                            {
                                await Client_Connections.Operation_Selection(server_socket.Accept());
                            }
                            catch { }
                        }
                    }
                });
                Server_Operation_Thread.Priority = System.Threading.ThreadPriority.Highest;
                Server_Operation_Thread.IsBackground = false;
                Server_Operation_Thread.Start();
            }
            catch
            {
                await Server_Variables_Mitigator.Set_If_Server_Is_On_Or_Off(0);
            }


            return true;
        }


    }
}