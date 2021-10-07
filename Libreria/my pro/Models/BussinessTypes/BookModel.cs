using my_pro.BussinessLogic;
using my_pro.Models.BussinessTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;

namespace my_pro.Models
{
    public class BookModel
    {
        public Book book { get; set; }
        public Image image { get; set; }
        public List<BookMan> writers { get; set; }
        public List<BookMan> Translators { get; set; }
        public List<Tag> tags { get; set; }
        public SellingInfo sellinginfos { get; set; }
        public Publication publication { get; set; }
        public List<CommentModel> comments { get; set; }
        public List<Favorite> favorites { get; set; }
        public double rate { get; set; }
        public int ratecount { get; set; }
        public AgeCategory age { get; set; }
        public long invoiceCount { get; set; }

        public UserAccount Seller { get; set; }
        public static BookModel GetBookInfos(long bookID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                var book = db.Books.Where(a => a.PK_BookID == bookID).FirstOrDefault();
                if (book.isDeletes != false)
                {
                    List<CommentModel> comments = CommentModel.GetCommentsByBookID(bookID);
                    var favorites = db.Favorites.Where(a => a.Fk_BookID == bookID).ToList();
                    var booktags = db.BookTags.Where(a => a.FK_BookID == bookID).ToList();
                    List<Tag> tags = new List<Tag>();
                    foreach (BookTag t in booktags)
                    {
                        var tag = db.Tags.Where(a => a.PK_TagID == t.FK_TagID).FirstOrDefault();
                        if (tag.ParentID != null)
                        {
                            var parenttag = db.Tags.Where(a => a.PK_TagID == tag.ParentID).FirstOrDefault();
                            tags.Add((Tag)parenttag);
                        }
                        tags.Add((Tag)tag);
                    }
                    var publication = db.Publications.Where(a => a.PK_PublicationID == book.FK_PublicationID).FirstOrDefault();
                    var sellinginfos = db.SellingInfoes.Where(a => a.Fk_BookID == bookID).FirstOrDefault();
                    var seller = UserAccount.GetMemberShortInfoByID(sellinginfos.Fk_memberID);
                    var writerpro = db.ProfessionTypes.Where(a => a.ProfrssionTypeName == "نویسنده").FirstOrDefault();
                    var bookwriter = db.BookMansProfessions.Where(a => a.FK_ProfessionTypeID == writerpro.PK_ProfessionTypeID && a.Fk_BookID == bookID).ToList();
                    List<BookMan> writers = new List<BookMan>();
                    foreach (BookMansProfession b in bookwriter)
                    {
                        var writer = db.BookMans.Where(a => a.PK_BookMansID == b.FK_BookMansID).FirstOrDefault();
                        writers.Add((BookMan)writer);
                    }
                    var transpro = db.ProfessionTypes.Where(a => a.ProfrssionTypeName == "مترجم").FirstOrDefault();
                    var booktrans = db.BookMansProfessions.Where(a => a.FK_ProfessionTypeID == transpro.PK_ProfessionTypeID && a.Fk_BookID == bookID).ToList();
                    List<BookMan> translators = new List<BookMan>();
                    foreach (BookMansProfession b in booktrans)
                    {
                        var translator = db.BookMans.Where(a => a.PK_BookMansID == b.FK_BookMansID).FirstOrDefault();
                        translators.Add((BookMan)translator);
                    }
                    var ib = db.BookImages.Where(a => a.FK_BookID == bookID).FirstOrDefault();
                    var image = new Image();
                    try
                    {
                        image = db.Images.ToList().Where(a => a.PK_ImageID == ib.FK_ImageID).FirstOrDefault();
                    }
                    catch
                    {

                    }
                    var rate = db.BookRates.Where(a => a.Fk_BookID == bookID).ToList();
                    double bookrate = 0;
                    if (rate.Count >= 1)
                    {
                        bookrate = rate.Average(a => a.Rate);
                    }
                    long invoicecount = db.InvoiceInfoes.ToList().Count();
                    var agecategory = db.AgeCategories.Where(a => a.PK_AgeCategory == book.FK_AgeCategoryID).FirstOrDefault();
                    

                    BookModel Book = new BookModel()
                    {
                        book = book,
                        comments = comments,
                        favorites = favorites,
                        image = image,
                        publication = publication,
                        sellinginfos = sellinginfos,
                        tags = tags,
                        Translators = translators,
                        writers = writers,
                        rate = bookrate,
                        age = agecategory,
                        ratecount = rate.Count,
                        invoiceCount = invoicecount,
                        Seller = seller
                    };
                    return Book;
                }
                else
                {
                    return new BookModel();
                }
            }
        }
        public static List<BookModel> GetBooksByCategory(long CategoryID)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                List<BookModel> BOOKS = new List<BookModel>();
                var tag = db.Tags.Where(a => a.PK_TagID == CategoryID).FirstOrDefault();
                var booktags = new List<BookTag>();
                if (tag.ParentID == null)
                {
                    var child = db.Tags.Where(a => a.ParentID == CategoryID).ToList();
                    foreach(var ch in child)
                    {
                        var booktag = db.BookTags.Where(a => a.FK_TagID == ch.PK_TagID).ToList();
                        foreach(var bt in booktag)
                        {
                            booktags.Add(bt);
                        }
                    }
                }
                else
                {
                    booktags = db.BookTags.Where(a => a.FK_TagID == CategoryID).ToList();
                }
                foreach (var t in booktags)
                {
                    Book book = db.Books.Where(a => a.PK_BookID == t.FK_BookID).FirstOrDefault();
                    if (book.isDeletes != true)
                    {
                        //var publication = db.Publications.Where(a =>a.PK_PublicationID  == book.FK_PublicationID).FirstOrDefault();
                        var sellinginfos = db.SellingInfoes.Where(a => a.Fk_BookID == book.PK_BookID).FirstOrDefault();
                        //var writerpro = db.ProfessionTypes.Where(a => a.ProfrssionTypeName == "نویسنده").FirstOrDefault();

                        //var bookwriter = db.BookMansProfessions.Where(a => a.FK_ProfessionTypeID == writerpro.PK_ProfessionTypeID && a.Fk_BookID == book.PK_BookID).ToList();
                        //List<BookMan> writers = new List<BookMan>();
                        //foreach (BookMansProfession b in bookwriter)
                        //{
                        //    var writer = db.BookMans.Where(a => a.PK_BookMansID == b.FK_BookMansID).FirstOrDefault();
                        //    writers.Add((BookMan)writer);
                        //}
                        //var transpro = db.ProfessionTypes.Where(a => a.ProfrssionTypeName == "مترجم").FirstOrDefault();
                        //var booktrans = db.BookMansProfessions.Where(a => a.FK_ProfessionTypeID == transpro.PK_ProfessionTypeID && a.Fk_BookID == book.PK_BookID).ToList();
                        //List<BookMan> translators = new List<BookMan>();
                        //foreach (BookMansProfession b in booktrans)
                        //{
                        //    var translator = db.BookMans.Where(a => a.PK_BookMansID == b.FK_BookMansID).FirstOrDefault();
                        //    translators.Add((BookMan)translator);
                        //}
                        var ib = db.BookImages.Where(a => a.FK_BookID == book.PK_BookID).FirstOrDefault();
                        var image = new Image();
                        try
                        {
                            image = db.Images.Where(a => a.PK_ImageID == ib.FK_ImageID).FirstOrDefault();
                        }
                        catch { }
                        var rate = db.BookRates.Where(a => a.Fk_BookID == book.PK_BookID).ToList();
                        double bookrate = 0;
                        if (rate.Count >= 1)
                        {
                            bookrate = rate.Average(a => a.Rate);
                        }
                        BookModel BOOK = new BookModel()
                        {
                            book = book,
                            //comments = comments,
                            //favorites = favorites,
                            image = image,
                            //publication = publication,
                            sellinginfos = sellinginfos,
                            //tags = tags,
                            //Translators = translators,
                            //writers = writers,
                            rate = bookrate
                        };
                        BOOKS.Add(BOOK);
                    }
                }
                return BOOKS;
            }
        }
        
        public static bool DeleteBook(long ID)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    try
                    {
                        var book = db.Books.ToList().Where(a => a.PK_BookID == ID).FirstOrDefault();
                        
                        #region sellingInfo
                        var sinfo = db.SellingInfoes.ToList().Where(a => a.Fk_BookID == ID).FirstOrDefault();
                        sinfo.isExpired = true;
                        sinfo.ExpirationDate = DateTime.Now;
                        db.SaveChanges();
                        #endregion
                        
                        #region invoice
                        long exId = db.States.ToList().Where(a => a.StateName == "منقضی").Select(a=>a.Pk_StateID).FirstOrDefault(); 
                        var invoice = db.InvoiceInfoes.ToList().Where(a => a.FK_BookID == ID).ToList();
                        foreach(var i in invoice)
                        {
                            i.Fk_StateID = exId;
                            db.SaveChanges();
                        }
                        #endregion

                        #region book 
                        book.isDeletes = true;
                        db.SaveChanges();
                        #endregion

                        scope.Complete();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        Transaction.Current.Rollback();
                        return false;
                    }
                }
            }
        }

        public static List<BookModel> SearchbyKeyword (string keyword)
        {
            using(onlinebookstore db = new onlinebookstore())
            {
                List<BookModel> Result = new List<BookModel>();
                var books = db.Books.Where(a => a.BookTitle.Contains(keyword.Trim())).ToList();
                foreach(var b in books)
                {
                    BookModel book = BookModel.GetBookInfos(b.PK_BookID);
                    Result.Add(book);
                }
                var publications = db.Publications.Where(a => a.PublicationName.Contains(keyword)).ToList();
                foreach(var p in publications)
                {
                    var find = db.Books.Where(a => a.FK_PublicationID == p.PK_PublicationID).ToList();
                    foreach(var f in find)
                    {
                        BookModel book = BookModel.GetBookInfos(f.PK_BookID);
                        Result.Add(book);
                    }
                }
                var writerandtrans = db.BookMans.Where(a => a.Fname.Contains(keyword) || a.Lname.Contains(keyword)).ToList();
                foreach(var w in writerandtrans)
                {
                    var prof = db.BookMansProfessions.Where(a => a.FK_BookMansID == w.PK_BookMansID).ToList();
                    foreach(var p in prof)
                    {
                        var finaly = db.Books.Where(a => a.PK_BookID == p.Fk_BookID).ToList();
                        foreach (var f in finaly)
                        {
                            BookModel book = BookModel.GetBookInfos(f.PK_BookID);
                            Result.Add(book);
                        }
                    }
                }
                return Result;
            }
        }


        public static List<BookModel> AdvanceSearch(string title, string writer, string trans, string publication)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                List<BookModel> Result = new List<BookModel>();
                var books = db.Books.Where(a => a.BookTitle.Contains(title.Trim())).ToList();
                if(books !=null)
                {
                    var publications = db.Publications.Where(a => a.PublicationName.Contains(publication)).ToList();
                    var writers = db.BookMans.Where(a => a.Fname.Contains(writer) || a.Lname.Contains(writer)).ToList();
                    var translators= db.BookMans.Where(a => a.Fname.Contains(trans) || a.Lname.Contains(trans)).ToList();

                    foreach (var b in books)
                    {
                        BookModel book = BookModel.GetBookInfos(b.PK_BookID);
                        foreach(var p in publications)
                        {
                            foreach(var w in writers)
                            {
                                foreach(var t in translators)
                                {
                                    if (book.publication.PublicationName == p.PublicationName && book.writers.Where(a=>a.Fname == w.Fname && a.Lname == w.Lname).Count()>0 && book.Translators.Where(a => a.Fname == t.Fname && a.Lname == t.Lname).Count() > 0)
                                    {
                                        Result.Add(book);
                                    }
                                }
                            }
                        }
                    }
                    
                }
                return Result;
            }
        }

        public static bool AddtoBasket (long bookid , long memberid , int count)
        {
            using(TransactionScope scope = new TransactionScope())
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    try
                    {
                        BookModel book = GetBookInfos(bookid);
                        var stateid = db.States.Where(a => a.StateName.Contains("سبد خرید")).Select(a => a.Pk_StateID).FirstOrDefault();
                        InvoiceInfo invoice = new InvoiceInfo()
                        {
                            Fk_memberIDbuyer = memberid,
                            FK_BookID = bookid,
                            Fk_memberIDseller = book.sellinginfos.Fk_memberID,
                            Date = DateTime.Now,
                            Fk_PayType = 1,
                            Fk_StateID = stateid,
                            FK_SellingInfo = book.sellinginfos.Pk_SellingInfoID,
                            TrackingCode = "Tr" + Utility.RandomDigit() + "ack" + Utility.RandomDigit()
                        };
                        db.InvoiceInfoes.Add(invoice);
                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    catch
                    {
                        Transaction.Current.Rollback();
                        return false;
                    }
                }
            }
        }

    }
}