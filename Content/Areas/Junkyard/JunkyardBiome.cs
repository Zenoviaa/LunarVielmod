using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard;

public class JunkyardBiome : ModBiome
{
    public override int Music
    {
        get
        {
            //Put your if statement here

            //Normal music
            return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Junkyard");
        }
    }
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => MapBackground;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player)
    {
        return BiomeTileCounts.InJunkyard && !player.ZoneOverworldHeight;
    }

    public override void OnEnter(Player player) => player.GetModPlayer<BiomePlayer>().ZoneJunkyard = true;
    public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneJunkyard = false;
}