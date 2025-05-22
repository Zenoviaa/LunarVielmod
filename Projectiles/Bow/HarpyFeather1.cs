using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    public class HarpyFeather1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Harpy Feather");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 1;
			Projectile.damage = 60;
			AIType = ProjectileID.Bullet;
		}
		
		int timer;
		int colortimer;
		public override bool PreAI()
		{
			timer++;
			if (timer <= 50)
			{
				colortimer++;
			}
			if (timer > 50)
			{
				colortimer--;
			}
			if (timer >= 100)
			{
				timer = 0;
			}
			return true;
		}
		
		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
			Projectile.localAI[0] += 1f;
			if (Projectile.localAI[0] == 16f)
			{
				Projectile.localAI[0] = 0f;
				for (int j = 0; j < 10; j++)
				{
					Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
					vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
					vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
					int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DungeonWater, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
					Main.dust[num8].scale = 1.3f;
					Main.dust[num8].noGravity = true;
					Main.dust[num8].position = Projectile.Center + vector2;
					Main.dust[num8].velocity = Projectile.velocity * 0.1f;
					Main.dust[num8].noLight = true;
					Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
				}
			}
		
			int num1222 = 74;
			for (int k = 0; k < 2; k++)
			{
				int index2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DungeonWater, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[index2].position = Projectile.Center - Projectile.velocity / num1222 * k;
				Main.dust[index2].scale = .95f;
				Main.dust[index2].velocity *= 0f;
				Main.dust[index2].noGravity = true;
				Main.dust[index2].noLight = false;
			}
		}
		
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(150 + colortimer * 2, 150 + colortimer * 2, 150 + colortimer * 2, 100);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, 0f, -2f, 0, default(Color), .8f);
				Main.dust[num1].noGravity = true;
				Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num1].position != Projectile.Center)
					Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, 0f, -2f, 0, default(Color), .8f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num].position != Projectile.Center)
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}
		}
	}

}