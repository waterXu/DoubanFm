﻿#pragma checksum "C:\Users\xyl\Documents\Visual Studio 2013\Projects\DouBanFM\DouBanFMBase\PopUp\DbLoginControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0C260ECFF72EA349D2883EFDD157BDEB"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34209
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace DouBanFMBase.PopUp {
    
    
    public partial class DbLoginControl : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid MainGrid;
        
        internal System.Windows.Controls.Grid ContentGrid;
        
        internal System.Windows.Controls.TextBlock GiantMLogin;
        
        internal System.Windows.Controls.TextBox DbFmAccount;
        
        internal System.Windows.Controls.PasswordBox DbFmPassword;
        
        internal System.Windows.Controls.TextBlock ForgetPwd;
        
        internal System.Windows.Controls.Button RegisterAccount;
        
        internal System.Windows.Controls.Button LoginBtn;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DouBanFMBase;component/PopUp/DbLoginControl.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MainGrid = ((System.Windows.Controls.Grid)(this.FindName("MainGrid")));
            this.ContentGrid = ((System.Windows.Controls.Grid)(this.FindName("ContentGrid")));
            this.GiantMLogin = ((System.Windows.Controls.TextBlock)(this.FindName("GiantMLogin")));
            this.DbFmAccount = ((System.Windows.Controls.TextBox)(this.FindName("DbFmAccount")));
            this.DbFmPassword = ((System.Windows.Controls.PasswordBox)(this.FindName("DbFmPassword")));
            this.ForgetPwd = ((System.Windows.Controls.TextBlock)(this.FindName("ForgetPwd")));
            this.RegisterAccount = ((System.Windows.Controls.Button)(this.FindName("RegisterAccount")));
            this.LoginBtn = ((System.Windows.Controls.Button)(this.FindName("LoginBtn")));
        }
    }
}

