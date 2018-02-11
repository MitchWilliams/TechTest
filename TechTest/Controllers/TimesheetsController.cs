using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TechTest.DAL;
using TechTest.Models;


namespace TechTest.Controllers
{
    public struct DateRange
    {
        public DateTime startDate;
        public DateTime endDate;
    }

    public class TimesheetsController : Controller
    {
        private TimesheetContext db = new TimesheetContext();

        public List<string> getListOfDays (DateTime startDate, DateTime endDate)
        {
            List<string> dayList = new List<string>();
            string day;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                day = date.ToShortDateString();
                dayList.Add(day);

            }

            return dayList;
        }

        public int calculateTimesheetsRequired (DateTime startDate, DateTime endDate, PlacementType placementType)
        {
            if (placementType == PlacementType.Weekly)
            {


                int weeks = 0;

                while (startDate != endDate)
                {
                    if (startDate.DayOfWeek == DayOfWeek.Sunday)
                        weeks++;

                    startDate = startDate.AddDays(1);
                }

                return weeks+1;
            }
            else
            {
                int months = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;

                return Math.Abs(months)+1;
            }

        }

        public string generateTimesheetTitle (string candidateName, string clientName, DateTime startDate, DateTime endDate)
        {

            string title = "TSREF:" + candidateName.ToUpper().Replace(" ", string.Empty) + clientName.ToUpper().Replace(" ", string.Empty) +
                startDate.ToShortDateString() + "-" + endDate.ToShortDateString();

            return title;
        }

        public List<DateRange> getMonthlyTimesheetDateRanges(DateTime startDate, DateTime endDate, int noOfTimesheets)
        {
            List<DateRange> dateRangeList = new List<DateRange>();
            DateTime monthStartDate = startDate;
            DateTime monthEndDate = endDate;
            
        
            for (int i = 0; i < noOfTimesheets; i++)
            {
                DateRange dr = new DateRange();

                if (i == 0)
                {
                    dr.startDate = monthStartDate;
                }
                else
                {
                    monthStartDate = monthEndDate.AddDays(1);
                    dr.startDate = monthStartDate;
                }

                if (endDate.Month == monthStartDate.Month)
                {
                    monthEndDate = endDate;
                    dr.endDate = monthEndDate;
                }
                else
                {
                    monthEndDate = new DateTime(monthStartDate.Year, monthStartDate.Month,
                        DateTime.DaysInMonth(monthStartDate.Year, monthStartDate.Month));

                    dr.endDate = monthEndDate;
                }

                dateRangeList.Add(dr);
            }

            return dateRangeList;
        }

        public List<DateRange> getWeeklyTimesheetDateRanges (DateTime startDate, DateTime endDate, int noOfTimesheets)
        {
            List<DateRange> dateRangeList = new List<DateRange>();

            DateTime weekStartDate = startDate;
            DateTime weekEndDate = endDate;
            DateTime currentDay;

            for (int i = 0; i < noOfTimesheets; i++)
            {
                DateRange dr = new DateRange();

                if (i == 0)
                {
                    dr.startDate = weekStartDate;
                }
                else
                {
                    weekStartDate = weekEndDate.AddDays(1);
                    dr.startDate = weekEndDate.AddDays(1);
                }

                currentDay = weekStartDate;
                while (currentDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (currentDay == endDate)
                    {
                        weekEndDate = currentDay;
                        break;
                    }
                    else
                        currentDay = currentDay.AddDays(1);

                    if (currentDay.DayOfWeek == DayOfWeek.Sunday)
                        weekEndDate = currentDay;

                }

                dr.endDate = weekEndDate;


                dateRangeList.Add(dr);
            }
            return dateRangeList;

        }

        // GET: Timesheets
        public ActionResult Index(int? jobId)
        {
            if (jobId != null)
            {
                var tsDB = db.Timesheet.
                    Where(s => s.timesheetJob == jobId).ToList();

                return View(tsDB);

                
            }
            return View(db.Timesheet.ToList());
        }

        // GET: Timesheets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Timesheet timesheet = db.Timesheet.Find(id);
            if (timesheet == null)
            {
                return HttpNotFound();
            }

            ViewBag.ListOfDays = getListOfDays(timesheet.startDate, timesheet.endDate);
            return View(timesheet);
        }

        // GET: Timesheets/Create
        public ActionResult GenerateTimesheets()
        {

            return View();
        }

        // POST: Timesheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateTimesheets([Bind(Include = "timesheetId,candidateName,clientName,jobTitle,startDate,endDate,placementType")] Timesheet timesheetForm)
        {

            if (timesheetForm.endDate < timesheetForm.startDate)
            {
                ModelState.AddModelError(string.Empty,
                    "The End Date must be after the Start Date.");
            }

            if (ModelState.IsValid)
            {

                var timesheetHistory = from s in db.Timesheet
                                       select s;

                if (timesheetHistory.Count() == 0)
                {
                    timesheetForm.timesheetJob = 1;
                }
                else
                {

                    var latestTimesheet = db.Timesheet
                        .OrderByDescending(s => s.timesheetId)
                        .FirstOrDefault();

                    timesheetForm.timesheetJob = latestTimesheet.timesheetJob + 1;
                }
 
;
                //Calculate number of timesheets required
                int noOfTimesheets = calculateTimesheetsRequired(timesheetForm.startDate, timesheetForm.endDate, timesheetForm.placementType);
                //Determine the break in weeks/months and create an array of date ranges
                List<DateRange> dateRangeList = new List<DateRange>();
                if (timesheetForm.placementType == PlacementType.Weekly)
                {
                    dateRangeList = getWeeklyTimesheetDateRanges(timesheetForm.startDate, timesheetForm.endDate,
                    noOfTimesheets);
                }
                else
                {
                    dateRangeList = getMonthlyTimesheetDateRanges(timesheetForm.startDate, timesheetForm.endDate,
                    noOfTimesheets);
                }
                
                //Loop through date range and call create timesheet function
                foreach (DateRange dr in dateRangeList)
                {
                    timesheetForm.startDate = dr.startDate;
                    timesheetForm.endDate = dr.endDate;
                    timesheetForm.timesheetTitle = generateTimesheetTitle(timesheetForm.candidateName, timesheetForm.clientName, 
                        dr.startDate, dr.endDate);

                    db.Timesheet.Add(timesheetForm);
                    db.SaveChanges();

                  

                }
                return RedirectToAction("Index", new { jobId = timesheetForm.timesheetJob });

                
            }

            
            return View(timesheetForm);
        }

   
        // GET: Timesheets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Timesheet timesheet = db.Timesheet.Find(id);
            if (timesheet == null)
            {
                return HttpNotFound();
            }
            return View(timesheet);
        }

        // POST: Timesheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Timesheet timesheet = db.Timesheet.Find(id);
            db.Timesheet.Remove(timesheet);
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
}
