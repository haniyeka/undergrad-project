using my_pro.BussinessLogic;
using RayanShafagh.SecurityProvider;
using RayanShafagh.SecurityProvider.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Xml.Linq;

namespace my_pro.Models.BussinessTypes
{
    public class UserAccount
    {
        public long Pk_memberID { get; set; }
        [Display(Name = "Fname")]
        [Required]
        public string Fname { get; set; }

        [Display(Name = "Lname")]
        [Required]
        public string Lname { get; set; }

        [Display(Name = "EmailAddress")]
        [Required]
        public string EmailAddress { get; set; }

        [Display(Name = "Username")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }

        [Display(Name = "MemberType")]
        [Required]
        public long MemberType { get; set; }

        [Display(Name = "Description")]

        public string Description { get; set; }

        [Display(Name = "Gender")]

        public string Gender { get; set; }

        [Display(Name = "Birthdate")]

        public DateTime? Birthdate { get; set; }

        [Display(Name = "Address")]

        public string Address { get; set; }

        [Display(Name = "PostalCode")]

        public string PostalCode { get; set; }

        [Display(Name = "City")]

        public string City { get; set; }

        [Display(Name = "Province")]

        public string Province { get; set; }

        [Display(Name = "Phone")]

        public long? Phone { get; set; }
        [Display(Name = "Image")]

        public string Image { get; set; }
        
        public List<CommentModel> Comments { get; set; }

        public List<BookModel> favorites { get; set; }

        public List<InvoiceInfo> Invoices { get; set; }

        public List<BookModel> MyBooksAsSeller { get; set; }

        public IEnumerable<AgeCategory> AllAgeCategories { get; set; }

        public List<InvoiceModel> Basket { get; set; }

        public static Boolean Register(UserAccount newuser)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    try
                    {
                        string username = newuser.EmailAddress.Split('@')[0].ToString();
                        string domain = newuser.EmailAddress.Split('@')[1].ToString();
                        var user = db.Users.ToList().Where(a => a.UserName == username).FirstOrDefault();
                        long uid = new long();
                        if (user == null)
                        {
                            db.sp_AddUserName(username);
                            db.SaveChanges();
                            uid = Convert.ToInt64(db.Users.Where(a => a.UserName == username).FirstOrDefault().PK_UserID);
                        }
                        else
                        {
                            uid = user.PK_UserID;
                        }
                        var dname = db.Domains.ToList().Where(a => a.DomainName == domain).FirstOrDefault();
                        long did = new long();
                        if (dname == null)
                        {

                            db.sp_AddDomain(domain);
                            db.SaveChanges();
                            did = Convert.ToInt64(db.Domains.Where(a => a.DomainName == domain).FirstOrDefault().PK_DomainID);
                        }
                        else
                        {
                            did = dname.PK_DomainID;
                        }

                        Person p = new Person()
                        {
                            Fname = newuser.Fname,
                            Lname = newuser.Lname,
                            FK_DomainID = Convert.ToInt64(did),
                            FK_UserID = Convert.ToInt64(uid)
                        };

                        db.People.Add(p);
                        db.SaveChanges();

                        var pid = db.People.Where(a => a.FK_DomainID == did && a.FK_UserID == uid).FirstOrDefault().PK_PersonID;
                        Member newmember = new Member()
                        {
                            FK_PersonID = pid,
                            FK_MemberTypeID = newuser.MemberType,
                            JoinDate = DateTime.Now,
                            UserName = newuser.Username
                        };
                        byte[] salt = Hash.GenerateSalt(10, 30);
                        HashPack newpack = Hash.ComputeHash(newuser.Password, salt, HashAlgorithm.SHA512);
                        newmember.PassHash = newpack.HashBytes;
                        newmember.PassSalt = salt;
                        db.Members.Add(newmember);
                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    catch (Exception)
                    {
                        Transaction.Current.Rollback();
                        return false;
                    }
                }
            }
        }
        
        public static UserAccount UserProfileInfo(bool loggedIn, long? UserId)
        {
            if (loggedIn != false)
            {
                using (onlinebookstore db = new onlinebookstore())
                {
                    Member member = (Member)db.Members.ToList().Where(a => a.PK_MemberID == Convert.ToInt64(UserId)).FirstOrDefault();
                    Person person = (Person)db.People.ToList().Where(a => a.PK_PersonID == member.FK_PersonID).FirstOrDefault();
                    User user = (User)db.Users.ToList().Where(a => a.PK_UserID == person.FK_UserID).FirstOrDefault();
                    Domain dom = (Domain)db.Domains.ToList().Where(a => a.PK_DomainID == person.FK_DomainID).FirstOrDefault();
                    Image img = (Image)db.Images.ToList().Where(a => a.PK_ImageID == person.FK_ImageID).FirstOrDefault();
                    MemberType type = (MemberType)db.MemberTypes.ToList().Where(a => a.PK_MemberTypeID == member.FK_MemberTypeID).FirstOrDefault();
                    Address addressline = (Address)db.Addresses.ToList().Where(a => a.PK_AddressID == person.FK_AddressID).FirstOrDefault();
                    Province province = new Province();
                    City city = new City();
                    if (addressline != null)
                    {
                        province = (Province)db.Provinces.ToList().Where(a => a.PK_ProvinceID == addressline.FK_ProvinceID).FirstOrDefault();
                        city = (City)db.Cities.ToList().Where(a => a.PK_CityID == addressline.FK_CityID).FirstOrDefault();
                    }
                    List<long> favs = db.Favorites.Where(a => a.Fk_memberID == UserId).Select(a => a.Fk_BookID).ToList();
                    List<BookModel> favorites = new List<BookModel>();
                    foreach (long bookid in favs)
                    {
                        BookModel b = BookModel.GetBookInfos(bookid);
                        if (b != null && b.book!=null)
                        {
                            if (b.book.isDeletes != false)
                            {
                                favorites.Add(b);
                            }
                        }
                    }
                    
                    List<CommentModel> comments = CommentModel.GetCommentsBymemberID(Convert.ToInt64(UserId));
                    var age = db.AgeCategories.ToArray();
                    var invoices = new List<InvoiceInfo>();
                    var mybooks = new List<BookModel>();
                    var mybasket = new List<InvoiceModel>();
                    if (type.PK_MemberTypeID == 1)
                    {
                        invoices = db.InvoiceInfoes.Where(a => a.Fk_memberIDbuyer == member.PK_MemberID).ToList();
                        var basketid = db.States.Where(a => a.StateName.Contains("سبد خرید")).FirstOrDefault();
                        var basket = db.InvoiceInfoes.Where(a => a.Fk_memberIDbuyer == member.PK_MemberID && a.Fk_StateID == basketid.Pk_StateID).ToList();
                        foreach(var b in basket)
                        {
                             if (b.Book.isDeletes != true)
                             {
                                var book = BookModel.GetBookInfos(b.Book.PK_BookID);
                                mybasket.Add(new InvoiceModel()
                                {
                                    Book = book,
                                    count = b.Count,
                                    Date = b.Date,
                                    State = basketid,
                                    Memberseller = book.Seller,
                                    PayType = db.PayTypes.Where(a => a.PaytypeName == "پرداخت در محل").FirstOrDefault(),
                                    InvoiceInfo1 = b
                                });

                             }
                        }
                    }
                    else if (type.PK_MemberTypeID == 2)
                    {
                        invoices = db.InvoiceInfoes.Where(a => a.Fk_memberIDseller == member.PK_MemberID).ToList();
                        var myb0k = db.SellingInfoes.Where(a => a.Fk_memberID == member.PK_MemberID).ToList();
                        foreach (var book in myb0k)
                        {
                            if (book.isExpired != true)
                            {
                                if (book.Book.isDeletes != true)
                                {
                                    mybooks.Add(BookModel.GetBookInfos(book.Book.PK_BookID));
                                }
                            }
                        }
                    }
                    UserAccount USER = new UserAccount()
                    {
                        Pk_memberID = member.PK_MemberID,
                        Fname = person.Fname,
                        Lname = person.Lname,
                        City = city != null ? city.CityName : null,
                        Province = province != null ? province.ProvinceName : null,
                        Address = addressline != null ? addressline.AddressLine : null,
                        Birthdate = person.Birthday,
                        Description = member.Description,
                        EmailAddress = user.UserName + '@' + dom.DomainName,
                        Gender = person.gender,
                        Image = img != null ? img.Path : null,
                        MemberType = type.PK_MemberTypeID,
                        PostalCode = addressline != null ? addressline.PostalCode : null,
                        Username = member.UserName,
                        favorites = favorites,
                        Invoices = invoices,
                        Comments = comments,
                        MyBooksAsSeller = mybooks,
                        Phone = person.PhoneNumber,
                        Basket = mybasket
                    };
                    return USER;
                }
            }
            else
            {
                return null;
            }
        }


        public static UserAccount GetMemberShortInfoByID(long UserId)
        {
            using (onlinebookstore db = new onlinebookstore())
            {
                Member member = (Member)db.Members.ToList().Where(a => a.PK_MemberID == Convert.ToInt64(UserId)).FirstOrDefault();
                Person person = (Person)db.People.ToList().Where(a => a.PK_PersonID == member.FK_PersonID).FirstOrDefault();
                User user = (User)db.Users.ToList().Where(a => a.PK_UserID == person.FK_UserID).FirstOrDefault();
                Domain dom = (Domain)db.Domains.ToList().Where(a => a.PK_DomainID == person.FK_DomainID).FirstOrDefault();
                Image img = (Image)db.Images.ToList().Where(a => a.PK_ImageID == person.FK_ImageID).FirstOrDefault();
                MemberType type = (MemberType)db.MemberTypes.ToList().Where(a => a.PK_MemberTypeID == member.FK_MemberTypeID).FirstOrDefault();
                Address addressline = (Address)db.Addresses.ToList().Where(a => a.PK_AddressID == person.FK_AddressID).FirstOrDefault();
                Province province = new Province();
                City city = new City();
                if (addressline != null)
                {
                    province = (Province)db.Provinces.ToList().Where(a => a.PK_ProvinceID == addressline.FK_ProvinceID).FirstOrDefault();
                    city = (City)db.Cities.ToList().Where(a => a.PK_CityID == addressline.FK_CityID).FirstOrDefault();
                }
                
                UserAccount USER = new UserAccount()
                {
                    Pk_memberID = member.PK_MemberID,
                    Fname = person.Fname,
                    Lname = person.Lname,
                    City = city != null ? city.CityName : null,
                    Province = province != null ? province.ProvinceName : null,
                    Address = addressline != null ? addressline.AddressLine : null,
                    Birthdate = person.Birthday,
                    Description = member.Description,
                    EmailAddress = user.UserName + '@' + dom.DomainName,
                    Gender = person.gender,
                    Image = img != null ? img.Path : null,
                    MemberType = type.PK_MemberTypeID,
                    PostalCode = addressline != null ? addressline.PostalCode : null,
                    Username = member.UserName,
                    
                };
                return USER;
            }
        }
    }
}