using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace my_pro.Models.BussinessTypes
{
    public class BooksModel
    {
        public long PK_BookID { get; set; }
        public string BookTitle { get; set; }
        public long NumberofPages { get; set; }
        public long Publicationyear { get; set; }
        public long PrintNumber { get; set; }
        public int edition { get; set; }
        public long AgeCategoryID { get; set; }
        public string AgeCategoryName { get; set; }
        public string writer1 { get; set; }
        public string writer2 { get; set; }
        public string writer3 { get; set; }
        public string writer4 { get; set; }
        public string writer5 { get; set; }
        public string translator1 { get; set; }
        public string translator2 { get; set; }
        public string translator3 { get; set; }
        public string translator4 { get; set; }
        public string translator5 { get; set; }
        public long TagsCategory { get; set; }
        public string publication { get; set; }
        public HttpPostedFileBase bookimage { get; set; }
        public string Description { get; set; }

        public long realPrice { get; set; }
        public long yourPrice { get; set; }
        public string isNew { get; set; }
        public string DoExist { get; set; }
        public string SellingDescription { get; set; }
        

    }
}