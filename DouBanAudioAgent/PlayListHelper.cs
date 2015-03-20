using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DouBanAudioAgent
{
    public class PlayListHelper
    {
        private static WebClient wc = new WebClient();

        static void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine("wc_DownloadStringCompleted  error：" + e.Error);
                return;
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
            getChannelSongsUrl = WpStorage.readIsolatedStorageFile("SongsUrl.dat");
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
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ReFreshSongList exception：" + e.Message);
            }
        }
    }
}
