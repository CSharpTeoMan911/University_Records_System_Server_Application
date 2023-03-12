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


        protected static string certificate_password = "k_e-i-y-91-11-80";


        protected static string MySql_Username = "stundent_records_server";
        protected static string MySql_Password = "stundent_records_server";


        protected static string SMTPS_Server_Email_Address = "student.records.system.smtps@gmail.com";
        protected static string SMTPS_Server_Email_Password = "hjtqpldmrvvcfdtm";






        private sealed class Server_Logs_Writer_Mitigator : Server_Logs_Writer
        {
            internal async static Task<bool> Error_Logs(Exception E, string function)
            {
                return await Server_Error_Logs(E, function);
            }
        }





        protected static Task<bool> Load_Certificate()
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

                return Task.FromResult(true);
            }
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Load_Certificate_At_Startup");

                return Task.FromResult(false);
            }

        }


        protected static Task<bool> Unload_Certificate()
        {
            try
            {
                server_certificate.Dispose();

                return Task.FromResult(true);
            }
            catch (Exception E)
            {
                Server_Logs_Writer_Mitigator.Error_Logs(E, "Load_Certificate_At_Startup");

                return Task.FromResult(false);
            }

        }

    }
}
