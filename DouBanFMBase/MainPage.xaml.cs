using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using DouBanFMBase.Resources;
using DouBanFMBase.PopUp;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace DouBanFMBase
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
           
            //显示模式  默认为 day
            string showMode = "day";
            string themeBgPath = DbFMCommonData.DefaultDayTheme;
            if (WpStorage.GetIsoSetting(DbFMCommonData.ThemePath) != null)
            {
                themeBgPath = WpStorage.GetIsoSetting(DbFMCommonData.ThemePath).ToString();
            }
            //获取显示模式 day or night
            if (WpStorage.GetIsoSetting(DbFMCommonData.ShowMode) != null)
            {
                showMode = WpStorage.GetIsoSetting(DbFMCommonData.ShowMode).ToString();
            }
            ImageSource image = new BitmapImage(new Uri(DbFMCommonData.MoonPngPath, UriKind.RelativeOrAbsolute));
            ImageSource bgImage;
            switch (showMode)
            {
                case "day":
                    if (!themeBgPath.Equals(DbFMCommonData.DefaultDayTheme))
                    {
                        bgImage = new BitmapImage(new Uri(themeBgPath, UriKind.RelativeOrAbsolute));
                        bgBrush.ImageSource = bgImage;
                        LayoutRoot.Background = bgBrush;
                    }
                    break;
                case "night":
                    image = new BitmapImage(new Uri(DbFMCommonData.SunPngPath, UriKind.RelativeOrAbsolute));
                    //判断是否自定义主题
                    //todo
                    bgImage = new BitmapImage(new Uri(DbFMCommonData.DefaultNightTheme, UriKind.RelativeOrAbsolute));
                    bgBrush.ImageSource = bgImage;
                    LayoutRoot.Background = bgBrush;
                    break;
                default:
                    break;
            }
            ellipseBrush.Stretch = Stretch.Uniform;
            ellipseBrush.ImageSource = image;
            ShowModeEllipse.Tag = showMode;
            ShowModeEllipse.Fill = ellipseBrush;
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }

        public void DataContextLoaded()
        {
            this.Dispatcher.BeginInvoke(() => 
            {
                DataContext = App.ViewModel;
                Binding channels = new Binding();
                channels.Path = new PropertyPath("Channels");
                AllChannels.SetBinding(ListBox.ItemsSourceProperty,channels);
                Binding collectChannels = new Binding();
                collectChannels.Path = new PropertyPath("CollectChannels");
                CollectChannels.SetBinding(ListBox.ItemsSourceProperty, collectChannels);
                if (App.ViewModel.Channels.Count > 0)
                {
                    AllChannels.SelectedIndex = 0;
                }
            });
           
        }
        public void DataContextLoadedFail()
        {
            //添加 重新加载按钮
            //。。。。
            throw new NotImplementedException();
        }
        ImageBrush ellipseBrush = new ImageBrush();
        ImageBrush bgBrush = new ImageBrush();
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DbFMCommonData.NickName))
            {
                showUserName.Text = DbFMCommonData.NickName;
            }
            CallbackManager.Mainpage = this;
            DbFMCommonData.MainPageLoaded = true;
            if (App.ViewModel.IsLoaded)
            {
                if (DbFMCommonData.DownLoadSuccess)
                {
                    DataContextLoaded();
                }
                else
                {
                    DataContextLoadedFail();

                }
            }
            //if (string.IsNullOrEmpty(DbFMCommonData.UserName))
            //{
            //    showUserName.Text = "请先登录";
            //}
            //else
            //{
            //    showUserName.Text = DbFMCommonData.NickName;
            //}
            ////显示模式  默认为 day
            //string showMode = "day";
            //string themeBgPath = DbFMCommonData.DefaultDayTheme;
            //if (WpStorage.GetIsoSetting(DbFMCommonData.ThemePath) != null)
            //{
            //    themeBgPath = WpStorage.GetIsoSetting(DbFMCommonData.ThemePath).ToString();
            //}
            ////获取显示模式 day or night
            //if (WpStorage.GetIsoSetting(DbFMCommonData.ShowMode) != null)
            //{
            //    showMode = WpStorage.GetIsoSetting(DbFMCommonData.ShowMode).ToString();
            //}
            //ImageSource image = new BitmapImage(new Uri("/Images/night.png", UriKind.RelativeOrAbsolute));
            //ImageSource bgImage;
            //switch (showMode)
            //{
            //    case "day":
            //        if (!themeBgPath.Equals(DbFMCommonData.DefaultDayTheme))
            //        {
            //            bgImage = new BitmapImage(new Uri(themeBgPath, UriKind.RelativeOrAbsolute));
            //            bgBrush.ImageSource = bgImage;
            //            LayoutRoot.Background = bgBrush;
            //        }
            //        break;
            //    case "night":
            //        image = new BitmapImage(new Uri("/Images/day.png", UriKind.RelativeOrAbsolute));
            //        //判断是否自定义主题
            //        //todo
            //        bgImage = new BitmapImage(new Uri(DbFMCommonData.DefaultNightTheme, UriKind.RelativeOrAbsolute));
            //        bgBrush.ImageSource = bgImage;
            //        LayoutRoot.Background = bgBrush;
            //        break;
            //    default:
            //        break;
            //}
            //ellipseBrush.Stretch = Stretch.Uniform;
            //ellipseBrush.ImageSource = image;
            //ShowModeEllipse.Tag = showMode;
            //ShowModeEllipse.Fill = ellipseBrush;
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
                if (MessageBox.Show("确定要退出应用？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Application.Current.Terminate();
                }
            }
        }
        private void ChanngeBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Ellipse_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ImageSource image;
            ImageSource bgImage;
            string showMode = ShowModeEllipse.Tag.ToString();
            switch (showMode)
            {
                case "day":
                    image = new BitmapImage(new Uri(DbFMCommonData.SunPngPath, UriKind.RelativeOrAbsolute));
                    showMode = "night";
                    //判断是否自定义主题
                    //todo
                    bgImage = new BitmapImage(new Uri(DbFMCommonData.DefaultNightTheme, UriKind.RelativeOrAbsolute));
                    bgBrush.ImageSource = bgImage;
                    LayoutRoot.Background = bgBrush;
                    break;
                case "night":
                    image = new BitmapImage(new Uri(DbFMCommonData.MoonPngPath, UriKind.RelativeOrAbsolute));
                    showMode = "day";
                    bgImage = new BitmapImage(new Uri(DbFMCommonData.DefaultDayTheme, UriKind.RelativeOrAbsolute));
                    bgBrush.ImageSource = bgImage;
                    LayoutRoot.Background = bgBrush;
                    break;
                default:
                    image = new BitmapImage(new Uri(DbFMCommonData.SunPngPath, UriKind.RelativeOrAbsolute));
                    break;
            }
            ellipseBrush.ImageSource = image;
            ShowModeEllipse.Fill = ellipseBrush;
            ShowModeEllipse.Tag = showMode;
            WpStorage.SetIsoSetting(DbFMCommonData.ShowMode,showMode);
        }

        private void All_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            ChannelViewModel cv = lb.SelectedItem as ChannelViewModel;
            //获取新歌曲
            HttpHelper.GetChannelSongs("n",cv.ChannelId);
            System.Diagnostics.Debug.WriteLine("Hz名称：" + cv.Name+ " Hz 是否收藏"+cv.IsChecked.ToString());
        }
        public void GetSongSuccess(int playIndex)
        {
            SongGrid.DataContext = DbFMCommonData.PlayingSongs[playIndex];
        }
        public void GetSongFail()
        {
            //....show err
            //SongGrid.DataContext = DbFMCommonData.PlayingSongs[playIndex];
        }
        private void Forward_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void SongInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DouBanFMBase;component/MusicPage.xaml", UriKind.RelativeOrAbsolute));
        }
     
        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

     
    }
}