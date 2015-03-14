﻿using System;
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
            PopupManager.Input_GotFocus((Control)sender,this.LayoutRoot);
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
                MessageBox.Show("用户名不能为空");
                return;
            }
            password = DbFmPassword.Password.Trim();
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            string loginUrlInfo = DbFMCommonData.LoginUrl+"?app_name=radio_desktop_win&version=100";
            loginUrlInfo += "&email=" + username + "&password=" + password;
            System.Diagnostics.Debug.WriteLine("登录请求url：" + loginUrlInfo);
            HttpHelper.httpGet(loginUrlInfo, new AsyncCallback(LoginResult));
        }
        private void LoginResult(IAsyncResult ar)
        {
            Dictionary<string, string> backInfo = new Dictionary<string, string>();
            string resultMsg = null;
            this.Dispatcher.BeginInvoke(() => 
            {
                try
                {
                    if (HttpHelper.LoginResultCodeInfo(ar))
                    {
                        WpStorage.SetIsoSetting(DbFMCommonData.UserName, username);
                        WpStorage.SetIsoSetting(DbFMCommonData.Password, password);
                        backInfo.Add("code", "0");
                        resultMsg = JsonConvert.SerializeObject(backInfo);
                        DbFMCommonData.loginSuccess = true;
                        DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.Login, resultMsg);
                    }
                    else
                    {
                        //MessageBox.Show("用户名或密码错误");
                    }
                }
                catch
                {
                    backInfo.Add("code", "-1");
                    backInfo.Add("err", "登录失败，请检查网络设置");
                    resultMsg = JsonConvert.SerializeObject(backInfo);
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.Login, resultMsg);
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
