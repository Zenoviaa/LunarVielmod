
using Stellamod.Trails;
using Stellamod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;

namespace Stellamod.Projectiles.Thrown
{
	public class AcidicLancherProj : ModProjectile
	{
		public bool OptionallySomeCondition { get; private set; }

		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("virulent Missile");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Bullet);
            AIType = ProjectileID.Bullet;
			Projectile.width = 10;
            Projectile.height = 10;
            Projectile.DamageType = DamageClass.Throwing;
        }
        public override void AI()
        {
            base.Projectile.spriteDirection = (base.Projectile.direction = (base.Projectile.velocity.X > 0f).ToDirectionInt());
            if (base.Projectile.spriteDirection == 1)
            {
                base.Projectile.rotation += MathHelper.ToRadians(45f);
                base.DrawOffsetX = -26;
                base.DrawOriginOffsetX = 13f;
                base.DrawOriginOffsetY = 2;
            }
            if (base.Projectile.spriteDirection == -1)
            {
                base.Projectile.rotation += MathHelper.ToRadians(45f);
                base.DrawOffsetX = -26;
                base.DrawOriginOffsetX = 13f;
                base.DrawOriginOffsetY = 2;
            }

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkSeaGreen, Color.Transparent, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SpikyTrail1);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            return false;
        }
        public override bool PreAI()
        {
            if (Main.rand.Next(6) == 1)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust1].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust1].noGravity = true;
            }
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            }
            for (int i = 0; i < 180; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].scale = 1.9f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
                }
            }
            for (int i = 0; i < 80; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), 2.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
                }
            }
        }

    }
}