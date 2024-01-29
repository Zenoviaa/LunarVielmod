using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Ambient
{
    public class CrystalRay : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BoomCirle");
		}
		public override void SetDefaults()
		{
			Projectile.width = 278;
			Projectile.height = 2000;

			Projectile.timeLeft = 320;
			Projectile.scale = 1f;
			Projectile.alpha = 255;
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

			if (Timer < 160)
			{
				Projectile.alpha--;
			}

			if (Timer > 160)
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
	}
}