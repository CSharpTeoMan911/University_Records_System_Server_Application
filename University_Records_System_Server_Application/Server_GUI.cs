using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University_Records_System_Server_Application
{
    internal class Server_GUI : Server_Variables
    {
      

        public static string Main_Menu()
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

            string input = Console.ReadLine();
            string return_value = String.Empty;


            if (input == "S")
            {
                if (On_Off == 0)
                {
                    On_Off = 1;
                    return_value = "Start";
                }
                else 
                {
                    On_Off = 0;
                    return_value = "Stop";
                }
            }
            else if (input == "O")
            {
                return_value = "Options menu";
            }
            else if (input == "E")
            {
                return_value = "Exit";
            }
            else
            {
                Main_Menu();
            }


            return return_value;
            
        }
    }
}
