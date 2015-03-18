using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouBanAudioAgent
{
    public static class PlayerListSync
    {
        /// <summary>
        /// 当前播放歌曲列表
        /// </summary>
        public static List<SongInfo> currentSongList { get; set; }
        public static bool SonglistChanged = false;
    }
}
