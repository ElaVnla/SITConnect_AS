<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="As200537F.Login" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Helper/css/style.default.css" rel="stylesheet" />
    <link href="Helper/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://www.google.com/recaptcha/api.js?render=6LcB91keAAAAAG7FcCWp4Lb72lAEMQEJZTWzHQSR"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class=" page-holder d-flex align-items-center">
            <div class="container">
                <div class="row align-items-center py-5">
                    <div class="col-5 col-lg-7 mx-auto mb-5 mb-lg-0">

                        <div class="pr-lg-5">
                            <img src="illustration.svg" alt="" class="img-fluid" />
                        </div>
                    </div>
                    <div class="col-lg-5 px-lg-4">
                        <a href="Signup.aspx" class="text-base text-primary text-uppercase mb-4" height="50px" width="100px">Signup Here</a>

                        <h2 class="mb-4">Welcome Back!</h2>
                        <div class="col">
                            <asp:Label ID="errormsg" runat="server"></asp:Label>

                        </div>
                        <div class="form-group mb-4">
                            <asp:TextBox required="true" ID="emailUser" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="Email" runat="server"></asp:TextBox>

                        </div>

                        <div class="form-group mb-4">
                            <asp:TextBox required="true" ID="passworduser" TextMode="Password" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="Password" runat="server"></asp:TextBox>

                        </div>
                        <div class="form-group mb-4">
                            <div class="custom-control custom-checkbox">
                                <asp:CheckBox Text="&nbsp&nbsp&nbspRemember Me" runat="server" />
                            </div>
                        </div>
                        <div class="form-group mb-4">
                            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
                            <asp:Label ID="lblMessage" runat="server" EnableViewState="false">Check if you're a robot</asp:Label>
                        </div>
                        <asp:Button Text="LOGIN" CssClass="btn btn-primary" OnClick="Button_Click" Height="50px" Width="400px" runat="server" />
                    </div>
                </div>
            </div>

        </div>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LcB91keAAAAAG7FcCWp4Lb72lAEMQEJZTWzHQSR', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });

    </script>
</body>
</html>

