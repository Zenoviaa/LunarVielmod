using Stellamod.Assets.Biomes;
using Stellamod.Content.Areas.Dungeon;
using Stellamod.Content.Areas.Terror;
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
        public override bool IsBiomeActive(Player player)
        {
            bool isaActive = BiomeTileCounts.InForest && player.ZoneOverworldHeight;
            if (!isaActive)
                return false;
            if (player.ZoneDesert)
                return false;
            if (player.ZoneJungle)
                return false;
            if (player.ZoneSnow)
                return false;
            if (player.ZoneBeach)
                return false;
            if (player.ZoneCorrupt)
                return false;
            if (player.ZoneCrimson)
                return false;
            if (player.ZoneDungeon)
                return false;
            if (player.InModBiome<FableBiome>())
                return false;
            if (player.InModBiome<AlcadziaBiome>())
                return false;
            if (player.InModBiome<MistyDungeonBiome>())
                return false;
            if (player.InModBiome<IlluriaBiome>())
                return false;
            if (player.InModBiome<AegislavBiome>())
                return false;

            return true;
        }


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

        public override bool IsBiomeActive(Player player)
        {
            if (!BiomeTileCounts.InSpringHills)
                return false;
            return !player.ZoneDesert;
        }
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
