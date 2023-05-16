using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant
{
	// Shows basic boss bar code using a custom colored texture. It only does visual things, so for a more practical boss bar, see the other example (MinionBossBossBar)
	// To use this, in an NPCs SetDefaults, write:
	//  NPC.BossBar = ModContent.GetInstance<ExampleBossBar>();

	// Keep in mind that if the NPC has a boss head icon, it will automatically have the common boss health bar from vanilla. A ModBossBar is not mandatory for a boss.

	// You can make it so your NPC never shows a boss bar, such as Dungeon Guardian or Lunatic Cultist Clone:
	//  NPC.BossBar = Main.BigBossProgressBar.NeverValid;
	public class BossBarTest : ModBossBar
	{
		private int VeribossHeadIndex = -1;
		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
		{
			if (VeribossHeadIndex != -1)
			{
				return TextureAssets.NpcHeadBoss[VeribossHeadIndex];
			}
			return null;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
		{
			// Make the bar shake the less health the NPC has
			float shakeIntensity = Utils.Clamp(1f - drawParams.Life/* tModPorter Note: Removed. Suggest: Life / LifeMax */ - 0.2f, 0f, 1f);
			drawParams.BarCenter.Y -= 20f;
			drawParams.BarCenter += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * shakeIntensity * 2f;

			

			return true;
		}
		public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)/* tModPorter Note: life and shield current and max values are now separate to allow for hp/shield number text draw */
		{
			// Here the game wants to know if to draw the boss bar or not. Return false whenever the conditions don't apply.
			// If there is no possibility of returning false (or null) the bar will get drawn at times when it shouldn't, so write defensive code!

			NPC npc = Main.npc[info.npcIndexToAimAt];
			if (!npc.active)
				return false;

			// We assign bossHeadIndex here because we need to use it in GetIconTexture
			VeribossHeadIndex = npc.GetBossHeadTextureIndex();

			life = Utils.Clamp(npc.life / (float)npc.lifeMax, 0f, 1f);

			return true;
		}
	}
}
