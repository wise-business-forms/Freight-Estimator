<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PlantAdmin.aspx.cs" Inherits="PlantAdmin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	
	<!-- CSS LINKS -->
	<link href="style/master.css" rel="stylesheet" type="text/css" media="all" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<table cellpadding="0" cellspacing="0" border="0" class="containerTable">
            <tr>
				<td>
					<table cellpadding="0" cellspacing="0" border="0" width="100%">
						<tr>
							<td>
                                <div class="HelpTopic">Per Package and Per Shipment Charges</div>
                                <div class="HelpDetail">The administrators at each plant maintain Per Package and Per Shipment charges that are added onto the total UPS cost (including service level markups) prior to showing you the final Rate.</div>

                                <div class="HelpTopic">Calculation of Markup and Charges</div>
                                <div class="HelpDetail">Any service level markups are applied prior to applying Per Package and Per Shipment charges.  For example, if the full UPS charge is $50, and the markup is 2%, and there are a total of $4 in Per Package and Per Shipment charges, the final displayed rate would be $55 (that is, (50 * 1.02) + 4).</div>

                                <div class="HelpTopic">Special Rates for Piedmont Customers in ALP</div>
                                <div class="HelpDetail">Any shipment rate requests originating in ALP now require an account number.  That account number is used to determine if the customer is a former Piedmont customer, and if so, uses different rating criteria.  The Plant abbreviation in the tables below for these special rates is ALZ.</div>
                                <br />

								<asp:MultiView ID="mvMain" runat="server" ActiveViewIndex="1">
									<asp:View ID="vwAllowed" runat="server">
										<h2>Current UPS Standard (Non-CWT) Charges per Plant</h2><br />
										<asp:GridView ID="gvPlantCharges" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="false"
													  DataSourceID="dsPlantChargesUPS"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>

                                        <br /><br />

                                        <h2>Current UPS HundredWeight (CWT) Charges per Plant</h2><br />
										<asp:GridView ID="GridView6" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="false"
													  DataSourceID="dsPlantChargesCWT"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>

                                        <br /><br />

                                        <h2>Current LTL Charges per Plant</h2><br />
										<asp:GridView ID="GridView12" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="false"
													  DataSourceID="dsPlantChargesM33"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>
                                        
                                        <br /><br />

                                        <h2>Current UPS Ground Freight Charges per Plant</h2><br />
										<asp:GridView ID="GridView21" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="false"
													  DataSourceID="dsPlantChargesGF"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>
									</asp:View>
									<asp:View ID="vwNotAllowed" runat="server">
										I'm sorry - your username is not recognized as one with Admin access to these rates.  Please <a href="Login.aspx">Login</a> again.
									</asp:View>
								</asp:MultiView>
							</td>
						</tr>
					</table>

                    <br />
                    <br />
                    <hr />
				
					<asp:Panel ID="pnlEditALP" runat="server" Visible="false">			
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h3>Edit ALP Plant UPS Standard (Non-CWT) Charges</h3><br />
									<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesUPSALP"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
                                        
                                    <h3>Edit ALP Plant UPS HundredWeight (CWT) Charges</h3><br />
									<asp:GridView ID="GridView13" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesCWTALP"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
                                        

                                    <h3>Edit ALP Plant LTL Charges</h3><br />
									<asp:GridView ID="GridView7" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesM33ALP"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>

                                    
                                    <h3>Edit ALP Plant UPS Ground Freight Charges</h3><br />
									<asp:GridView ID="GridView22" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesGFALP"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>
								</td>
							</tr>
						</table>
                        <hr />
					</asp:Panel>

                    
					<asp:Panel ID="pnlEditALZ" runat="server" Visible="false">			
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h3>Edit ALP (PDT Accounts) Plant UPS Standard (Non-CWT) Charges</h3><br />
									<asp:GridView ID="GridView18" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesUPSALZ"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
                                        
                                    <h3>Edit ALP (PDT Accounts) Plant UPS HundredWeight (CWT) Charges</h3><br />
									<asp:GridView ID="GridView19" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesCWTALZ"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
                                        

                                    <h3>Edit ALP (PDT Accounts) Plant LTL Charges</h3><br />
									<asp:GridView ID="GridView20" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesM33ALZ"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>
                                    
                                    <h3>Edit ALP (PDT Accounts) Plant UPS Ground Freight Charges</h3><br />
									<asp:GridView ID="GridView23" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesGFALZ"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>
								</td>
							</tr>
						</table>
                        <hr />
					</asp:Panel>
					
					
					<asp:Panel ID="pnlEditBUT" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h3>Edit BUT Plant UPS Standard (Non-CWT) Charges</h3><br />
									<asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesUPSBUT"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
									</asp:GridView>

                                    <h3>Edit BUT Plant UPS HundredWeight (CWT) Charges</h3><br />
									<asp:GridView ID="GridView8" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesCWTBUT"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
									</asp:GridView>

                                    

                                    <h3>Edit BUT Plant LTL Charges</h3><br />
									<asp:GridView ID="GridView14" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesM33BUT"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>

                                    

                                    <h3>Edit BUT Plant UPS Ground Freight Charges</h3><br />
									<asp:GridView ID="GridView24" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesGFBUT"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>
								</td>
							</tr>
						</table>
                        <hr />
					</asp:Panel>
					
					
					<asp:Panel ID="pnlEditFTW" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h3>Edit FTW Plant UPS Standard (Non-CWT) Charges</h3><br />
									<asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesUPSFTW"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>

                                    <h3>Edit FTW Plant UPS HundredWeight (CWT) Charges</h3><br />
									<asp:GridView ID="GridView9" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesCWTFTW"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>

                                        

                                    <h3>Edit FTW Plant LTL Charges</h3><br />
									<asp:GridView ID="GridView15" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesM33FTW"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>

                                    <h3>Edit FTW Plant UPS Ground Freight Charges</h3><br />
									<asp:GridView ID="GridView25" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesGFFTW"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>

									</td>
								</tr>
							</table>
                            <hr />
					</asp:Panel>
					
					<asp:Panel ID="pnlEditPOR" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h3>Edit POR Plant UPS Standard (Non-CWT) Charges</h3><br />
									<asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesUPSPOR"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
									<Columns>
										<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
										<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
										<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
										<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
										<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
									</Columns>
									</asp:GridView>

                                    <h3>Edit POR Plant UPS HundredWeight (CWT) Charges</h3><br />
									<asp:GridView ID="GridView11" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesCWTPOR"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
									<Columns>
										<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
										<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
										<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
										<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
										<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
										<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
									</Columns>
									</asp:GridView>

                                    

                                    <h3>Edit POR Plant LTL Charges</h3><br />
									<asp:GridView ID="GridView17" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesM33POR"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>

								</td>
							</tr>
						</table>
                        <hr />
					</asp:Panel>

                    
					
					<asp:Panel ID="pnlEditBMK" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h3>Edit BMK Plant UPS Standard (Non-CWT) Charges</h3><br />
									<asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesUPSBMK"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
                                    <h3>Edit BMK Plant UPS HundredWeight (CWT) Charges</h3><br />
									<asp:GridView ID="GridView10" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesCWTBMK"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Ground" DataFormatString="{0:F2}%"/>
												<asp:BoundField DataField="ThreeDaySelect" HeaderText="3 Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAir" HeaderText="2nd Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="SecondDayAirAM" HeaderText="2nd Day Air AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirSaver" HeaderText="Next Day Saver" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAir" HeaderText="Next Day" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
                                        

                                    <h3>Edit BMK Plant LTL Charges</h3><br />
									<asp:GridView ID="GridView16" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesM33BMK"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>
                                    
                                    <h3>Edit BMK Plant UPS Ground Freight Charges</h3><br />
									<asp:GridView ID="GridView26" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesGFBMK"
													  GridLines="None"   
													  CssClass="shipGrid"
													  AlternatingRowStyle-CssClass="alt"
													  DataKeyNames="CarrierPlantChargesId">
											<Columns>
												<asp:BoundField DataField="PlantCode" HeaderText="Plant" ReadOnly="true" />
												<asp:BoundField DataField="PerPackageCharge" HeaderText="Per Pkg" DataFormatString="{0:c}" />
												<asp:BoundField DataField="PerShipmentCharge" HeaderText="Per Shpmnt" DataFormatString="{0:c}" />
												<asp:BoundField DataField="Ground" HeaderText="Markup" DataFormatString="{0:F2}%"/>
											</Columns>
										</asp:GridView>

									</td>
								</tr>
							</table>
                            <hr />
					</asp:Panel>

				</td>
			</tr>
			<tr>
				<td><br /><a href="ValidateAndRate.aspx">Back to Freight Estimator</a></td>
			</tr>
		</table>

        <asp:SqlDataSource ID="dsPlantChargesUPS" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPS'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesUPSALP" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPS' AND PlantCode = 'ALP'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>
        
        <asp:SqlDataSource ID="dsPlantChargesUPSALZ" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPS' AND PlantCode = 'ALZ'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesUPSBUT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPS' AND PlantCode = 'BUT'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesUPSFTW" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPS' AND PlantCode = 'FTW'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesUPSBMK" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPS' AND PlantCode = 'BMK'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesUPSPOR" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPS' AND PlantCode = 'POR'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>




        <asp:SqlDataSource ID="dsPlantChargesCWT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPSCWT'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTALP" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPSCWT' AND PlantCode = 'ALP'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>
        
        <asp:SqlDataSource ID="dsPlantChargesCWTALZ" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPSCWT' AND PlantCode = 'ALZ'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTBUT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPSCWT' AND PlantCode = 'BUT'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTFTW" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPSCWT' AND PlantCode = 'FTW'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTBMK" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPSCWT' AND PlantCode = 'BMK'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTPOR" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'UPSCWT' AND PlantCode = 'POR'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,NextDayAirEarlyAM=@NextDayAirEarlyAM,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>



        
        <asp:SqlDataSource ID="dsPlantChargesM33" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'M33'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesM33ALP" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'M33' AND PlantCode = 'ALP'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>
                
        <asp:SqlDataSource ID="dsPlantChargesM33ALZ" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'M33' AND PlantCode = 'ALZ'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesM33BUT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'M33' AND PlantCode = 'BUT'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesM33FTW" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'M33' AND PlantCode = 'FTW'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesM33BMK" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'M33' AND PlantCode = 'BMK'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesM33POR" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'M33' AND PlantCode = 'POR'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>



        
        <asp:SqlDataSource ID="dsPlantChargesGF" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'GF'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesGFALP" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'GF' AND PlantCode = 'ALP'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>
                
        <asp:SqlDataSource ID="dsPlantChargesGFALZ" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'GF' AND PlantCode = 'ALZ'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesGFBUT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'GF' AND PlantCode = 'BUT'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesGFFTW" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'GF' AND PlantCode = 'FTW'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesGFBMK" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'GF' AND PlantCode = 'BMK'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesGFPOR" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges2017 WHERE CarrierId = 'GF' AND PlantCode = 'POR'"
                            UpdateCommand="UPDATE PlantCarrierCharges2017 SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,Ground=@Ground WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
