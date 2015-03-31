using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Fragments.Sort();

            //对Fragments集合中的每个Fragment计算结束时间
            //ComputeEnTime();
        }

        private void ComputeEnTime()
        {
            throw new NotImplementedException();
        }

        private void ParseLine(string line)
        {
            //不包含英文或中文的去掉  如[00:01:09]
            if (!Regex.IsMatch(line, "[\u4e00-\u9fa5]") && !Regex.IsMatch(line, "[a-zA-Z]"))
            {
                return;
            }
            TimeSpan ts;
            //按“]”号分割 line
            string[] lineInfo = line.Split(']');
            if (lineInfo.Length == 2 && string.IsNullOrEmpty(lineInfo[1]))
            {
                //例子
                //[ti:天路]
                //[ar:韩红]
                //[al:]
                //获取 歌曲信息
                string info = lineInfo[0];
                info = info.Substring(info.IndexOf(":")+1);
               
                ts = TimeSpan.Parse("00:00:00.00");
                Fragments.Add(new LrcFragment(info + "\r\n", ts.TotalMilliseconds));
            }
            else
            {
                //例子  
                //[03:10.59][02:21.53][01:03.24]don't wanna be the wind just nothing left to say 不希望你像风一样不和我交谈
                //处理相同歌词
                if (lineInfo.Length > 2)
                {
                    //获取歌词
                    string info = lineInfo[lineInfo.Length - 1];
                    for (int i = 0; i < lineInfo.Length - 2; i++)
                    {
                        string time = "00:" + lineInfo[i].Replace("[", "");
                         try
                        {
                            ts = TimeSpan.Parse(time);
                        }
                        catch (Exception e)
                        {
                            ts = TimeSpan.Parse("00:00:00.00");
                        }
                        Fragments.Add(new LrcFragment(info + "\r\n", ts.TotalMilliseconds));
                    }
                }
                else if(lineInfo.Length == 2)
                {
                    string time = "00:" + lineInfo[0].Replace("[","");
                    try
                    {
                        ts = TimeSpan.Parse(time);
                    }
                    catch (Exception e)
                    {
                        ts = TimeSpan.Parse("00:00:00.00");
                    }
                    Fragments.Add(new LrcFragment(lineInfo[1] + "\r\n", ts.TotalMilliseconds));
                }
            }
        }
        // 根据播放进度来获得当前进度下所应播放的歌词片段index
        // progress为播放进度，成功返回true
        public bool GetFragmentIndex(double progress, out int index)
        {
            index = 0;
            LrcFragment fragment;
            LrcFragment nextfragment;
            for (int i=0;i<Fragments.Count;i++)
            {
                fragment = Fragments[i];
                if(i+1 < Fragments.Count){
                    nextfragment = Fragments[i+1];
                    if(fragment.StartTime<=progress && nextfragment.StartTime>=progress)
                    {
                        index = i;
                        break;
                    }
                }
                else{
                    if (fragment.StartTime <= progress)
                    {
                        return false;
                    }
                    else
                    {
                        index = i;
                    }
                }
            }
            return true;
        }
     
    }
}
