using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace my_pro.BussinessLogic
{
    static class Settings
    {
        internal static string AdminUsername
        {
            get
            {
                return "admininstrator";
            }
        }


        private static string loggedinuser;
        internal static string LoggedInUser
        {
            get
            {
                return loggedinuser;
            }
            set
            {
                loggedinuser = value;
            }
        }


        private static Boolean Isloggedin;
        internal static Boolean IsLoggedIn
        {
            get
            {
                return Isloggedin;
            }
            set
            {

                Isloggedin= value;
            }
        }
    }
}