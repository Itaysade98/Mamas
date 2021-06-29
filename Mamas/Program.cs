using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace Mamas
{
    class Program
    {
        /// <summary>
        /// My main function calls utils.load() to load all configuration and
        /// employee data, then calls Menu.MainMenu() to enter a while loop that 
        /// functions as the menu for the application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Utils.Load();
                string option;
                Menu.MainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey();
            }
            
        }
    }
}
