using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mamas
{
    public static class Menu
    {

        /// <summary>
        /// Writes and manages the applications main menu.
        /// reads user input and calls function accordingly.
        /// </summary>
        public static void MainMenu()
        {
            string optionSelection;
            do
                {
                Console.WriteLine("Welcome to Ichilov Hospital!");
                Console.WriteLine("1. Clock in/out by ID number");
                Console.WriteLine("2. Get employee current monthly salary by ID number");
                Console.WriteLine("3. Exit");
                Console.WriteLine("Please choose an action: ");
                optionSelection = Console.ReadLine();
                switch (optionSelection)
                {
                    case "1":
                        {
                            ClockEmployee();
                            Console.WriteLine("Press any key to return to menu");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }

                    case "2":
                        {
                            GetEmployeePaycheck();
                            Console.WriteLine("Press any key to return to menu");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }

                    default:
                        Console.Clear();
                        break;
                }

            } while (optionSelection != "3");
         }


        /// <summary>
        /// asks user for employee ID and validates the input.
        /// if the input is not a valid ID it will call itself recursively
        /// until a valid id is given.
        /// </summary>
        /// <returns>valid employee ID entered by user</returns>
        private static int GetEmployeeID()
        {
            Console.WriteLine("Please enter employee ID:");
            string inputid = Console.ReadLine();
            int id;
            bool isnum = int.TryParse(inputid, out id);
            if (!isnum)
            {
                Console.Clear();
                Console.WriteLine("you did not input a valid id number, please try again.");
                return GetEmployeeID();
            }
            else
            {
                return id;
            }
        }

        /// <summary>
        /// handles option 1 of main menu.
        /// gets employee by id entered by the user,
        /// and calls its Clock() function.
        /// prints current employee clock status.
        /// </summary>
        private static void ClockEmployee()
        {
            Employee employee = GetEmployee();
            employee.Clock();
            string status = "out";
            if (employee.clockedin == 1)
            {
                status = "in";
            }
            Console.WriteLine($"{employee.name} is clocked {status}");
        }

        /// <summary>
        /// handles option 2 of main menu.
        /// gets employee by id entered by the user,
        /// and calls its CalculatePaycheck() function.
        /// prints current employees paycheck.
        /// </summary>
        private static void GetEmployeePaycheck()
        {
            Employee employee = GetEmployee();
            float currentpay = employee.CalculatePaycheck();
            Console.WriteLine($"{employee.name}s' paycheck for month-{employee.lasttimestamp.Month}/{employee.lasttimestamp.Year} is: {currentpay}");
        }

        /// <summary>
        /// calls GetEmployeeID() to get valid ID
        /// from user and checks if an employee with that id exists.
        /// if so, it returns the employee.
        /// if not, it calls itself again recursively until a valid employee ID is given.
        /// </summary>
        /// <returns>Employee with ID number entered by user</returns>
        private static Employee GetEmployee()
        {
            int id = GetEmployeeID();
            List<Employee> filteredEmployees = Utils.Employees.Where(a => a.id == id).ToList();
            if (filteredEmployees.Count > 0)
            {
                return filteredEmployees[0];
            }
            else
            {
                Console.Clear();
                Console.WriteLine("The Employee ID entered does not exist. please try again.");
                return GetEmployee();
            }
        }

    }
}
