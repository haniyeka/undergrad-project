using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using my_pro.Models;
using RayanShafagh.SecurityProvider;
using RayanShafagh.SecurityProvider.Encryption;
using my_pro.BussinessLogic;
using System.Activities.Statements;

namespace my_pro.Controllers
{
    public class AdminPanelController : Controller
    {
        // GET: AdminPanel
        [HttpGet]
        public ActionResult Index()
        {
            Session.Add("isLoggedIn", "false");
            if (Session["isLoggedIn"].ToString() == "true")
            {
                return View("Panel");
            }
            else
                return View("Login");
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var user = db.Admins.Where(a => a.Username == username).First();
                var pack = Hash.ComputeHash(password, user.PasswordSalt, HashAlgorithm.SHA512);
                if (!pack.HashBytes.SequenceEqual(user.PasswordHash)) { return HttpNotFound(); }
                Session[MyStateVariables.LoggedIn] = true;
                Session[MyStateVariables.UserID] = user.Pk_ID;
                Session[MyStateVariables.Username] = user.Username;
                return View("Panel");
            }
        }
    }
}