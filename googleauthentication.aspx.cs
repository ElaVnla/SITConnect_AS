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
        // declaring varaiables
        // for database
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        // for google authentication
        String AuthCode
        {
            get
            {
                if (ViewState["AuthCode"] != null)
                    return ViewState["AuthCode"].ToString().Trim();
                return String.Empty;
            }
            set
            {
                ViewState["AuthCode"] = value.Trim();
            }
        }

        String AuthTitle
        {
            get
            {
                // use user email as the authentication title
                return (string)Session["UserID"];
            }
        }


        String AuthBarCodeImage
        {
            get;
            set;
        }

        String AuthManualCode
        {
            get;
            set;


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("google authentication loading.....");

            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    System.Diagnostics.Debug.WriteLine("User account does not exist. Unable to access");

                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    if (!Page.IsPostBack)
                    {
                        // display result and qr code
                        resultlabel.Text = String.Empty;
                        resultlabel.Visible = false;
                        if (Generate2fa())
                        {
                            qrcodeimage.ImageUrl = AuthBarCodeImage;
                            mansetupcode.Text = AuthManualCode;
                            accname.Text = AuthTitle;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("an error occured when generating the two factor authentication.");

                            Response.Redirect("404.aspx", false);
                        }

                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User account does not exist. Unable to access");

                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                Response.Redirect("Login.aspx", false);
            }

        }

        protected void btnsubmitvalidate(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Validating verification code");

            String pinnumber = authsecuritycode.Text.Trim();
            Boolean statusofpin = ValidateTwoFactorPIN(pinnumber);
            if (statusofpin)
            {
                try
                {
                    using (SqlConnection connectionString = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmdstatement = new SqlCommand("UPDATE Account SET [EnableAuth]=@enableauth,[AuthCode]=@authcode WHERE email='" + (string)Session["UserID"] + "'"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                try
                                {
                                    cmdstatement.CommandType = CommandType.Text;
                                    cmdstatement.Parameters.AddWithValue("@enableauth", "true");
                                    cmdstatement.Parameters.AddWithValue("@authcode", AuthCode);
                                    /*cmd.Parameters.AddWithValue("@iv", Convert.ToBase64String(IV));
                                    cmd.Parameters.AddWithValue("@keyy", Convert.ToBase64String(Key));*/
                                    cmdstatement.Connection = connectionString;
                                    connectionString.Open();
                                    cmdstatement.ExecuteNonQuery();
                                    connectionString.Close();
                                }
                                catch (SqlException ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    Response.Redirect("404.aspx", false);
                                    //resultlabel.Text = ex.ToString();
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
                resultlabel.Visible = true;
                resultlabel.Text = "Code Successfully Verified. You can now Log In with Google Authentication";
            }
            else
            {
                resultlabel.Visible = true;
                resultlabel.Text = "Invalid Code. Please try again.";
            }
        }

        public bool ValidateTwoFactorPIN(String pin)
        {
            TwoFactorAuthenticator twofacauth = new TwoFactorAuthenticator();
            return twofacauth.ValidateTwoFactorPIN(AuthCode, pin);
        }

        public bool Generate2fa()
        {
            Guid guid = Guid.NewGuid();
            String uniqueSITUserKey = Convert.ToString(guid).Replace("-", "").Substring(0, 10);
            AuthCode = uniqueSITUserKey;

            Dictionary<String, String> result = new Dictionary<String, String>();
            TwoFactorAuthenticator twofacauth = new TwoFactorAuthenticator();
            var setupInformation = twofacauth.GenerateSetupCode("SITConnect Account", AuthTitle, AuthCode, false, 300);
            if (setupInformation != null)
            {
                AuthBarCodeImage = setupInformation.QrCodeSetupImageUrl;
                AuthManualCode = setupInformation.ManualEntryKey;
                return true;
            }
            return false;
        }
    }
}