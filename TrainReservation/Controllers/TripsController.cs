using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrainReservation.Models;
using TrainReservationDBContext;

namespace TrainReservation.Controllers
{
    public class TripsController : Controller
    {
        private TrainReservationDbContext db = new TrainReservationDbContext();
        private ApplicationDbContext udb = new ApplicationDbContext();
        // GET: Trips
        public ActionResult Index()
        {
            return View(db.Trips.ToList());
        }

        // GET: Trips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // GET: Trips/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            if (!User.IsInRole("admin"))
                return Redirect("Index");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TripID,Departure,Departure_Time,Destination,Arrival_Time,Seats,Price")] Trip trip)
        {
            
            trip.SeatPlan = generateSeatPlan(trip.Seats);
            if (ModelState.IsValid)
            {
                db.Trips.Add(trip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trip);
        }

        // GET: Trips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }



        // POST: Trips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TripID,Departure,Departure_Time,Destination,Arrival_Time,Seats,Price")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trip);
        }

        // GET: Trips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            deleteBookings(id);
            refundUsers(id);
            Trip trip = db.Trips.Find(id);
            db.Trips.Remove(trip);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Book(int? id, string sender)
        {
            Bookings booking = new Bookings();

            booking.TripID = (int)id;
            booking.UserId = sender;
            booking.SeatId = findNextAvailableSeat((int)id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }

            BookingConfirmationModel mymodel = new BookingConfirmationModel();

            mymodel.Trips = trip;
            mymodel.Bookings = booking;
            
            
           

            return View(mymodel);

        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Book")]
        [ValidateAntiForgeryToken]
        public ActionResult BookingConfirmed(int?id, string sender)
        {

         
            
            Bookings booking = new Bookings();
            booking.TripID = (int)id;
            booking.UserId = sender;
            booking.SeatId = findNextAvailableSeat((int)id);
            occupySeat(booking.SeatId, (int)id);
            db.Bookings.Add(booking);
            db.Trips.Find(booking.TripID).Seats--;
            db.SaveChanges();
            udb.Users.Find(sender).Due += db.Trips.Find(booking.TripID).Price;
            udb.SaveChanges();
            return RedirectToAction("Confirmed");
        }

        public ActionResult Confirmed()
        {
            return View();

        }

        private dynamic GetTrips()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void deleteBookings(int id)
        {
           var booking =  from b in db.Bookings
                          where b.TripID == id
                          select b;

            
            foreach (Bookings b in booking)
            {
                db.Bookings.Remove(b);
            }

        }

        public void refundUsers(int id)
        {
            var booking = from b in db.Bookings
                          where b.TripID == id
                          select b;

            decimal p = db.Trips.Find(id).Price;

            foreach (Bookings b in booking)
            {
                
                udb.Users.Find(b.UserId).Due -= p;
            }

            udb.SaveChanges();
        }

        public string findNextAvailableSeat(int id)
        {
            string [] seat = db.Trips.Find(id).SeatPlan.Split(',');
            
            for (int i = 0;  i < seat.GetLength(0); i++)
            {
                if (seat[i][3] == '0')
                    return seat[i].Substring(0, 3);
            }

            return "000";


        }

        public void occupySeat(string seat, int id)
        {
            string Plan = db.Trips.Find(id).SeatPlan;
            char[] array = Plan.ToCharArray();
            if (seat != "000")
            {
                if (Plan.ElementAt(Plan.IndexOf(seat) + 3) == '0')
                    array[Plan.IndexOf(seat) + 3] = '1';


            }

            db.Trips.Find(id).SeatPlan = new string(array);

        }

        

        public string generateSeatPlan(int seatNo)
        {
            string Plan = "";

            for (int i = 1; i<=seatNo/24; i ++)
            {
                for (int j = 1; j <=6; j++)
                {
                    Plan = Plan + i.ToString() + j.ToString() + "A0,";
                    Plan = Plan + i.ToString() + j.ToString() + "B0,";
                    Plan = Plan + i.ToString() + j.ToString() + "C0,";
                    Plan = Plan + i.ToString() + j.ToString() + "D0,";
                }
            }

           Plan =  Plan.Remove(Plan.Length - 1);

            return Plan;
        }
    }

}
