namespace TrainReservation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trips",
                c => new
                    {
                        TripID = c.Int(nullable: false, identity: true),
                        Departure = c.String(),
                        Departure_Time = c.DateTime(nullable: false),
                        Destination = c.String(),
                        Arrival_Time = c.DateTime(nullable: false),
                        Seats = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TripID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Trips");
        }
    }
}
