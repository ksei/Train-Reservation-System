using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainReservation.Models
{
    public class BookingConfirmationModel
    {
        
            public Trip Trips { get; set; }
            public Bookings Bookings { get; set; }
      
    }
}