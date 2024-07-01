using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PlantAdmin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!WiseSession.isLoggedIn())
        {
            Response.Redirect("Login.aspx");
        }
        /*
        else
        {
            Response.Write(WiseSession.isLoggedIn().ToString() + "<br/>");
            Response.Write(WiseSession.Username + "<br/>");
            Response.End();
        }
        */

        if (!Page.IsPostBack)
        {
            VerifyUserAllowed();
        }
    }

    private void VerifyUserAllowed()
    {
        //string[] allowedUsers = { "CISOVITSCH","charber", "jmacdougall", "jross", "rvinroe", "ckumfer", "bkaufmann", "jmcgreger", "jlarkins", "swhaley", "mpicardo" };
        string[] ALPusers = { "jmoss", "cturner", "htsmith" };
        string[] BUTusers = { "rvinroe", "mmcneely" };
        string[] FTWusers = { "ckumfer", "sspurr" };
        //string[] PDTusers = { "jross", "jperry" };
        string[] BMKusers = { "wprettyman" };
        string[] PORusers = { "pmorse", "gnadeau", "mjackman" };
        string[] ITusers = { "bkaufmann", "jlarkins", "swhaley", "mpicardo", "jmcgreger", "bsaunders" };

        string allowedPlant = "NONE";

        //string currentUser = "jmcgreger";// HttpContext.Current.User.Identity.Name.Replace("WISENT\\", "").ToLower();
        string currentUser = WiseSession.Username.ToLower().Replace("@wbf.com", "");
        currentUser = currentUser.Replace("@brandmarkinc.com", "");

        if (currentUser.Trim() == "")
        {
            Response.Redirect("Login.aspx");
        }

        foreach (string userName in ALPusers)
        {
            if (userName.ToLower() == currentUser)
            {
                allowedPlant = "ALP";
            }
        }
        foreach (string userName in BUTusers)
        {
            if (userName.ToLower() == currentUser)
            {
                allowedPlant = "BUT";
            }
        }
        foreach (string userName in FTWusers)
        {
            if (userName.ToLower() == currentUser)
            {
                allowedPlant = "FTW";
            }
        }
        foreach (string userName in BMKusers)
        {
            if (userName.ToLower() == currentUser)
            {
                allowedPlant = "BMK";
            }
        }
        foreach (string userName in PORusers)
        {
            if (userName.ToLower() == currentUser)
            {
                allowedPlant = "POR";
            }
        }
        foreach (string userName in ITusers)
        {
            if (userName.ToLower() == currentUser)
            {
                allowedPlant = "ALL";
            }
        }

        if (allowedPlant != "NONE")
        {
            mvMain.SetActiveView(vwAllowed);
            pnlEditALP.Visible = ((allowedPlant == "ALP") || (allowedPlant == "ALL"));
            pnlEditALZ.Visible = ((allowedPlant == "ALP") || (allowedPlant == "ALL"));
            pnlEditBUT.Visible = ((allowedPlant == "BUT") || (allowedPlant == "ALL"));
            pnlEditFTW.Visible = ((allowedPlant == "FTW") || (allowedPlant == "ALL"));
            //pnlEditPDT.Visible = ((allowedPlant == "PDT") || (allowedPlant == "ALL"));
            pnlEditBMK.Visible = ((allowedPlant == "BMK") || (allowedPlant == "ALL"));
            pnlEditPOR.Visible = ((allowedPlant == "POR") || (allowedPlant == "ALL"));
        }
        else
        {
            mvMain.SetActiveView(vwNotAllowed);
        }
    }
}