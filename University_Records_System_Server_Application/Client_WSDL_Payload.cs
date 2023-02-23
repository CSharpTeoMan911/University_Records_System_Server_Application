using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    public class Client_WSDL_Payload
    {
        public string email__or__log_in_session_key;
        public string password__or__binary_content;
        public string function;
    }
}
