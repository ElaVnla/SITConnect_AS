using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Authenticator;

namespace As200537F
{
    public partial class googleauthentication : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        String AuthenticationCode
        {
            get
            {
                if (ViewState["AuthenticationCode"] != null)
                    return ViewState["AuthenticationCode"].ToString().Trim();
                return String.Empty;
            }
            set
            {
                ViewState["AuthenticationCode"] = value.Trim();
            }
        }

        String AuthenticationTitle
        {
            get
            {
                return (string)Session["UserID"];
            }
        }


        String AuthenticationBarCodeImage
        {
            get;
            set;
        }

        String AuthenticationManualCode
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    if (!Page.IsPostBack)
                    {
                        lblResult.Text = String.Empty;
                        lblResult.Visible = false;
                        GenerateTwoFactorAuthentication();
                        imgQrCode.ImageUrl = AuthenticationBarCodeImage;
                        lblManualSetupCode.Text = AuthenticationManualCode;
                        lblAccountName.Text = AuthenticationTitle;
                    }
                }
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                Response.Redirect("Login.aspx", false);
            }

        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {
            String pin = txtSecurityCode.Text.Trim();
            Boolean status = ValidateTwoFactorPIN(pin);
            if (status)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Account SET [EnableAuth]=@enableauth,[AuthCode]=@authcode WHERE email='" + (string)Session["UserID"] + "'"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@enableauth", "true");
                                    cmd.Parameters.AddWithValue("@authcode", AuthenticationCode);
                                    /*cmd.Parameters.AddWithValue("@iv", Convert.ToBase64String(IV));
                                    cmd.Parameters.AddWithValue("@keyy", Convert.ToBase64String(Key));*/
                                    cmd.Connection = con;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                                catch (SqlException ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    Response.Redirect("404.aspx", false);
                                    //lblResult.Text = ex.ToString();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    Response.Redirect("404.aspx", false);
                    //throw new Exception(ex.ToString());
                }
                lblResult.Visible = true;
                lblResult.Text = "Code Successfully Verified. You can now Log In with Google Authentication";
            }
            else
            {
                lblResult.Visible = true;
                lblResult.Text = "Invalid Code.";
            }
        }

        public Boolean ValidateTwoFactorPIN(String pin)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(AuthenticationCode, pin);
        }

        public Boolean GenerateTwoFactorAuthentication()
        {
            Guid guid = Guid.NewGuid();
            String uniqueUserKey = Convert.ToString(guid).Replace("-", "").Substring(0, 10);
            AuthenticationCode = uniqueUserKey;

            Dictionary<String, String> result = new Dictionary<String, String>();
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("Complio", AuthenticationTitle, AuthenticationCode, false, 300);
            if (setupInfo != null)
            {
                AuthenticationBarCodeImage = setupInfo.QrCodeSetupImageUrl;
                AuthenticationManualCode = setupInfo.ManualEntryKey;
                return true;
            }
            return false;
        }
    }
}