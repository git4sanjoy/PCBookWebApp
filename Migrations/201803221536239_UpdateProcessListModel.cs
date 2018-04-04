namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProcessListModel : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ProcessLists", "UnitRoleId");
            AddForeignKey("dbo.ProcessLists", "UnitRoleId", "dbo.UnitRoles", "UnitRoleId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProcessLists", "UnitRoleId", "dbo.UnitRoles");
            DropIndex("dbo.ProcessLists", new[] { "UnitRoleId" });
        }
    }
}
