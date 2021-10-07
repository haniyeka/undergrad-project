using my_pro.BussinessLogic;
using my_pro.Models;
using my_pro.Models.BussinessTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace my_pro.Controllers
{
    public class BooksController : Controller
    {
        // GET: Books
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Cat(long id)
        {
            HomePageModel model = new HomePageModel();
            long usid = new long();
            usid = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
            model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), usid);
            model.Books = BookModel.GetBooksByCategory(id);
            return View(model);

        }

        [HttpGet]
        public ActionResult Detail(long id)
        {
            HomePageModel model = new HomePageModel();
            long usid = new long();
            usid = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
            model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), usid);
            BookModel book = BookModel.GetBookInfos(id);
            model.Books = new List<BookModel>() { book };
            return View(model);
        }
        [HttpPost]
        public ActionResult RateBook(long BookID, byte RateVal)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                long memberid = Convert.ToInt64(Session[MyStateVariables.UserID]);
                var rate = db.BookRates.ToList().Where(a => a.Fk_BookID == Convert.ToInt64(BookID) && a.Fk_memberID == memberid).FirstOrDefault();
                if (rate == null)
                {
                    BookRate RATE = new BookRate()
                    {
                        Fk_BookID = Convert.ToInt64(BookID),
                        Date = DateTime.Now,
                        Fk_memberID = memberid,
                        Rate = Convert.ToByte(RateVal)
                    };
                    db.BookRates.Add(RATE);
                    db.SaveChanges();
                }
                else
                {
                    rate.Rate = Convert.ToByte(RateVal);
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Favorite(long BookID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                long memberid = Convert.ToInt64(Session[MyStateVariables.UserID]);
                var fav = db.Favorites.ToList().Where(a => a.Fk_BookID == Convert.ToInt64(BookID) && a.Fk_memberID == memberid).FirstOrDefault();
                if (fav == null)
                {
                    Favorite FAV = new Favorite()
                    {
                        Fk_BookID = Convert.ToInt64(BookID),
                        Date = DateTime.Now,
                        Fk_memberID = memberid
                    };
                    db.Favorites.Add(FAV);
                    db.SaveChanges();
                }
                else
                {
                    db.Favorites.Remove(fav);
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult AddComment(string id,string Comment)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                Comment com = new Comment()
                {
                    Comment1 = Comment,
                    Fk_BookID = Convert.ToInt64(id),
                    Date = DateTime.Now,
                    Fk_memberID = Convert.ToInt64(Session[MyStateVariables.UserID])
                };
                db.Comments.Add(com);
                db.SaveChanges();
            }
            return RedirectToAction("Detail", "Books",new { id = Convert.ToInt64(id) });
        }

        [HttpPost]
        public ActionResult RemoveComment(long bookID , string commentID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var com = db.Comments.ToList().Where(a => a.Pk_CommentID == Convert.ToInt64(commentID)).FirstOrDefault();
                db.Comments.Remove(com);
                db.SaveChanges();
                return RedirectToAction("Detail", "Books", new { id = Convert.ToInt64(bookID) });
            }
        }

        [HttpPost]
        public ActionResult Search(string SearchKeyword)
        {
            HomePageModel model = new HomePageModel();
            long usid = new long();
            usid = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
            model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), usid);
            model.Books = BookModel.SearchbyKeyword(SearchKeyword);
            return View("Cat",model);
        }

        [HttpPost]
        public ActionResult AdvanceSearch(string title, string writer, string translator, string publisher)
        {
            HomePageModel model = new HomePageModel();
            long usid = new long();
            usid = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
            model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), usid);
            model.Books = BookModel.AdvanceSearch(title, writer, translator, publisher);
            return View("Cat", model);
        }

        [HttpPost]
        public ActionResult AddtoBasket(long bookid , int bookcount)
        {
            long usid = Convert.ToInt64(Session[MyStateVariables.UserID].ToString());
            bool flag = BookModel.AddtoBasket(bookid,usid, bookcount);
            long id = bookid;
            return Redirect("Detail/"+ id);
            
        }

        [HttpPost] 
        public ActionResult DeleteInvoice(long invoiceid)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var i = db.InvoiceInfoes.Where(a => a.Pk_InvoiceID == invoiceid).FirstOrDefault();
                db.InvoiceInfoes.Remove(i);
                db.SaveChanges();
                return Redirect("/Home/Index");
            }
        }
    }
}