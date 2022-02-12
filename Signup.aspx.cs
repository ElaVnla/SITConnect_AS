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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace As200537F
{
    public partial class Signup : System.Web.UI.Page
    {
        //declaring variables ----------------------------
        // for database
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        // for password
        static string finalHash;
        static string salt;
        // for encryption/descryption of creditcard number
        byte[] Key;
        byte[] IV;

        // declaring class --------------------------------
        // class for captcha
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        // void functions ----------------------------------
        public bool ValidateCaptcha()
        {
            System.Diagnostics.Debug.WriteLine("Start of validation of Captcha......");

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
                System.Diagnostics.Debug.WriteLine("End of validation of Captcha......");

                return result;
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Validation of Captcha -> " + ex.ToString());
                throw ex;
            }

        }
        // page function when Signup.aspx is launched
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("signup page starts loading......");
        }

        // delcaring password 
        protected void tb_passwword_TextChanged(object sender, EventArgs e)
        {

        }

        // When user wishes to submit the registration form
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                System.Diagnostics.Debug.WriteLine("captcha validation successful");
                StartUpLoad();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("captcha validation unsuccessful");
                Response.Redirect("404.aspx", false);
            }
        }
        private void StartUpLoad()
        {
            System.Diagnostics.Debug.WriteLine("-- Validation of registration form --");

            int scores = checkPassword(tb_password.Text);
            string status = "";
            System.Diagnostics.Debug.WriteLine("validating password...");

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
            System.Diagnostics.Debug.WriteLine("Status of Password: " + status);

            if (scores < 4)
            {
                lbl_pwdchecker.Text = "Status of password : " + status;
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Password validation: Passed");

                lbl_pwdchecker.ForeColor = Color.Green;

                // validate email, email must be unique
                System.Diagnostics.Debug.WriteLine("Validating email...");

                if (CheckExistingEmail(email.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("Email already exist");

                    lblMessage.Text = "Email is already in use. Please use another email";
                    lblMessage.ForeColor = Color.Red;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Email validation: Passed");
                    System.Diagnostics.Debug.WriteLine("Validating profile image....");

                    //get the file name of the posted image  
                    string imgName = FileUpload1.FileName;
                    //sets the image path  
                    string imgPath = "ImageStorage/" + imgName;
                    //get the size in bytes that  

                    int imgSize = FileUpload1.PostedFile.ContentLength;

                    //validate image before saving 
                    if (FileUpload1.PostedFile != null && FileUpload1.PostedFile.FileName != "")
                    {
                        System.Diagnostics.Debug.WriteLine("Image retrieve successfully...");

                        // 10240 KB means 10MB, You can change the value based on your requirement  
                        if (FileUpload1.PostedFile.ContentLength > 500000)
                        {
                            System.Diagnostics.Debug.WriteLine("File size too big. Unable to save.");
                            uploadchecker.Text = "File size too big. Please submit an image lesser than 500000Kb";
                            uploadchecker.ForeColor = Color.Red;
                            /*                        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('File is too big.')", true);
                            */
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Image size validation: Passed");
                            //Saving to image profile folder
                            FileUpload1.SaveAs(Server.MapPath(imgPath));
                            Image1.ImageUrl = "~/" + imgPath;
                            //Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Image saved!')", true);

                            System.Diagnostics.Debug.WriteLine("Hashing of Password.....");

                            // password hashing
                            string pwd = tb_password.Text.ToString().Trim();
                            //Generate random "salt"
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];
                            //Fills array of bytes with a cryptographically strong sequence of random values.
                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);
                            SHA512Managed hashing = new SHA512Managed();
                            string pwdWithSalt = pwd + salt;
                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            finalHash = Convert.ToBase64String(hashWithSalt);
                            RijndaelManaged cipher = new RijndaelManaged();
                            cipher.GenerateKey();
                            Key = cipher.Key;
                            IV = cipher.IV;
                            System.Diagnostics.Debug.WriteLine("Hashing Password: Complete");
                            System.Diagnostics.Debug.WriteLine("Creating Account....");

                            if (createAccount(imgPath))
                            {
                                System.Diagnostics.Debug.WriteLine("Creationg of Account: Complete");

                                Response.Redirect("Login.aspx");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Failure in creation of Account");

                                Response.Redirect("404.aspx", false);

                            }
                        }

                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Upload image fails");
                        uploadchecker.Text = "Please upload an image";
                        uploadchecker.ForeColor = Color.Red;
                    }
                }
            }
        }
        // bool functions ----------------------------------
        protected bool createAccount(string imgPath)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account(firstname,lastname,creditcard,email,DOB,PasswordHash,PasswordSalt,profilepicture,[IV],[Key]) VALUES(@firstname,@lastname,@creditcard,@email,@DOB,@PasswordHash,@PasswordSalt,@profilepicture,@IV,@Key)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            try
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@firstname", HttpUtility.HtmlEncode(firstname.Text.Trim()));
                                cmd.Parameters.AddWithValue("@lastname", HttpUtility.HtmlEncode(lastname.Text.Trim()));
                                cmd.Parameters.AddWithValue("@creditcard", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(creditcard.Text.Trim().ToString()))));
                                cmd.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email.Text.Trim()));
                                cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(DOB.Text.Trim()));
                                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                cmd.Parameters.AddWithValue("@profilepicture", imgPath);
                                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                                return true;
                            }
                            catch (SqlException ex)
                            {
                                //lblMessage.Text = ex.ToString();
                                System.Diagnostics.Debug.WriteLine(ex.ToString());
                                return false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {/*
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Response.Redirect("404.aspx", false);*/
                throw new Exception(ex.ToString());
            }
        }


        protected bool CheckExistingEmail(string email)
        {
            SqlConnection connectionString = new SqlConnection(MYDBConnectionString);
            string sqlStatement = "select email FROM Account";
            SqlCommand command = new SqlCommand(sqlStatement, connectionString);
            try
            {
                connectionString.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["email"] != DBNull.Value)
                        {
                            if (email == reader["email"].ToString())
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
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
                connectionString.Close();
            }
            return false;
        }

        // byte function -----------------------------------
        //encrypting items passed through in this function
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                System.Diagnostics.Debug.WriteLine("--- Start of Encryption ---");

                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally { }
            System.Diagnostics.Debug.WriteLine("--- End of Encryption ---");
            return cipherText;
        }

        // int function ------------------------------------
        // validation of password
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
                System.Diagnostics.Debug.WriteLine("--- small letters : true ---");
                score++;
            }
            // score 3 medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                System.Diagnostics.Debug.WriteLine("--- Capital letters : true ---");

                score++;
            }
            // score 4 strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                System.Diagnostics.Debug.WriteLine("--- Numbers : true ---");

                score++;
            }
            // score 5 excellent
            if (Regex.IsMatch(password, "(?=.*[^A-Za-z0-9])"))
            {
                System.Diagnostics.Debug.WriteLine("--- Special characters : true ---");

                score++;
            }

            return score;
        }


    }
}