using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
	public class STARSHOTT : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 444;
			Projectile.height = 232;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 16;
			Projectile.scale = 0.6f;
            Projectile.tileCollide = false;
        }

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
		}

		public override bool PreAI()
		{

			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 8)
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