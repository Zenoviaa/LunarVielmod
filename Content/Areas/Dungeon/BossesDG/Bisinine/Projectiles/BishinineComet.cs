using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BishinineCometBoom : ModProjectile
    {
        private Vector2[] _lightningPoints;
        private float _lightningPower;
        private float _lightningTime;
        private bool _drawLightning;
        private bool _calcLightningPoints;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }
        private float LightningWidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(180, 0, completionRatio);
        }

        private Color LightningColorFunction(float completionRatio)
        {
            Color lerpColor = Color.Lerp(Color.White, Color.Blue, Timer / 30f);
            lerpColor *= MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / 60f));
            return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
        }

        private void DrawLightning()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningShader lightningShader = LightningShader.Instance;
            lightningShader.Time = _lightningTime;
            lightningShader.Power = _lightningPower;
            lightningShader.InnerColor = Color.White;
            lightningShader.OuterColor = Color.GhostWhite;
            TrailDrawer.Draw(spriteBatch, _lightningPoints, LightningColorFunction, LightningWidthFunction, lightningShader);
           
            TrailDrawer.Draw(spriteBatch, _lightningPoints, LightningColorFunction, LightningWidthFunction, lightningShader);

        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _lightningPower = 0.9f;
                _lightningTime = 0;
                int[] gores = AutoGoreLoader.FindGores("GrayRock");
                foreach (int g in gores)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                }

                var p = Particle.NewBlackParticle<BlackSmokeParticle>(Projectile.Bottom, Vector2.Zero, Color.DarkGray);
      
                p.color *= 0.5f;
                p.fadeToColor = Color.Black;
                p.innerColor = Color.DarkGray;
                p.outerColor = Color.Black;

                var sear = Particle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);
                sear.innerColor = Color.White;
                sear.outerColor = Color.Blue;
                sear.fadeToColor = Color.Black;
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                ShakeModSystem.Shake = 2;

             
                for(float f = 0; f < 4f; f++)
                {
                    Vector2 pos = Projectile.Center;
                    pos += Main.rand.NextVector2Circular(80, 80);
                    var zap = Particle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                    zap.innerColor = Color.White;
                    zap.outerColor = Color.Blue;
                    zap.fadeToColor = Color.Black;
                    zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
                    zap.Rotation = Main.rand.NextFloat(0f, 3f);
                }

                SoundStyle smashSound;
                int sound = Main.rand.Next(3);
                switch (sound)
                {
                    default:
                    case 0:
                        smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
                        break;
                    case 1:
                        smashSound = AssetRegistry.Sounds.Bishinine.Comet1;
                        break;
                    case 2:
                        smashSound = AssetRegistry.Sounds.Bishinine.Comet2;
                        foreach (int g in gores)
                        {
                            Gore.NewGore(Projectile.GetSource_FromThis(),
                                Projectile.Center,
                                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                        }
                        FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                        _drawLightning = true; 
                        var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
                           innerColor: Color.White,
                           glowColor: Color.LightBlue,
                           outerGlowColor: Color.DarkBlue, duration: 15, baseSize: .09f);
                        p3.Scale *= 4;
                        break;
                }
   
                smashSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(smashSound, Projectile.position);


                var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.fadeToColor = Color.Black;
                part.outerColor = Color.GhostWhite;
                part.noStretch = true;
                part.shrink = true;

                var part2 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part2.fadeToColor = Color.Black;
                part2.outerColor = Color.GhostWhite;
                part2.noStretch = true;
                part2.color *= 0.5f;
                for (float f = 0; f < 5; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    vel.Y -= 10;
                    var d = Dust.NewDustPerfect(Projectile.Center,
                        ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0f, 2f) , Velocity: vel);

                }
                var soundStyle = AssetRegistry.Sounds.Stars.Starsingle5;
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.LightBlue,
                   outerGlowColor: Color.DarkBlue, duration: 15, baseSize: boomSize * 2);
            }


            if (Timer == 15)
            {
                _lightningPower = 5;
            }

            if (Timer == 15)
            {
                _lightningPower = 30;
            }
            if (_drawLightning)
            {

                if (Timer > 35)
                {
                    _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);



                }

                if (Timer == 42)
                {
                    _lightningPower = 1.5f;
                }
                if (Timer == 42)
                {
                    var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                      innerColor: Color.White,
                                      glowColor: Color.GhostWhite,
                                      outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
                }
                if (Timer == 52)
                {
                    _lightningPower = 2.3f;
                }
                if (Timer == 52)
                {
                    var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                      innerColor: Color.White,
                                      glowColor: Color.GhostWhite,
                                      outerGlowColor: Color.Blue, duration: 6, baseSize: 0.07f);
                }


                if (Timer == 58)
                {
                    SoundStyle zap = SoundID.DD2_LightningBugZap;
                    zap.PitchVariance = 0.3f;
                    SoundEngine.PlaySound(zap, Projectile.position);

                    for (float f = 0; f < 2; f++)
                    {
                        Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                        pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                        var spark = Particle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                        spark.Scale *= 0.5f;
                        spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
                        spark.outerColor = Color.Blue;
                    }
                }
            }
            _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);


            _lightningTime -= 0.01f;
            if (!_calcLightningPoints)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                Vector2 direction = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                float numPoints = 80;
                float randOffset = Main.rand.NextFloat(-1f, 1f);
                Vector2 start = Projectile.Center;
                Vector2 end = Projectile.Center + direction * Main.rand.NextFloat(600, 984);
                end.X += Main.rand.Next(-16, -16);
                for (float i = 0; i <= numPoints; i++)
                {


                    float interp = i / numPoints;
                    Vector2 point = Vector2.Lerp(start, end, interp);
                    point.X += EasingFunction.QuadraticBump(interp) * 64 * randOffset;
                    //if(i % 4 == 0)
                    //point.X += Main.rand.Next(-16, 16);
                    beamPoints.Add(point);
                }

                _lightningPoints = beamPoints.ToArray();
                _calcLightningPoints = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (_drawLightning && _lightningPoints != null)
            {
                DrawLightning();
            }
            return base.PreDraw(ref lightColor);
        }
    }
    public class BishinineComet : ModProjectile
    {
        private float _flameTimer;
        private ref float Timer => ref Projectile.ai[0];
        private ref float RotateRadians => ref Projectile.ai[1];
        private ref float Size => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 80;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override void AI()
        {

            base.AI();
            _flameTimer += 0.3f;
            Timer++;
            if (Timer == 1 && this.OwnedByLocalClient())
            {
                RotateRadians = Main.rand.NextFloat(-0.1f, 0);
                Size = Main.rand.NextFloat(1f, 2f);
                Projectile.netUpdate = true;
            }
            if (Timer % 10 == 0)
            {
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0f, 0.5f) * Size);
                d.velocity *= 0;
            }
            if (Timer >= 300 * Size)
            {
                Projectile.tileCollide = true;
            }
            if (Main.rand.NextBool(6))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.GemDiamond, newColor: Color.White, Scale: Main.rand.NextFloat(0f, 0.5f) * Size);

            }
            Size *= 0.9985f;
            Projectile.velocity.X += RotateRadians * 0.1f;
            Projectile.velocity.Y *= 1.002f;

            Projectile.scale = MathHelper.Lerp(0.2f, 0.3f, EasingFunction.InOutSine(Timer / 60f));
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(24, 0, completionRatio) * Size;
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
            shader.Time = _flameTimer;
            TrailDrawer.Draw(spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);


            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D starTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 drawOrigin = starTexture.Size() / 2f;
            Color cometColor = Color.GhostWhite;
            cometColor.A = 0;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Blue, interpolant) * 0.05f;
                fadeColor *= (1.0f - interpolant);
                fadeColor.A = 0;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], drawOrigin, Projectile.scale * Size * 2.2f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(starTexture, drawPosition, null, cometColor * 0.4f, Projectile.rotation, drawOrigin, Projectile.scale * Size * 2, SpriteEffects.None, 0);
            spriteBatch.Draw(starTexture, drawPosition, null, cometColor, Projectile.rotation, drawOrigin, Projectile.scale * Size, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CustomDraw();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.oldVelocity,
                ModContent.ProjectileType<BishinineCometBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
