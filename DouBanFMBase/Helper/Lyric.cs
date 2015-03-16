using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanFMBase
{
    public class Lyric
    {
        public List<LrcFragment> Fragments { get; private set; }
        public Lyric(string lrcText)
        {
            Fragments = new List<LrcFragment>();
            ParseLyricText(lrcText);
        }

        public void ParseLyricText(string lrcText)
        {
            StringReader reader = new StringReader(lrcText);
            string  line = null;
            //一行一行解析
            while ((line = reader.ReadLine()) != null)
            {
                ParseLine(line.Trim());
            }
            //对fragments按开始时间进行排序
           // Fragments.Sort( new LrcFragmentComparer());

            //对Fragments集合中的每个Fragment计算结束时间
            ComputeEnTime();
        }

        private void ComputeEnTime()
        {
            throw new NotImplementedException();
        }

        private void ParseLine(string p)
        {
            Fragments.Add(new LrcFragment(p,2));
        }
        // 根据播放进度来获得当前进度下所应播放的歌词片段index
        // progress为播放进度，成功返回true
        //public bool GetFragmentIndex(long progress, out int index)
        //{
        //    return true;
        //}
    }
}
