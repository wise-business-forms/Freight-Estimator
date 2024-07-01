<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PlantAdminCWT.aspx.cs" Inherits="PlantAdminCWT" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
                                <br />

								<asp:MultiView ID="mvMain" runat="server" ActiveViewIndex="1">
									<asp:View ID="vwAllowed" runat="server">
										<h2>Current <u>UPS HundredWeight (CWT)</u> Charges per Plant</h2><br />
										<asp:GridView ID="gvPlantCharges" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="false"
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
												<%--<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />--%>
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
									</asp:View>
									<asp:View ID="vwNotAllowed" runat="server">
										I'm sorry - your username is not on the list of allowed admin users for the Freight Estimator.  Please contact helpdesk@wbf.com to be added to the list.
									</asp:View>
								</asp:MultiView>
							</td>
						</tr>
					</table>
				
					<asp:Panel ID="pnlEditALP" runat="server" Visible="false">			
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h2>Edit ALP Plant Charges (HundredWeight)</h2><br />
									<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
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
												<%--<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />--%>
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
									</td>
								</tr>
							</table>
					</asp:Panel>
					
					
					<asp:Panel ID="pnlEditBUT" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h2>Edit BUT Plant Charges (HundredWeight)</h2><br />
									<asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
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
												<%--<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />--%>
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
									</asp:GridView>
								</td>
							</tr>
						</table>
					</asp:Panel>
					
					
					<asp:Panel ID="pnlEditFTW" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h2>Edit FTW Plant Charges (HundredWeight)</h2><br />
									<asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
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
												<%--<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />--%>
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
									</td>
								</tr>
							</table>
					</asp:Panel>
					
					
					<asp:Panel ID="pnlEditPDT" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h2>Edit PDT Plant Charges (HundredWeight)</h2><br />
									<asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
													  DataSourceID="dsPlantChargesCWTPDT"
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
												<%--<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />--%>
												<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
											</Columns>
										</asp:GridView>
									</td>
								</tr>
							</table>
					</asp:Panel>

					<asp:Panel ID="pnlEditPOR" runat="server" Visible="false">
						<table cellpadding="0" cellspacing="0" border="0" width="100%">
							<tr>
								<td>
									<br />
									<h2>Edit POR Plant Charges (HundredWeight)</h2><br />
									<asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="false" AutoGenerateEditButton="true"
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
										<%--<asp:BoundField DataField="NextDayAirEarlyAM" HeaderText="Next Day Early AM" DataFormatString="{0:F2}%" />--%>
										<asp:BoundField DataField="Saver" HeaderText="Saver" DataFormatString="{0:F2}%" />
									</Columns>
									</asp:GridView>
								</td>
							</tr>
						</table>
					</asp:Panel>
				</td>
			</tr>
			<tr>
				<td><br /><a href="ValidateAndRate.aspx">Back to Freight Estimator</a></td>
			</tr>
		</table>

        <asp:SqlDataSource ID="dsPlantChargesCWT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges WHERE CarrierId = 'CWT'"
                            UpdateCommand="UPDATE PlantCarrierCharges SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTALP" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges WHERE CarrierId = 'CWT' AND PlantCode = 'ALP'"
                            UpdateCommand="UPDATE PlantCarrierCharges SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTBUT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges WHERE CarrierId = 'CWT' AND PlantCode = 'BUT'"
                            UpdateCommand="UPDATE PlantCarrierCharges SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTFTW" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges WHERE CarrierId = 'CWT' AND PlantCode = 'FTW'"
                            UpdateCommand="UPDATE PlantCarrierCharges SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTPDT" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges WHERE CarrierId = 'CWT' AND PlantCode = 'PDT'"
                            UpdateCommand="UPDATE PlantCarrierCharges SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="dsPlantChargesCWTPOR" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection%>"
                            SelectCommand="SELECT * FROM PlantCarrierCharges WHERE CarrierId = 'CWT' AND PlantCode = 'POR'"
                            UpdateCommand="UPDATE PlantCarrierCharges SET PerPackageCharge=@PerPackageCharge,PerShipmentCharge=@PerShipmentCharge,NextDayAir=@NextDayAir,SecondDayAir=@SecondDayAir,Ground=@Ground,ThreeDaySelect=@ThreeDaySelect,NextDayAirSaver=@NextDayAirSaver,SecondDayAirAM=@SecondDayAirAM,Saver=@Saver WHERE CarrierPlantChargesId=@CarrierPlantChargesId">
        </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>