using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Microsoft.Phone.Info;
using System.IO;
using Windows.Networking.Connectivity;
using DouBanAudioAgent;

namespace DouBanFMBase
{
    class HttpHelper
    {

        public static string SyncResultTostring(IAsyncResult syncResult)
        {
            try
            {
                WebResponse response = ((HttpWebRequest)syncResult.AsyncState).EndGetResponse(syncResult);
                Stream stream = response.GetResponseStream();
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                string result = System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length);
                return result;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SyncResultTostring" + e.Message);
                // todo   show  tip server not conn  如何检测是否联网
                return null;
            }
           
        }
        public static byte[] SyncResultToByte(IAsyncResult syncResult)
        {
            try
            {
                WebResponse response = ((HttpWebRequest)syncResult.AsyncState).EndGetResponse(syncResult);
                Stream stream = response.GetResponseStream();
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return data;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SyncResultTostring" + e.Message);
                // todo   show  tip server not conn  如何检测是否联网
                return null;
            }

        }
        /// <summary>
        /// 豆瓣登录账号请求  服务器返回统一code解析
        /// </summary>
        /// <param name="syncResult"></param>
        /// <returns></returns>
        public static bool LoginResultCodeInfo(IAsyncResult syncResult) 
        {
            try
            {
                DbFMCommonData.loginSuccess = false;
                string result = SyncResultTostring(syncResult);
                LoginResult loginresult = JsonConvert.DeserializeObject<LoginResult>(result);
                if (loginresult.r == 0)
                {
                    //保存登录token
                    DbFMCommonData.Token = loginresult.token;
                    DbFMCommonData.Expire = loginresult.expire;
                    DbFMCommonData.NickName = loginresult.user_name;
                    DbFMCommonData.Email = loginresult.email;
                    DbFMCommonData.UserID = loginresult.user_id;
                    DbFMCommonData.loginSuccess = true;
                }else if(loginresult.r == 1)
                {
                    MessageBox.Show(loginresult.err);
                    return false;
                }
                else
                {
                    MessageBox.Show(loginresult.err);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ResultCodeInfo" + ex.Message);
                return false;
            }
        }

     
        /// <summary>
        /// HttpPsot功能函数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="asyncCallback">请求返回</param>
        public static void httpPost(string url, AsyncCallback asyncCallback)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers["Cache-Control"] = "no-cache";
            req.Headers["Pragma"] = "no-cache";
            //req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.AllowAutoRedirect = true;
            IAsyncResult token = req.BeginGetResponse(asyncCallback, req);
        }
        public static void GetChannelList()
        {
            App.ViewModel.IsLoaded = false;
            System.Diagnostics.Debug.WriteLine("获取频道列表url：" + DbFMCommonData.ChannelListUrl);
            try
            {
                HttpHelper.httpGet(DbFMCommonData.ChannelListUrl, new AsyncCallback((ar) =>
                {
                    string result = SyncResultTostring(ar);
                    if (!string.IsNullOrEmpty(result))
                    {
                        DbFMCommonData.ChannelList = JsonConvert.DeserializeObject<ChannelList>(result);
                        DbFMCommonData.DownLoadSuccess = true;
                        App.ViewModel.LoadData();

                    }
                    else
                    {
                        App.ViewModel.IsLoaded = true;
                        //加载失败
                    }
                }));
            }catch(Exception e){
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
                App.ViewModel.IsLoaded = true;
            }
        }
        /// <summary>
        /// 操作歌曲/获取歌曲信息
        /// </summary>
        /// <param name="type"> 报告类型b=bye e=end s=skip r=rate u=urate以上需要songid n=new p=playing</param>
        /// <param name="channelId">hz id</param>
        /// /// <param name="songId">要操作的歌曲id</param>
        public static void GetChannelSongs(string type, string channelId, string songId = null)
        {
            if (WpStorage.isoFile.FileExists("SongsLoaded"))
            {
                WpStorage.isoFile.DeleteFile("SongsLoaded");
            }
            bool loadSuccess = false;
            try
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
                }
                getChannelSongsUrl += "&type="+type;
                System.Diagnostics.Debug.WriteLine("操作歌曲url：" + getChannelSongsUrl);
                HttpHelper.httpGet(getChannelSongsUrl, new AsyncCallback((ar) =>
                {
                    string result = SyncResultTostring(ar);

                    //标识新列表请求已经返回
                    WpStorage.CreateFile("SongsLoaded");
                    WpStorage.SaveStringToIsoStore("CurrentSongs.dat", result);

                    if (!string.IsNullOrEmpty(result))
                    {
                        SongResult songresult = JsonConvert.DeserializeObject<SongResult>(result);
                        if (songresult.r == 0)
                        {
                            DbFMCommonData.PlayingSongs = songresult.song;
                            loadSuccess = true;
                        }
                    }
                    else
                    {
                        //加载失败
                    }
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadSongBack, loadSuccess);
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadSongBack, loadSuccess);
            }
        }
        public static void GetCollectChannelList()
        {
            System.Diagnostics.Debug.WriteLine("获取收藏频道列表url：" + DbFMCommonData.CollChannelListUrl);
            try
            {
                HttpHelper.httpGet(DbFMCommonData.CollChannelListUrl, new AsyncCallback((ar) =>
                {
                    string result = SyncResultTostring(ar);
                    if (!string.IsNullOrEmpty(result))
                    {
                        DbFMCommonData.ChannelList = JsonConvert.DeserializeObject<ChannelList>(result);
                    }
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
            }
        }
      
        public static void DownLoadMusic(string songInfo)
        {
            try
            {
                SongInfo song = JsonConvert.DeserializeObject<SongInfo>(songInfo);

                HttpHelper.httpGet(song.url, new AsyncCallback((ar) =>
                {
                    byte[] data = SyncResultToByte(ar);
                    if (data != null)
                    {
                        WpStorage.SaveFilesToIsoStore(DbFMCommonData.DownSongsIsoName + song.aid + ".mp3", data);

                    }
                }),songInfo);
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DownLoadMusic ex" + e.Message);
            }
        }
        /// <summary>
        /// 更改歌曲是否红心状态
        /// </summary>
        /// <param name="status"></param>
        public static void SetSongLoveStatus(bool status)
        {
        }
        /// <summary>
        /// HttpGet功能函数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="asyncCallback">请求返回</param>
        public static void httpGet(string url, AsyncCallback asyncCallback,string cookieTag = null)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            req.AllowAutoRedirect = true;
            //if (!string.IsNullOrEmpty(cookieTag))
            //{
            //    req.Headers["Cookie"] = "Tag="+cookieTag ;
            //    //Cookie c = new Cookie("Tag",cookieTag);
            //    //req.CookieContainer = new CookieContainer();
            //    //req.CookieContainer.Add(url,c);
            //}
            IAsyncResult token = req.BeginGetResponse(asyncCallback, req);
        }
    }
}
