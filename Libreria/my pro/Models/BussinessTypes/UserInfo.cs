using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace my_pro.Models.BussinessTypes
{
    public class UserInfo
    {
        public long Pk_ID { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Address { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public string  Gender { get; set; }
        public string Birthdate { get; set; }
    }
}