using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Course
    {
        public string course_ID { get; set; }
        public string course_Department { get; set; }
        public bool postgraduate { get; set; }
        public string location { get; set; }
        public int duration { get; set; }
    }
}
