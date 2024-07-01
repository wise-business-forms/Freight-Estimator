<%@ Page Language="C#" AutoEventWireup="true" CodeFile="M33.aspx.cs" Inherits="M33" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
	    <title id="PageTitle" runat="server">Wise Freight Estimator</title>
	    
	    <!-- CSS LINKS -->
	    <link href="style/master.css" rel="stylesheet" type="text/css" media="all" />
        <script type="text/javascript">
            function validateStateForCWT(oSrc, args) {
                var CWTAirMinPkgs = <%= Config.MinCWTPackagesAir %>;
                var CWTAirMinWeight = <%= Config.MinCWTWeightAir %>;
                var CWTGroundMinPkgs = <%= Config.MinCWTPackagesGround %>;
                var CWTGroundMinWeight = <%= Config.MinCWTWeightGround %>;

                var StateIsValid = false;
                var IsHundredWeight = false;
                var NumPackages = document.getElementById('txtNumPackages').value;
                var PackageWeight = document.getElementById('txtPackageWeight').value;
                var LastPackageWeight = document.getElementById('txtLastPackageWeight').value;

                if ((NumPackages != '') && (PackageWeight != '')) {
                    try {
                        var TotalWeight = 0;
                        if (LastPackageWeight != '') {
                            TotalWeight = ((NumPackages - 1) * PackageWeight) + LastPackageWeight;
                        } else {
                            TotalWeight = NumPackages * PackageWeight;
                        }

                        if (((NumPackages >= CWTAirMinPkgs) && (TotalWeight >= CWTAirMinWeight)) || ((NumPackages >= CWTAirMinPkgs) && (TotalWeight >= CWTAirMinWeight))) {
                            IsHundredWeight = true;
                        } else {
                            IsHundredWeight = false;
                        }
                    } catch (e) {
                        IsHundredWeight = false;
                    }
                } else {
                    IsHundredWeight = false;
                }

                if (IsHundredWeight) {
                    StateIsValid = (args.Value.length >= 2);
                } else {
                    StateIsValid = true;
                }
                
                args.IsValid = StateIsValid;
            }
        </script>
	</head>
<body>
    <form id="form1" runat="server">
    <div class="containerMain">
        <asp:MultiView ID="mvPageLayout" runat="server" ActiveViewIndex="0">
        	<!-- SHIPPING INFO -->
            <asp:View ID="vwMain" runat="server">
                <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                	<tr>
                		<td>
			                <table cellpadding="5" cellspacing="0" border="0" class="internalTable" width="100%">
								<tr>
			                		<td colspan="2" align="right"><a href="Help.aspx#request" class="links">? Help</a></td>
			                	</tr>
			                	<tr>
			                		<td colspan="2" align="center"><img src="images/wiseLogo.jpg" alt="Wise" /></td>
			                	</tr>
			                    <tr>
			                        <td class="label">Shipping From:</td>
			                        <td><asp:Literal ID="litShipFromPlant" runat="server" />&nbsp;<asp:LinkButton ID="btnChangePlant" runat="server" Text="[change plant]" OnClick="btnChangePlant_Click" CausesValidation="false" /></td>
			                    </tr>
			                    <tr>
			                        <td colspan="2" class="shipto">- Ship To -</td>
			                    </tr>
			                    <tr>
			                        <td class="label">Address:</td>
			                        <td><asp:TextBox runat="server" ID="txtAddress" class="input" border="0" /></td>
			                    </tr>
			                    <tr>
			                        <td class="label">City:</td>
			                        <td><asp:TextBox runat="server" ID="txtCity" class="input" border="0" /></td>
			                    </tr>
			                    <tr>
			                        <td class="label">State:</td>
			                        <td>
			                            <asp:TextBox runat="server" ID="txtState" class="input" border="0" />
                                        <asp:CustomValidator ID="valStateCWT" runat="server" ControlToValidate="txtState" Display="Dynamic" ClientValidationFunction="validateStateForCWT" ValidateEmptyText="true">< required for Hundred-Weight</asp:CustomValidator>
			                        </td>
			                    </tr>
			                    <tr>
			                        <td class="label">Zip:</td>
			                        <td>
                                        <asp:TextBox runat="server" ID="txtZip" class="input" border="0" />
                                        <asp:RequiredFieldValidator ID="valReqZip" runat="server" ControlToValidate="txtZip" Display="Dynamic">< required</asp:RequiredFieldValidator>
                                    </td>
			                    </tr>
			                    <tr>
			                        <td class="label">Country:</td>
			                        <td>
			                            <asp:ListBox runat="server" ID="lstCountry" Rows="1" class="input" border="0">
			                                <asp:ListItem Text="United States" Value="US" Selected="True" />
			                                <asp:ListItem Text="Puerto Rico" Value="PR" />
			                            </asp:ListBox>
			                        </td>
			                    </tr>
			                    <tr>
			                        <td class="label"># Packages</td>
			                        <td>
			                            <asp:TextBox runat="server" ID="txtNumPackages" class="input" border="0" />
			                            <asp:RequiredFieldValidator ID="valReqNumPackages" runat="server" ControlToValidate="txtNumPackages" Display="Dynamic">< required</asp:RequiredFieldValidator>
			                            <asp:RegularExpressionValidator ID="valRegNumPackages" runat="server" ControlToValidate="txtNumPackages" ValidationExpression="^\d+$" Display="Dynamic">< invalid value</asp:RegularExpressionValidator>
			                        </td>
			                    </tr>
			                    <tr>
			                        <td class="label">Package Weight (each)</td>
			                        <td>
			                            <asp:TextBox runat="server" ID="txtPackageWeight" class="input" border="0" />
			                            <asp:RequiredFieldValidator ID="valReqPackageWeight" runat="server" ControlToValidate="txtPackageWeight" Display="Dynamic">< required</asp:RequiredFieldValidator>
			                            <asp:RegularExpressionValidator ID="valRegPackageWeight" runat="server" ControlToValidate="txtPackageWeight" ValidationExpression="^\d+(\.)?(\d+)?$" Display="Dynamic">< invalid value</asp:RegularExpressionValidator>
			                        </td>
			                    </tr>
			                    <tr>
			                        <td class="label">Last Package Weight</td>
			                        <td>
			                            <asp:TextBox runat="server" ID="txtLastPackageWeight" class="input" border="0" />
			                            <asp:RegularExpressionValidator ID="valRegLastPackageWeight" runat="server" ControlToValidate="txtLastPackageWeight" ValidationExpression="^\d+(\.)?(\d+)?$" Display="Dynamic">< invalid value</asp:RegularExpressionValidator>
			                        </td>
			                    </tr>
			                    <tr>
			                        <td class="label">Delivery / Sig Req'd?</td>
			                        <td>
			                            <asp:ListBox ID="lstDeliveryConfirmation" runat="server" Rows="1" class="input" border="0">
			                                <asp:ListItem Text="None" Value="0" Selected="True" />
			                                <asp:ListItem Text="Delivery Confirmation" Value="1" />
			                                <asp:ListItem Text="Del Conf / Signature Req'd" Value="2" />
			                                <asp:ListItem Text="Del Conf / Adult Sig Req'd" Value="3" />
			                            </asp:ListBox>
			                        </td>
			                    </tr>
			                    <tr>
			                        <td class="label">Rate from Multiple Locations?</td>
			                        <td>
			                            <asp:ListBox ID="lstShopRates" runat="server" Rows="1" class="input" border="0">
			                                <asp:ListItem Text="No" Value="No" Selected="True" />
			                                <asp:ListItem Text="Yes" Value="Yes" />
			                            </asp:ListBox>
			                        </td>
			                    </tr>
			                    <tr>
			                        <td colspan="2" align="center">
			                            <%--<asp:Button runat="server" ID="btnValidateTest" OnClick="btnValidateTest_Click" Text="Validate Test" />--%>
			                            <%--<asp:Button runat="server" ID="btnRate" OnClick="btnRate_Click" Text="Get Rates" />--%>
			                            <asp:Button runat="server" ID="btnValidateAndRate" OnClick="btnValidateAndRate_Click" Text="Get Rates" class="btn" />
			                        </td>
			                    </tr>
			                </table>
                        </td>
                    </tr>
                </table>   
            </asp:View>
            
            <!-- CHOOSE PLANT -->
            <asp:View ID="vwSelectPlant" runat="server">
            	<table cellpadding="0" cellspacing="0" border="0" class="containerTable">
					<tr>
						<td colspan="2" align="right"><a href="Help.aspx#plant" class="links">? Help</a></td>
					</tr>
					<tr>
						<td align="center"><img src="images/wiseLogo.jpg" alt="Wise" /></td>
					</tr>
					<tr>
						<td>
			                <asp:Panel ID="pnlFirstVisitInstructions" runat="server" Visible="false">
			                    <center>
			                        Since this is your first time to access this site, we do not have a default plant on file for you.
			                    </center>
			                    <br />
			                </asp:Panel>
			                <center>
			                    Please select a Plant to use as the "Ship From" location.<br /><br />
			                    <asp:Button ID="btnPlantALP" runat="server" Text="Alpharetta" CommandArgument="ALP" Width="100" OnCommand="btnSelectPlant_Click" class="btn" />
			                    &nbsp;
			                    <asp:Button ID="btnPlantBUT" runat="server" Text="Butler" CommandArgument="BUT" Width="100" OnCommand="btnSelectPlant_Click" class="btn" />
			                    &nbsp;
			                    <asp:Button ID="btnPlantFTW" runat="server" Text="Fort Wayne" CommandArgument="FTW" Width="100" OnCommand="btnSelectPlant_Click" class="btn" />
			                    &nbsp;
			                    <asp:Button ID="btnPlantPDT" runat="server" Text="Piedmont" CommandArgument="PDT" Width="100" OnCommand="btnSelectPlant_Click" class="btn" />
			                    &nbsp;
			                    <asp:Button ID="btnPlantPOR" runat="server" Text="Portland" CommandArgument="POR" Width="100" OnCommand="btnSelectPlant_Click" class="btn" />
			                </center>
						</td>
					</tr>
				</table>
            </asp:View>
            
            <!-- CHOOSE NEW ADDRESS -->
            <asp:View ID="vwSelectAddressCandidate" runat="server">
                <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                    <tr>
                        <td>
                            <table cellpadding="0" cellspacing="0" border="0" class="internalTable" width="100%">
                                <tr>
			                		<td colspan="3" align="right"><a href="Help.aspx#address" class="links">? Help</a></td>
			                	</tr>
								<tr>
                                    <td colspan="3" align="center"><img src="images/wiseLogo.jpg" alt="Wise" /></td>
                                </tr>
                                <tr>
                                    <td valign="top" width="120px" class="label">
                                        Address Entered:<br />
                                    </td>
                                    <td align="left"><asp:Label ID="lblAddressSubmitted" runat="server" /></td>
                                    <td width="200px" align="right"><asp:Button ID="btnEditAddress" runat="server" Text="Edit Address" OnClick="btnEditAddress_Click" class="btn" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br/><br/>
                <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                    <tr>
                        <td align="left">UPS doesn't recognize the address you specified. Please select from the alternatives below:</td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView ID="gvCandidates" runat="server" AutoGenerateColumns="false" 
                              DataKeyNames="ID" OnRowDataBound="gvCandidates_RowDataBound" OnRowCommand="gvCandidates_RowCommand" GridLines="None" AlternatingRowStyle-CssClass="alt">
                                <Columns>
                                    <asp:ButtonField CommandName="Select" HeaderText="" Text="Use this Address" ItemStyle-CssClass="links" />
                                    <asp:BoundField HeaderText="" DataField="displayAddress" ItemStyle-CssClass="chooseAddress" />
                                </Columns>
                            </asp:GridView>
                            <asp:GridView ID="gvCandidateData" runat="server" AutoGenerateColumns="true" Visible="false" ></asp:GridView>
                        </td>
                        <td align="right"><img src="images/LOGO_L.gif" alt="UPS Logo" /></td>
                    </tr>
                </table>
                <br /><br />
                <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                    <tr>
                        <td>
                            <p>
                                UPS, the UPS brand mark, and the Color Brown are trademarks of United Parcel Service of America, Inc. All Rights Reserved
                            </p>
                        </td>
                    </tr>
                </table>
                
                        
            </asp:View>
            
            <!-- CORRECTED ADDRESS -->
            <asp:View ID="vwVerifyAddressCorrection" runat="server">
                UPS has suggested a correction to the address you specified.<br />
                <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                    <tr>
                        <th>Address Entered</th>
                        <th>Corrected Address</th>
                        <th></th>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblEnteredAddressStreet" runat="server" /></td>
                        <td><asp:Label ID="lblCorrectedAddressStreet" runat="server" /></td>
                        <td><asp:Label ID="lblChangeToAddressStreet" runat="server" Text="< corrected" ForeColor="Red" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblEnteredAddressCity" runat="server" /></td>
                        <td><asp:Label ID="lblCorrectedAddressCity" runat="server" /></td>
                        <td><asp:Label ID="lblChangeToAddressCity" runat="server" Text="< corrected" ForeColor="Red" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblEnteredAddressState" runat="server" /></td>
                        <td><asp:Label ID="lblCorrectedAddressState" runat="server" /></td>
                        <td><asp:Label ID="lblChangeToAddressState" runat="server" Text="< corrected" ForeColor="Red" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblEnteredAddressZip" runat="server" /></td>
                        <td><asp:Label ID="lblCorrectedAddressZip" runat="server" /></td>
                        <td><asp:Label ID="lblChangeToAddressZip" runat="server" Text="< corrected" ForeColor="Red" /></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnEditAddress2" runat="server" Text="Edit Address" OnClick="btnEditAddress_Click" />
                        </td>
                        <td align="center">
                            <asp:Button ID="btnAcceptCorrectedAddress" runat="server" Text="Accept Corrections" OnClick="btnAcceptCorrectedAddress_Click" />
                        </td>
                    </tr>
                </table>
                <img src="images/LOGO_L.gif" alt="UPS Logo" />
            </asp:View>
            
            <!-- DISPLAY RETURNED RATES -->
            <asp:View ID="vwRates" runat="server">
                <asp:Panel ID="pnlRates" runat="server">
                    <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                        <tr>
							<td colspan="3" align="right"><a href="Help.aspx#rates" class="links">? Help</a></td>
						</tr>
						<tr>
                            <td colspan="3" align="center"><img src="images/wiseLogo.jpg" alt="Wise" /></td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <table cellpadding="2" cellspacing="0" border="0">
                                    <tr>
                                        <td><asp:Label ID="lblRateAddress" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="lblRateCity" runat="server" />&nbsp;<asp:Label ID="lblRateState" runat="server" />&nbsp;<asp:Label ID="lblRateZip" runat="server" /></td>
                                    </tr>
                                </table>
                            </td>
                            <td width="100"></td>
                            <td valign="top">
                                <table cellpadding="2" cellspacing="0" border="0">
                                    <tr>
                                        <td>Address is: <asp:Label ID="lblAddressClassification" runat="server" style="color:#a83333;" /></td>
                                    </tr>
                                    <tr>
                                        <td>Billing Weight: <asp:Label ID="lblBillingWeight" runat="server" /> lbs. (<asp:Label ID="lblNumPackages" runat="server" /> pkgs)</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center"><br /><asp:Label ID="lblRateWarning" runat="server" ForeColor="#a83333" />
                            <!--<br/>Please note: UPS HundredWeight rating is not yet functional.--></td>
                        </tr>
                    </table>
                    <br/><br/>
                    <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                    <tr>
                        <td>
                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                <tr>
                                    <td>
                                        <!-- SHIP FROM SELECTED PLANT -->
                                        <asp:GridView ID="gvServices" runat="server" 
                                            AutoGenerateColumns="false"
                                            GridLines="None"   
                                            CssClass="shipGrid"
                                            AlternatingRowStyle-CssClass="alt">
                                            <Columns>
                                                <asp:BoundField DataField="Plant" HeaderText="Ship From"  />
                                                <asp:BoundField DataField="Desc" HeaderText="Service" />
                                                <asp:BoundField DataField="ServiceCharges" HeaderText="Service Charges" DataFormatString="{0:c}" Visible="false"/>
                                                <asp:BoundField DataField="TransportationCharges" HeaderText="Transportation Charges" DataFormatString="{0:c}" Visible="false"/>
                                                <asp:BoundField DataField="Rate" HeaderText="Rate" DataFormatString="{0:c}" />
                                                <asp:TemplateField HeaderText="CWT?">
                                                    <ItemTemplate><%# ((bool)Eval("IsHundredWeight")) ? "Yes" : "No"%></ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        
                                        <!-- SHOP SHIPPING PLANT -->
                                        <asp:GridView ID="gvShopServices" runat="server"
                                            AutoGenerateColumns="false"
                                            GridLines="None"   
                                            CssClass="shipGrid"
                                            AlternatingRowStyle-CssClass="alt">
                                            <Columns>
                                                <asp:BoundField DataField="Desc" HeaderText="Ship From" />
                                                <asp:BoundField DataField="RateALP" HeaderText="From ALP" DataFormatString="{0:c}" />
                                                <asp:BoundField DataField="RateBUT" HeaderText="From BUT" DataFormatString="{0:c}" />
                                                <asp:BoundField DataField="RateFTW" HeaderText="From FTW" DataFormatString="{0:c}" />
                                                <asp:BoundField DataField="RatePDT" HeaderText="From PDT" DataFormatString="{0:c}" />
                                                <asp:BoundField DataField="RatePOR" HeaderText="From POR" DataFormatString="{0:c}" />
                                                <asp:TemplateField HeaderText="CWT?">
                                                    <ItemTemplate><%# ((bool)Eval("IsHundredWeight")) ? "Yes" : "No"%></ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                            
                                    </td>
                                    <td align="right">
                                        <img src="images/LOGO_L.gif" alt="UPS Logo"/>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                    <br />
                    <br />
                    <center>
                        <asp:Button ID="btnBackToEdit" runat="server" Text="Back" OnClick="btnBackToEdit_Click" class="btn" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnStartOver" runat="server" Text="Start Over" OnClick="btnStartOver_Click" class="btn" />
                    </center>
                </asp:Panel>
            </asp:View>
        </asp:MultiView>
    </div>

    <asp:Label ID="lblResults" runat="server" />

        <asp:Panel ID="pnlUpsTrademarkInfo" runat="server" Visible="false">
            <div class="upstrademarkinfo">
                <p>
                    UPS, the UPS brand mark, and the Color Brown are trademarks of United Parcel Service of America, Inc. All Rights Reserved
                </p>
            </div>
        </asp:Panel>
    </form>
</body>
</html>

