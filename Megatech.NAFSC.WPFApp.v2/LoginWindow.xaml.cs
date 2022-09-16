using Megatech.NAFSC.WPFApp.Global;
using Megatech.NAFSC.WPFApp.Helpers;
using Megatech.NAFSC.WPFApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFTabTip;

namespace Megatech.NAFSC.WPFApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            TabTipAutomation.IgnoreHardwareKeyboard = HardwareKeyboardIgnoreOptions.IgnoreAll;
            TabTipAutomation.BindTo<TextBox>();
            TabTipAutomation.BindTo<PasswordBox>();
            txtUserName.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //ApiHelper helper = new ApiHelper();
            btnLogin.IsEnabled = false;
            var login = new LoginContext().Login(txtUserName.Text, txtPassword.Password);
            btnLogin.IsEnabled = true;
            if (login != null)
            {
                                
                ApiHelper.SetToken(login.Access_Token);

                AppSetting.CurrentSetting.UserId = login.UserId;
                AppSetting.CurrentSetting.UserName = login.UserName;
                AppSetting.CurrentSetting.AccessToken= login.Access_Token;
                AppSetting.CurrentSetting.Save();
                this.DialogResult = true;
                this.Close();
            }
            else
                MessageBox.Show(this.FindResource("login_error_msg").ToString(), this.FindResource("login_error_title").ToString(),MessageBoxButton.OK,MessageBoxImage.Error);



        }
    }
}
