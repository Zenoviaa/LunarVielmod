using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class LanternOfTheFallenFly : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
            Projectile.timeLeft = 320;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 6 == 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.position, DustID.GreenTorch, Vector2.Zero, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.position, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.Turquoise, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            if (Timer < 45)
            {
                Projectile.velocity *= 0.98f;
                Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);
            }

            if (Timer == 46)
            {
                Projectile.velocity += Vector2.UnitY;
            }
            if (Timer > 47)
            {
                if (Projectile.velocity.Length() < 1)
                    Projectile.velocity *= 1.02f;
                NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
                if (nearest != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 3);
                }

                Projectile.extraUpdates = (int)MathHelper.Lerp(0, 4, (Timer - 47) / 160f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 1; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Green, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black,
                    duration: Main.rand.NextFloat(6, 12),
                    baseSize: Main.rand.NextFloat(0.01f, 0.05f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos.Y += VectorHelper.Osc(-4f, 4f, speed: 1f, Projectile.whoAmI);
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = VectorHelper.Osc(0.75f, 1f, offset: Projectile.whoAmI);

            float ep = VectorHelper.Osc(0f, 1f);
            drawScale += MathHelper.Lerp(0.25f, 0.5f, ep);
            SpriteEffects dir = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, dir, 0);
            DrawGlow(ref lightColor);
            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, dir, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        private void DrawGlow(ref Color lightColor)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            Color startInner = Color.LightGreen;
            Color startGlow = Color.Lerp(Color.Teal, Color.Green, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2f;

            //Radius of the circle
            shader.Size = VectorHelper.Osc(0.09f, 0.12f, offset: Projectile.whoAmI);

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = VectorHelper.Osc(2.5f, 3.5f, offset: Projectile.whoAmI);
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 1; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }
        public PrimDrawer TrailDrawer2 { get; private set; } = null;
        public float WidthFunction2(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            if (Timer % 6 == 0)
            {
                baseWidth *= 1.2f;
            }
            return MathHelper.SmoothStep(baseWidth, 0f, completionRatio) * 0.8f;
        }

        public Color ColorFunction2(float completionRatio)
        {
            Color color = Color.Teal;
            if (Timer % 6 == 0)
            {
                color = Color.LightGoldenrodYellow;
            }
            return Color.Lerp(color, color, completionRatio);
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            TrailDrawer2 ??= new PrimDrawer(WidthFunction2, ColorFunction2, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer2.DrawPixelPrims(Projectile.oldPos, -Main.screenPosition, 128);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
