using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WiseSession
/// </summary>
public class WiseSession
{
    public static bool isLoggedIn()
    {
        if (HttpContext.Current.Session["Username"] == null)
            return false;
        else
            return true;
    }

    public static void logOff()
    {
        HttpContext.Current.Session["Username"] = null;
    }

    public static string Username
    {
        get
        {
            if (HttpContext.Current.Session["Username"] != null)
                return HttpContext.Current.Session["Username"].ToString();
            else
                return "";
        }
        set
        {
            if (value != null)
                HttpContext.Current.Session["Username"] = value.ToString();
            else
                HttpContext.Current.Session["Username"] = null;
        }
    }
}