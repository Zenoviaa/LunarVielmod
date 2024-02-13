
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class Laboratory : ModBiome
    {

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Mechanics");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/AcidWaterStyle");

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/MarrowSurfaceBackgroundStyle");

        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InLab;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneLab = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneLab = false;
    }
}