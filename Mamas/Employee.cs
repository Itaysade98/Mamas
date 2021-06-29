using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mamas
{
    /// <summary>
    /// Employee class, holds all relevant information need about an employee at Ichilov
    /// </summary>
    public class Employee
    {
        private int Id;
        private string Name;
        private string JobTitle;
        private float HoursWorked;
        private int ClockedIn;
        private DateTime LastTimestamp;

        public int id
        {
            get { return Id; }
        }

        public string name
        {
            get { return Name; }
        }

        public string jobtitle
        {
            get { return JobTitle; }
        }

        public int clockedin
        {
            get { return ClockedIn; }
        }
        public DateTime lasttimestamp
        {
            get { return LastTimestamp; }
        }
        public float hoursworked
        {
            get { return HoursWorked; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id">employee id (DB primary key)</param>
        /// <param name="name">employee name</param>
        /// <param name="jobTitle"> emplyee job title (through configuration defines ranks and salary bonuses)</param>
        /// <param name="hoursWorked">employees' total hour worked this month so far</param>
        /// <param name="clockedIn">boolean describing if the employee is on/off the clock - 0=off the clock, 1=on the clock</param>
        /// <param name="lastTimestamp">time of last change in employee status</param>
        public Employee(int id, string name, string jobTitle, float hoursWorked, int clockedIn, DateTime lastTimestamp)
        {
            this.Id = id;
            this.Name = name;
            this.JobTitle = jobTitle;
            this.HoursWorked = hoursWorked;
            this.ClockedIn = clockedIn;
            this.LastTimestamp = lastTimestamp;
        }

        /// <summary>
        /// clocks employee in or out depending on the current status, month and time
        /// ***resets hours worked if month has changed from last timestamp, to start a new month calculation.
        /// </summary>
        public void Clock()
        {
            var time = DateTime.Now;
            if (ClockedIn == 0)
            {
                if (LastTimestamp.Month == time.Month && LastTimestamp.Year == time.Year)
                {
                    LastTimestamp = time;
                    ClockedIn = 1;
                }
                else
                {
                    HoursWorked = 0;
                    LastTimestamp = time;
                    ClockedIn = 1;
                }
            }
            else
            {
                if (LastTimestamp.Month == time.Month && LastTimestamp.Year == time.Year)
                {
                    var delta = time - LastTimestamp;
                    HoursWorked += Convert.ToSingle(delta.TotalHours);
                    LastTimestamp = time;
                    ClockedIn = 0;
                }
                else
                {
                    var beginningOfMonth = new DateTime(time.Year, time.Month, 1);
                    var delta = time - beginningOfMonth;
                    HoursWorked = Convert.ToSingle(delta.TotalHours);
                    LastTimestamp = time;
                    ClockedIn = 0;
                }
            }
            Utils.UpdateEmployee(this);
        }

        /// <summary>
        /// pulls salary information from Jobs.json and Ranks.json for Employee and calculates paycheck
        /// according to current recorded stats
        /// </summary>
        /// <returns>calculated paycheck up to 2 digits past the decimal point</returns>
        public float CalculatePaycheck()
        {
            int priority = 0;
            float hourlybonuspercent = 0;
            float minimumhoursforglobal = -1;
            float dangerbonuspercent = (float)Utils.Jobs[JobTitle][2];
            float globalHours = 0;

            foreach (string Rank in Utils.Jobs[JobTitle][1])
            {
                if (Utils.Ranks[Rank]["Priority"] > priority)
                {
                    hourlybonuspercent = (float)(Utils.Ranks[Rank]["hourlyBonusPercent"]);
                    if (Utils.Ranks[Rank].ContainsKey("minimumHoursForGlobal"))
                    {
                        minimumhoursforglobal = (float)(Utils.Ranks[Rank]["minimumHoursForGlobal"]);
                        globalHours = (float)(Utils.Ranks[Rank]["globalHours"]);
                    }
                    priority = (int)Utils.Ranks[Rank]["Priority"];

                }
            }
            float salaryWithBonus = (Utils.Salary * (100 + hourlybonuspercent) / 100);
            float dangerMultiplier = (100 + dangerbonuspercent) / 100;
            float payCheck;
            if (minimumhoursforglobal != -1 && HoursWorked >= minimumhoursforglobal)
            {
                payCheck = salaryWithBonus * globalHours * dangerMultiplier;
                payCheck = (float)(Math.Round(payCheck * 100) / 100);
            }
            else
            {
                payCheck = salaryWithBonus * HoursWorked * dangerMultiplier;
                payCheck = (float)(Math.Round(payCheck * 100) / 100);
            }

            return payCheck;
        }
    }
}

