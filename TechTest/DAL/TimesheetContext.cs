using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechTest.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace TechTest.DAL
{
    public class TimesheetContext : DbContext
    {
        public TimesheetContext() : base("TimesheetContext")
        {

        }

        public DbSet<Timesheet> Timesheet { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}