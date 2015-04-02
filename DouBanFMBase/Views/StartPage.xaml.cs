using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Threading;
using DouBanFMBase.PopUp;
using Newtonsoft.Json;
using DouBanFMBase.Resources;
using DouBanAudioAgent;

namespace DouBanFMBase
{
    public partial class StartPage : PhoneApplicationPage
    {
        //private DispatcherTimer BgImgTimer = new DispatcherTimer();
        public StartPage()
        {
            InitializeComponent();
            App.GetNetName();
            //开始异步获取全部赫兹
            HttpHelper.GetChannelList();
            DbFMCommonData.informCallback = CallbackManager.CallBackTrigger;
            DbFMCommonData.CollectHashSet = new HashSet<string>();
        }
        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            CallbackManager.Startpage = this;
            if (WpStorage.GetIsoSetting(DbFMCommonData.IsFirstUse) == null)
            {
                WpStorage.SetIsoSetting(DbFMCommonData.IsFirstUse,true);
            }
            else
            {
                bgImg.Visibility = System.Windows.Visibility.Visible;
                firstStartGrid.Visibility = System.Windows.Visibility.Collapsed;
                //BgImgTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
                //BgImgTimer.Tick += new EventHandler(BgImgTimer_Tick);
                //BgImgTimer.Start();
                BgImgTimer_Tick();
            }
        }
        private void BgImgTimer_Tick()
        {
            try
            {
                if (WpStorage.GetIsoSetting(DbFMCommonData.UserName) != null && WpStorage.GetIsoSetting(DbFMCommonData.Password) != null)
                {
                    string username = WpStorage.GetIsoSetting(DbFMCommonData.UserName).ToString();
                    string password = WpStorage.GetIsoSetting(DbFMCommonData.Password).ToString();
                    string loginUrlInfo = DbFMCommonData.LoginUrl + "?app_name=" + DbFMCommonData.AppName + "&version=" + DbFMCommonData.Version;
                    loginUrlInfo += "&email=" + username + "&password=" + password;
                    System.Diagnostics.Debug.WriteLine("登录请求url：" + loginUrlInfo);
                    HttpHelper.httpGet(loginUrlInfo, new AsyncCallback((ar) =>
                    {
                        if (HttpHelper.LoginResultCodeInfo(ar))
                        {
                            WpStorage.SetIsoSetting(DbFMCommonData.UserName, username);
                            WpStorage.SetIsoSetting(DbFMCommonData.Password, password);
                        }
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.NavigationService.Navigate(new Uri(DbFMCommonData.MianPageUrl, UriKind.RelativeOrAbsolute));
                        });
                    }));
                    //登录验证
                    //验证成功 保存token

                }
                else
                {
                    //没有登录成功信息 跳转首页 显示登陆框
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.NavigationService.Navigate(new Uri(DbFMCommonData.MianPageUrl, UriKind.RelativeOrAbsolute));
                    });
                }
            }
            catch
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.NavigationService.Navigate(new Uri(DbFMCommonData.MianPageUrl, UriKind.RelativeOrAbsolute));
                });
            }
           
        }
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = pivotItems.SelectedIndex;
            if (index > -1)
            {
                switch(index)
                {
                    case 0:
                        ellipse0.Fill = new SolidColorBrush(Color.FromArgb(100, 230, 40, 40));
                        ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse2.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse3.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        break;
                    case 1:
                        ellipse0.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100, 230, 40, 40));
                        ellipse2.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse3.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        break;
                    case 2:
                        ellipse0.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse2.Fill = new SolidColorBrush(Color.FromArgb(100, 230, 40, 40));
                        ellipse3.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        break;
                    case 3:
                        ellipse0.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse2.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse3.Fill = new SolidColorBrush(Color.FromArgb(100, 230, 40, 40));
                        break;
                    default:
                        break;

                }
            }

        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            //DbFMCommonData.informCallback = LoginResult;
            PopupManager.ShowUserControl(PopupManager.UserControlType.LoginControl);
            //this.NavigationService.Navigate(new Uri("/DouBanFMBase;component/UserLogin.xaml", UriKind.RelativeOrAbsolute));
        }

        public void LoginResult(bool isSuccess)
        {
            this.Dispatcher.BeginInvoke(() => 
            {
                if (isSuccess)
                {
                    this.NavigationService.Navigate(new Uri(DbFMCommonData.MianPageUrl, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    this.NavigationService.Navigate(new Uri(DbFMCommonData.MianPageUrl, UriKind.RelativeOrAbsolute));
                }
                PopupManager.OffPopUp();
            });
            
        }
        private void ExperienceBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri(DbFMCommonData.MianPageUrl, UriKind.RelativeOrAbsolute));
        }
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PopupManager._popUp != null)
            {
                PopupManager.OffPopUp();
                e.Cancel = true;
            }
            else
            {
                if(MessageBox.Show("确定要退出应用？","",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Application.Current.Terminate(); 
                }
            }
        }

    }
}