using Microsoft.Xna.Framework;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Voyager
{
	public class Ink1 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BoomCirle");
		
		}
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 510;

			Projectile.timeLeft = 255;
			Projectile.scale = 0.2f;
			
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override bool PreAI()
		{


			Projectile.tileCollide = false;

			return true;
		}
		public override void AI()
		{


			Timer++;

			if (Timer < 255)
			{
				Projectile.alpha++;
			}

			if (Timer == 254)
			{
				ShakeModSystem.Shake = 4;
				float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
				float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<AlcaricMushBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
				Projectile.Kill();
			}
		}



		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
			overWiresUI.Add(index);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ShakeModSystem.Shake = 5;
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<AlcaricMushBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen);
			Projectile.Kill();




		}
	}
}