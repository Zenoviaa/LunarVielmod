using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    // Shows setting up two basic biomes. For a more complicated example, please request.
    public class AlcadziaBiome : ModBiome
	{
		public bool IsPrimaryBiome = true; // Allows this biome to impact NPC prices
		

		// Select all the scenery
		public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/StarbloomWaterStyle"); // Sets a water style for when inside this biome
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/StarbloomBackgroundStyle");
		public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

		// Select Music
		public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;



		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/AlcadizHurricane");
		public override void SpecialVisuals(Player player, bool isActive)
		{

			   if (!SkyManager.Instance["Stellamod:AlcadSky"].IsActive() && isActive)
					SkyManager.Instance.Activate("Stellamod:AlcadSky", player.Center);
				if (SkyManager.Instance["Stellamod:AlcadSky"].IsActive() && !isActive)
					SkyManager.Instance.Deactivate("Stellamod:AlcadSky");
		
		}

		// Populate the Bestiary Filter

		public override bool IsBiomeActive(Player player) => BiomeTileCounts.InRoyalCapital;

		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		// Use SetStaticDefaults to assign the display name
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cathedral of the Moon");
		}


		public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAlcadzia = true;
		public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAlcadzia = false;
		// Calculate when the biome is active.

		
	}
}
