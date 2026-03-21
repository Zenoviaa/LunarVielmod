using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills
{
    public class ForestBiome : BaseUrdveilBiome
    {        // Select all the scenery
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => -1;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InForest;


        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneForest = true;

        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.GetModPlayer<BiomePlayer>().ZoneForest = false;
        }
    }
    public class SpringHillsBiome : BaseUrdveilBiome
    {
        // Select all the scenery
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/SpringFields");
        public override void SpecialVisuals(Player player, bool isActive)
        {

        }

        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InSpringHills;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneSpringHills = true;

        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.GetModPlayer<BiomePlayer>().ZoneSpringHills = false;
        }
    }
}
