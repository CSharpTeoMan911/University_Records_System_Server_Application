using System;
using System.Threading;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Program:Server_Functions
    {
        private static System.Net.Sockets.Socket? server_socket;
        private static System.Timers.Timer server_main_loop;
        private static bool server_shutdown;


        static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += Shutdown;
            Startup();
        }

        private async static void Startup()
        {
            await Server_Initiation();
        }

        private static async void Shutdown(object? sender, EventArgs e)
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

        private static async Task<bool> Server_Initiation()
        {
            Server_GUI.Main_Menu();
            string? input = Console.ReadLine();

            if (input == "S")
            {
                await Start_Or_Stop_Server();
                await Server_Initiation();
            }
            else if (input == "O")
            {
                await Options_Menu();
            }
            else if (input == "E")
            {
                Environment.Exit(0);
            }
            else
            {
                await Server_Initiation();
            }

            return true;
        }



        private static async Task<bool> Start_Or_Stop_Server()
        {
            if (On_Off == 0)
            {
                On_Off = 1;

                /*
                bool result = await Load_Certificate();

                if(result == true)
                {
                    await Start_Main_Loop();
                    await Server_Operation();
                }
                else
                {

                }
                */

                await Load_Certificate();
                await Start_Main_Loop();
                await Server_Operation();
            }
            else
            {
                On_Off = 0;

                await Unload_Certificate();
                await Stop_Main_Loop();

                server_socket?.Dispose();
            }

            return true;
        }


        private static async Task<bool> Options_Menu()
        {
            Server_GUI.Settings_Menu();

            string input = Console.ReadLine();



            if(input == "P")
            {
                
            }
            else if (input == "G")
            {

            }
            else if (input == "M")
            {

            }
            else if (input == "E")
            {
                await Server_Initiation();
            }
            else
            {
                await Options_Menu();
            }

            return true;
        }




        private static async void Server_main_loop_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(server_shutdown == false)
            {
                await Delete_Expired_Database_Items();
                await Thread_Pool_Regulator();
            }
        }




        private static Task<bool> Server_Operation()
        {
            System.Threading.Thread Server_Operation_Thread = new System.Threading.Thread(async () =>
            {
                server_socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                server_socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 1024));
                server_socket?.Listen(1000);

                while (On_Off == 1)
                {
                    if (server_socket != null)
                    {
                        try
                        {
                            System.Net.Sockets.Socket? client = await server_socket.AcceptAsync();

                            if (client != null)
                            {
                                await Operation_Selection(client);
                            }
                        }
                        catch(Exception E)
                        {
                            System.Diagnostics.Debug.WriteLine("Error: " + E.Message);
                            On_Off = 0;
                        }
                    }
                }
            });
            Server_Operation_Thread.Priority = System.Threading.ThreadPriority.Highest;
            Server_Operation_Thread.IsBackground = false;
            Server_Operation_Thread.Start();


            return Task.FromResult(true);
        }



        private static Task<bool> Start_Main_Loop()
        {
            if (server_main_loop != null)
            {
                server_main_loop = new System.Timers.Timer();
                server_main_loop.Elapsed += Server_main_loop_Elapsed;
                server_main_loop.Interval = 100;
                server_main_loop.Start();
            }

            return Task.FromResult(true);
        }

        private static Task<bool> Stop_Main_Loop()
        {
            if (server_main_loop != null)
            {
                server_main_loop.Elapsed -= Server_main_loop_Elapsed;
                server_main_loop.Stop();
                server_main_loop.Dispose();
            }

            return Task.FromResult(true);
        }

        private static Task<bool> Port_Setup()
        {
            Server_GUI.Port_Setting_Menu();

            string input = Console.ReadLine();

            try
            {
                double converted_port_number = Convert.ToDouble(input);
                
            }
            catch
            {

            }

            return Task.FromResult(true);
        }
    }
}