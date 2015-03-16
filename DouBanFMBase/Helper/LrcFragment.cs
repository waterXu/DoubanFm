using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase
{
    public class LrcFragment
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        /// 歌词内容
        /// </summary>
        public string LrcText { get; set; }
        public LrcFragment(string text, long start, long duration)
        {
            StartTime = start;
            Duration = duration;
            LrcText = text;
        }
        public LrcFragment(string text, long start)
        {
            StartTime = start;
            Duration = 0;
            LrcText = text;
        }
    }
}
