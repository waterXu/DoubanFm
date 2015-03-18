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
using Microsoft.Phone.BackgroundAudio;
using DouBanAudioAgent;

namespace DouBanFMBase
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
           
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

        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(()=>
            {
                 if (BackgroundAudioPlayer.Instance.Track != null)
                {
                    // show soung 
                    SongName.Text = BackgroundAudioPlayer.Instance.Track.Title;
                    SongArtist.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                    if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
                    {
                        PlayBtn.IsChecked = false;
                    }
                    else
                    {
                        PlayBtn.IsChecked = true;
                    }
                }
            });
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

            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                // show soung 
                SongName.Text = BackgroundAudioPlayer.Instance.Track.Title;
                SongArtist.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
                {
                    PlayBtn.IsChecked = false;
                }
                else
                {
                    PlayBtn.IsChecked = true;
                }
            }
        }
    
        /// <summary>
        /// 切换夜间/白天模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            System.Diagnostics.Debug.WriteLine("Hz名称：" + cv.Name+ " Hz 是否收藏"+cv.IsChecked.ToString());

            WpStorage.SetIsoSetting("ChangeChannels", true);
            object o = WpStorage.GetIsoSetting("ChangeChannels");
            if (DbFMCommonData.IsFirstLoadSongs)
            {
                HttpHelper.GetChannelSongs("n",cv.ChannelId);
                DbFMCommonData.SetSongsUrl("n", cv.ChannelId);
                DbFMCommonData.IsFirstLoadSongs = false;
            }
            else
            {
                //有正在播放的歌曲  保存  获取新列表 url
                HttpHelper.GetChannelSongs("n", cv.ChannelId);
                DbFMCommonData.SetSongsUrl("n", cv.ChannelId);
            }
          
        }
        private void Collect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WpStorage.SetIsoSetting("ChangeChannels", true);
            ListBox lb = sender as ListBox;
            ChannelViewModel cv = lb.SelectedItem as ChannelViewModel;
            //有正在播放的歌曲  保存  获取新列表 url
            HttpHelper.GetChannelSongs("n", cv.ChannelId);
            DbFMCommonData.SetSongsUrl("n", cv.ChannelId);
        }
        public void GetSongSuccess(int p)
        {
            BackgroundAudioPlayer.Instance.Play();
        }
        public void GetSongFail()
        {
            //....show err
            //SongGrid.DataContext = DbFMCommonData.PlayingSongs[playIndex];
        }
        private void Forward_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipNext();
        }

        private void SongInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri(DbFMCommonData.MusicPageUrl, UriKind.RelativeOrAbsolute));
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("** Player Page -> call Play");
            if (PlayBtn.IsChecked)
            {
                BackgroundAudioPlayer.Instance.Play();
            }
            else
            {
                BackgroundAudioPlayer.Instance.Pause();
            }
        }
        private void UserImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PopupManager.ShowUserControl(PopupManager.UserControlType.LoginControl);
        }
        /// <summary>
        /// 登录成功
        /// </summary>
        /// <param name="isSuccess"></param>
        public void UserLoginSuccess(bool isSuccess)
        {
            if (isSuccess)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    showUserName.Text = DbFMCommonData.NickName;
                    PopupManager.OffPopUp();
                });
            }
           
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