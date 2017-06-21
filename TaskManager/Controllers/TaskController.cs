using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace TaskManager.Controllers
{
    public class TaskController : Controller
    {
        private TaskManagerDbEntities db = new TaskManagerDbEntities();

        //Görevler veri tabanından getirilir. Eğer userid dolu gelirse bu kullanıcı çalışandır 
        //çalışana ait ilk 5 öncelikli görev yan tabda gösterilir (ViewBag.Priority)
        //başarılı ilk 3 personel gösterilir (ViewBag.Personnel)
        [Auth.Auth("Admin", "User")]
        public ActionResult GetTask(Guid? userid)
        {
         
            ViewBag.Personnel = db.Sp_Personnel().ToList();

            if (userid != null && Session["Control"].ToString() == "0")
            {
                ViewBag.Priority = db.Sp_Priority(userid).ToList();
                var utask = db.Task.Include(t => t.TaskPriority).Include(t => t.TaskType1).Include(t => t.TUser).Include(t => t.TAdmin).Include(t => t.Project).Where(t => t.Status == true && t.UserID == userid).OrderByDescending(m => m.CompletedDate);
                return View(utask.ToList());
            }
            else
            {
              var atask = db.Task.Include(t => t.TaskPriority).Include(t => t.TaskType1).Include(t => t.TUser).Include(t => t.TAdmin).Include(t => t.Project).Where(t => t.Status == true).OrderByDescending(m => m.CompletedDate);
                return View(atask.ToList());
            }
           
        }

      

        [Auth.Auth("Admin", "User")]
        public ActionResult TaskDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Task.Find(id);

            if (task == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskID = task.TaskID;
            return View(task);
        }

        //Görev oluştur get
        [Auth.Auth("Admin")]
        public ActionResult CreateTask(Guid? id,string name)
        {
            ViewBag.ProjectName = new SelectList(db.Project, "ProjectID", "ProjectName");
            ViewBag.Priority = new SelectList(db.TaskPriority, "PriorityID", "PriorityName");
            ViewBag.TaskType = new SelectList(db.TaskType, "TypeID", "TypeName");
            ViewBag.UserID = new SelectList(db.TUser, "UserID", "Username");
            ViewBag.ManagerName = name;
            ViewBag.ManagerID = id;
            return View();
        }

        //görev oluşturulur. Tarih alanları ve görev puanı boş bırakılamaz
        //dosya ekle butonuyla dosyalar seçildikten sonra yükle butonuyla dosya kayıt işlemi şu an çalışmıyor.
        [Auth.Auth("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask([Bind(Include = "ProjectName,TaskType,Summary,Priority,SDate,EDate,UserID,Score,Description,FileUrl,TaskID,Manager")] Task task,Guid id)
        {
            if (ModelState.IsValid)
            {
                task.Manager = id;
                task.Status = true;
                
                db.Task.Add(task);
                db.SaveChanges();
                return RedirectToAction("GetTask");
            }
            ViewBag.ProjectName = new SelectList(db.Project, "ProjectID", "ProjectName",task.ProjectName);
            ViewBag.Priority = new SelectList(db.TaskPriority, "PriorityID", "PriorityName", task.Priority);
            ViewBag.TaskType = new SelectList(db.TaskType, "TypeID", "TypeName", task.TaskType);
            ViewBag.UserID = new SelectList(db.TUser, "UserID", "Username", task.UserID);
            ViewBag.Manager = new SelectList(db.TAdmin, "ManagerID", "Username", task.Manager);
            return View(task);
        }

        //Göreve yorum eklemek için
        [Auth.Auth("Admin", "User")]
        public ActionResult Edit(int? taskid)
        {
            if (taskid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Task.Find(taskid);
            if (task == null)
            {
                return HttpNotFound();
            }
            ViewData["CommentList"] = db.Comment.Where(m => m.TaskID == taskid).OrderByDescending(t=>t.CommentDate).ToList();
          
            ViewBag.UserID = task.UserID;
            ViewBag.TaskID = taskid;
            return View(task);
        }

        //Yorum ekle post
        [Auth.Auth("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask(int id,Task task)
        {
            Task tasks = db.Task.Find(id);
            tasks.Score = task.Score;
            tasks.Status = false;
            db.Task.Attach(tasks);
            var entry = db.Entry(tasks);
            entry.Property(e => e.Score).IsModified = true;
            entry.Property(e => e.Status).IsModified = true;
            db.SaveChanges();
           return RedirectToAction("Edit", "Task",new {taskid=id });
        }


        [Auth.Auth("User")]
        [HttpPost]
        public ActionResult Comment(int TaskID, string textAnswer, Guid? id)
        {
            if (textAnswer != null)
            {
                Comment com = new Comment()
                {
                    CommentContent = textAnswer,
                    TaskID = TaskID,
                    UserID = id,
                    CommentDate = DateTime.UtcNow

                };
                db.Comment.Add(com);
                db.SaveChanges();
            }
            return RedirectToAction("Edit",new { taskid = TaskID });
        }
      

        //Görev silmek için(henüz görevi kapatma fonksiyoneli mevcut değil)
        [Auth.Auth("Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Task.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        [Auth.Auth("User")]
        public ActionResult Completed(Guid? id,int? TaskID)
        {

            Task tasks = db.Task.Find(TaskID);
            tasks.Completed =true;
           
            db.Task.Attach(tasks);
            var entry = db.Entry(tasks);
            entry.Property(e => e.Completed).IsModified = true;

            db.SaveChanges();
            return RedirectToAction("GetTask", "Task", new { userid = id });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Task.Find(id);
            db.Task.Remove(task);
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
