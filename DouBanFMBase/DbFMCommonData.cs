using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DouBanAudioAgent;
using System.Collections.ObjectModel;

namespace DouBanFMBase
{
    public static class DbFMCommonData
    {
        #region Common  readonly Data
        /// <summary>
        /// 登录url
        /// </summary>
        public static string LoginUrl { get { return "http://www.douban.com/j/app/login"; } }
        /// <summary>
        /// 注册url
        /// </summary>
        public static string RegisterUrl { get { return "http://douban.fm/j/register"; } }
        public static string GetcaptchaId { get { return "http://douban.fm/j/reg_captcha"; } }
        public static string GetcaptchaImgUrl { get { return "http://douban.fm/misc/captcha?size=m&id="; } }
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
        /// 获取歌词url的url地址
        /// </summary>
        public static string LyricUrl { get { return "http://geci.me/api/lyric/"; } }
        /// <summary>
        /// 主页地址
        /// </summary>
        public static string MianPageUrl { get { return "/Views/MainPage.xaml"; } }
        /// <summary>
        /// 歌曲详情页地址
        /// </summary>
        public static string MusicPageUrl { get { return "/Views/MusicPage.xaml"; } }
        /// <summary>
        /// 请求appname
        /// </summary>
        public static string AppName { get { return "radio_desktop_win"; } }
        /// <summary>
        /// 请求version
        /// </summary>
        public static string Version { get { return "100"; } }
        /// <summary>
        /// 下载歌曲根目录
        /// </summary>
        public static string DownSongsIsoName { get { return "DownSongs\\"; } }
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
        /// <summary>
        /// 保存上次用户选择语言
        /// </summary>
        public static string NativeName { get { return "NativeLang"; } }

        #endregion

        #region IsolatedStorage FileName Or KeyName
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
        /// 应用显示 模式 
        /// </summary>
        public static string ShowMode { get { return "ShowMode"; } }
        /// <summary>
        /// 主题图片路径
        /// </summary>
        public static string ThemePath { get { return "ThemePath"; } }
        /// <summary>
        /// 是否自定义主题
        /// </summary>
        public static string IsCustom { get { return "IsCustom"; } }
        /// <summary>
        /// 获取自定义主题路径
        /// </summary>
        public static string CustomJpgPath { get { return "Custom.jpg"; } }
        /// <summary>
        /// 获取用户头像路径
        /// </summary>
        public static string UserJpgPath { get { return "User.jpg"; } }
        /// <summary>
        /// 独立存储收藏hz列表名称
        /// </summary>
        public static string CollectName { get { return "CollectChannels"; } }
        /// <summary>
        /// 独立存储歌曲id列表名称
        /// </summary>
        public static string DownSongIdsName { get { return "DownSongIds"; } }
        /// <summary>
        /// 独立存储下载歌曲信息保存文件名
        /// </summary>
        public static string SongsSavePath { get { return "DownSongsInfo.dat"; } }
        /// <summary>
        /// 红心赫兹 id
        /// </summary>
        public static string HotChannelId { get { return "-3"; } }

        #endregion 

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
        /// 当前网络状态
        /// </summary>
        public static string NetworkStatus { get; set; }
        /// <summary>
        /// 是否自动下载添加的红心歌曲
        /// </summary>
        public static bool AutoDownLoveSongInWifi { get; set; }
        /// <summary>
        /// 验证码 图片地址
        /// </summary>
        public static string CaptchaImgUrl { get; set; }
        /// <summary>
        /// 验证码id
        /// </summary>
        public static string CaptchaId { get; set; }

        public enum CallbackType
        {
            Login = 1,
            LoadedData = 2,
            LoadSongBack = 3,
            DownSongBack = 4,
            DownSongLyrBack = 5,
            OperationBack = 6
        }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public static bool loginSuccess = false;
        /// <summary>
        /// 下载歌曲是否完成
        /// </summary>
        public static bool DownLoadedSong = true;
        /// <summary>
        /// 播放歌曲是否来自本地
        /// </summary>
        public static string SongFormDown { get { return "SongFormDown.dat"; } }
        /// <summary>
        /// 记录用户所在的区域
        /// </summary>
        public static string LocalNative { get; set; }

        /// <summary>
        /// 当前播放歌曲用户是否改变红心（红心显示一致性）
        /// </summary>
        public static bool LoveSongChange = false;

        /// <summary>
        /// 定义popup回调函数
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public delegate void InformCallback(int action, bool isSuccess,string type =null);
        /// <summary>
        /// 回调函数
        /// </summary>
        public static InformCallback informCallback { get; set; }

        public static ChannelList ChannelList { get; set; }
        /// <summary>
        /// 记录收藏hz
        /// </summary>
        public static HashSet<string> CollectHashSet { get;set;}

        /// <summary>
        /// 记录下载歌曲的id
        /// </summary>
        private static HashSet<string> downSongIdList;
        public static HashSet<string> DownSongIdList {
            get {
                if (downSongIdList == null)
                    downSongIdList = new HashSet<string>();
                return downSongIdList; 
            }
            set 
            {
                if (downSongIdList == null)
                    downSongIdList = new HashSet<string>();
                downSongIdList = value;
            }
        }

        /// <summary>
        /// 记录下载的歌曲列表信息
        /// </summary>
        private static ObservableCollection<SongInfo> downSongsList;
        public static ObservableCollection<SongInfo> DownSongsList 
        {
            get
            {
                if (downSongsList == null)
                    downSongsList = new ObservableCollection<SongInfo>();
                return downSongsList;
            }
            set 
            {
                if (downSongsList == null)
                    downSongsList = new ObservableCollection<SongInfo>();
                downSongsList = value;
            }
        }
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

        public static bool IsFirstLoadSongs = true;

        public static bool allowPrev;
        public static bool AllowPrev
        {
            get { return allowPrev; }
            set 
            { 
                if (allowPrev != value) {
                    allowPrev = value;
                    WpStorage.SetIsoSetting("AllowPrev",allowPrev);
                }
            }
        }
        public static string CurrentChannelId {get;set;}
        public static int LastedIndex = -1;
        public static int LastedCollectIndex = -1;

        public static void SetSongsUrl(string type, string channelId, int selectIndex,string songId = null)
        {
            string getChannelSongsUrl = DbFMCommonData.ChannelSongsUrl + "?app_name=" + DbFMCommonData.AppName + "&version=" + DbFMCommonData.Version;
            if (!string.IsNullOrEmpty(DbFMCommonData.UserID))
            {
                getChannelSongsUrl += "&user_id=" + DbFMCommonData.UserID;
            }
            if (!string.IsNullOrEmpty(DbFMCommonData.Expire))
            {
                getChannelSongsUrl += "&expire=" + DbFMCommonData.Expire;
            }
            if (!string.IsNullOrEmpty(DbFMCommonData.Token))
            {
                getChannelSongsUrl += "&token=" + DbFMCommonData.Token;
            }
            if (songId != null)
            {
                getChannelSongsUrl += "&sid=" + songId;
            }
            if (channelId != null)
            {
                getChannelSongsUrl += "&channel=" + channelId;
                CurrentChannelId = channelId;
            }
            getChannelSongsUrl += "&type=" + type;
            //保存 获取歌曲列表Url
            WpStorage.SaveStringToIsoStore("SongsUrl.dat", getChannelSongsUrl);
            WpStorage.SetIsoSetting("LastedChannelId", selectIndex);
        }






    }
}
