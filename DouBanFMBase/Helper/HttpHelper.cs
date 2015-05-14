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
using DouBanFMBase.Resources;

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
                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }
                LoginResult loginresult = null;
                try 
                {
                     loginresult = JsonConvert.DeserializeObject<LoginResult>(result);

                }
                catch
                {
                    return false;
                }
                if (loginresult != null && loginresult.r == 0)
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
                        try
                        {
                            DbFMCommonData.ChannelList = JsonConvert.DeserializeObject<ChannelList>(result);
                        }
                        catch
                        {
                            return ;
                        }
                        
                        DbFMCommonData.DownLoadSuccess = true;
                        CallbackManager.currentPage.Dispatcher.BeginInvoke(() =>
                        {
                            App.ViewModel.LoadData();
                        });
                    }
                    else
                    {
                        App.ViewModel.IsLoaded = true;
                        DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadedData, App.ViewModel.IsLoaded);

                        //加载失败
                    }
                }));
            }catch(Exception e){
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
                App.ViewModel.IsLoaded = true;
                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadedData, App.ViewModel.IsLoaded);

            }
        }
        /// <summary>
        /// 操作歌曲/获取歌曲信息
        /// </summary>
        /// <param name="type"> 报告类型b=bye e=end s=skip r=rate u=urate以上需要songid n=new p=playing</param>
        /// <param name="channelId">hz id</param>
        /// /// <param name="songId">要操作的歌曲id</param>
        public static void OperationChannelSongs(string type, string channelId = null, string songId = null)
        {
            if (string.IsNullOrEmpty(type))
            {
                return;
            }
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

                    if (!string.IsNullOrEmpty(result))
                    {
                        //标识新列表请求已经返回
                        WpStorage.CreateFile("SongsLoaded");
                        WpStorage.SaveStringToIsoStore("CurrentSongs.dat", result);
                        SongResult songresult = null;
                        try
                        {
                            songresult = JsonConvert.DeserializeObject<SongResult>(result);
                        }
                        catch
                        {
                            return;
                        }
                        if (songresult!=null && songresult.r == 0)
                        {
                            DbFMCommonData.PlayingSongs = songresult.song;
                            loadSuccess = true;
                        }
                       
                    }
                    //表示操作的是歌曲
                    if (songId == null)
                    {
                        DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadSongBack, loadSuccess);
                    }
                    else
                    {
                        DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.OperationBack, loadSuccess,type);
                    }
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
                if (songId == null)
                {
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.LoadSongBack, loadSuccess);
                }
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
                        try
                        {
                            DbFMCommonData.ChannelList = JsonConvert.DeserializeObject<ChannelList>(result);
                        }
                        catch
                        {
                            return;
                        }
                    }
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetChannelList Exception：" + e.Message);
            }
        }

        public static void DownLoadMusic(SongInfo song)
        {
            DbFMCommonData.DownLoadedSong = false;
            bool downLoadSuccess = false;
            try
            {
                HttpHelper.httpGet(song.url, new AsyncCallback((ar) =>
                {
                    byte[] data = SyncResultToByte(ar);
                    if (data != null)
                    {
                        WpStorage.SaveFilesToIsoStore(DbFMCommonData.DownSongsIsoName + song.sid + ".mp3", data);
                        HttpHelper.httpGet(song.picture, new AsyncCallback((imgar) =>
                        {
                            byte[] imgdata = SyncResultToByte(imgar);
                            if (imgdata != null)
                            {
                                string imageType = song.picture.Remove(0, song.picture.Length - 4);
                                WpStorage.SaveFilesToIsoStore(DbFMCommonData.DownSongsIsoName + song.sid + imageType, imgdata);
                                song.url = DbFMCommonData.DownSongsIsoName + song.sid + ".mp3";
                                song.picture = DbFMCommonData.DownSongsIsoName + song.sid + imageType;
                                DbFMCommonData.DownSongIdList.Add(song.sid);
                                DbFMCommonData.DownSongsList.Add(song);
                                //App.ViewModel.LocalSongs.Add(song);
                                App.ViewModel.SaveDownSongs();
                                downLoadSuccess = true;
                                App.ShowToast("《"+song.title+"》" + AppResources.SongLoaded);
                            }
                            DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongBack, downLoadSuccess);
                        }));
                    }
                    else
                    {
                        DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongBack, downLoadSuccess);
                    }
                }));
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DownLoadMusic ex" + e.Message);
                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongBack, downLoadSuccess);
            }
        }
        /// <summary>
        /// 下载歌词
        /// </summary>
        /// <param name="song"></param>
        //public static void DownLoadSongLyr(SongInfo song, bool fromDownSong = false)
        //{
        //    bool downLoadSuccess = false;
        //    try
        //    {
        //        string loadLycUrl = DbFMCommonData.LyricUrl + song.title + "/" + song.artist;
        //        //string loadLycUrl = DbFMCommonData.LyricUrl +"天路/韩红";
        //        System.Diagnostics.Debug.WriteLine("歌词地址请求： " + loadLycUrl);
        //        HttpHelper.httpGet(loadLycUrl, new AsyncCallback((ar) =>
        //        {
        //            string result = SyncResultTostring(ar);
        //            if (result != null)
        //            {
        //                LyricResult lyric = null;

        //                try 
        //                {
        //                    lyric = JsonConvert.DeserializeObject<LyricResult>(result);
        //                }
        //                catch
        //                {
        //                    return;
        //                }
        //                if (lyric!= null && lyric.code == "0")
        //                {
        //                    if (lyric.result != null && lyric.result.Count > 0)
        //                    {
        //                        System.Diagnostics.Debug.WriteLine("歌词请求： " + lyric.result[0].lrc);
        //                        HttpHelper.httpGet(lyric.result[0].lrc, new AsyncCallback((lyricAr) =>
        //                        {
        //                            string lyricData = SyncResultTostring(lyricAr);
        //                            if (lyricData != null)
        //                            {
        //                                downLoadSuccess = true;
        //                                if (!fromDownSong)
        //                                {
        //                                    App.MusicViewModel.Lrc = lyricData;
        //                                }
        //                                else
        //                                {
        //                                    string lycUrl = DbFMCommonData.DownSongsIsoName + song.sid + ".lrc";
        //                                    WpStorage.SaveStringToIsoStore(lycUrl, lyricData);
        //                                }

        //                            }
        //                            if (!fromDownSong)
        //                            {
        //                                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
        //                            }

        //                        }));
        //                    }
        //                    else
        //                    {
        //                        if (!fromDownSong)
        //                        {
        //                            DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    if (!fromDownSong)
        //                    {
        //                        DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!fromDownSong)
        //                {
        //                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
        //                }
        //            }
        //        }));
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine("DownLoadMusic ex" + e.Message);
        //        if (!fromDownSong)
        //        {
        //            DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
        //        }
        //    }
        //}
        /// <summary>
        /// 下载歌词
        /// </summary>
        /// <param name="song"></param>
        public static void DownLoadSongLyr(SongInfo song, bool fromDownSong = false)
        {
            bool downLoadSuccess = false;
            try
            {
                string loadLycUrl = DbFMCommonData.LyricUrl + song.title + "/" + song.artist;
                //string loadLycUrl = DbFMCommonData.LyricUrl +"天路/韩红";
                System.Diagnostics.Debug.WriteLine("歌词地址请求： " + loadLycUrl);
                HttpHelper.httpGet(loadLycUrl, new AsyncCallback((ar) =>
                {
                    string result = SyncResultTostring(ar);
                    if (result != null)
                    {
                        LyricResult lyric = null;
                        try 
                        {
                            lyric = JsonConvert.DeserializeObject<LyricResult>(result);
                        }
                        catch 
                        {
                            DownLoadBaiduSongLyr(song, fromDownSong);
                            return;
                        }
                        if (lyric.code == "0")
                        {
                            if (lyric.result != null && lyric.result.Count > 0)
                            {
                                string lyricUrl = lyric.result[0].lrc;
                                System.Diagnostics.Debug.WriteLine("歌词请求： " + lyricUrl);
                                HttpHelper.httpGet(lyricUrl, new AsyncCallback((lyricAr) =>
                                {
                                    string lyricInfo = SyncResultTostring(lyricAr);
                                    if (lyricInfo != null)
                                    {
                                        if (!fromDownSong)
                                        {
                                            App.MusicViewModel.Lrc = lyricInfo;
                                            downLoadSuccess = true;
                                            DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                                        }
                                        else
                                        {
                                            string lycUrl = DbFMCommonData.DownSongsIsoName + song.sid + ".lrc";
                                            WpStorage.SaveStringToIsoStore(lycUrl, lyricInfo);
                                        }

                                    }
                                    else
                                    {
                                        DownLoadBaiduSongLyr(song, fromDownSong);
                                    }
                                }));
                            }
                            else
                            {
                                DownLoadBaiduSongLyr(song, fromDownSong);
                            }

                        }
                        else
                        {
                            DownLoadBaiduSongLyr(song, fromDownSong);
                        }
                    }
                    else
                    {
                        DownLoadBaiduSongLyr(song, fromDownSong);
                    }
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DownLoadMusic ex" + e.Message);
                DownLoadBaiduSongLyr(song, fromDownSong);
            }
        }

        /// <summary>
        /// 下载百度音乐歌词
        /// </summary>
        /// <param name="song"></param>
        public static void DownLoadBaiduSongLyr(SongInfo song, bool fromDownSong = false)
        {
            bool downLoadSuccess = false;
            try
            {
                string loadLycUrl = DbFMCommonData.BaiduLyricUrl + song.title + "-" + song.artist;
                System.Diagnostics.Debug.WriteLine("歌词地址请求： " + loadLycUrl);
                HttpHelper.httpGet(loadLycUrl, new AsyncCallback((ar) =>
                {
                    string result = SyncResultTostring(ar);
                    if (result != null)
                    {
                        BaiduLyricResult lyric = null;
                        try
                        {
                            lyric = JsonConvert.DeserializeObject<BaiduLyricResult>(result);
                        }
                        catch 
                        {
                            if (!fromDownSong)
                            {
                                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                            }
                        }
                        if (lyric != null && lyric.error_code !=null && lyric.error_code == "22000" &&　lyric.result != null && lyric.result.song_info != null && lyric.result.song_info.song_list != null && lyric.result.song_info.song_list.Count > 0 && lyric.result.song_info.song_list[0].lrclink != null)
                        {
                                BaiduSongList songInfo = lyric.result.song_info.song_list[0];
                                if (songInfo != null && songInfo.song_id != null)
                                {
                                    string getSongUrl = DbFMCommonData.BaiduMp3Host + DbFMCommonData.GetBaiduSongForId + "songIds=" + songInfo.song_id;
                                    HttpHelper.httpGet(getSongUrl, new AsyncCallback((songAr) =>
                                    {
                                        string songData = SyncResultTostring(songAr);
                                        BaiduMp3Result songListResult = null;
                                        try
                                        {
                                            songListResult = JsonConvert.DeserializeObject<BaiduMp3Result>(songData);
                                        }
                                        catch 
                                        {
                                            if (!fromDownSong)
                                            {
                                                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                                            }
                                        }
                                        if (songListResult != null && songListResult.errorCode == "22000" && songListResult.data != null && songListResult.data.songList != null && songListResult.data.songList.Count >0)
                                        {
                                            string songLrc = DbFMCommonData.BaiduMp3Host + songListResult.data.songList[0].lrcLink;
                                            //songLrc = "http://ting.baidu.com/data2/lrc/15265710/15265710.lrc";
                                            System.Diagnostics.Debug.WriteLine("歌词请求： " + songLrc);
                                            HttpHelper.httpGet(songLrc, new AsyncCallback((lyricAr) =>
                                            {
                                                string lyricData = SyncResultTostring(lyricAr);
                                                if (lyricData != null)
                                                {
                                                    downLoadSuccess = true;
                                                    if (!fromDownSong)
                                                    {
                                                        App.MusicViewModel.Lrc = lyricData;
                                                    }
                                                    else
                                                    {
                                                        string lycUrl = DbFMCommonData.DownSongsIsoName + song.sid + ".lrc";
                                                        WpStorage.SaveStringToIsoStore(lycUrl, lyricData);
                                                    }
                                                }
                                                if (!fromDownSong)
                                                {
                                                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                                                }
                                            }));
                                        }
                                        else 
                                        {
                                            if (!fromDownSong)
                                            {
                                                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                                            }
                                        }                              
                                    }));
                            }
                            else
                            {
                                if (!fromDownSong)
                                {
                                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                                }
                            }

                        }
                        else
                        {
                            if (!fromDownSong)
                            {
                                DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                            }
                        }
                    }
                    else
                    {
                        if (!fromDownSong)
                        {
                            DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                        }
                    }
                }));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DownLoadMusic ex" + e.Message);
                if (!fromDownSong)
                {
                    DbFMCommonData.informCallback((int)DbFMCommonData.CallbackType.DownSongLyrBack, downLoadSuccess);
                }
            }
        }
        /// <summary>
        /// HttpGet功能函数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="asyncCallback">请求返回</param>
        public static void httpGet(string url, AsyncCallback asyncCallback,string cookieTag = null)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "GET";
                req.AllowAutoRedirect = true;
                IAsyncResult token = req.BeginGetResponse(asyncCallback, req);
            }
            catch { }
        }
        /// <summary>
        /// HttpPsot功能函数
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="asyncCallback">请求返回</param>
        public static void httpPost(string url, AsyncCallback asyncCallback)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "POST";
                //req.Headers["Cache-Control"] = "no-cache";
                //req.Headers["Pragma"] = "no-cache";
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.101 Safari/537.36";
                //req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                req.AllowAutoRedirect = true;
                IAsyncResult token = req.BeginGetResponse(asyncCallback, req);
            }
            catch { }
        }
    }
}
