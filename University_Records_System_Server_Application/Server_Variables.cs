using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Variables:Server_Logs_Writer
    {
        protected static System.Security.Cryptography.X509Certificates.X509Certificate2 server_certificate;

        protected static short On_Off;


        protected static string certificate_password = "k_e-i-y-91-11-80";


        protected static string MySql_Username = "stundent_records_server";
        protected static string MySql_Password = "stundent_records_server";


        protected static string SMTPS_Server_Email_Address = "student.records.system.smtps@gmail.com";
        protected static string SMTPS_Server_Email_Password = "hjtqpldmrvvcfdtm";




        public static Task<System.Security.Cryptography.X509Certificates.X509Certificate2> Get_Server_Certificate()
        {
            return Task.FromResult(server_certificate);
        }


        public static Task<bool> Set_Server_Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            server_certificate = certificate;
            return Task.FromResult(true);
        }


        public static Task<short> Get_If_Server_Is_On_Or_Off()
        {
            return Task.FromResult(On_Off);
        }











        protected static async Task<bool> Load_Certificate()
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

                server_certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(Environment.CurrentDirectory + cert_name_segment, certificate_password);

                return true;
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Load_Certificate_At_Startup");
                return false;
            }

        }


        protected static async Task<bool> Unload_Certificate()
        {
            try
            {
                server_certificate.Dispose();
                return true;
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Load_Certificate_At_Startup");
                return false;
            }

        }

    }
}
