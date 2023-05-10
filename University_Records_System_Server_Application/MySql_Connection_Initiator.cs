using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class MySql_Connection_Initiator:Authentification_Functions
    {
        public async Task<Tuple<bool, string>> Initiate_MySql_Connection<Password__Or__Binary_Content>(string email__or__log_in_session_key, Password__Or__Binary_Content password__or__binary_content, string function)
        {
            bool is_binary_file = false;
            string function_result = String.Empty;

            MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection("Server=localhost;UID=" + MySql_Username + ";Password=" + MySql_Password + ";Database=university_records_system");

            try
            {
                await connection.OpenAsync();

                
                switch(function)
                {
                    case "Log In":
                        function_result = await Authentificate_User(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Register":
                        function_result = await Register_User(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Account validation":
                        function_result = await Account_Validation(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Account log in":
                        function_result = await Log_In_Account(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Account log out":
                        function_result = await Log_Out_Account(email__or__log_in_session_key, connection);
                        break;

                    case "Log in session key validation":
                        function_result = await Log_In_Session_Key_Validation(email__or__log_in_session_key, connection);
                        break;
                }
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Initiate_MySql_Connection");
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

            return new Tuple<bool, string>(is_binary_file, function_result);
        }
    }
}
