using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DouBanFMBase
{
    public static class DbFMCommonData
    {
        /// <summary>
        /// 登录url
        /// </summary>
        public static string LoginUrl { get { return "http://www.douban.com/j/app/login"; } }
        /// <summary>
        /// 注册url
        /// </summary>
        public static string RegisterUrl { get { return "http://www.douban.com/j/app/register"; } }
        /// <summary>
        /// 获取频道列表
        /// </summary>
        public static string ChannelListUrl { get { return "http://www.douban.com/j/app/radio/channels"; } }
        public static string CollChannelListUrl { get { return "http://www.douban.com/j/app/radio/channels/collect"; } }
        /// <summary>
        /// 获取歌曲列表url
        /// </summary>
        public static string ChannelSongsUrl { get { return "http://www.douban.com/j/app/radio/people"; } }

        /// <summary>
        /// 请求appname
        /// </summary>
        public static string AppName { get { return "radio_desktop_win"; } }
        /// <summary>
        /// 请求version
        /// </summary>
        public static string Version { get { return "100"; } }
        /// <summary>
        /// 是否第一次使用该App
        /// </summary>
        public static string IsFirstUse { get { return "IsFirstUse"; } }
        /// <summary>
        /// 上次成功登陆用户名
        /// </summary>
        public static string UserName { get { return "UserName"; } }
        /// <summary>
        /// 上次成功登录密码
        /// </summary>
        public static string Password { get { return "Password"; } }
        /// <summary>
        /// 启屏时间
        /// </summary>
        public static int TimerCount = 2;
        /// <summary>
        /// 保存登录成功Token
        /// </summary>
        public static string Token { set; get;}
        /// <summary>
        /// 保存登录成功Expire
        /// </summary>
        public static string Expire { set; get; }
        /// <summary>
        /// 豆瓣昵称
        /// </summary>
        public static string NickName { set; get; }
        /// <summary>
        /// userId
        /// </summary>
        public static string UserID { set; get; }
        /// <summary>
        /// 豆瓣绑定邮箱
        /// </summary>
        public static string Email { get; set; }

        /// <summary>
        /// 应用显示 模式 
        /// </summary>
        public static string ShowMode { get { return "ShowMode"; } }
        /// <summary>
        /// 主题图片路径
        /// </summary>
        public static string ThemePath { get; set; }
        /// <summary>
        /// 默认白天主题路径
        /// </summary>
        public static string DefaultDayTheme { get { return "/Images/theme/theme3.jpg"; } }
        /// <summary>
        /// 默认夜间主题路径
        /// </summary>
        public static string DefaultNightTheme { get { return "/Images/theme/theme1.jpg"; } }
        /// <summary>
        /// 白天小图标
        /// </summary>
        public static string MoonPngPath { get { return "/Images/moon.png"; } }
        /// <summary>
        /// 夜晚小图标
        /// </summary>
        public static string SunPngPath { get { return "/Images/sun.png"; } }

        public enum CallbackType
        {
            Login = 1,
            LoadedData = 2,
            LoadSongBack = 3
        }
        public static bool loginSuccess = false;
       
        /// <summary>
        /// 定义popup回调函数
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public delegate void InformCallback(int action, bool isSuccess);
        /// <summary>
        /// 回调函数
        /// </summary>
        public static InformCallback informCallback { get; set; }

        public static ChannelList ChannelList { get; set; }
        public static string CollectName { get { return "CollectChannels"; } }
        /// <summary>
        /// 记录收藏hz
        /// </summary>
        public static HashSet<string> CollectHashSet { get;set;}
        /// <summary>
        /// MainPage页面是否load完成
        /// </summary>
        public static bool MainPageLoaded = false;
        /// <summary>
        /// 是否获取所有hz成功
        /// </summary>
        public static bool DownLoadSuccess = false;
        /// <summary>
        /// 记录当前内存中的歌曲信息
        /// </summary>
        public static List<SongInfo> PlayingSongs { get; set; }
    }
}
