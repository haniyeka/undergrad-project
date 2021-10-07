using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using my_pro.BussinessLogic;
using my_pro.Models;
using my_pro.Models.BussinessTypes;
namespace my_pro.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            HomePageModel model = new HomePageModel();
            long id = new long();
            id = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
            model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), id);
            return View(model);
        }
        
        public ActionResult sampla()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }
    }
}