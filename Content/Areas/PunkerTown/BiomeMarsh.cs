using Microsoft.Xna.Framework;
using Stellamod.Backgrounds;
using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown
{
    public class BiomeMarsh : ModBiome
    {
        public override int Music
        {
            get
            {
                //Put your if statement here


                //Normal music
                if (Main.dayTime && Main.raining)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Acidic_Terors");
                }
                else
                {
                    return -1;
                }
            }
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<RainForestBackgroundStyle>();

        public override bool IsBiomeActive(Player player) => (player.ZoneOverworldHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InMarsh;
        public override void OnEnter(Player player) => player.GetModPlayer<BiomePlayer>().ZoneMarsh = true;
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneMarsh = false;
    }
}
