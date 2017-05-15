using LeaRun.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeaRun.WebApp.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/
        public Base_User currentUser { get; set; }
        public BaseController()
        {
            currentUser = (Base_User)System.Web.HttpContext.Current.Session["User"];
        }

    }
}
