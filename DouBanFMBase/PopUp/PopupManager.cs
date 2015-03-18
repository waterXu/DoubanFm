using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Info;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Text.RegularExpressions;


namespace DouBanFMBase.PopUp
{
    /// <summary>
    /// 管理所有的弹出框中的用户控件
    /// </summary>
    internal  class PopupManager
    {
         public  enum UserControlType
         {
             /// <summary>
             /// 登录界面  DbLoginControl.xaml
             /// </summary>
             LoginControl,
             /// <summary>
             /// 注册界面  DbRegisterControl.xaml
             /// </summary>
             RegisterControl,
             /// <summary>
             /// web注册界面  WebRegisterControl.xaml
             /// </summary>
             WebRegisterControl,
             /// <summary>
             /// 忘记密码页面
             /// </summary>
             ForgetPassword,
             /// <summary>
             /// 协议隐私条款信息页面
             /// </summary>
             LicenseControl,
         };

        /// <summary>
        /// 调用popup的page
        /// </summary>
         public static PhoneApplicationPage GamePage { get; set; }
        /// <summary>
        /// 是否重新获取账号密码
        /// </summary>
        public static bool IsReloadAccount = false;
         
         private static DbLoginControl _loginControl ;
         /// <summary>
         /// 获取登录UserControl
         /// </summary>
         public static DbLoginControl LoginControl 
         {
             get
             {
                 if(_loginControl == null)
                 {
                     _loginControl = new DbLoginControl();
                     _loginControl.Width = screenWidth;
                     _loginControl.Height = screenHeight;
                 }
                 return _loginControl;
             }
         }
         private static DbRegisterControl _registerControl;
         /// <summary>
         /// 获取巨人移动注册UserControl
         /// </summary>
         public static DbRegisterControl RegisterControl
         {
             get
             {
                 if (_registerControl == null)
                 {
                     _registerControl = new DbRegisterControl();
                     _registerControl.Width = screenWidth;
                     _registerControl.Height = screenHeight;
                 }
                 return _registerControl;
             }
         }
         //private static ForgetPassword _forgetControl;
         ///// <summary>
         ///// 忘记密码UserControl
         ///// </summary>
         //public static ForgetPassword ForgetControl
         //{
         //    get
         //    {
         //        if (_forgetControl == null)
         //        {
         //            _forgetControl = new ForgetPassword();
         //            _forgetControl.Width = screenWidth;
         //            _forgetControl.Height = screenHeight;
         //        }
         //        return _forgetControl;
         //    }
         //}
         
         //private static ChangePwdControl _changePwdControl;
         ///// <summary>
         ///// 忘记密码UserControl
         ///// </summary>
         //public static ChangePwdControl ChangePwdControl
         //{
         //    get
         //    {
         //        if (_changePwdControl == null)
         //        {
         //            _changePwdControl = new ChangePwdControl();
         //            _changePwdControl.Width = screenWidth;
         //            _changePwdControl.Height = screenHeight;
         //        }
         //        return _changePwdControl;
         //    }
         //}

         //private static AccountManageControl _accountManageControl;
         ///// <summary>
         ///// 账号管理UserControl
         ///// </summary>
         //public static AccountManageControl AccountManage
         //{
         //    set
         //    {
         //        _accountManageControl = value;
         //    }
         //    get
         //    {
         //        if (_accountManageControl == null)
         //        {
         //            _accountManageControl = new AccountManageControl();
         //            _accountManageControl.Width = screenWidth;
         //            _accountManageControl.Height = screenHeight;
         //        }
         //        return _accountManageControl;
         //    }
         //}
         
         //private static LicenseInfoControl _licenseInfoControl;
         ///// <summary>
         ///// 账号管理UserControl
         ///// </summary>
         //public static LicenseInfoControl LicenseInfoControl
         //{
         //    get
         //    {
         //        if (_licenseInfoControl == null)
         //        {
         //            _licenseInfoControl = new LicenseInfoControl();
         //            _licenseInfoControl.Width = screenWidth;
         //            _licenseInfoControl.Height = screenHeight;
         //        }
         //        return _licenseInfoControl;
         //    }
         //}

         public static Popup _popUp;
         /// <summary>
         /// 获取应用程序区域宽度
         /// </summary>
         public static double screenWidth;
         /// <summary>
         /// 获取应用程序区域高度
         /// </summary>
         public static double screenHeight;
         public static void InitPopupManager() 
         {
              screenWidth = Application.Current.Host.Content.ActualWidth;
              screenHeight = Application.Current.Host.Content.ActualHeight;
             _popUp = new Popup
             {
                 Height = screenWidth,
                 Width = screenHeight,
             };
         }

         /// <summary>
         /// 初始化用户控件 加载相应组件
         /// </summary>
         internal static void ShowUserControl( UserControlType type)
         {
             //判断是否初始化Popup
             if (_popUp == null) 
             {
                 InitPopupManager();
             }
             _popUp.Child = null;
             _popUp.IsOpen = false;
             //根据type显示相应的UserControl
             switch (type) 
             {
                 case UserControlType.LoginControl:
                     _popUp.Child = LoginControl;
                     break;
                 case UserControlType.RegisterControl:
                     _popUp.Child = RegisterControl;
                     break;
                 default:
                     System.Diagnostics.Debug.WriteLine("找不到该弹出框{"+ type.ToString()+ "}");
                     break;
             }
             _popUp.IsOpen = true;
         }
        /// <summary>
        /// 移除弹出框
        /// </summary>
         public static void OffPopUp() 
         {
             if (_popUp != null)
             {
                 _popUp.Child = null;
                 _popUp = null;
             }
         }
         private static int _keyboardPortraitHeight = 0;
         private static int _keyboardLandscapeHeight = 0;
         public static PageOrientation GameOrientation { get; set; }
        /// <summary>
        /// 获取软键盘的高度
        /// </summary>
         public static int KeyboardHeight
         {
             get
             {
                  int _keyboardHeight = 0;
                  switch (GameOrientation)
                  {
                      case PageOrientation.PortraitUp:
                      case PageOrientation.PortraitDown:
                          if (_keyboardPortraitHeight == 0)
                          {
                    
                             _keyboardPortraitHeight = 408;
                             // for 1520的分辨率
                             if (Environment.OSVersion.Version.Major == 8 && Environment.OSVersion.Version.Minor == 10 && ExScreenInfo >= 6.0)
                             {
                                 //_keyboardHeight = 334;
                                 _keyboardPortraitHeight = 430;
                             }
                          }
                          _keyboardHeight = _keyboardPortraitHeight;
                          break;
                      case PageOrientation.LandscapeLeft:
                      case PageOrientation.LandscapeRight:
                          if (_keyboardLandscapeHeight == 0)
                          {
                              _keyboardLandscapeHeight = 280;
                              // for 1520的分辨率
                              if (Environment.OSVersion.Version.Major == 8 && Environment.OSVersion.Version.Minor == 10 && ExScreenInfo >= 6.0)
                              {
                                  _keyboardLandscapeHeight = 300;
                              }
                          }
                          _keyboardHeight = _keyboardLandscapeHeight;
                          break;
                  }
                  return _keyboardHeight;
             }
         }
        
         private static double ExScreenInfo
         {
             get
             {
                 // Only support on OS 8.1+
                 object resolution;
                 DeviceExtendedProperties.TryGetValue("PhysicalScreenResolution", out resolution);

                 object dpix;
                 DeviceExtendedProperties.TryGetValue("RawDpiX", out dpix);

                 if (resolution != null && dpix != null && (double)dpix != 0.0)
                 {
                     Size s = (Size)resolution;
                     return Math.Sqrt(Math.Pow(s.Width / (double)dpix, 2.0) + Math.Pow(s.Height / (double)dpix, 2.0));
                 }

                 return 1;
             }
         }

        private static int marginLength = 0;
        /// <summary>
        /// 当输入框获取焦点时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Input_GotFocus(Control inputBox, Grid layoutRoot)
        {
            if (inputBox.GetType().ToString() == "System.Windows.Controls.TextBox")
            {
                TextBox input = (TextBox)inputBox;
                if (Regex.IsMatch(input.Text.Trim(), "^[\u4e00-\u9fa5]*$"))
                {
                    //清空带中文的提示
                    input.Text = "";
                }
            }
            //改变输入框的背景色
            inputBox.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            layoutRoot.RenderTransform = new CompositeTransform();
            layoutRoot.RenderTransformOrigin = new Point(0.5,0.5);
            var layoutRootTranslate = layoutRoot.RenderTransform as CompositeTransform;
            var gt = inputBox.TransformToVisual(null);
            Point p = gt.Transform(new Point(0, 0));
            int inputBottom = 0;
            switch (GameOrientation)
            {
                case PageOrientation.LandscapeLeft:
                    p.X = p.X - inputBox.ActualHeight/2;
                    inputBottom = (int)p.X;
                    break;
                case PageOrientation.LandscapeRight:
                    p.X = p.X + inputBox.ActualHeight/2;
                    inputBottom = (int)screenWidth - (int)p.X;
                    break;
                case PageOrientation.PortraitUp:
                    inputBottom = (int)screenHeight - (int)p.Y;
                    break;
                case PageOrientation.PortraitDown:
                    inputBottom = (int)p.Y;
                    break;
            }
            marginLength = KeyboardHeight - inputBottom;
            //判断软件盘是否挡住了输入框
            if (marginLength > 0) 
            {
                layoutRootTranslate.TranslateY = -marginLength;
            }
        }
        /// <summary>
        /// 当输入框失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Input_LostFocus(Grid layoutRoot)
        {
            //软键盘收回 弹出框恢复原位置
            layoutRoot.RenderTransform = new CompositeTransform();
            layoutRoot.RenderTransformOrigin = new Point(0.5, 0.5);
            var layoutRootTranslate = layoutRoot.RenderTransform as CompositeTransform;
            layoutRootTranslate.TranslateY = 0;
        }
    }
}
