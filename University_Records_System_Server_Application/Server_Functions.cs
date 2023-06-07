using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Functions:Client_Connections
    {
        protected static Task<bool> Thread_Pool_Regulator()
        {
            int Worker_Threads = 0;
            int Port_Threads = 0;

            System.Threading.ThreadPool.GetAvailableThreads(out Worker_Threads, out Port_Threads);


            if (Worker_Threads < 1000)
            {
                System.Threading.ThreadPool.SetMaxThreads(Worker_Threads + (1000 - Worker_Threads), Port_Threads);
            }
            else if (Worker_Threads > 1000)
            {
                System.Threading.ThreadPool.SetMaxThreads(Worker_Threads - (Worker_Threads - 1000), Port_Threads);
            }



            if (Port_Threads < 1000)
            {
                System.Threading.ThreadPool.SetMaxThreads(Worker_Threads, Port_Threads + (1000 - Port_Threads));
            }
            else if (Port_Threads > 1000)
            {
                System.Threading.ThreadPool.SetMaxThreads(Worker_Threads, Port_Threads - (Port_Threads - 1000));
            }

            return Task.FromResult(true);
        }


        protected static Task<bool> Verify_SMTPS_Credentials(string SMTPS_Email_Address, string SMTPS_Email_Password)
        {
            bool Are_SMTPS_Credentials_Valid = false;


            MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                string service_provider = String.Empty;


                switch (SMTPS_Server_Service_Provider)
                {
                    case SMTPS_Provider.Google:
                        service_provider = "smtp.gmail.com";
                        break;

                    case SMTPS_Provider.Microsoft:
                        service_provider = "smtp.office365.com";
                        break;
                }


                client.Connect(service_provider, 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(SMTPS_Email_Address, SMTPS_Email_Password);
                client.Disconnect(true);

                Are_SMTPS_Credentials_Valid = true;
            }
            catch
            {
                Are_SMTPS_Credentials_Valid = false;
            }
            finally
            {
                if(client != null)
                {
                    client.Dispose();
                }
            }


            return Task.FromResult(Are_SMTPS_Credentials_Valid);
        }

        protected static async Task<string> SMTPS_Service(string random_key, string receipient_email_address, string function)
        {
            string SMTPS_Session_Result = String.Empty;

            string service_provider = String.Empty;

            MimeKit.MimeMessage message = new MimeKit.MimeMessage();

            


            try
            {
                StringBuilder message_body_builder = new StringBuilder();
                message_body_builder.Append("Your one time ");
                message_body_builder.Append(function);
                message_body_builder.Append(" code: ");
                message_body_builder.Append(random_key);


                message.From.Add(new MimeKit.MailboxAddress("Student Records System", SMTPS_Server_Email_Address));
                message.To.Add(new MimeKit.MailboxAddress("User", receipient_email_address));
                message.Subject = function.ToUpper() + " CODE";
                message.Body = new MimeKit.TextPart("plain") {Text = message_body_builder.ToString()};

                message_body_builder.Clear();

                MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    switch(SMTPS_Server_Service_Provider)
                    {
                        case SMTPS_Provider.Google:
                            service_provider = "smtp.gmail.com";
                            break;

                        case SMTPS_Provider.Microsoft:
                            service_provider = "smtp.office365.com";
                            break;

                    }
                    client.Connect(service_provider, 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(SMTPS_Server_Email_Address, SMTPS_Server_Email_Password);
                    client.Send(message);
                    client.Disconnect(true);

                    SMTPS_Session_Result = "SMTPS session successful";
                }
                catch(Exception E)
                {
                    await Server_Error_Logs(E, "SMTPS_Service");
                    SMTPS_Session_Result = "SMTPS client socket connection failed";

                    if (client != null)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
                finally
                {
                    if (client != null)
                    {
                        client.Dispose();
                    }
                }
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "SMTPS_Service");
                SMTPS_Session_Result = "Message formatting failed";
            }
            finally
            {
                if(message != null)
                {
                    message.Dispose();
                }
            }

            return SMTPS_Session_Result;
        }


        protected static async Task<bool> MySql_Connection_Validation()
        {
            bool Is_MySql_Connection_Successful = false;

            MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection("Server=localhost;UID=" + MySql_Username + ";Password=" + MySql_Password + ";Database=university_records_system");

            try
            {
                await connection.OpenAsync();
                Is_MySql_Connection_Successful = true;
            }
            catch
            {

            }
            finally
            {
                if(connection != null)
                {
                    await connection.DisposeAsync();
                }
            }

            return Is_MySql_Connection_Successful;
        }


        protected static async Task<bool> Delete_Expired_Database_Items()
        {


            MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection("Server=localhost;UID=" + MySql_Username + ";Password=" + MySql_Password + ";Database=university_records_system");


            try
            {
                await connection.OpenAsync();


                MySqlConnector.MySqlCommand delete_expired_log_in_session_keys_command = new MySqlConnector.MySqlCommand("DELETE FROM log_in_session_keys WHERE Expiration_Date <= NOW();", connection);

                try
                {
                    await delete_expired_log_in_session_keys_command.ExecuteNonQueryAsync();
                }
                catch (Exception E)
                {
                    await Server_Error_Logs(E, "Delete_Expired_Database_Items");
                }
                finally
                {
                    if(delete_expired_log_in_session_keys_command != null)
                    {
                        await delete_expired_log_in_session_keys_command.DisposeAsync();
                    }
                }




                MySqlConnector.MySqlCommand delete_expired_pending_log_in_sessions = new MySqlConnector.MySqlCommand("DELETE FROM pending_log_in_sessions WHERE Expiration_Date <= NOW();", connection);

                try
                {
                    await delete_expired_pending_log_in_sessions.ExecuteNonQueryAsync();
                }
                catch (Exception E)
                {
                    await Server_Error_Logs(E, "Delete_Expired_Database_Items");
                }
                finally
                {
                    if (delete_expired_pending_log_in_sessions != null)
                    {
                        await delete_expired_pending_log_in_sessions.DisposeAsync();
                    }
                }






                List<string> expired_accounts_list = new List<string>();



                MySqlConnector.MySqlCommand load_expired_accounts_pending_for_validation = new MySqlConnector.MySqlCommand("SELECT USER_ID FROM pending_account_validation WHERE Expiration_Date <= NOW();", connection);

                try
                {
                    MySqlConnector.MySqlDataReader load_expired_accounts_pending_for_validation_reader = load_expired_accounts_pending_for_validation.ExecuteReader();

                    try
                    {
                        while(await load_expired_accounts_pending_for_validation_reader.ReadAsync() == true)
                        {
                            expired_accounts_list.Add((string)load_expired_accounts_pending_for_validation_reader["USER_ID"]);
                        }
                    }
                    catch(Exception E)
                    {
                        await Server_Error_Logs(E, "Delete_Expired_Database_Items");

                        if (load_expired_accounts_pending_for_validation_reader != null)
                        {
                            await load_expired_accounts_pending_for_validation_reader.CloseAsync();
                        }
                    }
                    finally
                    {
                        if(load_expired_accounts_pending_for_validation_reader != null)
                        {
                            await load_expired_accounts_pending_for_validation_reader.CloseAsync();
                            await load_expired_accounts_pending_for_validation_reader.DisposeAsync();
                        }
                    }
                }
                catch (Exception E)
                {
                    await Server_Error_Logs(E, "Delete_Expired_Database_Items");
                }
                finally
                {
                    if (load_expired_accounts_pending_for_validation != null)
                    {
                        await load_expired_accounts_pending_for_validation.DisposeAsync();
                    }
                }





                for(int index = 0; index< expired_accounts_list.Count(); index++)
                {
                    MySqlConnector.MySqlCommand delete_expired_accounts_pending_for_validation = new MySqlConnector.MySqlCommand("DELETE FROM user_credentials WHERE USER_ID = @email;", connection);

                    delete_expired_accounts_pending_for_validation.Parameters.AddWithValue("email", expired_accounts_list[index]);

                    try
                    {
                        await delete_expired_accounts_pending_for_validation.ExecuteNonQueryAsync();
                    }
                    catch (Exception E)
                    {
                        await Server_Error_Logs(E, "Delete_Expired_Database_Items");
                    }
                    finally
                    {
                        if (delete_expired_accounts_pending_for_validation != null)
                        {
                            await delete_expired_accounts_pending_for_validation.DisposeAsync();
                        }
                    }
                }
                

            }
            catch(Exception E)
            {
                await Server_Error_Logs(E, "Delete_Expired_Database_Items");
            }
            finally
            {
                if (connection != null)
                {
                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
            }


            return true;
        }

        
        protected static async Task<bool> Delete_Expired_Log_In_Session_Keys_Accounts()
        {
            return true;
        }

        protected static async Task<bool> Delete_Expired_Pending_Log_In_Sessions_Accounts()
        {
            return true;
        }


        protected static async Task<bool> Delete_Expired_Accounts_Pending_For_Validation_Accounts()
        {
            return true;
        }

    }
}
