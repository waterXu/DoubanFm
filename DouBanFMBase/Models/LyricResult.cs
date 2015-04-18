using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase
{
    public class LyricResult
    {
        public string count { get; set; }
        public string code { get; set; }
        public List<LyricInfo> result { get; set; }
    }
    public class LyricInfo
    {
        public string aida { get; set; }
        public string artist_id { get; set; }
        public string song { get; set; }
        public string lrc { get; set; }
        public string sid { get; set; }
    }
    public class BaiduLyricResult
    {
        /// <summary>
        /// code 为22000 返回正确
        /// </summary>
        public string error_code { get; set; }
        public BaiduLyricResultInfo result { get; set; }
    }
    public class BaiduLyricResultInfo
    {
        public string query { get; set; }
        public string rqt_type { get; set; }
        public BaiduLyricInfo song_info { get; set; }
    }
    public class BaiduLyricInfo
    {
        public string total { get; set; }
        public List<BaiduSongList> song_list { get; set; }
    }
    public class BaiduSongList 
    {
        public string lrclink { get; set; }
        public string song_id { get; set; }
    }
}
