<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Analysis.aspx.cs" Inherits="Analysis" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Wise Freight Estimator - Usage Analysis</title>
	    
	<!-- CSS LINKS -->
    <link href="style/master.css" rel="stylesheet" type="text/css" media="all" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="containerMain">
            <table cellpadding="0" cellspacing="0" border="0" class="containerTable">
                <tr>
                    <td>
                        <h3>All Rating Requests</h3>
                        <asp:GridView ID="gvRateRequestTotals" runat="server" GridLines="None" CssClass="analysisGrid" AlternatingRowStyle-CssClass="alt"
                                      AutoGenerateColumns="true" DataSourceID="dsRateRequestTotals">
                                      <HeaderStyle CssClass="analysisHeader" />
                        </asp:GridView>
                    
                        <br />
                        <h3>Rating Requests by Plant</h3>
                        <asp:GridView ID="gvRateRequestsByPlant" runat="server" GridLines="None" CssClass="analysisGrid" AlternatingRowStyle-CssClass="alt"
                                      AutoGenerateColumns="true" DataSourceID="dsRateRequestsByPlant" AllowSorting="true">
                                      <HeaderStyle CssClass="analysisHeader" />
                        </asp:GridView>

                        
                        <br />
                        <h3>Rating Requests by User</h3>
                        <asp:GridView ID="gvRateRequestsByUser" runat="server" GridLines="None" CssClass="analysisGrid" AlternatingRowStyle-CssClass="alt"
                                      AutoGenerateColumns="true" DataSourceID="dsRateRequestsByUser" AllowSorting="true">
                                      <HeaderStyle CssClass="analysisHeader" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </div>

        <asp:SqlDataSource ID="dsRateRequestTotals" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection %>" 
                           SelectCommandType="StoredProcedure" SelectCommand="Analysis_GetRateRequestTotals" />
        <asp:SqlDataSource ID="dsRateRequestsByPlant" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection %>" 
                           SelectCommandType="StoredProcedure" SelectCommand="Analysis_GetPlantRateRequestTotals" />
        <asp:SqlDataSource ID="dsRateRequestsByUser" runat="server" ConnectionString="<%$ ConnectionStrings:UpsRateSqlConnection %>" 
                           SelectCommandType="StoredProcedure" SelectCommand="Analysis_GetUserRateRequestTotals" />
    </form>
</body>
</html>
