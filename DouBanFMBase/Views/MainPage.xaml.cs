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
            UpdateTheme();
        }
        #region Page EventHandler Method
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
        #endregion

        #region Control EnventHandler
        private void ToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            bool showMode = ToggleBtn.IsChecked ? false : true;
            WpStorage.SetIsoSetting(DbFMCommonData.ShowMode, showMode);
            UpdateTheme();
        }
        private void All_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            ChannelViewModel cv = lb.SelectedItem as ChannelViewModel;
            System.Diagnostics.Debug.WriteLine("Hz名称：" + cv.Name + " Hz 是否收藏" + cv.IsChecked.ToString());

            WpStorage.SetIsoSetting("ChangeChannels", true);
            object o = WpStorage.GetIsoSetting("ChangeChannels");
            if (DbFMCommonData.IsFirstLoadSongs)
            {
                HttpHelper.GetChannelSongs("n", cv.ChannelId);
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
        private void Forward_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipNext();
        }

        private void SongInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                this.NavigationService.Navigate(new Uri(DbFMCommonData.MusicPageUrl, UriKind.RelativeOrAbsolute));
            }
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
        #endregion
        #region Audio Method

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
        #endregion

        #region hleper Method

        public void UpdateTheme()
        {
            ImageBrush bgBrush = new ImageBrush();
            //获取主题路径
            string themeBgPath = DbFMCommonData.DefaultDayTheme;
            if (WpStorage.GetIsoSetting(DbFMCommonData.ThemePath) != null)
            {
                themeBgPath = WpStorage.GetIsoSetting(DbFMCommonData.ThemePath).ToString();
            }

            ImageSource bgImage;
            bool showMode = false;

            //获取显示模式 true 为夜间模式
            if (WpStorage.GetIsoSetting(DbFMCommonData.ShowMode) != null)
            {
                showMode = (bool)WpStorage.GetIsoSetting(DbFMCommonData.ShowMode);
            }

            if (showMode)
            {
                bgImage = new BitmapImage(new Uri(DbFMCommonData.DefaultNightTheme, UriKind.RelativeOrAbsolute));
                bgBrush.ImageSource = bgImage;
                LayoutRoot.Background = bgBrush;
            }
            else
            {
                bgImage = new BitmapImage(new Uri(themeBgPath, UriKind.RelativeOrAbsolute));
                bgBrush.ImageSource = bgImage;
                LayoutRoot.Background = bgBrush;
            }
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

        public void GetSongSuccess(int p)
        {
            BackgroundAudioPlayer.Instance.Play();
        }
        public void GetSongFail()
        {
            //....show err
            //SongGrid.DataContext = DbFMCommonData.PlayingSongs[playIndex];
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

        #endregion

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