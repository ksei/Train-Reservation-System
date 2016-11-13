using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using TrainReservationDBContext;

namespace TrainReservation.Models
{
    public class Trip
    {
        public int TripID { get; set; }
        public string Departure { get; set; }
       [ Display(Name = "Departure Time")]
        public DateTime Departure_Time { get; set; }
        public string Destination { get; set; }
        [Display(Name = "Arrival Time")]
        public DateTime Arrival_Time { get; set; }
        public int Seats { get; set; }
        public decimal Price { get; set; }
        public string SeatPlan { get; set; }

    }

 
}