
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;


namespace Stellamod.Assets.Biomes
{
    public class AcidBiome : ModBiome
    {
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("SpiritMod/Biomes/SpiritUgBgStyle");
        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/Acidic_Terors") : MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/Acidic_Nightmares");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/AcidWaterStyle");

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/MarrowSurfaceBackgroundStyle");

        public override bool IsBiomeActive(Player player) => (player.ZoneOverworldHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAcid;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAcid = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAcid = false;
    }
}