using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;

namespace CS_SoundPlugin
{
    public class JoinSoundConfig : BasePluginConfig
    {
        public string SoundMode { get; set; } = "order";
        public List<SoundItem> MusicList { get; set; } = new List<SoundItem>
        {
            new SoundItem
            {
                Path = "zvuki/join_sound.vsnd_c",
                Knife = "zvuki/kill_knife.vsnd_c"
            }
        };
    }
}
