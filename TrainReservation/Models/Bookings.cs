using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using TrainReservationDBContext;

namespace TrainReservation.Models
{
    public class Bookings
    {
        [Key]
        public int BookingID { get; set; }
        public int TripID { get; set; }
        public string UserId { get; set; }
        public int Number_of_Seats { get; set; }


    }


    
}