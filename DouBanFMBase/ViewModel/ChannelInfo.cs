using DouBanAudioAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase
{
    public class ChannelList
    {
        public List<ChannelInfo> Channels { get; set; }
    }
    public class ChannelInfo
    {
        /// <summary>
        /// 兆赫名称
        /// </summary>
        public string name { get; set; }
        public string seq_id { get; set; }
        /// <summary>
        /// 兆赫英文名
        /// </summary>
        public string name_en { get; set; }
        /// <summary>
        /// 兆赫id
        /// </summary>
        public string channel_id { get; set; }
        /// <summary>
        /// 兆赫缩写
        /// </summary>
        public string abbr_en { get; set; }
      
    }
    public class SongResult
    {
        /// <summary>
        /// 返回code
        /// </summary>
        public int r { get; set; }
        /// <summary>
        /// r为1时值有效，错误信息
        /// </summary>
        public string err { get; set; }
        /// <summary>
        /// 歌曲信息列表
        /// </summary>
        public List<SongInfo> song { get; set; }
    }
   
}
