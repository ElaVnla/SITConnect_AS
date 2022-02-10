<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="stationaryPage.aspx.cs" Inherits="As200537F.stationaryPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SQL InJection</title>
</head>
<body>
    <form id="form1" runat="server">

        <div style="width: 50%; margin: 0 auto; text-align: center;">
            <table>
                <tr>
                    <td colspan="2">
                        <h2>SQL Injection Demo</h2>
                    </td>
                </tr>
                <tr>
                    <td>Search by userid
                        <asp:TextBox ID="txtUserID" runat="server">
                        </asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnSubmit" OnClick="BtnSubmit_Click"
                            runat="server" Text="Search" />
                    </td>
                </tr>
                <tr>
                    <asp:GridView ID="gvUserInfo" Width="100%"
                        runat="server" DataKeyNames="userID" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="userID" HeaderText="userID" />
                            <asp:BoundField DataField="name" HeaderText="name" />
                            <asp:BoundField DataField="email" HeaderText="email" />
                            <asp:HyperLinkField DataNavigateUrlFields="userID"
                                DataNavigateUrlFormatString="viewuser.aspx?userid={0}"
                                Text="View User" HeaderText="action" />
                        </Columns>
                    </asp:GridView>
                </tr>
            </table>
            <center>
                <asp:Label ID="Label1" runat="server" ForeColor="Red"></asp:Label></center>
        </div>
    </form>
</body>
</html>
