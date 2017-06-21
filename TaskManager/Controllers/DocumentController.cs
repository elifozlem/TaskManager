using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TaskManager;

namespace TaskManager.Controllers
{
    [Auth.Auth("Admin")]
    public class DocumentController : Controller
    {
        private TaskManagerDbEntities db = new TaskManagerDbEntities();

      
        public ActionResult Index()
        {
          
            return View();
        }

        //veri tabanından gelen veri listesini tabloya döker
        public ActionResult MyChart()
        {
            var task = db.Sp_ListTask().ToList();
          
            var testChart = new Chart(width: 600, height: 400)
            .AddTitle("Test")
            .DataBindTable(dataSource: task, xField: "Score")
            .GetBytes("png");
            return File(testChart, "image/png");

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
