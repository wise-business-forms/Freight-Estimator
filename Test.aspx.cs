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
using UpsRateWebReference;
using UpsAddressValidationWebReference;



public partial class Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if((!Page.IsPostBack) || (Session["DefaultPlant"] == null))
        {
            VerifyUserPrefs();
            PopulateTestAddress();
            pnlDebugging.Visible = true;
            //ShowRates("", "", "", "", "30114", "US", "ALP", false);
        }
        //RunTestCode();
    }

    protected void btnValidateAndRate_Click(object sender, EventArgs e)
    {
        if (txtAddress.Text.Trim() != "")
        {
            ValidateAndRate();
        }
        else
        {
            ShowRates("", "", "", "", txtZip.Text.Trim(), lstCountry.SelectedValue, GetPlantToRate(), false);
        }
    }

    protected void btnToggleDebugging_Click(object sender, EventArgs e)
    {
        pnlDebugging.Visible = !(pnlDebugging.Visible);
    }

    protected void gvCandidates_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow selectedRow = gvCandidateData.Rows[index];

            txtAddress.Text = selectedRow.Cells[3].Text.ToString();
            txtCity.Text = selectedRow.Cells[4].Text.ToString();
            if (lstCountry.SelectedValue == "US")
            {
                lstState.SelectedValue = selectedRow.Cells[5].Text.ToString();
            }
            else
            {
                txtState.Text = selectedRow.Cells[5].Text.ToString();
            }
            txtZip.Text = selectedRow.Cells[6].Text.ToString();

            mvPageLayout.SetActiveView(vwMain);
            pnlUpsTrademarkInfo.Visible = false;
        }
    }

    protected void gvCandidates_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string displayAddress = e.Row.Cells[1].Text;
        //e.Row.Cells[1].Text = comment.Replace("&lt;br&gt;", "<br/>");
        e.Row.Cells[1].Text = displayAddress.Replace("\n", "<br/>");
    }


    private void ValidateAndRate()
    {
        StringBuilder sbResults = new StringBuilder();

        try
        {
            XAVService xavService = new XAVService();
            XAVRequest xavRequest = new XAVRequest();

            UpsAddressValidationWebReference.RequestType request = new UpsAddressValidationWebReference.RequestType();
            String[] requestOption = { "3" };
            request.RequestOption = requestOption;
            xavRequest.Request = request;

            #region -- Access Security (license number, username, password) --
            UpsAddressValidationWebReference.UPSSecurity upss = new UpsAddressValidationWebReference.UPSSecurity();
            UpsAddressValidationWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsAddressValidationWebReference.UPSSecurityServiceAccessToken();
            upsSvcToken.AccessLicenseNumber = "CC83ED82D080DC80";
            upss.ServiceAccessToken = upsSvcToken;
            UpsAddressValidationWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsAddressValidationWebReference.UPSSecurityUsernameToken();
            upsSecUsrnameToken.Username = "WiseWebSupport";
            upsSecUsrnameToken.Password = "wise_forms";
            upss.UsernameToken = upsSecUsrnameToken;
            xavService.UPSSecurityValue = upss;
            #endregion

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

            XAVResponse xavResponse = xavService.ProcessXAV(xavRequest);
            
            string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
            string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

            string targetUrl = xavService.Url;
            string UserName = HttpContext.Current.User.Identity.Name.Replace("WISENT\\", "").ToLower();

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

                //cmdLog.ExecuteNonQuery();
                connLog.Close();
            }
            catch (Exception e)
            {
                sbResults.Append("Error in saving request to DB: " + e.Message + "<br/>");
            }

            //sbResults.AppendLine("Response Status Code " + xavResponse.Response.ResponseStatus.Code);
            //sbResults.AppendLine("Response Status Description " + xavResponse.Response.ResponseStatus.Description);


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
                if(lstCountry.SelectedValue == "US") {
                    addressedSubmitted += txtCity.Text.Trim() + " " + lstState.SelectedValue + " " + txtZip.Text.Trim();
                } else {
                    addressedSubmitted += txtCity.Text.Trim() + " " + txtState.Text.Trim() + " " + txtZip.Text.Trim();
                }
                lblAddressSubmitted.Text = addressedSubmitted.ToUpper();

                DataView dvCandidates = tblCandidates.DefaultView;
                dvCandidates.Sort = "ID";

                gvCandidates.DataSource = dvCandidates;
                gvCandidates.DataBind();

                //DataView dvCandidateData = tblCandidateData.DefaultView;
                //dvCandidateData.Sort = "ID";
                gvCandidateData.DataSource = dvCandidates;
                gvCandidateData.DataBind();

                mvPageLayout.SetActiveView(vwSelectAddressCandidate);
                pnlUpsTrademarkInfo.Visible = false;
            }


        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
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
            #region -- Handle misc error --
            sbResults.Append("<br/>");
            sbResults.Append("-------------------------<br/>");
            sbResults.Append(" General Exception= " + ex.Message + "<br/>");
            sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
            sbResults.Append("-------------------------<br/>");
            #endregion
        }
        lblResults.Text += sbResults.ToString();
    }

    private void ShowRates(string shipToConsignee, string shipToStreet, string shipToCity, string shipToState, string shipToZip, string shipToCountry, string plantCode, bool addressCorrected)
    {
        //lblRateConsignee.Text = shipToConsignee;
        lblRateAddress.Text = shipToStreet;
        lblRateCity.Text = shipToCity;
        lblRateState.Text = shipToState;
        lblRateZip.Text = shipToZip;

        StringBuilder sbResults = new StringBuilder();

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


        #region -- Define Per Package Charge and Per Shipment Charge dictionaries standard (charge per plant) --
        Dictionary<string, double> dPerPackageChargeUPS = new Dictionary<string, double>();
        Dictionary<string, double> dPerShipmentChargeUPS = new Dictionary<string, double>();

        SqlConnection connCharges = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
        connCharges.Open();

        SqlCommand cmdCharges = new SqlCommand();
        cmdCharges.Connection = connCharges;
        cmdCharges.CommandType = CommandType.Text;
        cmdCharges.CommandText = "SELECT PlantCode, PerPackageCharge, PerShipmentCharge FROM PlantCarrierCharges WHERE CarrierId = 'UPS' ORDER BY PlantCode";

        SqlDataReader drCharges = cmdCharges.ExecuteReader();

        while (drCharges.Read())
        {
            dPerPackageChargeUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerPackageCharge"].ToString()));
            dPerShipmentChargeUPS.Add(drCharges["PlantCode"].ToString(), Convert.ToDouble(drCharges["PerShipmentCharge"].ToString()));
        }

        connCharges.Close();

        #endregion


        #region -- Define Per Package Charge and Per Shipment Charge dictionaries HundredWeight (charge per plant) --
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

        #endregion
        

        if (plantCode != "ALL")
        {  
            #region -- Process rate request for a single plant --
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
                upsSvcToken.AccessLicenseNumber = "CC83ED82D080DC80";
                upss.ServiceAccessToken = upsSvcToken;
                UpsRateWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsRateWebReference.UPSSecurityUsernameToken();
                upsSecUsrnameToken.Username = "WiseWebSupport";
                upsSecUsrnameToken.Password = "wise_forms";
                upss.UsernameToken = upsSecUsrnameToken;
                rateService.UPSSecurityValue = upss;
                #endregion

                #region -- Build Shipment object --
                ShipmentType shipment = new ShipmentType();

                #region -- Shipper --
                ShipperType shipper = new ShipperType();
                shipper.ShipperNumber = "391287";
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
                int numPackages = Convert.ToInt16(txtNumPackages.Text.Trim());
                string sPkgWeight = txtPackageWeight.Text.Trim();
                string sLastPkgWeight = txtLastPackageWeight.Text.Trim();

                if (sLastPkgWeight == "")
                {
                    sLastPkgWeight = sPkgWeight;
                }

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

                ShipmentRatingOptionsType ratingOptions = new ShipmentRatingOptionsType();
                ratingOptions.NegotiatedRatesIndicator = "";
                shipment.ShipmentRatingOptions = ratingOptions;

                rateRequest.Shipment = shipment;

                #endregion

                #region -- Submit Rate Request --

                RateResponse rateResponse = rateService.ProcessRate(rateRequest);
                
                RatedShipmentType[] ratedShipments = rateResponse.RatedShipment;

                string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
                string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

                string targetUrl = rateService.Url;
                string UserName = HttpContext.Current.User.Identity.Name.Replace("WISENT\\", "").ToLower();

                #region -- log request --
                try
                {
                    SqlConnection connLog = new SqlConnection(ConfigurationManager.ConnectionStrings["UpsRateSqlConnection"].ConnectionString);
                    connLog.Open();

                    SqlCommand cmdLog = new SqlCommand();
                    cmdLog.Connection = connLog;
                    cmdLog.CommandType = CommandType.StoredProcedure;
                    cmdLog.CommandText = "LogRequest_Rate_Test";

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
                    pAddress.Value = shipToStreet;
                    pCity.Value = shipToCity;
                    pState.Value = shipToState;
                    pZip.Value = shipToZip;
                    pCountry.Value = shipToCountry;
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
                tblServices.Columns.Add("NegotiatedRate", typeof(double));

                #endregion

                #region -- Process each rated service --

                double shipmentWeight = 0;
                int shipmentClassification = 0;

                foreach (RatedShipmentType ratedShipment in ratedShipments)
                {
                    string serviceDesc = "";
                    int addressClassification = 1;
                    double billingWeight = 0;
                    double serviceCharges = 0;
                    double transportationCharges = 0;
                    double totalCharges = 0;
                    double negotiatedRate = 0;

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
                    negotiatedRate = Convert.ToDouble(ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue);

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
                        sbResults.Append("Negotiated Rate Total Charges: " + ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue + "<br/>");
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

                    if (dPerPackageChargeUPS.ContainsKey(plantCode))
                    {
                        totalCharges += (0.1 * totalCharges);
                        totalCharges += (dPerPackageChargeUPS[plantCode] * numPackages) + dPerShipmentChargeUPS[plantCode];
                    }
                    else
                    {
                        sbResults.Append("Unable to find charges for plant code: " + plantCode + "<br/>");
                    }

                    tblServices.Rows.Add(plantCode, serviceDesc, totalCharges, billingWeight, addressClassification, serviceCharges, transportationCharges, negotiatedRate);

                    shipmentWeight = billingWeight;
                    shipmentClassification = addressClassification;
                }
                /*
                foreach (DataRow row in tblServices.Rows)
                {
                    sbResults.Append(row["Desc"] + " - " + row["Rate"] + "<br/>");
                }
                */
                #endregion

                #region -- Display our Rating Results --

                #region -- Display Service Rates in Gridview --
                DataView dvServices = tblServices.DefaultView;
                dvServices.Sort = "Rate";

                gvServices.DataSource = dvServices;
                gvServices.DataBind();

                gvShopServices.Visible = false;
                gvServices.Visible = true;

                #endregion

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

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                #region -- Handle SOAP error --
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
                #region -- Handle misc error --
                sbResults.Append("<br/>");
                sbResults.Append("-------------------------<br/>");
                sbResults.Append(" General Exception= " + ex.Message + "<br/>");
                sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                #endregion
            }

            lblResults.Text += sbResults.ToString();

            mvPageLayout.SetActiveView(vwRates);
            pnlUpsTrademarkInfo.Visible = true;

            #endregion
        } //end of single Plant rating process
        else 
        {
            // Shop rates for all plants
            string[] plantCodes = { "ALP", "BUT", "FTW", "PDT", "POR" };

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
            
            #endregion

            for (int iPlantCount = 0; iPlantCount < plantCodes.Count(); iPlantCount++) 
            {
                string currentPlant = plantCodes[iPlantCount];

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
                    upsSvcToken.AccessLicenseNumber = "CC83ED82D080DC80";
                    upss.ServiceAccessToken = upsSvcToken;
                    UpsRateWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsRateWebReference.UPSSecurityUsernameToken();
                    upsSecUsrnameToken.Username = "WiseWebSupport";
                    upsSecUsrnameToken.Password = "wise_forms";
                    upss.UsernameToken = upsSecUsrnameToken;
                    rateService.UPSSecurityValue = upss;
                    #endregion

                    #region -- Build Shipment object --
                    ShipmentType shipment = new ShipmentType();

                    #region -- Shipper --
                    ShipperType shipper = new ShipperType();
                    shipper.ShipperNumber = "391287";
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
                    int numPackages = Convert.ToInt16(txtNumPackages.Text.Trim());
                    string sPkgWeight = txtPackageWeight.Text.Trim();
                    string sLastPkgWeight = txtLastPackageWeight.Text.Trim();

                    if (sLastPkgWeight == "")
                    {
                        sLastPkgWeight = sPkgWeight;
                    }

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

                    rateRequest.Shipment = shipment;

                    #endregion

                    #region -- Submit Rate Request --

                    RateResponse rateResponse = rateService.ProcessRate(rateRequest);

                    RatedShipmentType[] ratedShipments = rateResponse.RatedShipment;

                    string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
                    string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

                    string targetUrl = rateService.Url;
                    string UserName = HttpContext.Current.User.Identity.Name.Replace("WISENT\\", "").ToLower();

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

                        pUserName.Value = UserName;
                        pTargetUrl.Value = targetUrl;
                        pAddress.Value = shipToStreet;
                        pCity.Value = shipToCity;
                        pState.Value = shipToState;
                        pZip.Value = shipToZip;
                        pCountry.Value = shipToCountry;
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
                        sbResults.Append("Error in saving request to DB: " + e.Message + "<br/>");
                    }
                    #endregion

                    #endregion

                    #region -- Process each rated service --

                    double shipmentWeight = 0;
                    int shipmentClassification = 0;

                    foreach (RatedShipmentType ratedShipment in ratedShipments)
                    {
                        string serviceDesc = "";
                        int addressClassification = 1;
                        double billingWeight = 0;
                        double serviceCharges = 0;
                        double transportationCharges = 0;
                        double totalCharges = 0;

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
                            sbResults.Append("Negotiated Rate Total Charges: " + ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue + "<br/>");
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

                        if (dPerPackageChargeUPS.ContainsKey(currentPlant))
                        {
                            totalCharges += (0.1 * totalCharges);
                            totalCharges += (dPerPackageChargeUPS[currentPlant] * numPackages) + dPerShipmentChargeUPS[currentPlant];
                        }
                        else
                        {
                            sbResults.Append("Unable to find charges for plant code: " + plantCode + "<br/>");
                        }

                        tblShopServices.Rows.Add(currentPlant, serviceDesc, totalCharges, billingWeight, addressClassification, serviceCharges, transportationCharges);

                        shipmentWeight = billingWeight;
                        shipmentClassification = addressClassification;
                    }
                    /*
                    foreach (DataRow row in tblServices.Rows)
                    {
                        sbResults.Append(row["Desc"] + " - " + row["Rate"] + "<br/>");
                    }
                    */
                    #endregion

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

                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    #region -- Handle SOAP error --
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
                    #region -- Handle misc error --
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

            DataSet dsPriceComparison = new DataSet();
            DataTable tblPriceComparison = dsPriceComparison.Tables.Add();

            tblPriceComparison.Columns.Add("Desc", typeof(string));
            tblPriceComparison.Columns.Add("RateALP", typeof(double));
            tblPriceComparison.Columns.Add("RateBUT", typeof(double));
            tblPriceComparison.Columns.Add("RateFTW", typeof(double));
            tblPriceComparison.Columns.Add("RatePDT", typeof(double));
            tblPriceComparison.Columns.Add("RatePOR", typeof(double));

            foreach (KeyValuePair<string, string> kvp in dServices)
            {
                tblPriceComparison.Rows.Add(kvp.Value, 0, 0, 0, 0, 0);
                sbResults.Append("added row for " + kvp.Value + " to tblPriceComparison<br/>");
            }

            foreach (DataRow row in tblShopServices.Rows)
            {
                string serviceDesc = row["Desc"].ToString();
                double serviceRate = Convert.ToDouble(row["Rate"].ToString());
                string servicePlantFrom = row["Plant"].ToString();

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
                    }
                    else
                    {
                        //Plant code not found -- something is wrong
                    }
                    
                }
                
                sbResults.Append("Row data: " + serviceDesc + " - " + serviceRate + " - " + servicePlantFrom + "<br/>");
            }

            //foreach (DataRow row in tblPriceComparison.Rows)
            for(int i=tblPriceComparison.Rows.Count - 1; i >= 0; i--)
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

            gvShopServices.DataSource = dvPriceComparison;
            gvShopServices.DataBind();

            gvServices.Visible = false;
            gvShopServices.Visible = true;

            #endregion

            lblResults.Text += sbResults.ToString();

            mvPageLayout.SetActiveView(vwRates);
            pnlUpsTrademarkInfo.Visible = true;
        }

    }

    protected void lstCountry_SelectedIndexChanged(object sender, System.EventArgs e)
    {/*
        if (lstCountry.SelectedValue == "US")
        {
            mvStateSelection.SetActiveView(vwStateListBox);
        }
        else
        {
            mvStateSelection.SetActiveView(vwStateTextBox);
        }
      */
    }

    private void VerifyUserPrefs()
    {
        string UserName = HttpContext.Current.User.Identity.Name.Replace("WISENT\\", "").ToLower();

        Session["UserName"] = UserName;

        string DefaultPlant = GetDefaultPlant(UserName);

        if (DefaultPlant != "")
        {   
            Session["DefaultPlant"] = DefaultPlant;
            ShowMainView();
        }
        else
        {
            Session["DefaultPlant"] = "";
            ShowPlantOptions();
        }
    }

    private void ShowPlantOptions()
    {
        mvPageLayout.SetActiveView(vwSelectPlant);
        pnlUpsTrademarkInfo.Visible = false;
        pnlFirstVisitInstructions.Visible = (Session["DefaultPlant"].ToString() == "");
    }

    protected void btnPlantALP_Click(object sender, EventArgs e)
    {
        SetDefaultPlant("ALP");
    }

    protected void btnPlantBUT_Click(object sender, EventArgs e)
    {
        SetDefaultPlant("BUT");
    }

    protected void btnPlantFTW_Click(object sender, EventArgs e)
    {
        SetDefaultPlant("FTW");
    }

    protected void btnPlantPDT_Click(object sender, EventArgs e)
    {
        SetDefaultPlant("PDT");
    }

    protected void btnPlantPOR_Click(object sender, EventArgs e)
    {
        SetDefaultPlant("POR");
    }

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

    private void SetDefaultPlant(string PlantCode)
    {
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
    }

    protected void btnChangePlant_Click(object sender, EventArgs e)
    {
        pnlFirstVisitInstructions.Visible = false;
        mvPageLayout.SetActiveView(vwSelectPlant);
        pnlUpsTrademarkInfo.Visible = false;
    }

    protected void btnEditAddress_Click(object sender, EventArgs e)
    {
        mvPageLayout.SetActiveView(vwMain);
        pnlUpsTrademarkInfo.Visible = false;
    }

    protected void btnAcceptCorrectedAddress_Click(object sender, EventArgs e)
    {
        RateWithCorrectedAddress();      
    }

    private void RateWithCorrectedAddress()
    {
        ShowRates("", lblCorrectedAddressStreet.Text, lblCorrectedAddressCity.Text, lblCorrectedAddressState.Text, lblCorrectedAddressZip.Text, lstCountry.SelectedValue, GetPlantToRate(), true);
    }

    private void RateWithEnteredAddress()
    {
        ShowRates("", txtAddress.Text.Trim().ToUpper(), txtCity.Text.Trim().ToUpper(), GetSelectedState().ToUpper(), txtZip.Text.Trim().ToUpper(), lstCountry.SelectedValue, GetPlantToRate(), false);
    }

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

    private string GetSelectedState()
    {
        string selectedState = "";
        /*if (lstCountry.SelectedValue == "US")
        {
            selectedState = lstState.SelectedValue;
        }
        else
        {*/
            selectedState = txtState.Text.Trim();
        /*}*/

        return (selectedState);
    }

    private void ShowMainView()
    {
        Dictionary<string, string> dPlants = new Dictionary<string, string>();
        dPlants.Add("ALP", "Alpharetta");
        dPlants.Add("BUT", "Butler");
        dPlants.Add("FTW", "Ft Wayne");
        dPlants.Add("PDT", "Piedmont");
        dPlants.Add("POR", "Portland");

        mvPageLayout.SetActiveView(vwMain);
        pnlUpsTrademarkInfo.Visible = false;

        lblShipFromPlant.Text = dPlants[Session["DefaultPlant"].ToString()];

    }


    private void PopulateTestAddress()
    {
        txtAddress.Text = "405 Main St";
        txtCity.Text = "New York";
        txtState.Text = "NY";
        txtZip.Text = "10044-0352";
        lstCountry.SelectedValue = "US";
        txtNumPackages.Text = "4";
        txtPackageWeight.Text = "10";   
    }

    protected void btnStartOver_Click(object sender, EventArgs e)
    {
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
    }

    protected void btnBackToEdit_Click(object sender, EventArgs e)
    {
        mvPageLayout.SetActiveView(vwMain);
        pnlUpsTrademarkInfo.Visible = false;
    }

    protected void btnValidateTest_Click(object sender, EventArgs e)
    {

        StringBuilder sbResults = new StringBuilder();

        try
        {
            XAVService xavService = new XAVService();
            XAVRequest xavRequest = new XAVRequest();

            UpsAddressValidationWebReference.RequestType request = new UpsAddressValidationWebReference.RequestType();
            String[] requestOption = { "3" };
            request.RequestOption = requestOption;
            xavRequest.Request = request;

            #region -- Access Security (license number, username, password) --
            UpsAddressValidationWebReference.UPSSecurity upss = new UpsAddressValidationWebReference.UPSSecurity();
            UpsAddressValidationWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsAddressValidationWebReference.UPSSecurityServiceAccessToken();
            upsSvcToken.AccessLicenseNumber = "CC83ED82D080DC80";
            upss.ServiceAccessToken = upsSvcToken;
            UpsAddressValidationWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsAddressValidationWebReference.UPSSecurityUsernameToken();
            upsSecUsrnameToken.Username = "WiseWebSupport";
            upsSecUsrnameToken.Password = "wise_forms";
            upss.UsernameToken = upsSecUsrnameToken;
            xavService.UPSSecurityValue = upss;
            #endregion

            AddressKeyFormatType addressKeyFormat = new AddressKeyFormatType();
            String[] addressLine = { txtAddress.Text.Trim() };
            addressKeyFormat.AddressLine = addressLine;
            addressKeyFormat.PoliticalDivision2 = txtCity.Text.Trim();
            if (lstCountry.SelectedValue == "US")
            {
                addressKeyFormat.PoliticalDivision1 = lstState.SelectedValue;
            }
            else
            {
                addressKeyFormat.PoliticalDivision1 = txtState.Text.Trim();
            }
            addressKeyFormat.PostcodePrimaryLow = txtZip.Text.Trim();
            addressKeyFormat.CountryCode = lstCountry.SelectedValue;
            xavRequest.AddressKeyFormat = addressKeyFormat;

            XAVResponse xavResponse = xavService.ProcessXAV(xavRequest);

            //sbResults.AppendLine("Response Status Code " + xavResponse.Response.ResponseStatus.Code);
            //sbResults.AppendLine("Response Status Description " + xavResponse.Response.ResponseStatus.Description);


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
                        displayAddress += candidateAttention + "<br/>";
                    }
                    if (candidateConsignee != "")
                    {
                        displayAddress += candidateConsignee + "<br/>";
                    }
                    displayAddress += candidateAddress + "<br/>";
                    displayAddress += candidateCity + " " + candidateState + " " + candidateZip;

                    tblCandidates.Rows.Add(candidateCount, candidateAttention, candidateConsignee, candidateAddress, candidateCity, candidateState, candidateZip, displayAddress);

                    sbResults.Append("<br/>");
                } // end of foreach candidate loop
            }


            if (numCandidates == 1)
            {
                string selectedState = "";
                if (lstCountry.SelectedValue == "US")
                {
                    selectedState = lstState.SelectedValue;
                }
                else
                {
                    selectedState = txtState.Text.Trim();
                }

                if ((candidateAddress.ToUpper() != txtAddress.Text.Trim().ToUpper())
                 || (candidateCity.ToUpper() != txtCity.Text.Trim().ToUpper())
                 || (candidateState.ToUpper() != selectedState.ToUpper())
                 || (candidateZip.ToUpper() != txtZip.Text.Trim().ToUpper()))
                {
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

                    mvPageLayout.SetActiveView(vwVerifyAddressCorrection);
                    pnlUpsTrademarkInfo.Visible = true;
                }
                else // Our address exactly matches the UPS candidate, so move forward
                {
                    sbResults.Append(" -- EXACT MATCH -- ");

                }
            } // end of numCandidates == 1 logic
            else if (numCandidates > 1)
            {

            }


        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
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

        }
        catch (System.ServiceModel.CommunicationException ex)
        {
            sbResults.Append("<br/>");
            sbResults.Append("--------------------<br/>");
            sbResults.Append("CommunicationException= " + ex.Message + "<br/>");
            sbResults.Append("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
            sbResults.Append("-------------------------<br/>");
            sbResults.Append("<br/>");

        }
        catch (Exception ex)
        {
            sbResults.Append("<br/>");
            sbResults.Append("-------------------------<br/>");
            sbResults.Append(" General Exception= " + ex.Message + "<br/>");
            sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
            sbResults.Append("-------------------------<br/>");

        }
        lblResults.Text = sbResults.ToString();

    }

    /*    protected void btnValidate_Click(object sender, EventArgs e)
        {
        
            StringBuilder sbResults = new StringBuilder();

            try
            {
                XAVService xavService = new XAVService();
                XAVRequest xavRequest = new XAVRequest();

                UpsAddressValidationWebReference.RequestType request = new UpsAddressValidationWebReference.RequestType();
                String[] requestOption = { "3" };
                request.RequestOption = requestOption;
                xavRequest.Request = request;

                #region -- Access Security (license number, username, password) --
            UpsAddressValidationWebReference.UPSSecurity upss = new UpsAddressValidationWebReference.UPSSecurity();
            UpsAddressValidationWebReference.UPSSecurityServiceAccessToken upsSvcToken = new UpsAddressValidationWebReference.UPSSecurityServiceAccessToken();
            upsSvcToken.AccessLicenseNumber = "CC83ED82D080DC80";
            upss.ServiceAccessToken = upsSvcToken;
            UpsAddressValidationWebReference.UPSSecurityUsernameToken upsSecUsrnameToken = new UpsAddressValidationWebReference.UPSSecurityUsernameToken();
            upsSecUsrnameToken.Username = "WiseWebSupport";
            upsSecUsrnameToken.Password = "wise_forms";
            upss.UsernameToken = upsSecUsrnameToken;
            xavService.UPSSecurityValue = upss;
            #endregion

                AddressKeyFormatType addressKeyFormat = new AddressKeyFormatType();
                String[] addressLine = { txtAddress.Text.Trim() };
                addressKeyFormat.AddressLine = addressLine;
                addressKeyFormat.PoliticalDivision2 = txtCity.Text.Trim();
                if (lstCountry.SelectedValue == "US")
                {
                    addressKeyFormat.PoliticalDivision1 = lstState.SelectedValue;
                }
                else
                {
                    addressKeyFormat.PoliticalDivision1 = txtState.Text.Trim();
                }
                addressKeyFormat.PostcodePrimaryLow = txtZip.Text.Trim();
                addressKeyFormat.CountryCode = lstCountry.SelectedValue;
                xavRequest.AddressKeyFormat = addressKeyFormat;

                XAVResponse xavResponse = xavService.ProcessXAV(xavRequest);
            
                //sbResults.AppendLine("Response Status Code " + xavResponse.Response.ResponseStatus.Code);
                //sbResults.AppendLine("Response Status Description " + xavResponse.Response.ResponseStatus.Description);

            
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
                            displayAddress += candidateAttention + "<br/>";
                        }
                        if (candidateConsignee != "")
                        {
                            displayAddress += candidateConsignee + "<br/>";
                        }
                        displayAddress += candidateAddress + "<br/>";
                        displayAddress += candidateCity + " " + candidateState + " " + candidateZip;

                        tblCandidates.Rows.Add(candidateCount, candidateAttention, candidateConsignee, candidateAddress, candidateCity, candidateState, candidateZip, displayAddress);

                        sbResults.Append("<br/>");
                    } // end of foreach candidate loop
                }


                if (numCandidates == 1)
                {
                    string selectedState = "";
                    if (lstCountry.SelectedValue == "US")
                    {
                        selectedState = lstState.SelectedValue;
                    }
                    else
                    {
                        selectedState = txtState.Text.Trim();
                    }

                    if ((candidateAddress.ToUpper() != txtAddress.Text.Trim().ToUpper())
                     || (candidateCity.ToUpper() != txtCity.Text.Trim().ToUpper())
                     || (candidateState.ToUpper() != selectedState.ToUpper())
                     || (candidateZip.ToUpper() != txtZip.Text.Trim().ToUpper()))
                    {
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

                        mvPageLayout.SetActiveView(vwVerifyAddressCorrection);
                    }
                    else // Our address exactly matches the UPS candidate, so move forward
                    {
                        sbResults.Append(" -- EXACT MATCH -- ");

                    }
                } // end of numCandidates == 1 logic
                else if (numCandidates > 1)
                {

                }


            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
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

            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                sbResults.Append("<br/>");
                sbResults.Append("--------------------<br/>");
                sbResults.Append("CommunicationException= " + ex.Message + "<br/>");
                sbResults.Append("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");
                sbResults.Append("<br/>");

            }
            catch (Exception ex)
            {
                sbResults.Append("<br/>");
                sbResults.Append("-------------------------<br/>");
                sbResults.Append(" General Exception= " + ex.Message + "<br/>");
                sbResults.Append(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
                sbResults.Append("-------------------------<br/>");

            }
            lblResults.Text = sbResults.ToString();
        
        }

        protected void btnRate_Click(object sender, EventArgs e)
        {
        
        }
    
    private void RunTestCode()
    {
        Response.Write("--- beginning of test code --- " + DateTime.Now.ToShortTimeString() + "<br/>");

        try
        {
            Dictionary<string, string> dServices = new Dictionary<string, string>();
            dServices.Add("01", "UPS Next Day Air");
            dServices.Add("02", "UPS Second Day Air");
            dServices.Add("03", "UPS Ground");
            dServices.Add("12", "UPS Three-Day Select");
            dServices.Add("13", "UPS Next Day Air Saver");
            dServices.Add("14", "UPS Next Day Air Early A.M.");
            dServices.Add("59", "UPS Second Day Air A.M.");
            dServices.Add("65", "UPS Saver");

            bool useTestData = true;

            RateService rateService = new RateService();
            RateRequest rateRequest = new RateRequest();

            RequestType request = new RequestType();
            String[] requestOption = { "Shop" };
            request.RequestOption = requestOption;
            rateRequest.Request = request;

            #region -- Access Security (license number, username, password) --
            UPSSecurity upss = new UPSSecurity();
            UPSSecurityServiceAccessToken upsSvcToken = new UPSSecurityServiceAccessToken();
            upsSvcToken.AccessLicenseNumber = "CC83ED82D080DC80";
            upss.ServiceAccessToken = upsSvcToken;
            UPSSecurityUsernameToken upsSecUsrnameToken = new UPSSecurityUsernameToken();
            upsSecUsrnameToken.Username = "WiseWebSupport";
            upsSecUsrnameToken.Password = "wise_forms";
            upss.UsernameToken = upsSecUsrnameToken;
            rateService.UPSSecurityValue = upss;
            #endregion

            #region -- Shipper --
            ShipmentType shipment = new ShipmentType();
            ShipperType shipper = new ShipperType();
            shipper.ShipperNumber = "391287";
            AddressType shipperAddress = new AddressType();
            String[] shipperAddressLine = { "555 McFarland / 400 Dr" };
            shipperAddress.AddressLine = shipperAddressLine;
            shipperAddress.City = "Alpharetta";
            shipperAddress.StateProvinceCode = "GA";
            shipperAddress.PostalCode = "30004";
            shipperAddress.CountryCode = "US";
            shipper.Address = shipperAddress;

            shipment.Shipper = shipper;
            #endregion

            #region -- Ship To --
            ShipToType shipTo = new ShipToType();
            ShipToAddressType shipToAddress = new ShipToAddressType();
            String[] shipToAddressLines = { "" };
            string shipToCity = "";
            string shipToState = "";
            string shipToZip = "";
            string shipToCountry = "";

            if (useTestData)
            {
                shipToAddressLines[0] = "123 Main St";
                shipToCity = "Canton";
                shipToState = "GA";
                shipToZip = "30114";
                shipToCountry = "US";
            }

            shipToAddress.AddressLine = shipToAddressLines;
            shipToAddress.City = shipToCity;
            shipToAddress.StateProvinceCode = shipToState;
            shipToAddress.PostalCode = shipToZip;
            shipToAddress.CountryCode = shipToCountry;

            shipTo.Address = shipToAddress;

            shipment.ShipTo = shipTo;

            #endregion

            #region -- Packages --
            PackageType package = new PackageType();
            PackageWeightType packageWeight = new PackageWeightType();
            CodeDescriptionType uomCodeDesc = new CodeDescriptionType();
            uomCodeDesc.Code = "LBS";
            packageWeight.Weight = "10";
            packageWeight.UnitOfMeasurement = uomCodeDesc;
            package.PackageWeight = packageWeight;

            CodeDescriptionType packageTypeCodeDesc = new CodeDescriptionType();
            packageTypeCodeDesc.Code = "00";
            package.PackagingType = packageTypeCodeDesc;

            PackageServiceOptionsType packageServiceOptions = new PackageServiceOptionsType();
            DeliveryConfirmationType deliveryConfirmation = new DeliveryConfirmationType();
            deliveryConfirmation.DCISType = "2";
            packageServiceOptions.DeliveryConfirmation = deliveryConfirmation;
            package.PackageServiceOptions = packageServiceOptions;
            
            PackageType[] packages = { package };
            shipment.Package = packages;
            #endregion

            ShipmentRatingOptionsType shipmentRatingOptions = new ShipmentRatingOptionsType();
            shipmentRatingOptions.NegotiatedRatesIndicator = "ON";

            shipment.ShipmentRatingOptions = shipmentRatingOptions;
            rateRequest.Shipment = shipment;
            

            RateResponse rateResponse = rateService.ProcessRate(rateRequest);

            RatedShipmentType[] ratedShipments = rateResponse.RatedShipment;

            foreach (RatedShipmentType ratedShipment in ratedShipments)
            {
                Response.Write("Service: " + ratedShipment.Service.Code + " - ");
                if (dServices.ContainsKey(ratedShipment.Service.Code))
                {
                    Response.Write(dServices[ratedShipment.Service.Code] + "<br/>");
                }
                else
                {
                    Response.Write("unknown service<br/>");
                }
                Response.Write("Billing Weight: " + ratedShipment.BillingWeight.Weight + "<br/>");
                //Response.Write("Guaranteed Delivery / Business Days in Transit: " + ratedShipment.GuaranteedDelivery.BusinessDaysInTransit + "<br/>");
                //Response.Write("Guaranteed Delivery / Delivery By Time: " + ratedShipment.GuaranteedDelivery.DeliveryByTime + "<br/>");
                //Response.Write("Negotiated Rate Total Charges: " + ratedShipment.NegotiatedRateCharges.TotalCharge.MonetaryValue + "<br/>");
                RatedShipmentInfoType[] ratedShipmentAlerts = ratedShipment.RatedShipmentAlert;
                foreach(RatedShipmentInfoType ratedShipmentAlert in ratedShipmentAlerts) {
                    Response.Write("Shipment Alert: " + ratedShipmentAlert.Code + " - " + ratedShipmentAlert.Description + "<br/>");
                }
                Response.Write("Service Options Charges: " + ratedShipment.ServiceOptionsCharges.MonetaryValue + "<br/>");
                Response.Write("Transportation Charges: " + ratedShipment.TransportationCharges.MonetaryValue + "<br/>");
                Response.Write("Total charges: " + ratedShipment.TotalCharges.MonetaryValue + "<br/>");
                Response.Write("<br/>");
            }

        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            Response.Write("---------Freight Rate Web Service returns error----------------<br/>");
            Response.Write("---------\"Hard\" is user error \"Transient\" is system error----------------<br/>");
            Response.Write("SoapException Message= " + ex.Message + "<br/>");
            Response.Write("<br/>");
            Response.Write("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText + "<br/>");
            Response.Write("<br/>");
            Response.Write("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml + "<br/>");
            Response.Write("<br/>");
            Response.Write("SoapException StackTrace= " + ex.StackTrace + "<br/>");
            Response.Write("-------------------------<br/>");

        }
        catch (System.ServiceModel.CommunicationException ex)
        {
            Response.Write("<br/>");
            Response.Write("--------------------<br/>");
            Response.Write("CommunicationException= " + ex.Message + "<br/>");
            Response.Write("CommunicationException-StackTrace= " + ex.StackTrace + "<br/>");
            Response.Write("-------------------------<br/>");
            Response.Write("<br/>");

        }
        catch (Exception ex)
        {
            Response.Write("<br/>");
            Response.Write("-------------------------<br/>");
            Response.Write(" General Exception= " + ex.Message + "<br/>");
            Response.Write(" General Exception-StackTrace= " + ex.StackTrace + "<br/>");
            Response.Write("-------------------------<br/>");

        }

        Response.Write("<br/>--- end of test code --- " + DateTime.Now.ToShortTimeString() + "<br/>");
        
    }
    */
}
