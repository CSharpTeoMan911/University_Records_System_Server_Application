using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Authentification_Functions
    {

        protected static async Task<string> Authentificate_User(string email, string password, MySqlConnector.MySqlConnection connection)
        {
            string authentification_result = "Authentification failed";


            MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand("SELECT USER_PASSWORD FROM user_credentials WHERE USER_ID = @email;", connection);

            try
            {
                command.Parameters.AddWithValue("email", email);

                MySqlConnector.MySqlDataReader reader = await command.ExecuteReaderAsync();

                try
                {
                    if (await reader.ReadAsync() == true)
                    {
                        byte[] received_password = (byte[])reader["USER_PASSWORD"];


                        if (Encoding.UTF8.GetString(received_password) == password)
                        {
                            authentification_result = "Logged in";
                        }
                    }
                }
                catch
                {
                    if (reader != null)
                    {
                        await reader.CloseAsync();
                    }

                    authentification_result = "Reader failed";
                }
                finally
                {
                    if (reader != null)
                    {
                        await reader .CloseAsync();
                        reader.Dispose();
                    }
                }

            }
            catch
            {
                if (command != null)
                {
                    command.Cancel();
                }

                authentification_result = "Command execution error";
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }

            return authentification_result;
        }



        protected static async Task<string> Register_User(string email, string password, MySqlConnector.MySqlConnection connection)
        {
            string registration_result = "Registration failed";


            MySqlConnector.MySqlCommand command = new MySqlConnector.MySqlCommand("SELECT USER_PASSWORD FROM user_credentials WHERE USER_ID = @email;", connection);

            try
            {
                command.Parameters.AddWithValue("email", email);

                MySqlConnector.MySqlDataReader reader = await command.ExecuteReaderAsync();

                try
                {
                    
                    await reader.ReadAsync();

                    MySqlConnector.MySqlCommand query_command = new MySqlConnector.MySqlCommand("INSERT INTO user_credentials VALUES(@email, @password);", connection);

                    try
                    {
                        if (reader.FieldCount > 0)
                        {
                            query_command.Parameters.AddWithValue("email", email);
                            query_command.Parameters.AddWithValue("password", password);

                            registration_result = "Registered";

                            await reader.CloseAsync();

                            await query_command.ExecuteNonQueryAsync();
                        }
                    }
                    catch
                    {
                        if (query_command != null)
                        {
                            query_command.Cancel();
                        }
                    }
                    finally
                    {
                        if(query_command != null)
                        {
                            await query_command.DisposeAsync();
                        }
                    }

                }
                catch(Exception E)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + E.Message);

                    if (reader != null)
                    {
                        await reader.CloseAsync();
                    }

                    registration_result = "Reader failed";
                }
                finally
                {
                    if (reader != null)
                    {
                        await reader.CloseAsync();
                        reader.Dispose();
                    }
                }

            }
            catch
            {
                if (command != null)
                {
                    command.Cancel();
                }

                registration_result = "Command execution error";
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }

            return registration_result;
        }
    }
}
