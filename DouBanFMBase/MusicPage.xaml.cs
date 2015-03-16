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

namespace DouBanFMBase
{
    public partial class MusicPage : PhoneApplicationPage
    {
        public MusicPage()
        {
            InitializeComponent();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}