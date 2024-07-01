<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Help" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Wise Freight Estimator - Help</title>
    <link href="style/master.css" rel="stylesheet" type="text/css" media="all" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
           
        <div class="HelpSection"><a name="request"></a>Submitting Your Request</div>

        <div class="HelpTopic">Ship From Plant</div>
        <div class="HelpDetail">Your default Ship From Plant is loaded from your profile automatically.  You can use the <b>[change plant]</b> link to change the Ship From Plant for the duration of your session.  When you start a new session, the Ship From Plant will automatically reset to your default value.</div>

        <div class="HelpTopic">Required Fields</div>
        <div class="HelpDetail"><b>Zip Code</b>, <b># of Packages</b>, and <b>Package Weight</b> fields are required to retrieve Rates.</div>

        <div class="HelpTopic">Using Only a Zip Code</div>
        <div class="HelpDetail">If <b>Address</b>, <b>City</b>, and <b>State</b> are not provided, the rates provided are an estimate based on a Commercial Address with no extended area service charges.  Actual rates may be higher depending on the full Ship To Address.</div>
        <div class="HelpDetail">If your shipment qualifies as HundredWeight, the <b>State</b> is also required.  You will be prompted to supply the State if necessary.</div>

        <div class="HelpTopic">Package Details</div>
        <div class="HelpDetail">Enter the number of Packages in the <b># Packages</b> field, and the weight of each package in the <b>Package Weight</b> field.  If the last package has a different weight, enter it in the <b>Last Package Weight</b> field.</div>
        <div class="HelpDetail">For example, if you were shipping 9 packages, and the first eight each weighed 12 lbs and the final package weighed 6 lbs, you would enter 9 as the <b># of Packages</b>, 12 as the <b>Package Weight</b>, and 6 as the <b>Last Package Weight</b>, which would give you a billable weight of 102 lbs for the shipment.</div>

        <div class="HelpTopic">Delivery Confirmation / Signature Required Option</div>
        <div class="HelpDetail">You may select a Delivery Confirmation or Signature Required option.  Any related surcharges will automatically be included in the Rate.</div>

        <div class="HelpTopic">Rating from Multiple Locations</div>
        <div class="HelpDetail">If you set <b>Rate from Multiple Locations</b> to "No," the Rates will be based on shipping from your current Ship From Plant.</div>
        <div class="HelpDetail">If you set this option to "Yes," you will see the rates to ship from each of the five plants.  This type of request will take approximately five times longer to process than when setting this option to "No."</div>

        <div class="HelpTopic">No Address Validation for Puerto Rico</div>
        <div class="HelpDetail">Addresses in Puerto Rico are not validated before being submitted for Rating.</div>

        <div class="HelpBackLink"><a href="javascript:history.go(-1);" class="links">return to previous page</a></div>
        <br /><br />

        <div class="HelpSection"><a name="address"></a>Selecting an Alternative Address</div>

        <div class="HelpTopic">Viewing Alternative Addresses</div>
        <div class="HelpDetail">If the entered address is not successfully validated, UPS may return multiple alternatives from which you can choose.  Please review the returned addresses to see if one of them is the correct address, and if so, click the <b>Select this Address</b> link.</div>
        <div class="HelpDetail">If you notice a typo in the address you specified and would like to edit it, click the <b>Edit Address</b> button.</div>

        <div class="HelpTopic">No Suitable Alternatives</div>
        <div class="HelpDetail">If none of the alternatives returned for the entered address are correct, you may need to reverify the address information with the customer.</div>

        <div class="HelpBackLink"><a href="javascript:history.go(-1);" class="links">return to previous page</a></div>
        <br /><br />

        <div class="HelpSection"><a name="rates"></a>Viewing Rates</div>

        <div class="HelpTopic">Alerts and Warnings</div>
        <div class="HelpDetail">You may see a red alert or warning displayed below your address when viewing the returned rates.  Please make note of this as it could impact the cost of the shipment.</div>

        <div class="HelpTopic">Rates from a Single Ship From Plant</div>
        <div class="HelpDetail">If you specified "No" for the <b>Rate from Multiple Locations</b> option, you will see a list of the available services and rates from your Ship From plant.  The <b>Rate</b> column is the final cost of the shipment, including any applicable Wise surcharges or markup.</div>

        <div class="HelpTopic">Rates for All Plants</div>
        <div class="HelpDetail">If you specified "Yes" for the <b>Rate from Multiple Locations</b> option, you will see a list of the available services and rates for each of our plants as the Ship From plant.  The rate shown for each service and plant is the final cost of the shipment, including any applicable Wise surcharges or markup.</div>

        <div class="HelpTopic">HundredWeight Indicator</div>
        <div class="HelpDetail">The <b>CWT?</b> column in the returned rate tables indicates if a shipment qualifies as HundredWeight for a given shipment method.</div>

        <div class="HelpTopic">Going Back or Starting Over</div>
        <div class="HelpDetail">Click the <b>Back</b> button to edit your existing request.</div>
        <div class="HelpDetail">Click the <b>Start Over</b> button to start from scratch with a blank request.</div>

        <div class="HelpBackLink"><a href="javascript:history.go(-1);" class="links">return to previous page</a></div>
        <br /><br />

        <div class="HelpSection"><a name="plant"></a>Selecting a "Ship From" Plant</div>

        <div class="HelpTopic">Recognizing Who You Are and Your Default Plant</div>
        <div class="HelpDetail">The Freight Estimator recognizes who you are based on the Username you used when logging onto your computer.</div>
        <div class="HelpDetail">The first time you access the Freight Estimator, you are prompted to choose a default Ship From plant.</div>
        <div class="HelpDetail">On subsequent visits to the Freight Estimator, your default Ship From plant will automatically be loaded for you.  By using the <b>[change plant]</b> link on the Request screen, you can temporarily change your Ship From plant for the duration of your session.</div>

        <div class="HelpBackLink"><a href="javascript:history.go(-1);" class="links">return to previous page</a></div>
        <br /><br />

        <div class="HelpSection">UPS HundredWeight (CWT) Rates</div>

        <div class="HelpTopic">Accessing Rates for CWT Shipments</div>
        <div class="HelpDetail">Any shipments of 2 or more packages that weigh a minimum combined 100 lbs for Air or 200 lbs for Ground are automatically considered HundredWeight shipments.</div>
        <div class="HelpDetail">The plant-level charges and markups may differ between HundredWeight and non-HundredWeight shipments.</div>
        <div class="HelpDetail">The <b>CWT?</b> column in the returned rate tables indicates if a shipment qualifies as HundredWeight for a given shipment method.</div>

        <div class="HelpBackLink"><a href="javascript:history.go(-1);" class="links">return to previous page</a></div>
        <br /><br />

        <div class="HelpSection">Wise Plant Charges and Markups</div>
        
        <div class="HelpTopic">Service Level Markups</div>
        <div class="HelpDetail">The administrators at each plant maintain markups for each service level (Ground, 2nd Day, etc) that are applied to the total UPS cost prior to showing you the final Rate.</div>

        <div class="HelpTopic">Per Package and Per Shipment Charges</div>
        <div class="HelpDetail">The administrators at each plant maintain Per Package and Per Shipment charges that are added onto the total UPS cost (including service level markups) prior to showing you the final Rate.</div>

        <div class="HelpTopic">Calculation of Markup and Charges</div>
        <div class="HelpDetail">Any service level markups are applied prior to applying Per Package and Per Shipment charges.  For example, if the full UPS charge is $50, and the markup is 2%, and there are a total of $4 in Per Package and Per Shipment charges, the final displayed rate would be $55 (that is, (50 * 1.02) + 4).</div>
        
        <div class="HelpTopic">Maintaining Charges and Markups Per Plant</div>
        <div class="HelpDetail">Administrators can maintain the plant charges and service level markups via the <a href="PlantAdmin.aspx" class="links">Plant Administration</a> page.</div>
        <div class="HelpDetail">If you are an administrator who requires access to maintain your plant, please email helpdesk@wbf.com.</div>
        
        <div class="HelpBackLink"><a href="javascript:history.go(-1);" class="links">return to previous page</a></div>
        <br /><br />

        <div class="HelpSection">Additional Questions / Issues</div>
        
        <div class="HelpDetail">If you have additional questions or problems not addressed here, please email helpdesk@wbf.com with a full description.  Thank you.</div>

        <div class="HelpBackLink"><a href="javascript:history.go(-1);" class="links">return to previous page</a></div>
        


    </div>
    </form>
</body>
</html>
