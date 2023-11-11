using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Dicein : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dicein");
			Main.projFrames[Projectile.type] = 30;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 58;
			Projectile.height = 58;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 180;
			Projectile.scale = 1.3f;
			DrawOriginOffsetY = -110;
			
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI()
        {
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			Main.dust[dust].noGravity = true;

			Vector3 RGB = new(2.55f, 2.55f, 0.94f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

			Player player = Main.player[Projectile.owner];
			Projectile.Center = player.Center;

		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
			overWiresUI.Add(index);
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
			return true;

			
		}
	
	}
}