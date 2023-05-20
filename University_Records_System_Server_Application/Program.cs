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
            _= Startup().Result;
        }

        private async static Task<bool> Startup()
        {
            await Settings_File_Controller(Settings_File_Options.Load_Settings_From_File);
            await Server_Initiation();
            return true;
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

            await X509_Server_Certificate_Operational_Controller(X509_Server_Certificate_Operations.Unload_Certificate);

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
                await Server_Initiation();
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

                await Settings_File_Controller(Settings_File_Options.Load_Settings_From_File);

                if (await X509_Server_Certificate_Operational_Controller(X509_Server_Certificate_Operations.Load_Certificate) == true)
                {
                    if(await MySql_Connection_Validation() == true)
                    {
                        if(await Verify_SMTPS_Credentials(SMTPS_Server_Email_Address, SMTPS_Server_Email_Password) == true)
                        {
                            await Start_Main_Loop();
                            await Server_Operation();
                        }
                        else
                        {
                            Server_GUI.SMTPS_Service_Setup_Unsuccessful();
                            Console.ReadLine();

                            On_Off = 0;

                            await SMTPS_Credentials_Setup_Processing();
                        }
                    }
                    else
                    {
                        Server_GUI.MySql_Credentials_Setup_Error();
                        Console.ReadLine();

                        On_Off = 0;

                        await MySQL_Credentials_Setup_Result_Processing();
                    }

                }
                else
                {
                    Server_GUI.Certificate_Loadup_Error();
                    Console.ReadLine();

                    On_Off = 0;

                    await Certificate_Generation_Result_Processing();
                }
            }
            else
            {
                On_Off = 0;

                await X509_Server_Certificate_Operational_Controller(X509_Server_Certificate_Operations.Unload_Certificate);
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
                await Port_Setup_Result_Processing();
            }
            else if (input == "G")
            {
                await Certificate_Generation_Result_Processing();
            }
            else if (input == "M")
            {
                await MySQL_Credentials_Setup_Result_Processing();
            }
            else if (input == "S")
            {
                await SMTPS_Credentials_Setup_Processing();
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


        private static async Task<bool> Port_Setup_Result_Processing()
        {
            if (await Port_Setup_Menu() == false)
            {
                Server_GUI.Port_Setup_Unsuccsessful();
                Console.ReadLine();
            }
            else
            {
                Server_GUI.Port_Setup_Succsessful();
                Console.ReadLine();
            }

            return true;
        }

        private static async Task<bool> Certificate_Generation_Result_Processing()
        {
            if (await Certificate_Generation_Menu() == false)
            {
                Server_GUI.Certificate_Generation_Unsuccessful();
                Console.ReadLine();
            }
            else
            {
                Server_GUI.Certificate_Generation_Successful();
                Console.ReadLine();
            }

            return true;
        }


        private static async Task<bool> MySQL_Credentials_Setup_Result_Processing()
        {
            if (await MySQL_Credentials_Setup_Menu() == false)
            {
                Server_GUI.MySql_Credentials_Setup_Error();
                Console.ReadLine();
            }
            else
            {
                Server_GUI.MySql_Credentials_Setup_Successful();
                Console.ReadLine();
            }

            return true;
        }


        private static async Task<bool> SMTPS_Credentials_Setup_Processing()
        {
            if (await SMTPS_Credentials_Setup_Menu() == false)
            {
                Server_GUI.SMTPS_Service_Setup_Unsuccessful();
                Console.ReadLine();
            }
            else
            {
                Server_GUI.SMTPS_Service_Setup_Successful();
                Console.ReadLine();
            }

            return true;
        }






        private static async Task<bool> Port_Setup_Menu()
        {
            bool Is_Port_Settup_Successful = false;

            Server_GUI.Port_Setting_Menu();

            string input = Console.ReadLine();

            if(input != "E")
            {
                try
                {
                    double converted_port_number = Convert.ToDouble(input);
                    port_number = (int)converted_port_number;
                    await Settings_File_Controller(Settings_File_Options.Update_Settings_File);
                    Is_Port_Settup_Successful = true;
                }
                catch
                {
                    await Port_Setup_Menu();
                }
            }

            return Is_Port_Settup_Successful;
        }






        private static async Task<bool> SMTPS_Credentials_Setup_Menu()
        {
            bool Is_SMTPS_Settup_Successful = false;




            Server_GUI.SMTPS_Service_Provider_Menu();

            string smtps_service_provider_input = Console.ReadLine();




            if(smtps_service_provider_input != "E")
            {
                if(smtps_service_provider_input == "G" || smtps_service_provider_input == "M")
                {



                    Server_GUI.SMTPS_Email_Menu();

                    string smtps_service_provider_email_input = Console.ReadLine();




                    if(smtps_service_provider_email_input != "E")
                    {


                        Server_GUI.SMTPS_Password_Menu();

                        string smtps_service_provider_password_input = Console.ReadLine();




                        if(smtps_service_provider_password_input != "E")
                        {
                            if(smtps_service_provider_input != null)
                            {
                                if(smtps_service_provider_email_input != null)
                                {
                                    if(smtps_service_provider_password_input != null)
                                    {
                                        if (smtps_service_provider_input == "G")
                                        {
                                            SMTPS_Server_Service_Provider = SMTPS_Provider.Google;
                                        }
                                        else if (smtps_service_provider_input == "M")
                                        {
                                            SMTPS_Server_Service_Provider = SMTPS_Provider.Microsoft;
                                        }



                                        try
                                        {
                                            System.Net.Mail.MailAddress valid_email = new System.Net.Mail.MailAddress(smtps_service_provider_email_input);

                                            if(await Verify_SMTPS_Credentials(smtps_service_provider_email_input, smtps_service_provider_password_input) == true)
                                            {
                                                SMTPS_Server_Email_Address = smtps_service_provider_email_input;

                                                SMTPS_Server_Email_Password = smtps_service_provider_password_input;

                                                Is_SMTPS_Settup_Successful =  await Settings_File_Controller(Settings_File_Options.Update_Settings_File);
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    await SMTPS_Credentials_Setup_Menu();
                }
            }



            return Is_SMTPS_Settup_Successful;
        }





        private static async Task<bool> MySQL_Credentials_Setup_Menu()
        {
            bool Is_MySQL_Account_Setup_Successful = false;

            Server_GUI.MySql_Username_Menu();

            string mysql_username_input = Console.ReadLine();

            if(mysql_username_input != "E")
            {
                Server_GUI.MySql_Password_Menu();

                string mysql_password_input = Console.ReadLine();

                if(mysql_username_input != "E")
                {
                    string mysql_username_buffer = MySql_Username;
                    string mysql_password_buffer = MySql_Password;

                    MySql_Username = mysql_username_input;
                    MySql_Password = mysql_password_input;


                    if(await MySql_Connection_Validation() == true)
                    {
                        await Settings_File_Controller(Settings_File_Options.Update_Settings_File);
                        Is_MySQL_Account_Setup_Successful = true;
                    }
                    else
                    {
                        MySql_Username = mysql_username_buffer;
                        MySql_Password = mysql_password_buffer;
                    }
                }
            }

            return Is_MySQL_Account_Setup_Successful;
        }




        private static async Task<bool> Certificate_Generation_Menu()
        {
            bool Is_Certificate_Generation_Successful = false;

            Server_GUI.Certificate_Generation_Password_Menu();

            string password_input = Console.ReadLine();

            if (password_input != "E")
            {

            Certificate_Generation_Expiry_Date_Setup:

                Server_GUI.Certificate_Generation_Expiry_Date_Menu();

                string expiry_date_input = Console.ReadLine();

                if (expiry_date_input != "E")
                {
                    try
                    {
                        double converted_expiry_date_input = Convert.ToDouble(expiry_date_input);

                        certificate_password = password_input;

                        Is_Certificate_Generation_Successful = await Settings_File_Controller(Settings_File_Options.Update_Settings_File);

                        Is_Certificate_Generation_Successful = await X509_Server_Certificate_Operational_Controller(X509_Server_Certificate_Operations.Create_X509_Server_Certificate, (int)converted_expiry_date_input);
                    }
                    catch
                    {
                        goto Certificate_Generation_Expiry_Date_Setup;
                    }
                }
            }

            return Is_Certificate_Generation_Successful;
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
                server_socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, port_number));
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
                        catch
                        {
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

    }
}