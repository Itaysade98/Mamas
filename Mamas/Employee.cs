using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mamas
{
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


        public Employee(int id, string name, string jobTitle, float hoursWorked, int clockedIn, DateTime lastTimestamp)
        {
            this.Id = id;
            this.Name = name;
            this.JobTitle = jobTitle;
            this.HoursWorked = hoursWorked;
            this.ClockedIn = clockedIn;
            this.LastTimestamp = lastTimestamp;
        }

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

