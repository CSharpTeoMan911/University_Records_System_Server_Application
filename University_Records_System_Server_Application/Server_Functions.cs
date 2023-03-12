﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Functions
    {

        private sealed class Server_Logs_Writer_Mitigator : Server_Logs_Writer
        {
            internal async static Task<bool> Error_Logs(Exception E, string function)
            {
                return await Server_Error_Logs(E, function);
            }
        }


        private sealed class Server_Variables_Mitigator : Server_Variables
        {
            internal static Task<string> Get_SMTPS_Server_Email()
            {
                return Task.FromResult(SMTPS_Server_Email_Address);
            }

            internal static Task<string> Get_SMTPS_Server_Email_Password()
            {
                return Task.FromResult(SMTPS_Server_Email_Password);
            }

            internal static Task<System.Security.Cryptography.X509Certificates.X509Certificate2> Get_Server_Certificate()
            {
                return Task.FromResult(server_certificate);
            }

            internal static Task<string> Get_MySql_Username()
            {
                return Task.FromResult(MySql_Username);
            }

            internal static Task<string> Get_MySql_Password()
            {
                return Task.FromResult(MySql_Password);
            }
        }





        protected static async Task<string> SMTPS_Service(string random_key, string receipient_email_address, string function)
        {
            string SMTPS_Session_Result = String.Empty;

            MimeKit.MimeMessage message = new MimeKit.MimeMessage();

            


            try
            {
                message.From.Add(new MimeKit.MailboxAddress("Student Records System", await Server_Variables_Mitigator.Get_SMTPS_Server_Email()));
                message.To.Add(new MimeKit.MailboxAddress("User", receipient_email_address));
                message.Subject = function.ToUpper() + " CODE";
                message.Body = new MimeKit.TextPart("plain") {Text = "Your one time " + function + " code: " + random_key};


                MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync(await Server_Variables_Mitigator.Get_SMTPS_Server_Email(), await Server_Variables_Mitigator.Get_SMTPS_Server_Email_Password());
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    SMTPS_Session_Result = "SMTPS session successful";
                }
                catch(Exception E)
                {
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "SMTPS_Service");

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
                Server_Logs_Writer_Mitigator.Error_Logs(E, "SMTPS_Service");

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




        protected static async Task<bool> Delete_Expired_Database_Items()
        {


            MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection("Server=localhost;UID=" + await Server_Variables_Mitigator.Get_MySql_Username() + ";Password=" + await Server_Variables_Mitigator.Get_MySql_Password() + ";Database=university_records_system");


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
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Delete_Expired_Database_Items");
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
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Delete_Expired_Database_Items");
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
                        Server_Logs_Writer_Mitigator.Error_Logs(E, "Delete_Expired_Database_Items");

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
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Delete_Expired_Database_Items");
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
                        Server_Logs_Writer_Mitigator.Error_Logs(E, "Delete_Expired_Database_Items");
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
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Delete_Expired_Database_Items");
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

        /*
        protected static async Task<bool> Delete_Expired_Log_In_Session_Keys_Accounts()
        {

        }

        protected static async Task<bool> Delete_Expired_Pending_Log_In_Sessions_Accounts()
        {

        }
        */

    }
}
