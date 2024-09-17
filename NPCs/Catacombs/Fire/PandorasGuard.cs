using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Catacombs.Fire
{
	public class PandorasGuard : ModNPC
	{
		// States
		public enum ActionState
		{

			Jump,
			Fall,
			Wait
		}
		// Current state

		public ActionState State = ActionState.Jump;
		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;
			NPCID.Sets.TrailCacheLength[NPC.type] = 3;
			NPCID.Sets.TrailingMode[NPC.type] = 0;

		}
		public override void SetDefaults()
		{
			NPC.width = 58; // The width of the npc's hitbox (in pixels)
			NPC.height = 58; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = 41; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 90; // The amount of damage that this npc deals
			NPC.defense = 20; // The amount of defense that this npc has
			NPC.lifeMax = 1500; // The amount of health that this npc has
			NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
			NPC.value = 50f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0.5f;

		}
		public override void AI()
		{
			NPC.velocity.X *= 0.99f;
			timer++;
			if (timer == 1)
			{



				int fireball = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<KaBoom>(), 0, 0f, 0, 0f, 0f);

				Projectile ichor = Main.projectile[fireball];
				ichor.hostile = true;
				ichor.friendly = false;
			}

		}


		
		

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "They guard what is theirs!"))
			});
		}
		int frame = 0;
		public override void FindFrame(int frameHeight)
		{


			bool expertMode = Main.expertMode;
			Player player = Main.player[NPC.target];

			NPC.frameCounter += 0.5f;
			if (NPC.frameCounter >= 4)
			{
				frame++;
				NPC.frameCounter = 0;
			}
			if (frame >= 8)
			{
				frame = 0;
			}
			NPC.frame.Y = frameHeight * frame;

		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
			}
			if (NPC.life <= 0)
			{
				EventWorld.GintzeKills += 1;
				for (int i = 0; i < 5; i++)
				{
					int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Copper, 0f, -2f, 180, default, .6f);
					Main.dust[num].noGravity = true;
					Dust expr_62_cp_0 = Main.dust[num];
					expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-50, 51) / 20 - 1.5f);
					Dust expr_92_cp_0 = Main.dust[num];
					expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-50, 51) / 20 - 1.5f);
					if (Main.dust[num].position != NPC.Center)
					{
						Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
					}
				}
			}

			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Morrowpes"));
		}




		Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
		public virtual string GlowTexturePath => Texture + "_Glow";
		private Asset<Texture2D> _glowTexture;
		public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			float num108 = 4;
			float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
			float num106 = 0f;
			Color color1 = Color.LightBlue * num107 * .8f;
			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(
				GlowTexture,
				NPC.Center - Main.screenPosition + Drawoffset,
				NPC.frame,
				color1,
				NPC.rotation,
				NPC.frame.Size() / 2,
				NPC.scale,
				effects,
				0
			);
			SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
			Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
			for (int num103 = 0; num103 < 4; num103++)
			{
				Color color28 = color29;
				color28 = NPC.GetAlpha(color28);
				color28 *= 1f - num107;
				Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
				Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
		{
			Lighting.AddLight(NPC.Center, Color.Silver.ToVector3() * 0.25f * Main.essScale);
			SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
			for (int k = 0; k < NPC.oldPos.Length; k++)
			{
				Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
				Color color = NPC.GetAlpha(Color.Lerp(new Color(191, 20, 10), new Color(191, 59, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
				spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
			}

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return true;
		}
	}
}