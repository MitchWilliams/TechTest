namespace TechTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Timesheet",
                c => new
                    {
                        timesheetId = c.Int(nullable: false),
                        timesheetTitle = c.String(),
                        candidateName = c.String(),
                        clientName = c.String(),
                        jobTitle = c.String(),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        placementType = c.Int(nullable: false),
                        timesheetJob = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.timesheetId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Timesheet");
        }
    }
}
