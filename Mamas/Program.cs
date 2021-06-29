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
        /// employee data, then enters a while loop that functions as the menu for
        /// the application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Utils.Load();
                string option;
                do
                {
                    Console.WriteLine("Welcome to Ichilov Hospital!");
                    Console.WriteLine("1. Clock in/out by ID number");
                    Console.WriteLine("2. Get employee current monthly salary by ID number");
                    Console.WriteLine("3. Exit");
                    Console.WriteLine("Please choose an action: ");
                    option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            {
                                Console.WriteLine("Please enter employee ID to clock in/out: ");
                                string inputid = Console.ReadLine();
                                int id;
                                bool isnum = int.TryParse(inputid, out id);
                                if (!isnum)
                                {
                                    Console.WriteLine("you did not input a valid id number, please try again.");
                                }
                                else
                                {
                                    List<Employee> filteredEmployees = Utils.Employees.Where(a => a.id == id).ToList();
                                    if (filteredEmployees.Count > 0)
                                    {
                                        filteredEmployees[0].Clock();
                                        string status = "out";
                                        if (filteredEmployees[0].clockedin == 1)
                                        {
                                            status = "in";
                                        }

                                        Console.WriteLine($"{filteredEmployees[0].name} is clocked {status}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("The Employee ID entered does not exist. please try again.");
                                    }
                                }
                                Console.WriteLine("Press any key to return to menu");
                                Console.ReadKey();
                                Console.Clear();
                                break;
                            }

                        case "2":
                            {
                                Console.WriteLine("Please enter employee ID to calculate paycheck: ");
                                string inputid = Console.ReadLine();
                                int id;
                                bool isnum = int.TryParse(inputid, out id);
                                if (!isnum)
                                {
                                    Console.WriteLine("you did not input a valid id number, please try again.");
                                }
                                else
                                {
                                    List<Employee> filteredEmployees = Utils.Employees.Where(a => a.id == id).ToList();
                                    if (filteredEmployees.Count > 0)
                                    {
                                        float currentpay = filteredEmployees[0].CalculatePaycheck();
                                        Console.WriteLine($"{filteredEmployees[0].name}s' paycheck for month-{filteredEmployees[0].lasttimestamp.Month}/{filteredEmployees[0].lasttimestamp.Year} is: {currentpay}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("The Employee ID entered does not exist. please try again.");
                                    }
                                }
                                Console.WriteLine("Press any key to return to menu");
                                Console.ReadKey();
                                Console.Clear();
                                break;
                            }

                        default:

                            break;
                    }

                } while (option != "3");
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
