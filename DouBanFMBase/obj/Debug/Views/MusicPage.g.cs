﻿#pragma checksum "C:\Users\xyl\Documents\Visual Studio 2013\Projects\DouBanFM\DouBanFMBase\Views\MusicPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B7695E52B29E23AD9EAB0B0A4789B919"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34209
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using DouBanFMBase;
using DouBanFMBase.ViewModel;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace DouBanFMBase {
    
    
    public partial class MusicPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Canvas BackgroundLight;
        
        internal System.Windows.Controls.TextBlock TitleText;
        
        internal Microsoft.Phone.Controls.Pivot pivotItems;
        
        internal System.Windows.Controls.Image AlbumArtImage;
        
        internal System.Windows.Controls.Slider SongSlider;
        
        internal System.Windows.Controls.TextBlock EndTextBlock;
        
        internal System.Windows.Controls.TextBlock StartTextBlock;
        
        internal DouBanFMBase.ChangeButton PlayButton;
        
        internal System.Windows.Controls.TextBlock ArtistText;
        
        internal System.Windows.Controls.TextBlock AlbumText;
        
        internal Microsoft.Phone.Controls.PivotItem LrcPivotItem;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal DouBanFMBase.ViewModel.LrcDisplayControl LrcControl;
        
        internal System.Windows.Controls.Grid LoadSongLyrGrid;
        
        internal System.Windows.Controls.TextBlock LoadSongLyr;
        
        internal System.Windows.Shapes.Ellipse ellipse0;
        
        internal System.Windows.Shapes.Ellipse ellipse1;
        
        internal System.Windows.Controls.Image DownSong;
        
        internal DouBanFMBase.ChangeButton LoveImage;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/DouBanFMBase;component/Views/MusicPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.BackgroundLight = ((System.Windows.Controls.Canvas)(this.FindName("BackgroundLight")));
            this.TitleText = ((System.Windows.Controls.TextBlock)(this.FindName("TitleText")));
            this.pivotItems = ((Microsoft.Phone.Controls.Pivot)(this.FindName("pivotItems")));
            this.AlbumArtImage = ((System.Windows.Controls.Image)(this.FindName("AlbumArtImage")));
            this.SongSlider = ((System.Windows.Controls.Slider)(this.FindName("SongSlider")));
            this.EndTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("EndTextBlock")));
            this.StartTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("StartTextBlock")));
            this.PlayButton = ((DouBanFMBase.ChangeButton)(this.FindName("PlayButton")));
            this.ArtistText = ((System.Windows.Controls.TextBlock)(this.FindName("ArtistText")));
            this.AlbumText = ((System.Windows.Controls.TextBlock)(this.FindName("AlbumText")));
            this.LrcPivotItem = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("LrcPivotItem")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.LrcControl = ((DouBanFMBase.ViewModel.LrcDisplayControl)(this.FindName("LrcControl")));
            this.LoadSongLyrGrid = ((System.Windows.Controls.Grid)(this.FindName("LoadSongLyrGrid")));
            this.LoadSongLyr = ((System.Windows.Controls.TextBlock)(this.FindName("LoadSongLyr")));
            this.ellipse0 = ((System.Windows.Shapes.Ellipse)(this.FindName("ellipse0")));
            this.ellipse1 = ((System.Windows.Shapes.Ellipse)(this.FindName("ellipse1")));
            this.DownSong = ((System.Windows.Controls.Image)(this.FindName("DownSong")));
            this.LoveImage = ((DouBanFMBase.ChangeButton)(this.FindName("LoveImage")));
        }
    }
}

