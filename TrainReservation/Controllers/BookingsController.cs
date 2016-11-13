using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrainReservationDBContext;
using Microsoft.AspNet.Identity;


namespace TrainReservation.Models
{
    public class BookingsController : Controller
    {
        private TrainReservationDbContext db = new TrainReservationDbContext();
        private ApplicationDbContext udb = new ApplicationDbContext();
        // GET: Bookings
        public ActionResult Index()
        {
            List<ViewBookingsModel> data = new List<ViewBookingsModel>();

            if (User.IsInRole("admin"))
            {
                foreach (var bing in db.Bookings.ToList())
                {
                    ViewBookingsModel mymodel = new ViewBookingsModel();

                    mymodel.bookingId = bing.BookingID;
                    mymodel.tripId = bing.TripID;
                    mymodel.seatId = bing.SeatId;
                    mymodel.Name = udb.Users.Find(bing.UserId).Name;
                    mymodel.tripName = db.Trips.Find(bing.TripID).Departure + " - " + db.Trips.Find(bing.TripID).Destination;
                    mymodel.Time = db.Trips.Find(bing.TripID).Departure_Time;

                    data.Add(mymodel);

                }
            }
            else
            {
                string id = User.Identity.GetUserId();
                
                foreach (var bing in db.Bookings.Where(booking => booking.UserId == id  ).ToList())
                {
                    ViewBookingsModel mymodel = new ViewBookingsModel();

                    mymodel.bookingId = bing.BookingID;
                    mymodel.tripId = bing.TripID;
                    mymodel.seatId = bing.SeatId;
                    mymodel.Name = udb.Users.Find(bing.UserId).Name;
                    mymodel.tripName = db.Trips.Find(bing.TripID).Departure + " - " + db.Trips.Find(bing.TripID).Destination;
                    mymodel.Time = db.Trips.Find(bing.TripID).Departure_Time;

                    data.Add(mymodel);

                }

            }

            return View(data.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings bookings = db.Bookings.Find(id);
            if (bookings == null)
            {
                return HttpNotFound();
            }
            return View(bookings);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,TripID,UserId")] Bookings bookings)
        {
           
            if (ModelState.IsValid)
            {
                db.Bookings.Add(bookings);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bookings);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings bookings = db.Bookings.Find(id);
            if (bookings == null)
            {
                return HttpNotFound();
            }
            return View(bookings);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,TripID,UserId")] Bookings bookings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookings).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookings);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBookingsModel mymodel = new ViewBookingsModel();
            mymodel.bookingId = (int)id; 
            mymodel.seatId = db.Bookings.Find(id).SeatId;
            mymodel.tripId = db.Bookings.Find(id).TripID;
            mymodel.tripName = db.Trips.Find(db.Bookings.Find(id).TripID).Departure + " - " + db.Trips.Find(db.Bookings.Find(id).TripID).Destination;
            mymodel.Time = db.Trips.Find(db.Bookings.Find(id).TripID).Departure_Time;
            if (mymodel == null)
            {
                return HttpNotFound();
            }
            return View(mymodel);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bookings bookings = db.Bookings.Find(id);
            db.Bookings.Remove(bookings);
            releaseSeat(bookings.SeatId, bookings.TripID);
            db.Trips.Find(bookings.TripID).Seats++;
            db.SaveChanges();
            refundUser(bookings.TripID, bookings.UserId);
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void releaseSeat(string seat, int id)
        {
            string Plan = db.Trips.Find(id).SeatPlan;
            char[] array = Plan.ToCharArray();
            if (seat != "000")
            {
                if (Plan.ElementAt(Plan.IndexOf(seat) + 3) == '1')
                    array[Plan.IndexOf(seat) + 3] = '0';


            }

            db.Trips.Find(id).SeatPlan = new string(array);

        }

        public void refundUser(int tripid, string uid)
        {
           

            decimal p = db.Trips.Find(tripid).Price;

            
                udb.Users.Find(uid).Due -= p;
            

            udb.SaveChanges();
        }
    }
}
