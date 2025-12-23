using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class BlackTornadoWind
    {
        private readonly TexturedQuad _texturedQuad;
        public BlackTornadoWind()
        {
            _texturedQuad = new TexturedQuad();
        }

        public float alpha;
        public void Draw(Vector2 drawCenter, float length, float width)
        {
            if (alpha <= 0)
                return;

            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.Black;
            flamingTrailShader.InnerColor = Color.White * 0.1f;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = new Vector2(1, 3);
            flamingTrailShader.BlendState = BlendState.AlphaBlend;
            flamingTrailShader.Time = Main.GlobalTimeWrappedHourly * 64;


            _texturedQuad.CalculateCenterVertices(drawCenter,
                length, width);
            _texturedQuad.SetColor(Color.White * alpha);
            _texturedQuad.DrawWithShader(flamingTrailShader);
        }
    }
    public class BlackTornadoStar2 : BlackTornadoStar
    {

    }
    public class BlackTornadoStar3 : BlackTornadoStar
    {

    }

    public class BlackTornadoStar : ModProjectile,
         IDrawBlackStar,
         IDrawOutlines
    {
        private float _telegraphLineRot;
        private float _telegraphLineAlpha;
        private ref float Timer => ref Projectile.ai[0];
        private enum AIState
        {
            Fly,
            ShootDown
        }
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float ShouldFall => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 800;
            Projectile.hostile = true;
        }
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Fly:
                    AI_Fly();
                    break;
                case AIState.ShootDown:
                    AI_ShootDown();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        private void AI_ShootDown()
        {
            Timer++;
            if (Timer % 8 == 0)
            {
                var donut = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero), Color.White);
                donut.Scale *= 0.3f;
            }

            Projectile.velocity.Y += 0.5f;
            Projectile.velocity.Y *= 1.01f;
            Projectile.rotation += 0.01f * MathF.Sign(Projectile.velocity.X);
            Projectile.rotation += Projectile.velocity.Length() * 0.0025f;

        }

        private void AI_Fly()
        {
            Timer++;
            if (Timer == 1)
            {
                ShouldFall = 1;

            }



            if (Timer % 10 == 0)
            {
                var p = Particle.NewParticle<StarParticle>(Projectile.Center, Vector2.Zero, Color.White, Scale: 0.4f);
                p.fast = true;
                var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.Dirt, Vector2.Zero,
                    newColor: Color.White,
                    Scale: 1);
                d.noGravity = true;
            }

            float outScale = (float)Projectile.timeLeft / 15f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = 1f * outScale;
            Projectile.rotation += 0.01f * MathF.Sign(Projectile.velocity.X);
            Projectile.rotation += Projectile.velocity.Length() * 0.0025f;

            if (ShouldFall == 1)
            {
                Player closestPlayer = PlayerHelper.FindClosestPlayer(Projectile.position, 8000);
                if (closestPlayer != null)
                {
                    //Once above the player, we're going to shoot down really fast and explode lol.
                    Vector2 directionToPlayer = (closestPlayer.Center - Projectile.Center);
                    directionToPlayer = directionToPlayer.SafeNormalize(Vector2.Zero);
                    Vector2 normalVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    float dp = Vector2.Dot(directionToPlayer, Vector2.UnitY);
                    if (dp > 0.75f)
                    {
                        SwitchState(AIState.ShootDown);
                    }
                }
            }

            if (this.OwnedByLocalClient() && Main.rand.NextBool(60))
            {
                SwitchState(AIState.ShootDown);
            }
            Projectile.velocity.Y += 0.005f;
            Projectile.velocity.Y *= 1.005f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawAfterImages(Main.spriteBatch);
            DrawHelper.DrawBloomLine(Main.spriteBatch, Projectile.Center, Color.White, _telegraphLineRot, _telegraphLineAlpha * 0.2f);
            return false;
        }

        private void DrawAfterImages(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = (float)i / (float)Projectile.oldPos.Length;

                Vector2 drawCenter = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float rotation = Projectile.oldRot[i];
                float scale = Projectile.scale;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= 0.15f;
                Vector2 drawScale = Vector2.One;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawPosition, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            // DrawSprite(spriteBatch, Projectile.Center + v - screenPos, Color.White);
            // DrawSprite(spriteBatch, Projectile.Center - v - screenPos, Color.White);
            //  DrawSprite(spriteBatch, Projectile.Center + h - screenPos, Color.White);
            // DrawSprite(spriteBatch, Projectile.Center - h - screenPos, Color.White);
        }
        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawSprite(spriteBatch, Projectile.Center - Main.screenPosition, Color.White);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
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
            sear.innerColor = Color.Gray;
            sear.outerColor = Color.Blue;
            sear.fadeToColor = Color.Black;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeModSystem.Shake = 2;


            for (float f = 0; f < 4f; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(80, 80);
                var zap = Particle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                zap.innerColor = Color.Gray;
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
                    var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
                       innerColor: Color.Gray,
                       glowColor: Color.LightBlue,
                       outerGlowColor: Color.DarkBlue, duration: 15, baseSize: .09f);
                    p3.Scale *= 4;
                    break;
            }


            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);


            var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part.fadeToColor = Color.Black;
            part.outerColor = Color.Gray;
            part.noStretch = true;
            part.shrink = true;

            var part2 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part2.fadeToColor = Color.Black;
            part2.outerColor = Color.Gray;
            part2.noStretch = true;
            part2.color *= 0.5f;
            for (float f = 0; f < 5; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                vel.Y -= 10;
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);

            }
            var soundStyle = AssetRegistry.Sounds.Stars.Starsingle5;
            soundStyle.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Gray,
               glowColor: Color.LightBlue,
               outerGlowColor: Color.DarkBlue, duration: 15, baseSize: boomSize * 2);
        }
    }

    public class BlackTornadoDebrisLarge : BlackTornadoDebris
    {

    }

    public class BlackTornadoDebrisMedium : BlackTornadoDebris
    {

    }
    public class BlackTornadoDebrisMedium2 : BlackTornadoDebris
    {

    }
    public class BlackTornadoDebrisMedium3 : BlackTornadoDebris
    {

    }
    public class BlackTornadoDebris : ModProjectile,
        IDrawBlackStar,
        IDrawOutlines
    {
        private float _telegraphLineRot;
        private float _telegraphLineAlpha;
        private ref float Timer => ref Projectile.ai[0];
        private enum AIState
        {
            Fly,
            ShootDown
        }
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float ShouldFall => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 800;
            Projectile.hostile = true;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Fly:
                    AI_Fly();
                    break;
                case AIState.ShootDown:
                    AI_ShootDown();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        private void AI_ShootDown()
        {
            Timer++;
            if (Timer % 8 == 0)
            {
                var donut = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero), Color.White);
                donut.Scale *= 0.3f;
            }

            Projectile.velocity.Y += 0.5f;
            Projectile.velocity.Y *= 1.01f;
            Projectile.rotation -= 0.01f * MathF.Sign(Projectile.velocity.X);
            Projectile.rotation -= Projectile.velocity.Length() * 0.0025f;

        }

        private void AI_Fly()
        {
            Timer++;
            if (Timer == 1)
            {
                ShouldFall = 1;

            }



            if (Timer % 10 == 0)
            {
                var p = Particle.NewParticle<StarParticle>(Projectile.Center, Vector2.Zero, Color.White, Scale: 0.4f);
                p.fast = true;
                var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.Dirt, Vector2.Zero,
                    newColor: Color.White,
                    Scale: 1);
                d.noGravity = true;
            }

            float outScale = (float)Projectile.timeLeft / 15f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = 1f * outScale;
            Projectile.rotation += 0.01f * MathF.Sign(Projectile.velocity.X);
            Projectile.rotation += Projectile.velocity.Length() * 0.0025f;

            Projectile.velocity.Y += 0.05f;
            Projectile.velocity.Y *= 1.0005f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawAfterImages(Main.spriteBatch);
            DrawHelper.DrawBloomLine(Main.spriteBatch, Projectile.Center, Color.White, _telegraphLineRot, _telegraphLineAlpha * 0.2f);
            return false;
        }

        private void DrawAfterImages(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = (float)i / (float)Projectile.oldPos.Length;

                Vector2 drawCenter = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float rotation = Projectile.oldRot[i];
                float scale = Projectile.scale;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= 0.15f;
                Vector2 drawScale = Vector2.One;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawPosition, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
           // DrawSprite(spriteBatch, Projectile.Center + v - screenPos, Color.White);
           // DrawSprite(spriteBatch, Projectile.Center - v - screenPos, Color.White);
          //  DrawSprite(spriteBatch, Projectile.Center + h - screenPos, Color.White);
           // DrawSprite(spriteBatch, Projectile.Center - h - screenPos, Color.White);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
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
            sear.innerColor = Color.Gray;
            sear.outerColor = Color.Blue;
            sear.fadeToColor = Color.Black;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeModSystem.Shake = 2;


            for (float f = 0; f < 4f; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(80, 80);
                var zap = Particle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                zap.innerColor = Color.Gray;
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
                    var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
                       innerColor: Color.Gray,
                       glowColor: Color.LightBlue,
                       outerGlowColor: Color.DarkBlue, duration: 15, baseSize: .09f);
                    p3.Scale *= 4;
                    break;
            }


            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);


            var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part.fadeToColor = Color.Black;
            part.outerColor = Color.Gray;
            part.noStretch = true;
            part.shrink = true;

            var part2 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part2.fadeToColor = Color.Black;
            part2.outerColor = Color.Gray;
            part2.noStretch = true;
            part2.color *= 0.5f;
            for (float f = 0; f < 5; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                vel.Y -= 10;
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);

            }
            var soundStyle = AssetRegistry.Sounds.Stars.Starsingle5;
            soundStyle.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Gray,
               glowColor: Color.LightBlue,
               outerGlowColor: Color.DarkBlue, duration: 15, baseSize: boomSize * 2);
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawSprite(spriteBatch, Projectile.Center - Main.screenPosition, Color.White);
        }
    }


    public class BlackTornado : ModProjectile,
        IDrawPixelated
    {
        private LittleStarParticleManager _tornadoStreakParticlesBackingField;
        private LittleStarParticleManager TornadoStreakParticles
        {
            get
            {
                _tornadoStreakParticlesBackingField ??= new LittleStarParticleManager(300, 8, GetTrailWidth);
                return _tornadoStreakParticlesBackingField;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];

        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 180;
            Projectile.height = 600;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (Timer > 100)
                Projectile.hostile = true;
            //For this projectile what we're going to need to do is create a tornado visual with projectiles coming outward and then coming in
            //Gustbeak has a tornado but it's meh
            //Gintzia's winds look a bit better and should look fine when combined with swirling particles
            //So make a new particle manager for this
            Timer++;
            if(Timer % 12 == 0)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
                jiitasSit.PitchVariance = 0.2f;
                jiitasSit.Pitch = 0f;
                jiitasSit.Volume = 0.25f;
                SoundEngine.PlaySound(jiitasSit, Projectile.position);
            }
            if (Timer % 6 == 0)
            {
          
                if (this.OwnedByLocalClient())
                {
                    int projType;
                    float direction = Main.rand.NextBool(2) ? -1 : 1;
                    float xOffset = direction * 2000;
                    Vector2 spawnOffset = new Vector2(xOffset, Main.rand.NextFloat(-400f, -300f));
                    Vector2 spawnPos = Projectile.Center + spawnOffset;
                    Vector2 velocity = Vector2.UnitX * -direction * Main.rand.NextFloat(8, 17);
                    switch (Main.rand.Next(4))
                    {
                        default:
                        case 0:
                            projType = ModContent.ProjectileType<BlackTornadoDebris>();
                            velocity *= 0.6f;
                            break;
                        case 1:
                            velocity *= 0.6f;
                            projType = ModContent.ProjectileType<BlackTornadoDebrisMedium>();
                            switch (Main.rand.Next(3))
                            {
                                case 0:
                                    projType = ModContent.ProjectileType<BlackTornadoDebrisMedium2>();
                                    break;
                                case 1:
                                    projType = ModContent.ProjectileType<BlackTornadoDebrisMedium3>();
                                    break;
                            }
                            break;
                        case 2:
                            velocity *= 0.6f;
                            projType = ModContent.ProjectileType<BlackTornadoDebrisLarge>();
                            break;
                        case 3:
                            switch (Main.rand.Next(3))
                            {
                                default:
                                case 0:
                                    projType = ModContent.ProjectileType<BlackTornadoStar>();
                                    break;
                                case 1:
                                    projType = ModContent.ProjectileType<BlackTornadoStar2>();
                                    break;
                                case 2:
                                    projType = ModContent.ProjectileType<BlackTornadoStar3>();
                                    break;
                            }
                            break;
                    }
      
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, velocity,
                        projType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            float inTornado = Timer / 30f;
            float outTornado = (float)Projectile.timeLeft / 30f;

            inTornado = EasingFunction.InOutSine(inTornado);
            outTornado = EasingFunction.InOutSine(outTornado);
            float alpha = inTornado * outTornado;
            TornadoStreakParticles.xOvalRadius = 5;
            TornadoStreakParticles.yOvalRadius = MathHelper.Lerp(150, 750, EasingFunction.InOutSine(Timer / 150f));
            TornadoStreakParticles.minX = ExtraMath.Osc(25, 45, speed: 3) + MathHelper.Lerp(0f, 25f, EasingFunction.InOutSine(Timer / 150f));
            TornadoStreakParticles.spinTime = 25;
            TornadoStreakParticles.rotationAxis = new Vector3(0, 1, 0.2f);
            TornadoStreakParticles.alpha = 0.65f * alpha;
            TornadoStreakParticles.topOnly = true;
            TornadoStreakParticles.Update(Projectile.Center);
           
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0.2f, 2, EasingFunction.QuadraticBump(completionRatio));
        }
        public override bool PreDraw(ref Color lightColor)
        {
           // TornadoStreakParticles.Draw();
            //    TornadoStreakParticles.Draw();
            return false;
        }

        public void DrawPixelated()
        {
           TornadoStreakParticles.Draw();
        }
    }
    public class TornadoSuckPlayer : ModPlayer
    {
        public Vector2? TornadoCenter;
        public float TornadoPullStrength;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (TornadoCenter.HasValue)
            {

                Vector2 tornadoCenter = TornadoCenter.Value;
                Vector2 tornadoPullDirection = tornadoCenter - Player.Center;
                tornadoPullDirection = tornadoPullDirection.SafeNormalize(Vector2.Zero);

                Player.velocity += tornadoPullDirection * TornadoPullStrength;
                TornadoCenter = null;
            }
        }
    }
    public partial class E
    {
        private int TornadoDamage => 45;

        /// <summary>
        /// Sucks in all players to him
        /// </summary>
        /// <param name="strength"></param>
        private void SuckAllPlayers(float strength)
        {
            foreach (var player in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(NPC.Center, player.Center);
                TornadoSuckPlayer tornadoSuckPlayer = player.GetModPlayer<TornadoSuckPlayer>();
                tornadoSuckPlayer.TornadoCenter = NPC.Center;
                tornadoSuckPlayer.TornadoPullStrength = distance / 2560f * strength;
            }
        }

        private void AI_TornadoStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            float startupTime = 60;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = MyTarget.Center - new Vector2(0, 32);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            Animator.PlayAnimation(Anim_BattleIdle);
            if (Timer >= startupTime)
            {
                SwitchState(AIState.Tornado_PreSpin);
            }
        }

        private void AI_TornadoPreSpin()
        {
            //In this state, he'll slowly start speeding up and then creating the tornado, 
            //The earlier startup state is just to get him into the position
            //This is mostly done with a sound and animation, so not much happens here
            Timer++;
            if(Timer == 1)
            {
                TargetVector = NPC.Center;
            }


            float prespinTime = 60f;
            float completionRatio = Timer / prespinTime;
            float ease = EasingFunction.InOutSine(completionRatio);

            Animator.PlayAnimation(Anim_Running);

            //Speed up
            float xOffset = MathF.Sin(Timer * -0.15f) * 64;
            float yOffset = MathF.Cos(Timer * 0.15f) * 32f;
            Vector2 targetOffset = new Vector2(xOffset, yOffset);
            Vector2 positionToMoveTo = TargetVector + targetOffset;
            Vector2 tornadoVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = tornadoVelocity;
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;

            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.5f, ease);
            ShakeModSystem.Shake = MathHelper.Lerp(0f, 2f, ease);
            if (Timer >= prespinTime)
            {
                SwitchState(AIState.Tornado_Spin);
            }
        }

        private void AI_TornadoSpin()
        {
            //Here the tornado projectile will actually spawn and we'll begin sucking in all of the players
            //At the same time we'll slowly move towards our target
            Timer++;
            if (Timer == 1)
            {
                SoundStyle hurricaneBlack = AssetRegistry.Sounds.E.HurricaneBlack;
                hurricaneBlack.Volume = 1.5f;
                SoundEngine.PlaySound(hurricaneBlack);
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, TargetVector, Vector2.Zero,
                        ModContent.ProjectileType<BlackTornado>(), TornadoDamage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                }
            }

            float tornadoTime = 600;
            _extraAfterImageAlpha = 0.5f;
            float xOffset = MathF.Sin(Timer * -0.5f) * 164;
            float yOffset = MathF.Cos(Timer * 0.5f) * 32f;
            Vector2 targetOffset = new Vector2(xOffset, yOffset);
            Vector2 positionToMoveTo = TargetVector + targetOffset;
            Vector2 tornadoVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = tornadoVelocity;
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
            Animator.PlayAnimation(Anim_Running);
            ShakeModSystem.Shake = 4;

            //Unsure how strong this should actually be so make sure to balance this number properly
            float tornadoStrength = 1f;
            SuckAllPlayers(tornadoStrength);
            if (Timer >= tornadoTime)
            {
                SwitchState(AIState.Tornado_End);
            }
        }

        private void AI_TornadoEnd()
        {
            Timer++;
            float endTime = 15f;
            float completionRatio = Timer / endTime;
            float ease = EasingFunction.InOutSine(completionRatio);
  
            NPC.velocity *= 0.9f;
            _extraAfterImageAlpha = MathHelper.Lerp(0.5f, 0f, ease);
            if (Timer >= endTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
