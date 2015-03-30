using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase
{
    public class LrcFragment : IComparable<LrcFragment>
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public double StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public double Duration { get; set; }
        /// <summary>
        /// 歌词内容
        /// </summary>
        public string LrcText { get; set; }
        public LrcFragment(string text, double start, double duration)
        {
            StartTime = start;
            Duration = duration;
            LrcText = text;
        }
        public LrcFragment(string text, double start)
        {
            StartTime = start;
            Duration = 0;
            LrcText = text;
        }

        public int CompareTo(LrcFragment other)
        {
            if (other == null)
                return 1;
            if (this.StartTime > other.StartTime)
            {
                return 1;
            }
            else if (this.StartTime < other.StartTime)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
