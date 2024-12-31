using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
	public class SMALLBOMB : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 90;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 81;
			Projectile.height = 81;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 500;
			Projectile.scale = 2f;

		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public float Dream = 0f;
		public override void AI()
		{
			var entitySource = Projectile.GetSource_FromAI();
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);

			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			Projectile.velocity *= 0.99f;

			Dream++;
			if (Dream == 250)
			{
				SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
				Projectile.ownerHitCheck = true;

	
				for (int i = 0; i < 150; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
					d.noGravity = true;
				}
				
				if(Main.myPlayer == Projectile.owner)
				{
                    NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<STARLING>());
                }

				Projectile.Kill();
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 90)
				{
					Projectile.frame = 0;
				}
			}
			return true;


		}


		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
			behindProjectiles.Add(index);

		}
	}

}