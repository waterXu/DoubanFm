using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DouBanFMBase.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
