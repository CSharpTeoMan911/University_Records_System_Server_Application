using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Variables
    {
        protected static System.Security.Cryptography.X509Certificates.X509Certificate2 server_certificate;

        protected static short On_Off;


        protected static string MySql_Username = "stundent_records_server";
        protected static string MySql_Password = "stundent_records_server";


        protected static string SMTPS_Server_Email_Address = "student.records.system.smtps@gmail.com";
        protected static string SMTPS_Server_Email_Password = "hjtqpldmrvvcfdtm";

        protected static Task<bool> Load_Certificate_At_Startup()
        {
            string cert_name_segment = String.Empty;


            try
            {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    cert_name_segment = "\\student-records-system-certificate.pfx";
                }
                else
                {
                    cert_name_segment = "/student-records-system-certificate.pfx";
                }

                server_certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(Environment.CurrentDirectory + cert_name_segment);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }

        }

    }
}
