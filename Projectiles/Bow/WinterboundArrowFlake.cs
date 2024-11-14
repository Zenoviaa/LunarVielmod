using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class WinterboundArrowFlake : ModProjectile
    {
        private float _drawScale;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 11;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                Projectile.rotation = Main.rand.NextFloat(0f, 1f);
            }

            if(Timer < 60)
            {
                _drawScale = MathHelper.Lerp(_drawScale, 1f, 0.1f);
            } else if (Timer > 90)
            {
                _drawScale = MathHelper.Lerp(_drawScale, 0f, 0.1f);
            }
            Projectile.velocity *= 0.92f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Projectile.rotation += 0.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D snowflakeTexture = ModContent.Request<Texture2D>("Stellamod/Particles/SnowFlakeParticleSmall").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = snowflakeTexture.Size() / 2;
            float drawRotation = Projectile.rotation;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.White;
            drawColor.A = 0;
            float drawScale = 1f;
            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 glowOffset = rot.ToRotationVector2() * 12 * VectorHelper.Osc(0.75f, 1f, speed: 3);
                Vector2 glowDrawPos = drawPos + glowOffset;

                drawColor = Color.LightSkyBlue;
                drawColor.A = 0;
                spriteBatch.Draw(snowflakeTexture, glowDrawPos, null, drawColor * 0.3f, drawRotation, drawOrigin, _drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(snowflakeTexture, drawPos, null, drawColor, drawRotation, drawOrigin, _drawScale, SpriteEffects.None, 0);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            float drawScale = 1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
            {
                Color glowColor = Color.LightBlue * 0.5f;
                glowColor.A = 0;
                spriteBatch.Draw(dimLightTexture, Projectile.Center - Main.screenPosition, null, glowColor,
                    Projectile.rotation, dimLightTexture.Size() / 2f, _drawScale * VectorHelper.Osc(0.75f, 1f, speed: 32, offset: Projectile.whoAmI), SpriteEffects.None, 0f);
            }
        }
    }
}
