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
using DouBanFMBase.Commands;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows;
using Microsoft.Phone.Tasks;

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
        
        private bool _themeMode;
         public bool ThemeMode
        {
            get
            {
                return _themeMode;
            }
            set
            {
                if (_themeMode != value)
                {
                    _themeMode = value;
                    //保存用户选择到独立存储
                    WpStorage.SetIsoSetting(DbFMCommonData.ShowMode, _themeMode);
                    if (_themeMode)
                    {
                        NightOpacity = 0.6;
                    }
                    else
                    {
                        NightOpacity = 0;
                    }
                    UpdateTheme();
                    NotifyPropertyChanged("ThemeMode");
                }
            }
        }
         private double _nightOpacity;
         public double NightOpacity
        {
            get
            {
                return _nightOpacity;
            }
            set
            {
                if (_nightOpacity != value)
                {
                    _nightOpacity = value;
                    NotifyPropertyChanged("NightOpacity");
                }
              
            }
        }
         private BitmapImage _customThemeImg = new BitmapImage();
         public BitmapImage CustomThemeImg
         {
             get
             {
                 return _customThemeImg;
             }
             set
             {
                 if(_customThemeImg != value)
                 {
                     _customThemeImg = value;
                     NotifyPropertyChanged("CustomThemeImg");
                 }
             }
         }
         private BitmapImage _userPictrue = new BitmapImage();
         public BitmapImage UserPictrue
         {
             get
             {
                 return _userPictrue;
             }
             set
             {
                 if (_userPictrue != value)
                 {
                     _userPictrue = value;
                     NotifyPropertyChanged("UserPictrue");
                 }
             }
         }
        
        #endregion

        #region Command
        private DelegateCommand<string> _selectThemeCommand;
        public DelegateCommand<string> SelectThemeCommand
        {
            get
            {
                return _selectThemeCommand??(_selectThemeCommand = new DelegateCommand<string>((imageId)=>{
                    string backImgPath = "/Images/theme/theme"+imageId+".jpg";
                    //BackgroundImg = new BitmapImage(new Uri(backImgPath, UriKind.RelativeOrAbsolute));
                    WpStorage.SetIsoSetting(DbFMCommonData.ThemePath, backImgPath);
                    WpStorage.SetIsoSetting(DbFMCommonData.IsCustom, false);
                    UpdateTheme();
                }));
            }
        }
  
        //使用自定义主题
        private DelegateCommand _selectCustomCommand;
        public DelegateCommand SelectCustomCommand
        {
            get
            {
                return _selectCustomCommand ?? (_selectCustomCommand = new DelegateCommand(() =>
                {
                    if (WpStorage.isoFile.FileExists(DbFMCommonData.CustomJpgPath))
                    {
                        try
                        {
                            using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(DbFMCommonData.CustomJpgPath, FileMode.Open, FileAccess.ReadWrite, WpStorage.isoFile))
                            {
                                BackgroundImg.SetSource(isoFileStream);
                            }
                            WpStorage.SetIsoSetting(DbFMCommonData.IsCustom, true);
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("SelectCustomCommand ex" + ex.Message);
                        }
                    }
                    else
                    {
                        CallbackManager.Mainpage.Dispatcher.BeginInvoke(() => {
                            MessageBox.Show("请先从本地图库添加主题图片");
                        });
                    }
                }));
            }
        }
        //从本地图片库选择自定义主题
        private DelegateCommand _customThemeCommand;
        public DelegateCommand CustomThemeCommand
        {
            get
            {
                return _customThemeCommand ?? (_customThemeCommand = new DelegateCommand(() =>
                {
                    PhotoChooserTask photoTask = new PhotoChooserTask();
                    photoTask.Completed += new EventHandler<PhotoResult>(PhotoTask_Completed);
                    photoTask.Show();
                }));
            }
        }
        private void PhotoTask_Completed(object sender, PhotoResult e)
        {
             if (e.TaskResult == TaskResult.OK)//选择器操作完成
             {
                 BackgroundImg.SetSource(e.ChosenPhoto);
                 CustomThemeImg.SetSource(e.ChosenPhoto);

                 MemoryStream stream = new MemoryStream();
                 WriteableBitmap wbmp = new WriteableBitmap(BackgroundImg);
                 wbmp.SaveJpeg(stream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);
                 stream.Seek(0, SeekOrigin.Begin);
                 if (WpStorage.isoFile.FileExists(DbFMCommonData.CustomJpgPath))
                 {
                     WpStorage.isoFile.DeleteFile(DbFMCommonData.CustomJpgPath);
                 }
                 using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(DbFMCommonData.CustomJpgPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, WpStorage.isoFile))
                 {
                     Extensions.SaveJpeg(wbmp, isoFileStream, 400, 800, 0, 85);//保存到独立存储
                 }

                 if (WpStorage.isoFile.FileExists(DbFMCommonData.CustomJpgPath))
                 {
                     WpStorage.SetIsoSetting(DbFMCommonData.IsCustom, true);
                 }
             }
        }
          //从本地图片库头像
        private DelegateCommand _selectUserPictrue;
        public DelegateCommand SelectUserPictrue
        {
            get
            {
                return _selectUserPictrue ?? (_selectUserPictrue = new DelegateCommand(() =>
                {
                    PhotoChooserTask photoTask = new PhotoChooserTask();
                    photoTask.Completed += new EventHandler<PhotoResult>(UserPhotoTask_Completed);
                    //是否显示拍照按钮
                    photoTask.ShowCamera = true;
                    //知识点②
                    //设置剪切区域的宽度
                    photoTask.PixelWidth = 800;
                    //设置剪切区域的高度
                    photoTask.PixelHeight = 800;
                    photoTask.Show();
                }));
            }
        }
        private void UserPhotoTask_Completed(object sender, PhotoResult e)
        {
             if (e.TaskResult == TaskResult.OK)//选择器操作完成
             {
                 UserPictrue.SetSource(e.ChosenPhoto);

                 MemoryStream stream = new MemoryStream();
                 WriteableBitmap wbmp = new WriteableBitmap(UserPictrue);
                 wbmp.SaveJpeg(stream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);
                 stream.Seek(0, SeekOrigin.Begin);

                 if (WpStorage.isoFile.FileExists(DbFMCommonData.UserJpgPath))
                 {
                     WpStorage.isoFile.DeleteFile(DbFMCommonData.UserJpgPath);
                 }
                 using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(DbFMCommonData.UserJpgPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, WpStorage.isoFile))
                 {
                     Extensions.SaveJpeg(wbmp, isoFileStream, 256, 256, 0, 85);//保存到独立存储
                 }
             }
        }
        
        #endregion

        #region Method

        private bool isCustom = false;
        public void UpdateTheme()
        {
            //ImageBrush bgBrush = new ImageBrush();
            //获取主题路径
            string themeBgPath = DbFMCommonData.DefaultDayTheme;
            if (WpStorage.GetIsoSetting(DbFMCommonData.ThemePath) != null)
            {
                themeBgPath = WpStorage.GetIsoSetting(DbFMCommonData.ThemePath).ToString();
            }

            if (WpStorage.GetIsoSetting(DbFMCommonData.IsCustom) != null)
            {
                isCustom = (bool)WpStorage.GetIsoSetting(DbFMCommonData.IsCustom);
            }

            //判断显示模式 true 为夜间模式
            if (isCustom)
            {
                //从独立存储中获取
                if (WpStorage.isoFile.FileExists(DbFMCommonData.CustomJpgPath))
                {
                    try
                    {
                        using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(DbFMCommonData.CustomJpgPath, FileMode.Open, FileAccess.ReadWrite, WpStorage.isoFile))
                        {
                             BackgroundImg.SetSource(isoFileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        BackgroundImg = new BitmapImage(new Uri(themeBgPath, UriKind.RelativeOrAbsolute));
                        System.Diagnostics.Debug.WriteLine("UpdateTheme ex:" + ex.Message);
                    }
                    
                }
                else
                {
                    BackgroundImg = new BitmapImage(new Uri(themeBgPath, UriKind.RelativeOrAbsolute));
                }
            }
            else
            {
                BackgroundImg = new BitmapImage(new Uri(themeBgPath, UriKind.RelativeOrAbsolute));
            }
        }
        public void InitPropertyValue()
        {
            //获取用户自定义主题
            if (WpStorage.isoFile.FileExists(DbFMCommonData.CustomJpgPath))
            {
                try
                {
                    using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(DbFMCommonData.CustomJpgPath, FileMode.Open, FileAccess.ReadWrite, WpStorage.isoFile))
                    {
                        CustomThemeImg.SetSource(isoFileStream);
                    }
                }
                catch (Exception ex)
                {
                    CustomThemeImg = new BitmapImage(new Uri("/Images/theme/theme7.jpg", UriKind.RelativeOrAbsolute));
                    System.Diagnostics.Debug.WriteLine("CustomThemeImg ex " + ex.Message);
                }

            }
            else
            {
                CustomThemeImg = new BitmapImage(new Uri("/Images/theme/theme7.jpg", UriKind.RelativeOrAbsolute));
            }

            //获取用户头像
            if (WpStorage.isoFile.FileExists(DbFMCommonData.UserJpgPath))
            {
                try
                {
                    using (IsolatedStorageFileStream isoFileStream = new IsolatedStorageFileStream(DbFMCommonData.UserJpgPath, FileMode.Open, FileAccess.ReadWrite, WpStorage.isoFile))
                    {
                        UserPictrue.SetSource(isoFileStream);
                    }
                }
                catch (Exception ex)
                {
                    UserPictrue = new BitmapImage(new Uri("/Images/defaultuser.png", UriKind.RelativeOrAbsolute));
                    System.Diagnostics.Debug.WriteLine("UserPictrue ex " + ex.Message);
                }
            }
            else
            {
                UserPictrue = new BitmapImage(new Uri("/Images/defaultuser.png", UriKind.RelativeOrAbsolute));
            }
        }
        public void LoadData()
        {
            bool ischecked = false;
            IsLoaded = true;
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
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadedData, IsLoaded);
                }
                else
                {
                    DbFMCommonData.DownLoadSuccess = false;
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadedData, IsLoaded);
                }
            }
            catch(Exception e)
            {
                //todo  显示没有加载成功
                //重新加载按钮。。。。
                DbFMCommonData.DownLoadSuccess = false;
                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadedData, IsLoaded);
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
