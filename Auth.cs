using EffortSheet.App_Data;
using EffortSheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EffortSheet
{
    public class Auth
    {
        public static List<string> GetID()
        {
            var list = new List<string>();
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    list = db.NameList.Select(a => a.NetworkID).ToList();
                }
                return list;
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }  
        }

        public static string GetUser()
        {
            var loginUser = System.Web.HttpContext.Current.User.Identity.Name;

            if (loginUser.ToUpper().Contains("ADM"))
                loginUser = loginUser.Substring(12);
            if (loginUser.ToUpper().Contains("SYSTEMS"))
                loginUser = loginUser.Substring(8);

            return loginUser;
        }
    }
}