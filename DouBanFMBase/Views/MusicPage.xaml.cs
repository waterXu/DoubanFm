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

namespace DouBanFMBase
{
    public partial class MusicPage : PhoneApplicationPage
    {
        // Reference to App Class
        private App app = App.Current as App;
        // How many songs are allowed to Latest List
        private const int LATESTS_MAX = 20;
        // Check Songs playing time -> update TextBlocks and Slider
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        // Latest AlbumArt path
        public string latestAlbumArtPath { get; set; }
        //Latest song srouce
        public string latestSource { get; set; }
        //当前播放歌曲的信息
        private string currentSongInfo { get; set; }
        public MusicPage()
        {
            InitializeComponent();
            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);

        }
        #region Page EventHandler Method
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SongSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(SongSlider_ValueChanged);
            SongSlider.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(SongSlider_Tap);
        }

        // Navigated to this Player Page
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Playing - set Pause Button text
            if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
            {
                PlayButton.IsChecked = false;
            }
            // Paused - set Play Button text
            else if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Paused)
            {
                PlayButton.IsChecked = true;
            }
            Instance_PlayStateChanged(null,new EventArgs());
            // start timer to check position of the song
            startTimer();
        }
        // Navigated from this Page
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Stop timer and remove Event Handlers
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
            BackgroundAudioPlayer.Instance.PlayStateChanged -= new EventHandler(Instance_PlayStateChanged);
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
                        break;
                    case 1:
                        ellipse0.Fill = new SolidColorBrush(Color.FromArgb(100, 128, 128, 128));
                        ellipse1.Fill = new SolidColorBrush(Color.FromArgb(100, 230, 40, 40));
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
            HttpHelper.DownLoadMusic(currentSongInfo);
            //check this music exsit
            //HttpHelper.DownLoadMusic();
        }

        private void LoveImage_Click(object sender, RoutedEventArgs e)
        {
            HttpHelper.SetSongLoveStatus(LoveImage.IsChecked);
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
            this.AlbumArtImage.Opacity = 1;
            ImageChange();
        }
        #endregion
        #region Hleper Method
        private void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("State=" + BackgroundAudioPlayer.Instance.PlayerState);

            // ff something is playing (a new song)
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
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

            }
        }
        // Load AlbumArt
        private void LoadAlbumArt()
        {
            System.Diagnostics.Debug.WriteLine("AlbumArt = " + BackgroundAudioPlayer.Instance.Track.AlbumArt);

            Uri songURL = BackgroundAudioPlayer.Instance.Track.Source;

            //如果与上次歌曲url不一致  则重新获取最新 tag 数据（songInfo）
            if (songURL.AbsolutePath != latestSource)
            {
                string tag = BackgroundAudioPlayer.Instance.Track.Tag;
                if(!string.IsNullOrEmpty(tag)){
                    currentSongInfo = tag;
                }
            }

            // get album art Uri from Audio Playback Agetn
            Uri albumArtURL = BackgroundAudioPlayer.Instance.Track.AlbumArt;
            // load album art from net
            if (albumArtURL != null && latestAlbumArtPath != albumArtURL.AbsolutePath)
            {
                latestAlbumArtPath = albumArtURL.AbsolutePath;
                AlbumArtImage.Source = new BitmapImage(albumArtURL);
            }
            // there is no album art in net, load album art from project resources
            else if (albumArtURL == null)
            {
                Random random = new Random();
                //get random theme image
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
        }
        public void DownLoadSongBack(bool isSuccess)
        {

        }
        private void SaveDownSongsHashSet()
        {
            string downSongIds = null;
            if (DbFMCommonData.CollectHashSet.Count > 0)
            {
                //把hashset表反序列化为字符串 存入独立存储
                downSongIds = JsonConvert.SerializeObject(DbFMCommonData.DownSongIdList);
            }
            WpStorage.SetIsoSetting(DbFMCommonData.DownSongIdsName, downSongIds);
        }
        #endregion

    }
}