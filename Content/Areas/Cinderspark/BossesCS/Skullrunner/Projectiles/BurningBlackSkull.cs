
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner.Projectiles
{
    public class BurningBlackSkull : ScarletProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private bool ChosenFrame;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 2;
            Projectile.hostile = true;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                Vector2 velocity = Projectile.velocity;
                Vector2 position = Projectile.Center;
                for (float f = 0; f < 16; f++)
                {
                    Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.Red,
                        outerColor: Color.Orange,
                        fadeToColor: Color.Purple,
                        distortOut: true);

                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                         velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                         velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                    }
                    if (Main.rand.NextBool(4))
                    {

                        var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                         innerColor: Color.DarkRed,
                         outerColor: Color.DarkBlue,
                         fadeToColor: Color.Black,
                         distortOut: false);
                        part.Scale *= 1.3f;
                    }
                }
            }
            if (!ChosenFrame)
            {
                Projectile.frame = Main.rand.Next(3);
                ChosenFrame = true;
            }
            if (Timer > 200)
            {
                Projectile.tileCollide = true;
            }

            if (Timer % 8 == 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Red, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity * 0.1f, 0, Color.Red, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
            if (target != null)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, target.Center, 4);

                //very slight lerp to this thing
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, homingVelocity, 0.1f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.OrangeRed;
            glowColor.A = 0;


            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
            }


            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            TrailDrawer.Draw(spriteBatch, OldCenterPos, OldCenterRot, ColorFunction, WidthFunction, flamingTrailShader, Vector2.Zero);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.Red, completionRatio) * 0.7f;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 32; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
            }

            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp"), Projectile.position);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red,
                duration: Main.rand.Next(10, 25),
                baseSize: Main.rand.NextFloat(0.05f, 0.16f));
        }
    }
}
