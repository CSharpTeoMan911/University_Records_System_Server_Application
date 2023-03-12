using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Logs_Writer
    {
        //////////////////////////////////
        /////////// SERVER LOGS //////////
        //////////////////////////////////


        protected static async Task<bool> Server_Error_Logs(Exception E, string funtion)
        {
            bool successful = false;


            if (System.IO.File.Exists("logs.txt") == false)
            {
                System.IO.StreamWriter server_logs_writer = System.IO.File.CreateText("logs.txt");

                try
                {
                    await server_logs_writer.WriteLineAsync("\n\n\t\t\t///////////////////////////////////" + "\n\t\t\t/////////// SERVER LOGS ///////////" + "\n\t\t\t///////////////////////////////////");
                    await server_logs_writer.WriteLineAsync("\n\n\n\n\n\n\n\n Date: " + DateTime.Now + " \n\n HResult: " + E.HResult + "\n\n Exception: " + E + "\n\n Error Message: " + E.Message + "\n\n Method that caused the error: " + funtion);
                    await server_logs_writer.FlushAsync();

                    successful = true;
                }
                catch
                {
                    if (server_logs_writer != null)
                    {
                        server_logs_writer.Close();
                    }
                }
                finally
                {
                    if (server_logs_writer != null)
                    {
                        await server_logs_writer.FlushAsync();
                        server_logs_writer.Close();
                        server_logs_writer.Dispose();
                    }
                }
            }
            else
            {
                System.IO.StreamWriter server_logs_writer = System.IO.File.AppendText("logs.txt");

                try
                {
                    await server_logs_writer.WriteLineAsync("\n\n\n\n\n\n\n\n Date: " + DateTime.Now + " \n\n HResult: " + E.HResult + "\n\n Exception: " + E + "\n\n Error Message: " + E.Message + "\n\n Method that caused the error: " + funtion);
                    await server_logs_writer.FlushAsync();

                    successful = true;
                }
                catch
                {
                    if (server_logs_writer != null)
                    {
                        server_logs_writer.Close();
                    }
                }
                finally
                {
                    if (server_logs_writer != null)
                    {
                        await server_logs_writer.FlushAsync();
                        server_logs_writer.Close();
                        server_logs_writer.Dispose();
                    }
                }
            }


            return successful;
        }
    }
}
