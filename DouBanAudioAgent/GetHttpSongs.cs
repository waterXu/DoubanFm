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

namespace DouBanAudioAgent
{
    class GetHttpSongs
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
        /// <summary>
        /// 操作歌曲/获取歌曲信息
        /// </summary>
        /// <param name="type"> 报告类型b=bye e=end s=skip r=rate u=urate以上需要songid n=new p=playing</param>
        /// <param name="channelId">hz id</param>
        /// /// <param name="songId">要操作的歌曲id</param>
        public static void GetChannelSongs()
        {
            WpStorage.SetIsoSetting("SongsLoaded", null);
            try
            {
                string getChannelSongsUrl = null;
                if (WpStorage.GetIsoSetting("SongsUrl") != null)
                {
                    getChannelSongsUrl = WpStorage.GetIsoSetting("SongsUrl").ToString();
                }
                System.Diagnostics.Debug.WriteLine("操作歌曲url：" + getChannelSongsUrl);
                httpGet(getChannelSongsUrl, new AsyncCallback((ar) =>
                {
                    WpStorage.SetIsoSetting("SongsLoaded",true);
                    string result = SyncResultTostring(ar);
                    WpStorage.SetIsoSetting("Songs",result);
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
                WpStorage.SetIsoSetting("SongsLoaded", true);
            }
        }
        /// <summary>
        /// HttpGet功能函数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="asyncCallback">请求返回</param>
        public static void httpGet(string url, AsyncCallback asyncCallback)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            req.AllowAutoRedirect = true;
            IAsyncResult token = req.BeginGetResponse(asyncCallback, req);
        }
     }
}
