using Microsoft.Xna.Framework;
using Stellamod.Backgrounds;
using Stellamod.Core.Biomes;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills
{
    // Shows setting up two basic biomes. For a more complicated example, please request.
    public class XixVillageBiome : BaseUrdveilBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int Music
        {
            get
            {
                if (Main.dayTime)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Witchtown4");
                }
                else
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/LibraryWorld");
                }
            }
        }


        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InXixVillage;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<MyPlayer>().ZoneVillage = true;
        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.GetModPlayer<MyPlayer>().ZoneVillage = false;
        }
    }
}
