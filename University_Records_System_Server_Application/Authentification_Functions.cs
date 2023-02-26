using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Authentification_Functions
    {

        private sealed class Server_Functions_Mitigator:Server_Functions
        {
            internal static async Task<string> SMTPS_Service_Initiator(string random_key, string receipient_email_address, string function)
            {
                return await SMTPS_Service(random_key, receipient_email_address, function);
            }
        }


        private sealed class Server_Cryptographic_Functions_Mitigator:Server_Cryptographic_Functions
        {
            internal static async Task<string> Create_Random_Key_Initiator()
            {
                return await Create_Random_Key();
            }


            internal static async Task<byte[]> Content_Hasher_Initiator(string content)
            {
                return await Content_Hasher(content);
            }
        }

        protected static async Task<string> Authentificate_User(string email, string password, MySqlConnector.MySqlConnection connection)
        {
            string authentification_result = "Connection error";


            MySqlConnector.MySqlCommand password_extraction_command = new MySqlConnector.MySqlCommand("SELECT USER_PASSWORD FROM user_credentials WHERE USER_ID = @email;", connection);

            try
            {
                password_extraction_command.Parameters.AddWithValue("email", email);

                MySqlConnector.MySqlDataReader password_extraction_command_reader = await password_extraction_command.ExecuteReaderAsync();

                try
                {
                    if(await password_extraction_command_reader.ReadAsync() == true)
                    {

                        byte[] user_password = (byte[])password_extraction_command_reader["USER_PASSWORD"];


                        if(Encoding.UTF8.GetString(user_password) == Encoding.UTF8.GetString(await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(password)))
                        {


                            await password_extraction_command_reader.CloseAsync();



                            MySqlConnector.MySqlCommand account_validation_checkup_command = new MySqlConnector.MySqlCommand("SELECT USER_ID FROM user_credentials WHERE USER_ID = @email AND account_validated = TRUE;", connection);

                            try
                            {
                                account_validation_checkup_command.Parameters.AddWithValue("email", email);

                                MySqlConnector.MySqlDataReader account_validation_checkup_command_reader = await account_validation_checkup_command.ExecuteReaderAsync();


                                try
                                {
                                    if(await account_validation_checkup_command_reader.ReadAsync() == true)
                                    {
                                        
                                    }
                                    else
                                    {
                                        authentification_result = "Account not validated";
                                    }
                                }
                                catch
                                {
                                    if (account_validation_checkup_command_reader != null)
                                    {
                                        await account_validation_checkup_command_reader.CloseAsync();
                                    }
                                }
                                finally
                                {
                                    if (account_validation_checkup_command_reader != null)
                                    {
                                        await account_validation_checkup_command_reader.CloseAsync();
                                        await account_validation_checkup_command_reader.DisposeAsync();
                                    }
                                }
                            }
                            catch
                            {

                            }
                            finally
                            {
                                if(account_validation_checkup_command != null)
                                {
                                    await account_validation_checkup_command.DisposeAsync();
                                }
                            }
                        }
                        else
                        {
                            authentification_result = "Wrong password";
                        }
                    }
                    else
                    {
                        authentification_result = "Wrong email address";
                    }
                }
                catch
                {
                    if (password_extraction_command_reader != null)
                    {
                        await password_extraction_command_reader.CloseAsync();
                    }
                }
                finally
                {
                    if(password_extraction_command_reader != null)
                    {
                        await password_extraction_command_reader.CloseAsync();
                        await password_extraction_command_reader.DisposeAsync();
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if(password_extraction_command != null)
                {
                    await password_extraction_command.DisposeAsync();
                }
            }

            return authentification_result;
        }



        protected static async Task<string> Register_User(string email, string password, MySqlConnector.MySqlConnection connection)
        {
            string registration_result = "Connection error";


            MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand("SELECT USER_PASSWORD FROM user_credentials WHERE USER_ID = @email;", connection);

            try
            {
                command.Parameters.AddWithValue("email", email);

                MySqlConnector.MySqlDataReader reader = await command.ExecuteReaderAsync();

                try
                {

                    if (await reader.ReadAsync() == false)
                    {

                        MySqlConnector.MySqlCommand query_command = new MySqlConnector.MySqlCommand("INSERT INTO user_credentials VALUES(@email, @validated, @password);", connection);

                        try
                        {

                            try
                            {
                                System.Net.Mail.MailAddress received_email = new System.Net.Mail.MailAddress(email);
                            }
                            catch
                            {
                                registration_result = "Invalid email address";
                            }

                            if(registration_result != "Invalid email address")
                            {
                                if (password.Length >= 6)
                                {
                                    Random_Key_Generation:
                                    string random_key = await Server_Cryptographic_Functions_Mitigator.Create_Random_Key_Initiator();


                                    //  !!!!!!!!!! TO DO !!!!!!!!!!!!
                                    //
                                    //  CHECK IF KEY EXIST
                                    //
                                    //  IF EXIST:
                                    //
                                    //     goto Random_Key_Generation;
                                    //


                                    string smtps_result = await Server_Functions_Mitigator.SMTPS_Service_Initiator(random_key, email, "register");


                                    if (smtps_result == "SMTPS session successful")
                                    {

                                        byte[] hashed_password = await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(password);


                                        query_command.Parameters.AddWithValue("email", email);
                                        query_command.Parameters.AddWithValue("validated", false);
                                        query_command.Parameters.AddWithValue("password", hashed_password);

                                        registration_result = "Registered";

                                        await reader.CloseAsync();

                                        await query_command.ExecuteNonQueryAsync();

                                        string current_date_time_string = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");



                                        MySqlConnector.MySqlCommand move_account_to_validation_queue = new MySqlConnector.MySqlCommand("INSERT INTO pending_account_validation VALUES(@one_time_account_validation_code, @email, @expiration_date)", connection);

                                        try
                                        {
                                            move_account_to_validation_queue.Parameters.AddWithValue("one_time_account_validation_code", random_key);
                                            move_account_to_validation_queue.Parameters.AddWithValue("email", email);
                                            move_account_to_validation_queue.Parameters.AddWithValue("expiration_date", current_date_time_string);
                                            await move_account_to_validation_queue.ExecuteNonQueryAsync();
                                        }
                                        catch
                                        {

                                        }
                                        finally
                                        {
                                            if(move_account_to_validation_queue != null)
                                            {
                                                await move_account_to_validation_queue.DisposeAsync();
                                            }
                                        }

                                    }
                                    else
                                    {
                                        registration_result = "Email server connection error";
                                    }
                                }
                                else
                                {
                                    registration_result = "Password does not contain the amount of characters required";
                                }
                            }
                        }
                        catch
                        {
                            
                        }
                        finally
                        {
                            if (query_command != null)
                            {
                                await query_command.DisposeAsync();
                            }
                        }
                    }
                    else
                    {
                        registration_result = "Account already exist";
                    }

                }
                catch
                {
                    

                    //registration_result = "Reader failed";
                }
                finally
                {
                    if (reader != null)
                    {
                        await reader.CloseAsync();
                        await reader.DisposeAsync();
                    }
                }

            }
            catch
            {
                
                //registration_result = "Command execution error";
            }
            finally
            {
                if (command != null)
                {
                    await command.DisposeAsync();
                }
            }

            return registration_result;
        }
    }
}
