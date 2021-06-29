using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;

namespace Mamas
{
    /// <summary>
    /// class for static variables and functions used through-out the project.
    /// </summary>
   public static class Utils
    {
        public static List<Employee> Employees = new List<Employee>();
        public static Dictionary<string, Dictionary<string, int>> Ranks;
        public static Dictionary<string, List<dynamic>> Jobs;
        public static float Salary = 30;
        private static string ProjectDir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        private static string cs = $@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = {ProjectDir}\Database.mdf; Integrated Security = True";
        private static SqlConnection con = new SqlConnection(cs);
        private static string RanksJsonPath = @"..\..\Ranks.json";
        private static string JobsJsonPath = @"..\..\Jobs.json";

        /// <summary>
        /// Loads variables from app.config, if it encounters an exception 
        /// it uses the default parameters hard-coded above and writes to console
        /// that there was an error
        /// </summary>
        private static void LoadVars()
        {
            AppSettingsReader r = new AppSettingsReader();
            try
            {
                Salary = (float)r.GetValue("Salary", typeof(float));
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't read Salary from config file: " + e.Message);
            }

            try
            {
                cs = (string)r.GetValue("ConnectionString", typeof(string));
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't read Connection String from config file: " + e.Message);
            }

            try
            {
                RanksJsonPath = (string)r.GetValue("RanksJsonPath", typeof(string));
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't read Ranks.Json Path from config file: " + e.Message);
            }

            try
            {
                JobsJsonPath = (string)r.GetValue("JobsJsonPath", typeof(string));
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't read Jobs.Json Path from config file: " + e.Message);
            }
        }

        /// <summary>
        /// Loads Ranks.json into a static Dictionary.
        /// in case of an error, writes to console and re-throws excpetion.
        /// </summary>
        private static void LoadRanks()
        {
            try
            {
                using (StreamReader r = new StreamReader(RanksJsonPath))
                {
                    string json = r.ReadToEnd();
                    Ranks = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed while trying to read Ranks.json, got the following error: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads Jobs.json into a static Dictionary.
        /// in case of an error or if one of the ranks doesn't exist - throws an excpetion.
        /// </summary>
        private static void LoadJobs()
        {
            try
            {
                using (StreamReader r = new StreamReader(JobsJsonPath))
                {
                    string json = r.ReadToEnd();
                    Jobs = JsonConvert.DeserializeObject<Dictionary<string, List<dynamic>>>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed while trying to read Jobs.json, got the following error: {e.Message}");
                throw;
            }

            foreach ( List<dynamic> job in Jobs.Values)
            {
                foreach (string rank in job[1])
                {
                    if (!Ranks.ContainsKey(rank))
                    {
                        throw new ApplicationException($"rank: {rank} appears in Jobs.json but is not configured!");
                    }
                }
            }
            
        }

        /// <summary>
        /// Loads all Employees from DB file to static list of employees.
        /// if an employee has a jobtitle that is not configured in Jobs.json he/she will not
        /// be entered into the list.
        /// </summary>
        private static void LoadEmployees()
        {
            con = new SqlConnection(String.Format(cs, ProjectDir));
            SqlDataReader dataReader;
            string query = "select * from [dbo].[Employees]";
            con.Open();
            SqlCommand command = new SqlCommand(query, con);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
            
                int id = dataReader.GetFieldValue<int>(0);
                string name = dataReader.GetFieldValue<string>(1);
                string jobtitle = dataReader.GetFieldValue<string>(2);
                float hoursworked = float.Parse(dataReader.GetFieldValue<double>(3).ToString());
                int clockedin = Convert.ToInt32(dataReader.GetValue(4));
                DateTime lasttimestamp = dataReader.GetFieldValue<DateTime>(5);
                if (!Jobs.ContainsKey(jobtitle))
                {
                    Console.WriteLine($"Employee ID: {id} was not added to list because job: {jobtitle} does not exist in configuration.");
                    continue;
                }
                else
                {
                    Employees.Add(new Employee(id, name, jobtitle, hoursworked, clockedin, lasttimestamp));
                }
            }
            dataReader.Close();
            command.Dispose();
            con.Close();
        }

        /// <summary>
        /// sychronizes DB with updates made to given employee.
        /// </summary>
        /// <param name="employee">Employee object to update in DB</param>
        public static void UpdateEmployee(Employee employee)
        {
            string sql = $"update [dbo].[Employees] set HoursWorked={ employee.hoursworked}, ClockedIn={employee.clockedin}, LastTimestamp='{ employee.lasttimestamp.ToString()}' where Id={ employee.id}";
            SqlDataAdapter adapter = new SqlDataAdapter();
            con.Open();
            adapter.UpdateCommand = new SqlCommand(sql, con);
            adapter.UpdateCommand.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// calls all other static load functions in the right order.
        /// </summary>
        public static void Load()
        {
            LoadVars();
            LoadRanks();
            LoadJobs();
            LoadEmployees();
        }
    }
}
