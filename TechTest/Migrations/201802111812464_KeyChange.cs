namespace TechTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KeyChange : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Timesheet");
            AlterColumn("dbo.Timesheet", "timesheetId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Timesheet", "timesheetJob", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Timesheet", "timesheetId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Timesheet");
            AlterColumn("dbo.Timesheet", "timesheetJob", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Timesheet", "timesheetId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Timesheet", "timesheetId");
        }
    }
}
