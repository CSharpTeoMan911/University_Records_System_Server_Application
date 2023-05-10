using System;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Program:Server_Functions
    {
        private static string Selected_Menu = "Main Menu";
        private static bool Server_Startup = false;
        private static System.Net.Sockets.Socket? server_socket;


        private static System.Timers.Timer server_main_loop = new System.Timers.Timer();
        private static bool server_shutdown;






        static void Main()
        {
            System.Threading.ThreadPool.SetMinThreads(1000, 1000);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Server_Initiation();
        }

        private static async void CurrentDomain_ProcessExit(object? sender, EventArgs e)
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

            await Unload_Certificate();

            server_shutdown = true;
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
                if(server_main_loop != null)
                {
                    await Load_Certificate();

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
                    await Unload_Certificate();

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
                System.Threading.Thread server_main_loop_thread = new System.Threading.Thread(async() =>
                {
                    await Delete_Expired_Database_Items();
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
                int Worker_Threads = 0;
                int Port_Threads = 0;

                System.Threading.ThreadPool.GetAvailableThreads(out Worker_Threads, out Port_Threads);


                if(Worker_Threads < 1000)
                {
                    System.Threading.ThreadPool.SetMaxThreads(Worker_Threads + 1000, Port_Threads);
                }
                else if(Worker_Threads > 1000)
                {
                    System.Threading.ThreadPool.SetMaxThreads(Worker_Threads - (Worker_Threads - 1000), Port_Threads);
                }



                if(Port_Threads < 1000)
                {
                    System.Threading.ThreadPool.SetMaxThreads(Worker_Threads, Port_Threads + 1000);
                }
                else if(Port_Threads > 1000)
                {
                    System.Threading.ThreadPool.SetMaxThreads(Worker_Threads, Port_Threads - (Port_Threads - 1000));
                }





                server_socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                server_socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 1024));
                server_socket.Listen(1000);

                System.Threading.Thread Server_Operation_Thread = new System.Threading.Thread(async () =>
                {
                    short Server_On_or_Off = await Get_If_Server_Is_On_Or_Off();

                    while (Server_On_or_Off == 1)
                    {
                        Server_On_or_Off = await Get_If_Server_Is_On_Or_Off();

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
                On_Off = 0;
            }


            return true;
        }


        

    }
}