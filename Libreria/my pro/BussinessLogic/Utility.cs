using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using my_pro.Models.BussinessTypes;
using my_pro.Models;

namespace my_pro.BussinessLogic
{
    public static class Utility
    {      
        public static HomePageModel getHomeInfo(bool loggedIn, long? UserId)
        {
            //navcategories 
            HomePageModel model = new HomePageModel();
            using (onlinebookstore db = new onlinebookstore())
            {
                var tags = db.Tags.ToArray();
                model.Categories = tags;
                var AgeCategories = db.AgeCategories.ToArray();
                model.AgeCategories = AgeCategories;
            }
            UserAccount user = UserAccount.UserProfileInfo(loggedIn, UserId);
            model.User = user;
             
            return model;
        }
        public static string ConvertToPersianDate(DateTime date)
        {
            System.Globalization.PersianCalendar pcal = new System.Globalization.PersianCalendar();
            string m = pcal.GetMonth(date).ToString();
            if (pcal.GetMonth(date) < 10)
            {
                m = "0" + pcal.GetMonth(date).ToString();
            }
            string d = pcal.GetDayOfMonth(date).ToString();
            if (pcal.GetDayOfMonth(date) < 10)
            {
                d = "0" + pcal.GetDayOfMonth(date).ToString();
            }
            string Date = pcal.GetYear(date).ToString() + m + d;
            return Date;
        }
        public static DateTime ConvertToGregorianDate(string date)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            DateTime dt = new DateTime();
            int year = Convert.ToInt32(date.Substring(0, 4).Trim());
            int month = Convert.ToInt32(date.Substring(5, 2).Trim());
            int day = Convert.ToInt32(date.Substring(8).Trim());
            dt = pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            return dt;
        }


        public static string RandomDigit()
        {
            Random r = new Random();
            return r.Next(0, 999999).ToString();
        }
    }

}