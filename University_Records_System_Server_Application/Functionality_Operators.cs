using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal interface Functionality_Operators
    {
        internal Task<bool> Insert_Value_In_MySql_Database(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);

        internal Task<bool> Select_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);

        internal Task<bool> Select_Values_From_MySql_Database(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);

        internal Task<bool> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);

        internal Task<bool> Modify_Entity_Data(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);
    }
}
