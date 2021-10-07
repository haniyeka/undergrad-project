using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace my_pro.BussinessLogic
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,AllowMultiple=false)]
    public class CustomAuthorizeAttribute:Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {

                bool isLoggedIn = Convert.ToBoolean(
                    filterContext.HttpContext.Session[MyStateVariables.LoggedIn]
                    );
                if (!isLoggedIn)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                filterContext.Result =  new HttpUnauthorizedResult("Access Denied!!! Your are not allowed to see this page ....");
            }
        }
    }
}