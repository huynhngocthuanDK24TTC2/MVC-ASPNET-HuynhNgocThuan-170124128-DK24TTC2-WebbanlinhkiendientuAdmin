namespace WebbanlinhkiendientuAdmin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnNote : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Orders", new[] { "CustomerID" });
            AddColumn("dbo.Orders", "Note", c => c.String());
            AlterColumn("dbo.Orders", "CustomerID", c => c.Int());
            CreateIndex("dbo.Orders", "CustomerID");
            DropColumn("dbo.Orders", "Notes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Notes", c => c.String());
            DropIndex("dbo.Orders", new[] { "CustomerID" });
            AlterColumn("dbo.Orders", "CustomerID", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "Note");
            CreateIndex("dbo.Orders", "CustomerID");
        }
    }
}
