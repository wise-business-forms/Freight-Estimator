<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>Username:</td>
                <td><asp:TextBox ID="txtUsername" runat="server" /></td>
            </tr>
            <tr>
                <td>Password:</td>
                <td><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" /></td>
            </tr>
            <tr?>
                <td></td>
                <td>
                    <asp:Button ID="btnSubmitLogin" runat="server" OnClick="btnSubmitLogin_Click" Text="Login" /><br />
                    <asp:Label ID="lblLoginMessage" runat="server" ForeColor="Red" />

                </td>
            </tr>
        </table>
        <br />
        Please use your WiseLink credentials.
    </div>
    </form>
</body>
</html>
