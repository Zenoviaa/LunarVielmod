using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common;

public static class MusicPlayerExtensions
{
    extension(Player player)
    {
        public bool DisableMusic
        {
            get => player.GetModPlayer<MusicPlayer>().noMusic;
            set => player.GetModPlayer<MusicPlayer>().noMusic = value;
        }
    }
}
public class MusicPlayer : ModPlayer
{
    public bool noMusic;
    public override void ResetEffects()
    {
        base.ResetEffects();
        noMusic = false;
    }
}
public class NoMusicBiome : ModBiome
{
    // Select Music
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    public override int Music
    {
        get
        {
            return 0;
        }
    }
    public override bool IsBiomeActive(Player player)
    {
        if (player.DisableMusic)
        {
           // Main.NewText("no music");
        }
        return player.DisableMusic;
    }
}
