using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Functions
    {
        private sealed class Server_Variables_Mitigator : Server_Variables
        {
            internal static Task<string> Get_SMTPS_Server_Email()
            {
                return Task.FromResult(SMTPS_Server_Email_Address);
            }

            internal static Task<string> Get_SMTPS_Server_Email_Password()
            {
                return Task.FromResult(SMTPS_Server_Email_Password);
            }
        }





        protected static async Task<string> SMTPS_Service(string random_key, string receipient_email_address, string function)
        {
            string SMTPS_Session_Result = String.Empty;

            MimeKit.MimeMessage message = new MimeKit.MimeMessage();

            


            try
            {
                message.From.Add(new MimeKit.MailboxAddress("Student Records System", await Server_Variables_Mitigator.Get_SMTPS_Server_Email()));
                message.To.Add(new MimeKit.MailboxAddress("User", receipient_email_address));
                message.Subject = "Log-in code";
                message.Body = new MimeKit.TextPart("plain") {Text = "Your one time " + function + " code: " + random_key};


                MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync(await Server_Variables_Mitigator.Get_SMTPS_Server_Email(), await Server_Variables_Mitigator.Get_SMTPS_Server_Email_Password());
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    SMTPS_Session_Result = "SMTPS session successful";
                }
                catch
                {
                    SMTPS_Session_Result = "SMTPS client socket connection failed";

                    if (client != null)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
                finally
                {
                    if (client != null)
                    {
                        client.Dispose();
                    }
                }
            }
            catch
            {
                SMTPS_Session_Result = "Message formatting failed";
            }
            finally
            {
                if(message != null)
                {
                    message.Dispose();
                }
            }

            return SMTPS_Session_Result;
        }
    }
}
