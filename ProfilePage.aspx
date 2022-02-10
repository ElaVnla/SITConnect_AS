<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProfilePage.aspx.cs" Inherits="As200537F.ProfilePage" ValidateRequest="false" %>

<!DOCTYPE html>
<script src="//code.jquery.com/jquery-1.11.2.min.js" type="text/javascript"></script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Helper/css/style.default.css" rel="stylesheet" />
    <link href="Helper/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

</head>
<body>

    <center>
        <h4 style="color: mediumpurple;">Welcome! You have successfully logged in!</h4>
        <div class="container mt-4 mb-4 p-3 d-flex justify-content-center">

            <div class="card p-4">
                <div class=" image d-flex flex-column justify-content-center align-items-center">
                    <asp:Image ID="userprofile" Height="100" Width="100" runat="server" />
                </div>
                <div class="d-flex flex-column justify-content-center align-items-center">
                    <span class="name mt-3">
                        <asp:Label ID="userfirstname" runat="server" Text="Label"></asp:Label>
                        &nbsp
                        <asp:Label ID="userlastname" runat="server" Text="Label"></asp:Label></span>
                    <div class="d-flex flex-row justify-content-center align-items-center gap-2">
                        <span><strong>Credit Card: </strong>
                            <asp:Label ID="usercreditcard" runat="server" Text="Label"></asp:Label></span>
                    </div>
                    <div class="d-flex flex-row justify-content-center align-items-center gap-2">
                        <span><strong>Email: </strong></span>
                        <asp:Label ID="useremail" runat="server" Text="Label"></asp:Label>
                    </div>
                    <div class="d-flex flex-row justify-content-center align-items-center gap-2"><span><strong>Date of Birth:</strong></span><asp:Label ID="userDOB" runat="server" Text="Label"></asp:Label></div>
                    <div class=" d-flex mt-2">

                        <form id="form1" runat="server">
                            <asp:Button ID="Logout" CssClass="btn btn-primary" OnClick="LogoutMe" Text="Logout" Height="50px" Width="100px" runat="server" />
                        </form>
                    </div>
                    <div class=" d-flex mt-2 row">

                        <a href="stationaryPage.aspx" type="button" class="btn btn-primary" height="50px" width="100px">Buy Stationary</a>
                        <a href="ChangePassword.aspx" type="button" class="btn btn-primary" height="50px" width="100px">Change Password</a>
                        <a href="googleauthentication.aspx" type="button" class="btn btn-primary" height="50px" width="100px">Enable Google Authentication</a>


                    </div>

                </div>
            </div>
        </div>

    </center>
</body>
</html>
