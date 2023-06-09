using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Student_Records_System_Functions
    {
        // CLASS THAT IS THE IMPLEMENTOR CLASS OF THE STARTEGY DESIGN PATTERN IMPLEMENTATION
        // THAT IS RESPONSIBLE FOR GRADE, COURSES, AND STUDENT RELATED OPERATIONS WITHIN
        // THE STUDENT RECORDS SYSTEM


        private Functionality_Operators functions;

        public Student_Records_System_Functions(Functionality_Operators functionality)
        {
            functions = functionality;
        }


        public async Task<string> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return await functions.Delete_Value_From_MySql_Database(log_in_session_key, value, connection);
        }

        public async Task<string> Insert_Value_In_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return await functions.Insert_Value_In_MySql_Database(log_in_session_key, value, connection);
        }

        public async Task<string> Select_Values_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return await functions.Select_Values_From_MySql_Database(log_in_session_key, value, connection);
        }

        public async Task<string> Modify_Entity_Data_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return await functions.Modify_Entity_Data(log_in_session_key, value, connection);
        }
    }
}
