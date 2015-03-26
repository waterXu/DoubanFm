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
using Microsoft.Phone.Info;
using Newtonsoft.Json;
using DouBanFMBase.ViewModel;


namespace DouBanFMBase
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// 标识首次启动时音乐是否在播放
        /// </summary>
        private bool FirstLoadMusicIsPlaying = false;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            //绑定数据源
            DataContext = App.ViewModel;
            bool showMode = false;
           // 获取显示模式 true 为夜间模式
            if (WpStorage.GetIsoSetting(DbFMCommonData.ShowMode) != null)
            {
                showMode = (bool)WpStorage.GetIsoSetting(DbFMCommonData.ShowMode);
            }
            ToggleBtn.IsChecked = showMode;

            App.ViewModel.UpdateTheme();
        }
        #region Page EventHandler Method
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            DbFMCommonData.MainPageLoaded = true;

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
                FirstLoadMusicIsPlaying = true;

            }
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
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
        // Navigated to this Player Page
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CallbackManager.Mainpage = this;
            if (DbFMCommonData.DownLoadedSong)
            {
                App.ViewModel.LocalSongs = DbFMCommonData.DownSongsList;
                Binding localSongs = new Binding();
                localSongs.Path = new PropertyPath("LocalSongs");
                DownSongList.SetBinding(ListBox.ItemsSourceProperty, localSongs);
            }
            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
        }
        // Navigated from this Page
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Stop timer and remove Event Handlers
            BackgroundAudioPlayer.Instance.PlayStateChanged -= new EventHandler(Instance_PlayStateChanged);

            DownSongList.ClearValue(ListBox.ItemsSourceProperty);

           CallbackManager.Mainpage = null;

        }
       
        #endregion

        #region Control EnventHandler
     
        private void All_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            ChannelViewModel cv = lb.SelectedItem as ChannelViewModel;

            DbFMCommonData.SetSongsUrl("n",lb.SelectedIndex.ToString());
            if (FirstLoadMusicIsPlaying)
            {
                FirstLoadMusicIsPlaying = false;
                //启动应用时有音乐在播放则不获取新列表
                return;
            }
            DbFMCommonData.SongFormDown = false;
          
            HttpHelper.GetChannelSongs("n", cv.ChannelId);
            // 保存获取新列表 url 以便给background audio调用
            System.Diagnostics.Debug.WriteLine("Hz名称：" + cv.Name + " Hz 是否收藏" + cv.IsChecked.ToString());

        }
        private void Collect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //WpStorage.SetIsoSetting("ChangeChannels", true);
            ListBox lb = sender as ListBox;
            ChannelViewModel cv = lb.SelectedItem as ChannelViewModel;
            DbFMCommonData.SongFormDown = false;
            //有正在播放的歌曲  保存  获取新列表 url
            HttpHelper.GetChannelSongs("n", cv.ChannelId);
            DbFMCommonData.SetSongsUrl("n", lb.SelectedIndex.ToString());
          
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
            //跳转至用户中心
            MainPiovt.SelectedIndex = 3;
        }
        /// <summary>
        /// 登录账号/切换账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginStatusBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupManager.ShowUserControl(PopupManager.UserControlType.LoginControl);
        }
        private void ChangeUserImgBtn_Click(object sender, RoutedEventArgs e)
        {
            //访问手机照片
        }
        private void DownSongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex == -1)
            {
                return;
            }
            SongInfo songinfo = lb.SelectedItem as SongInfo;
            if (songinfo != null)
            {
                //判断是否多选删除模式  不是则播放该歌曲
                if (!CheckSongs.IsChecked)
                {
                    string tag = JsonConvert.SerializeObject(songinfo);
                    AudioTrack track = new AudioTrack(new Uri(songinfo.url,UriKind.Relative),songinfo.title,songinfo.artist,songinfo.albumtitle,new Uri(songinfo.picture,UriKind.Relative),tag ,EnabledPlayerControls.All);
                    BackgroundAudioPlayer.Instance.Track = track;
                    BackgroundAudioPlayer.Instance.Play();
                    DbFMCommonData.SongFormDown = true;
                }
            }
            //lb.SelectedIndex = -1;
            //lb.SelectedItem = null;
        }

        private void DeleteSongs_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DownSongList.SelectedItems.Count == 0)
            {
                return;
            }

            if (MessageBox.Show("确定要删除选定歌曲？", "", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            SongInfo playingSong = null;
            foreach (SongInfo song in DownSongList.SelectedItems)
            {
                
                //判断是否正在播放下载歌曲
                if (DbFMCommonData.SongFormDown)
                {
                    string playUrl = "";
                    if(BackgroundAudioPlayer.Instance.Track!=null){
                        playUrl = BackgroundAudioPlayer.Instance.Track.Source.ToString().Replace("/","\\");
                    }
                    if (playUrl == song.url)
                    {
                        playingSong = song;
                        //跳出 执行下一个删除
                        continue;
                    }
                }
                string imageType = song.picture.Remove(0, song.picture.Length - 4);
                string localurl = DbFMCommonData.DownSongsIsoName + song.aid + ".mp3";
                string picture = DbFMCommonData.DownSongsIsoName + song.aid + imageType;
                WpStorage.isoFile.DeleteFile(localurl);
                WpStorage.isoFile.DeleteFile(picture);
                DbFMCommonData.DownSongIdList.Remove(song.aid);
                DbFMCommonData.DownSongsList.Remove(song);
            }
            if (playingSong != null)
            {
                //删除当前播放歌曲
                BackgroundAudioPlayer.Instance.Stop();
                BackgroundAudioPlayer.Instance.Track = null;
                string imageType = playingSong.picture.Remove(0, playingSong.picture.Length - 4);
                string localurl = DbFMCommonData.DownSongsIsoName + playingSong.aid + ".mp3";
                string picture = DbFMCommonData.DownSongsIsoName + playingSong.aid + imageType;
                WpStorage.isoFile.DeleteFile(localurl);
                WpStorage.isoFile.DeleteFile(picture);
                DbFMCommonData.DownSongIdList.Remove(playingSong.aid);
                DbFMCommonData.DownSongsList.Remove(playingSong);

                if (DbFMCommonData.DownSongsList.Count > 0)
                {
                    playingSong = DbFMCommonData.DownSongsList[0];
                    string tag = JsonConvert.SerializeObject(playingSong);
                    AudioTrack track = new AudioTrack(new Uri(playingSong.url, UriKind.Relative), playingSong.title, playingSong.artist, playingSong.albumtitle, new Uri(playingSong.picture, UriKind.Relative), tag, EnabledPlayerControls.All);
                    BackgroundAudioPlayer.Instance.Track = track;
                    BackgroundAudioPlayer.Instance.Play();
                }
            }

            App.ViewModel.SaveDownSongs();
            CheckSongs_Click(CheckSongs,new RoutedEventArgs());
            CheckSongs.IsChecked = false;
        }

        private void CheckSongs_Click(object sender, RoutedEventArgs e)
        {
            if (CheckSongs.IsChecked)
            {
                DeleteSongs.Visibility = System.Windows.Visibility.Collapsed;
                Style itemStyle = Resources["ListBoxItemStyle2"] as Style;
                DownSongList.ItemContainerStyle = itemStyle;
                DownSongList.SelectionMode = SelectionMode.Single;
            }
            else
            {
                DeleteSongs.Visibility = System.Windows.Visibility.Visible;
                Style itemStyle = Resources["ListBoxItemStyle1"] as Style;
                DownSongList.ItemContainerStyle = itemStyle;
                DownSongList.SelectionMode = SelectionMode.Multiple;
            }
            DownSongList.SelectedIndex = -1;
            DownSongList.SelectedItem = null;
        }

        private void LoadChannelGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LoadChannelGrid.Visibility = System.Windows.Visibility.Collapsed;
            HttpHelper.GetChannelList();
        }
        private void AddTheme_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void ThemeSetting_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MainPiovt.SelectedIndex = 3;
            SettingScroll.ScrollToVerticalOffset(0);
        }
        private void aboutTile_Loaded(object sender, RoutedEventArgs e)
        {
            this.trexStoryboard.Begin();
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
              
                App.ViewModel.LoginSuccess = DbFMCommonData.loginSuccess;
                Binding channels = new Binding();
                channels.Path = new PropertyPath("Channels");
                AllChannels.SetBinding(ListBox.ItemsSourceProperty,channels);
                Binding collectChannels = new Binding();
                collectChannels.Path = new PropertyPath("CollectChannels");
                CollectChannels.SetBinding(ListBox.ItemsSourceProperty, collectChannels);
                if (App.ViewModel.Channels.Count > 0)
                {
                    int channelIndex = 0;
                    if (FirstLoadMusicIsPlaying && WpStorage.GetIsoSetting("LastedChannelId") != null)
                    {
                        string index = WpStorage.GetIsoSetting("LastedChannelId").ToString();
                        channelIndex = Convert.ToInt32(index);
                        if (channelIndex == -1)
                        {
                            channelIndex = 0;
                        }
                    }
                    AllChannels.SelectedIndex = channelIndex;
                }
            });
           
        }
        public void DataContextLoadedFail()
        {
            //添加 重新加载按钮
            //。。。。
            this.Dispatcher.BeginInvoke(() => 
            {
                LoadChannelGrid.Visibility = System.Windows.Visibility.Visible;
            });
        }

        public void GetSongSuccess(int p)
        {
            //标识切换频道 请求play
            WpStorage.CreateFile("ChangeChannels");
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
                    PopupManager.OffPopUp();
                });
            }

        }
        public void DownSongBack(bool isSuccess)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                App.ViewModel.LocalSongs = DbFMCommonData.DownSongsList;
                Binding localSongs = new Binding();
                localSongs.Path = new PropertyPath("LocalSongs");
                DownSongList.SetBinding(ListBox.ItemsSourceProperty, localSongs);
            });
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            long memory = DeviceStatus.ApplicationCurrentMemoryUsage / 1048576;
            long memoryLimit= DeviceStatus.ApplicationMemoryUsageLimit / 1048576;
            long memoryMax = DeviceStatus.ApplicationPeakMemoryUsage / 1048576;
            MessageBox.Show("当前内存使用情况："+memory.ToString() + " MB 当前最大内存使用情况： "+memoryMax.ToString()+ "MB  当前可分配最大内存： " + memoryLimit.ToString()+"  MB");
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