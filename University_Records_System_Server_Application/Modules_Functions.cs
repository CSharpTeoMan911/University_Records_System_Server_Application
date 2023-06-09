using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Modules_Functions:Functionality_Operators
    {
        public async Task<string> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return "";
        }

        public async Task<string> Insert_Value_In_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return "";
        }

        public async Task<string> Select_Values_From_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            return "";
        }

        public async Task<string> Modify_Entity_Data(string log_in_session_key, string value, MySqlConnection connection)
        {
            return "";
        }
        public async Task<string> Select_Values_By_Criteria_MySql_Database(string log_in_session_key, string value, MySqlConnection connection)
        {
            string value_deletion_result = "Value deletion failed";


            return value_deletion_result;
        }
    }
}
