using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Microsoft.Phone.Shell;
using System.IO;
using DouBanAudioAgent;
using DouBanFMBase.Resources;

namespace DouBanFMBase.PopUp
{
    public partial class DbLoginControl : UserControl
    {
        public DbLoginControl()
        {
            InitializeComponent();
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox input = sender as TextBox;
            if (input == null)
            {
                return;
            }
            if (input.Text == AppResources.AccountTip)
            {
                input.Text = "";
            }
            PopupManager.Input_GotFocus((Control)sender,this.LayoutRoot);
        }
        private void PassWordInput_GotFocus(object sender, RoutedEventArgs e)
        {
            PopupManager.Input_GotFocus((Control)sender, this.LayoutRoot);
        }
        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            PopupManager.Input_LostFocus(this.LayoutRoot);
        }
        string username = null;
        string password = null;
        bool isLogining = false;
        private void DbFMLogin_Click(object sender, RoutedEventArgs e)
        {
            if (isLogining)
            {
                return;
            }
            else
            {
                isLogining = true;
            }
            username = DbFmAccount.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show(AppResources.AccountEmpty);
                return;
            }
            password = DbFmPassword.Password.Trim();
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show(AppResources.PasswordEmpty);
                return;
            }
            string loginUrlInfo = DbFMCommonData.LoginUrl + "?app_name=" + DbFMCommonData.AppName + "&version=" + DbFMCommonData.Version;
            loginUrlInfo += "&email=" + username + "&password=" + password;
            System.Diagnostics.Debug.WriteLine("登录请求url：" + loginUrlInfo);
            HttpHelper.httpGet(loginUrlInfo, new AsyncCallback(LoginResult));
        }
        private void LoginResult(IAsyncResult ar)
        {
            this.Dispatcher.BeginInvoke(() => 
            {
                try
                {
                    if (HttpHelper.LoginResultCodeInfo(ar))
                    {
                        WpStorage.SetIsoSetting(DbFMCommonData.UserName, username);
                        WpStorage.SetIsoSetting(DbFMCommonData.Password, password);
                        DbFMCommonData.loginSuccess = true;
                    }
                    else
                    {
                        //todo
                        //MessageBox.Show("用户名或密码错误");
                    }
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.Login, DbFMCommonData.loginSuccess);
                }
                catch
                {
                    MessageBox.Show("登录失败，请检查网络设置");
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.Login, false);
                    //MessageBox.Show("登录失败，请检查网络设置");
                }
                isLogining = false;
            });
        }

        private void RegisterAccount_Click(object sender, RoutedEventArgs e)
        {
            //string registerUrl = DbFMCommonData.RegisterUrl + "?app_name=radio_desktop_win&version=100";
            //System.Diagnostics.Debug.WriteLine("注册页面请求url：" + registerUrl);
            //try
            //{
            //    HttpHelper.httpGet(registerUrl, new AsyncCallback((ar) =>
            //    {
            //        WebResponse response = ((HttpWebRequest)ar.AsyncState).EndGetResponse(ar);
            //        Stream stream = response.GetResponseStream();
            //        byte[] data = new byte[stream.Length];
            //        stream.Read(data, 0, (int)stream.Length);
            //        string result = System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length);
            //        if (!string.IsNullOrEmpty(result))
            //        {
            //            WebRegisterControl.registerHtml = result;
            //            this.Dispatcher.BeginInvoke(() =>
            //            {
            //                PopupManager.ShowUserControl(PopupManager.UserControlType.WebRegisterControl);
            //            });
            //        }
            //    }));
            //}
            //catch
            //{
            //    MessageBox.Show("注册请求失败，请确保网络设置正常");
            //}
            PopupManager.ShowUserControl(PopupManager.UserControlType.RegisterControl);
        }

        private void ForgetPwd_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}
