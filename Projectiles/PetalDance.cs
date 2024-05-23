using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Projectiles
{
    public class PetalDance : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
		private ref float SwordRotation => ref Projectile.ai[1];
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("death");
			Main.projFrames[Projectile.type] = 14;
		}
		public override void SetDefaults()
		{
			Projectile.damage = 0;
			Projectile.width = 340;
			Projectile.height = 260;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 600;
			Projectile.scale = 0.3f;
			DrawOriginOffsetX = -90;
			DrawOriginOffsetY = -8;
		}

		public override void AI()
		{
			Timer++;
			Player player = Main.player[Projectile.owner];
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			if (Main.myPlayer == Projectile.owner)
			{
				player.ChangeDir(Projectile.direction);
				SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
				Projectile.netUpdate = true;
			}

			Projectile.velocity = SwordRotation.ToRotationVector2();		
			if (Timer == 1)
				player.AddBuff(ModContent.BuffType<Elegance>(), 600);
			if (!Projectile.active)
				player.ClearBuff(ModContent.BuffType<Elegance>());

			Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position
			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 14)
				{
					Projectile.frame = 0;
				}
			}
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}
	}
}