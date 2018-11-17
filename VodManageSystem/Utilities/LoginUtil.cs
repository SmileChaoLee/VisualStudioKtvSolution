using System;
using Microsoft.AspNetCore.Http;

namespace VodManageSystem.Utilities
{
    public class LoginUtil
    {
        public LoginUtil()
        {
        }

        public static bool CheckIfLoggedIn(HttpContext httpContext)
        {
            bool result = false;
            ISession session = httpContext.Session;
            if (session.GetInt32("LoggedIn") == 1)
            {
                result = true;
            }
            return result;
        }
    }
}
