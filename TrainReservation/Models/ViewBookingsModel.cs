using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TrainReservation.Models
{
    public class ViewBookingsModel
    {

        [Display(Name = "Booking Number")]
        public int bookingId { get; set; }

        [Display(Name = "Trip Number")]
        public int tripId { get; set; }

        [Display(Name = "Interval")]
        public string tripName { get; set; }

        [Display(Name = "Seat Number")]
        public string seatId { get; set; }

        [Display(Name = "Reserved by")]
        public string Name { get; set; }

        [Display(Name = "Departure Time")]
        public DateTime Time {get; set;} 



    }
}