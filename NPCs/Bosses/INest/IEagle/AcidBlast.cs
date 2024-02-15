
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.INest.IEagle
{
    public class AcidBlast : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Blast");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 70;
			Projectile.height = 70;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = 10;
            Projectile.alpha = 255;
            Projectile.timeLeft = 1000;
			Projectile.tileCollide = false;
            Projectile.aiStyle = 1;
			Projectile.damage = 60;
			AIType = ProjectileID.Bullet;
		}
		int timer;
		int colortimer;
		public override bool PreAI()
        {
            Projectile.velocity /= 0.98f;
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





            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, Color.SpringGreen with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
            }
            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, drawColor with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, 0, 0);
			}
			Main.EntitySpriteDraw(texture, drawPosition, null, drawColor with { A = 130 }, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);


			return false;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(150 + colortimer * 2, 150 + colortimer * 2, 150 + colortimer * 2, 100);
		}
	}
}