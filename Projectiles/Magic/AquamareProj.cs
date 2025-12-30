using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class AquamareProj : ModProjectile
    {
        float distance = 8;
        int rotationalSpeed = 4;
        bool initialized = false;
        Vector2 initialSpeed = Vector2.Zero;
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.width = Projectile.height = 50;
            Projectile.hostile = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Timer++;
            if (Timer % 8 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Aquamarine, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            Projectile.velocity *= 0.991f;
            int rightValue = (int)Projectile.ai[1] - 1;
            if (rightValue < (double)Main.projectile.Length && rightValue != -1)
            {
                Projectile other = Main.projectile[rightValue];
                Vector2 direction9 = other.Center - Projectile.Center;
                int distance = (int)Math.Sqrt((direction9.X * direction9.X) + (direction9.Y * direction9.Y));
                direction9.Normalize();
            }
            if (!initialized)
            {
                initialSpeed = Projectile.velocity;
                initialized = true;
            }
            if (initialSpeed.Length() < 15)
                initialSpeed *= 1.01f;
            Projectile.spriteDirection = 1;
            if (Projectile.ai[0] > 0)
            {
                Projectile.spriteDirection = 0;
            }

            distance += 0.4f;
            Projectile.ai[0] += rotationalSpeed;

            Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
            offset.Normalize();
            offset *= (float)(Math.Cos(Projectile.ai[0] * (Math.PI / 180)) * (distance / 3));
            Projectile.velocity = initialSpeed + offset;
            Projectile.rotation -= 0.5f;
            Projectile.ai[0]++;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = 1 * (Projectile.width / 4) * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Aquamarine, Color.Transparent, completionRatio) * 0.7f;
        }
        private void DrawEnergyBall(ref Color lightColor)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = 0.12f;


            //Colors
            Color startInner = Color.White;
            Color startGlow = Color.Lerp(Color.LightBlue, Color.CadetBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Blue, Color.Aquamarine, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            DrawEnergyBall(ref lightColor);

            return false;
        }
    }
}