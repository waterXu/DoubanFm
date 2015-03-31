using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanAudioAgent
{
    public class SongInfo
    {
        /// <summary>
        /// 歌曲id
        /// </summary>
        public string sid { get; set; }
        /// <summary>
        /// 专辑id
        /// </summary>
        public string aid { get; set; }
        /// <summary>
        /// 专辑位置
        /// </summary>
        public string album { get; set; }
        /// <summary>
        /// 专辑名称
        /// </summary>
        public string albumtitle { get; set; }
        /// <summary>
        /// 歌手
        /// </summary>
        public string artist { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string company { get; set; }
        /// <summary>
        /// 千位每秒
        /// </summary>
        public string kbps { get; set; }
        /// <summary>
        /// 播放长度 s
        /// </summary>
        public string length { get; set; }
        /// <summary>
        /// 是否红心？
        /// </summary>
        public string like { get; set; }
        /// <summary>
        /// 歌曲图片
        /// </summary>
        public string picture { get; set; }
        /// <summary>
        /// 歌曲名字
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 歌曲地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 发行时间
        /// </summary>
        public string public_time { get; set; }
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
