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
using System.IO.IsolatedStorage;
using System.IO;
using System.Threading;


namespace DouBanFMBase
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// 标识首次启动时音乐是否在播放
        /// </summary>
        public static bool FirstLoadMusicIsPlaying = false;
        /// <summary>
        /// 是否从专辑页面返回
        /// </summary>
        public static bool IsFromMusicPage = false;
        public static int LastPivotIndex = 0;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
        }
        #region Page EventHandler Method
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsFromMusicPage)
            {
                DbFMCommonData.MainPageLoaded = true;
               
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
                App.ViewModel.InitPropertyValue();
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
                if (!App.ViewModel.OffExitTip)
                {
                    if (MessageBox.Show(AppResources.ConfirmExit, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        if (App.ViewModel.ExitStopMusic && BackgroundAudioPlayer.Instance.Track != null)
                        {
                            BackgroundAudioPlayer.Instance.Stop();
                            BackgroundAudioPlayer.Instance.Track = null;
                        }
                        Application.Current.Terminate();
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    if (App.ViewModel.ExitStopMusic && BackgroundAudioPlayer.Instance.Track != null)
                    {
                        BackgroundAudioPlayer.Instance.Stop();
                        BackgroundAudioPlayer.Instance.Track = null;
                    }
                    Application.Current.Terminate();
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
            if (App.ViewModel.Channels == null || App.ViewModel.Channels.Count == 0) 
            {
                //从新获取数据
                LoadChannelGrid.Visibility = System.Windows.Visibility.Visible;
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
           IsFromMusicPage = false;
        }
       
        #endregion

        #region Control EnventHandler
        //标识是否从不能收听红心赫兹改变频道
        private bool IsFormLoveChannel = false;
        private void All_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //启动应用时有音乐在播放则不获取新列表
            if (FirstLoadMusicIsPlaying)
            {
                FirstLoadMusicIsPlaying = false;
                return;
            }
            if (DbFMCommonData.NetworkStatus != null && DbFMCommonData.NetworkStatus == "None")
            {
                AllChannels.SelectedIndex = DbFMCommonData.LastedIndex;
                App.ShowToast(AppResources.OperationError);
                return;
            }
            else if (DbFMCommonData.NetworkStatus != "WiFi")
            {
                App.ShowToast(AppResources.CostTip);
            }
            if (IsFormLoveChannel)
            {
                IsFormLoveChannel = false;
                return;
            }
           
            ListBox lb = sender as ListBox;
            ChannelViewModel cv = lb.SelectedItem as ChannelViewModel;
            if (cv == null)
            {
                return;
            }
            //处理未登录用户 收听红心hz
            if (!DbFMCommonData.loginSuccess && cv.ChannelId == DbFMCommonData.HotChannelId)
            {
                MessageBox.Show(AppResources.ListenLoveRadioTip);
                IsFormLoveChannel = true;
                AllChannels.SelectedIndex = DbFMCommonData.LastedIndex;
                return;
            }
            WpStorage.SetIsoSetting("LastedPlayPivotIndex",0);
            if (WpStorage.isoFile.FileExists(DbFMCommonData.SongFormDown))
            {
                WpStorage.isoFile.DeleteFile(DbFMCommonData.SongFormDown);
            }
            HttpHelper.OperationChannelSongs("n", cv.ChannelId);
            // 保存获取新列表 url 以便给background audio调用
            DbFMCommonData.SetSongsUrl("p", cv.ChannelId, lb.SelectedIndex);
            System.Diagnostics.Debug.WriteLine("Hz名称：" + cv.Name + " Hz 是否收藏" + cv.IsChecked.ToString());
            DbFMCommonData.LastedIndex = lb.SelectedIndex;
        }
        private void Collect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //启动应用时有音乐在播放则不获取新列表
            if (FirstLoadMusicIsPlaying)
            {
                FirstLoadMusicIsPlaying = false;
                return;
            }
            if (DbFMCommonData.NetworkStatus != null && DbFMCommonData.NetworkStatus == "None")
            {
                CollectChannels.SelectedIndex = DbFMCommonData.LastedCollectIndex;
                App.ShowToast(AppResources.OperationError);
                return;
            }else if (DbFMCommonData.NetworkStatus != "WiFi")
            {
                App.ShowToast(AppResources.CostTip);
            }
            if (IsFormLoveChannel)
            {
                IsFormLoveChannel = false;
                return;
            }
            //WpStorage.SetIsoSetting("ChangeChannels", true);
            ListBox lb = sender as ListBox;
            ChannelViewModel cv = lb.SelectedItem as ChannelViewModel;
            if (cv == null)
            {
                return;
            }
            //处理未登录用户 收听红心hz
            if (!DbFMCommonData.loginSuccess && cv.ChannelId == DbFMCommonData.HotChannelId)
            {
                MessageBox.Show(AppResources.ListenLoveRadioTip);
                IsFormLoveChannel = true;
                CollectChannels.SelectedIndex = DbFMCommonData.LastedCollectIndex;
                return;
            }
            if (WpStorage.isoFile.FileExists(DbFMCommonData.SongFormDown))
            {
                WpStorage.isoFile.DeleteFile(DbFMCommonData.SongFormDown);
            }
            WpStorage.SetIsoSetting("LastedPlayPivotIndex", 1);
            //保存获取新列表 url
            HttpHelper.OperationChannelSongs("n", cv.ChannelId);
            DbFMCommonData.SetSongsUrl("p", cv.ChannelId, lb.SelectedIndex);
            DbFMCommonData.LastedCollectIndex = lb.SelectedIndex;

        }
        private void Forward_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                BackgroundAudioPlayer.Instance.SkipNext();
            }
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
                    if (WpStorage.isoFile.FileExists(songinfo.url)) 
                    {
                        WpStorage.SetIsoSetting("LastedPlayPivotIndex", 2);
                        AllChannels.SelectedIndex = -1;
                        CollectChannels.SelectedIndex = -1;
                        WpStorage.SaveStringToIsoStore(DbFMCommonData.SongFormDown, songinfo.sid);
                        BackgroundAudioPlayer.Instance.Play();
                    }
                    else
                    {
                        if(WpStorage.isoFile.FileExists(songinfo.picture))
                            WpStorage.isoFile.DeleteFile(songinfo.picture);
                        DbFMCommonData.DownSongIdList.Remove(songinfo.sid);
                        DbFMCommonData.DownSongsList.Remove(songinfo);
                        App.ShowToast(AppResources.PlayLocalSongError);
                        App.ViewModel.SaveDownSongs();
                    }
                }
            }
        }

        private void DeleteSongs_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DownSongList.SelectedItems.Count == 0)
            {
                return;
            }

            if (MessageBox.Show(AppResources.DeleteSongsTip, "", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            SongInfo playingSong = null;
            foreach (SongInfo song in DownSongList.SelectedItems)
            {
                //判断是否正在播放下载歌曲
                if (WpStorage.isoFile.FileExists(DbFMCommonData.SongFormDown))
                {
                    string playUrl = "";
                    if(BackgroundAudioPlayer.Instance.Track!=null)
                    {
                        playUrl = BackgroundAudioPlayer.Instance.Track.Source.ToString().Replace("/","\\");
                    }
                    if (playUrl == song.url)
                    {
                        playingSong = song;
                        //跳出 执行下一个删除
                        continue;
                    }
                }
                WpStorage.isoFile.DeleteFile(song.url);
                WpStorage.isoFile.DeleteFile(song.picture);
                DbFMCommonData.DownSongIdList.Remove(song.sid);
                DbFMCommonData.DownSongsList.Remove(song);
            }
            if (playingSong != null)
            {
                //删除当前播放歌曲
                BackgroundAudioPlayer.Instance.Stop();
                BackgroundAudioPlayer.Instance.Track = null;
                WpStorage.isoFile.DeleteFile(playingSong.url);
                WpStorage.isoFile.DeleteFile(playingSong.picture);
                DbFMCommonData.DownSongIdList.Remove(playingSong.sid);
                DbFMCommonData.DownSongsList.Remove(playingSong);

                if (DbFMCommonData.DownSongsList.Count > 0)
                {
                    playingSong = DbFMCommonData.DownSongsList[0];
                    WpStorage.SaveStringToIsoStore(DbFMCommonData.SongFormDown, playingSong.sid);
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

        private void ThemeSetting_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MainPiovt.SelectedIndex = 3;
            SettingScroll.ScrollToVerticalOffset(0);
        }
        private void aboutTile_Loaded(object sender, RoutedEventArgs e)
        {
            this.trexStoryboard.Begin();
            this.AppAboutStoryboard.Begin();
        }
       
        #endregion

        #region Audio Method

        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(()=>
            {
                 if (BackgroundAudioPlayer.Instance.Track != null)
                {
                    try 
                    {
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
                    catch { }
                }
            });
        }

        #endregion

        #region hleper Method
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
                if (App.ViewModel.Channels != null && App.ViewModel.Channels.Count > 0)
                {
                    if (AllChannels.ItemsSource != null && DbFMCommonData.NetworkStatus=="WiFi")
                    {
                        int channelIndex = 0;
                        if (FirstLoadMusicIsPlaying && WpStorage.GetIsoSetting("LastedChannelId") != null)
                        {
                            if (WpStorage.GetIsoSetting("LastedPlayPivotIndex") != null)
                            {
                                LastPivotIndex = (int)WpStorage.GetIsoSetting("LastedPlayPivotIndex");
                            }
                            MainPiovt.SelectedIndex = LastPivotIndex;
                            channelIndex = (int)WpStorage.GetIsoSetting("LastedChannelId");
                            if (channelIndex == -1)
                            {
                                channelIndex = 0;
                            }
                            if (LastPivotIndex == 0)
                            {
                                AllChannels.SelectedIndex = channelIndex;
                            }
                            else if(LastPivotIndex == 1)
                            {
                                CollectChannels.SelectedIndex = channelIndex;
                            }
                        }
                        else
                        {
                            //已经登陆则播放红心赫兹
                            if (DbFMCommonData.loginSuccess)
                            {
                                AllChannels.SelectedIndex = channelIndex;
                            }
                            else
                            {
                                AllChannels.SelectedIndex = 1;
                            }
                        }
                    }
                }
                LoadChannelGrid.Visibility = System.Windows.Visibility.Collapsed;
            });
           
        }
        public void DataContextLoadedFail()
        {
            this.Dispatcher.BeginInvoke(() => 
            {
                LoadChannelGrid.Visibility = System.Windows.Visibility.Visible;
            });
            App.ShowToast(AppResources.OperationError);
        }

        public void GetSongSuccess(int p)
        {
            //标识切换频道 请求play
            WpStorage.CreateFile("ChangeChannels");
            BackgroundAudioPlayer.Instance.Play();
        }
        public void GetSongFail()
        {
            Dispatcher.BeginInvoke(() => 
            {
                AllChannels.SelectedIndex = -1;
                CollectChannels.SelectedIndex = -1;
            });
            App.ShowToast(AppResources.OperationError);
         
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