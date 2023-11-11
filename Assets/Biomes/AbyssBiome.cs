
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Assets.Biomes
{
    public class AbyssBiome : ModBiome
    {
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("SpiritMod/Biomes/SpiritUgBgStyle");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Hidding_In_The_Shadows");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override bool IsBiomeActive(Player player) => (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAbyss;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAbyss = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAbyss = false;
    }
}