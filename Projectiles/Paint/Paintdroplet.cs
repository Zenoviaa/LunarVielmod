using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Dusts;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Paint
{
    public class Paintdroplet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 28;
		}
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = 14;
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 56;
			Projectile.scale = 0.8f;
			Projectile.tileCollide = true;

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
				if (++Projectile.frame >= 28)
				{
					Projectile.frame = 0;
				}
			}


			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob2>(), 0f, 0f);
			Main.dust[dust].scale = 1f;
			return true;
		}
	}
}
