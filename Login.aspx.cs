using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSubmitLogin_Click(object sender, EventArgs e)
    {
        processLogin(txtUsername.Text.ToUpper().Trim(), txtPassword.Text.Trim());
        if (!WiseSession.isLoggedIn())
        {
            lblLoginMessage.Text = "Error logging in - please try again.";
        }
    }



    public bool areWiselinkCredentialsValid(string username, string password)
    {
        bool credentialsValid = false;

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WiseLinkSqlConnection"].ConnectionString))
        {
            string sql = "FE_VerifyWiseLinkCredentials";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter pEmailAddress = new SqlParameter("@emailAddress", SqlDbType.VarChar, 50);
            SqlParameter pPassword = new SqlParameter("@password", SqlDbType.VarChar, 100);
            SqlParameter pIsValid = new SqlParameter("@isValid", SqlDbType.Bit);

            pEmailAddress.Value = username;
            pPassword.Value = password;
            pIsValid.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(pEmailAddress);
            cmd.Parameters.Add(pPassword);
            cmd.Parameters.Add(pIsValid);


            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            bool.TryParse(pIsValid.Value.ToString(), out credentialsValid);

        }

        return credentialsValid;
    }

    protected void processLogin(string username, string password)
    {
        bool validLogin = false;

        if (areWiselinkCredentialsValid(username, password))
        {
            validLogin = true;
        }

        if (validLogin)
        {
            WiseSession.Username = username;
            Response.Redirect("~/PlantAdmin.aspx");
        }
        else
        {
            WiseSession.Username = null;
        }
    }
    /*
    private bool isPiedmontAccount()
    {
        bool isPDT = false;
        int acctNumber = 0;
        Int32.TryParse(txtAcctNumber.Text.Trim(), out acctNumber);

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString))
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "isPiedmontAccount";

            SqlParameter pAcctNumber = new SqlParameter("@AcctNumber", SqlDbType.Int);
            SqlParameter pIsPiedmont = new SqlParameter("@IsPiedmont", SqlDbType.Int);

            pAcctNumber.Value = acctNumber;
            pIsPiedmont.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(pAcctNumber);
            cmd.Parameters.Add(pIsPiedmont);

            cmd.ExecuteNonQuery();

            isPDT = pIsPiedmont.Value.ToString() == "1";

            conn.Close();
        }


        return (isPDT);
    }
    */
}