using Microsoft.Owin.Security;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TaskManager.Controllers
{
    public class LoginController : Controller
    {
        private TaskManagerDbEntities db = new TaskManagerDbEntities();

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //çalışanlar için giriş panelidir. öncelikle böyle bir kullanıcı var mı diye veri tabanında aranır
        //varsa şifre ve kullanıcı adı eşleşmesini sağlıyor mu diye bakılır.
        //başarılı bir şekilde giriş yaparsa idsi ve ismi cookie olarak alınır.
        //bu bilgiler yetkilendirmede kullanılmıştır (bkz. Auth.Auth())
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(TUser user)
        {
            if (ModelState.IsValid)
            {
                var rp = db.TUser.Where(m => m.Username == user.Username).ToList();
                if (rp.Count == 1)
                {
                    var result = db.Sp_Login(user.Username, user.Password).ToList();
                    if (result.Count == 1)
                    {
                        FormsAuthentication.SetAuthCookie(result[0].UserID.ToString()+"|"+result[0].Username.ToString(), true);
                        Session["ID"] = result[0].UserID;
                        Session["Control"] = 0;
                        return RedirectToAction("GetTask", "Task", new { userid = result[0].UserID });
                    }

                    else
                    {
                        ModelState.AddModelError("", "Şifre hatalı!");
                    }
                }
                else
                    ModelState.AddModelError("", "Böyle bir kullanıcı bulunmamaktadır.");
 
            }
            return View(user);
        }


        [AllowAnonymous]
        public ActionResult Admin()
        {
            return View();
        }


        //Admin giriş sayfası
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Admin(TAdmin auser)
        {
            if (ModelState.IsValid)
            {
                var rp = db.TAdmin.Where(m => m.Username == auser.Username).ToList();
                if (rp.Count == 1)
                {
                    var result = db.Sp_AdminLogin(auser.Username,auser.Password).ToList();
                    if (result.Count == 1)
                    {
                        FormsAuthentication.SetAuthCookie(result[0].ManagerID.ToString()+"|"+result[0].Username.ToString(), true);
                        Session["ID"] = result[0].ManagerID;
                        Session["ManagerName"] = result[0].Username;
                        Session["Control"] = 1;
                        return RedirectToAction("Index", "Project",new { id = result[0].ManagerID,name=result[0].Username });
                    }

                    else
                    {
                        ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
                    }
                }
                else
                    ModelState.AddModelError("", "Yetkisiz giriş.");
            }
            return View(auser);
        }
        public ActionResult RedirectReason()
        {
            TempData["notice"] = "Yetkisiz Giriş!";
            // return Content("<script language='javascript' type='text/javascript'>alert('Yetkisiz giriş!'); window.location.reload(false); </script>");
            return RedirectToAction("GetTask", "Task");
        }

        //Çıkış yap butonuna basılınca bu fonksiyon aktif olur kullanıcıya ait geçici bilgiler silinir
        [HttpPost]
        [Auth.Auth("Admin", "User")]
        public ActionResult LogOff()
        {
            //IAuthenticationManager authManager = HttpContext.GetOwinContext().Authentication;
            // authManager.SignOut();
            //  //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.User =
                new GenericPrincipal(new GenericIdentity(string.Empty), null);
            return RedirectToAction("Login", "Login");
        }


    }
}