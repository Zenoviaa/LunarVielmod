using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Hyua
{
    public class RingedAlcd : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BoomCirle");
			Main.projFrames[Projectile.type] = 30;
		}
		public override void SetDefaults()
		{

			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.width = 125;
			Projectile.height = 125;
			Projectile.friendly = true;
			Projectile.timeLeft = 255;
			Projectile.scale = 1f;
			
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override bool PreAI()
		{

			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 30)
				{
					Projectile.frame = 0;
				}
			}

			Projectile.tileCollide = false;

			return true;
		}
		public override void AI()
		{
			Projectile.rotation -= 0.2f;


			Projectile.velocity *= 0.97f;

			Timer++;

			if (Timer < 255)
			{
				Projectile.alpha++;
			}

		
		}





		




		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
			overWiresUI.Add(index);
		}

	
		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, 0f, -2f, 0, default, .8f);
				Main.dust[num1].noGravity = true;
				Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num1].position != Projectile.Center)
					Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, 0f, -2f, 0, default, .8f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num].position != Projectile.Center)
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
			}

		}

		


		
	}
}