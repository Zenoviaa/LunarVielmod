using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class FrostKaboom : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 20;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 128;
			Projectile.height = 129;
			
			Projectile.timeLeft = 40;
			Projectile.scale = 1.2f;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI()
        {
			Projectile.rotation += 0.03f;
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 20)
				{
					Projectile.frame = 0;
				}
			}
			return true;

			
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);

		}
	}

}