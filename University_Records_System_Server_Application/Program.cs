using System;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Program:Client_Connections
    {
        private static string Selected_Menu = "Main Menu";
        private static bool Server_Startup = false;
        private static System.Net.Sockets.Socket? server_socket;


        private static System.Timers.Timer server_main_loop = new System.Timers.Timer();


        private static bool server_shutdown;


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

            internal static async Task<bool> Load_Certificate_At_Startup_Initiator()
            {
                return await Load_Certificate_At_Startup();
            }
        }

        private sealed class Server_Functions_Mitigator:Server_Functions
        {
            internal static void Delete_Expired_Database_Items_Initiator()
            {
                Delete_Expired_Database_Items();
            }
        }

        static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Server_Initiation();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            if(server_main_loop != null)
            {
                server_main_loop.Close();
                server_main_loop.Dispose();
            }

            if(server_socket != null)
            {
                server_socket.Close();
                server_socket.Dispose();
            }

            server_shutdown = true;
        }

        private static async void Server_Initiation()
        {
            if(Server_Startup == false)
            {
                await Server_Variables_Mitigator.Load_Certificate_At_Startup_Initiator();
                Server_Startup = true;
            }


            string output = String.Empty;

            if (Selected_Menu == "Main Menu")
            {
                output = Server_GUI.Main_Menu();
            }


            if (output == "Start")
            {
                if(server_main_loop != null)
                {
                    server_main_loop.Elapsed += Server_main_loop_Elapsed;
                    server_main_loop.Interval = 100;
                    server_main_loop.Start();
                }

                await Server_Operation();
                Server_Initiation();
            }
            else if (output == "Stop")
            {
                if (server_main_loop != null)
                {
                    server_main_loop.Elapsed -= Server_main_loop_Elapsed;
                    server_main_loop.Stop();
                }

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




        private static void Server_main_loop_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(server_shutdown == false)
            {
                System.Threading.Thread server_main_loop_thread = new System.Threading.Thread(() =>
                {
                    Server_Functions_Mitigator.Delete_Expired_Database_Items_Initiator();
                });
                server_main_loop_thread.Priority = System.Threading.ThreadPriority.AboveNormal;
                server_main_loop_thread.IsBackground = true;
                server_main_loop_thread.Start();
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