using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase.ViewModel
{
   public class MusicViewModel : ViewModelBase
    {
        private double _progress;
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                NotifyPropertyChanged("Progress");
            }
        }
        private string _lrc;
        public string Lrc
        {
            get
            {
                return _lrc;
            }
            set
            {
                _lrc = value;
                NotifyPropertyChanged("Lrc");
            }
        }
    }
}
