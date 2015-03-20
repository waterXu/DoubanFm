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

namespace DouBanFMBase
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }
        public ObservableCollection<ChannelViewModel> CollectChannels { get; set; }
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
        public MainViewModel()
        {
            this.Channels = new ObservableCollection<ChannelViewModel>();
            this.CollectChannels = new ObservableCollection<ChannelViewModel>();
           // Channels.CollectionChanged += new NotifyCollectionChangedEventHandler(ChannelsChanged);
            //Channels.PropertyChanged += new PropertyChangedEventHandler(ChannelPropertyChanged);
        }
        public bool IsLoaded = false;
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
        private void ChannelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChannelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<ChannelViewModel> cvCol = sender as ObservableCollection<ChannelViewModel>;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //判断新增的项是否被收藏
                ChannelViewModel cv = cvCol[cvCol.Count - 1];
                if (cv.IsChecked)
                {
                    CollectChannels.Add(cv);
                }
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
   
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string porpertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (PropertyChanged != null)
            {
                handler(this, new PropertyChangedEventArgs(porpertyName));
            }
        }
    }
}
