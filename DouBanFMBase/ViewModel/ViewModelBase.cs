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
        public BitmapImage backgroundImg;
        public BitmapImage BackgroundImg
        {
            get
            {
                if (backgroundImg == null)
                {
                    backgroundImg = new BitmapImage();
                }
                return backgroundImg;
            }
            set
            {
                backgroundImg = value;
                NotifyPropertyChanged("BackgroundImg");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
