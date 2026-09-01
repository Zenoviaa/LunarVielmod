using Stellamod.Assets.Biomes;
using Stellamod.Content.Biomes;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror
{
    public class CrimsonBridewellBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<StarbloomWaterStyle>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

        public override int Music
        {
            get
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Aegislav");
            }
        }


        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InCrimsonBridewell;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.ZoneCrimsonBridewell = true;
        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.ZoneCrimsonBridewell = false;
        }
    }
}
