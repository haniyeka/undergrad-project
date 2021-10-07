using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace my_pro.Models.BussinessTypes
{
    public class InvoiceModel
    {
        public System.DateTime Date { get; set; }

        public  BookModel Book { get; set; }
        public  InvoiceInfo InvoiceInfo1 { get; set; }
        public  UserAccount Memberseller { get; set; }
        public  UserAccount Memberbuyer { get; set; }
        public  PayType PayType { get; set; }
        public  State State { get; set; }
        public long count { get; set; }
    }
}