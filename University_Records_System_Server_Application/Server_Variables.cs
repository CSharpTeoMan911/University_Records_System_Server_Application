﻿using Org.BouncyCastle.Crypto.Prng;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    class Server_Variables:Server_Logs_Writer
    {
        protected static System.Security.Cryptography.X509Certificates.X509Certificate2 server_certificate;

        protected static short On_Off;

        protected static int port_number = 1024;


        protected static string certificate_password;


        protected static string MySql_Username;
        protected static string MySql_Password;


        protected static string SMTPS_Server_Email_Address = "student.records.system.smtps@gmail.com";
        protected static string SMTPS_Server_Email_Password = "hjtqpldmrvvcfdtm";




        private static string client_certificate_name = "Client_Student_Records_System_Certificate.crt";
        private static string server_certificate_name = "Server_Student_Records_System_Certificate.crt";


        private static Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator randomGenerator = new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator();
        private static Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator = new Org.BouncyCastle.X509.X509V3CertificateGenerator();


        private static Org.BouncyCastle.Asn1.X509.X509Name subjectDN = new Org.BouncyCastle.Asn1.X509.X509Name("CN=Student_Records_System");
        private static Org.BouncyCastle.Asn1.X509.X509Name issuerDN = subjectDN;


        private static int strength = 2048;


        private static string settings_file_name = "server_settings.json";


        public enum X509_Server_Certificate_Operations
        {
            Create_X509_Server_Certificate,
            Load_Certificate,
            Unload_Certificate
        }

        public enum Settings_File_Options
        {
            Load_Settings_From_File,
            Update_Settings_File
        }





        internal static async Task<bool> X509_Server_Certificate_Operational_Controller(X509_Server_Certificate_Operations operation)
        {
            bool Operation_Result = false;

            switch(operation)
            {
                case X509_Server_Certificate_Operations.Load_Certificate:
                    Operation_Result = await Load_Certificate();
                    break;

                case X509_Server_Certificate_Operations.Unload_Certificate:
                    Operation_Result = await Unload_Certificate();
                    break;
            }

            return Operation_Result;
        }


        internal static async Task<bool> X509_Server_Certificate_Operational_Controller(X509_Server_Certificate_Operations operation, int expiry_date_period)
        {
            bool Operation_Result = false;

            if (operation == X509_Server_Certificate_Operations.Create_X509_Server_Certificate)
            {
                Operation_Result = await Create_X509_Server_Certificate(expiry_date_period);
            }

            return Operation_Result;
        }







        // METHOD THAT IS CREATING A X509 SSL CERTIFICATE THAT HAS SHA256 WITH RSA BASED ENCRYPTION
        private static async Task<bool> Create_X509_Server_Certificate(int certificate_valid_time_period_in_days)
        {
            bool Certificate_Generation_Is_Successful = true;

            try
            {
                Org.BouncyCastle.Security.SecureRandom random = new Org.BouncyCastle.Security.SecureRandom(randomGenerator);
                Org.BouncyCastle.Math.BigInteger serialNumber = Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(Org.BouncyCastle.Math.BigInteger.One, Org.BouncyCastle.Math.BigInteger.ValueOf(int.MaxValue), random);
                certificateGenerator.SetSerialNumber(serialNumber);





                certificateGenerator.SetIssuerDN(issuerDN);
                certificateGenerator.SetSubjectDN(subjectDN);



                DateTime notBefore = DateTime.UtcNow.Date;
                DateTime notAfter = notBefore.AddDays(certificate_valid_time_period_in_days);

                certificateGenerator.SetNotBefore(notBefore);
                certificateGenerator.SetNotAfter(notAfter);




                Org.BouncyCastle.Crypto.KeyGenerationParameters keyGenerationParameters = new Org.BouncyCastle.Crypto.KeyGenerationParameters(random, strength);

                Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator keyPairGenerator = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
                keyPairGenerator.Init(keyGenerationParameters);
                Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();

                certificateGenerator.SetPublicKey(subjectKeyPair.Public);


                Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory("SHA256WithRSA", subjectKeyPair.Private, random);



                Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(signatureFactory);



                Org.BouncyCastle.Pkcs.Pkcs12Store store = new Org.BouncyCastle.Pkcs.Pkcs12Store();


                string friendlyName = certificate.SubjectDN.ToString();



                Org.BouncyCastle.Pkcs.X509CertificateEntry certificateEntry = new Org.BouncyCastle.Pkcs.X509CertificateEntry(certificate);
                store.SetCertificateEntry(friendlyName, certificateEntry);




                store.SetKeyEntry(friendlyName, new Org.BouncyCastle.Pkcs.AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });





                MemoryStream certificate_memory_stream = new MemoryStream();

                try
                {
                    store.Save(certificate_memory_stream, certificate_password.ToCharArray(), random);
                    await certificate_memory_stream.FlushAsync();





                    System.Security.Cryptography.X509Certificates.X509Certificate2 client_certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate_memory_stream.ToArray(), certificate_password.ToCharArray());
                    byte[] client_certificate_binary_data = client_certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert, new string(certificate_password.ToCharArray()));






                    FileStream server_certificate_file_stream = File.Create(server_certificate_name, (int)certificate_memory_stream.Length, FileOptions.Asynchronous);
                    try
                    {
                        await server_certificate_file_stream.WriteAsync(certificate_memory_stream.ToArray());
                        await server_certificate_file_stream.FlushAsync();
                    }
                    catch (Exception E)
                    {
                        await Server_Error_Logs(E, "Create_X509_Server_Certificate");
                        Certificate_Generation_Is_Successful = false;

                        if (server_certificate_file_stream != null)
                        {
                            await server_certificate_file_stream.FlushAsync();
                            server_certificate_file_stream.Close();
                        }
                    }
                    finally
                    {
                        if (server_certificate_file_stream != null)
                        {
                            await server_certificate_file_stream.FlushAsync();
                            server_certificate_file_stream.Close();
                            await server_certificate_file_stream.DisposeAsync();
                        }
                    }


                    FileStream client_certificate_file_stream = File.Create(client_certificate_name, client_certificate_binary_data.Length, FileOptions.Asynchronous);
                    try
                    {
                        await client_certificate_file_stream.WriteAsync(client_certificate_binary_data);
                        await client_certificate_file_stream.FlushAsync();
                    }
                    catch (Exception E)
                    {
                        await Server_Error_Logs(E, "Create_X509_Server_Certificate");
                        Certificate_Generation_Is_Successful = false;

                        if (client_certificate_file_stream != null)
                        {
                            await client_certificate_file_stream.FlushAsync();
                            client_certificate_file_stream.Close();
                        }
                    }
                    finally
                    {
                        if (client_certificate_file_stream != null)
                        {
                            await client_certificate_file_stream.FlushAsync();
                            client_certificate_file_stream.Close();
                            await client_certificate_file_stream.DisposeAsync();
                        }
                    }
                }
                catch (Exception E)
                {
                    await Server_Error_Logs(E, "Create_X509_Server_Certificate");
                    Certificate_Generation_Is_Successful = false;

                    if (certificate_memory_stream != null)
                    {
                        await certificate_memory_stream.FlushAsync();
                        certificate_memory_stream.Close();
                    }
                }
                finally
                {
                    if (certificate_memory_stream != null)
                    {
                        await certificate_memory_stream.FlushAsync();
                        certificate_memory_stream.Close();
                        await certificate_memory_stream.DisposeAsync();
                    }
                }
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Create_X509_Server_Certificate");
                Certificate_Generation_Is_Successful = false;
            }

            return Certificate_Generation_Is_Successful;
        }




        private static async Task<bool> Load_Certificate()
        {
            bool Certificate_Loadup_Is_Successful = false;
            string cert_name_segment = String.Empty;


            try
            {


                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    cert_name_segment = "\\" + server_certificate_name;
                }
                else
                {
                    cert_name_segment = "/" + server_certificate_name;
                }



                server_certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + cert_name_segment, certificate_password);
                Certificate_Loadup_Is_Successful = true;

            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Load_Certificate_At_Startup");
            }

            return Certificate_Loadup_Is_Successful;
        }


        private static async Task<bool> Unload_Certificate()
        {
            bool Certificate_Unload_Is_Successful = false;

            try
            {
                server_certificate?.Dispose();
                Certificate_Unload_Is_Successful = true;
            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Load_Certificate_At_Startup");
            }

            return Certificate_Unload_Is_Successful;
        }





        internal static async Task<bool> Settings_File_Controller(Settings_File_Options option)
        {
            bool Settings_File_Controller_Operation_Is_Successful = false;

            switch(option)
            {
                case Settings_File_Options.Load_Settings_From_File:
                    Settings_File_Controller_Operation_Is_Successful = await Load_Settings_From_File();
                    break;

                case Settings_File_Options.Update_Settings_File:
                    Settings_File_Controller_Operation_Is_Successful = await Update_Settings_File();
                    break;
            }

            return Settings_File_Controller_Operation_Is_Successful;
        }




        private static Task<bool> Grant_Settings_File_Permissions()
        {
            if (File.Exists(settings_file_name) == true)
            {
                if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    FileInfo settings_file_info = new FileInfo(settings_file_name);

                    System.Security.AccessControl.FileSecurity settings_file_security = settings_file_info.GetAccessControl();
                    settings_file_security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name, System.Security.AccessControl.FileSystemRights.Write, System.Security.AccessControl.AccessControlType.Allow));
                    settings_file_security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name, System.Security.AccessControl.FileSystemRights.Read, System.Security.AccessControl.AccessControlType.Allow));
                    settings_file_security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(System.Security.Principal.WindowsIdentity.GetCurrent().Name, System.Security.AccessControl.FileSystemRights.Delete, System.Security.AccessControl.AccessControlType.Allow));
                    settings_file_info.SetAccessControl(settings_file_security);
                }
                else
                {
                    File.SetUnixFileMode(settings_file_name, UnixFileMode.UserRead | UnixFileMode.UserWrite);
                }
            }

            return Task.FromResult(true);
        }


        private async static Task<bool> Create_Settings_File()
        {
            bool Create_Settings_File_Is_Successful = false;

            if (File.Exists(settings_file_name) == false)
            {
                FileStream file_stream = File.Create(settings_file_name);

                try
                {

                    Settings_File settings = new Settings_File();

                    settings.port_number = port_number;
                    settings.certificate_password = certificate_password;
                    settings.MySql_Password = MySql_Password;
                    settings.MySql_Username = MySql_Username;
                    settings.SMTPS_Server_Email_Address = SMTPS_Server_Email_Address;
                    settings.SMTPS_Server_Email_Password = SMTPS_Server_Email_Password;


                    byte[] serialized_settings = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(settings));
                    await file_stream.WriteAsync(serialized_settings, 0, serialized_settings.Length);

                    Create_Settings_File_Is_Successful = true;
                }
                catch(Exception E)
                {
                    await Server_Error_Logs(E, "Create_Settings_File");
                }
                finally
                {
                    if (file_stream != null)
                    {
                        await file_stream.DisposeAsync();
                    }
                }


                await Grant_Settings_File_Permissions();
            }

            return Create_Settings_File_Is_Successful;
        }





        private async static Task<bool> Update_Settings_File()
        {
            bool Update_Settings_File_Is_Successful = false;

            try
            {
                await Grant_Settings_File_Permissions();


                if (File.Exists(settings_file_name) == true)
                {
                    File.Delete(settings_file_name);
                }


                Update_Settings_File_Is_Successful = await Create_Settings_File();

            }
            catch (Exception E)
            {
                await Server_Error_Logs(E, "Update_Settings_File");
            }


            return Update_Settings_File_Is_Successful;
        }


        private async static Task<bool> Load_Settings_From_File()
        {
            bool Load_Settings_File_Is_Successful = false;

            await Grant_Settings_File_Permissions();


            if (File.Exists(settings_file_name) == true)
            {

                FileStream file_stream = File.Open(settings_file_name, FileMode.Open);

                try
                {
                    byte[] serialized_file = new byte[file_stream.Length];
                    await file_stream.ReadAsync(serialized_file, 0, serialized_file.Length);

                    string serialized_file_string = Encoding.UTF8.GetString(serialized_file);

                    Settings_File? settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings_File>(serialized_file_string);

                    if(settings != null)
                    {
                        port_number = settings.port_number;
                        certificate_password = settings.certificate_password;
                        MySql_Password = settings.MySql_Password;
                        MySql_Username = settings.MySql_Username;
                        SMTPS_Server_Email_Address = settings.SMTPS_Server_Email_Address;
                        SMTPS_Server_Email_Password = settings.SMTPS_Server_Email_Password;

                        Load_Settings_File_Is_Successful = true;
                    }
                }
                catch (Exception E)
                {
                    await Server_Error_Logs(E, "Load_Settings_From_File");
                }
                finally
                {
                    if (file_stream != null)
                    {
                        file_stream.Dispose();
                    }
                }
            }
            else
            {
                Load_Settings_File_Is_Successful = await Create_Settings_File();
            }

            return Load_Settings_File_Is_Successful;
        }

    }
}
