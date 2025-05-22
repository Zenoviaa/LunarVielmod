using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class BackgroundOrb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Background Orb");
			Main.projFrames[Projectile.type] = 20;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 147;
			Projectile.height = 147;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 200;
			Projectile.scale = 1.5f;
			
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


			Timer++;
			if (Timer < 150)
			{
				Projectile.scale -= 0.01f;
			}


			if (Projectile.scale == 0f)
            {
				Projectile.Kill();
            }


			
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