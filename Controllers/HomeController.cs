using EffortSheet.App_Data;
using EffortSheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace EffortSheet.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var loginUser = Auth.GetUser();

            var userId = Auth.GetID();

            if (!userId.Contains(loginUser))
            {
                return RedirectToAction("Authentication");
            }


            IEnumerable<EffortModel> lst = null;
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    IEnumerable<NameModel> names = db.NameList.ToList();
                    ViewBag.nameList = names;
                    IEnumerable<ActivityModel> activities = db.ActivityList.ToList();
                    ViewBag.activityList = activities;
                    IEnumerable<TeamModel> teams = db.TeamList.ToList();
                    ViewBag.teamList = teams;
                    IEnumerable<PriorityModel> priorities = db.PriorityList.ToList();
                    ViewBag.priorityList = priorities;

                    lst = db.EffortTracker.ToList();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (lst != null)
                return View(lst);
            else
                return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Add(EffortModel obj)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    db.EffortTracker.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

        }

        [HttpPost]
        public ActionResult Edit(EffortModel obj)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var Effort = db.EffortTracker.Find(obj.Id);
                    Effort.DateOfActivity = obj.DateOfActivity;
                    Effort.Name = obj.Name;
                    Effort.Activity = obj.Activity;
                    Effort.Description = obj.Description;
                    Effort.Reference = obj.Reference;
                    Effort.Priority = obj.Priority;
                    Effort.Hours = obj.Hours;
                    Effort.ForwardedTeam = obj.ForwardedTeam;

                    //db.EffortTracker.Update(Effort);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }

        [HttpPost]
        public ActionResult Delete(EffortModel obj)
        {
            try
            {
                int? id = obj.Id;

                if (id == 0 || id == null)
                {
                    return HttpNotFound();
                }

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    EffortModel Effort = db.EffortTracker.Find(id);

                    db.EffortTracker.Remove(Effort);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public ActionResult Filter()
        {
            var loginUser = Auth.GetUser();

            var userId = Auth.GetID();

            if (!userId.Contains(loginUser))
            {
                return RedirectToAction("Authentication");
            }

            IEnumerable<EffortModel> lst = null;
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    IEnumerable<NameModel> names = db.NameList.ToList();
                    ViewBag.nameList = names;
                    IEnumerable<ActivityModel> activities = db.ActivityList.ToList();
                    ViewBag.activityList = activities;
                    IEnumerable<TeamModel> teams = db.TeamList.ToList();
                    ViewBag.teamList = teams;
                    IEnumerable<PriorityModel> priorities = db.PriorityList.ToList();
                    ViewBag.priorityList = priorities;

                    var Filter = db.FilterData.Find(1);


                    if (Filter.startDate != null && Filter.endDate != null)
                    {
                        if (Filter.Name == null)
                        {
                            lst = db.EffortTracker.Where(o => o.DateOfActivity >= DateTime.Parse(Filter.startDate) && o.DateOfActivity <= DateTime.Parse(Filter.endDate)).ToList();
                        }
                        else
                        {
                            lst = db.EffortTracker.Where(o => o.DateOfActivity >= DateTime.Parse(Filter.startDate) && o.DateOfActivity <= DateTime.Parse(Filter.endDate) && o.Name == Filter.Name).ToList();
                        }
                    }
                    else
                    {
                        if (Filter.Name == null)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            lst = db.EffortTracker.Where(o => o.Name == Filter.Name).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (lst != null)
                return View(lst);
            else
                return HttpNotFound();

        }

        [HttpPost]
        public ActionResult PostFilter(FilterModel obj)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var Filter = db.FilterData.Find(obj.Id);
                    Filter.Name = obj.Name;
                    Filter.startDate = obj.startDate;
                    Filter.endDate = obj.endDate;

                    //_db.FilterData.Update(Filter);
                    db.SaveChanges();
                    return RedirectToAction("Filter");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

        }

        public ActionResult Download()
        {
            IEnumerable<EffortModel> obj = null;
            StringBuilder builder = new StringBuilder();
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    obj = db.EffortTracker.ToList().OrderBy(x => x.Name);
                    builder.AppendLine("Year,Month,Date,Name,Tower,Activity,Description,Reference,Priority,Hours Spent,Forwarded Team");
                    foreach (var item in obj)
                    {
                        builder.AppendLine($"{item.DateOfActivity.Year},{item.DateOfActivity.ToString("MMMM")},{item.DateOfActivity.Day}," +
                                            $"{item.Name},{item.Tower},{item.Activity}," +
                                                $"{item.Description},{item.Reference},{item.Priority},{item.Hours},{item.ForwardedTeam}");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "EffortTracker_" + DateTime.Now.ToString("MMM") + ".csv");
        }

        public ActionResult FilterDownload()
        {
            IEnumerable<EffortModel> lst = null;
            StringBuilder builder = new StringBuilder();
            FilterModel Filter = null;
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Filter = db.FilterData.Find(1);


                    if (Filter.startDate != null && Filter.endDate != null)
                    {
                        if (Filter.Name == null)
                        {
                            lst = db.EffortTracker.Where(o => o.DateOfActivity >= DateTime.Parse(Filter.startDate) && o.DateOfActivity <= DateTime.Parse(Filter.endDate)).ToList();
                        }
                        else
                        {
                            lst = db.EffortTracker.Where(o => o.DateOfActivity >= DateTime.Parse(Filter.startDate) && o.DateOfActivity <= DateTime.Parse(Filter.endDate) && o.Name == Filter.Name).ToList();
                        }
                    }
                    else
                    {
                        if (Filter.Name != null)
                        {
                            lst = db.EffortTracker.Where(o => o.Name == Filter.Name).ToList();
                        }
                    }

                    builder.AppendLine("Year,Month,Date,Name,Tower,Activity,Description,Reference,Priority,Hours Spent,Forwarded Team");
                    foreach (var item in lst)
                    {
                        builder.AppendLine($"{item.DateOfActivity.Year},{item.DateOfActivity.ToString("MMMM")},{item.DateOfActivity.Day}," +
                                            $"{item.Name},{item.Tower},{item.Activity}," +
                                                $"{item.Description},{item.Reference},{item.Priority},{item.Hours},{item.ForwardedTeam}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "EffortTracker_" + Filter.Name + "_" + DateTime.Now.ToString("MMM") + ".csv");
        }

        public ActionResult Authentication()
        {
            return View();
        }
    }
}