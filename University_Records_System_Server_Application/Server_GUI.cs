using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Server_GUI : Server_Variables
    {
      

        public static Task<bool> Main_Menu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            
            Console.Write("\n\n\n\t\t" + @"    ////////////|" + "\n");
            Console.Write("\t\t" + @"  /////////////||" + "\n");
            Console.Write("\t\t !!!!!!!!!!!!!|||\n");
            Console.Write("\t\t :===========:|||\n");
            Console.Write("\t\t :===========:|||\t\n");
            Console.Write("\t\t :===========:|||\t\n");
            Console.Write("\t\t :-----------:|||\t||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||\n");
            Console.Write("\t\t :[[[[[[[[[");

            if(On_Off == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.Write("O");

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.Write("]:|||\t||                                                            ||\n");
            Console.Write("\t\t :-----------:|||\t|| Enter [ S ] to start or stop the server, [ O ] to open the ||\n");
            Console.Write("\t\t :===========:|||\t|| options menu, or [ E ] to exit.                            ||\n");
            Console.Write("\t\t :===========:|||\t||                                                            ||\n");
            Console.Write("\t\t :===========:|||\t||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||\n");
            Console.Write("\t\t !!!!!!!!!!!!!|| \n");
            Console.Write("\t\t !!!!!!!!!!!!!/  \n\n");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;

            return Task.FromResult(true);
        }


        public static void Settings_Menu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                          SETTINGS                        ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||     [ P ] Set the port number                            ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||     [ G ] Generate a self-signed SSL certificate         ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||     [ M ] Set MySql credentials                          ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||     [ E ] Exit the settings menu                         ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }




        public static void Port_Setting_Menu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                            PORT                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||     Enter the port number or [ E ] to exit               ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }




        public static void Certificate_Loadup_Error()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                X509 CERTIFICATE NOT VALID                ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    The server SSL certificate does not exist or it is    ||");
            Console.WriteLine("\t\t ||    not valid. Enter any key to exit.                     ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }




        public static void Certificate_Generation_Password_Menu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                 X509 CERTIFICATE PASSWORD                ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter the server certificate's password that will be  ||");
            Console.WriteLine("\t\t ||    used by the generated certificate.                    ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }




        public static void Certificate_Generation_Expiry_Date_Menu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                 X509 CERTIFICATE EXIPRY DATE             ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter the number of days that this certificate will   ||");
            Console.WriteLine("\t\t ||    be valid.                                             ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
