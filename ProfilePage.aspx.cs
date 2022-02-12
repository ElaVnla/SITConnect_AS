using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace As200537F
{
    public partial class ProfilePage : System.Web.UI.Page
    {
        // declaring variables
        // for database
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        // decrypting
        byte[] Key;
        byte[] IV;
        // for displaying user info
        byte[] creditcard = null;
        string userID = null;

        // void functions
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Profile page loading.....");

            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    System.Diagnostics.Debug.WriteLine("User not found, unable to access page.");

                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("User found, page access allowed.");

                    userID = (string)Session["UserID"];
                    displayUserProfile(userID);
                }
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                Response.Redirect("Login.aspx", false);
            }
        }
        // display user profile details
        protected void displayUserProfile(string userid)
        {
            System.Diagnostics.Debug.WriteLine("Retrieving user details....");

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["firstname"] != DBNull.Value)
                        {
                            userfirstname.Text = reader["firstname"].ToString();
                        }
                        if (reader["lastname"] != DBNull.Value)
                        {
                            userlastname.Text = reader["lastname"].ToString();
                        }
                        if (reader["creditcard"] != DBNull.Value)
                        {
                            creditcard = Convert.FromBase64String(reader["creditcard"].ToString());
                        }
                        if (reader["email"] != DBNull.Value)
                        {
                            useremail.Text = reader["email"].ToString();
                        }
                        if (reader["DOB"] != DBNull.Value)
                        {
                            userDOB.Text = reader["DOB"].ToString();
                        }
                        if (reader["profilepicture"] != DBNull.Value)
                        {
                            userprofile.Attributes["src"] = reader["profilepicture"].ToString();
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }
                    usercreditcard.Text = decryptData(creditcard);
                }
            }//try
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("error when displaying. Error msg -> " + ex.ToString());
                Response.Redirect("404.aspx", false);
                //throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            System.Diagnostics.Debug.WriteLine("Display complete");

        }


        // when user wishes to log out
        protected void LogoutMe(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("User logging out.....");

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies["AuthToken"].Value = string.Empty;
                    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                }

            }
        }

        // string function
        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecryot = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string
                            plainText = srDecryot.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { }
            return plainText;
        }
    }
}