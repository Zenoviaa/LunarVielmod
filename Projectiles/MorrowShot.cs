using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class MorrowShot : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 3 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            if (Timer < 100)
            {
                if (Projectile.velocity.Length() < 35)
                    Projectile.velocity *= 1.05f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Setup the shader
            MotionBlurShader shader = MotionBlurShader.Instance;
            float maxSpeed = 0.4f;
            float speed = MathHelper.Clamp(Projectile.velocity.Length() * 0.02f, 0f, maxSpeed);

            //This is gonna make it like stretch itself as it moves faster
            Vector2 scale = Vector2.Lerp(Vector2.One, new Vector2(2f, 0.18f), Easing.InOutCubic(speed));

            shader.Velocity = Vector2.UnitY * speed;

            //This just affects the opacity of the blur, prob don't need to change this number
            shader.BlurStrength = 2f;
            shader.Apply();

            //Draw the texture
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Color finalColor = Color.White.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, drawPos, frame, finalColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);

            //Draw the blurring on top
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPos, frame, finalColor * 0.5f, rotation, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for(float f = 0; f < 3; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), Velocity: Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.3f, 0.6f), newColor: Color.White, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Velocity: Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.3f, 0.6f), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }
    }
}
