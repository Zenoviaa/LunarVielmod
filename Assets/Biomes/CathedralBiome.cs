using Microsoft.Xna.Framework;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    // Shows setting up two basic biomes. For a more complicated example, please request.
    public class CathedralBiome : ModBiome
	{
		public bool IsPrimaryBiome = true; // Allows this biome to impact NPC prices

		public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
		// Select all the scenery
		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/CathedralWaterStyle"); // Sets a water style for when inside this biome
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/CathedralBackgroundStyle");
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

		// Select Music
		
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/OggMoonblight");
		




		// Populate the Bestiary Filter
		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		// Use SetStaticDefaults to assign the display name
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cathedral of the Moon");
		}
		public override void SpecialVisuals(Player player, bool isActive)
		{

			//   if (!SkyManager.Instance["Stellamod:NaxtrinSky"].IsActive() && isActive)
		//		SkyManager.Instance.Activate("Stellamod:NaxtrinSky", player.Center);
		//	if (SkyManager.Instance["Stellamod:NaxtrinSky"].IsActive() && !isActive)
		//		SkyManager.Instance.Deactivate("Stellamod:NaxtrinSky");
		}
		// Calculate when the biome is active.
		public override bool IsBiomeActive(Player player)
		{
			// First, we will use the exampleBlockCount from our added ModSystem for our first custom condition
			bool b1 = ModContent.GetInstance<CathedralBiomeTileCount>().BlockCount >= 20;

			

			// Finally, we will limit the height at which this biome can be active to above ground (ie sky and surface). Most (if not all) surface biomes will use this condition.
			bool b3 = player.ZoneNormalSpace || player.ZoneOverworldHeight;
			return b1 && b3;
		}
	}
}
