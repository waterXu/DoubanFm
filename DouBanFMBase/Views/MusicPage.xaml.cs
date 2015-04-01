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
using Microsoft.Phone.BackgroundAudio;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using DouBanAudioAgent;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Documents;
using System.Windows.Data;
using DouBanFMBase.ViewModel;
using DouBanFMBase.Resources;

namespace DouBanFMBase
{
    public partial class MusicPage : PhoneApplicationPage
    {
        // Reference to App Class
        //private App app = App.Current as App;
        // Check Songs playing time -> update TextBlocks and Slider
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        // Latest AlbumArt path
        public string latestAlbumArtPath { get; set; }
        //Latest song srouce
        public string latestSource { get; set; }
        //latest song id
        public static string latestSid { get; set; }
        //当前播放歌曲的信息
        private SongInfo currentSongInfo { get; set; }
        /// <summary>
        /// 是否第一次跳转到musicpage
        /// </summary>
        private bool isFirstComeHere { get; set; }
        public MusicPage()
        {
            InitializeComponent();
            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
            DataContext = App.ViewModel;
            //LrcControl.DataContext = App.MusicViewModel;
        }
        #region Page EventHandler Method
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SongSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(SongSlider_ValueChanged);
            SongSlider.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(SongSlider_Tap);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CallbackManager.musicPage = this;
            isFirstComeHere = true;
            if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
            {
                PlayButton.IsChecked = false;
            }
            else if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Paused)
            {
                PlayButton.IsChecked = true;
            }
            Instance_PlayStateChanged(null,new EventArgs());
            startTimer();
        }
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Stop timer and remove Event Handlers
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
            BackgroundAudioPlayer.Instance.PlayStateChanged -= new EventHandler(Instance_PlayStateChanged);
            CallbackManager.musicPage = null;
            MainPage.IsFromMusicPage = true;
            LrcControl.DataContext = null;
        }
        #endregion


        #region Control Eventhandler Method
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayButton.IsChecked)
            {
                BackgroundAudioPlayer.Instance.Play();
            }
            else
            {
                BackgroundAudioPlayer.Instance.Pause();
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = pivotItems.SelectedIndex;
            if (index > -1)
            {
                switch (index)
                {
                    case 0:
                        ellipse0.Fill = new SolidColorBrush(Color.FromArgb(100, 230, 40, 40));
                        ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        BackgroundLight.Opacity = 0;
                        break;
                    case 1:
                        ellipse0.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100, 230, 40, 40));
                        BackgroundLight.Opacity = 0.5;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ForwardImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipNext();
        }

        private void DownMusic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //查看该歌曲是否已经下载
            if (DbFMCommonData.DownSongIdList.Contains(currentSongInfo.sid))
            {
                MessageBox.Show(AppResources.SongIsDown);
                return;
            }
            DownSong.Visibility = System.Windows.Visibility.Collapsed;
            HttpHelper.DownLoadSongLyr(currentSongInfo, true);
            HttpHelper.DownLoadMusic(currentSongInfo);
        }

        private void LoveImage_Click(object sender, RoutedEventArgs e)
        {
            //r	sid	rate，歌曲正在播放，标记喜欢当前歌曲	短报告
            //u	sid	unrate，歌曲正在播放，标记取消喜欢当前歌曲
            if (!DbFMCommonData.loginSuccess)
            {
                MessageBox.Show(AppResources.AddLoveSongTip);
                LoveImage.IsChecked = currentSongInfo.like=="1"?false:true;
                return;
            }
            string type = "";
            type = LoveImage.IsChecked? "u":"r";
            HttpHelper.OperationChannelSongs(type, DbFMCommonData.CurrentChannelId, currentSongInfo.sid);
        }
        private void DeleteSong_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HttpHelper.OperationChannelSongs("b", DbFMCommonData.CurrentChannelId, currentSongInfo.sid);
        }
        private void SongSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //判断是否用户拖动  false则为计时器触发
            if (SongSlider.Tag != null)
            {
                string songSilderTag = SongSlider.Tag.ToString();
                if (songSilderTag.Equals("isFoucesed"))
                {
                    SongSlider.Tag = "loseFoucesed";
                    dispatcherTimer.Stop();
                    int sliderValue = (int)e.OldValue;
                    TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, sliderValue);
                    // set a new position of the song
                    BackgroundAudioPlayer.Instance.Position = timeSpan;
                    dispatcherTimer.Start();
                }
            }
        }

        private void SongSlider_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SongSlider.Tag = "isFoucesed";
        }

        private void AlbumArtImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            ImageChange();
        }
        private void AlbumArtImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Random random = new Random();
            //get random theme image
            int imageindex = random.Next(1, 10);
            Uri uri = new Uri("/Images/theme/theme" + imageindex.ToString() + ".jpg", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(uri);
            AlbumArtImage.Source = bitmapImage;
        }

        private void LoadSongLycGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LoadSongLyrGrid.Visibility = System.Windows.Visibility.Collapsed;
            LoadSongLyric();
        }
        #endregion

        #region Hleper Method
        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("State=" + BackgroundAudioPlayer.Instance.PlayerState);

            // ff something is playing (a new song)
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                DownSong.Visibility = System.Windows.Visibility.Visible;
                TitleText.Text = BackgroundAudioPlayer.Instance.Track.Title;
                ArtistText.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                AlbumText.Text = BackgroundAudioPlayer.Instance.Track.Album;
                SongSlider.Minimum = 0;
                SongSlider.Maximum = BackgroundAudioPlayer.Instance.Track.Duration.TotalMilliseconds;
                string text = BackgroundAudioPlayer.Instance.Track.Duration.ToString();
                EndTextBlock.Text = text.Substring(3, 5);
                // album art
                LoadAlbumArt();
            }
            if (isFirstComeHere)
            {
                isFirstComeHere = false;
                if (!DbFMCommonData.DownLoadedSong)
                {
                    DownSong.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        private void startTimer()
        {
            // start timer to check position of the song
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }
        private string endtext;
        private string starttext;
        // Timer is calling this function to check plaing song data
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // song is playing
            if (PlayState.Playing == BackgroundAudioPlayer.Instance.PlayerState)
            {
                // handle slider
                SongSlider.Minimum = 0;
                SongSlider.Value = BackgroundAudioPlayer.Instance.Position.TotalMilliseconds;
                SongSlider.Maximum = BackgroundAudioPlayer.Instance.Track.Duration.TotalMilliseconds;
                SongSlider.Tag = "loseFoucesed";
                // display text
                starttext = BackgroundAudioPlayer.Instance.Position.ToString();
                StartTextBlock.Text = starttext.Substring(3, 5);
                endtext = BackgroundAudioPlayer.Instance.Track.Duration.ToString();
                EndTextBlock.Text = endtext.Substring(3, 5);
                App.MusicViewModel.Progress = SongSlider.Value;
            }
        }
        // Load AlbumArt
        private void LoadAlbumArt()
        {
            System.Diagnostics.Debug.WriteLine("AlbumArt = " + BackgroundAudioPlayer.Instance.Track.AlbumArt);

            Uri songURL = BackgroundAudioPlayer.Instance.Track.Source;
            //如果与上次歌曲url不一致  则重新获取最新 tag 数据（songInfo）
            if (songURL.ToString() != latestSource)
            {
                latestSource = songURL.ToString();
                string tag = BackgroundAudioPlayer.Instance.Track.Tag;
                if(!string.IsNullOrEmpty(tag)){
                    currentSongInfo = JsonConvert.DeserializeObject<SongInfo>(tag);
                }
                
                LoveImage.IsChecked = currentSongInfo.like == "1" ? true : false;
                //获取歌词
                LoadSongLyric();
            }
            if (DbFMCommonData.LoveSongChange && currentSongInfo.sid == latestSid)
            {
                LoveImage.IsChecked = currentSongInfo.like == "1" ? false : true;
            }
            else
            {
                latestSid = currentSongInfo.sid;
                DbFMCommonData.LoveSongChange = false;
            }
            // get album art Uri from Audio Playback Agetn
            Uri albumArtURL = BackgroundAudioPlayer.Instance.Track.AlbumArt;
            // load album art from net or local
            if (albumArtURL != null && latestAlbumArtPath != albumArtURL.ToString())
            {
                latestAlbumArtPath = albumArtURL.ToString();
                if (DbFMCommonData.SongFormDown)
                {
                    if (WpStorage.isoFile.FileExists(latestAlbumArtPath))
                    {
                        using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(latestAlbumArtPath, FileMode.Open, FileAccess.ReadWrite, WpStorage.isoFile))
                        {
                            BitmapImage background = new BitmapImage();
                            background.SetSource(isoFileStream);
                            AlbumArtImage.Source = background;
                            ImageChange();
                        }
                    }
                }
                else
                {
                    AlbumArtImage.Source = new BitmapImage(albumArtURL);
                }
            }
            else if (albumArtURL == null)
            {
                Random random = new Random();
                int imageindex = random.Next(1, 10);
                Uri uri = new Uri("/Images/theme/theme" + imageindex.ToString() + ".jpg", UriKind.Relative);
                BitmapImage bitmapImage = new BitmapImage(uri);
                AlbumArtImage.Source = bitmapImage;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Don't load same album art again");
            }
        }
        private bool IsLoadingLyric = false;
        private void LoadSongLyric()
        {
            //绑定数据源
            if (LrcControl.DataContext != null)
            {
                LrcControl.DataContext = null;
                App.MusicViewModel.Lrc = null;
            }
            bool needDownLyr = true;
            if (DbFMCommonData.DownSongIdList.Contains(currentSongInfo.sid))
            {
                try
                {
                    if (WpStorage.isoFile.FileExists(DbFMCommonData.DownSongsIsoName + currentSongInfo.sid + ".lrc"))
                    {
                        //解析歌词并显示
                        string lyricInfo = WpStorage.ReadIsolatedStorageFile(DbFMCommonData.DownSongsIsoName + currentSongInfo.sid + ".lrc");
                        App.MusicViewModel.Lrc = lyricInfo;
                        LrcControl.DataContext = App.MusicViewModel;
                        Binding lrc = new Binding();
                        lrc.Path = new PropertyPath("Lrc");
                        LrcControl.SetBinding(LrcDisplayControl.LrcTextProperty, lrc);
                        needDownLyr = false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("LoadSongLyric exception:" + ex.Message);
                }
            }

            if (needDownLyr && !IsLoadingLyric)
            {
                IsLoadingLyric = true;
                HttpHelper.DownLoadSongLyr(currentSongInfo);
            }
           
        }
       
        public void LoadSongLyricBack(bool isSuccess)
        {
            IsLoadingLyric = false;
            Dispatcher.BeginInvoke(() =>
            {
                if (isSuccess)
                {
                   LrcControl.DataContext = App.MusicViewModel;
                   Binding lrc = new Binding();
                   lrc.Path = new PropertyPath("Lrc");
                   LrcControl.SetBinding(LrcDisplayControl.LrcTextProperty,lrc);
                   LoadSongLyrGrid.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    LoadSongLyrGrid.Visibility = System.Windows.Visibility.Visible;
                }

            });
          
        }
        public void OperationSongBack(bool isSuccess,string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return;
            }
            Dispatcher.BeginInvoke(() =>
            {
                if (isSuccess)
                {
                    if (type == "b")
                    {
                        BackgroundAudioPlayer.Instance.SkipNext();
                    }
                    else 
                    {
                        if (type == "r" && currentSongInfo.like == "0")
                        {
                            DbFMCommonData.LoveSongChange = true;
                        }
                        else if (type == "u" && currentSongInfo.like == "1")
                        {
                            DbFMCommonData.LoveSongChange = true;
                        }
                    }
                }
                else
                {
                    if (type != "b")
                    {
                        LoveImage.IsChecked = LoveImage.IsChecked ? false : true;
                    }
                    //todo toast
                    MessageBox.Show(AppResources.OperationError);
                }
            });
        }
        
        SlideTransition st;
        private void ImageChange()
        {
            if (st == null)
            {
                st = new SlideTransition();
            }
            st.Mode = SlideTransitionMode.SlideLeftFadeIn;
            ITransition transition = st.GetTransition(this.AlbumArtImage);
            transition.Completed += delegate
            {
                transition.Stop();
            };
            transition.Begin();
            AlbumArtImage.Opacity = 1;
        }
        public void DownLoadSongBack(bool isSuccess)
        {
            this.Dispatcher.BeginInvoke(()=>{
                DownSong.Visibility = System.Windows.Visibility.Visible;
            });
        }
        #endregion
    }
}