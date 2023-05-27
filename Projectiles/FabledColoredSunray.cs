using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
	public class FabledColoredSunray : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BoomCirle");
		}
		public override void SetDefaults()
		{
			Projectile.width = 360;
			Projectile.height = 360;

			Projectile.timeLeft = 2000;
			Projectile.scale = 2f;
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

			if (Timer < 200)
            {
				Projectile.alpha++;
			}
			
			if (Timer > 200)
            {
				Projectile.alpha--;
			}
			
			if(Timer == 400)
            {
				Timer = 0;
            }
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - (float)Projectile.alpha / 255f);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
			overWiresUI.Add(index);
		}
	}
}