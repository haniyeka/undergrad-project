// Author: Haniye Kashgarani
// ORCID: 0000-0001-6059-1920
// Github: haniyeka

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using my_pro.Models;
using my_pro.Models.BussinessTypes;
using RayanShafagh.SecurityProvider.Encryption;
using RayanShafagh.SecurityProvider;
using my_pro.BussinessLogic;
using System.Transactions;
using System.Data.SqlClient;

namespace my_pro.Controllers
{
    public class UserAccountController : Controller
    {
        // GET: UserAccount
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var user = db.Members.Where(a => a.UserName == username).FirstOrDefault();
                if (user != null)
                {
                    var pack = Hash.ComputeHash(password, user.PassSalt, HashAlgorithm.SHA512);
                    if (!pack.HashBytes.SequenceEqual(user.PassHash)) { return View("NotFoundError"); }
                    Session[MyStateVariables.LoggedIn] = true;
                    Session[MyStateVariables.UserID] = user.PK_MemberID;
                    Session[MyStateVariables.Username] = user.UserName;
                   

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("NotFoundError");
                }
            }
        }

        [HttpPost]
        public ActionResult Register(UserAccount newuser)
        {
            bool flag = UserAccount.Register(newuser);
            if (flag)
            {
                HomePageModel model = new HomePageModel();
                long id = new long();
                id = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
                model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), id);
                return View(model);
            }
            else
            {
                return View("NotFoundError");
            }
        }

        
        public ActionResult Logout()
        {
            Session[MyStateVariables.LoggedIn] = false;
            Session[MyStateVariables.UserID] = -1;
            Session[MyStateVariables.Username] = "";
            return RedirectToAction("Index", "Home");
        }

       
        [HttpGet]
        public ActionResult UserProfile()
        {
            if (Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) != false)
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    HomePageModel home = new HomePageModel();
                    long id = new long();
                    id = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
                    home = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), id);
                    if(home.User.MemberType == 1)
                    {
                        return View(home);
                    }
                    else if(home.User.MemberType == 2)
                    {
                        return View("SellerProfile", home);
                    }
                    else
                        return View("NotFoundError");
                }
            }
            else
            {
                return View("NotFoundError");
            }
        }

        [HttpPost] 
        public ActionResult UpdateProfile(UserInfo UserInfo)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    try
                    {
                        long usid = Convert.ToInt64(Session[MyStateVariables.UserID].ToString());
                        var member = db.Members.Where(a => a.PK_MemberID == usid).FirstOrDefault();
                        var person = db.People.Where(a => a.PK_PersonID == member.FK_PersonID).FirstOrDefault();
                        long? addressid = null;
                        long? ImageID = null;
                        #region image
                        if (UserInfo.Image!= null)
                        {
                            string pic = System.IO.Path.GetFileName(UserInfo.Image.FileName);
                                string path = System.IO.Path.Combine(
                                                       Server.MapPath("~/Content/Images"), pic);
                                // file is uploaded
                                UserInfo.Image.SaveAs(path);
                                Image image = new Image()
                                {
                                    Path = "/Content/Images/" + pic,
                                    TemporaryName = pic,
                                    Uploaddate = DateTime.Now
                                };
                                db.Images.Add(image);
                                db.SaveChanges();
                            ImageID = image.PK_ImageID;
                        }
                        #endregion

                        person.Fname = UserInfo.Fname;
                        person.Lname = UserInfo.Lname;
                        person.gender = UserInfo.Gender;
                        person.FK_ImageID = ImageID;
                        member.Description = UserInfo.Description;
                        if (UserInfo.Birthdate != null)
                        {
                            person.Birthday = Utility.ConvertToGregorianDate(UserInfo.Birthdate);
                        }
                            if (UserInfo.Address != null)
                        {
                            Address address = new Address()
                            {
                                AddressLine = UserInfo.Address,
                                PostalCode = UserInfo.PostalCode
                            };
                            Province p = new Province();
                            if (UserInfo.Province != null)
                            {
                                var province = db.Provinces.Where(a => a.ProvinceName.Trim() == UserInfo.Province.Trim()).FirstOrDefault();
                                if (province != null)
                                {
                                    p = province;
                                    address.FK_ProvinceID = p.PK_ProvinceID;
                                }
                                else
                                {
                                    p.ProvinceName = UserInfo.Province.Trim();
                                    db.Provinces.Add(p);
                                    db.SaveChanges();

                                    address.FK_ProvinceID = p.PK_ProvinceID;
                                }
                            }
                            City c = new City();
                            if(UserInfo.City !=null)
                            {
                                var city = db.Cities.Where(a => a.CityName.Trim() == UserInfo.City.Trim()).FirstOrDefault();
                                if(city!=null)
                                {
                                    c = city;
                                    address.FK_CityID = c.PK_CityID;
                                }
                                else
                                {
                                    c.CityName = UserInfo.City.Trim();
                                    db.Cities.Add(c);
                                    db.SaveChanges();

                                    address.FK_CityID= c.PK_CityID;
                                }
                            }
                            db.Addresses.Add(address);
                            db.SaveChanges();
                            addressid = address.PK_AddressID;
                        }
                        person.FK_AddressID = addressid;
                        db.SaveChanges();
                        scope.Complete();

                        return Redirect("UserProfile");
                    }
                    catch (SqlException e)
                    {
                        Transaction.Current.Rollback();

                        return Redirect("UserProfile");
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult AddSellerBook(BooksModel Bookinfo)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    try
                    {
                        #region publicationCheck
                        long publicationID = 0;
                        if (Bookinfo.publication != null)
                        {
                            var existingpub = db.Publications.ToList().Where(a => a.PublicationName == Bookinfo.publication).FirstOrDefault();
                            if (existingpub != null)
                            {
                                publicationID = existingpub.PK_PublicationID;
                            }
                            else
                            {
                                Publication newpub = new Publication()
                                {
                                    PublicationName = Bookinfo.publication
                                };
                                db.Publications.Add(newpub);
                                db.SaveChanges();
                                existingpub = db.Publications.ToList().Where(a => a.PublicationName == Bookinfo.publication).FirstOrDefault();
                                publicationID = existingpub.PK_PublicationID;
                            }
                        }
                        #endregion

                        #region AgeCategory
                        long agecategoryID = 0;
                        if (Bookinfo.AgeCategoryName !=null)
                        {
                            AgeCategory age = new AgeCategory()
                            {
                                AgeCategoryName = Bookinfo.AgeCategoryName
                            };
                            db.AgeCategories.Add(age);
                            db.SaveChanges();
                            agecategoryID = db.AgeCategories.ToList().Where(a => a.AgeCategoryName == Bookinfo.AgeCategoryName).Select(a=>a.PK_AgeCategory).FirstOrDefault();
                        }
                        else
                        {
                            agecategoryID = Bookinfo.AgeCategoryID;


                        }
                        #endregion

                        #region book 
                        Book newbook = new Book()
                        {
                            BookTitle = Bookinfo.BookTitle,
                            NumberofPages = Bookinfo.NumberofPages,
                            edition = Bookinfo.edition,
                            Description = Bookinfo.Description,
                            PrintNumber = Bookinfo.PrintNumber,
                            Publicationyear = Bookinfo.Publicationyear,
                            FK_PublicationID = publicationID,
                            FK_AgeCategoryID = agecategoryID
                        };
                        db.Books.Add(newbook);
                        db.SaveChanges();
                        var Book = db.Books.ToList().Where(a => a.BookTitle == newbook.BookTitle && a.NumberofPages == newbook.NumberofPages && a.FK_AgeCategoryID == newbook.FK_AgeCategoryID && a.FK_PublicationID == newbook.FK_PublicationID).FirstOrDefault();
                        #endregion

                        #region writers
                        List<string> writers = new List<string>();
                        writers.Add(Bookinfo.writer1);
                        writers.Add(Bookinfo.writer2);
                        writers.Add(Bookinfo.writer3);
                        writers.Add(Bookinfo.writer4);
                        writers.Add(Bookinfo.writer5);
                        foreach(string writer in writers)
                        {
                            if(writer != "" && writer !=null)
                            {
                                var currentWriter = db.BookMans.ToList().Where(a => (a.Fname + " " + a.Lname) == writer).FirstOrDefault();
                                BookMan newbookman = new BookMan();
                                if (currentWriter == null)
                                {
                                    string[] names = writer.ToString().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
                                    string lname = "";
                                    for(int i = 1; i< names.Length; i++)
                                    {
                                        lname += names[i] + " ";
                                    }
                                    newbookman = new BookMan()
                                    {
                                        Fname = names[0],
                                        Lname= lname
                                    };
                                    db.BookMans.Add(newbookman);
                                    db.SaveChanges();
                                }
                                long writerIDProfession = db.ProfessionTypes.ToList().Where(a => a.ProfrssionTypeName.Trim() == "نویسنده").Select(a => a.PK_ProfessionTypeID).FirstOrDefault();
                                BookMansProfession bmp = new BookMansProfession()
                                {
                                    FK_BookMansID = newbookman.PK_BookMansID,
                                    FK_ProfessionTypeID = writerIDProfession,
                                    Fk_BookID = Book.PK_BookID
                                };
                                db.BookMansProfessions.Add(bmp);
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region translators
                        List<string> translators = new List<string>();
                        translators.Add(Bookinfo.translator1);
                        translators.Add(Bookinfo.translator2);
                        translators.Add(Bookinfo.translator3);
                        translators.Add(Bookinfo.translator4);
                        translators.Add(Bookinfo.translator5);
                        foreach (string translator in translators)
                        {
                            if (translator != "" && translator != null)
                            {
                                var currentTraslator = db.BookMans.ToList().Where(a => (a.Fname + " " + a.Lname).Trim() == translator).FirstOrDefault();
                                BookMan newbookman = new BookMan();
                                if (currentTraslator == null)
                                {
                                    string[] names = translator.ToString().Split(new char[] { ' ' });
                                    string lname = "";
                                    for (int i = 1; i < names.Length; i++)
                                    {
                                        lname += names[i] + " ";
                                    }
                                    newbookman = new BookMan()
                                    {
                                        Fname = names[0],
                                        Lname = lname
                                    };
                                    db.BookMans.Add(newbookman);
                                    db.SaveChanges();
                                }
                                long transIDProfession = db.ProfessionTypes.ToList().Where(a => a.ProfrssionTypeName.Trim() == "مترجم").Select(a => a.PK_ProfessionTypeID).FirstOrDefault();
                                BookMansProfession bmp = new BookMansProfession()
                                {
                                    FK_BookMansID = newbookman.PK_BookMansID,
                                    FK_ProfessionTypeID = transIDProfession,
                                    Fk_BookID = Book.PK_BookID
                                };
                                db.BookMansProfessions.Add(bmp);
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region Tags

                        BookTag bt = new BookTag()
                        {
                            FK_TagID = Bookinfo.TagsCategory,
                            FK_BookID = Book.PK_BookID
                        };
                        db.BookTags.Add(bt);
                        db.SaveChanges();

                        #endregion

                        #region image
                        if (Bookinfo.bookimage != null)
                        {
                            string pic = System.IO.Path.GetFileName(Bookinfo.bookimage.FileName);
                            string path = System.IO.Path.Combine(
                                                   Server.MapPath("~/Content/Images"), pic);
                            // file is uploaded
                            Bookinfo.bookimage.SaveAs(path);
                            Image image = new Image()
                            {
                                Path = "/Content/Images/" + pic,
                                TemporaryName = pic,
                                Uploaddate = DateTime.Now
                            };
                            db.Images.Add(image);
                            db.SaveChanges();
                            long imageID = db.Images.ToList().Where(a => a.TemporaryName == pic).Select(a=>a.PK_ImageID).FirstOrDefault();
                            BookImage bi = new BookImage()
                            {
                                FK_BookID = Book.PK_BookID,
                                FK_ImageID = imageID,
                            };
                            db.BookImages.Add(bi);
                            db.SaveChanges();
                        }
                        #endregion

                        #region sellingInfo
                        bool isUsed = Bookinfo.isNew == "on" ? false : true;
                        bool doexist = Bookinfo.DoExist == "on" ? true : false;
                        SellingInfo selling = new SellingInfo()
                        {
                            Do_Exist = doexist,
                            isUsed = isUsed,
                            Fk_BookID = Book.PK_BookID,
                            Fk_memberID = Convert.ToInt64(Session[MyStateVariables.UserID].ToString()),
                            RealPrice = Bookinfo.realPrice,
                            SellerPrice = Bookinfo.yourPrice,
                            Description = Bookinfo.SellingDescription,
                            EnteredDate = DateTime.Now,
                            isExpired = false
                        };
                        db.SellingInfoes.Add(selling);
                        db.SaveChanges();

                        #endregion

                        scope.Complete();

                        return Redirect("UserProfile"); 
                    }
                    catch (SqlException E)
                    {
                        Transaction.Current.Rollback();

                        return Redirect("UserProfile"); 
                    }


                }
            }
        }

        [HttpGet]
        public ActionResult UpdateSellerBook(long id)
        {
            if (Convert.ToBoolean(Session[MyStateVariables.LoggedIn]))
            {
                HomePageModel model = new HomePageModel();
                long usid = new long();
                usid = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
                model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), usid);
                if (model.User.MemberType == 2)
                {
                    var sellerbooktoedit = model.User.MyBooksAsSeller.Where(a => a.book.PK_BookID == id).FirstOrDefault();
                    if (sellerbooktoedit != null)
                    {
                        BookModel book = BookModel.GetBookInfos(id);
                        model.Books = new List<BookModel>() { book };
                        return View(model);
                    }
                    else
                    {
                        return View("NotFoundError");
                    }
                }
                else
                {
                    return View("NotFoundError");
                }                
            }
            else
            {
                return View("NotFoundError");
            }
        }

        [HttpPost]
        public ActionResult UpdateSellerBook(BooksModel Bookinfo)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    try
                    {

                        #region publicationCheck
                        long publicationID = 0;
                        if (Bookinfo.publication != null)
                        {
                            var existingpub = db.Publications.ToList().Where(a => a.PublicationName == Bookinfo.publication).FirstOrDefault();
                            if (existingpub != null)
                            {
                                publicationID = existingpub.PK_PublicationID;
                            }
                            else
                            {
                                Publication newpub = new Publication()
                                {
                                    PublicationName = Bookinfo.publication
                                };
                                db.Publications.Add(newpub);
                                db.SaveChanges();
                                existingpub = db.Publications.ToList().Where(a => a.PublicationName == Bookinfo.publication).FirstOrDefault();
                                publicationID = existingpub.PK_PublicationID;
                            }
                        }
                        #endregion

                        #region AgeCategory
                        long agecategoryID = 0;
                        if (Bookinfo.AgeCategoryName != null)
                        {
                            AgeCategory age = new AgeCategory()
                            {
                                AgeCategoryName = Bookinfo.AgeCategoryName
                            };
                            db.AgeCategories.Add(age);
                            db.SaveChanges();
                            agecategoryID = db.AgeCategories.ToList().Where(a => a.AgeCategoryName == Bookinfo.AgeCategoryName).Select(a => a.PK_AgeCategory).FirstOrDefault();
                        }
                        else
                        {
                            agecategoryID = Bookinfo.AgeCategoryID;


                        }
                        #endregion

                        #region sellingInfo
                        bool isUsed = Bookinfo.isNew == "on" ? false : true;
                        bool doexist = Bookinfo.DoExist == "on" ? true : false;
                        long usid = Convert.ToInt64(Session[MyStateVariables.UserID].ToString());
                        var sellinfo = db.SellingInfoes.Where(a => a.Fk_BookID == Bookinfo.PK_BookID && a.Fk_memberID == usid).FirstOrDefault();
                        sellinfo.Do_Exist = doexist;
                        sellinfo.isUsed = isUsed;
                        sellinfo.RealPrice = Bookinfo.realPrice;
                        sellinfo.SellerPrice = Bookinfo.yourPrice;
                        sellinfo.Description = Bookinfo.SellingDescription;
                        db.SaveChanges();

                        #endregion

                        #region book 
                        var book = db.Books.Where(a => a.PK_BookID == Bookinfo.PK_BookID).FirstOrDefault();
                        book.BookTitle = Bookinfo.BookTitle;
                        book.NumberofPages = Bookinfo.NumberofPages;
                        book.edition = Bookinfo.edition;
                        book.Description = Bookinfo.Description;
                        book.PrintNumber = Bookinfo.PrintNumber;
                        book.Publicationyear = Bookinfo.Publicationyear;
                        book.FK_PublicationID = publicationID;
                        book.FK_AgeCategoryID = agecategoryID;
                        db.SaveChanges();
                        #endregion

                        #region writers
                        long writerIDProfession = db.ProfessionTypes.ToList().Where(a => a.ProfrssionTypeName.Trim() == "نویسنده").Select(a => a.PK_ProfessionTypeID).FirstOrDefault();

                        var ws = db.BookMansProfessions.Where(a => a.FK_ProfessionTypeID == writerIDProfession && a.Fk_BookID == Bookinfo.PK_BookID).ToList();
                        foreach(var w in ws)
                        {
                            db.BookMansProfessions.Remove(w);
                            db.SaveChanges();
                        }                        
                        List<string> writers = new List<string>();
                        writers.Add(Bookinfo.writer1);
                        writers.Add(Bookinfo.writer2);
                        writers.Add(Bookinfo.writer3);
                        writers.Add(Bookinfo.writer4);
                        writers.Add(Bookinfo.writer5);
                        foreach (string writer in writers)
                        {
                            if (writer != "" && writer != null)
                            {
                                var currentWriter = db.BookMans.ToList().Where(a => (a.Fname + " " + a.Lname).Trim() == writer.Trim()).FirstOrDefault();
                                BookMan newbookman = currentWriter;
                                if (currentWriter == null)
                                {
                                    string[] names = writer.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    string lname = "";
                                    for (int i = 1; i < names.Length; i++)
                                    {
                                        lname += names[i] + " ";
                                    }
                                    newbookman = new BookMan()
                                    {
                                        Fname = names[0],
                                        Lname = lname
                                    };
                                    db.BookMans.Add(newbookman);
                                    db.SaveChanges();

                                }
                                BookMansProfession bmp = new BookMansProfession()
                                {
                                    FK_BookMansID = newbookman.PK_BookMansID,
                                    FK_ProfessionTypeID = writerIDProfession,
                                    Fk_BookID = Bookinfo.PK_BookID
                                };
                                db.BookMansProfessions.Add(bmp);
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region translators
                        long transIDProfession = db.ProfessionTypes.ToList().Where(a => a.ProfrssionTypeName.Trim() == "مترجم").Select(a => a.PK_ProfessionTypeID).FirstOrDefault();

                        var ts = db.BookMansProfessions.Where(a => a.FK_ProfessionTypeID == transIDProfession && a.Fk_BookID == Bookinfo.PK_BookID).ToList();
                        foreach (var t in ts)
                        {
                            db.BookMansProfessions.Remove(t);
                            db.SaveChanges();
                        }
                        List<string> translators = new List<string>();
                        translators.Add(Bookinfo.translator1);
                        translators.Add(Bookinfo.translator2);
                        translators.Add(Bookinfo.translator3);
                        translators.Add(Bookinfo.translator4);
                        translators.Add(Bookinfo.translator5);
                        foreach (string translator in translators)
                        {
                            if (translator != "" && translator != null)
                            {
                                var currentTraslator = db.BookMans.ToList().Where(a => (a.Fname + " " + a.Lname).Trim() == translator.Trim()).FirstOrDefault();
                                BookMan newbookman = currentTraslator;
                                if (currentTraslator == null)
                                {
                                    string[] names = currentTraslator.ToString().Split(new char[] { ' ' });
                                    string lname = "";
                                    for (int i = 1; i < names.Length; i++)
                                    {
                                        lname += names[i] + " " ;
                                    }
                                    newbookman = new BookMan()
                                    {
                                        Fname = names[0],
                                        Lname = lname
                                    };
                                    db.BookMans.Add(newbookman);
                                    db.SaveChanges();
                               }
                               BookMansProfession bmp = new BookMansProfession()
                                {
                                    FK_BookMansID = newbookman.PK_BookMansID,
                                    FK_ProfessionTypeID = transIDProfession,
                                    Fk_BookID = Bookinfo.PK_BookID
                                };
                                db.BookMansProfessions.Add(bmp);
                                db.SaveChanges();
                            }
                        }
                        #endregion

                        #region Tags
                        var bts = db.BookTags.Where(a => a.FK_BookID == Bookinfo.PK_BookID).FirstOrDefault();
                        if (bts == null || bts.FK_TagID != Bookinfo.TagsCategory)
                        {
                            if (bts != null)
                            {
                                db.BookTags.Remove(bts);
                                db.SaveChanges();
                            }
                            BookTag bt = new BookTag()
                            {
                                FK_TagID = Bookinfo.TagsCategory,
                                FK_BookID = Bookinfo.PK_BookID
                            };
                            db.BookTags.Add(bt);
                            db.SaveChanges();
                        }
                        #endregion

                        #region image
                        
                        if (Bookinfo.bookimage != null)
                        {
                            var bimage = db.BookImages.Where(a => a.FK_BookID == Bookinfo.PK_BookID).FirstOrDefault();
                            if (bimage != null)
                            {
                                db.BookImages.Remove(bimage);
                                db.SaveChanges();
                            }
                            string pic = System.IO.Path.GetFileName(Bookinfo.bookimage.FileName);
                            string path = System.IO.Path.Combine(
                                                   Server.MapPath("~/Content/Images"), pic);
                            // file is uploaded
                            Bookinfo.bookimage.SaveAs(path);
                            Image image = new Image()
                            {
                                Path = "/Content/Images/" + pic,
                                TemporaryName = pic,
                                Uploaddate = DateTime.Now
                            };
                            db.Images.Add(image);
                            db.SaveChanges();
                            long imageID = db.Images.ToList().Where(a => a.TemporaryName == pic).Select(a => a.PK_ImageID).FirstOrDefault();
                            BookImage bi = new BookImage()
                            {
                                FK_BookID = Bookinfo.PK_BookID,
                                FK_ImageID = imageID,
                            };
                            db.BookImages.Add(bi);
                            db.SaveChanges();
                        }
                        #endregion

                      

                        scope.Complete();

                        return Redirect("UserProfile");
                    }
                    catch(SqlException e)
                    {
                        Transaction.Current.Rollback();

                        return Redirect("UserProfile");
                    }


                }
            }
        }

        [HttpPost]
        public ActionResult DeleteSellerBook(long id)
        {
            if (Convert.ToBoolean(Session[MyStateVariables.LoggedIn]))
            {
                HomePageModel model = new HomePageModel();
                long usid = new long();
                usid = Convert.ToBoolean(Session[MyStateVariables.LoggedIn]) ? Convert.ToInt64(Session[MyStateVariables.UserID]) : -1;
                model = Utility.getHomeInfo(Convert.ToBoolean(Session[MyStateVariables.LoggedIn]), usid);
                if (model.User.MemberType == 2)
                {
                    var sellerbooktodelete = model.User.MyBooksAsSeller.Where(a => a.book.PK_BookID == id).FirstOrDefault();
                    if (sellerbooktodelete != null)
                    {
                        if(BookModel.DeleteBook(id))
                        {
                            return Redirect("UserProfile");
                        }
                        else
                        { return View("NotFoundError"); }
                    }
                    else
                    {
                        return View("NotFoundError");
                    }
                }
                else
                {
                    return View("NotFoundError");
                }
            }
            else
            {
                return View("NotFoundError");
            }
        }

        #region "AJAX"


        [HttpPost]
        public ActionResult DeleteFavorite(long favoriteID, long UserID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                try
                {
                    Favorite f = db.Favorites.Where(a => a.Fk_BookID == favoriteID && a.Fk_memberID == UserID).FirstOrDefault();
                    db.Favorites.Remove(f);
                    db.SaveChanges();
                    return Redirect("UserProfile");
                }
                catch
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

        }


        [HttpPost]
        public ActionResult validateEmail(object[] email)
        {
            bool[] result = new bool[2];
            string[] domain = email[0].ToString().Split('@');
            string[] dend = email[0].ToString().Split('.');
            if (domain.Length != 2 || dend.Length != 2)
            {
                result[0] = false;
            }
            else
            {
                if (domain[0] == "" || domain[1] == "" || dend[0] == "" || dend[1] == "")
                {
                    result[0] = false;
                }
                else
                {
                    using (onlinebookstore db = new onlinebookstore())
                    {
                        try
                        {
                            var Emailusername = db.Viw_IfRepetitiveEmail.ToList().Where(a => a.DomainName == domain[1].ToString() && a.UserName == domain[0].ToString()).FirstOrDefault();
                            if (Emailusername != null)
                            { result[1] = false; }
                            else { result[1] = true; }
                        }
                        catch
                        {
                            result[1] = false;
                        }
                    }
                    result[0] = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult validateUsername(object[] username)
        {
            bool[] result = new bool[1];
            using (onlinebookstore db = new onlinebookstore())
            {
                try
                {
                    var Username = db.Members.ToList().Where(a => a.UserName == username[0].ToString()).FirstOrDefault();
                    if (Username != null)
                    { result[0] = false; }
                    else { result[0] = true; }
                }
                catch
                {
                    result[0] = false;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Logincheck(string username, string password)
        {
            bool flag = false;
            using (onlinebookstore db = new onlinebookstore())
            {
                var user = db.Members.Where(a => a.UserName == username).FirstOrDefault();
                if (user != null)
                {
                    var pack = Hash.ComputeHash(password, user.PassSalt, HashAlgorithm.SHA512);
                    if (!pack.HashBytes.SequenceEqual(user.PassHash)) { flag = false; }
                    Session[MyStateVariables.LoggedIn] = true;
                    Session[MyStateVariables.UserID] = user.PK_MemberID;
                    Session[MyStateVariables.Username] = user.UserName;
                    flag = true;

                }
                else
                {
                    flag = false;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBookMan(string phrase)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var bookmans = db.BookMans.ToList().Where(a => a.Fname.Contains(phrase.ToString()) || a.Lname.Contains(phrase.ToString())).ToList();
                List<BookmanModel> Mans = new List<BookmanModel>();
                foreach(var b in bookmans)
                {
                    Mans.Add(new BookmanModel()
                    {
                        Fname = b.Fname,
                        ID = b.PK_BookMansID,
                        Lname = b.Lname
                    });
                }
                return Json(Mans, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPublications(string phrase)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var publi = db.Publications.ToList().Where(a => a.PublicationName.Contains(phrase)).Select(a=>a.PublicationName);
                
                return Json(publi, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteMyBookAsSeller(long favoriteID, long UserID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                try
                {
                    Favorite f = db.Favorites.Where(a => a.Fk_BookID == favoriteID && a.Fk_memberID == UserID).FirstOrDefault();
                    db.Favorites.Remove(f);
                    db.SaveChanges();
                    return Redirect("UserProfile");
                }
                catch
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

        }


        [HttpPost]
        public ActionResult RemoveComment(long bookID, string commentID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var com = db.Comments.ToList().Where(a => a.Pk_CommentID == Convert.ToInt64(commentID)).FirstOrDefault();
                db.Comments.Remove(com);
                db.SaveChanges();
                return RedirectToAction("UserProfile"); 
            }
        }
        #endregion
    }
}
