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


        private sealed class Server_Logs_Writer_Mitigator:Server_Logs_Writer
        {
            internal async static Task<bool> Error_Logs(Exception E, string function)
            {
                return await Server_Error_Logs(E, function);
            }
        }



        protected static async Task<string> Log_In_Account(string email, string log_in_code, MySqlConnector.MySqlConnection connection)
        {
            string log_in_result = "Connection error";

            MySqlConnector.MySqlCommand select_log_in_code_command = new MySqlConnector.MySqlCommand("SELECT one_time_log_in_code FROM pending_log_in_sessions WHERE USER_ID = @email;", connection);

            try
            {

                select_log_in_code_command.Parameters.AddWithValue("email", email);


                MySqlConnector.MySqlDataReader select_log_in_code_command_reader = await select_log_in_code_command.ExecuteReaderAsync();

                try
                {
                    while(await select_log_in_code_command_reader.ReadAsync() == true)
                    {
                        if(Encoding.UTF8.GetString((byte[])select_log_in_code_command_reader["one_time_log_in_code"]) == Encoding.UTF8.GetString(await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(log_in_code)))
                        {


                            await select_log_in_code_command_reader.CloseAsync();

                            MySqlConnector.MySqlCommand delete_log_in_code_command = new MySqlConnector.MySqlCommand("DELETE FROM pending_log_in_sessions WHERE USER_ID = @email AND one_time_log_in_code = @code;", connection);

                            try
                            {
                                delete_log_in_code_command.Parameters.AddWithValue("email", email);
                                delete_log_in_code_command.Parameters.AddWithValue("code", log_in_code);

                                await delete_log_in_code_command.ExecuteNonQueryAsync();



                                string log_in_session_key = await Valid_Random_Key_Generator(connection, email);


                                MySqlConnector.MySqlCommand insert_log_in_session_key = new MySqlConnector.MySqlCommand("INSERT INTO log_in_session_keys VALUES(@email, @code, NOW() + INTERVAL 14 HOUR);", connection);

                                try
                                {
                                    insert_log_in_session_key.Parameters.AddWithValue("email", email);
                                    insert_log_in_session_key.Parameters.AddWithValue("code", await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(log_in_session_key));

                                    await insert_log_in_session_key.ExecuteNonQueryAsync();

                                    log_in_result = log_in_session_key;
                                }
                                catch(Exception E)
                                {
                                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Log_In_Account");
                                }
                                finally
                                {
                                    if(insert_log_in_session_key != null)
                                    {
                                        await insert_log_in_session_key.DisposeAsync();
                                    }
                                }

                            }
                            catch (Exception E)
                            {
                                Server_Logs_Writer_Mitigator.Error_Logs(E, "Log_In_Account");
                            }
                            finally
                            {
                                if(delete_log_in_code_command != null)
                                {
                                    await delete_log_in_code_command.DisposeAsync();
                                }
                            }

                        }
                        else
                        {
                            log_in_result = "Wrong log in code";
                        }
                    }
                }
                catch (Exception E)
                {
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Log_In_Account");

                    if (select_log_in_code_command_reader != null)
                    {
                        await select_log_in_code_command_reader.CloseAsync();
                    }
                }
                finally 
                {
                    if(select_log_in_code_command_reader != null)
                    {
                        await select_log_in_code_command_reader.CloseAsync();
                        await select_log_in_code_command_reader.DisposeAsync();
                    }
                }

            }
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Log_In_Account");
            }
            finally
            {
                if(select_log_in_code_command != null)
                {
                    await select_log_in_code_command.DisposeAsync();
                }
            }

            return log_in_result;
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

                        if(Encoding.UTF8.GetString((byte[])password_extraction_command_reader["USER_PASSWORD"]) == Encoding.UTF8.GetString(await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(password)))
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
                                        await account_validation_checkup_command_reader.CloseAsync();



                                        string random_key = await Valid_Random_Key_Generator(connection, email);

                                        string smtps_result = await Server_Functions_Mitigator.SMTPS_Service_Initiator(random_key, email, "log in");



                                        if (smtps_result == "SMTPS session successful")
                                        {
                                            
                                            MySqlConnector.MySqlCommand log_in_session_key_insertion_command = new MySqlConnector.MySqlCommand("INSERT INTO pending_log_in_sessions VALUES(@email, @one_time_log_in_code, NOW() + INTERVAL 5 MINUTE);", connection);

                                            try
                                            {
                                                log_in_session_key_insertion_command.Parameters.AddWithValue("email", email);
                                                log_in_session_key_insertion_command.Parameters.AddWithValue("one_time_log_in_code", await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(random_key));

                                                await log_in_session_key_insertion_command.ExecuteNonQueryAsync();

                                                authentification_result = "Log in successful";
                                            }
                                            catch(Exception E)
                                            {
                                                Server_Logs_Writer_Mitigator.Error_Logs(E, "Authentificate_User");
                                            }
                                            finally
                                            {
                                                if(log_in_session_key_insertion_command != null)
                                                {
                                                    await log_in_session_key_insertion_command.DisposeAsync();
                                                }
                                            }

                                        }
                                        else
                                        {
                                            authentification_result = "Email server connection error";
                                        }

                                    }
                                    else
                                    {
                                        authentification_result = "Account not validated";
                                    }
                                }
                                catch (Exception E)
                                {
                                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Authentificate_User");

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
                            catch(Exception E)
                            {
                                Server_Logs_Writer_Mitigator.Error_Logs(E, "Authentificate_User");
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
                            authentification_result = "Invalid password";
                        }
                    }
                    else
                    {
                        authentification_result = "Invalid email address";
                    }
                }
                catch (Exception E)
                {
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Authentificate_User");

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
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Authentificate_User");
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



        protected static async Task<string> Log_Out_Account(string log_in_session_key, MySqlConnector.MySqlConnection connection)
        {
            string log_out_result = "Connection error";

            MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand("DELETE FROM log_in_session_keys WHERE log_in_session_key = @log_in_session_key;", connection);

            try
            {
                command.Parameters.AddWithValue("log_in_session_key", await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(log_in_session_key));
                command.ExecuteNonQuery();

                log_out_result = "Logged out";
            }
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Log_Out_Account");
            }
            finally
            {
                if (command != null)
                {
                    await command.DisposeAsync();
                }
            }

            return log_out_result;
        }



        protected static async Task<string> Account_Validation(string email, string account_validation_key, MySqlConnector.MySqlConnection connection)
        {
            string account_validation_result = "Connection error";

            MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand("SELECT one_time_account_validation_code FROM pending_account_validation WHERE USER_ID = @email;", connection);

            try
            {
                command.Parameters.AddWithValue("email", email);

                MySqlConnector.MySqlDataReader reader = command.ExecuteReader();

                try
                {
                    if(await reader.ReadAsync() == true)
                    {
                        if(Encoding.UTF8.GetString((byte[])reader["one_time_account_validation_code"]) == Encoding.UTF8.GetString(await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(account_validation_key)))
                        {
                            await reader.CloseAsync();

                            MySqlConnector.MySqlCommand account_validation_command = new MySqlConnector.MySqlCommand("UPDATE user_credentials SET account_validated = TRUE WHERE USER_ID = @email;", connection);
                            account_validation_command.Parameters.AddWithValue("email", email);
                            await account_validation_command.ExecuteNonQueryAsync();

                            account_validation_result = "Account validation successful";
                        }
                        else
                        {
                            account_validation_result = "Account validation not successful";
                        }
                    }
                }
                catch (Exception E)
                {
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Account_Validation");

                    if (reader != null)
                    {
                        await reader.CloseAsync();
                    }
                }
                finally
                {
                    if(reader != null)
                    {
                        await reader.CloseAsync();
                        await reader.DisposeAsync();
                    }
                }
            }
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Account_Validation");
            }
            finally
            {
                if(command != null)
                {
                    await command.DisposeAsync();
                }
            }

            return account_validation_result;
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

                        await reader.CloseAsync();

                        MySqlConnector.MySqlCommand query_command = new MySqlConnector.MySqlCommand("INSERT INTO user_credentials VALUES(@email, @validated, @password);", connection);

                        try
                        {

                            try
                            {
                                System.Net.Mail.MailAddress received_email = new System.Net.Mail.MailAddress(email);
                            }
                            catch (Exception E)
                            {
                                Server_Logs_Writer_Mitigator.Error_Logs(E, "Register_User");

                                registration_result = "Invalid email address";
                            }

                            if(registration_result != "Invalid email address")
                            {
                                if (password.Length >= 6)
                                {
                               
                                    string random_key = await Valid_Random_Key_Generator(connection, email);


                                  
                                    string smtps_result = await Server_Functions_Mitigator.SMTPS_Service_Initiator(random_key, email, "register");


                                    if (smtps_result == "SMTPS session successful")
                                    {

                                        byte[] hashed_password = await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(password);


                                        query_command.Parameters.AddWithValue("email", email);
                                        query_command.Parameters.AddWithValue("validated", false);
                                        query_command.Parameters.AddWithValue("password", hashed_password);

                                        registration_result = "Registration successful";

                                        await query_command.ExecuteNonQueryAsync();


                                        MySqlConnector.MySqlCommand move_account_to_validation_queue = new MySqlConnector.MySqlCommand("INSERT INTO pending_account_validation VALUES(@email, @one_time_account_validation_code, NOW() + INTERVAL 2 HOUR);", connection);

                                        try
                                        {
                                            move_account_to_validation_queue.Parameters.AddWithValue("email", email);
                                            move_account_to_validation_queue.Parameters.AddWithValue("one_time_account_validation_code", await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(random_key));
                                            await move_account_to_validation_queue.ExecuteNonQueryAsync();
                                        }
                                        catch (Exception E)
                                        {
                                            Server_Logs_Writer_Mitigator.Error_Logs(E, "Register_User");
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
                        catch (Exception E)
                        {
                            Server_Logs_Writer_Mitigator.Error_Logs(E, "Register_User");
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
                catch (Exception E)
                {
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Register_User");
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
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Register_User");
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


        protected static async Task<string> Log_In_Session_Key_Validation(string log_in_session_key, MySqlConnector.MySqlConnection connection)
        {
            string Log_In_Session_Key_Validation_Result = "Connection error";

            MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand("SELECT log_in_session_key FROM log_in_session_keys WHERE log_in_session_key = @log_in_session_key;", connection);

            command.Parameters.AddWithValue("log_in_session_key", await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(log_in_session_key));

            try
            {
                MySqlConnector.MySqlDataReader reader = await command.ExecuteReaderAsync();

                try
                {
                    if(await reader.ReadAsync() == true)
                    {
                        Log_In_Session_Key_Validation_Result = "Log in session key validated";
                    }
                    else
                    {
                        Log_In_Session_Key_Validation_Result = "Invalid log in session key";
                    }
                }
                catch
                {
                    if (reader != null)
                    {
                        await reader.CloseAsync();
                    }
                }
                finally
                {
                    if(reader != null)
                    {
                        await reader.CloseAsync();
                        await reader.DisposeAsync();
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if(command != null)
                {
                    await command.DisposeAsync();
                }
            }

            return Log_In_Session_Key_Validation_Result;
        }


        protected static async Task<string> Valid_Random_Key_Generator(MySqlConnector.MySqlConnection connection, string email)
        {
        Valid_Random_Key_Generator:

            string random_key = await Server_Cryptographic_Functions_Mitigator.Create_Random_Key_Initiator();
            byte[] hashed_random_key = await Server_Cryptographic_Functions_Mitigator.Content_Hasher_Initiator(random_key);




            

            MySqlConnector.MySqlCommand verify_if_log_in_sessions_exists_command = new MySqlConnector.MySqlCommand("SELECT one_time_log_in_code FROM pending_log_in_sessions WHERE USER_ID = @email;", connection);

            try
            {




                verify_if_log_in_sessions_exists_command.Parameters.AddWithValue("email", email);

                MySqlConnector.MySqlDataReader verify_if_log_in_sessions_exists_command_reader = await verify_if_log_in_sessions_exists_command.ExecuteReaderAsync();

                try
                {
                    while (await verify_if_log_in_sessions_exists_command_reader.ReadAsync() == true)
                    {
                        if (Encoding.UTF8.GetString((byte[])verify_if_log_in_sessions_exists_command_reader["one_time_log_in_code"]) == Encoding.UTF8.GetString(hashed_random_key))
                        {
                            if (verify_if_log_in_sessions_exists_command_reader != null)
                            {
                                await verify_if_log_in_sessions_exists_command_reader.CloseAsync();
                                await verify_if_log_in_sessions_exists_command_reader.DisposeAsync();
                            }

                            goto Valid_Random_Key_Generator;
                        }
                    }
                }
                catch (Exception E)
                {
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Valid_Random_Key_Generator");

                    if (verify_if_log_in_sessions_exists_command_reader != null)
                    {
                        await verify_if_log_in_sessions_exists_command_reader.CloseAsync();
                    }
                }
                finally
                {
                    if (verify_if_log_in_sessions_exists_command_reader != null)
                    {
                        await verify_if_log_in_sessions_exists_command_reader.CloseAsync();
                        await verify_if_log_in_sessions_exists_command_reader.DisposeAsync();
                    }
                }

            }
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Valid_Random_Key_Generator");
            }
            finally
            {
                if (verify_if_log_in_sessions_exists_command != null)
                {
                    await verify_if_log_in_sessions_exists_command.DisposeAsync();
                }
            }




            MySqlConnector.MySqlCommand verify_if_log_in_key_exists_command = new MySqlConnector.MySqlCommand("SELECT log_in_session_key FROM log_in_session_keys WHERE USER_ID = @email;", connection);

            try
            {
                verify_if_log_in_key_exists_command.Parameters.AddWithValue("email", email);

                MySqlConnector.MySqlDataReader verify_if_log_in_key_exists_command_reader = await verify_if_log_in_key_exists_command.ExecuteReaderAsync();

                try
                {
                    while(await verify_if_log_in_key_exists_command_reader.ReadAsync() == true)
                    {
                        if (Encoding.UTF8.GetString((byte[])verify_if_log_in_key_exists_command_reader["log_in_session_key"]) == Encoding.UTF8.GetString(hashed_random_key))
                        {
                            if (verify_if_log_in_key_exists_command_reader != null)
                            {
                                await verify_if_log_in_key_exists_command_reader.CloseAsync();
                                await verify_if_log_in_key_exists_command_reader.DisposeAsync();
                            }

                            goto Valid_Random_Key_Generator;
                        }
                    }
                }
                catch(Exception E)
                {
                    Server_Logs_Writer_Mitigator.Error_Logs(E, "Valid_Random_Key_Generator");

                    if (verify_if_log_in_key_exists_command_reader != null)
                    {
                        await verify_if_log_in_key_exists_command_reader.CloseAsync();
                    }
                }
                finally 
                {
                    if(verify_if_log_in_key_exists_command_reader != null)
                    {
                        await verify_if_log_in_key_exists_command_reader.CloseAsync();
                        await verify_if_log_in_key_exists_command_reader.DisposeAsync();
                    }
                }

            }
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Valid_Random_Key_Generator");
            }
            finally
            {
                if(verify_if_log_in_key_exists_command != null)
                {
                    await verify_if_log_in_key_exists_command.DisposeAsync();
                }
            }


            return random_key;
        }



    }
}
