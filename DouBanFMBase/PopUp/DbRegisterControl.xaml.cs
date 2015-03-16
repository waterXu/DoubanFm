using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;

namespace DouBanFMBase.PopUp
{
    public partial class DbRegisterControl : UserControl
    {
        public DbRegisterControl()
        {
            InitializeComponent();
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            PopupManager.Input_GotFocus((Control)sender, this.LayoutRoot);
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            PopupManager.Input_LostFocus(this.LayoutRoot);
        }

        private void HaveAccount_Click(object sender, RoutedEventArgs e)
        {
            PopupManager.ShowUserControl(PopupManager.UserControlType.LoginControl);
        }
        string userName;
        string password;
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            //user_name=xuyanlan1&password=xuyanlan1&confirm_password=xuyanlan1&captcha_solution=country&captcha_id=o5ehRbcmiHsF9i2QeM9kmxgt%3Aen&agreement=on
            userName = Account.Text.Trim();
            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("用户名不能为空");
                return;
            }
            password = AccountPwdInput.Password.Trim();
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            string repassword = ReAccountPwdInput.Password.Trim();
            if (string.IsNullOrEmpty(repassword))
            {
                MessageBox.Show("请重复密码");
                return;
            }
            if (!password.Equals(repassword))
            {
                MessageBox.Show("密码不一致");
                return;
            }
            string registerUrl = DbFMCommonData.RegisterUrl + "?app_name=" + DbFMCommonData.AppName + "&version=" + DbFMCommonData.Version;
             registerUrl +="&user_name=" + userName;
            registerUrl += "&password=" + password + "&confirm_password=" + repassword;
            registerUrl += "&agreement=on";
            System.Diagnostics.Debug.WriteLine("注册请求url：" + registerUrl);
            HttpHelper.httpPost(registerUrl, new AsyncCallback(RegisterResult));
        }
        private void RegisterResult(IAsyncResult ar)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    if (HttpHelper.LoginResultCodeInfo(ar))
                    {
                        WpStorage.SetIsoSetting(DbFMCommonData.UserName, userName);
                        WpStorage.SetIsoSetting(DbFMCommonData.Password, password);
                        DbFMCommonData.loginSuccess = true;
                        DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.Login, true);
                    }
                    else
                    {
                        //MessageBox.Show("用户名或密码错误");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("登录失败，请检查网络设置");
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.Login, false);
                    //MessageBox.Show("登录失败，请检查网络设置");
                }
            });
        }

        private void ShowLicense_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void DeleteImg_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PopupManager.OffPopUp();
        }
    }
}
