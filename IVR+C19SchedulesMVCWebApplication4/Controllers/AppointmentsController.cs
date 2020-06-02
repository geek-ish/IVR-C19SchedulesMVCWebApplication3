using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IVR_C19SchedulesMVCWebApplication4.Models;
using System.Globalization;
using System.Threading;
using Microsoft.Ajax.Utilities;

namespace IVR_C19SchedulesMVCWebApplication4.Controllers
{
    public partial class AppointmentsController : Controller
    {
        private AppointmentsContext db = new AppointmentsContext();

        private DateTime[] appointments = new DateTime[] { };

        [HttpGet]
        public JsonResult GetDateHourMessage(string requestedDate, string requestedHour, string meridianId)
        {
            try
            {
                if (string.IsNullOrEmpty(requestedDate)) throw new Exception($"Requested Date cannot be null or empty: {requestedDate}");
                if (string.IsNullOrEmpty(requestedHour)) throw new Exception($"Requested Hour cannot be null or empty: {requestedHour}");
                requestedDate = UnStringify(requestedDate);
                requestedHour = UnStringify(requestedHour.Replace("#", string.Empty));
                //meridianId = !string.IsNullOrEmpty(meridianId) ? UnStringify(meridianId) : string.Empty;

                int hour = int.Parse(requestedHour);
                if (hour == 1 || hour == 2)
                    hour += 12;
                string meridianName = hour > 11 ? "PM" : "AM"; //meridianNames[meridianId];
                if (hour >= 9 && hour <= 14)
                {
                    //requestedHour = int.Parse(requestedHour.ToString());
                    //int mId = int.Parse(meridianId.ToString());
                    DateTime reqDate = new DateTime(DateTime.Now.Year, int.Parse(requestedDate.Substring(0, 2)),
                        int.Parse(requestedDate.Substring(2, 2)));
                    var i = 1;
                    appointments = GetHourAvailableAppointmentDateTimes(
                        requestedDate: reqDate, hour: hour);
                    var appointmentsMessage = string.Join("\n",
                        appointments.Select(dt => $"{i++}:  {dt.ToShortTimeString()}").ToList());
                    var unorderedAppointmentsMessage = string.Join("\n\n",
                        appointments.Select(dt => dt.ToShortTimeString()).ToArray());

                    return new JsonResult()
                    {
                        Data = new IvrResponseMessage()
                        {
                            SuccessResponseMessage =
                                $"starting at {hour} {meridianName} \n"/* for {reqDate.ToLongDateString()}" + 
                                $"there are {appointments.Length} appointment times available:" + 
                                $"{unorderedAppointmentsMessage}"*/,
                            Parts = new string[] { unorderedAppointmentsMessage, appointmentsMessage }
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                }

                throw new Exception($"Requested hour outside of valid 9AM - 2PM range: {hour} {meridianName}");
            }
            catch (Exception)
            {
                throw;
            }

        }

        private string UnStringify(string s)
        {
            var pattern = "\"";
            return s.Replace(pattern, string.Empty);
        }

        private DateTime[] GetHourAvailableAppointmentDateTimes(DateTime requestedDate, int hour)
        {
            //return new string[] {"00", "15", "30", "45"}.Select(mm =>
            //        DateTime.Parse($"{requestedDate} {DateTime.Parse(hour.ToString() + mm).ToShortTimeString()}"))
            return new string[] { "00", "15", "30", "45" }.Select(mm =>
                      DateTime.Parse(
                          $"{requestedDate.ToShortDateString()} {DateTime.Parse($"{hour.ToString()}:{mm}").ToShortTimeString()}"))
                .ToArray();
        }

        [HttpGet]
        public JsonResult GetDayPartsMessage(int meridianId)
        {
            try
            {
                var dayPart = dayParts[meridianId];
                return new JsonResult()
                {
                    Data = new IvrResponseMessage()
                    {
                        SuccessResponseMessage = $" {(meridianId == 1 ? "a" : "an")} {dayPart} appointment",
                        Parts = new string[]
                        {
                            meridianNames[meridianId],
                            dayPart
                        }
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// /// </summary>
        /// <param name="mmdd"></param>
        /// <returns></returns>
        ///<references>
        ///     c# - Check if dateTime is a weekend or a weekday - Stack Overflow: https://stackoverflow.com/questions/39715947/check-if-datetime-is-a-weekend-or-a-weekday
        ///     DateTime.ToShortTimeString Method (System) | Microsoft Docs: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.toshorttimestring?view=netframework-4.8
        ///</references>
        [HttpGet]
        public JsonResult GetIsWeekday(string mmdd)
        {
            DateTime requestedDate = new DateTime();
            try
            {
                mmdd = UnStringify(mmdd);
                int month = int.Parse(mmdd.Substring(0, 2));
                int day = int.Parse(mmdd.Substring(2, 2));
                requestedDate = new DateTime(DateTime.Now.Year, month, day);
                if (requestedDate.DayOfWeek != DayOfWeek.Saturday && requestedDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    return new JsonResult()
                    {
                        Data = new IvrResponseMessage()
                        {
                            SuccessResponseMessage = requestedDate.DayOfWeek.ToString()
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }

                throw new Exception();
            }
            catch
            {
                throw new Exception($"The requested date is on a weekend: {requestedDate.ToLongDateString()}");
            }
        }

        [HttpGet]
        public JsonResult GetRequestedDateMessage(string mmdd)
        {
            try
            {
                mmdd = UnStringify(mmdd);
                int month = int.Parse(mmdd.Substring(0, 2));
                int day = int.Parse(mmdd.Substring(2, 2));
                DateTime requestedDate =
                    new DateTime(DateTime.Now.Year, month, day);
                if (requestedDate.DayOfWeek != DayOfWeek.Saturday && requestedDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    return new JsonResult()
                    {
                        Data = new IvrResponseMessage()
                        {
                            SuccessResponseMessage = requestedDate.ToLongDateString(),
                            Parts = new string[] { requestedDate.ToLongDateString(), requestedDate.ToString("d") }
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    throw new Exception($"The requested date is on a weekend: {requestedDate.ToLongDateString()}");
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public JsonResult GetCountyIdMessage(int countyId)
        {
            try
            {
                var countyName = countyNames[countyId];
                if (!string.IsNullOrEmpty(countyName))
                    return new JsonResult()
                    {
                        Data = new IvrResponseMessage()
                        {
                            SuccessResponseMessage = $"{countyName} county",
                            Parts = new string[] { countyName }
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                else
                {
                    throw new Exception($"'{countyId}' is not a valid county selection.");
                }
            }
            catch
            {
                throw;
            }
        }

        //[HttpGet]
        //public JsonResult VerifyHhmm(string hhmm, string mmdd)
        //{
        //    try
        //    {
        //        int h = int.Parse(hhmm.Substring(0, 2));
        //        if (hhmm.Trim().Length > 1)
        //        {
        //            hhmm = UnStringify(hhmm);
        //            int hour = h <= 2 ? h += 12 : h;
        //            int minute = int.Parse(hhmm.Substring(2, 2));

        //            mmdd = UnStringify(mmdd);
        //            int month = int.Parse(mmdd.Substring(0, 2));
        //            int day = int.Parse(mmdd.Substring(2, 2));
        //        }


        //        DateTime[] appts = GetHourAvailableAppointmentDateTimes(hour: hour,
        //            requestedDate: new DateTime(year: 2020, month: month, day: day));

        //        return new JsonResult()
        //        {
        //            Data = new IvrResponseMessage()
        //            {
        //                SuccessResponseMessage = $"ok, {verifiedApptTime}"
        //            },
        //            JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //        };
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //}

        [HttpGet]
        public JsonResult GetRequestedAppointment(string hhmm, int countyId, string mmdd)
        {
            try
            {
                int h = int.Parse(hhmm.Substring(0, 2));
                hhmm = UnStringify(hhmm);
                int hour = h <= 2 ? h += 12 : h;
                int minute = int.Parse(hhmm.Substring(2, 2));

                mmdd = UnStringify(mmdd);
                int month = int.Parse(mmdd.Substring(0, 2));
                int day = int.Parse(mmdd.Substring(2, 2));

                bool apptFound = false;
                DateTime appointment = new DateTime();

                DateTime[] appts = GetHourAvailableAppointmentDateTimes(hour: hour,
                    requestedDate: new DateTime(year: 2020, month: month, day: day));

                if (hhmm.Trim().Length > 1)
               {
                    for (int i = 0; i < appts.Length; i++)
                    {
                        appointment = appts[i];
                        if (appointment == new DateTime(year: 2020, month: month,
                            day: day, hour: hour, minute: minute, second: 0))
                        {
                            i = appts.Length + 1;
                            apptFound = true;
                        }
                    }
                }
                else
                {
                    appointment = appts[h];
                    if (appointment > new DateTime())
                        apptFound = true;
                }
                if (apptFound)
                {
                    var dt2 = appointment;//GetSuggestedAppointmentDateTime(appointment);
                    int trainerId = trainerIds[countyId];
                    string trainerName = trainerNames[countyId];
                    string countyName = countyNames[countyId];
                    var a = new Appointment(trainerId, trainerName, countyName, countyId, dt2);
                    AddAppointmentEventToSpCalendar(a);

                    return new JsonResult()
                    {
                        Data = new IvrResponseMessage()
                        {
                            SuccessResponseMessage =
                                $"congrats! your {appointment.ToLongDateString()} {appointment.ToShortTimeString()} appointment has been submitted, and a DART representative will follow up with you with details."
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    throw new Exception("Appointment not found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddAppointmentEventToSpCalendar(Appointment a)
        {
            string siteUrl = "http://sp16.geek-ish.com", calendarName /*listName*/ = "Calendar1"/*"Customers"*/;
            string[] listFields = new string[] { "ID", "Title", "Start Time", "End Time"/*"FirstName", "LastName", "City", "State", "Body" */ };
            string camlXml = "<View><ViewFields><FieldRef Name='ID'/><FieldRef Name='Title'/><FieldRef Name='Start Time'/><FieldRef Name='Title'/>'FirstName'/><FieldRef Name='LastName'/><FieldRef Name='City'/><FieldRef Name='State'/><FieldRef Name='Body'/>*/</ViewFields></View>";
            camlXml = "<View><ViewFields><FieldRef Name='ID'/><FieldRef Name='Title'/><FieldRef Name='EventDate'/><FieldRef Name='EndDate'/></ViewFields></View>";
            System.Net.NetworkCredential creds = new System.Net.NetworkCredential("app4rent_sp16@geek-ish.com", "helloWorld55!");
            Microsoft.SharePoint.Client.ClientContext ctx = new Microsoft.SharePoint.Client.ClientContext(siteUrl) { Credentials = creds };
            Microsoft.SharePoint.Client.ListItemCollection lic = GetSpListItems(ctx, siteUrl, calendarName, camlXml);

            Microsoft.SharePoint.Client.ListItemCreationInformation lici = new Microsoft.SharePoint.Client.ListItemCreationInformation();
            Microsoft.SharePoint.Client.List list = ctx.Web.Lists.GetByTitle(calendarName);
            Microsoft.SharePoint.Client.ListItem li = list.AddItem(lici);

            //string[] trainerName = a.Trainer.Split(new char[] { ',' });
            //string trainer = string.Format("{0}. {1}", trainerName[1].Trim().Substring(0, 1), trainerName[0].Trim());
            //string apptTime = a.StartDateTime.ToString("hh:mm tt");

            li["Title"] = a.Title; /*string.Format("{0} {1}", trainer, apptTime);*///"Added " + DateTime.Now.ToString();
            li["County"] = a.County;/*"New Castle";*/
            li["CountyId"] = a.CountyId; /*1;*/
            li["Trainer"] = a.Trainer;/*"Fish, Harry";*/
                                      //li["TrainerId"] = 90;
            li["EventDate"] = a.StartDateTime;
            li["EndDate"] = a.EndDateTime;/*new DateTime(a.StartDateTime.Ticks + 3600000);*/
            li.Update();
            ctx.ExecuteQuery();
        }

        static Microsoft.SharePoint.Client.ListItemCollection GetSpListItems(Microsoft.SharePoint.Client.ClientContext ctx, string siteUrl, string listName, string camlXml)//string[] listFields)
        {
            Microsoft.SharePoint.Client.List list = ctx.Web.Lists.GetByTitle(listName);
            Microsoft.SharePoint.Client.CamlQuery camlQuery = new Microsoft.SharePoint.Client.CamlQuery() { ViewXml = camlXml };
            Microsoft.SharePoint.Client.ListItemCollection lic = list.GetItems(camlQuery);

            ctx.Load(lic);
            ctx.ExecuteQuery();
            return lic;
        }

        protected DateTime GetSuggestedAppointmentDateTime(DateTime appReceiveDate)
        {
            const long hour = 36000000000;
            DateTime dt = new DateTime(appReceiveDate.Ticks + (36 * hour));

            //!(doy > Mon and doy < Fri)
            for (DayOfWeek d = DayOfWeek.Sunday; d <= DayOfWeek.Saturday; d++)
            {
                if (!((dt.DayOfWeek >= DayOfWeek.Monday) && (dt.DayOfWeek < DayOfWeek.Friday)))
                    dt = new DateTime(dt.Ticks + (24 * hour));
            }
            return ScheduleAppointmentTime(dt);
        }

        protected DateTime ScheduleAppointmentTime(DateTime unscheduledApptTime)
        {
            const long hour = 36000000000;
            DateTime dt = unscheduledApptTime;// new DateTime(unscheduledApptTime.Ticks + 3600000);
            if (dt >= new DateTime(dt.Year, dt.Month, dt.Day, 14, 45, 0))
            {
                dt = new DateTime(dt.Year, dt.Month, dt.Day + 1, 9, 15, 0);
                //!(doy > Mon and doy < Fri)
                while (!((dt.DayOfWeek >= DayOfWeek.Monday) && (dt.DayOfWeek < DayOfWeek.Friday)))
                {
                    dt = new DateTime(dt.Ticks + (24 * hour));
                }
            }
            //!(dt > 915 and dt < 1445)
            while (!((dt >= new DateTime(dt.Year, dt.Month, dt.Day, 9, 15, 0)) &&
                (dt < new DateTime(dt.Year, dt.Month, dt.Day, 14, 45, 0))))
            {
                dt = new DateTime(dt.Ticks + 9000000000);
            }
            return dt;
        }

        // GET: Appointments
        public ActionResult Index()
        {
            return View(db.Appointments.ToList());
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
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
    }

    public partial class AppointmentsController
    {
        private readonly string[] countyNames = new string[] { "", "New Castle", "Kent", "Sussex" };
        private readonly string[] meridianNames = new string[] { "", "AM", "PM" };
        private readonly string[] dayParts = new string[] { "", "morning", "afternoon" };
        private readonly string[] trainerNames = new string[] { "", "Carter, Cean", "Irving, Tonya", "Wilhelm, Sharon" };
        private readonly int[] trainerIds = new int[] { 0, 20, 31, 10 };
    }
}

