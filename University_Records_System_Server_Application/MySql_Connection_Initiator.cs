using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class MySql_Connection_Initiator
    {
        private sealed class Server_Variables_Mitigator : Server_Variables
        {
            internal static Task<string> Get_MySql_Username()
            {
                return Task.FromResult(MySql_Username);
            }

            internal static Task<string> Get_MySql_Password()
            {
                return Task.FromResult(MySql_Password);
            }
        }


        private sealed class Authentification_Functions_Mitigator : Authentification_Functions
        {
            internal static async Task<string> Authentificate_User_Initiator(string email__or__log_in_session_key, string password__or__binary_content, MySqlConnector.MySqlConnection connection)
            {
                return await Authentificate_User(email__or__log_in_session_key, password__or__binary_content, connection);
            }

            internal static async Task<string> Register_User_Initiator(string email__or__log_in_session_key, string password__or__binary_content, MySqlConnector.MySqlConnection connection)
            {
                return await Register_User(email__or__log_in_session_key, password__or__binary_content, connection);
            }
        }


        protected static async Task<string> Initiate_MySql_Connection<Password__Or__Binary_Content>(string email__or__log_in_session_key, Password__Or__Binary_Content password__or__binary_content, string function)
        {
            string authentification_result = "Connection error";

            MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection("Server=localhost;UID=" + await Server_Variables_Mitigator.Get_MySql_Username() + ";Password=" + await Server_Variables_Mitigator.Get_MySql_Password() + ";Database=university_records_system");

            try
            {
                await connection.OpenAsync();
                
                switch(function)
                {
                    case "Log In":
                        authentification_result = await Authentification_Functions_Mitigator.Authentificate_User_Initiator(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Register":
                        authentification_result = await Authentification_Functions_Mitigator.Register_User_Initiator(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;
                }
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + E.ToString());


                if (connection != null)
                {
                    await connection.CloseAsync();
                }
            }
            finally
            {
                if (connection != null)
                {
                    await connection.CloseAsync();
                    connection.Dispose();
                }
            }

            return authentification_result;
        }
    }
}
