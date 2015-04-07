using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DouBanAudioAgent
{
    public class PlayListHelper
    {
        private static WebClient wc = new WebClient();

        static void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            long memory = DeviceStatus.ApplicationCurrentMemoryUsage / (1024 * 1024);
            long memoryLimit = DeviceStatus.ApplicationMemoryUsageLimit / (1024 * 1024);
            long memoryMax = DeviceStatus.ApplicationPeakMemoryUsage / (1024 * 1024);
            System.Diagnostics.Debug.WriteLine("当前内存使用情况：" + memory.ToString() + " MB 当前最大内存使用情况： " + memoryMax.ToString() + "MB  当前可分配最大内存： " + memoryLimit.ToString() + "  MB");
            if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine("wc_DownloadStringCompleted  error：" + e.Error);
            }
           else if (e.Result != "")
            {
                System.Diagnostics.Debug.WriteLine("wc_DownloadStringCompleted  Result：" + e.Result);
                WpStorage.CreateFile("SongsLoaded");
                WpStorage.SaveStringToIsoStore("CurrentSongs.dat", e.Result);
            }
        }

      
        public static void ReFreshSongList()
        {
            if (WpStorage.isoFile.FileExists("SongsLoaded"))
            {
                WpStorage.isoFile.DeleteFile("SongsLoaded");
            }
            if (WpStorage.isoFile.FileExists("CurrentSongs.dat"))
            {
                WpStorage.isoFile.DeleteFile("CurrentSongs.dat");
            }
            if (wc.IsBusy)
            {
                try
                {
                    wc.CancelAsync();
                }
                catch (Exception e)
                {

                }
            }
           string getChannelSongsUrl = null;
            getChannelSongsUrl = WpStorage.ReadIsolatedStorageFile("SongsUrl.dat");
            if (string.IsNullOrEmpty(getChannelSongsUrl))
            {
                return;
            }
            try
            {
                Random random = new Random();
                int r = random.Next();
                //getChannelSongsUrl += "&r=" + r;
                System.Diagnostics.Debug.WriteLine("操作歌曲url：" + getChannelSongsUrl);
                
                wc.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.DownloadStringAsync(new Uri(getChannelSongsUrl, UriKind.Absolute));
                Thread.Sleep(1000);
                //long memory = DeviceStatus.ApplicationCurrentMemoryUsage / (1024 * 1024);
                //long memoryLimit = DeviceStatus.ApplicationMemoryUsageLimit / (1024 * 1024);
                //long memoryMax = DeviceStatus.ApplicationPeakMemoryUsage / (1024 * 1024);
                //System.Diagnostics.Debug.WriteLine("当前内存使用情况：" + memory.ToString() + " MB 当前最大内存使用情况： " + memoryMax.ToString() + "MB  当前可分配最大内存： " + memoryLimit.ToString() + "  MB");

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ReFreshSongList exception：" + e.Message);
            }
        }
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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SyncResultTostring" + e.Message);
                // todo   show  tip server not conn  如何检测是否联网
                return null;
            }

        }
        /// <summary>
        /// 操作歌曲/获取歌曲信息
        /// </summary>
        /// <param name="type"> 报告类型b=bye e=end s=skip r=rate u=urate以上需要songid n=new p=playing</param>
        /// <param name="channelId">hz id</param>
        /// /// <param name="songId">要操作的歌曲id</param>
        public static void OperationChannelSongs()
        {
             if (WpStorage.isoFile.FileExists("SongsLoaded"))
            {
                WpStorage.isoFile.DeleteFile("SongsLoaded");
            }
            if (WpStorage.isoFile.FileExists("CurrentSongs.dat"))
            {
                WpStorage.isoFile.DeleteFile("CurrentSongs.dat");
            }
            string getChannelSongsUrl = null;
            getChannelSongsUrl = WpStorage.ReadIsolatedStorageFile("SongsUrl.dat");
            if (string.IsNullOrEmpty(getChannelSongsUrl))
            {
                return;
            }
            try
            {
                System.Diagnostics.Debug.WriteLine("操作歌曲url：" + getChannelSongsUrl);
                httpGet(getChannelSongsUrl, new AsyncCallback((ar) =>
                {
                    string result = SyncResultTostring(ar);

                    if (!string.IsNullOrEmpty(result))
                    {
                        //标识新列表请求已经返回
                        WpStorage.CreateFile("SongsLoaded");
                        WpStorage.SaveStringToIsoStore("CurrentSongs.dat", result);

                    }
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
            }
        }
        /// <summary>
        /// HttpGet功能函数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="asyncCallback">请求返回</param>
        public static void httpGet(string url, AsyncCallback asyncCallback, string cookieTag = null)
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
