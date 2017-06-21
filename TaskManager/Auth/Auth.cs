using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TaskManager.Auth
{
    public class Auth : AuthorizeAttribute
    {
        private readonly string[] allowedroles;
        private TaskManagerDbEntities db = new TaskManagerDbEntities();
        private bool authorize = false;
        public Auth(params string[] roles)
        {
            this.allowedroles = roles;

        }
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
            authorize = false;  
            foreach (var role in allowedroles)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var userid = ticket.Name.Split('|');
                Guid UserID = new Guid();
                Guid.TryParse(userid[0], out UserID);
               
                   
                if (role == "Admin")
                {
                    var admin = db.TAdmin.Find(UserID);
                    if (admin != null)
                    {
                        authorize = true;
                    }
                   


                }
                else if (role == "User")
                {
                    var user = db.TUser.Find(UserID);

                    if (user != null)
                    {
                        authorize = true;
                    }
                   

                }


            }
             return authorize;
            }
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    base.OnAuthorization(filterContext);

        //    if (!authorize)
        //    {
        //        filterContext.Controller.TempData.Add("RedirectReason", "Login");
        //    }
        //}
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        

    }
}