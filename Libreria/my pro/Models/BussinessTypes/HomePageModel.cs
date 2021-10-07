using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace my_pro.Models.BussinessTypes
{
    public class HomePageModel
    {
        public IEnumerable<Tag> Categories { get; set; }

        public UserAccount User { get; set; }

        public List<BookModel> Books { get; set; }

        public IEnumerable<AgeCategory> AgeCategories { get; set; }
    }
}