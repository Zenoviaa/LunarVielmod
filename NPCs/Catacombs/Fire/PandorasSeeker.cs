using Stellamod.Assets.Biomes;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Items.Accessories;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.Utilis;
using System;
using Terraria.Audio;
using Terraria.GameContent;

using static Terraria.ModLoader.ModContent;
using Stellamod.Projectiles;

namespace Stellamod.NPCs.Catacombs.Fire
{
	public class PandorasSeeker : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 60;
			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 0;

		}


		public enum ActionState
		{

			Speed,
			Wait
		}
		// Current state
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 42;
			NPC.height = 88;
			NPC.damage = 40;
			NPC.defense = 30;
			NPC.lifeMax = 700;
			NPC.HitSound = SoundID.NPCHit56;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 560f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 85;
			AIType = NPCID.StardustCellBig;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

		}



		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 1, -1f, 1, default, .61f);
			}


		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{

			timer++;
			NPC.spriteDirection = NPC.direction;

			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
				{
					int fireball = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 0,
						ModContent.ProjectileType<KaBoom>(), 0, 0f, Owner: Main.myPlayer);

					Projectile ichor = Main.projectile[fireball];
					ichor.hostile = true;
					ichor.friendly = false;
				}
			}

			invisibilityTimer++;
			if (invisibilityTimer >= 100)
			{
				if (StellaMultiplayer.IsHost)
				{
					int fireball = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -NPC.velocity.X, -NPC.velocity.Y,
						ProjectileID.GreekFire2, 0, 0f, Owner: Main.myPlayer);
					Projectile ichor = Main.projectile[fireball];
					ichor.hostile = true;
					ichor.friendly = false;
				}

				invisibilityTimer = 0;
			}
			NPC.noTileCollide = true;

		}

		public void Speed()
		{
			timer++;


			if (timer > 50)
			{

				NPC.velocity.X *= 5f;
				NPC.velocity.Y *= 0.5f;
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, NPC.direction, -1f, 1, default, .61f);

					if (StellaMultiplayer.IsHost)
					{
						float speedXB = NPC.velocity.X * Main.rand.NextFloat(-0.5f, 0.5f);
						float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, 4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXB * 3, speedY,
							ProjectileID.GreekFire3, 25, 0f, Owner: Main.myPlayer);
					}
				}

				if (timer == 100)
				{
					State = ActionState.Wait;
					timer = 0;
				}
			}
		}
	}
}