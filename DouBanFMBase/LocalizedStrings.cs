using DouBanAudioAgent;
using DouBanFMBase.Resources;
using System.ComponentModel;

namespace DouBanFMBase
{
    /// <summary>
    /// 提供对字符串资源的访问权。
    /// </summary>
    public class LocalizedStrings : ViewModel.ViewModelBase
    {
        private static AppResources _localizedResources = new AppResources();
        public AppResources LocalizedResources
        { 
            get 
            { 
                return _localizedResources; 
            }
            set
            {
                if (_localizedResources != value)
                {
                    _localizedResources = value;
                    NotifyPropertyChanged("LocalizedStrings");
                }
            }
        }
        internal void ChangeCulture(string cultureName)
        {
            WpStorage.SetIsoSetting(DbFMCommonData.NativeName, cultureName);
            AppResources.Culture = new System.Globalization.CultureInfo(cultureName);
            NotifyPropertyChanged("LocalizedResources");
        }
        //public event PropertyChangedEventHandler PropertyChanged;
        //public void NotifyPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (null != handler)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}