namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTreeViewToGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "Group_GroupId", c => c.Int());
            CreateIndex("dbo.Groups", "Group_GroupId");
            AddForeignKey("dbo.Groups", "Group_GroupId", "dbo.Groups", "GroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Groups", "Group_GroupId", "dbo.Groups");
            DropIndex("dbo.Groups", new[] { "Group_GroupId" });
            DropColumn("dbo.Groups", "Group_GroupId");
        }
    }
}
