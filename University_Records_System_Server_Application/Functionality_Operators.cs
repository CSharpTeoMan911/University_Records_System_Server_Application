﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal interface Functionality_Operators
    {
        // INTERFACE METHODS USED BY THE GRADES, COURSES, AND STUDENT FUNCTIONS 
        internal Task<string> Insert_Value_In_MySql_Database(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);

        internal Task<string> Select_Values_From_MySql_Database(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);

        internal Task<string> Delete_Value_From_MySql_Database(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);

        internal Task<string> Modify_Entity_Data(string log_in_session_key, string value, MySqlConnector.MySqlConnection connection);
    }
}
