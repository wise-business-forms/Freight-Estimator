using FreightRateWebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SandboxTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        testGroundFreight();
    }

    private void testGroundFreight()
    {
        Response.Write("In testGroundFreight<br/>");

        try
        {
            FreightRateService freightRateService = new FreightRateService();
            FreightRateRequest freightRateRequest = new FreightRateRequest();
            RequestType request = new RequestType();
            String[] requestOption = { "1" }; // 1 = Ground
            request.RequestOption = requestOption;
            freightRateRequest.Request = request;

            /** ****************ShipFrom******************************* */
            ShipFromType shipFrom = new ShipFromType();
            AddressType shipFromAddress = new AddressType();
            String[] shipFromAddressLines = { "555 McFarland 400 Drive" };
            shipFromAddress.AddressLine = shipFromAddressLines;
            shipFromAddress.City = "Alpharetta";
            shipFromAddress.StateProvinceCode = "GA";
            shipFromAddress.PostalCode = "30004";
            shipFromAddress.CountryCode = "US";
            shipFrom.Address = shipFromAddress;
            shipFrom.AttentionName = "";
            shipFrom.Name = "Wise";
            freightRateRequest.ShipFrom = shipFrom;
            /** ****************ShipFrom******************************* */

            /** ****************ShipTo*************************************** */
            ShipToType shipTo = new ShipToType();
            AddressType shipToAddress = new AddressType();
            String[] shipToAddressLines = { "502 Sapphire Valley Ln" };
            shipToAddress.AddressLine = shipToAddressLines;
            shipToAddress.City = "Canton";
            shipToAddress.StateProvinceCode = "GA";
            shipToAddress.PostalCode = "30114";
            shipToAddress.CountryCode = "US";
            shipTo.Address = shipToAddress;
            shipTo.AttentionName = "";
            shipTo.Name = "";
            freightRateRequest.ShipTo = shipTo;
            /** ****************ShipTo*************************************** */

            /** ***************PaymentInformationType************************* */
            PaymentInformationType paymentInfo = new PaymentInformationType();
            PayerType payer = new PayerType();
            payer.AttentionName = "";
            payer.Name = "Wise";
            payer.ShipperNumber = "391287";
            AddressType payerAddress = new AddressType();
            String[] payerAddressLines = { "555 McFarland 400 Drive" };
            payerAddress.AddressLine = payerAddressLines;
            payerAddress.City = "Alpharetta";
            payerAddress.StateProvinceCode = "GA";
            payerAddress.PostalCode = "30114";
            payerAddress.CountryCode = "US";
            payer.Address = payerAddress;
            paymentInfo.Payer = payer;
            RateCodeDescriptionType shipBillOption = new RateCodeDescriptionType();
            shipBillOption.Code = "10";
            shipBillOption.Description = "Prepaid";
            paymentInfo.ShipmentBillingOption = shipBillOption;
            freightRateRequest.PaymentInformation = paymentInfo;
            /** ***************PaymentInformationType************************* */

            //Below code use dummy data for referenced. Please update as required


            /** ***************Service************************************** */
            RateCodeDescriptionType service = new RateCodeDescriptionType();
            service.Code = "309";
            service.Description = "UPS Ground Freight";
            freightRateRequest.Service = service;
            /** ***************Service************************************** */


            /** **************Commodity************************************* */
            CommodityType commodity = new CommodityType();
            CommodityValueType commValue = new CommodityValueType();
            commValue.CurrencyCode = "USD";
            commValue.MonetaryValue = "5670";
            commodity.CommodityValue = commValue;
            commodity.NumberOfPieces = "20";

            RateCodeDescriptionType packagingType = new RateCodeDescriptionType();
            packagingType.Code = "BAG";
            packagingType.Description = "BAG";
            commodity.PackagingType = packagingType;
            WeightType weight = new WeightType();
            UnitOfMeasurementType unitOfMeasurement = new UnitOfMeasurementType();
            unitOfMeasurement.Code = "LBS";
            unitOfMeasurement.Description = "Pounds";
            weight.UnitOfMeasurement = unitOfMeasurement;
            weight.Value = "200";
            commodity.Weight = weight;
            commodity.Description = "LCD TVS";

            CommodityValueType commodityValue = new CommodityValueType();
            commodityValue.CurrencyCode = "USD";
            commodityValue.MonetaryValue = "100";
            commodity.CommodityValue = commodityValue;
            commodity.Description = "LCD TVS";
            commodity.FreightClass = "60";
            CommodityType[] commodityArray = { commodity };
            freightRateRequest.Commodity = commodityArray;
            /** **************Commodity************************************* */


            /** **************HandlingUnitOne************************************* */
            HandlingUnitType handUnitType = new HandlingUnitType();
            handUnitType.Quantity = "1";
            RateCodeDescriptionType rateCodeDescType = new RateCodeDescriptionType();
            rateCodeDescType.Code = "SKD";
            rateCodeDescType.Description = "SKID";
            handUnitType.Type = rateCodeDescType;
            freightRateRequest.HandlingUnitOne = handUnitType;

            /** **************HandlingUnitOne************************************* */


            UPSSecurity upss = new UPSSecurity();
            UPSSecurityServiceAccessToken upsSvcToken = new UPSSecurityServiceAccessToken();
            upsSvcToken.AccessLicenseNumber = "CC83ED82D080DC80";
            upss.ServiceAccessToken = upsSvcToken;
            UPSSecurityUsernameToken upsSecUsrnameToken = new UPSSecurityUsernameToken();
            upsSecUsrnameToken.Username = "WiseWebSupport";
            upsSecUsrnameToken.Password = "Wise_forms";
            upss.UsernameToken = upsSecUsrnameToken;
            freightRateService.UPSSecurityValue = upss;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11; //This line will ensure the latest security protocol for consuming the web service call.
            Response.Write(freightRateRequest);
            FreightRateResponse freightRateResponse = freightRateService.ProcessFreightRate(freightRateRequest);

            string requestXml = SoapTrace.TraceExtension.XmlRequest.OuterXml.ToString();
            string responseXml = SoapTrace.TraceExtension.XmlResponse.OuterXml.ToString();

            Response.Write("Request: " + requestXml);
            Response.Write("Response: " + responseXml);

            Response.Write("Response code: " + freightRateResponse.Response.ResponseStatus.Code);
            Response.Write("Response description: " + freightRateResponse.Response.ResponseStatus.Description);
            Response.Write("Debug: " + freightRateResponse.RatingSchedule.Description);
            Response.Write("Debug: " + freightRateResponse.TotalShipmentCharge.MonetaryValue);
            //Console.ReadKey();
        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            Response.Write("");
            Response.Write("---------Freight Rate Web Service returns error----------------");
            Response.Write("---------\"Hard\" is user error \"Transient\" is system error----------------");
            Response.Write("SoapException Message= " + ex.Message);
            Response.Write("");
            Response.Write("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText);
            Response.Write("");
            Response.Write("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml);
            Response.Write("");
            Response.Write("SoapException StackTrace= " + ex.StackTrace);
            Response.Write("-------------------------");
            Response.Write("");
        }
        catch (System.ServiceModel.CommunicationException ex)
        {
            Response.Write("");
            Response.Write("--------------------");
            Response.Write("CommunicationException= " + ex.Message);
            Response.Write("CommunicationException-StackTrace= " + ex.StackTrace);
            Response.Write("-------------------------");
            Response.Write("");

        }
        catch (Exception ex)
        {
            Response.Write("");
            Response.Write("-------------------------");
            Response.Write(" General Exception= " + ex.Message);
            Response.Write(" General Exception-StackTrace= " + ex.StackTrace);
            Response.Write("-------------------------");

        }
        finally
        {
            Response.Write("in finally section");
        }
    }
}