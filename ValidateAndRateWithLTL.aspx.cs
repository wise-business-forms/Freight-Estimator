using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.IO;
using System.Xml.Linq;
using UpsRateWebReference;
using UpsAddressValidationWebReference;

public partial class ValidateAndRateWithLTL : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        /*
        // If this is initial page load or session is not initialized, call VerifyUserPrefs
        if ((!Page.IsPostBack) || (Session["DefaultPlant"] == null))
        {
            VerifyUserPrefs();
            txtPickupDate.Text = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString().Substring(2);

            pnlAcctNumber.Visible = (Session["DefaultPlant"].ToString() == "ALP");

        }
        */

        if (Request.Cookies["plant"] != null)
        {
            HttpCookie aCookie = Request.Cookies["plant"];
            litPlant.Text = Server.HtmlEncode(aCookie.Value);
            Session["DefaultPlant"] = litPlant.Text;
        }
        else
        {
            Session["DefaultPlant"] = null;
        }

        txtPickupDate.Text = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString().Substring(2);

        if (Session["DefaultPlant"] == null)
        {
            AllowPlantSelect();

        }
        else if (!Page.IsPostBack)
        {
            ShowMainView();
        }
    }

    private void AllowPlantSelect()
    {
        // Pull Username of current user
        string UserName = "";// HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();

        // Store Username to Session variable
        Session["UserName"] = UserName;

        // Pull Default plant for User
        string DefaultPlant = "";// GetDefaultPlant(UserName);

        // If the user has a default plant, save it to session and show main view
        if (DefaultPlant != "")
        {
            Session["DefaultPlant"] = DefaultPlant;
            ShowMainView();
        }
        // Otherwise, allow user to select a default plant
        else
        {
            Session["DefaultPlant"] = "";
            ShowPlantOptions();
        }
    }
    private void ValidateAndRateAddress()
    {
        // Initial sbResults to serve as our plain-text log of the Validation process
        StringBuilder sbResults = new StringBuilder();

        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        try
        {
            #region -- Initialize the XAV Service and Request objects --
            XAVService xavService = new XAVService();
            XAVRequest xavRequest = new XAVRequest();

            UpsAddressValidationWebReference.RequestType request = new UpsAddressValidationWebReference.RequestType();
            String[] requestOption = { "3" };
            request.RequestOption = requestOption;
            xavRequest.Request = request;
            #endregion

            #region -- Access Security (license number, username, password) --
            UpsAddressValidationWebReference.UPSSecurity upss = new UpsAddressValidationWebReference.UPSSecurity();
            UpsAddressValidationWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsAddressValidationWebReference.UPSSecurityServiceAccessToken();
            upsSvcToken.AccessLicenseNumber = Config.UPSAccessKey;
            upss.ServiceAccessToken = upsSvcToken;
            UpsAddressValidationWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsAddressValidationWebReference.UPSSecurityUsernameToken();
            upsSecUsrnameToken.Username = Config.UPSUserName;
            upsSecUsrnameToken.Password = Config.UPSPassword;
            upss.UsernameToken = upsSecUsrnameToken;
            xavService.UPSSecurityValue = upss;
            #endregion

            #region -- Add the entered Address data to the request --
            string valAddress = txtAddress.Text.Trim();
            string valCity = txtCity.Text.Trim();
            string valState = GetSelectedState();
            string valZip = txtZip.Text.Trim();
            string valCountry = lstCountry.SelectedValue;

            AddressKeyFormatType addressKeyFormat = new AddressKeyFormatType();
            String[] addressLine = { valAddress };
            addressKeyFormat.AddressLine = addressLine;
            addressKeyFormat.PoliticalDivision2 = valCity;
            addressKeyFormat.PoliticalDivision1 = valState;
            addressKeyFormat.PostcodePrimaryLow = valZip;
            addressKeyFormat.CountryCode = valCountry;
            xavRequest.AddressKeyFormat = addressKeyFormat;
            #endregion

            #region -- Post our request to UPS Online Tools and capture the response --
            XAVResponse xavResponse = xavService.ProcessXAV(xavRequest);

            string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
            string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();
            #endregion

            #region -- Log our request to the DB --
            string targetUrl = xavService.Url;
            string UserName = HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();

            try
            {
                SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                connLog.Open();

                SqlCommand cmdLog = new SqlCommand();
                cmdLog.Connection = connLog;
                cmdLog.CommandType = CommandType.StoredProcedure;
                cmdLog.CommandText = "LogRequest_XAV";

                SqlParameter pUserName = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                SqlParameter pTargetUrl = new SqlParameter("@TargetUrl", SqlDbType.VarChar, 200);
                SqlParameter pAddress = new SqlParameter("@Address", SqlDbType.VarChar, 200);
                SqlParameter pCity = new SqlParameter("@City", SqlDbType.VarChar, 200);
                SqlParameter pState = new SqlParameter("@State", SqlDbType.VarChar, 50);
                SqlParameter pZip = new SqlParameter("@Zip", SqlDbType.VarChar, 50);
                SqlParameter pCountry = new SqlParameter("@Country", SqlDbType.VarChar, 2);
                SqlParameter pRequestXml = new SqlParameter("@RequestXml", SqlDbType.NText);
                SqlParameter pResponseXml = new SqlParameter("@ResponseXml", SqlDbType.NText);

                pUserName.Value = UserName;
                pTargetUrl.Value = targetUrl;
                pAddress.Value = valAddress;
                pCity.Value = valCity;
                pState.Value = valState;
                pZip.Value = valZip;
                pCountry.Value = valCountry;
                pRequestXml.Value = requestXml;
                pResponseXml.Value = responseXml;

                cmdLog.Parameters.Add(pUserName);
                cmdLog.Parameters.Add(pTargetUrl);
                cmdLog.Parameters.Add(pAddress);
                cmdLog.Parameters.Add(pCity);
                cmdLog.Parameters.Add(pState);
                cmdLog.Parameters.Add(pZip);
                cmdLog.Parameters.Add(pCountry);
                cmdLog.Parameters.Add(pRequestXml);
                cmdLog.Parameters.Add(pResponseXml);

                cmdLog.ExecuteNonQuery();
                connLog.Close();
            }
            catch (Exception e)
            {
                handleError("ValidateAndRateAddress", "Error in saving request to DB: " + e.Message);
                sbResults.Append("Error in saving request to DB: " + e.Message + "<br/>");
            }
            #endregion

            if (xavResponse.Response.Alert != null)
            {
                UpsAddressValidationWebReference.CodeDescriptionType[] alerts = xavResponse.Response.Alert;
                foreach (UpsAddressValidationWebReference.CodeDescriptionType alert in alerts)
                {
                    sbResults.Append("Alert: " + alert.Code + " - " + alert.Description + "<br/>");
                }
            }

            string addressClassCode = xavResponse.AddressClassification.Code;

            sbResults.Append("Address Classification: " + addressClassCode + " - " + xavResponse.AddressClassification.Description + "<br/>");

            int numCandidates = 0;
            string candidateClassification = "";
            string candidateAttention = "";
            string candidateConsignee = "";
            string candidateAddress = "";
            string candidateCity = "";
            string candidateState = "";
            string candidateZip = "";

            DataSet dsCandidates = new DataSet();
            DataTable tblCandidates = dsCandidates.Tables.Add();

            tblCandidates.Columns.Add("ID", typeof(int));
            tblCandidates.Columns.Add("Attention", typeof(string));
            tblCandidates.Columns.Add("Consignee", typeof(string));
            tblCandidates.Columns.Add("AddressLine", typeof(string));
            tblCandidates.Columns.Add("City", typeof(string));
            tblCandidates.Columns.Add("State", typeof(string));
            tblCandidates.Columns.Add("Zip", typeof(string));
            tblCandidates.Columns.Add("DisplayAddress", typeof(string));

            DataSet dsCandidateData = new DataSet();
            DataTable tblCandidateData = dsCandidateData.Tables.Add();
            tblCandidateData.Columns.Add("ID", typeof(int));
            tblCandidateData.Columns.Add("Attention", typeof(string));
            tblCandidateData.Columns.Add("Consignee", typeof(string));
            tblCandidateData.Columns.Add("AddressLine", typeof(string));
            tblCandidateData.Columns.Add("City", typeof(string));
            tblCandidateData.Columns.Add("State", typeof(string));
            tblCandidateData.Columns.Add("Zip", typeof(string));

            if (xavResponse.Candidate != null)
            {
                CandidateType[] candidates = xavResponse.Candidate;
                numCandidates = candidates.Count();
                sbResults.Append("Number of Candidates: " + numCandidates + "<br/>");
                int candidateCount = 0;
                foreach (CandidateType candidate in candidates)
                {
                    candidateCount++;

                    candidateClassification = candidate.AddressClassification.Code;

                    sbResults.Append("Candidate #" + candidateCount.ToString() + " Classification: " + candidateClassification + " - " + candidate.AddressClassification.Description + "<br/>");

                    if (candidate.AddressKeyFormat.AttentionName != null)
                    {
                        candidateAttention = candidate.AddressKeyFormat.AttentionName;
                        sbResults.Append("Attention: " + candidateAttention + "<br/>");
                    }
                    else { candidateAttention = ""; }

                    if (candidate.AddressKeyFormat.ConsigneeName != null)
                    {
                        candidateConsignee = candidate.AddressKeyFormat.ConsigneeName;
                        sbResults.Append("Consignee: " + candidateConsignee + "<br/>");
                    }
                    else { candidateConsignee = ""; }

                    candidateAddress = "";
                    for (int i = 0; i < candidate.AddressKeyFormat.AddressLine.Count(); i++)
                    {
                        candidateAddress += candidate.AddressKeyFormat.AddressLine[i];
                        if ((i + 1) < candidate.AddressKeyFormat.AddressLine.Count())
                        {
                            candidateAddress += " ";
                        }
                        sbResults.Append(candidate.AddressKeyFormat.AddressLine[i] + "<br/>");
                    }

                    candidateCity = candidate.AddressKeyFormat.PoliticalDivision2;
                    candidateState = candidate.AddressKeyFormat.PoliticalDivision1;
                    candidateZip = candidate.AddressKeyFormat.PostcodePrimaryLow + "-" + candidate.AddressKeyFormat.PostcodeExtendedLow;

                    sbResults.Append(candidateCity + " " + candidateState + " " + candidateZip + "<br/>");

                    string displayAddress = "";
                    if (candidateAttention != "")
                    {
                        displayAddress += candidateAttention + "\n";
                    }
                    if (candidateConsignee != "")
                    {
                        displayAddress += candidateConsignee + "\n";
                    }
                    displayAddress += candidateAddress + "\n";
                    displayAddress += candidateCity + " " + candidateState + " " + candidateZip;

                    tblCandidates.Rows.Add(candidateCount, candidateAttention, candidateConsignee, candidateAddress, candidateCity, candidateState, candidateZip, displayAddress);
                    tblCandidateData.Rows.Add(candidateCount, candidateAttention, candidateConsignee, candidateAddress, candidateCity, candidateState, candidateZip);


                    sbResults.Append("<br/>");
                } // end of foreach candidate loop
            }


            if (numCandidates == 1)
            {
                string selectedState = GetSelectedState();

                if ((candidateAddress.ToUpper() != txtAddress.Text.Trim().ToUpper())
                 || (candidateCity.ToUpper() != txtCity.Text.Trim().ToUpper())
                 || (candidateState.ToUpper() != selectedState.ToUpper())
                 || (candidateZip.ToUpper() != txtZip.Text.Trim().ToUpper()))
                {
                    bool AutoAcceptAddressCorrections = true;

                    lblEnteredAddressStreet.Text = txtAddress.Text.Trim().ToUpper();
                    lblEnteredAddressCity.Text = txtCity.Text.Trim().ToUpper();
                    lblEnteredAddressState.Text = selectedState.ToUpper();
                    lblEnteredAddressZip.Text = txtZip.Text.Trim().ToUpper();

                    lblCorrectedAddressStreet.Text = candidateAddress.ToUpper();
                    lblCorrectedAddressCity.Text = candidateCity.ToUpper();
                    lblCorrectedAddressState.Text = candidateState.ToUpper();
                    lblCorrectedAddressZip.Text = candidateZip.ToUpper();

                    lblChangeToAddressStreet.Visible = (lblEnteredAddressStreet.Text != lblCorrectedAddressStreet.Text);
                    lblChangeToAddressCity.Visible = (lblEnteredAddressCity.Text != lblCorrectedAddressCity.Text);
                    lblChangeToAddressState.Visible = (lblEnteredAddressState.Text != lblCorrectedAddressState.Text);
                    lblChangeToAddressZip.Visible = (lblEnteredAddressZip.Text != lblCorrectedAddressZip.Text);

                    if (AutoAcceptAddressCorrections)
                    {
                        sbResults.Append("  -- not an exact match, but auto-accepting UPS address correction <BR/>");
                        RateWithCorrectedAddress();
                    }
                    else
                    {
                        mvPageLayout.SetActiveView(vwVerifyAddressCorrection);
                        pnlUpsTrademarkInfo.Visible = true;
                    }
                }
                else // Our address exactly matches the UPS candidate, so move forward
                {
                    sbResults.Append(" -- EXACT MATCH -- Continuing to Rating -- ");

                    RateWithEnteredAddress();

                }
            } // end of numCandidates == 1 logic
            else if (numCandidates > 1)
            {

                string addressedSubmitted = txtAddress.Text.Trim() + "<br/>";
                addressedSubmitted += txtCity.Text.Trim() + " " + txtState.Text.Trim() + " " + txtZip.Text.Trim();
                lblAddressSubmitted.Text = addressedSubmitted.ToUpper();

                DataView dvCandidates = tblCandidates.DefaultView;
                dvCandidates.Sort = "ID";

                gvCandidates.DataSource = dvCandidates;
                gvCandidates.DataBind();

                gvCandidateData.DataSource = dvCandidates;
                gvCandidateData.DataBind();

                mvPageLayout.SetActiveView(vwSelectAddressCandidate);
                pnlUpsTrademarkInfo.Visible = false;
            }


        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            handleError("ValidateAndRateAddress", "Soap Exception - SoapException Message= " + ex.Message + " SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + " SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + " SoapException StackTrace= " + ex.StackTrace);
            #region -- Handle SOAP error --
            sbResults.Append("---------XAV Web Service returns error----------------<br/>");
            sbResults.Append("---------\"Hard\" is user error \"Transient\" is system error----------------<br/>");
            sbResults.Append("SoapException Message= " + ex.Message + "<br/>");
            sbResults.Append("<br/>");
            sbResults.Append("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + "<br/>");
            sbResults.Append("<br/>");
            sbResults.Append("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + "<br/>");
            sbResults.Append("<br/>");
            sbResults.Append("SoapException StackTrace= " + ex.StackTrace + "<br/>");
            sbResults.Append("-------------------------<br/>");
            #endregion
        }
        catch (System.ServiceModel.CommunicationException ex)
        {
            handleError("ValidateAndRateAddress", "Communication Exception - CommunicationException= " + ex.Message + " CommunicationException-StackTrace= " + ex.StackTrace);
            #region -- Handle General Communication error --
            sbResults.Append("<br/>");
            sbResults.Append("--------------------<br/>");
            sbResults.Append("CommunicationException= " + ex.Message + "<br/>");
            sbResults.Append("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
            sbResults.Append("-------------------------<br/>");
            sbResults.Append("<br/>");
            #endregion
        }
        catch (Exception ex)
        {
            handleError("ValidateAndRateAddress", "General Error - General Exception= " + ex.Message + " General Exception-StackTrace= " + ex.StackTrace);
            #region -- Handle misc error --
            sbResults.Append("<br/>");
            sbResults.Append("-------------------------<br/>");
            sbResults.Append(" General Exception= " + ex.Message + "<br/>");
            sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
            sbResults.Append("-------------------------<br/>");
            #endregion
        }
        //lblResults.Text += sbResults.ToString();
    }

    private void ShowLTLRates(string shipToCity, string shipToState, string shipToZip, string shipToCountry, string plantShortCode, int shipWeight, int numPackages)
    {
        //Response.Write("In ShowLTLRates<br/>");
        int acctNumber = 0;
        Int32.TryParse(txtAcctNumber.Text.Trim(), out acctNumber);
        try
        {
            #region -- Define Per Package Charge and Per Shipment Charge dictionaries for LTL (charge per plant) --
            Dictionary<string, double> dPerPackageChargeLTL = new Dictionary<string, double>();
            Dictionary<string, double> dPerShipmentChargeLTL = new Dictionary<string, double>();
            Dictionary<string, double> dUpchargeLTL = new Dictionary<string, double>();

            SqlConnection connCharges = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
            connCharges.Open();

            SqlCommand cmdCharges = new SqlCommand();
            cmdCharges.Connection = connCharges;
            //cmdCharges.CommandType = CommandType.Text;
            //cmdCharges.CommandText = "SELECT PlantCode, PerPackageCharge, PerShipmentCharge, Ground FROM PlantCarrierCharges WHERE CarrierId = 'M33' ORDER BY PlantCode";

            cmdCharges.CommandText = "GetPlantCharges";
            cmdCharges.CommandType = CommandType.StoredProcedure;

            cmdCharges.Parameters.Add("@Carrier", SqlDbType.VarChar, 50).Value = "M33";
            cmdCharges.Parameters.Add("@AcctNumber", SqlDbType.Int).Value = acctNumber;

            SqlDataReader drCharges = cmdCharges.ExecuteReader();

            while (drCharges.Read())
            {
                dPerPackageChargeLTL.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerPackageCharge"].ToString()));
                dPerShipmentChargeLTL.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerShipmentCharge"].ToString()));
                dUpchargeLTL.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["Ground"].ToString()));
            }

            connCharges.Close();

            #endregion

            StringBuilder sbResults = new StringBuilder();

            //string url = Config.M33Url;
            //string token = Config.M33Token;
            string url = Config.TransPlaceUrl;
            string token = Config.TransPlaceToken;

            string fullPostData = "";

            string pickupDate = "";

            //Response.Write("In ShowLTLRates - a<br/>");
            try
            {
                DateTime datePickup = DateTime.Parse(txtPickupDate.Text);
                pickupDate = datePickup.Year.ToString() + "-" + datePickup.Month.ToString() + "-" + datePickup.Day.ToString();
            }
            catch (Exception e)
            {
                pickupDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            }
            string ltlClass = lstLTLClass.SelectedValue.ToString();

            #region -- Define tblLTLServices to hold rate data --

            DataSet dsLTLServices = new DataSet();
            DataTable tblLTLServices = dsLTLServices.Tables.Add();

            tblLTLServices.Columns.Add("Plant", typeof(string));
            tblLTLServices.Columns.Add("Service", typeof(string));
            tblLTLServices.Columns.Add("Rate", typeof(double));
            tblLTLServices.Columns.Add("TransitDays", typeof(int));
            tblLTLServices.Columns.Add("Direct", typeof(string));

            #endregion

            //Response.Write("In ShowLTLRates - b<br/>");

            string[] plantCodes = { "" };

            if (plantShortCode == "ALL")
            {
                plantCodes = Config.PlantCodesMultiRate;
            }
            else
            {
                plantCodes[0] = plantShortCode;
            }

            string combinedResponses = "";
            foreach (string plantCode in plantCodes)
            {

                WebRequest request = WebRequest.Create(url + "rate/quote?loginToken=" + token);
                request.Method = "POST";

                #region -- Load Plant Ship From Address --
                string shipFromCity = "";
                string shipFromState = "";
                string shipFromZip = "";
                string shipFromCountry = "";

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT City, State, Zip, Country FROM Plants WHERE PlantCode = '" + plantCode + "'";

                SqlDataReader drResults = cmd.ExecuteReader();

                if (drResults.Read())
                {
                    shipFromCity = drResults["City"].ToString();
                    shipFromState = drResults["State"].ToString();
                    shipFromZip = drResults["Zip"].ToString();
                    shipFromCountry = drResults["Country"].ToString();
                }
                else
                {
                    //error looking up Address Info
                    sbResults.Append("Unable to lookup address info for Plant " + plantCode + "<br/>");
                }

                conn.Close();

                #endregion

                string postData = "<?xml version=\"1.0\"?>";
                postData += "<quote>";
                postData += "<requestedMode>LTL</requestedMode>";
                postData += "<requestedPickupDate>" + pickupDate + "</requestedPickupDate>";
                postData += "<shipper>";
                postData += "<city>" + shipFromCity + "</city>";
                postData += "<region>" + shipFromState + "</region>";
                postData += "<country>" + shipFromCountry + "</country>";
                postData += "<postalCode>" + shipFromZip + "</postalCode>";
                postData += "</shipper>";
                postData += "<consignee>";
                postData += "<city>" + shipToCity + "</city>";
                postData += "<region>" + shipToState + "</region>";
                postData += "<country>" + shipToCountry + "</country>";
                postData += "<postalCode>" + shipToZip + "</postalCode>";
                postData += "</consignee>";
                postData += "<lineItems>";
                postData += "<lineItem>";
                postData += "<freightClass>" + ltlClass + "</freightClass>";
                postData += "<weight>" + shipWeight + "</weight>";
                postData += "<weightUnit>LB</weightUnit>";
                postData += "</lineItem>";
                postData += "</lineItems>";

                string accessorials = GetLTLAccessorials();
                if (accessorials.Length > 0)
                {
                    postData += "<accessorials>";
                    string[] accessorialArray = accessorials.Split(';');
                    for (int i = 0; i < accessorialArray.Count(); i++)
                    {
                        postData += "<accessorial><type>" + accessorialArray[i] + "</type></accessorial>";
                    }
                    postData += "</accessorials>";
                }
                //postData += "<accessorials>";
                //postData += "<accessorial>";
                //postData += "<type>LIFTGATE-PICKUP</type>";
                //postData += "</accessorial>";
                //postData += "</accessorials>";
                postData += "</quote>";

                fullPostData += postData;

                lblResults.Text = postData;
                lblResults.Text += "\n\n\n";
                lblResults.Text += "<br/>" + url + "rate/quote?loginToken=" + token;
                lblResults.Text += "\n\n\n";

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.
                request.ContentType = "text/xml";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse WebResponse = request.GetResponse();
                // Display the status.
                //Console.WriteLine(((HttpWebResponse)WebResponse).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = WebResponse.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                lblResults.Text += responseFromServer;
                lblResults.Text += "\n\n\n";
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                WebResponse.Close();

                combinedResponses += responseFromServer;
                

                XDocument xmlDoc = XDocument.Parse(responseFromServer);

                foreach (var rate in xmlDoc.Descendants("rate"))
                {
                    string carrier = rate.Element("carrier").Element("name").Value.Trim();
                    string direct = rate.Element("direct").Value.Trim();
                    int transitDays = Convert.ToInt16(rate.Element("transitDays").Value.Trim());
                    double cost = double.Parse(rate.Element("cost").Element("totalAmount").Value.Trim());
                    double totalCharges = 0;

                    #region -- Define variables for markup calculations --
                    double markupPercentage = 0;
                    double perPackageCharge = 0;
                    double perShipmentCharge = 0;
                    #endregion

                    /* Temporarily disable markup for testing for Transplace - JMM - 4/8/22
                     * 
                    #region -- Determine the LTL markup percentage --
                    if (dPerPackageChargeLTL.ContainsKey(plantCode))
                    {
                        markupPercentage = Convert.ToDouble(dUpchargeLTL[plantCode]);
                        perPackageCharge = dPerPackageChargeLTL[plantCode];
                        perShipmentCharge = dPerShipmentChargeLTL[plantCode];
                    }
                    else
                    {
                        sbResults.Append("Unable to find LTL markup charges for plant code: " + plantCode + "<br/>");
                    }
                    #endregion
                    */

                    lblResults.Text += "Cost is " + cost.ToString() + "\n";
                    lblResults.Text += "Markup percentage is " + markupPercentage.ToString() + "\n";
                    lblResults.Text += "Number of Packages is " + numPackages.ToString() + "\n";
                    lblResults.Text += "Per package charge is " + perPackageCharge.ToString() + "\n";
                    lblResults.Text += "Per shipment charge is " + perShipmentCharge.ToString() + "\n";

                    totalCharges = cost;
                    totalCharges += ((markupPercentage / 100) * cost);
                    totalCharges += (perPackageCharge * numPackages) + perShipmentCharge;

                    lblResults.Text += "Calculated total charge is " + totalCharges.ToString() + "\n\n";

                    //if ((carrier != "LTL BENCHMARK") || (Session["DefaultPlant"].ToString() == "POR"))
                    if (carrier != "LTL BENCHMARK")
                    {
                        tblLTLServices.Rows.Add(plantCode, carrier, totalCharges, transitDays, direct);
                    }
                    else if (Session["DefaultPlant"].ToString() == "POR")
                    {
                        tblLTLServices.Rows.Add(plantCode, carrier, cost, transitDays, direct);
                    }

                }
            }

            #region -- Display Shopped Service Rates in Gridview --

            //Response.Write("In ShowLTLRates - cc<br/>");

            pnlLTLRates.Visible = (tblLTLServices.Rows.Count > 0);
            DataView dvLTLRates = tblLTLServices.DefaultView;

            gvLTLRates.DataSource = dvLTLRates;
            gvLTLRates.DataBind();

            gvLTLRates.Visible = true;


            //Response.Write("In ShowLTLRates - dd<br/>");

            #region -- log results --
            try
            {
                string UserName = HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();

                System.IO.StringWriter swGridResults = new System.IO.StringWriter();

                System.Web.UI.HtmlTextWriter htwGridResults = new HtmlTextWriter(swGridResults);

                gvLTLRates.RenderControl(htwGridResults);

                SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                connLog.Open();

                SqlCommand cmdLog = new SqlCommand();
                cmdLog.Connection = connLog;
                cmdLog.CommandType = CommandType.StoredProcedure;
                cmdLog.CommandText = "LogResultsLTL";

                SqlParameter pPlantCode = new SqlParameter("@PlantCode", SqlDbType.VarChar, 10);
                SqlParameter pUserName = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                SqlParameter pFullRequest = new SqlParameter("@FullRequest", SqlDbType.NText);
                SqlParameter pFullResults = new SqlParameter("@FullResults", SqlDbType.NText);
                SqlParameter pXmlResponse = new SqlParameter("@XmlResponse", SqlDbType.NText);

                pPlantCode.Value = plantShortCode;
                pUserName.Value = UserName;
                pFullRequest.Value = fullPostData;
                pFullResults.Value = swGridResults.ToString();
                pXmlResponse.Value = ""; // combinedResponses;  // <- Combined Responses only saved during testing for performance reasons

                cmdLog.Parameters.Add(pPlantCode);
                cmdLog.Parameters.Add(pUserName);
                cmdLog.Parameters.Add(pFullRequest);
                cmdLog.Parameters.Add(pFullResults);
                cmdLog.Parameters.Add(pXmlResponse);

                cmdLog.ExecuteNonQuery();
                connLog.Close();
            }
            catch (Exception e)
            {
                handleError("ShowLTLRates", "Error in saving results to DB: " + e.Message);
                sbResults.Append("Error in saving results to DB: " + e.Message + "<br/>");
                lblResults.Text += sbResults.ToString();
                lblResults.Visible = true;
            }
            #endregion

            #endregion
 
           
        }
        catch (Exception e)
        {

            //Response.Write("In ShowLTLRates - err - " + e.ToString() +"<br/>");
            handleError("ShowLTLRates", e.ToString());
            lblResults.Text += e.ToString();
            lblResults.Visible = true;
        }

        lblResults.Visible = true;
    }

    public string GetLTLAccessorialsForDisplay()
    {
        string basicList = GetLTLAccessorials();
        basicList = basicList.Replace(";", "<br/>&nbsp;&nbsp;");

        if (basicList == "")
        {
            basicList = "NONE";
        }
        else
        {
            basicList = "<br/>&nbsp;&nbsp;" + basicList;
        }

        return basicList;
    }

    private string GetLTLAccessorials() 
    {
        string fullList = "";

        if(chkLTLNotifyBeforeDelivery.Checked) {
            fullList += "NOTIFY-BEFORE-DELIVERY;";
        }
        if(chkLTLLiftgatePickup.Checked) {
            fullList += "LIFTGATE-PICKUP;";
        }
        if(chkLTLLiftgateDelivery.Checked) {
            fullList += "LIFTGATE-DELIVERY;";
        }
        if(chkLTLLimitedAccessPickup.Checked) {
            fullList += "LIMITED-ACCESS-PICKUP;";
        }
        if(chkLTLLimitedAccessDelivery.Checked) {
            fullList += "LIMITED-ACCESS-DELIVERY;";
        }
        if(chkLTLResidentialPickup.Checked) {
            fullList += "RESIDENTIAL-PICKUP;";
        }
        if(chkLTLResidentialDelivery.Checked) {
            fullList += "RESIDENTIAL-DELIVERY;";
        }
        if(chkLTLInsidePickup.Checked) {
            fullList += "INSIDE-PICKUP;";
        }
        if(chkLTLInsideDelivery.Checked) {
            fullList += "INSIDE-DELIVERY;";
        }
        if(chkLTLSortAndSegregate.Checked) {
            fullList += "SORT-AND-SEGREGATE;";
        }
        if(chkLTLStopoffCharge.Checked) {
            fullList += "STOPOFF-CHARGE;";
        }

        if(fullList.Length > 0) {
            fullList = fullList.Substring(0,fullList.Length - 1);
        }

        return fullList;
    }
    private void ShowGFRate(string shipToConsignee, string shipToStreet, string shipToCity, string shipToState, string shipToZip, string shipToCountry, string plantCode, bool addressCorrected)
    {
        int acctNumber = 0;
        Int32.TryParse(txtAcctNumber.Text.Trim(), out acctNumber);

        lblRateAddress.Text = shipToStreet;
        lblRateCity.Text = shipToCity;
        lblRateState.Text = shipToState;
        lblRateZip.Text = shipToZip;

        StringBuilder sbResults = new StringBuilder();

        bool SoapError = false;

        double shipmentWeight = 0;
        int shipmentClassification = 0;
        
        
        Dictionary<string, double> dPerPackageChargeGF = new Dictionary<string, double>();
        Dictionary<string, double> dPerShipmentChargeGF = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeGF = new Dictionary<string, double>();

        SqlConnection connCharges = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
        connCharges.Open();

        SqlCommand cmdCharges = new SqlCommand();
        cmdCharges.Connection = connCharges;

        cmdCharges.CommandText = "GetPlantCharges";
        cmdCharges.CommandType = CommandType.StoredProcedure;

        cmdCharges.Parameters.Add("@Carrier", SqlDbType.VarChar, 50).Value = "GF";
        cmdCharges.Parameters.Add("@AcctNumber", SqlDbType.Int).Value = acctNumber;

        SqlDataReader drCharges = cmdCharges.ExecuteReader();

        while (drCharges.Read())
        {
            dPerPackageChargeGF.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerPackageCharge"].ToString()));
            dPerShipmentChargeGF.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerShipmentCharge"].ToString()));
            dUpchargeGF.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["Ground"].ToString()));
        }

        connCharges.Close();

        
        int numPackages = Convert.ToInt16(txtNumPackages.Text.Trim());
        string sPkgWeight = txtPackageWeight.Text.Trim();
        string sLastPkgWeight = txtLastPackageWeight.Text.Trim();

        if (sLastPkgWeight == "")
        {
            sLastPkgWeight = sPkgWeight;
        }

        int totalWeight = (Convert.ToInt16(sPkgWeight) * (numPackages - 1)) + Convert.ToInt16(sLastPkgWeight);

        sbResults.Append("Total weight: " + totalWeight.ToString() + "<br/>");
        

        if (plantCode != "ALL")
        {
            // -- Process rate request for a single plant --
            try
            {

                int RateRequestId = 0;

                // -- Load Plant Ship From Address --
                string shipFromAddress = "";
                string shipFromCity = "";
                string shipFromState = "";
                string shipFromZip = "";
                string shipFromCountry = "";

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Address, City, State, Zip, Country FROM Plants WHERE PlantCode = '" + plantCode + "'";

                SqlDataReader drResults = cmd.ExecuteReader();

                if (drResults.Read())
                {
                    shipFromAddress = drResults["Address"].ToString();
                    shipFromCity = drResults["City"].ToString();
                    shipFromState = drResults["State"].ToString();
                    shipFromZip = drResults["Zip"].ToString();
                    shipFromCountry = drResults["Country"].ToString();
                }
                else
                {
                    //error looking up Address Info
                    sbResults.Append("Unable to lookup address info for Plant " + plantCode + "<br/>");
                }

                conn.Close();
                

                // -- Begin building Rate Request --
                RateService rateService = new RateService();
                RateRequest rateRequest = new RateRequest();

                UpsRateWebReference.RequestType request = new UpsRateWebReference.RequestType();
                String[] requestOption = { "Rate" }; // Needed for GF
                request.RequestOption = requestOption;
                rateRequest.Request = request;
                

                // -- Access Security (license number, username, password) --
                UpsRateWebReference.UPSSecurity upss = new UpsRateWebReference.UPSSecurity();
                UpsRateWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsRateWebReference.UPSSecurityServiceAccessToken();
                upsSvcToken.AccessLicenseNumber = Config.UPSAccessKey;
                upss.ServiceAccessToken = upsSvcToken;
                UpsRateWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsRateWebReference.UPSSecurityUsernameToken();
                upsSecUsrnameToken.Username = Config.UPSUserName;
                upsSecUsrnameToken.Password = Config.UPSPassword;
                upss.UsernameToken = upsSecUsrnameToken;
                rateService.UPSSecurityValue = upss;
                

                // -- Build Shipment object --
                ShipmentType shipment = new ShipmentType();

                // -- Shipper --
                ShipperType shipper = new ShipperType();
                shipper.ShipperNumber = Config.ShipFromShipperNumber;
                AddressType shipperAddress = new AddressType();
                String[] shipperAddressLine = { shipFromAddress };
                shipperAddress.AddressLine = shipperAddressLine;
                shipperAddress.City = shipFromCity;
                shipperAddress.StateProvinceCode = shipFromState;
                shipperAddress.PostalCode = shipFromZip;
                shipperAddress.CountryCode = shipFromCountry;
                shipper.Address = shipperAddress;

                shipment.Shipper = shipper;
                

                // -- Ship To --
                ShipToType shipTo = new ShipToType();
                ShipToAddressType shipToAddress = new ShipToAddressType();
                String[] shipToAddressLines = { shipToStreet };
                shipToAddress.AddressLine = shipToAddressLines;
                shipToAddress.City = shipToCity;
                shipToAddress.StateProvinceCode = shipToState;
                shipToAddress.PostalCode = shipToZip;
                shipToAddress.CountryCode = shipToCountry;

                shipTo.Address = shipToAddress;

                shipment.ShipTo = shipTo;

                

                // -- Packages --
                PackageType package;
                PackageWeightType packageWeight;
                UpsRateWebReference.CodeDescriptionType uomCodeDesc;

                UpsRateWebReference.CodeDescriptionType packageTypeCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                packageTypeCodeDesc.Code = "00";

                string deliveryConfirmationOption = lstDeliveryConfirmation.SelectedValue;

                PackageType[] packages = new PackageType[numPackages];
                for (int i = 0; i < numPackages; i++)
                {
                    package = new PackageType();
                    packageWeight = new PackageWeightType();
                    uomCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                    uomCodeDesc.Code = "LBS";
                    if (i == (numPackages - 1))
                    {
                        packageWeight.Weight = sLastPkgWeight;
                    }
                    else
                    {
                        packageWeight.Weight = sPkgWeight;
                    }
                    packageWeight.UnitOfMeasurement = uomCodeDesc;
                    package.PackageWeight = packageWeight;

                    package.PackagingType = packageTypeCodeDesc;

                    if (deliveryConfirmationOption != "0")
                    {
                        PackageServiceOptionsType packageServiceOptions = new PackageServiceOptionsType();
                        DeliveryConfirmationType deliveryConfirmation = new DeliveryConfirmationType();
                        deliveryConfirmation.DCISType = deliveryConfirmationOption;
                        packageServiceOptions.DeliveryConfirmation = deliveryConfirmation;
                        package.PackageServiceOptions = packageServiceOptions;
                    }

                    //Needed for GF
                    CommodityType commodity = new CommodityType();
                    commodity.FreightClass = lstLTLClass.SelectedValue;
                    package.Commodity = commodity;

                    packages[i] = package;
                }

                shipment.Package = packages;
                
                
                ShipmentRatingOptionsType ratingOptions = new ShipmentRatingOptionsType();
                ratingOptions.FRSShipmentIndicator = "";
                ratingOptions.NegotiatedRatesIndicator = "";
                shipment.ShipmentRatingOptions = ratingOptions;
                
                UpsRateWebReference.CodeDescriptionType service = new UpsRateWebReference.CodeDescriptionType();
                service.Code = "03";
                shipment.Service = service;

                UpsRateWebReference.FRSPaymentInfoType frsPaymentInfo = new UpsRateWebReference.FRSPaymentInfoType();
                UpsRateWebReference.CodeDescriptionType frsPaymentInfoType = new UpsRateWebReference.CodeDescriptionType();

                frsPaymentInfoType.Code = "01";
                frsPaymentInfo.Type = frsPaymentInfoType;

                shipment.FRSPaymentInformation = frsPaymentInfo;

                rateRequest.Shipment = shipment;
                

                // -- Submit Rate Request --

                RateResponse rateResponse = rateService.ProcessRate(rateRequest);

                RatedShipmentType[] ratedShipments = rateResponse.RatedShipment;

                string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
                string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

                //Response.Write("<br/><br/><br/>");
               // Response.Write(requestXml);
               // Response.Write("<br/><br/><br/>");
                //Response.Write(responseXml);
               // Response.Write("<br/><br/><br/>");

                string targetUrl = rateService.Url;
                string UserName = HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();

                // -- log request --
                try
                {
                    SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                    connLog.Open();

                    SqlCommand cmdLog = new SqlCommand();
                    cmdLog.Connection = connLog;
                    cmdLog.CommandType = CommandType.StoredProcedure;
                    cmdLog.CommandText = "LogRequest_Rate";

                    SqlParameter pUserName = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                    SqlParameter pTargetUrl = new SqlParameter("@TargetUrl", SqlDbType.VarChar, 200);
                    SqlParameter pAddress = new SqlParameter("@Address", SqlDbType.VarChar, 200);
                    SqlParameter pCity = new SqlParameter("@City", SqlDbType.VarChar, 200);
                    SqlParameter pState = new SqlParameter("@State", SqlDbType.VarChar, 50);
                    SqlParameter pZip = new SqlParameter("@Zip", SqlDbType.VarChar, 50);
                    SqlParameter pCountry = new SqlParameter("@Country", SqlDbType.VarChar, 2);
                    SqlParameter pRequestXml = new SqlParameter("@RequestXml", SqlDbType.NText);
                    SqlParameter pResponseXml = new SqlParameter("@ResponseXml", SqlDbType.NText);
                    SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);

                    pUserName.Value = UserName;
                    pTargetUrl.Value = targetUrl;
                    pAddress.Value = shipToStreet;
                    pCity.Value = shipToCity;
                    pState.Value = shipToState;
                    pZip.Value = shipToZip;
                    pCountry.Value = shipToCountry;
                    pRequestXml.Value = requestXml;
                    pResponseXml.Value = responseXml;
                    pRequestId.Direction = ParameterDirection.Output;

                    cmdLog.Parameters.Add(pUserName);
                    cmdLog.Parameters.Add(pTargetUrl);
                    cmdLog.Parameters.Add(pAddress);
                    cmdLog.Parameters.Add(pCity);
                    cmdLog.Parameters.Add(pState);
                    cmdLog.Parameters.Add(pZip);
                    cmdLog.Parameters.Add(pCountry);
                    cmdLog.Parameters.Add(pRequestXml);
                    cmdLog.Parameters.Add(pResponseXml);
                    cmdLog.Parameters.Add(pRequestId);

                    cmdLog.ExecuteNonQuery();

                    RateRequestId = Convert.ToInt32(pRequestId.Value);

                    connLog.Close();
                }
                catch (Exception e)
                {
                    handleError("ShowRates", "Error in saving request to DB: " + e.Message);
                    sbResults.Append("Error in saving request to DB: " + e.Message + "<br/>");
                }
                

                

                // -- Define tblServices to hold rate data --

                DataSet dsServices = new DataSet();
                DataTable tblServices = dsServices.Tables.Add();

                tblServices.Columns.Add("Plant", typeof(string));
                tblServices.Columns.Add("Desc", typeof(string));
                tblServices.Columns.Add("Rate", typeof(double));

                

                // -- Process each rated service --

                foreach (RatedShipmentType ratedShipment in ratedShipments)
                {
                    string serviceCode = "";
                    int addressClassification = 1;
                    double totalCharges = 0; //will pull from Negotiated
                    
                    serviceCode = ratedShipment.Service.Code;

                    totalCharges = Convert.ToDouble(ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                    
                    sbResults.Append("Negotiated charges: " + totalCharges.ToString() + "<br/>");
                    
                    if (ratedShipment.RatedShipmentAlert != null)
                    {
                        RatedShipmentInfoType[] ratedShipmentAlerts = ratedShipment.RatedShipmentAlert;
                        foreach (RatedShipmentInfoType ratedShipmentAlert in ratedShipmentAlerts)
                        {
                            sbResults.Append("Shipment Alert: " + ratedShipmentAlert.Code + " - " + ratedShipmentAlert.Description + "<br/>");
                        }
                    }

                    sbResults.Append("<br/>");

                    // -- Define variables for markup calculations --
                    double markupPercentage = 0;
                    double perPackageCharge = 0;
                    double perShipmentCharge = 0;

                    // -- Determine the UPS Standard markup percentage --
                    if (dPerPackageChargeGF.ContainsKey(plantCode))
                    {
                        markupPercentage = Convert.ToDouble(dUpchargeGF[plantCode]);
                        perPackageCharge = dPerPackageChargeGF[plantCode];
                        perShipmentCharge = dPerShipmentChargeGF[plantCode];
                    }
                    else
                    {
                        sbResults.Append("Unable to find UPS Ground Freight markup charges for plant code: " + plantCode + "<br/>");
                    }
                    
                    totalCharges += ((markupPercentage / 100) * totalCharges);
                    totalCharges += (perPackageCharge * numPackages) + perShipmentCharge;

                    tblServices.Rows.Add(plantCode, "Ground Freight", totalCharges);
                    
                }
                

                // -- Display Service Rates in Gridview --
                DataView dvServices = tblServices.DefaultView;
                dvServices.Sort = "Rate";

                gvGFRates.DataSource = dvServices;
                gvGFRates.DataBind();
                
                gvGFRates.Visible = true;
                pnlGFRates.Visible = true;

                // -- log results --
                try
                {
                    /*
                    System.IO.StringWriter swGridResults = new System.IO.StringWriter();

                    System.Web.UI.HtmlTextWriter htwGridResults = new HtmlTextWriter(swGridResults);

                    gvServices.RenderControl(htwGridResults);

                    SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                    connLog.Open();

                    SqlCommand cmdLog = new SqlCommand();
                    cmdLog.Connection = connLog;
                    cmdLog.CommandType = CommandType.StoredProcedure;
                    cmdLog.CommandText = "LogResults";

                    SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);
                    SqlParameter pPlantCode = new SqlParameter("@PlantCode", SqlDbType.VarChar, 10);
                    SqlParameter pFullResults = new SqlParameter("@FullResults", SqlDbType.NText);

                    pRequestId.Value = RateRequestId;
                    pPlantCode.Value = plantCode;
                    pFullResults.Value = swGridResults.ToString();

                    cmdLog.Parameters.Add(pRequestId);
                    cmdLog.Parameters.Add(pPlantCode);
                    cmdLog.Parameters.Add(pFullResults);

                    cmdLog.ExecuteNonQuery();
                    connLog.Close();
                    */
                }
                catch (Exception e)
                {
                    handleError("ShowRates", "Error in saving results to DB: " + e.Message);
                    sbResults.Append("Error in saving results to DB: " + e.Message + "<br/>");
                }
                


                

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                if ((ex.Detail.LastChild.InnerText != "Hard110003Maximum number of packages exceeded (50)") //Do not show "scary" red message if only soap error is that it exceeds 50 packages (also catch 200 pkg message)
                   && (ex.Detail.LastChild.InnerText != "Hard110003Maximum number of packages exceeded (200)"))
                {
                    handleError("ShowRates", "Soap Exception - SoapException Message= " + ex.Message + " SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + " SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + " SoapException StackTrace= " + ex.StackTrace);
                }
                // -- Handle SOAP error --
                SoapError = true;
                sbResults.Append("---------Freight Rate Web Service returns error----------------<br/>");
                sbResults.Append("---------\"Hard\" is user error \"Transient\" is system error----------------<br/>");
                sbResults.Append("SoapException Message= " + ex.Message + "<br/>");
                sbResults.Append("<br/>");
                sbResults.Append("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + "<br/>");
                sbResults.Append("<br/>");
                sbResults.Append("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + "<br/>");
                sbResults.Append("<br/>");
                sbResults.Append("SoapException StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                handleError("ShowRates", "Communication Exception - CommunicationException= " + ex.Message + " CommunicationException-StackTrace= " + ex.StackTrace);
                SoapError = true;
                // -- Handle General Communication Error --
                sbResults.Append("<br/>");
                sbResults.Append("--------------------<br/>");
                sbResults.Append("CommunicationException= " + ex.Message + "<br/>");
                sbResults.Append("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                sbResults.Append("<br/>");
                
            }
            catch (Exception ex)
            {
                handleError("ShowRates", "General Error - General Exception= " + ex.Message + " General Exception-StackTrace= " + ex.StackTrace);
                SoapError = true;
                // -- Handle misc error --
                sbResults.Append("<br/>");
                sbResults.Append("-------------------------<br/>");
                sbResults.Append(" General Exception= " + ex.Message + "<br/>");
                sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                
            }


            if (!SoapError)
            {
                lblResultsGF.Visible = false;
                //pnlUpsCommError.Visible = false;
            }
            else
            {
                lblResultsGF.Visible = true;
                //gvShopServices.Visible = false;
                gvGFRates.Visible = false;
                //litUpsCommError.Text = "There was a error getting Ground Freight results from UPS.  Please check your information and try again.";
               
                //pnlUpsCommError.Visible = true;
            }

            
        } //end of single Plant rating process
        else
        { // Shop rates for all plants
            // -- Process rate request for all plants and combine into a single dataset --
            // -- Define tblShopServices to hold rate data --
            
            DataSet dsShopServices = new DataSet();
            DataTable tblShopServices = dsShopServices.Tables.Add();

            tblShopServices.Columns.Add("Plant", typeof(string));
            tblShopServices.Columns.Add("Desc", typeof(string));
            tblShopServices.Columns.Add("Rate", typeof(double));

            int RateRequestId = 0;

            for (int iPlantCount = 0; iPlantCount < Config.PlantCodesMultiRate.Count(); iPlantCount++)
            {
                string currentPlant = Config.PlantCodesMultiRate[iPlantCount];

                // -- Process rate request for currentPlant --
                try
                {

                    // -- Load Plant Ship From Address --
                    string shipFromAddress = "";
                    string shipFromCity = "";
                    string shipFromState = "";
                    string shipFromZip = "";
                    string shipFromCountry = "";

                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT Address, City, State, Zip, Country FROM Plants WHERE PlantCode = '" + currentPlant + "'";

                    SqlDataReader drResults = cmd.ExecuteReader();

                    if (drResults.Read())
                    {
                        shipFromAddress = drResults["Address"].ToString();
                        shipFromCity = drResults["City"].ToString();
                        shipFromState = drResults["State"].ToString();
                        shipFromZip = drResults["Zip"].ToString();
                        shipFromCountry = drResults["Country"].ToString();
                    }
                    else
                    {
                        //error looking up Address Info
                        sbResults.Append("Unable to lookup address info for Plant " + currentPlant + "<br/>");
                    }

                    conn.Close();

                    

                    // -- Begin building Rate Request --
                    RateService rateService = new RateService();
                    RateRequest rateRequest = new RateRequest();

                    UpsRateWebReference.RequestType request = new UpsRateWebReference.RequestType();
                    String[] requestOption = { "Rate" };
                    request.RequestOption = requestOption;
                    rateRequest.Request = request;
                    

                    // -- Access Security (license number, username, password) --
                    UpsRateWebReference.UPSSecurity upss = new UpsRateWebReference.UPSSecurity();
                    UpsRateWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsRateWebReference.UPSSecurityServiceAccessToken();
                    upsSvcToken.AccessLicenseNumber = Config.UPSAccessKey;
                    upss.ServiceAccessToken = upsSvcToken;
                    UpsRateWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsRateWebReference.UPSSecurityUsernameToken();
                    upsSecUsrnameToken.Username = Config.UPSUserName;
                    upsSecUsrnameToken.Password = Config.UPSPassword;
                    upss.UsernameToken = upsSecUsrnameToken;
                    rateService.UPSSecurityValue = upss;
                    

                    // -- Build Shipment object --
                    ShipmentType shipment = new ShipmentType();

                    // -- Shipper --
                    ShipperType shipper = new ShipperType();
                    shipper.ShipperNumber = Config.ShipFromShipperNumber;
                    AddressType shipperAddress = new AddressType();
                    String[] shipperAddressLine = { shipFromAddress };
                    shipperAddress.AddressLine = shipperAddressLine;
                    shipperAddress.City = shipFromCity;
                    shipperAddress.StateProvinceCode = shipFromState;
                    shipperAddress.PostalCode = shipFromZip;
                    shipperAddress.CountryCode = shipFromCountry;
                    shipper.Address = shipperAddress;

                    shipment.Shipper = shipper;
                    

                    // -- Ship To --
                    ShipToType shipTo = new ShipToType();
                    ShipToAddressType shipToAddress = new ShipToAddressType();
                    String[] shipToAddressLines = { shipToStreet };
                    shipToAddress.AddressLine = shipToAddressLines;
                    shipToAddress.City = shipToCity;
                    shipToAddress.StateProvinceCode = shipToState;
                    shipToAddress.PostalCode = shipToZip;
                    shipToAddress.CountryCode = shipToCountry;

                    shipTo.Address = shipToAddress;

                    shipment.ShipTo = shipTo;

                    

                    // -- Packages --
                    PackageType package;
                    PackageWeightType packageWeight;
                    UpsRateWebReference.CodeDescriptionType uomCodeDesc;

                    UpsRateWebReference.CodeDescriptionType packageTypeCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                    packageTypeCodeDesc.Code = "00";

                    string deliveryConfirmationOption = lstDeliveryConfirmation.SelectedValue;

                    PackageType[] packages = new PackageType[numPackages];
                    for (int i = 0; i < numPackages; i++)
                    {
                        package = new PackageType();
                        packageWeight = new PackageWeightType();
                        uomCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                        uomCodeDesc.Code = "LBS";
                        if (i == (numPackages - 1))
                        {
                            packageWeight.Weight = sLastPkgWeight;
                        }
                        else
                        {
                            packageWeight.Weight = sPkgWeight;
                        }
                        packageWeight.UnitOfMeasurement = uomCodeDesc;
                        package.PackageWeight = packageWeight;

                        package.PackagingType = packageTypeCodeDesc;

                        if (deliveryConfirmationOption != "0")
                        {
                            PackageServiceOptionsType packageServiceOptions = new PackageServiceOptionsType();
                            DeliveryConfirmationType deliveryConfirmation = new DeliveryConfirmationType();
                            deliveryConfirmation.DCISType = deliveryConfirmationOption;
                            packageServiceOptions.DeliveryConfirmation = deliveryConfirmation;
                            package.PackageServiceOptions = packageServiceOptions;
                        }

                        //Needed for GF
                        CommodityType commodity = new CommodityType();
                        commodity.FreightClass = lstLTLClass.SelectedValue;
                        package.Commodity = commodity;

                        packages[i] = package;
                    }

                    shipment.Package = packages;


                    ShipmentRatingOptionsType ratingOptions = new ShipmentRatingOptionsType();
                    ratingOptions.FRSShipmentIndicator = "";
                    ratingOptions.NegotiatedRatesIndicator = "";
                    shipment.ShipmentRatingOptions = ratingOptions;

                    UpsRateWebReference.CodeDescriptionType service = new UpsRateWebReference.CodeDescriptionType();
                    service.Code = "03";
                    shipment.Service = service;

                    UpsRateWebReference.FRSPaymentInfoType frsPaymentInfo = new UpsRateWebReference.FRSPaymentInfoType();
                    UpsRateWebReference.CodeDescriptionType frsPaymentInfoType = new UpsRateWebReference.CodeDescriptionType();

                    frsPaymentInfoType.Code = "01";
                    frsPaymentInfo.Type = frsPaymentInfoType;

                    shipment.FRSPaymentInformation = frsPaymentInfo;

                    rateRequest.Shipment = shipment;
                  

                    // -- Submit Rate Request --

                    RateResponse rateResponse = rateService.ProcessRate(rateRequest);

                    RatedShipmentType[] ratedShipments = rateResponse.RatedShipment;

                    string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
                    string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

                    string targetUrl = rateService.Url;
                    string UserName = HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();

                    // -- log request --
                    try
                    {/*
                        SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                        connLog.Open();

                        SqlCommand cmdLog = new SqlCommand();
                        cmdLog.Connection = connLog;
                        cmdLog.CommandType = CommandType.StoredProcedure;
                        cmdLog.CommandText = "LogRequest_Rate";

                        SqlParameter pUserName = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                        SqlParameter pTargetUrl = new SqlParameter("@TargetUrl", SqlDbType.VarChar, 200);
                        SqlParameter pAddress = new SqlParameter("@Address", SqlDbType.VarChar, 200);
                        SqlParameter pCity = new SqlParameter("@City", SqlDbType.VarChar, 200);
                        SqlParameter pState = new SqlParameter("@State", SqlDbType.VarChar, 50);
                        SqlParameter pZip = new SqlParameter("@Zip", SqlDbType.VarChar, 50);
                        SqlParameter pCountry = new SqlParameter("@Country", SqlDbType.VarChar, 2);
                        SqlParameter pRequestXml = new SqlParameter("@RequestXml", SqlDbType.NText);
                        SqlParameter pResponseXml = new SqlParameter("@ResponseXml", SqlDbType.NText);
                        SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);

                        pUserName.Value = UserName;
                        pTargetUrl.Value = targetUrl;
                        pAddress.Value = shipToStreet;
                        pCity.Value = shipToCity;
                        pState.Value = shipToState;
                        pZip.Value = shipToZip;
                        pCountry.Value = shipToCountry;
                        pRequestXml.Value = requestXml;
                        pResponseXml.Value = responseXml;
                        pRequestId.Direction = ParameterDirection.Output;

                        cmdLog.Parameters.Add(pUserName);
                        cmdLog.Parameters.Add(pTargetUrl);
                        cmdLog.Parameters.Add(pAddress);
                        cmdLog.Parameters.Add(pCity);
                        cmdLog.Parameters.Add(pState);
                        cmdLog.Parameters.Add(pZip);
                        cmdLog.Parameters.Add(pCountry);
                        cmdLog.Parameters.Add(pRequestXml);
                        cmdLog.Parameters.Add(pResponseXml);
                        cmdLog.Parameters.Add(pRequestId);

                        cmdLog.ExecuteNonQuery();

                        RateRequestId = Convert.ToInt32(pRequestId.Value);

                        connLog.Close();*/
                    }
                    catch (Exception e)
                    {
                        handleError("ShowRates", "Error in saving request to DB: " + e.Message);
                        sbResults.Append("Error in saving request to DB: " + e.Message + "<br/>");
                    }


                    // -- Process each rated service --

                    foreach (RatedShipmentType ratedShipment in ratedShipments)
                    {
                        string serviceCode = "";
                        double totalCharges = 0;

                        serviceCode = ratedShipment.Service.Code;
                        
                        totalCharges = Convert.ToDouble(ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                        
                        sbResults.Append("Total charges: " + totalCharges.ToString() + "<br/>");
                        

                        if (ratedShipment.RatedShipmentAlert != null)
                        {
                            RatedShipmentInfoType[] ratedShipmentAlerts = ratedShipment.RatedShipmentAlert;
                            foreach (RatedShipmentInfoType ratedShipmentAlert in ratedShipmentAlerts)
                            {
                                sbResults.Append("Shipment Alert: " + ratedShipmentAlert.Code + " - " + ratedShipmentAlert.Description + "<br/>");
                            }
                        }

                        sbResults.Append("<br/>");

                        // -- Define variables for markup calculations --
                        // -- Define variables for markup calculations --
                        double markupPercentage = 0;
                        double perPackageCharge = 0;
                        double perShipmentCharge = 0;

                        // -- Determine the UPS Standard markup percentage --
                        if (dPerPackageChargeGF.ContainsKey(currentPlant))
                        {
                            markupPercentage = Convert.ToDouble(dUpchargeGF[currentPlant]);
                            perPackageCharge = dPerPackageChargeGF[currentPlant];
                            perShipmentCharge = dPerShipmentChargeGF[currentPlant];
                        }
                        else
                        {
                            sbResults.Append("Unable to find UPS Ground Freight markup charges for plant code: " + currentPlant + "<br/>");
                        }

                        totalCharges += ((markupPercentage / 100) * totalCharges);
                        totalCharges += (perPackageCharge * numPackages) + perShipmentCharge;

                        tblShopServices.Rows.Add(currentPlant, "Ground Freight", totalCharges);
                        
                    }





                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    handleError("ShowRates", "Soap Exception - SoapException Message= " + ex.Message + " SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + " SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + " SoapException StackTrace= " + ex.StackTrace);
                    // -- Handle SOAP error --
                    SoapError = true;
                    sbResults.Append("---------Freight Rate Web Service returns error----------------<br/>");
                    sbResults.Append("---------\"Hard\" is user error \"Transient\" is system error----------------<br/>");
                    sbResults.Append("SoapException Message= " + ex.Message + "<br/>");
                    sbResults.Append("<br/>");
                    sbResults.Append("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + "<br/>");
                    sbResults.Append("<br/>");
                    sbResults.Append("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + "<br/>");
                    sbResults.Append("<br/>");
                    sbResults.Append("SoapException StackTrace= " + ex.StackTrace + "<br/>");
                    sbResults.Append("-------------------------<br/>");
                    
                }
                catch (System.ServiceModel.CommunicationException ex)
                {
                    handleError("ShowRates", "Communication Exception - CommunicationException= " + ex.Message + " CommunicationException-StackTrace= " + ex.StackTrace);
                    // -- Handle General Communication Error --
                    SoapError = true;
                    sbResults.Append("<br/>");
                    sbResults.Append("--------------------<br/>");
                    sbResults.Append("CommunicationException= " + ex.Message + "<br/>");
                    sbResults.Append("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
                    sbResults.Append("-------------------------<br/>");
                    sbResults.Append("<br/>");
                    
                }
                catch (Exception ex)
                {
                    handleError("ShowRates", "General Error - General Exception= " + ex.Message + " General Exception-StackTrace= " + ex.StackTrace);
                    // -- Handle misc error --
                    SoapError = true;
                    sbResults.Append("<br/>");
                    sbResults.Append("-------------------------<br/>");
                    sbResults.Append(" General Exception= " + ex.Message + "<br/>");
                    sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
                    sbResults.Append("-------------------------<br/>");
                    
                }


                

            }
            

            if (!SoapError)
            {
                lblResultsGF.Visible = false;

                // -- Display Service Rates in Gridview --
                DataView dvServices = tblShopServices.DefaultView;
                dvServices.Sort = "Plant";

                gvGFRates.DataSource = dvServices;
                gvGFRates.DataBind();
                
                gvGFRates.Visible = true;
                pnlGFRates.Visible = true;

                //pnlUpsCommError.Visible = false;
            }
            else
            {
                    //lblResultsGF.Visible = true;
                    //litUpsCommError.Text = "There was a error getting Ground Freight results from UPS.  Please check your information and try again.";
                
                //pnlUpsCommError.Visible = true;
            }

            // -- log results --
            try
            {/*
                System.IO.StringWriter swGridResults = new System.IO.StringWriter();

                System.Web.UI.HtmlTextWriter htwGridResults = new HtmlTextWriter(swGridResults);

                gvShopServices.RenderControl(htwGridResults);

                SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                connLog.Open();

                SqlCommand cmdLog = new SqlCommand();
                cmdLog.Connection = connLog;
                cmdLog.CommandType = CommandType.StoredProcedure;
                cmdLog.CommandText = "LogResults";

                SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);
                SqlParameter pPlantCode = new SqlParameter("@PlantCode", SqlDbType.VarChar, 10);
                SqlParameter pFullResults = new SqlParameter("@FullResults", SqlDbType.NText);

                pRequestId.Value = RateRequestId;
                pPlantCode.Value = plantCode;
                pFullResults.Value = swGridResults.ToString();

                cmdLog.Parameters.Add(pRequestId);
                cmdLog.Parameters.Add(pPlantCode);
                cmdLog.Parameters.Add(pFullResults);

                cmdLog.ExecuteNonQuery();
                connLog.Close();
                */
            }
            catch (Exception e)
            {
                handleError("ShowRates", "Error in saving results to DB: " + e.Message);
                sbResults.Append("Error in saving results to DB: " + e.Message + "<br/>");
            }

            
        } //end of all plants rating process

        
        lblResultsGF.Text += sbResults.ToString();
        // Uncomment to show results for debugging
        lblResultsGF.Visible = true;

    }


    private void ShowRates(string shipToConsignee, string shipToStreet, string shipToCity, string shipToState, string shipToZip, string shipToCountry, string plantCode, bool addressCorrected)
    {
        int acctNumber = 0;
        Int32.TryParse(txtAcctNumber.Text.Trim(), out acctNumber);

        lblRateAddress.Text = shipToStreet;
        lblRateCity.Text = shipToCity;
        lblRateState.Text = shipToState;
        lblRateZip.Text = shipToZip;
        
        StringBuilder sbResults = new StringBuilder();

        bool SoapError = false;

        double shipmentWeight = 0;
        int shipmentClassification = 0;

        #region -- Define Services dictionary (mapping codes to descriptions) --
        Dictionary<string, string> dServices = new Dictionary<string, string>();
        dServices.Add("01", "UPS Next Day Air");
        dServices.Add("02", "UPS Second Day Air");
        dServices.Add("03", "UPS Ground");
        dServices.Add("12", "UPS Three-Day Select");
        dServices.Add("13", "UPS Next Day Air Saver");
        dServices.Add("14", "UPS Next Day Air Early A.M.");
        dServices.Add("59", "UPS Second Day Air A.M.");
        dServices.Add("65", "UPS Saver");
        #endregion

        #region -- Define Service CWT Types dictionary (mapping codes to Ground, Air, or Neither) --
        Dictionary<string, string> dServiceTypes = new Dictionary<string, string>();
        dServiceTypes.Add("01", "AIR");
        dServiceTypes.Add("02", "AIR");
        dServiceTypes.Add("03", "GROUND");
        dServiceTypes.Add("12", "GROUND");
        dServiceTypes.Add("13", "AIR");
        dServiceTypes.Add("14", "AIR-NN"); //means AIR - No Negotiated Rate - on this service, we ignore negotiated rate for CWT as it is not allowed
        dServiceTypes.Add("59", "AIR");
        dServiceTypes.Add("65", "GROUND");
        #endregion

        #region -- Define Per Package Charge and Per Shipment Charge dictionaries standard (charge per plant) --
        Dictionary<string, double> dPerPackageChargeUPS = new Dictionary<string, double>();
        Dictionary<string, double> dPerShipmentChargeUPS = new Dictionary<string, double>();

        Dictionary<string, double> dUpchargeNextDayAirUPS = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeSecondDayAirUPS = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeGroundUPS = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeThreeDaySelectUPS = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeNextDayAirSaverUPS = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeNextDayAirEarlyAMUPS = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeSecondDayAirAMUPS = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeSaverUPS = new Dictionary<string, double>();

        SqlConnection connCharges = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
        connCharges.Open();

        SqlCommand cmdCharges = new SqlCommand();
        cmdCharges.Connection = connCharges;
        //cmdCharges.CommandType = CommandType.Text;
        //cmdCharges.CommandText = "SELECT PlantCode, PerPackageCharge, PerShipmentCharge, NextDayAir, SecondDayAir, Ground, ThreeDaySelect, NextDayAirSaver, NextDayAirEarlyAM, SecondDayAirAM, Saver FROM PlantCarrierCharges WHERE CarrierId = 'UPS' ORDER BY PlantCode";
        
        cmdCharges.CommandText = "GetPlantCharges";
        cmdCharges.CommandType = CommandType.StoredProcedure;

        cmdCharges.Parameters.Add("@Carrier", SqlDbType.VarChar, 50).Value = "UPS";
        cmdCharges.Parameters.Add("@AcctNumber", SqlDbType.Int).Value = acctNumber;

        SqlDataReader drCharges = cmdCharges.ExecuteReader();

        while (drCharges.Read())
        {
            dPerPackageChargeUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerPackageCharge"].ToString()));
            dPerShipmentChargeUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerShipmentCharge"].ToString()));

            dUpchargeNextDayAirUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["NextDayAir"].ToString()));
            dUpchargeSecondDayAirUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["SecondDayAir"].ToString()));
            dUpchargeGroundUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["Ground"].ToString()));
            dUpchargeThreeDaySelectUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["ThreeDaySelect"].ToString()));
            dUpchargeNextDayAirSaverUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["NextDayAirSaver"].ToString()));
            dUpchargeNextDayAirEarlyAMUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["NextDayAirEarlyAM"].ToString()));
            dUpchargeSecondDayAirAMUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["SecondDayAirAM"].ToString()));
            dUpchargeSaverUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["Saver"].ToString()));
        }

        connCharges.Close();

        #endregion

        #region -- Define Per Package Charge and Per Shipment Charge dictionaries HundredWeight (charge per plant) --
        Dictionary<string, double> dPerPackageChargeCWT = new Dictionary<string, double>();
        Dictionary<string, double> dPerShipmentChargeCWT = new Dictionary<string, double>();

        Dictionary<string, double> dUpchargeNextDayAirCWT = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeSecondDayAirCWT = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeGroundCWT = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeThreeDaySelectCWT = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeNextDayAirSaverCWT = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeNextDayAirEarlyAMCWT = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeSecondDayAirAMCWT = new Dictionary<string, double>();
        Dictionary<string, double> dUpchargeSaverCWT = new Dictionary<string, double>();

        connCharges = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
        connCharges.Open();

        cmdCharges = new SqlCommand();
        cmdCharges.Connection = connCharges;
        //cmdCharges.CommandType = CommandType.Text;
        //cmdCharges.CommandText = "SELECT PlantCode, PerPackageCharge, PerShipmentCharge, NextDayAir, SecondDayAir, Ground, ThreeDaySelect, NextDayAirSaver, NextDayAirEarlyAM, SecondDayAirAM, Saver FROM PlantCarrierCharges WHERE CarrierId = 'UPSCWT' ORDER BY PlantCode";

        cmdCharges.CommandText = "GetPlantCharges";
        cmdCharges.CommandType = CommandType.StoredProcedure;

        cmdCharges.Parameters.Add("@Carrier", SqlDbType.VarChar, 50).Value = "UPSCWT";
        cmdCharges.Parameters.Add("@AcctNumber", SqlDbType.Int).Value = acctNumber;

        drCharges = cmdCharges.ExecuteReader();

        while (drCharges.Read())
        {
            dPerPackageChargeCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerPackageCharge"].ToString()));
            dPerShipmentChargeCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerShipmentCharge"].ToString()));

            dUpchargeNextDayAirCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["NextDayAir"].ToString()));
            dUpchargeSecondDayAirCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["SecondDayAir"].ToString()));
            dUpchargeGroundCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["Ground"].ToString()));
            dUpchargeThreeDaySelectCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["ThreeDaySelect"].ToString()));
            dUpchargeNextDayAirSaverCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["NextDayAirSaver"].ToString()));
            dUpchargeNextDayAirEarlyAMCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["NextDayAirEarlyAM"].ToString()));
            dUpchargeSecondDayAirAMCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["SecondDayAirAM"].ToString()));
            dUpchargeSaverCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["Saver"].ToString()));
        }

        connCharges.Close();

        /*
        Dictionary<string, double> dPerPackageChargeCWT = new Dictionary<string, double>();
        Dictionary<string, double> dPerShipmentChargeCWT = new Dictionary<string, double>();

        connCharges = new SqlConnection(ConfigurationManager.ConnectionStrings["UPSRateSqlConnection"].ConnectionString);
        connCharges.Open();

        cmdCharges = new SqlCommand();
        cmdCharges.Connection = connCharges;
        cmdCharges.CommandType = CommandType.Text;
        cmdCharges.CommandText = "SELECT PlantCode, PerPackageCharge, PerShipmentCharge FROM PlantCarrierCharges WHERE CarrierId = 'UPSCWT' ORDER BY PlantCode";

        drCharges = cmdCharges.ExecuteReader();

        while (drCharges.Read())
        {
            dPerPackageChargeCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerPackageCharge"].ToString()));
            dPerShipmentChargeCWT.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerShipmentCharge"].ToString()));
        }

        connCharges.Close();
        */
        #endregion

        #region -- Calculate HundredWeight eligibility --
        int numPackages = Convert.ToInt16(txtNumPackages.Text.Trim());
        string sPkgWeight = txtPackageWeight.Text.Trim();
        string sLastPkgWeight = txtLastPackageWeight.Text.Trim();

        if (sLastPkgWeight == "")
        {
            sLastPkgWeight = sPkgWeight;
        }

        int totalWeight = (Convert.ToInt16(sPkgWeight) * (numPackages - 1)) + Convert.ToInt16(sLastPkgWeight);
        bool isAirCWT = (numPackages >= Config.MinCWTPackagesAir) && (totalWeight >= Config.MinCWTWeightAir);
        bool isGroundCWT = (numPackages >= Config.MinCWTPackagesGround) && (totalWeight >= Config.MinCWTWeightGround);

        sbResults.Append("Total weight: " + totalWeight.ToString() + "<br/>");
        sbResults.Append("Qualifies for CWT Air? " + isAirCWT.ToString() + "<br/>");
        sbResults.Append("Qualifies for CWT Grd? " + isGroundCWT.ToString() + "<br/>");
        #endregion

        if (lstRateWithGF.SelectedValue.ToString() == "Yes")
        {
            ShowGFRate(shipToConsignee,  shipToStreet,  shipToCity,  shipToState,  shipToZip,  shipToCountry,  plantCode,  addressCorrected);
        }
        else
        {
            pnlGFRates.Visible = false;
        }

        if (lstRateWithLTL.SelectedValue.ToString() == "Yes")
        {
            ShowLTLRates(shipToCity, shipToState, shipToZip, shipToCountry, plantCode, totalWeight, numPackages);
        }
        else
        {
            pnlLTLRates.Visible = false;
        }

        if (plantCode != "ALL")
        {
            #region -- Process rate request for a single plant --
            try
            {

                int RateRequestId = 0;

                #region -- Load Plant Ship From Address --
                string shipFromAddress = "";
                string shipFromCity = "";
                string shipFromState = "";
                string shipFromZip = "";
                string shipFromCountry = "";

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Address, City, State, Zip, Country FROM Plants WHERE PlantCode = '" + plantCode + "'";

                SqlDataReader drResults = cmd.ExecuteReader();

                if (drResults.Read())
                {
                    shipFromAddress = drResults["Address"].ToString();
                    shipFromCity = drResults["City"].ToString();
                    shipFromState = drResults["State"].ToString();
                    shipFromZip = drResults["Zip"].ToString();
                    shipFromCountry = drResults["Country"].ToString();
                }
                else
                {
                    //error looking up Address Info
                    sbResults.Append("Unable to lookup address info for Plant " + plantCode + "<br/>");
                }

                conn.Close();

                #endregion

                #region -- Begin building Rate Request --
                RateService rateService = new RateService();
                RateRequest rateRequest = new RateRequest();

                UpsRateWebReference.RequestType request = new UpsRateWebReference.RequestType();
                String[] requestOption = { "Shop" };
                request.RequestOption = requestOption;
                rateRequest.Request = request;
                #endregion

                #region -- Access Security (license number, username, password) --
                UpsRateWebReference.UPSSecurity upss = new UpsRateWebReference.UPSSecurity();
                UpsRateWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsRateWebReference.UPSSecurityServiceAccessToken();
                upsSvcToken.AccessLicenseNumber = Config.UPSAccessKey;
                upss.ServiceAccessToken = upsSvcToken;
                UpsRateWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsRateWebReference.UPSSecurityUsernameToken();
                upsSecUsrnameToken.Username = Config.UPSUserName;
                upsSecUsrnameToken.Password = Config.UPSPassword;
                upss.UsernameToken = upsSecUsrnameToken;
                rateService.UPSSecurityValue = upss;
                #endregion

                #region -- Build Shipment object --
                ShipmentType shipment = new ShipmentType();

                #region -- Shipper --
                ShipperType shipper = new ShipperType();
                shipper.ShipperNumber = Config.ShipFromShipperNumber;
                AddressType shipperAddress = new AddressType();
                String[] shipperAddressLine = { shipFromAddress };
                shipperAddress.AddressLine = shipperAddressLine;
                shipperAddress.City = shipFromCity;
                shipperAddress.StateProvinceCode = shipFromState;
                shipperAddress.PostalCode = shipFromZip;
                shipperAddress.CountryCode = shipFromCountry;
                shipper.Address = shipperAddress;

                shipment.Shipper = shipper;
                #endregion

                #region -- Ship To --
                ShipToType shipTo = new ShipToType();
                ShipToAddressType shipToAddress = new ShipToAddressType();
                String[] shipToAddressLines = { shipToStreet };
                shipToAddress.AddressLine = shipToAddressLines;
                shipToAddress.City = shipToCity;
                shipToAddress.StateProvinceCode = shipToState;
                shipToAddress.PostalCode = shipToZip;
                shipToAddress.CountryCode = shipToCountry;

                shipTo.Address = shipToAddress;

                shipment.ShipTo = shipTo;

                #endregion

                #region -- Packages --
                PackageType package;
                PackageWeightType packageWeight;
                UpsRateWebReference.CodeDescriptionType uomCodeDesc;

                UpsRateWebReference.CodeDescriptionType packageTypeCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                packageTypeCodeDesc.Code = "00";

                string deliveryConfirmationOption = lstDeliveryConfirmation.SelectedValue;

                PackageType[] packages = new PackageType[numPackages];
                for (int i = 0; i < numPackages; i++)
                {
                    package = new PackageType();
                    packageWeight = new PackageWeightType();
                    uomCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                    uomCodeDesc.Code = "LBS";
                    if (i == (numPackages - 1))
                    {
                        packageWeight.Weight = sLastPkgWeight;
                    }
                    else
                    {
                        packageWeight.Weight = sPkgWeight;
                    }
                    packageWeight.UnitOfMeasurement = uomCodeDesc;
                    package.PackageWeight = packageWeight;

                    package.PackagingType = packageTypeCodeDesc;

                    if (deliveryConfirmationOption != "0")
                    {
                        PackageServiceOptionsType packageServiceOptions = new PackageServiceOptionsType();
                        DeliveryConfirmationType deliveryConfirmation = new DeliveryConfirmationType();
                        deliveryConfirmation.DCISType = deliveryConfirmationOption;
                        packageServiceOptions.DeliveryConfirmation = deliveryConfirmation;
                        package.PackageServiceOptions = packageServiceOptions;
                    }

                    packages[i] = package;
                }

                shipment.Package = packages;
                #endregion

                #region -- Negotiated Rates Indicator (if needed) --
                if (isAirCWT || isGroundCWT)
                {
                    ShipmentRatingOptionsType ratingOptions = new ShipmentRatingOptionsType();
                    ratingOptions.NegotiatedRatesIndicator = "";
                    shipment.ShipmentRatingOptions = ratingOptions;
                }
                #endregion

                rateRequest.Shipment = shipment;

                #endregion

                #region -- Submit Rate Request --

                RateResponse rateResponse = rateService.ProcessRate(rateRequest);

                RatedShipmentType[] ratedShipments = rateResponse.RatedShipment;

                string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
                string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

                string targetUrl = rateService.Url;
                string UserName = HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();

                #region -- log request --
                try
                {
                    SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                    connLog.Open();

                    SqlCommand cmdLog = new SqlCommand();
                    cmdLog.Connection = connLog;
                    cmdLog.CommandType = CommandType.StoredProcedure;
                    cmdLog.CommandText = "LogRequest_Rate";

                    SqlParameter pUserName = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                    SqlParameter pTargetUrl = new SqlParameter("@TargetUrl", SqlDbType.VarChar, 200);
                    SqlParameter pAddress = new SqlParameter("@Address", SqlDbType.VarChar, 200);
                    SqlParameter pCity = new SqlParameter("@City", SqlDbType.VarChar, 200);
                    SqlParameter pState = new SqlParameter("@State", SqlDbType.VarChar, 50);
                    SqlParameter pZip = new SqlParameter("@Zip", SqlDbType.VarChar, 50);
                    SqlParameter pCountry = new SqlParameter("@Country", SqlDbType.VarChar, 2);
                    SqlParameter pRequestXml = new SqlParameter("@RequestXml", SqlDbType.NText);
                    SqlParameter pResponseXml = new SqlParameter("@ResponseXml", SqlDbType.NText);
                    SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);

                    pUserName.Value = UserName;
                    pTargetUrl.Value = targetUrl;
                    pAddress.Value = shipToStreet;
                    pCity.Value = shipToCity;
                    pState.Value = shipToState;
                    pZip.Value = shipToZip;
                    pCountry.Value = shipToCountry;
                    pRequestXml.Value = requestXml;
                    pResponseXml.Value = responseXml;
                    pRequestId.Direction = ParameterDirection.Output;

                    cmdLog.Parameters.Add(pUserName);
                    cmdLog.Parameters.Add(pTargetUrl);
                    cmdLog.Parameters.Add(pAddress);
                    cmdLog.Parameters.Add(pCity);
                    cmdLog.Parameters.Add(pState);
                    cmdLog.Parameters.Add(pZip);
                    cmdLog.Parameters.Add(pCountry);
                    cmdLog.Parameters.Add(pRequestXml);
                    cmdLog.Parameters.Add(pResponseXml);
                    cmdLog.Parameters.Add(pRequestId);

                    cmdLog.ExecuteNonQuery();

                    RateRequestId = Convert.ToInt32(pRequestId.Value);

                    connLog.Close();
                }
                catch (Exception e)
                {
                    handleError("ShowRates", "Error in saving request to DB: " + e.Message);
                    sbResults.Append("Error in saving request to DB: " + e.Message + "<br/>");
                }
                #endregion

                #endregion

                #region -- Define tblServices to hold rate data --

                DataSet dsServices = new DataSet();
                DataTable tblServices = dsServices.Tables.Add();

                tblServices.Columns.Add("Plant", typeof(string));
                tblServices.Columns.Add("Desc", typeof(string));
                tblServices.Columns.Add("Rate", typeof(double));
                tblServices.Columns.Add("BillingWeight", typeof(double));
                tblServices.Columns.Add("Classification", typeof(int));
                tblServices.Columns.Add("ServiceCharges", typeof(double));
                tblServices.Columns.Add("TransportationCharges", typeof(double));
                tblServices.Columns.Add("IsHundredWeight", typeof(bool));

                #endregion

                #region -- Process each rated service --

                foreach (RatedShipmentType ratedShipment in ratedShipments)
                {
                    string serviceDesc = "";
                    string serviceCWTType = "";
                    string serviceCode = "";
                    int addressClassification = 1;
                    double billingWeight = 0;
                    double serviceCharges = 0;
                    double transportationCharges = 0;
                    double totalCharges = 0;
                    double negotiatedCharges = 0;
                    bool isHundredWeight = false;

                    serviceCode = ratedShipment.Service.Code;

                    if (dServiceTypes.ContainsKey(ratedShipment.Service.Code))
                    {
                        serviceCWTType = dServiceTypes[ratedShipment.Service.Code];
                    }
                    else
                    {
                        serviceCWTType = "";
                    }

                    if (dServices.ContainsKey(ratedShipment.Service.Code))
                    {
                        serviceDesc = dServices[ratedShipment.Service.Code];
                    }
                    else
                    {
                        serviceDesc = "Unknown Service";
                    }

                    billingWeight = Convert.ToDouble(ratedShipment.BillingWeight.Weight);
                    serviceCharges = Convert.ToDouble(ratedShipment.ServiceOptionsCharges.MonetaryValue);
                    transportationCharges = Convert.ToDouble(ratedShipment.TransportationCharges.MonetaryValue);
                    totalCharges = Convert.ToDouble(ratedShipment.TotalCharges.MonetaryValue);

                    sbResults.Append("Service: " + ratedShipment.Service.Code + " - " + serviceDesc + "<br/>");
                    sbResults.Append("Billing Weight: " + billingWeight.ToString() + "<br/>");
                    sbResults.Append("Service Options Charges: " + serviceCharges.ToString() + "<br/>");
                    //sbResults.Append("Transportation Charges: " + transportationCharges.ToString() + "<br/>");
                    //sbResults.Append("Total charges: " + totalCharges.ToString() + "<br/>");

                    if (ratedShipment.GuaranteedDelivery != null)
                    {
                        sbResults.Append("Guaranteed Delivery / Business Days in Transit: " + ratedShipment.GuaranteedDelivery.BusinessDaysInTransit + "<br/>");
                        sbResults.Append("Guaranteed Delivery / Delivery By Time: " + ratedShipment.GuaranteedDelivery.DeliveryByTime + "<br/>");
                    }
                    if (ratedShipment.NegotiatedRateCharges != null)
                    {
                        negotiatedCharges = Convert.ToDouble(ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                        sbResults.Append("Negotiated Rate Total Charges: " + negotiatedCharges.ToString() + "<br/>");
                    }

                    if (ratedShipment.RatedShipmentAlert != null)
                    {
                        RatedShipmentInfoType[] ratedShipmentAlerts = ratedShipment.RatedShipmentAlert;
                        foreach (RatedShipmentInfoType ratedShipmentAlert in ratedShipmentAlerts)
                        {
                            if (ratedShipmentAlert.Code == "110920")
                            {
                                //Then the "Commercial" classification has changed to "Residential" so set classification to 2
                                addressClassification = 2;
                            }
                            sbResults.Append("Shipment Alert: " + ratedShipmentAlert.Code + " - " + ratedShipmentAlert.Description + "<br/>");
                        }
                    }

                    sbResults.Append("<br/>");

                    #region -- Define variables for markup calculations --
                    double markupPercentage = 0;
                    double perPackageCharge = 0;
                    double perShipmentCharge = 0;

                    double markupPercentageUPS = 0;
                    double perPackageChargeUPS = 0;
                    double perShipmentChargeUPS = 0;

                    double markupPercentageCWT = 0;
                    double perPackageChargeCWT = 0;
                    double perShipmentChargeCWT = 0;
                    #endregion

                    #region -- Determine the UPS Standard markup percentage --
                    if (dPerPackageChargeUPS.ContainsKey(plantCode))
                    {
                        switch (serviceCode)
                        {
                            case "01":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeNextDayAirUPS[plantCode]);
                                break;
                            case "02":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeSecondDayAirUPS[plantCode]);
                                break;
                            case "03":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeGroundUPS[plantCode]);
                                break;
                            case "12":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeThreeDaySelectUPS[plantCode]);
                                break;
                            case "13":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeNextDayAirSaverUPS[plantCode]);
                                break;
                            case "14":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeNextDayAirEarlyAMUPS[plantCode]);
                                break;
                            case "59":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeSecondDayAirAMUPS[plantCode]);
                                break;
                            case "65":
                                markupPercentageUPS = Convert.ToDouble(dUpchargeSaverUPS[plantCode]);
                                break;
                        }

                        perPackageChargeUPS = dPerPackageChargeUPS[plantCode];
                        perShipmentChargeUPS = dPerShipmentChargeUPS[plantCode];
                    }
                    else
                    {
                        sbResults.Append("Unable to find UPS Standard markup charges for plant code: " + plantCode + "<br/>");
                    }
                    #endregion

                    #region -- Determine the HundredWeight markup percentage --
                    if (dPerPackageChargeUPS.ContainsKey(plantCode))
                    {
                        switch (serviceCode)
                        {
                            case "01":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeNextDayAirUPS[plantCode]);
                                break;
                            case "02":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeSecondDayAirUPS[plantCode]);
                                break;
                            case "03":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeGroundUPS[plantCode]);
                                break;
                            case "12":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeThreeDaySelectUPS[plantCode]);
                                break;
                            case "13":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeNextDayAirSaverUPS[plantCode]);
                                break;
                            case "14":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeNextDayAirEarlyAMUPS[plantCode]);
                                break;
                            case "59":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeSecondDayAirAMUPS[plantCode]);
                                break;
                            case "65":
                                markupPercentageCWT = Convert.ToDouble(dUpchargeSaverUPS[plantCode]);
                                break;
                        }

                        perPackageChargeCWT = dPerPackageChargeCWT[plantCode];
                        perShipmentChargeCWT = dPerShipmentChargeCWT[plantCode];
                    }
                    else
                    {
                        sbResults.Append("Unable to find HundredWeight markup charges for plant code: " + plantCode + "<br/>");
                    }
                    #endregion

                    //if shipment qualifies for hundredweight air and hundredweight method class is air, consider it hundredweight
                    //if shipment qualifies for hundredweight ground and hundredweight method class is ground, consider it hundredweight

                    if (isAirCWT && (serviceCWTType == "AIR-NN"))
                    {
                        isHundredWeight = true;
                        //Do not adjust total charges
                    }
                    else if ((isAirCWT && (serviceCWTType == "AIR")) || (isGroundCWT && (serviceCWTType == "GROUND")))
                    {
                        isHundredWeight = true;

                        // Since our negotiated CWT rates is 70% of the published rate, reverse that and divide neg. rate by .7 to get published rate
                        totalCharges = negotiatedCharges / .7;

                    }
                    else
                    {
                        isHundredWeight = false;
                    }

                    if (isHundredWeight)
                    {
                        markupPercentage = markupPercentageCWT;
                        perPackageCharge = perPackageChargeCWT;
                        perShipmentCharge = perShipmentChargeCWT;
                    }
                    else
                    {
                        markupPercentage = markupPercentageUPS;
                        perPackageCharge = perPackageChargeUPS;
                        perShipmentCharge = perShipmentChargeUPS;
                    }

                    totalCharges += ((markupPercentage / 100) * totalCharges);
                    totalCharges += (perPackageCharge * numPackages) + perShipmentCharge;

                    tblServices.Rows.Add(plantCode, serviceDesc, totalCharges, billingWeight, addressClassification, serviceCharges, transportationCharges, isHundredWeight);

                    shipmentWeight = billingWeight;
                    shipmentClassification = addressClassification;
                }
                #endregion

                #region -- Display Service Rates in Gridview --
                DataView dvServices = tblServices.DefaultView;
                dvServices.Sort = "Rate";

                gvServices.DataSource = dvServices;
                gvServices.DataBind();

                gvShopServices.Visible = false;
                gvServices.Visible = true;

                #region -- log results --
                try
                {
                    System.IO.StringWriter swGridResults = new System.IO.StringWriter();

                    System.Web.UI.HtmlTextWriter htwGridResults = new HtmlTextWriter(swGridResults);

                    gvServices.RenderControl(htwGridResults);

                    SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                    connLog.Open();

                    SqlCommand cmdLog = new SqlCommand();
                    cmdLog.Connection = connLog;
                    cmdLog.CommandType = CommandType.StoredProcedure;
                    cmdLog.CommandText = "LogResults";

                    SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);
                    SqlParameter pPlantCode = new SqlParameter("@PlantCode", SqlDbType.VarChar, 10);
                    SqlParameter pFullResults = new SqlParameter("@FullResults", SqlDbType.NText);

                    pRequestId.Value = RateRequestId;
                    pPlantCode.Value = plantCode;
                    pFullResults.Value = swGridResults.ToString();

                    cmdLog.Parameters.Add(pRequestId);
                    cmdLog.Parameters.Add(pPlantCode);
                    cmdLog.Parameters.Add(pFullResults);

                    cmdLog.ExecuteNonQuery();
                    connLog.Close();
                }
                catch (Exception e)
                {
                    handleError("ShowRates", "Error in saving results to DB: " + e.Message);
                    sbResults.Append("Error in saving results to DB: " + e.Message + "<br/>");
                }
                #endregion


                #endregion

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                if ((ex.Detail.LastChild.InnerText != "Hard110003Maximum number of packages exceeded (50)") //Do not show "scary" red message if only soap error is that it exceeds 50 packages (also catch 200 pkg message)
                   && (ex.Detail.LastChild.InnerText != "Hard110003Maximum number of packages exceeded (200)"))
                {
                    handleError("ShowRates", "Soap Exception - SoapException Message= " + ex.Message + " SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + " SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + " SoapException StackTrace= " + ex.StackTrace);
                }
                #region -- Handle SOAP error --
                SoapError = true;
                sbResults.Append("---------Freight Rate Web Service returns error----------------<br/>");
                sbResults.Append("---------\"Hard\" is user error \"Transient\" is system error----------------<br/>");
                sbResults.Append("SoapException Message= " + ex.Message + "<br/>");
                sbResults.Append("<br/>");
                sbResults.Append("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + "<br/>");
                sbResults.Append("<br/>");
                sbResults.Append("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + "<br/>");
                sbResults.Append("<br/>");
                sbResults.Append("SoapException StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                #endregion
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                handleError("ShowRates", "Communication Exception - CommunicationException= " + ex.Message + " CommunicationException-StackTrace= " + ex.StackTrace);
                SoapError = true;
                #region -- Handle General Communication Error --
                sbResults.Append("<br/>");
                sbResults.Append("--------------------<br/>");
                sbResults.Append("CommunicationException= " + ex.Message + "<br/>");
                sbResults.Append("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                sbResults.Append("<br/>");
                #endregion
            }
            catch (Exception ex)
            {
                handleError("ShowRates", "General Error - General Exception= " + ex.Message + " General Exception-StackTrace= " + ex.StackTrace);
                SoapError = true;
                #region -- Handle misc error --
                sbResults.Append("<br/>");
                sbResults.Append("-------------------------<br/>");
                sbResults.Append(" General Exception= " + ex.Message + "<br/>");
                sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                #endregion
            }


            if (!SoapError)
            {
                lblResults.Visible = true;
                pnlUpsCommError.Visible = false;
            }
            else
            {
                //lblResults.Visible = true;
                gvShopServices.Visible = false;
                gvServices.Visible = false;
                if (numPackages > 50)
                {
                    litUpsCommError.Text = "Unable to rate shipments of more than 50 packages via UPS.";
                    shipmentWeight = totalWeight;
                }
                else
                {
                    //lblResults.Visible = true;
                    litUpsCommError.Text = "There was a error getting results from UPS.  Please check your information and try again.";
                }
                pnlUpsCommError.Visible = true;
            }

            #endregion
        } //end of single Plant rating process
        else
        { // Shop rates for all plants
            #region -- Process rate request for all plants and combine into a single dataset --
            #region -- Define tblShopServices to hold rate data --

            DataSet dsShopServices = new DataSet();
            DataTable tblShopServices = dsShopServices.Tables.Add();

            tblShopServices.Columns.Add("Plant", typeof(string));
            tblShopServices.Columns.Add("Desc", typeof(string));
            tblShopServices.Columns.Add("Rate", typeof(double));
            tblShopServices.Columns.Add("BillingWeight", typeof(double));
            tblShopServices.Columns.Add("Classification", typeof(int));
            tblShopServices.Columns.Add("ServiceCharges", typeof(double));
            tblShopServices.Columns.Add("TransportationCharges", typeof(double));
            tblShopServices.Columns.Add("IsHundredWeight", typeof(bool));

            #endregion

            int RateRequestId = 0;

            for (int iPlantCount = 0; iPlantCount < Config.PlantCodesMultiRate.Count(); iPlantCount++)
            {
                string currentPlant = Config.PlantCodesMultiRate[iPlantCount];

                #region -- Process rate request for currentPlant --
                try
                {

                    #region -- Load Plant Ship From Address --
                    string shipFromAddress = "";
                    string shipFromCity = "";
                    string shipFromState = "";
                    string shipFromZip = "";
                    string shipFromCountry = "";

                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT Address, City, State, Zip, Country FROM Plants WHERE PlantCode = '" + currentPlant + "'";

                    SqlDataReader drResults = cmd.ExecuteReader();

                    if (drResults.Read())
                    {
                        shipFromAddress = drResults["Address"].ToString();
                        shipFromCity = drResults["City"].ToString();
                        shipFromState = drResults["State"].ToString();
                        shipFromZip = drResults["Zip"].ToString();
                        shipFromCountry = drResults["Country"].ToString();
                    }
                    else
                    {
                        //error looking up Address Info
                        sbResults.Append("Unable to lookup address info for Plant " + plantCode + "<br/>");
                    }

                    conn.Close();

                    #endregion

                    #region -- Begin building Rate Request --
                    RateService rateService = new RateService();
                    RateRequest rateRequest = new RateRequest();

                    UpsRateWebReference.RequestType request = new UpsRateWebReference.RequestType();
                    String[] requestOption = { "Shop" };
                    request.RequestOption = requestOption;
                    rateRequest.Request = request;
                    #endregion

                    #region -- Access Security (license number, username, password) --
                    UpsRateWebReference.UPSSecurity upss = new UpsRateWebReference.UPSSecurity();
                    UpsRateWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsRateWebReference.UPSSecurityServiceAccessToken();
                    upsSvcToken.AccessLicenseNumber = Config.UPSAccessKey;
                    upss.ServiceAccessToken = upsSvcToken;
                    UpsRateWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsRateWebReference.UPSSecurityUsernameToken();
                    upsSecUsrnameToken.Username = Config.UPSUserName;
                    upsSecUsrnameToken.Password = Config.UPSPassword;
                    upss.UsernameToken = upsSecUsrnameToken;
                    rateService.UPSSecurityValue = upss;
                    #endregion

                    #region -- Build Shipment object --
                    ShipmentType shipment = new ShipmentType();

                    #region -- Shipper --
                    ShipperType shipper = new ShipperType();
                    shipper.ShipperNumber = Config.ShipFromShipperNumber;
                    AddressType shipperAddress = new AddressType();
                    String[] shipperAddressLine = { shipFromAddress };
                    shipperAddress.AddressLine = shipperAddressLine;
                    shipperAddress.City = shipFromCity;
                    shipperAddress.StateProvinceCode = shipFromState;
                    shipperAddress.PostalCode = shipFromZip;
                    shipperAddress.CountryCode = shipFromCountry;
                    shipper.Address = shipperAddress;

                    shipment.Shipper = shipper;
                    #endregion

                    #region -- Ship To --
                    ShipToType shipTo = new ShipToType();
                    ShipToAddressType shipToAddress = new ShipToAddressType();
                    String[] shipToAddressLines = { shipToStreet };
                    shipToAddress.AddressLine = shipToAddressLines;
                    shipToAddress.City = shipToCity;
                    shipToAddress.StateProvinceCode = shipToState;
                    shipToAddress.PostalCode = shipToZip;
                    shipToAddress.CountryCode = shipToCountry;

                    shipTo.Address = shipToAddress;

                    shipment.ShipTo = shipTo;

                    #endregion

                    #region -- Packages --
                    PackageType package;
                    PackageWeightType packageWeight;
                    UpsRateWebReference.CodeDescriptionType uomCodeDesc;

                    UpsRateWebReference.CodeDescriptionType packageTypeCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                    packageTypeCodeDesc.Code = "00";

                    string deliveryConfirmationOption = lstDeliveryConfirmation.SelectedValue;

                    PackageType[] packages = new PackageType[numPackages];
                    for (int i = 0; i < numPackages; i++)
                    {
                        package = new PackageType();
                        packageWeight = new PackageWeightType();
                        uomCodeDesc = new UpsRateWebReference.CodeDescriptionType();
                        uomCodeDesc.Code = "LBS";
                        if (i == (numPackages - 1))
                        {
                            packageWeight.Weight = sLastPkgWeight;
                        }
                        else
                        {
                            packageWeight.Weight = sPkgWeight;
                        }
                        packageWeight.UnitOfMeasurement = uomCodeDesc;
                        package.PackageWeight = packageWeight;

                        package.PackagingType = packageTypeCodeDesc;

                        if (deliveryConfirmationOption != "0")
                        {
                            PackageServiceOptionsType packageServiceOptions = new PackageServiceOptionsType();
                            DeliveryConfirmationType deliveryConfirmation = new DeliveryConfirmationType();
                            deliveryConfirmation.DCISType = deliveryConfirmationOption;
                            packageServiceOptions.DeliveryConfirmation = deliveryConfirmation;
                            package.PackageServiceOptions = packageServiceOptions;
                        }

                        packages[i] = package;
                    }

                    shipment.Package = packages;
                    #endregion

                    #region -- Negotiated Rates Indicator (if needed) --
                    if (isAirCWT || isGroundCWT)
                    {
                        ShipmentRatingOptionsType ratingOptions = new ShipmentRatingOptionsType();
                        ratingOptions.NegotiatedRatesIndicator = "";
                        shipment.ShipmentRatingOptions = ratingOptions;
                    }
                    #endregion

                    rateRequest.Shipment = shipment;

                    #endregion

                    #region -- Submit Rate Request --

                    RateResponse rateResponse = rateService.ProcessRate(rateRequest);

                    RatedShipmentType[] ratedShipments = rateResponse.RatedShipment;

                    string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
                    string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

                    string targetUrl = rateService.Url;
                    string UserName = HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();

                    #region -- log request --
                    try
                    {
                        SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                        connLog.Open();

                        SqlCommand cmdLog = new SqlCommand();
                        cmdLog.Connection = connLog;
                        cmdLog.CommandType = CommandType.StoredProcedure;
                        cmdLog.CommandText = "LogRequest_Rate";

                        SqlParameter pUserName = new SqlParameter("@UserName", SqlDbType.VarChar, 50);
                        SqlParameter pTargetUrl = new SqlParameter("@TargetUrl", SqlDbType.VarChar, 200);
                        SqlParameter pAddress = new SqlParameter("@Address", SqlDbType.VarChar, 200);
                        SqlParameter pCity = new SqlParameter("@City", SqlDbType.VarChar, 200);
                        SqlParameter pState = new SqlParameter("@State", SqlDbType.VarChar, 50);
                        SqlParameter pZip = new SqlParameter("@Zip", SqlDbType.VarChar, 50);
                        SqlParameter pCountry = new SqlParameter("@Country", SqlDbType.VarChar, 2);
                        SqlParameter pRequestXml = new SqlParameter("@RequestXml", SqlDbType.NText);
                        SqlParameter pResponseXml = new SqlParameter("@ResponseXml", SqlDbType.NText);
                        SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);

                        pUserName.Value = UserName;
                        pTargetUrl.Value = targetUrl;
                        pAddress.Value = shipToStreet;
                        pCity.Value = shipToCity;
                        pState.Value = shipToState;
                        pZip.Value = shipToZip;
                        pCountry.Value = shipToCountry;
                        pRequestXml.Value = requestXml;
                        pResponseXml.Value = responseXml;
                        pRequestId.Direction = ParameterDirection.Output;

                        cmdLog.Parameters.Add(pUserName);
                        cmdLog.Parameters.Add(pTargetUrl);
                        cmdLog.Parameters.Add(pAddress);
                        cmdLog.Parameters.Add(pCity);
                        cmdLog.Parameters.Add(pState);
                        cmdLog.Parameters.Add(pZip);
                        cmdLog.Parameters.Add(pCountry);
                        cmdLog.Parameters.Add(pRequestXml);
                        cmdLog.Parameters.Add(pResponseXml);
                        cmdLog.Parameters.Add(pRequestId);

                        cmdLog.ExecuteNonQuery();

                        RateRequestId = Convert.ToInt32(pRequestId.Value);

                        connLog.Close();
                    }
                    catch (Exception e)
                    {
                        handleError("ShowRates", "Error in saving request to DB: " + e.Message);
                        sbResults.Append("Error in saving request to DB: " + e.Message + "<br/>");
                    }
                    #endregion

                    #endregion

                    #region -- Process each rated service --

                    foreach (RatedShipmentType ratedShipment in ratedShipments)
                    {
                        string serviceDesc = "";
                        string serviceCWTType = "";
                        string serviceCode = "";
                        int addressClassification = 1;
                        double billingWeight = 0;
                        double serviceCharges = 0;
                        double transportationCharges = 0;
                        double totalCharges = 0;
                        double negotiatedCharges = 0;
                        bool isHundredWeight = false;

                        serviceCode = ratedShipment.Service.Code;

                        if (dServiceTypes.ContainsKey(ratedShipment.Service.Code))
                        {
                            serviceCWTType = dServiceTypes[ratedShipment.Service.Code];
                        }
                        else
                        {
                            serviceCWTType = "";
                        }

                        if (dServices.ContainsKey(ratedShipment.Service.Code))
                        {
                            serviceDesc = dServices[ratedShipment.Service.Code];
                        }
                        else
                        {
                            serviceDesc = "Unknown Service";
                        }
                        billingWeight = Convert.ToDouble(ratedShipment.BillingWeight.Weight);
                        serviceCharges = Convert.ToDouble(ratedShipment.ServiceOptionsCharges.MonetaryValue);
                        transportationCharges = Convert.ToDouble(ratedShipment.TransportationCharges.MonetaryValue);
                        totalCharges = Convert.ToDouble(ratedShipment.TotalCharges.MonetaryValue);

                        sbResults.Append("Service: " + ratedShipment.Service.Code + " - " + serviceDesc + "<br/>");
                        sbResults.Append("Billing Weight: " + billingWeight.ToString() + "<br/>");
                        sbResults.Append("Service Options Charges: " + serviceCharges.ToString() + "<br/>");
                        sbResults.Append("Transportation Charges: " + transportationCharges.ToString() + "<br/>");
                        sbResults.Append("Total charges: " + totalCharges.ToString() + "<br/>");

                        if (ratedShipment.GuaranteedDelivery != null)
                        {
                            sbResults.Append("Guaranteed Delivery / Business Days in Transit: " + ratedShipment.GuaranteedDelivery.BusinessDaysInTransit + "<br/>");
                            sbResults.Append("Guaranteed Delivery / Delivery By Time: " + ratedShipment.GuaranteedDelivery.DeliveryByTime + "<br/>");
                        }
                        if (ratedShipment.NegotiatedRateCharges != null)
                        {
                            negotiatedCharges = Convert.ToDouble(ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue);
                            sbResults.Append("Negotiated Rate Total Charges: " + negotiatedCharges.ToString() + "<br/>");
                        }

                        if (ratedShipment.RatedShipmentAlert != null)
                        {
                            RatedShipmentInfoType[] ratedShipmentAlerts = ratedShipment.RatedShipmentAlert;
                            foreach (RatedShipmentInfoType ratedShipmentAlert in ratedShipmentAlerts)
                            {
                                if (ratedShipmentAlert.Code == "110920")
                                {
                                    //Then the "Commercial" classification has changed to "Residential" so set classification to 2
                                    addressClassification = 2;
                                }
                                sbResults.Append("Shipment Alert: " + ratedShipmentAlert.Code + " - " + ratedShipmentAlert.Description + "<br/>");
                            }
                        }

                        sbResults.Append("<br/>");

                        #region -- Define variables for markup calculations --
                        double markupPercentage = 0;
                        double perPackageCharge = 0;
                        double perShipmentCharge = 0;

                        double markupPercentageUPS = 0;
                        double perPackageChargeUPS = 0;
                        double perShipmentChargeUPS = 0;

                        double markupPercentageCWT = 0;
                        double perPackageChargeCWT = 0;
                        double perShipmentChargeCWT = 0;
                        #endregion

                        #region -- Determine the UPS Standard markup percentage --
                        if (dPerPackageChargeUPS.ContainsKey(currentPlant))
                        {
                            switch (serviceCode)
                            {
                                case "01":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeNextDayAirUPS[currentPlant]);
                                    break;
                                case "02":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeSecondDayAirUPS[currentPlant]);
                                    break;
                                case "03":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeGroundUPS[currentPlant]);
                                    break;
                                case "12":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeThreeDaySelectUPS[currentPlant]);
                                    break;
                                case "13":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeNextDayAirSaverUPS[currentPlant]);
                                    break;
                                case "14":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeNextDayAirEarlyAMUPS[currentPlant]);
                                    break;
                                case "59":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeSecondDayAirAMUPS[currentPlant]);
                                    break;
                                case "65":
                                    markupPercentageUPS = Convert.ToDouble(dUpchargeSaverUPS[currentPlant]);
                                    break;
                            }

                            perPackageChargeUPS = dPerPackageChargeUPS[currentPlant];
                            perShipmentChargeUPS = dPerShipmentChargeUPS[currentPlant];
                        }
                        else
                        {
                            sbResults.Append("Unable to find UPS Standard markup charges for plant code: " + currentPlant + "<br/>");
                        }
                        #endregion

                        #region -- Determine the HundredWeight markup percentage --
                        if (dPerPackageChargeUPS.ContainsKey(currentPlant))
                        {
                            switch (serviceCode)
                            {
                                case "01":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeNextDayAirUPS[currentPlant]);
                                    break;
                                case "02":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeSecondDayAirUPS[currentPlant]);
                                    break;
                                case "03":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeGroundUPS[currentPlant]);
                                    break;
                                case "12":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeThreeDaySelectUPS[currentPlant]);
                                    break;
                                case "13":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeNextDayAirSaverUPS[currentPlant]);
                                    break;
                                case "14":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeNextDayAirEarlyAMUPS[currentPlant]);
                                    break;
                                case "59":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeSecondDayAirAMUPS[currentPlant]);
                                    break;
                                case "65":
                                    markupPercentageCWT = Convert.ToDouble(dUpchargeSaverUPS[currentPlant]);
                                    break;
                            }

                            perPackageChargeCWT = dPerPackageChargeCWT[currentPlant];
                            perShipmentChargeCWT = dPerShipmentChargeCWT[currentPlant];
                        }
                        else
                        {
                            sbResults.Append("Unable to find HundredWeight markup charges for plant code: " + currentPlant + "<br/>");
                        }
                        #endregion

                        //if shipment qualifies for hundredweight air and hundredweight method class is air, consider it hundredweight
                        //if shipment qualifies for hundredweight ground and hundredweight method class is ground, consider it hundredweight

                        if (isAirCWT && (serviceCWTType == "AIR-NN"))
                        {
                            isHundredWeight = true;
                            //Do not adjust total charges
                        }
                        else if ((isAirCWT && (serviceCWTType == "AIR")) || (isGroundCWT && (serviceCWTType == "GROUND")))
                        {
                            isHundredWeight = true;
							
							// Since our negotiated CWT rates is 70% of the published rate, reverse that and divide neg. rate by .7 to get published rate
							totalCharges = negotiatedCharges / .7;

                        }
                        else
                        {
                            isHundredWeight = false;
                        }

                        if (isHundredWeight)
                        {
                            markupPercentage = markupPercentageCWT;
                            perPackageCharge = perPackageChargeCWT;
                            perShipmentCharge = perShipmentChargeCWT;
                        }
                        else
                        {
                            markupPercentage = markupPercentageUPS;
                            perPackageCharge = perPackageChargeUPS;
                            perShipmentCharge = perShipmentChargeUPS;
                        }

                        totalCharges += ((markupPercentage / 100) * totalCharges);
                        totalCharges += (perPackageCharge * numPackages) + perShipmentCharge;

                        tblShopServices.Rows.Add(currentPlant, serviceDesc, totalCharges, billingWeight, addressClassification, serviceCharges, transportationCharges, isHundredWeight);

                        shipmentWeight = billingWeight;
                        shipmentClassification = addressClassification;
                    }
                    #endregion



                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    handleError("ShowRates", "Soap Exception - SoapException Message= " + ex.Message + " SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + " SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + " SoapException StackTrace= " + ex.StackTrace);
                    #region -- Handle SOAP error --
                    SoapError = true;
                    sbResults.Append("---------Freight Rate Web Service returns error----------------<br/>");
                    sbResults.Append("---------\"Hard\" is user error \"Transient\" is system error----------------<br/>");
                    sbResults.Append("SoapException Message= " + ex.Message + "<br/>");
                    sbResults.Append("<br/>");
                    sbResults.Append("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + "<br/>");
                    sbResults.Append("<br/>");
                    sbResults.Append("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + "<br/>");
                    sbResults.Append("<br/>");
                    sbResults.Append("SoapException StackTrace= " + ex.StackTrace + "<br/>");
                    sbResults.Append("-------------------------<br/>");
                    #endregion
                }
                catch (System.ServiceModel.CommunicationException ex)
                {
                    handleError("ShowRates", "Communication Exception - CommunicationException= " + ex.Message + " CommunicationException-StackTrace= " + ex.StackTrace);
                    #region -- Handle General Communication Error --
                    SoapError = true;
                    sbResults.Append("<br/>");
                    sbResults.Append("--------------------<br/>");
                    sbResults.Append("CommunicationException= " + ex.Message + "<br/>");
                    sbResults.Append("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
                    sbResults.Append("-------------------------<br/>");
                    sbResults.Append("<br/>");
                    #endregion
                }
                catch (Exception ex)
                {
                    handleError("ShowRates", "General Error - General Exception= " + ex.Message + " General Exception-StackTrace= " + ex.StackTrace);
                    #region -- Handle misc error --
                    SoapError = true;
                    sbResults.Append("<br/>");
                    sbResults.Append("-------------------------<br/>");
                    sbResults.Append(" General Exception= " + ex.Message + "<br/>");
                    sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
                    sbResults.Append("-------------------------<br/>");
                    #endregion
                }


                #endregion

            }

            #region -- Convert to Comparison Format --

            Dictionary<string, int> dPlantColumns = new Dictionary<string, int>();
            dPlantColumns.Add("ALP", 1);
            dPlantColumns.Add("BUT", 2);
            dPlantColumns.Add("FTW", 3);
            dPlantColumns.Add("PDT", 4);
            dPlantColumns.Add("POR", 5);
            dPlantColumns.Add("BMK", 6);

            DataSet dsPriceComparison = new DataSet();
            DataTable tblPriceComparison = dsPriceComparison.Tables.Add();

            tblPriceComparison.Columns.Add("Desc", typeof(string));
            tblPriceComparison.Columns.Add("RateALP", typeof(double));
            tblPriceComparison.Columns.Add("RateBUT", typeof(double));
            tblPriceComparison.Columns.Add("RateFTW", typeof(double));
            tblPriceComparison.Columns.Add("RatePDT", typeof(double));
            tblPriceComparison.Columns.Add("RatePOR", typeof(double));
            tblPriceComparison.Columns.Add("RateBMK", typeof(double));
            tblPriceComparison.Columns.Add("IsHundredWeight", typeof(bool));

            foreach (KeyValuePair<string, string> kvp in dServices)
            {
                tblPriceComparison.Rows.Add(kvp.Value, 0, 0, 0, 0, 0, false);
                sbResults.Append("added row for " + kvp.Value + " to tblPriceComparison<br/>");
            }

            foreach (DataRow row in tblShopServices.Rows)
            {
                string serviceDesc = row["Desc"].ToString();
                double serviceRate = Convert.ToDouble(row["Rate"].ToString());
                string servicePlantFrom = row["Plant"].ToString();
                bool isHundredWeight = Convert.ToBoolean(row["IsHundredWeight"].ToString());

                int iServiceRowIndex = 0;
                bool serviceRowFound = false;
                while ((iServiceRowIndex < tblPriceComparison.Rows.Count) && (!serviceRowFound))
                {
                    if (tblPriceComparison.Rows[iServiceRowIndex]["Desc"] == serviceDesc)
                    {
                        serviceRowFound = true;
                    }
                    else
                    {
                        iServiceRowIndex++;
                    }
                }

                if (serviceRowFound)
                {
                    if (dPlantColumns.ContainsKey(servicePlantFrom))
                    {
                        tblPriceComparison.Rows[iServiceRowIndex][dPlantColumns[servicePlantFrom]] = serviceRate;
                        tblPriceComparison.Rows[iServiceRowIndex]["IsHundredWeight"] = isHundredWeight;
                    }
                    else
                    {
                        //Plant code not found -- something is wrong
                    }

                }

                sbResults.Append("Row data: " + serviceDesc + " - " + serviceRate + " - " + servicePlantFrom + "<br/>");
            }

            //foreach (DataRow row in tblPriceComparison.Rows)
            for (int i = tblPriceComparison.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = tblPriceComparison.Rows[i];
                if ((row[1].ToString() == "0") && (row[2].ToString() == "0") && (row[3].ToString() == "0") && (row[4].ToString() == "0") && (row[5].ToString() == "0"))
                {
                    row.Delete();
                }
            }

            foreach (DataRow row in tblPriceComparison.Rows)
            {
                sbResults.Append("Combined: " + row[0] + " - " + row[1] + " - " + row[2] + " - " + row[3] + " - " + row[4] + " - " + row[5] + "<br/>");
            }

            #endregion

            #region -- Display Shopped Service Rates in Gridview --
            DataView dvPriceComparison = tblPriceComparison.DefaultView;
            dvPriceComparison.Sort = "RateALP";

            if (!SoapError)
            {
                lblResults.Visible = true;// false;
                gvShopServices.DataSource = dvPriceComparison;
                gvShopServices.DataBind();

                gvServices.Visible = false;
                gvShopServices.Visible = true;

                pnlUpsCommError.Visible = false;
            }
            else
            {
                if (numPackages > 50)
                {
                    litUpsCommError.Text = "Unable to rate shipments of more than 50 packages via UPS.";
                    shipmentWeight = totalWeight;
                }
                else
                {
                    //lblResults.Visible = true;
                    litUpsCommError.Text = "There was a error getting results from UPS.  Please check your information and try again.";
                } 
                pnlUpsCommError.Visible = true;
            }

            #region -- log results --
            try
            {
                System.IO.StringWriter swGridResults = new System.IO.StringWriter();

                System.Web.UI.HtmlTextWriter htwGridResults = new HtmlTextWriter(swGridResults);

                gvShopServices.RenderControl(htwGridResults);

                SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                connLog.Open();

                SqlCommand cmdLog = new SqlCommand();
                cmdLog.Connection = connLog;
                cmdLog.CommandType = CommandType.StoredProcedure;
                cmdLog.CommandText = "LogResults";

                SqlParameter pRequestId = new SqlParameter("@RequestId", SqlDbType.Int);
                SqlParameter pPlantCode = new SqlParameter("@PlantCode", SqlDbType.VarChar, 10);
                SqlParameter pFullResults = new SqlParameter("@FullResults", SqlDbType.NText);

                pRequestId.Value = RateRequestId;
                pPlantCode.Value = plantCode;
                pFullResults.Value = swGridResults.ToString();

                cmdLog.Parameters.Add(pRequestId);
                cmdLog.Parameters.Add(pPlantCode);
                cmdLog.Parameters.Add(pFullResults);

                cmdLog.ExecuteNonQuery();
                connLog.Close();
            }
            catch (Exception e)
            {
                handleError("ShowRates", "Error in saving results to DB: " + e.Message);
                sbResults.Append("Error in saving results to DB: " + e.Message + "<br/>");
            }
            #endregion

            #endregion

            #endregion
        } //end of all plants rating process

        #region -- Display our Rating Results --

        lblBillingWeight.Text = shipmentWeight.ToString();

        lblNumPackages.Text = numPackages.ToString();

        if (shipmentClassification == 2)
        {
            lblAddressClassification.Text = "RESIDENTIAL";
        }
        else if (shipmentClassification == 1)
        {
            lblAddressClassification.Text = "COMMERCIAL";
        }
        else
        {
            lblAddressClassification.Text = "UNKNOWN";
        }

        if (addressCorrected)
        {
            lblRateWarning.Text = "The address was automatically corrected to match what UPS had on file.  Please make note of the changes.<br/>";
        }
        else if (shipToStreet == "")
        {
            lblRateWarning.Text = "This is an estimate based on the Zip Code provided.  Please be aware additional charges may apply if the address is residential, in an extended delivery area, etc.<br/>";
        }
        else
        {
            lblRateWarning.Text = "";
        }

        #endregion

        #region -- Show Results View --
        mvPageLayout.SetActiveView(vwRates);
        pnlUpsTrademarkInfo.Visible = true;
        #endregion

        lblResults.Text += sbResults.ToString();
        // Uncomment to show results for debugging
        //lblResults.Visible = true;

    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
    }

    #region -- Address Candidates GridView procedures --
    /// <summary>
    /// When "Select" command is triggered on a displayed Address Candidate, it loads that candidate's details into the user's input fields
    ///  and returns the user to the main data entry view to make any necessary adjustments
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvCandidates_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow selectedRow = gvCandidateData.Rows[index];

            txtAddress.Text = selectedRow.Cells[3].Text.ToString();
            txtCity.Text = selectedRow.Cells[4].Text.ToString();
            txtState.Text = selectedRow.Cells[5].Text.ToString();
            txtZip.Text = selectedRow.Cells[6].Text.ToString();

            mvPageLayout.SetActiveView(vwMain);
            pnlUpsTrademarkInfo.Visible = false;
        }
    }

    /// <summary>
    /// When Address Candidate gridview is populated, converts line breaks in address to HTML breaks for formatting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvCandidates_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string displayAddress = e.Row.Cells[1].Text;
        e.Row.Cells[1].Text = displayAddress.Replace("\n", "<br/>");
    }
    #endregion

    /// <summary>
    /// If address is specified, validates and then attempts to rate.  Otherwise, skips validation and rates using information provided. (on btnValidateAndRate click)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnValidateAndRate_Click(object sender, EventArgs e)
    {
        pnlPageError.Visible = false;
        lblRateAlert.Text = "";
        lblResults.Text = "";
        phPiedmontAccount.Visible = false;
        if (IsValid)
        {
            phPiedmontAccount.Visible = isPiedmontAccount();
            phFreightClassDetailSummary.Visible = (lstRateWithLTL.SelectedValue == "Yes") || (lstRateWithGF.SelectedValue == "Yes");
            phLTLDetailSummary.Visible = (lstRateWithLTL.SelectedValue == "Yes");
            if ((lstRateWithLTL.SelectedValue == "Yes") && ((txtCity.Text.Trim() == "") || (txtState.Text.Trim() == "") || (txtZip.Text.Trim() == "")))
            {
                lblRateAlert.Text = "LTL Rates require City, State, and Zip -- please check your info.";
            }
            else if (((lstShopRates.SelectedValue == "Yes") || (Session["DefaultPlant"].ToString() == "ALP")) && (txtAcctNumber.Text.Trim() == ""))
            {
                lblRateAlert.Text = "For shipments from ALP or marked as \"Rate from Multiple Locations,\" Account # is required.";
            }
            else if (isPoOnlyZip())
			{
				// Value of lblRateAlert.Text has been set appropriately in isPoOnlyZip 
			}
            else if (txtAddress.Text.Trim() != "")
            {
                ValidateAndRateAddress();
            }
            else
            {
                ShowRates("", "", txtCity.Text.ToUpper().Trim(), txtState.Text.ToUpper().Trim(), txtZip.Text.ToUpper().Trim(), lstCountry.SelectedValue, GetPlantToRate(), false);
            }
        }
        lblResults.Visible = true;
    }
	
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
	
	private bool isPoOnlyZip()
	{
		bool isPoOnly = false;
		string valCity = txtCity.Text.Trim();
        string valState = GetSelectedState();
        string valZip = txtZip.Text.Trim();
		
		SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
		conn.Open();

		SqlCommand cmd = new SqlCommand();
		cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
		cmd.CommandText = "VerifyAndCorrect_PO_ZipCode";

        SqlParameter pCity = new SqlParameter("@City", SqlDbType.VarChar, 200);
        SqlParameter pState = new SqlParameter("@State", SqlDbType.VarChar, 50);
        SqlParameter pZipCode = new SqlParameter("@ZipCode", SqlDbType.VarChar, 50);
        SqlParameter pCorrectZipCode = new SqlParameter("@CorrectZipCode", SqlDbType.VarChar, 50);

        pCity.Value = valCity;
        pState.Value = valState;
        pZipCode.Value = valZip;
        pCorrectZipCode.Direction = ParameterDirection.Output;

        cmd.Parameters.Add(pCity);
        cmd.Parameters.Add(pState);
        cmd.Parameters.Add(pZipCode);
        cmd.Parameters.Add(pCorrectZipCode);

        cmd.ExecuteNonQuery();

        string correctedZip = pCorrectZipCode.Value.ToString();

        conn.Close();

        string errorMsg = "";

        if (valZip != correctedZip)
        {
            isPoOnly = true;
            if (correctedZip == "BAD_INPUT")
            {
                errorMsg = "Please check your City / State / Zip values, as they appear to be invalid.";
            }
            else if (correctedZip == "NO_ALTERNATIVE")
            {
                errorMsg = "The Zip Code you entered is a PO Only zip code, and there is no alternative zip code available.  We cannot rate to PO Only zip codes.";
            }
            else
            {
                errorMsg = "The Zip Code you entered is a PO Only zip code.  We cannot rate to PO Only zip codes.  Our suggestion is to use this Zip Code instead: " + correctedZip;
            }
        }

        lblRateAlert.Text = errorMsg;

        return (isPoOnly);
	}

    /// <summary>
    /// Loads user's default plant if exists, allows choice if first time use
    /// </summary>
    private void VerifyUserPrefs()
    {
        // Pull Username of current user
        string UserName = HttpContext.Current.User.Identity.Name.Replace(Config.NetworkDomain + "\\", "").ToLower();
        lblUserName.Text = "(" + UserName + ") " + DateTime.Now.ToLongTimeString();

        // Store Username to Session variable
        Session["UserName"] = UserName;

        // Pull Default plant for User
        string DefaultPlant = GetDefaultPlant(UserName);

        // If the user has a default plant, save it to session and show main view
        if (DefaultPlant != "")
        {
            Session["DefaultPlant"] = DefaultPlant;
            ShowMainView();
        }
        // Otherwise, allow user to select a default plant
        else
        {
            Session["DefaultPlant"] = "";
            ShowPlantOptions();
        }
    }

    /// <summary>
    /// Displays Plant Selection view
    /// </summary>
    private void ShowPlantOptions()
    {
        mvPageLayout.SetActiveView(vwSelectPlant);
        pnlUpsTrademarkInfo.Visible = false;
        pnlFirstVisitInstructions.Visible = (Session["DefaultPlant"].ToString() == "");
    }

    /// <summary>
    /// Calls SetDefaultPlant, passing the plant code passed in as an argument (on btnSelectPlant click)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelectPlant_Click(object sender, CommandEventArgs e)
    {
        SetDefaultPlant(e.CommandArgument.ToString());
    }

    /// <summary>
    /// Gets the Default Plant Code for user based on Username
    /// </summary>
    /// <param name="UserName">User's domain username</param>
    /// <returns>Plant code for Default Plant if one exists for this user, otherwise empty string</returns>
    private string GetDefaultPlant(string UserName)
    {
        string DefaultPlant = "";

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT DefaultPlant FROM Users WHERE UserName = '" + UserName + "'";

        SqlDataReader drResults = cmd.ExecuteReader();

        if (drResults.Read())
        {
            DefaultPlant = drResults["DefaultPlant"].ToString();
        }
        else
        {
            DefaultPlant = "";
        }

        conn.Close();

        return (DefaultPlant);
    }

    /// <summary>
    /// Sets the Default Plant Session variable, and optionally stores default plant to DB if this is first time use
    /// </summary>
    /// <param name="PlantCode">Plant Code of selected Default Plant</param>
    private void SetDefaultPlant(string PlantCode)
    {
        /*
        // If the Session variable is empty, then this is the first plant selection for this user, so store the default to the DB
        if (Session["DefaultPlant"].ToString() == "")
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO Users (UserName, DefaultPlant) VALUES ('" + Session["UserName"].ToString() + "','" + PlantCode + "')";

            cmd.ExecuteNonQuery();
        }

        Session["DefaultPlant"] = PlantCode;

        ShowMainView();
        */

        litPlant.Text = PlantCode;

        Response.Cookies["plant"].Value = PlantCode;
        Response.Cookies["plant"].Expires = DateTime.Now.AddDays(180);

        Session["DefaultPlant"] = PlantCode;
        pnlAcctNumber.Visible = (Session["DefaultPlant"].ToString() == "ALP");

        ShowMainView();
    }

    /// <summary>
    /// Show the Select Plant view (on btnChangePlant click)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnChangePlant_Click(object sender, EventArgs e)
    {
        pnlFirstVisitInstructions.Visible = false;
        mvPageLayout.SetActiveView(vwSelectPlant);
        pnlUpsTrademarkInfo.Visible = false;
    }


    protected void lstRateWithGF_SelectedIndexChanged(object sender, EventArgs e)
    {
        pnlFreightClass.Visible = (lstRateWithLTL.SelectedValue == "Yes") || (lstRateWithGF.SelectedValue == "Yes");
    }

    protected void lstRateWithLTL_SelectedIndexChanged(object sender, EventArgs e)
    {
        pnlLTLOptions.Visible = (lstRateWithLTL.SelectedValue == "Yes");
        pnlFreightClass.Visible = (lstRateWithLTL.SelectedValue == "Yes") || (lstRateWithGF.SelectedValue == "Yes");
    }

    /// <summary>
    /// Calls ShowRates with the corrected UPS address as parameters rather than the user's entered text
    /// </summary>
    private void RateWithCorrectedAddress()
    {
        ShowRates("", lblCorrectedAddressStreet.Text, lblCorrectedAddressCity.Text, lblCorrectedAddressState.Text, lblCorrectedAddressZip.Text, lstCountry.SelectedValue, GetPlantToRate(), true);
    }

    /// <summary>
    /// Calls ShowRates with the user's entered text
    /// </summary>
    private void RateWithEnteredAddress()
    {
        ShowRates("", txtAddress.Text.Trim().ToUpper(), txtCity.Text.Trim().ToUpper(), GetSelectedState().ToUpper(), txtZip.Text.Trim().ToUpper(), lstCountry.SelectedValue, GetPlantToRate(), false);
    }

    /// <summary>
    /// Gets the individual Plant Code we are "Shipping From" for the purposes of rating, or "ALL" if we should shop rates across all plants
    /// </summary>
    /// <returns>Plant Code or "ALL"</returns>
    private string GetPlantToRate()
    {
        string plantToRate = "";
        if (lstShopRates.SelectedValue == "Yes")
        {
            plantToRate = "ALL";
        }
        else
        {
            plantToRate = Session["DefaultPlant"].ToString();
        }

        return (plantToRate);
    }

    /// <summary>
    /// Gets State / Province value entered by user
    /// </summary>
    /// <returns>State / Province value</returns>
    private string GetSelectedState()
    {
        string selectedState = "";
        selectedState = txtState.Text.Trim();

        return (selectedState);
    }

    /// <summary>
    /// Displays main data entry view
    /// </summary>
    private void ShowMainView()
    {
        mvPageLayout.SetActiveView(vwMain);
        pnlUpsTrademarkInfo.Visible = false;

        litShipFromPlant.Text = Config.PlantNames[Session["DefaultPlant"].ToString()];
        
        pnlAcctNumber.Visible = (Session["DefaultPlant"].ToString() == "ALP");
        if (Session["DefaultPlant"].ToString() != "ALP")
        {
            txtAcctNumber.Text = "";
        }

        lblRateAlert.Text = "";

    }

    /// <summary>
    /// Clears user data from and displays main data entry view (on btnStartOver click)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnStartOver_Click(object sender, EventArgs e)
    {
        pnlPageError.Visible = false;
        lblResults.Visible = true;// false;

        mvPageLayout.SetActiveView(vwMain);
        pnlUpsTrademarkInfo.Visible = false;
        txtAddress.Text = "";
        txtCity.Text = "";
        txtState.Text = "";
        txtZip.Text = "";
        lstCountry.SelectedIndex = 0;
        lstDeliveryConfirmation.SelectedIndex = 0;
        lstShopRates.SelectedIndex = 0;
        txtNumPackages.Text = "";
        txtPackageWeight.Text = "";
        txtLastPackageWeight.Text = "";

        txtPickupDate.Text = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString().Substring(2);
        lstLTLClass.SelectedValue = "55";
        lstRateWithLTL.SelectedValue = "No";
        lstRateWithGF.SelectedValue = "No";
        pnlLTLOptions.Visible = false;
        chkLTLInsideDelivery.Checked = false;
        chkLTLInsidePickup.Checked = false;
        chkLTLLiftgateDelivery.Checked = false;
        chkLTLLiftgatePickup.Checked = false;
        chkLTLLimitedAccessDelivery.Checked = false;
        chkLTLLimitedAccessPickup.Checked = false;
        chkLTLNotifyBeforeDelivery.Checked = false;
        chkLTLResidentialDelivery.Checked = false;
        chkLTLResidentialPickup.Checked = false;
        chkLTLSortAndSegregate.Checked = false;
        chkLTLStopoffCharge.Checked = false;

    }

    /// <summary>
    /// Shows the main data entry view without changing existing data (on btnBackToEdit click)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBackToEdit_Click(object sender, EventArgs e)
    {

        lblResults.Visible = true;// false;

        mvPageLayout.SetActiveView(vwMain);
        pnlUpsTrademarkInfo.Visible = false;
    }

    protected void handleError(string source, string details)
    {
        pnlPageError.Visible = true;
        mvPageLayout.SetActiveView(vwMain);

        // Create a writer and open the file:
        StreamWriter log;

        //string logFile = @"C:\WebSites\Freight Estimator\Log\Freight_Estimator_Log.txt";
        string logFile = HttpRuntime.AppDomainAppPath + @"\Log\Freight_Estimator_Log.txt";
        

        if (!File.Exists(logFile))
        {
            log = new StreamWriter(logFile);
        }
        else
        {
            log = File.AppendText(logFile);
        }

        // Write to the file:
        log.WriteLine(DateTime.Now);
        log.WriteLine(source);
        log.WriteLine(details);
        log.WriteLine();

        // Close the stream:
        log.Close();
    }


    #region -- Not Currently Used --
    /// <summary>
    /// Currently not used, as corrected address is _automatically_ used to proceed.
    /// If used, shows the Main data entry view without changing existing data (on btnEditAddress click)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnEditAddress_Click(object sender, EventArgs e)
    {
        mvPageLayout.SetActiveView(vwMain);
        pnlUpsTrademarkInfo.Visible = false;
    }

    /// <summary>
    /// Currently not used, as corrected address is _automatically_ used to proceed.
    /// If used, proceed with Rating request using corrected address (on btnAcceptCorrectedAddress click)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAcceptCorrectedAddress_Click(object sender, EventArgs e)
    {
        RateWithCorrectedAddress();
    }
    #endregion
    

    protected void lstShopRates_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool acctRequired = false;
        acctRequired = ((lstShopRates.SelectedValue.ToString().ToUpper() == "YES") || (Session["DefaultPlant"].ToString() == "ALP"));

        if (!acctRequired)
        {
            txtAcctNumber.Text = "";
            pnlAcctNumber.Visible = false;
        }
        else
        {
            pnlAcctNumber.Visible = true;
        }

    }
}