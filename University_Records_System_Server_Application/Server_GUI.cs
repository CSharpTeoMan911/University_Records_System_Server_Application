using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Server_GUI : Server_Variables
    {
      

        public static void Main_Menu()
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



        public static void Port_Setup_Unsuccsessful()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                       PORT SETUP                         ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    The port setup was cancelled.                         ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter any key to exit.                                ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }




        public static void Port_Setup_Succsessful()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                       PORT SETUP                         ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    The port was set up successfully.                     ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter any key to exit.                                ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }
















        // X509 CERTIFICATE SETUP MENUS
        //
        // [ BEGIN ]


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
            Console.WriteLine("\t\t ||    Enter [ E ] to exit the configuration menu            ||");
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
            Console.WriteLine("\t\t ||                 X509 CERTIFICATE EXPIRY DATE             ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter the number of days that this certificate will   ||");
            Console.WriteLine("\t\t ||    be valid.                                             ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter [ E ] to exit the configuration menu            ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }



        public static void Certificate_Generation_Unsuccessful()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||              X509 CERTIFICATE GENERATION ERROR           ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    The X509 certificate unsucessful. The opertion was    ||");
            Console.WriteLine("\t\t ||    cancelled or the OS is corrupted.                     ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter any key to exit.                                ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }


        public static void Certificate_Generation_Successful()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||           X509 CERTIFICATE GENERATION SUCCESSFUL         ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    The X509 certificate was generated successfully.      ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter any key to exit.                                ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }

        // [ END ]











        // MYSQL SETUP MENUS
        //
        // [ BEGIN ]

        public static void MySql_Username_Menu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                       MySQL USERNAME                     ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter the username of the MySQL account that the      ||");
            Console.WriteLine("\t\t ||    server will use.                                      ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter [ E ] to exit the configuration menu            ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }



        public static void MySql_Password_Menu()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                       MySQL PASSWORD                     ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter the password of the MySQL account that the      ||");
            Console.WriteLine("\t\t ||    server will use.                                      ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter [ E ] to exit the configuration menu            ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void MySql_Credentials_Setup_Error()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                         MySQL ERROR                      ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    The username and password entered are not valid or    ||");
            Console.WriteLine("\t\t ||    the MySQL database management system does not         ||");
            Console.WriteLine("\t\t ||    contain the [university_records_system] database.     ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter any key to exit.                                ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }



        public static void MySql_Credentials_Setup_Successful()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("\n\n\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                   MySQL SETUP SUCCESSFUL                 ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    The MySQL credentials were set successfully           ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||    Enter any key to exit.                                ||");
            Console.WriteLine("\t\t ||                                                          ||");
            Console.WriteLine("\t\t ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            Console.Write("\n\n\t\t\t                [ - ] Input: ");

            Console.ForegroundColor = ConsoleColor.White;
        }

        // [ END ]
    }
}
