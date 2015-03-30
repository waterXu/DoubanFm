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
}
