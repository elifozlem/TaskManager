using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace TaskManager.Controllers
{

    [Auth.Auth("Admin")]
    public class ProjectController : Controller
    {
        private TaskManagerDbEntities db = new TaskManagerDbEntities();


        /*Arama textboxında kelimeyi otomatik tamamlaması için*/
        [HttpPost]
        public ActionResult Autocomplete(string key)
        {

            var sonuc = (from r in db.Project
                         where r.ProjectName.ToLower().Contains(key.ToLower())
                         select new { r.ProjectName }).Distinct();
          
            return this.Json(sonuc,
                       JsonRequestBehavior.AllowGet);/*json tipinde döner*/

        }
  
        //Projelerin tamamını listeler. id ve name admin idsi ve ismidir.
        public ActionResult Index(Guid? id,string name)
        {

            var project = db.Project.Include(p => p.TAdmin);
            ViewBag.ManagerID = id;
            ViewBag.ManagerName = name;
            return View(project.ToList());
        }

       //Post işlemleri için. Arama yapmak için entera basarsa veri tabanında o projeyi aratıp sonucu döner
        [HttpPost]
        public ActionResult Index(string Search,Guid id,string name)
        {

            var sonuc = db.Project.Where(m => m.ProjectName.Contains(Search)).ToList();
            ViewBag.ManagerID = id;
            ViewBag.ManagerName = name;
            return View(sonuc);


        }
     
        //proje oluştur get işlemleri
        public ActionResult Create(Guid ManagerId,string ManagerName)
        {
            ViewBag.ManagerId =ManagerId;
            ViewBag.ManagerName = ManagerName;
          
            return View();
        }

        //proje oluşturulur managerid ve managername adminin başka admin adına proje oluşturmaması içindir. 
        //Admin sadece kendi adıyla proje oluşturabilir.
         [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectName,ProjectKey,ProjectManager,ProjectID")] Project project,Guid managerid,string managername)
        {
            project.ProjectManager = managerid;
            if (ModelState.IsValid)
            {
                db.Project.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index",new { id = managerid, name = managername });
            }

            ViewBag.ProjectManager = new SelectList(db.TAdmin, "ManagerID", "Username", project.ProjectManager);
            return View(project);
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
