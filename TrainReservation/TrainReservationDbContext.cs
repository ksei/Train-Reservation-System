using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TrainReservation.Models;

namespace TrainReservationDBContext
{
    public class TrainReservationDbContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Bookings> Bookings { get; set; }

    }
}