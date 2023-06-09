using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class MySql_Connection_Initiator:Server_Variables
    {
        private static Student_Records_System_Functions student_functions = new Student_Records_System_Functions(new Student_Functions());
        private static Student_Records_System_Functions courses_functions = new Student_Records_System_Functions(new Courses_Functions());
        private static Student_Records_System_Functions grades_functions = new Student_Records_System_Functions(new Grades_Functions());

        private static Authentification_Functions authentification_functions = new Authentification_Functions();



        public async Task<string> Initiate_MySql_Connection<Password__Or__Binary_Content>(string email__or__log_in_session_key, Password__Or__Binary_Content password__or__binary_content, string function)
        {
            string function_result = String.Empty;

            MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection("Server=localhost;UID=" + MySql_Username + ";Password=" + MySql_Password + ";Database=university_records_system");

            try
            {
                await connection.OpenAsync();

                
                switch(function)
                {
                    case "Log In":
                        function_result = await authentification_functions.Authentificate_User(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Register":
                        function_result = await authentification_functions.Register_User(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Account validation":
                        function_result = await authentification_functions.Account_Validation(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Account log in":
                        function_result = await authentification_functions.Log_In_Account(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Account log out":
                        function_result = await authentification_functions.Log_Out_Account(email__or__log_in_session_key, connection);
                        break;

                    case "Log in session key validation":
                        function_result = await authentification_functions.Log_In_Session_Key_Validation(email__or__log_in_session_key, connection);
                        break;

                    case "Insert student":
                        function_result = await student_functions.Insert_Value_In_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Delete student":
                        function_result = await student_functions.Delete_Value_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Select students":
                        function_result = await student_functions.Select_Values_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Update student data":
                        function_result = await student_functions.Modify_Entity_Data_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Insert course":
                        function_result = await courses_functions.Insert_Value_In_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Delete course":
                        function_result = await courses_functions.Delete_Value_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Select courses":
                        function_result = await courses_functions.Select_Values_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Update course data":
                        function_result = await courses_functions.Modify_Entity_Data_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Insert grade":
                        function_result = await grades_functions.Insert_Value_In_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Delete grade":
                        function_result = await grades_functions.Delete_Value_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Select grades":
                        function_result = await grades_functions.Select_Values_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
                        break;

                    case "Update grade data":
                        function_result = await grades_functions.Modify_Entity_Data_From_MySql_Database(email__or__log_in_session_key, password__or__binary_content as string, connection);
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

            return function_result;
        }
    }
}
