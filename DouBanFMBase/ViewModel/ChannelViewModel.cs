using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase.ViewModel
{
    public class ChannelViewModel : ViewModelBase
    {
        public ChannelViewModel()
        {
            string a = "i am here";
        }
        private string _name;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        private string _channel_id;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string ChannelId
        {
            get
            {
                return _channel_id;
            }
            set
            {
                if (value != _channel_id)
                {
                    _channel_id = value;
                    NotifyPropertyChanged("ChannelId");
                }
            }
        }
        private bool _isChecked;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    NotifyPropertyChanged("IsChecked");
                    //通知收藏赫兹变更
                    App.ViewModel.TriggerChangeCollectChannels(this,_isChecked);
                }
            }
        }
      
    }
}
