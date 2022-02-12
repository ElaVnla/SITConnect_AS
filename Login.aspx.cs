using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace As200537F
{
    public partial class Login : System.Web.UI.Page
    {
        //declaring variables ----------------------------
        // for database
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        // for class captcha
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        // void functions
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Login page starts loading......");

        }

        // login button
        protected void Button_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                System.Diagnostics.Debug.WriteLine("captcha validation successful");

                if (checkAccountLockout())
                {
                    System.Diagnostics.Debug.WriteLine("Account has 3 failed attempts... validating if account can be recovered...");

                    var accountfaileddate = getFailedDateTime();
                    if (accountfaileddate == DateTime.Now)
                    {
                        System.Diagnostics.Debug.WriteLine("There is an error retrieving the failed date of the account....");

                        Response.Redirect("404.aspx", false);
                    }
                    else
                    {
                        // 50 seconds for testing purposes
                        var recoveryTime = accountfaileddate.AddSeconds(50);
                        if (recoveryTime > DateTime.Now)
                        {
                            System.Diagnostics.Debug.WriteLine("Account is still locked");

                            lblMessage.Text = "Your Account is locked after 3 failed attempts. Please wait until " + recoveryTime;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Account can now be unlocked");

                            restartFailedAttempts();
                            submitcontinuation();
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Account has lesser than 3 or no failed attempts. They may proceed..");

                    submitcontinuation();

                }

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("captcha validation failed");
                Response.Redirect("404.aspx", false);

            }

        }

        // continuation of loggin in
        protected void submitcontinuation()
        {
            System.Diagnostics.Debug.WriteLine("Validating password inputed.......");

            string email = HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim());
            string pwd = HttpUtility.HtmlEncode(passworduser.Text.ToString().Trim());
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Successfully retrieve the password hash and salt");

                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {
                        System.Diagnostics.Debug.WriteLine("Password validation: Success");

                        Session["UserID"] = email;

                        // create a new GUID and save into the session
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;
                        System.Diagnostics.Debug.WriteLine("Validating if use enabled 2fa....");

                        var result = checkauth(email);

                        // now create a new cookie with this guid value
                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                        if (result == "true")
                        {
                            System.Diagnostics.Debug.WriteLine("User has 2fa enabled");
                            Response.Redirect("verifyAuth.aspx", false);
                        }
                        else if (result == "false")
                        {
                            System.Diagnostics.Debug.WriteLine("User does not have 2fa enabled");

                            Response.Redirect("ProfilePage.aspx", false);

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("clearing sessions and auth token");

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

                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Password validation: Failed");

                        errormsg.Text = "Invalid email/password login details";
                        errormsg.ForeColor = Color.Red;
                        if (!checkAccountLockout())
                        {
                            addfailcounter();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Response.Redirect("404.aspx", false);
                /*                    throw new Exception(ex.ToString());
                */
            }
            finally { }
        }

        // refresh account lockout and failed attempts after time has passed
        protected void restartFailedAttempts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Update Account set FailedNoAttempts = @failattempt, failedTime = @failtime WHERE email = '" + HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim()) + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@failattempt", 0);
                            cmd.Parameters.AddWithValue("@failtime", DateTime.Now);
                            cmd.Connection = connection;
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("404.aspx", false);
                throw new Exception(ex.ToString());
            }
        }

        protected void addfailcounter()
        {
            //var wee = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Update Account set FailedNoAttempts = @failattempt, failedTime = @failtime WHERE email = '" + HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim()) + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@failattempt", getnumberfailed() + 1);
                            cmd.Parameters.AddWithValue("@failtime", DateTime.Now);
                            cmd.Connection = connection;
                            connection.Open();

                            cmd.ExecuteNonQuery();
                            connection.Close();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("404.aspx", false);

                throw new Exception(ex.ToString());
            }


            /*SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update Account set FailedNoAttempts = @failattempt, failedTime = @failtime WHERE email = '" + HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim()) + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            string sql2 = "select FailedNoAttempts FROM Account WHERE email='" + HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim()) + "'";
            SqlCommand command2 = new SqlCommand(sql2, connection);
            var wee = false;
            try
            {
                connection.Open();
                using (SqlDataReader reader = command2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FailedNoAttempts"] != DBNull.Value)
                        {
                            if ((reader["FailedNoAttempts"] != null) && (Convert.ToInt32(reader["FailedNoAttempts"]) < 3))
                            {
                                wee = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            if (wee)
            {
                try
                {
                    connection.Open();

                    SqlDataReader reader2 = command.ExecuteReader();
                    while (reader2.Read())
                    {
                        if (reader2["FailedNoAttempts"] != DBNull.Value || Convert.ToInt32(reader2["FailedNoAttempts"]) < 3)
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@failattempt", Convert.ToInt32(reader2["FailedNoAttempts"]) + 1);
                            command.Parameters.AddWithValue("@failtime", DateTime.Now);
                            command.Connection = connection;
                            command.ExecuteNonQuery();
                        }
                        connection.Close();




                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    connection.Close();
                }
            }*/
        }

        // string functions
        // check whether the account enabled the 2 factor authentication, if yes it returns true else false
        protected string checkauth(string userid)
        {
            var result = "";
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
                        if (reader["EnableAuth"] != DBNull.Value)
                        {
                            string authtf = reader["EnableAuth"].ToString();
                            if (authtf == "true")
                            {
                                result = "true";
                            }
                            else
                            {
                                result = "false";
                            }
                        }
                        else
                        {
                            result = "false";
                        }
                    }
                }
            }//try
            catch (Exception ex)
            {
                result = "error";
                throw new Exception(ex.ToString());

            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        // retrieve the password hash from database
        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
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

        // retrieve the password salt from database
        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
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
        // bool functions
        // validation of captcha
        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           ("https://www.google.com/recaptcha/api/siteverify?secret=6LcB91keAAAAANE-_UwF_zKRmTWJTADw3gYCVfke &response=" + captchaResponse);


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

        // checking if account has any failed attempts of 3 or lesser
        protected bool checkAccountLockout()
        {
            SqlConnection connectionString = new SqlConnection(MYDBConnectionString);
            string sqlStatement = "select FailedNoAttempts FROM Account WHERE email='" + HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim()) + "'";
            SqlCommand commandStatement = new SqlCommand(sqlStatement, connectionString);
            try
            {
                connectionString.Open();
                using (SqlDataReader reader = commandStatement.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FailedNoAttempts"] != DBNull.Value)
                        {
                            if ((reader["FailedNoAttempts"] != null) && (Convert.ToInt32(reader["FailedNoAttempts"]) != 3))
                            {
                                System.Diagnostics.Debug.WriteLine("account lockout still below 3");
                                return false;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("account lockout exist");

                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }//try
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connectionString.Close();
            }
            return false;
        }

        // int function
        protected int getnumberfailed()
        {
            var result = 0;
            SqlConnection connectionString = new SqlConnection(MYDBConnectionString);
            string sqlStatement = "select FailedNoAttempts FROM Account WHERE email='" + HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim()) + "'";
            SqlCommand commandStatement = new SqlCommand(sqlStatement, connectionString);
            try
            {
                connectionString.Open();
                using (SqlDataReader reader = commandStatement.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FailedNoAttempts"] != DBNull.Value)
                        {
                            result = Convert.ToInt32(reader["FailedNoAttempts"]);
                        }
                    }
                }
            }//try
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connectionString.Close();
            }
            return result;
        }

        // DateTime function
        // obtain the failed loging date time from account to validate
        protected DateTime getFailedDateTime()
        {
            DateTime faileddate = DateTime.Now;
            SqlConnection connectionString = new SqlConnection(MYDBConnectionString);
            string sql = "select failedTime FROM Account WHERE email='" + HttpUtility.HtmlEncode(emailUser.Text.ToString().Trim()) + "'";
            SqlCommand command = new SqlCommand(sql, connectionString);
            try
            {
                connectionString.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["failedTime"] != DBNull.Value)
                        {
                            faileddate = (DateTime)reader["failedTime"];
                        }
                    }
                }
            }//try
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connectionString.Close();
            }
            return faileddate;
        }





    }

}