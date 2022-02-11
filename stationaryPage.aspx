<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="stationaryPage.aspx.cs" Inherits="As200537F.stationaryPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Statioary</title>
</head>
<body>
    <form id="form1" runat="server">

        <div style="width: 50%; margin: 0 auto; text-align: center;">
            <table>
                <tr>
                    <td colspan="2">
                        <h2>Stationaries we are selling now!</h2>
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
                        runat="server" DataKeyNames="stationaryID" AutoGenerateColumns="false">
                        <Columns>
                            <asp:BoundField DataField="stationaryID" HeaderText="stationaryID" />
                            <asp:BoundField DataField="name" HeaderText="name" />
                            <asp:BoundField DataField="brand" HeaderText="brand" />
                            <asp:BoundField DataField="price" HeaderText="price" />

                            <asp:HyperLinkField DataNavigateUrlFields="stationaryID"
                                DataNavigateUrlFormatString="viewuser.aspx?stationaryid={0}"
                                Text="Buy Now" HeaderText="action" />
                        </Columns>
                    </asp:GridView>
                </tr>
            </table>
            <center>
                <asp:Label ID="Label1" runat="server" ForeColor="Red"></asp:Label></center>
        </div>
    </form>
    <a href="ProfilePage.aspx" type="button" class="btn btn-primary" height="50px" width="100px">Go Back</a>

</body>
</html>
