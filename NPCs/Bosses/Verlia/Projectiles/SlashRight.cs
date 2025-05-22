using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class SlashRight : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Empress's Moon Slash");
			Main.projFrames[Projectile.type] = 7;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.width = 200;
			Projectile.height = 200;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 14;
			Projectile.scale = 1.5f;
			DrawOffsetX = -100;
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
			if (Timer == 2)
            {
				Projectile.scale *= 0.98f;
				Timer = 0;
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
				if (++Projectile.frame >= 7)
				{
					Projectile.frame = 0;
				}
			}
			return true;

			
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(100, 100, 100, 0) * (1f - Projectile.alpha / 50f);
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
			behindProjectiles.Add(index);
		}

	}
}