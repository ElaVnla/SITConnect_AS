<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="As200537F.ChangePassword" ValidateRequest="false" %>

<script>
    function validate() {
        var str = document.getElementById('<%=txt_ccpassword.ClientID %>').value;

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
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LcB91keAAAAAG7FcCWp4Lb72lAEMQEJZTWzHQSR"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Current password" Width="120px"
                Font-Bold="True" ForeColor="#996633"></asp:Label>
            <asp:TextBox ID="txt_cpassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                ControlToValidate="txt_cpassword"
                ErrorMessage="Please enter Current password"></asp:RequiredFieldValidator>
            <br />
            <asp:Label ID="Label2" runat="server" Text="New password" Width="120px"
                Font-Bold="True" ForeColor="#996633"></asp:Label>
            <asp:TextBox ID="txt_npassword" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                ControlToValidate="txt_npassword" ErrorMessage="Please enter New password" onkeyup="javascript:validate()"></asp:RequiredFieldValidator>
            <br />

            <asp:Label ID="Label3" runat="server" Text="Confirm password" Width="120px"
                Font-Bold="True" ForeColor="#996633"></asp:Label>

            <asp:TextBox ID="txt_ccpassword" runat="server" TextMode="Password"></asp:TextBox>

            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                ControlToValidate="txt_ccpassword"
                ErrorMessage="Please enter Confirm  password"></asp:RequiredFieldValidator>
            <asp:Label ID="lbl_pwdchecker" runat="server"></asp:Label>

            <asp:CompareValidator ID="CompareValidator1" runat="server"
                ControlToCompare="txt_npassword" ControlToValidate="txt_ccpassword"
                ErrorMessage="Password Mismatch"></asp:CompareValidator>
        </div>
        <asp:Button ID="btn_update" runat="server" Font-Bold="True" BackColor="#CCFF99" OnClick="btn_update_Click" Text="Update" />
        <asp:Label ID="lbl_msg" Font-Bold="True" BackColor="#FFFF66" ForeColor="#FF3300" runat="server" Text=""></asp:Label><br />
        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
        <asp:Label ID="lblMessage" runat="server" EnableViewState="false">Check if you're a robot</asp:Label>
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
