using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes;

public class Colosseum : ModBiome
{

    public override int Music
    {
        get
        {
            if (NPC.AnyNPCs(ModContent.NPCType<ColosseumWaveManager>()))
            {
                return AssetReferences.Assets.Music.The_Gintzing_Winds.Slot;//MusicLoader.GetMusicSlot(Mod, "Assets/Music/The_Gintzing_Winds");
            }
            return -1;
        }
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => MapBackground;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InColosseum;
    public override void OnEnter(Player player)
    {
        player.ZoneColloseum = true;
        player.ZoneDesert = true;
    }
    public override void OnLeave(Player player)
    {
        player.ZoneColloseum = false;
        player.ZoneDesert = false;
    }
}
