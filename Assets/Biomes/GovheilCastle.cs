
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class GovheilCastle : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/GovheilCastle");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InGovheil;

        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneGovheil = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneGovheil = false;
    }
}