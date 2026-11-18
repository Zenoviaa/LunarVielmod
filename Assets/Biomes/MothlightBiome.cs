using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class MothlightBiome : ModBiome
    {
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("SpiritMod/Biomes/SpiritUgBgStyle");
        public override int Music => AssetReferences.Assets.Music.MothlightManor.Slot;//MusicLoader.GetMusicSlot(Mod, "Assets/Music/MothlightManor");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InMothlight;
        public override void OnEnter(Player player) => player.ZoneMothlight = true;
        public override void OnLeave(Player player) => player.ZoneMothlight = false;
    }
}
