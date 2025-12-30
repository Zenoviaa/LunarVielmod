using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BouncingScythe : ModProjectile,
        IDrawOutlines
    {
        private float _alpha = 1f;
        private ref float Timer => ref Projectile.ai[0];
        private ref float AttackNumber => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 700;
        }
        public override void AI()
        {
            base.AI();
            _alpha = ExtraMath.Saturate((float)Projectile.timeLeft / 60f);
   
            Timer++;
            Projectile.rotation += 0.05f;
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: 0.5f);
            }
            Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 40000);
            if (target != null)
            {
                float targetX = target.Center.X < Projectile.Center.X ? -17 : 17;
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetX, 0.01f);
            }

            if(Timer >= 20)
            {
                if (Projectile.velocity.Y < 12)
                    Projectile.velocity.Y += 1;
            }
         
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Projectile.velocity.Y != oldVelocity.Y)
            {
                Timer = 0;
                AttackNumber++;
                Projectile.velocity.Y = -oldVelocity.Y;
                var part = FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.GhostWhite,
                    outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
                int[] gores = AutoGoreLoader.FindGores("SilverBell");
                for (int i = 0; i < 2; i++)
                {
                    foreach (int g in gores)
                    {
                        Gore.NewGore(Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                    }
                }

                var p3 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);
                FXUtil.ShakeCamera(Projectile.position, 1024, 24);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);

                SoundStyle bellHitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Bishinine.BellHit1 : AssetRegistry.Sounds.Bishinine.BellHit2;
                bellHitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bellHitSound, Projectile.position);
            }
            return false;
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Gray, Color.Transparent, completionRatio) * _alpha;
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(64, 0, completionRatio);
        }
        private void CustomDraw()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.WhispyTrail;
            shader.PrimaryTexture2 = TrailRegistry.StarTrail;
            shader.InnerColor = Color.Aqua;
            shader.OuterColor = Color.LightBlue;
            shader.Distortion = MathHelper.Lerp(0.6f, 0.2f, EasingFunction.InOutSine(Timer / 30f)) * MathHelper.Lerp(1, 0, EasingFunction.InOutExpo(Timer / 90f));
            TrailDrawer.Draw(spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CustomDraw();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            Vector2 drawScale = Vector2.One * Projectile.scale;
            SpriteEffects spriteEffects = SpriteEffects.None;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Gray, Color.Transparent, interpolant) * 0.15f;
                fadeColor *= _alpha;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }

            Texture2D starTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 sdrawOrigin = starTexture.Size() / 2f;
            Color cometColor = Color.Gray * _alpha;
            cometColor.A = 0;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Gray, Color.Blue, interpolant) * 0.1f;
                fadeColor *= (1.0f - interpolant);
                fadeColor.A = 0;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], sdrawOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor) * _alpha, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = SpriteEffects.None;
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            Color outlineColor = Color.Red * _alpha;
            Rectangle? drawFrame = null;
            Vector2 drawOrigin = texture.Size() / 2;
            Vector2 scale = Vector2.One * Projectile.scale;
            float rotation = Projectile.rotation;

            spriteBatch.Draw(texture, drawPos + left, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
        }
    }
}
