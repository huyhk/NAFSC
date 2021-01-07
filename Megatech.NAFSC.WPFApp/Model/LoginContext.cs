using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Megatech.NAFSC.WPFApp.Model
{
    public class LoginContext: DbContext
    {
        private static string appPath = Environment.CurrentDirectory;
        private static string ConnectionString
        {
            get
            {
                string cnString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + appPath + @"\Security.mdf;Integrated Security=True";

                return cnString;
            }
        }
        
        public LoginContext():base(ConnectionString)
        {

        }
        public DbSet<LoginModel> Logins { get; set; }

        public LoginModel Login(string username, string password)
        {
            if (AppSetting.CheckInternet(false))
            {
                ApiHelper helper = new ApiHelper();
                var login = helper.Login(username, password);

                if (login != null)
                {
                    ApiHelper.SetToken(login.Access_Token);

                    AppSetting.CurrentSetting.UserId = login.UserId;
                    AppSetting.CurrentSetting.UserName = login.UserName;
                    AppSetting.CurrentSetting.AccessToken = login.Access_Token;
                    AppSetting.CurrentSetting.Save();

                    var localLogin = Logins.FirstOrDefault(l => l.UserName == username);
                    if (localLogin == null)
                    {
                        localLogin = new LoginModel { UserId= login.UserId,
                            UserName = username,
                            Password = Encrypt(password, username),
                            Access_Token = Encrypt(login.Access_Token, username) };
                        Logins.Add(localLogin);
                    }
                    else
                    {
                        localLogin.Password = Encrypt(password, username);
                        localLogin.Access_Token = Encrypt(login.Access_Token, username);
                    }
                    SaveChanges();
                }
                return login;
            }
            else
            {
                var localLogin = Logins.FirstOrDefault(l => l.UserName == username);
                if (localLogin != null && localLogin.Password == Encrypt( password, username))
                {
                    ApiHelper.SetToken(Decrypt(localLogin.Access_Token, username));

                    AppSetting.CurrentSetting.UserId = localLogin.UserId;
                    AppSetting.CurrentSetting.UserName = localLogin.UserName;
                    AppSetting.CurrentSetting.AccessToken = Decrypt(localLogin.Access_Token, username);
                    AppSetting.CurrentSetting.Save();
                }
                return localLogin;
            }
        }

        private string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public static string Encrypt(string text, string key)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string Decrypt(string cipher, string key)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }
    }
}
