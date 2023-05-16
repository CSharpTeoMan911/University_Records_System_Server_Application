using Org.BouncyCastle.Crypto.Prng;
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

        protected static int port_number = 1024;


        protected static string certificate_password = "k_e-i-y-91-11-80";


        protected static string MySql_Username = "stundent_records_server";
        protected static string MySql_Password = "stundent_records_server";


        protected static string SMTPS_Server_Email_Address = "student.records.system.smtps@gmail.com";
        protected static string SMTPS_Server_Email_Password = "hjtqpldmrvvcfdtm";




        private static string client_certificate_name = "Client_Student_Records_System_Certificate.crt";
        private static string server_certificate_name = "Server_Student_Records_System_Certificate.crt";


        private static Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator randomGenerator = new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator();
        private static Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator = new Org.BouncyCastle.X509.X509V3CertificateGenerator();


        private static Org.BouncyCastle.Asn1.X509.X509Name subjectDN = new Org.BouncyCastle.Asn1.X509.X509Name("CN=Student_Records_System");
        private static Org.BouncyCastle.Asn1.X509.X509Name issuerDN = subjectDN;


        private static int strength = 2048;




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






        // METHOD THAT IS CREATING A X509 SSL CERTIFICATE THAT HAS SHA256 WITH RSA BASED ENCRYPTION
        internal async Task<bool> Create_X509_Server_Certificate(string password, int certificate_valid_time_period_in_days)
        {

            bool server_certificate_creation_successful = false;


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





                System.IO.MemoryStream certificate_memory_stream = new System.IO.MemoryStream();

                try
                {
                    store.Save(certificate_memory_stream, password.ToCharArray(), random);
                    await certificate_memory_stream.FlushAsync();





                    System.Security.Cryptography.X509Certificates.X509Certificate2 client_certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate_memory_stream.ToArray(), password.ToCharArray());
                    byte[] client_certificate_binary_data = client_certificate.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Cert, new string(password.ToCharArray()));






                    System.IO.FileStream server_certificate_file_stream = System.IO.File.Create(server_certificate_name, (int)certificate_memory_stream.Length, System.IO.FileOptions.Asynchronous);
                    try
                    {
                        await server_certificate_file_stream.WriteAsync(certificate_memory_stream.ToArray());
                        await server_certificate_file_stream.FlushAsync();
                    }
                    catch
                    {
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


                    System.IO.FileStream client_certificate_file_stream = System.IO.File.Create(client_certificate_name, client_certificate_binary_data.Length, System.IO.FileOptions.Asynchronous);
                    try
                    {
                        await client_certificate_file_stream.WriteAsync(client_certificate_binary_data);
                        await client_certificate_file_stream.FlushAsync();
                    }
                    catch
                    {
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
                catch
                {
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

                server_certificate_creation_successful = true;
            }
            catch
            {

            }


            return server_certificate_creation_successful;
        }




        protected static async Task<bool> Load_Certificate()
        {
            string cert_name_segment = String.Empty;


            try
            {
                /*
                 
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    cert_name_segment = "\\" + server_certificate_name;
                }
                else
                {
                    cert_name_segment = "/" + server_certificate;
                }

                 */

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
                server_certificate?.Dispose();
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
