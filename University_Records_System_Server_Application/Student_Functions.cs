using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Student_Functions : Functionality_Operators
    {
        public async Task<bool> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return true;
        }

        public async Task<bool> Insert_Value_In_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return true;
        }

        public async Task<bool> Select_Values_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return true;
        }

        public async Task<bool> Select_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return true;
        }
        public async Task<bool> Modify_Entity_Data(string log_in_session_key, string value, MySqlConnection connection)
        {
            return true;
        }
    }
}
