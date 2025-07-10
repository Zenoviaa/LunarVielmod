using Microsoft.Xna.Framework;
using Stellamod.Content.Backgrounds;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Biomes
{
    public class RainforestBiome : ModBiome
    {
        // Select all the scenery
        //public override ModWaterStyle WaterStyle => ModContent.GetInstance<ExampleWaterStyle>(); // Sets a water style for when inside this biome
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<RainForestBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

        // Select Music
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/SpringFields");
        /* If you need the music choice to be conditional, such as supporting the Otherworld soundtrack toggle, you can use this approach:
		public override int Music {
			get {
				if (!Main.swapMusic == Main.drunkWorld && !Main.remixWorld) {
					return MusicID.OtherworldlyEerie;
				}
				return MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");
			}
		}
		*/

        // Populate the Bestiary Filter
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player)
        {
            return false;
        }

        // Declare biome priority. The default is BiomeLow so this is only necessary if it needs a higher priority.
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    }
}
