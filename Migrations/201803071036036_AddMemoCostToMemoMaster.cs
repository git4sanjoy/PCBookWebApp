namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMemoCostToMemoMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemoMasters", "MemoCost", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MemoMasters", "MemoCost");
        }
    }
}
