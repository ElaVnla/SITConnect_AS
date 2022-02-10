<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="As200537F.Signup" ValidateRequest="false" %>

<script src="//code.jquery.com/jquery-1.11.2.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function ShowImagePreview(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#<%=Image1.ClientID%>').prop('src', e.target.result)
                    .width(240)
                    .height(150);
            };
            reader.readAsDataURL(input.files[0]);
        }
    }
    function validate() {
        var str = document.getElementById('<%=tb_password.ClientID %>').value;

        if (str.length < 12) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password Length Must be at least 12 characters";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("too_short");
        }

        else if (str.search(/[0-9]/) == -1) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least 1 number";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_number");
        }

        else if (str.search(/[A-Z]/) == -1) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least Upper cases";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_upper_case");
        }
        else if (str.search(/[a-z]/) == -1) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least lower cases";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_lower_case");
        }
        else if (str.search(/[!@#$%^&*]/) == -1) {
            document.getElementById("lbl_pwdchecker").innerHTML = "Password require at least special characters";
            document.getElementById("lbl_pwdchecker").style.color = "Red";
            return ("no_special_characters");
        }

        document.getElementById("lbl_pwdchecker").innerHTML = "Excellent!"
        document.getElementById("lbl_pwdchecker").style.color = "Blue";
    }
</script>
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
                        <h1 class="text-base text-primary text-uppercase mb-4">Sign up Here</h1>
                        <h2 class="mb-4">Welcome!</h2>

                        <div class="form-group mb-4">
                            <asp:TextBox required="true" ID="firstname" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="First Name" runat="server"></asp:TextBox>

                        </div>
                        <div class="form-group mb-4">
                            <asp:TextBox required="true" ID="lastname" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="Last Name" runat="server"></asp:TextBox>

                        </div>
                        <div class="form-group mb-4">
                            <asp:TextBox required="true" ID="creditcard" MaxLength="16" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="Credit Card Info" runat="server"></asp:TextBox>

                        </div>
                        <div class="form-group mb-4">
                            <asp:TextBox required="true" ID="email" TextMode="Email" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="Email address" runat="server"></asp:TextBox>

                        </div>

                        <div class="form-group mb-4">
                            <div class="form-row">
                                <asp:TextBox ID="tb_password" required="true" runat="server" Width="209px" TextMode="Password" onkeyup="javascript:validate()" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="Password"></asp:TextBox>
                                <div class="col-6">
                                    <asp:Label ID="lbl_pwdchecker" runat="server"></asp:Label>
                                </div>
                            </div>

                        </div>
                        <div class="form-group mb-4">
                            <asp:TextBox required="true" ID="DOB" TextMode="Date" CssClass="form-control border-0 shadow form-control-lg text-base" placeholder="Date of Birth" runat="server"></asp:TextBox>

                        </div>
                        <div class="form-group mb-4">
                            <h6>Upload profile picture</h6>
                            <div style="text-align: center;">
                                <asp:Image ID="Image1" Height="150px" Width="240px" runat="server" /><br />
                                <asp:FileUpload ID="FileUpload1" runat="server" onchange="ShowImagePreview(this);" />
                            </div>
                            <div class="col-6">
                                <asp:Label ID="uploadchecker" runat="server"></asp:Label>
                            </div>
                        </div>
                        <script>

                        </script>
                        <div class="form-group mb-4">
                            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
                            <asp:Label ID="lblMessage" runat="server" EnableViewState="false">Check if you're a robot</asp:Label>
                        </div>
                        <asp:Button ID="Button1" OnClick="Button1_Click" Text="Sign up" CssClass="btn btn-primary" Height="50px" Width="400px" runat="server" />
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
