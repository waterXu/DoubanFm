using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DouBanAudioAgent;
using System.Windows.Media.Imaging;

namespace DouBanFMBase.ViewModel
{
    public class MainViewModel:ViewModelBase
    {
        public bool IsLoaded = false;

        public MainViewModel()
        {
            this.Channels = new ObservableCollection<ChannelViewModel>();
            this.CollectChannels = new ObservableCollection<ChannelViewModel>();
            this.LocalSongs = new ObservableCollection<SongInfo>();
            // Channels.CollectionChanged += new NotifyCollectionChangedEventHandler(ChannelsChanged);
            //Channels.PropertyChanged += new PropertyChangedEventHandler(ChannelPropertyChanged);
        }
        #region Property
        public ObservableCollection<ChannelViewModel> Channels { get; set; }
        public ObservableCollection<ChannelViewModel> CollectChannels { get; set; }

        private ObservableCollection<SongInfo> localSongs;
        /// <summary>
        /// 下载歌曲绑定
        /// </summary>
        public ObservableCollection<SongInfo> LocalSongs 
        {
            get { return localSongs; }
            set
            {
                if (localSongs == null)
                {
                    localSongs = new ObservableCollection<SongInfo>();
                }
                if (localSongs != value)
                {
                    localSongs = value;
                    NotifyPropertyChanged("LocalSongs");
                }
            }
        }
        private bool _loginSuccess;
        public bool LoginSuccess
        {
            get { return _loginSuccess; }
            set
            {
                if (_loginSuccess != value)
                {
                    _loginSuccess = value;
                    NotifyPropertyChanged("LoginSuccess");
                }
            }
        }

        private BitmapImage backgroundImg;
        public BitmapImage BackgroundImg
        {
            get
            {
                return backgroundImg;
            }
            set
            {
                backgroundImg = value;
                NotifyPropertyChanged("BackgroundImg");
            }
        }
        #endregion

        #region Method
        public void UpdateTheme()
        {
            //ImageBrush bgBrush = new ImageBrush();
            //获取主题路径
            string themeBgPath = DbFMCommonData.DefaultDayTheme;
            if (WpStorage.GetIsoSetting(DbFMCommonData.ThemePath) != null)
            {
                themeBgPath = WpStorage.GetIsoSetting(DbFMCommonData.ThemePath).ToString();
            }

            bool showMode = false;

            //获取显示模式 true 为夜间模式
            if (WpStorage.GetIsoSetting(DbFMCommonData.ShowMode) != null)
            {
                showMode = (bool)WpStorage.GetIsoSetting(DbFMCommonData.ShowMode);
            }

            if (showMode)
            {
                BackgroundImg = new BitmapImage(new Uri(DbFMCommonData.DefaultNightTheme, UriKind.RelativeOrAbsolute));
            }
            else
            {
                BackgroundImg = new BitmapImage(new Uri(themeBgPath, UriKind.RelativeOrAbsolute));
            }
        }
        public void LoadData()
        {
            bool ischecked = false;
            HashSet<string> collectHashSet = new HashSet<string>();
            try
            {
                if (WpStorage.GetIsoSetting(DbFMCommonData.CollectName) != null)
                {
                    //获取独立存储中收藏列表的 channelid
                    collectHashSet = JsonConvert.DeserializeObject<HashSet<string>>(WpStorage.GetIsoSetting(DbFMCommonData.CollectName).ToString());
                }
                if (DbFMCommonData.ChannelList.Channels != null && DbFMCommonData.ChannelList.Channels.Count > 0)
                {
                    foreach (ChannelInfo channelInfo in DbFMCommonData.ChannelList.Channels)
                    {
                        if (collectHashSet.Contains(channelInfo.channel_id))
                        {
                            ischecked = true;
                        }
                        else
                        {
                            ischecked = false;
                        }
                        Channels.Add(new ChannelViewModel()
                        {
                            Name = channelInfo.name,
                            ChannelId = channelInfo.channel_id,
                            IsChecked = ischecked
                        });
                    }
                    IsLoaded = true;
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadedData, IsLoaded);
                }
                else
                {
                    //todo  显示没有加载成功
                    //重新加载按钮。。。。
                }
            }
            catch(Exception e)
            {
                //todo  显示没有加载成功
                //重新加载按钮。。。。
                System.Diagnostics.Debug.WriteLine("LoadData异常：" + e.Message);
            }
           
        }

        public void TriggerChangeCollectChannels(ChannelViewModel channelInfo,bool isChecked)
        {
            try
            {
                if (isChecked)
                {
                    //ischecked为真则为新增收藏 必定来自全部赫兹
                    CollectChannels.Add(channelInfo);
                    DbFMCommonData.CollectHashSet.Add(channelInfo.ChannelId);
                    SaveCollectHashSet();
                }
                else
                {
                    //ischecked为假则为减去收藏
                    int index = CollectChannels.IndexOf(channelInfo);
                    if (index != -1)
                    {
                        //存在则删除
                        CollectChannels.RemoveAt(index);
                    }
                    foreach(ChannelViewModel cmv in Channels){
                        if (cmv.ChannelId == channelInfo.ChannelId)
                        {
                            cmv.IsChecked = isChecked;
                            break;
                        }
                    }
                    DbFMCommonData.CollectHashSet.Remove(channelInfo.ChannelId);
                    SaveCollectHashSet();
                }
                
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("TriggerChangeCollectChannels异常：" + e.Message);
            }
          
        }
        private void SaveCollectHashSet()
        {
            string collectChannels = null;
            if (DbFMCommonData.CollectHashSet.Count > 0)
            {
                //把hashset表反序列化为字符串 存入独立存储
                collectChannels = JsonConvert.SerializeObject(DbFMCommonData.CollectHashSet);
            }
            WpStorage.SetIsoSetting(DbFMCommonData.CollectName, collectChannels);
        }
        public void SaveDownSongs()
        {
            //this.LocalSongs = DbFMCommonData.DownSongsList;
            string downSongIds = null;
            if (DbFMCommonData.DownSongIdList.Count > 0)
            {
                //把hashset表反序列化为字符串 存入独立存储
                downSongIds = JsonConvert.SerializeObject(DbFMCommonData.DownSongIdList);
            }
            WpStorage.SetIsoSetting(DbFMCommonData.DownSongIdsName, downSongIds);

            string downSongs = null;
            if (LocalSongs.Count > 0)
            {
                //把下载歌曲反序列化为字符串 存入独立存储
                downSongs = JsonConvert.SerializeObject(DbFMCommonData.DownSongsList);
            }
            WpStorage.SaveStringToIsoStore(DbFMCommonData.SongsSavePath, downSongs);

        }
        #endregion
    }
}
