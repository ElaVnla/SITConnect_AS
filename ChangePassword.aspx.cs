using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace As200537F
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        // declaring variables
        // for database
        string strConnString = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string str = null;
        SqlCommand com;
        byte up;

        // for password
        static string finalHash;
        static string salt;

        // for encryption/decryption
        byte[] Key;
        byte[] IV;

        // for captcha
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        // bool functions
        // upon loading
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Change password page loading....");

            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    System.Diagnostics.Debug.WriteLine("User not found, unable to access");

                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                    Response.Redirect("Login.aspx", false);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User not found, unable to access");

                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Restricted access. Please login!!')", true);
                Response.Redirect("Login.aspx", false);
            }
        }
        protected void btn_update_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                System.Diagnostics.Debug.WriteLine("Captcha validation successful");
                int scores = checkPassword(txt_npassword.Text);
                string status = "";
                System.Diagnostics.Debug.WriteLine("validating password......");

                switch (scores)
                {
                    case 1:
                        status = "Very Weak";
                        break;
                    case 2:
                        status = "Weak";
                        break;
                    case 3:
                        status = "Medium";
                        break;
                    case 4:
                        status = "Strong";
                        break;
                    case 5:
                        status = "Excellent";
                        break;
                    default:
                        break;
                }
                //lbl_pwdchecker.Text = "Status : " + status;
                if (scores < 4)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid password");

                    lbl_pwdchecker.ForeColor = Color.Red;
                    return;
                }
                else
                {
                    SqlConnection con = new SqlConnection(strConnString);
                    con.Open();
                    str = "select * from Account ";
                    com = new SqlCommand(str, con);
                    /*SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        if (txt_cpassword.Text == reader["Password"].ToString())
                        {
                            up = 1;
                        }
                    }
                    reader.Close();
                    con.Close();*/
                    string email = (string)Session["userID"];
                    string pwd = HttpUtility.HtmlEncode(txt_cpassword.Text);
                    SHA512Managed hashing = new SHA512Managed();
                    string dbHash = getDBHash(email);
                    string dbSalt = getDBSalt(email);
                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("retrieval of password hash and salt success");

                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {
                                System.Diagnostics.Debug.WriteLine("Password is the same: may proceed");

                                string pwdnew = HttpUtility.HtmlEncode(txt_npassword.Text.ToString().Trim());
                                //Generate random "salt"
                                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                byte[] saltByte = new byte[8];
                                //Fills array of bytes with a cryptographically strong sequence of random values.
                                rng.GetBytes(saltByte);

                                salt = Convert.ToBase64String(saltByte);
                                SHA512Managed hashingg = new SHA512Managed();
                                string pwdWithSaltnew = pwdnew + salt;
                                byte[] plainHash = hashingg.ComputeHash(Encoding.UTF8.GetBytes(pwdnew));
                                byte[] hashWithSaltnew = hashingg.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSaltnew));

                                finalHash = Convert.ToBase64String(hashWithSaltnew);
                                RijndaelManaged cipher = new RijndaelManaged();
                                cipher.GenerateKey();
                                Key = cipher.Key;
                                IV = cipher.IV;

                                if (changepwd())
                                {
                                    lbl_msg.Text = "Password changed Successfully";

                                }
                                else
                                {
                                    lbl_msg.Text = "Password changed Unsuccessful Please try again.";

                                }
                            }
                            else
                            {
                                lbl_msg.Text = "Please enter correct Current password";
                                lbl_msg.ForeColor = Color.Red;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        Response.Redirect("404.aspx", false);
                        //throw new Exception(ex.ToString());
                    }
                    finally { }
                }

            }
            else
            {
                Response.Redirect("404.aspx", false);
            }
        }
        // captcha validation
        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6LcB91keAAAAANE-_UwF_zKRmTWJTADw3gYCVfke &response=" + captchaResponse);


            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose
                        //lbl_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g success or Error
                        //Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
        protected bool changepwd()
        {
            try
            {
                using (SqlConnection connectionString = new SqlConnection(strConnString))
                {
                    using (SqlCommand cmdstatement = new SqlCommand("UPDATE Account SET [PasswordHash]=@hash,[PasswordSalt]=@Salt WHERE email='" + Session["UserID"].ToString() + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            try
                            {
                                cmdstatement.CommandType = CommandType.Text;
                                cmdstatement.Parameters.AddWithValue("@hash", finalHash);
                                cmdstatement.Parameters.AddWithValue("@Salt", salt);
                                /*cmd.Parameters.AddWithValue("@iv", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@keyy", Convert.ToBase64String(Key));*/
                                cmdstatement.Connection = connectionString;
                                connectionString.Open();
                                cmdstatement.ExecuteNonQuery();
                                connectionString.Close();
                                return true;
                            }
                            catch (SqlException ex)
                            {
                                //lblMessage.Text = ex.ToString();
                                return false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        // void functions

        // string functions
        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(strConnString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }
        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(strConnString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        // int function
        // validation for password
        private int checkPassword(string password)
        {
            int score = 0;

            // include your implementation here

            //score 0 very weak;
            // if length of password is less than 8 chars
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            // score 2 weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            // score 3 medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            // score 4 strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            // score 5 excellent
            if (Regex.IsMatch(password, "(?=.*[^A-Za-z0-9])"))
            {
                score++;
            }

            return score;
        }




    }
}