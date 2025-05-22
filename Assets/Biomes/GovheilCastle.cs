
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class GovheilCastle : ModBiome
	{
		// Select all the scenery


		// Select Music
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/GovheilCastle");
		public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
		public override string BestiaryIcon => base.BestiaryIcon;
		public override string BackgroundPath => MapBackground;
		public override Color? BackgroundColor => base.BackgroundColor;
		// Calculate when the biome is active.
		public override bool IsBiomeActive(Player player) => BiomeTileCounts.InGovheil;

		public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneGovheil = true;
		public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneGovheil = false;


		// In the event that both our biome AND one or more modded SceneEffect layers are active with the same SceneEffect Priority, this can decide which one.
		// It's uncommon that need to assign a weight - you'd have to specifically believe that you don't need higher SceneEffectPriority, but do need to be the active SceneEffect within the priority you designated
		// In this case, we don't need it, so this inclusion is purely to demonstrate this is available.
		// See the GetWeight documentation for more information.
		/*
		public override float GetWeight(Player player) {
			int distanceToCenter = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2);
			// We declare that our biome should have be more likely than not to be active if in center 1/6 of the world, and decreases in need to be active as player gets further away to the 1/3 mark.
			if (distanceToCenter <= Main.maxTilesX / 12) {
				return 1f;
			}
			else {
				return 1f - (distanceToCenter - Main.maxTilesX / 12) / (Main.maxTilesX / 12);
			}
		}
		*/
	}
}