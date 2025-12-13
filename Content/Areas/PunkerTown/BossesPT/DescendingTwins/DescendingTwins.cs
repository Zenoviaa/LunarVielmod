using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public class DescendingNodeTriggeringBeam : ScarletProjectile,
        IDrawPixelated
    {
        private ref float Timer => ref Projectile.ai[0];
        private int TargetNPCIndex => (int)Projectile.ai[1];
        private NPC Target => Main.npc[TargetNPCIndex];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle shootSound = AssetRegistry.Sounds.SteamPunking.DescendingRetinaBeam;
                shootSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shootSound, Projectile.position);
            }

            float degreeToRotate = 15f;
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Target.Center, degreeToRotate);
        }

        public void DrawPixelated()
        {
            float outScale = (float)Projectile.timeLeft / 10f;
            float outScaleEase = EasingFunction.InOutSine(outScale);

            Texture2D drawTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_56").Value;
            Vector2 drawOrigin = drawTexture.Size() / 2f;
            float numAfterImages = TrailCacheLength;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 centerPos = OldCenterPos[i] - Main.screenPosition;
                float f = i;
                float completionRatio = f / numAfterImages;

                Color drawColor = Color.Lerp(Color.White, Color.Red, completionRatio);
                drawColor.A = 0;
                drawColor *= MathHelper.Lerp(1f, 0f, completionRatio);

                float scale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                scale *= outScaleEase;
                scale *= 0.25f;
                spriteBatch.Draw(drawTexture, centerPos, null, drawColor, OldCenterRot[i], drawOrigin, scale, SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkRed, Color.Black);
        }
    }


    public class DescendingNodeBeam : ScarletProjectile,
        IDrawPixelated
    {

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle shootSound = AssetRegistry.Sounds.SteamPunking.DescendingRetinaBeam;
                shootSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shootSound, Projectile.position);
                SpawnFlameDonut();
            }
            if(Timer % 5 == 0)
            {
                var p = Particle.NewParticle<GlowFragmentParticle>(Projectile.Center, Vector2.Zero, Color.White, Scale: 4f);
                Color twinColor = Color.Green;
                p.innerColor = twinColor;
                p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
                p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
            }
        }
        private void SpawnFlameDonut()
        {
            //movement donut particles
            var donut = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.White);
            Color twinColor = Color.Green;
            donut.innerColor = twinColor;
            donut.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            donut.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }

        public void DrawPixelated()
        {
            float outScale = (float)Projectile.timeLeft / 10f;
            float outScaleEase = EasingFunction.InOutSine(outScale);

            Texture2D drawTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Backglow").Value;
            Vector2 drawOrigin = drawTexture.Size() / 2f;
            float numAfterImages = TrailCacheLength;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 centerPos = OldCenterPos[i] - Main.screenPosition;
                float f = i;
                float completionRatio = f / numAfterImages;

                Color drawColor = Color.Lerp(Color.White, Color.Green, completionRatio);
                drawColor.A = 0;
                drawColor *= MathHelper.Lerp(1f, 0f, completionRatio);

                float scale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                scale *= outScaleEase;
                scale *= 0.5f;
                spriteBatch.Draw(drawTexture, centerPos, null, drawColor, OldCenterRot[i], drawOrigin, scale, SpriteEffects.None, 0f);
            }
        }

    }

    public class DescendingNode : ModNPC,
        IDrawOutlines
    {
        private enum AIState
        {
            Idle,
            Death
        }
        private ref float Timer => ref NPC.ai[0];
        private ref float StartRotation => ref NPC.ai[1];
        private AIState State
        {
            get => (AIState)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }

        private ref float ShotAt => ref NPC.ai[3];
        private int BeamDamage => 25;
        private Color _outlineColor;
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 96;
            NPC.height = 96;
            NPC.damage = 100;
            NPC.defense = 19;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;

            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
            }
            _outlineColor = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 16));
        }

        private void AI_Idle()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle mineDeploy = AssetRegistry.Sounds.SteamPunking.DescendingMineDeploy;
                mineDeploy.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mineDeploy, NPC.position);
            }

            float inTime = 60f;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 initialVelocity = StartRotation.ToRotationVector2() * MathHelper.Lerp(75f, 0f, ease);
            Vector2 hoverVelocity = new Vector2(0, MathF.Sin(Timer * 0.06f));
            NPC.velocity = initialVelocity + hoverVelocity;
            if(Timer > 5)
            {
                NPC.dontTakeDamage = false;
            }
        }

        private void AI_Death()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 targetNormal = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    Vector2 fireVelocity = targetNormal * 15f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity, 
                        ModContent.ProjectileType<DescendingNodeBeam>(), BeamDamage, 1, Main.myPlayer);
                }
            }


            //Make a cool little explosion
            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Lerp(Color.Green, Color.DarkBlue, 0.5f),
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
                particle.Scale *= 0.5f;
            }

            NPC.active = false;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(projectile, hit, damageDone);

            //This will be called on the server I'm pretty sure
            //Since the server owns the projectile, meaning our method will work :)
            if (projectile.type == ModContent.ProjectileType<DescendingNodeTriggeringBeam>())
            {
                projectile.Kill();
                SwitchState(AIState.Death);
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawSprite(spriteBatch, screenPos, drawColor);

            drawColor *= ExtraMath.Osc(0f, 0.5f, speed: 10f);
            drawColor.A = 0;
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D twinTexture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, 1, spriteEffects, 0f);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            DrawSprite(spriteBatch, screenPos + Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos + Vector2.UnitY * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitY * outlineOffset, _outlineColor);
        }
    }

    public class DescendingFire : ScarletProjectile
    {
        private float _fireTime;
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[1];
        private Vector2[] _oldSmokeCenterPos;
        public Vector2[] SmokeOldCenterPos
        {
            get
            {
                if (_oldSmokeCenterPos == null)
                    _oldSmokeCenterPos = new Vector2[SmokeTrailCacheLength];
                return _oldSmokeCenterPos;
            }
            private set
            {
                _oldSmokeCenterPos = value;
            }
        }


        private Vector2 StartWhipPosition;
        private Vector2 TargetWhipPosition;
        private Vector2 InitialVelocity;
        private Vector2 TargetVelocity;


        public override string Texture => TextureRegistry.CandleFlame;

        public int SmokeTrailCacheLength;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(StartWhipPosition);
            writer.WriteVector2(TargetWhipPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            StartWhipPosition = reader.ReadVector2();
            TargetWhipPosition = reader.ReadVector2();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 15;
            SmokeTrailCacheLength = 25;
            Projectile.width = 11;
            Projectile.height = 11;
            Projectile.hostile = true;
            Projectile.light = 0.278f;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            if (SmokeTrailCacheLength > 0)
            {
                for (int i = SmokeTrailCacheLength - 1; i > 0; i--)
                {
                    SmokeOldCenterPos[i] = SmokeOldCenterPos[i - 1];
                }
                SmokeOldCenterPos[0] = Projectile.Center;
            }

            Color twinColor = GetTwinColor();
            Timer++;
            float lightningAuraProgress = Timer / 180f;
            float easedLightningAuraProgress = Easing.SpikeOutCirc(lightningAuraProgress);
            if (Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }

            if (Timer > 30f)
            {
                Projectile.extraUpdates = 0;
            }
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer < 30 && Timer % 5 == 0)
            {
                FXUtil.GlowCircleBoom(Projectile.Center,
                  innerColor: twinColor,
                  glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                  outerGlowColor: Color.Black, duration: 5, baseSize: 0.04f);
            }
            if (Timer == 30)
            {
                Particle.NewParticle<SkullParticle>(Projectile.Center, Vector2.Zero, Color.Red);
            }
            if (Timer == 70)
            {
                //Ping Sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                for (float i = 0; i < 2; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    //     rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(TargetWhipPosition,
                        innerColor: Color.White,
                        glowColor: GetTwinColor(),
                        outerGlowColor: Color.Black,
                        baseSize: 0.1f,
                        duration: 15);
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            if (Timer == 30)
            {
                //Ping Sound
                var part = FXUtil.GlowCircleBoom(TargetWhipPosition,
                                  innerColor: twinColor,
                                  glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                                  outerGlowColor: Color.Red, duration: 12, baseSize: 0.06f);
                part.Scale *= 0.5f;
            }
            if (Timer == 50)
            {
                //Ping Sound
                var part = FXUtil.GlowCircleBoom(TargetWhipPosition,
                                  innerColor: twinColor,
                                  glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                                  outerGlowColor: Color.Black, duration: 12, baseSize: 0.06f);
                part.Scale *= 0.5f;
            }

            if (Timer > 200)
            {
                _fireTime += MathHelper.Lerp(0.1f, 0.0f, (Timer - 200) / 40f);
            }
            else
            {
                _fireTime += 0.1f;
            }

            if (Timer > 90 && Timer % 4 == 0)
            {
                Particle.NewParticle<FlareParticle>(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero);
            }
            if (Timer > 90 && Timer < 100)
            {
                Projectile.velocity *= 1.1f;
            }
            else if (Timer > 100)
            {
                if (Projectile.velocity.Length() > InitialVelocity.Length())
                {
                    Projectile.velocity *= 0.9f;
                }
            }
            if (Timer % 4 == 0)
            {
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    innerColor: twinColor,
                    glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = -Projectile.velocity.RotatedByRandom(0.6f);
                particle.Scale *= 0.5f;
                particle.Rotation = particle.Velocity.ToRotation();
            }

            if (Timer > 200)
            {
                Projectile.velocity *= 0.96f;
            }
            if (Timer % 6 == 0)
            {
                for (float f = 0; f < 1; f++)
                {
                    Vector2 pVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<SparkParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.Yellow;
                    spark.outerColor = Color.Red;
                }
            }


            if (Timer > 90)
            {

            }
            if (Timer >= 120 && Projectile.velocity.Length() <= 3)
            {
                Projectile.Kill();
            }

            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1000);
            if (player != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 1);
            }
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }
        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Red;
            }
        }


        public float WidthFunction(float completionRatio)
        {
            float w = MathHelper.SmoothStep(26, 54, EasingFunction.QuadraticBump(completionRatio));
            //       w = MathHelper.Lerp(w, 0f, EasingFunction.InOutSine((Timer - 200) / 40f));
            return w;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color twinColor = GetTwinColor();
            Color tipColor = Color.Lerp(twinColor, Color.Lerp(twinColor, Color.DarkBlue, 0.5f), completionRatio);
            Color finalColor = Color.Lerp(twinColor, tipColor, EasingFunction.QuadraticBump(MathF.Pow(completionRatio, 0.5f)));
            Color finalColor2 = Color.Lerp(Color.Transparent, finalColor, EasingFunction.QuadraticBump(completionRatio));
            finalColor2 = Color.Lerp(finalColor2, Color.DarkRed, (Timer - 200) / 40f);
            return finalColor2;
        }
        public float SmokeWidthFunction(float completionRatio)
        {
            float w = MathHelper.SmoothStep(0, 75, EasingFunction.QuadraticBump(completionRatio));
            return w;
        }

        public Color SmokeColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, EasingFunction.InOutSine(completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BlackFireSmokeShader blackSmokeShader = BlackFireSmokeShader.Instance;
            TrailDrawer.Draw(Main.spriteBatch, SmokeOldCenterPos, OldCenterRot, SmokeColorFunction, SmokeWidthFunction, blackSmokeShader, Vector2.Zero);

            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.Time = _fireTime;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, OldCenterRot, ColorFunction, WidthFunction, blackFireShader, Vector2.Zero);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Color twinColor = GetTwinColor();
            Color darkerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            SoundStyle shot = AssetRegistry.Sounds.Magic.RadiantCast1;
            shot.PitchVariance = 0.3f;
            SoundEngine.PlaySound(shot, Projectile.position);
            SoundStyle shot2 = SoundID.DD2_BetsyFireballImpact;
            shot2.PitchVariance = 0.3f;
            SoundEngine.PlaySound(shot2, Projectile.position);
            var part = FXUtil.GlowCircleBoom(Projectile.Center,
                              innerColor: twinColor,
                              glowColor: darkerColor,
                              outerGlowColor: Color.Black, duration: 24, baseSize: 0.14f);
            part.Scale *= 1.225f;
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (int i = 0; i < SmokeOldCenterPos.Length; i++)
            {
                Vector2 pos = SmokeOldCenterPos[i];
                if (i < 8)
                    continue;
                if (Main.rand.NextBool(4))
                {
                    Vector2 velocity = -Projectile.oldVelocity;
                    Particle.NewBlackParticle<BlackSmokeParticle>(pos, velocity * 0.5f, Color.White);
                }
            }

            for (float i = 0; i < 15; i++)
            {
                float rot = rot = -Vector2.UnitY.ToRotation();
                rot += Main.rand.NextFloat(-0.5f, 0.5f);

                Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center + offset,
                    innerColor: GetTwinColor(),
                    glowColor: darkerColor,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = velocity;
                particle.Scale *= 0.35f;
                particle.Rotation = rot;
            }

            FXUtil.ShakeCamera(Projectile.position, 100, 4);
            Vector2 position = Projectile.Center;
            Vector2 lvelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero) * 8;
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: twinColor,
                    outerColor: darkerColor,
                    fadeToColor: Color.Red,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
            }
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = Particle.NewParticle<SparkParticle>(position + Main.rand.NextVector2Circular(64, 64), pVelocity);
            }

            var sear = Particle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);
            sear.innerColor = twinColor;
            sear.outerColor = Color.Lerp(sear.innerColor, Color.Black, 0.5f);
            sear.fadeToColor = Color.Black;
            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), pVelocity, newColor: Color.Black);
            }

        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);

        }
    }

    public class DescendingTwins : ScarletBoss
    {
        private enum TwinAttackState
        {
            SummonTwins,
            Idle,
            DashDance_Part1,
            DashDance_Part2,
            TwinFlameSword,
            HighSpeedCrash,
            BouncingDash,
            NodeLay,
            FlameTornado,
        }

        private ref float Timer => ref NPC.ai[0];

        private int _retinaIndex;
        private int _spazzIndex;
        private NPC Retina => Main.npc[_retinaIndex];
        private NPC Spazz => Main.npc[_spazzIndex];

        public Vector2 GetBouncingDashAnchorPoint()
        {
            return Spazz.Center;
        }

        private bool IsAwaitingCommand(NPC npc)
        {
            DescendingTwin.TwinAIState state = (DescendingTwin.TwinAIState)npc.ai[1];
            if (state == DescendingTwin.TwinAIState.Idle)
                return true;
            return false;
        }

        private void Command(NPC npc, DescendingTwin.TwinAIState state)
        {
            npc.ai[2] = (float)state;
        }

        private bool RetinaAwaitingCommand => IsAwaitingCommand(Retina);
        private bool SpazzAwaitingCommand => IsAwaitingCommand(Spazz);
        private void CommandRetina(DescendingTwin.TwinAIState state) => Command(Retina, state);
        private void CommandSpazz(DescendingTwin.TwinAIState state) => Command(Spazz, state);

        public bool StopFiringAtNodes => SpazzAwaitingCommand;
        private TwinAttackState State
        {
            get => (TwinAttackState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[2];


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_retinaIndex);
            writer.Write(_spazzIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _retinaIndex = reader.ReadInt32();
            _spazzIndex = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 19;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case TwinAttackState.SummonTwins:
                    AI_SummonTwins();
                    break;
                case TwinAttackState.Idle:
                    AI_Idle();
                    break;
                case TwinAttackState.DashDance_Part1:
                    AI_DashDancePart1();
                    break;
                case TwinAttackState.DashDance_Part2:
                    AI_DashDancePart2();
                    break;
                case TwinAttackState.TwinFlameSword:
                    AI_TwinFlameSword();
                    break;
                case TwinAttackState.HighSpeedCrash:
                    AI_HighSpeedCrash();
                    break;
                case TwinAttackState.BouncingDash:
                    AI_BouncingDash();
                    break;
                case TwinAttackState.NodeLay:
                    AI_NodeLay();
                    break;
                case TwinAttackState.FlameTornado:
                    AI_FlameTornado();
                    break;
            }
        }


        private void SwitchState(TwinAttackState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_SummonTwins()
        {
            Timer++;
            if (Timer == 3)
            {
                if (MultiplayerHelper.IsHost)
                {
                    var source = NPC.GetSource_FromThis();
                    int x = (int)NPC.Center.X;
                    int y = (int)NPC.Center.Y;
                    _retinaIndex = NPC.NewNPC(source, x, y, ModContent.NPCType<DescendingTwin>(), ai0: 0,
                        ai1: (int)DescendingTwin.TwinAIState.SpawnRetina,
                        ai2: NPC.whoAmI);

                    _spazzIndex = NPC.NewNPC(source, x, y, ModContent.NPCType<DescendingTwin>(), ai0: 0,
                        ai1: (int)DescendingTwin.TwinAIState.SpawnSpazz,
                        ai2: NPC.whoAmI);

                    SwitchState(TwinAttackState.Idle);
                }
            }
        }

        private void ChooseAttack()
        {
            SwitchState(TwinAttackState.FlameTornado);
        }

        private void AI_Idle()
        {

            //Alright, So nowe have the commander setup, let's get this dash dance attack working
            AttackNumber = 0f;
            if (SpazzAwaitingCommand && RetinaAwaitingCommand)
            {
                Timer++;
                if (Timer == 1)
                {
                    NPC.TargetClosest();
                }


                if (Timer >= 60)
                {
                    ChooseAttack();
                }
            }
        }

        private void AI_DashDancePart1()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //So how do we want this to work?
            //It should be pretty simple actually,
            //We're going to have each twin dash 5 times
            //Alternating between each other for a total of 10 dashes
            //Then we'll wait for them to both stop and throw it into the second dash dance
            if (AttackNumber < 10)
            {
                if (Timer >= 60)
                {                //Alternate between the twins and make them dash at you
                                 //The timing between these is based on the twin itself, not the commander
                                 //If you want to make it faster or slower, just edit that
                    if (AttackNumber % 2 == 0)
                    {
                        if (SpazzAwaitingCommand)
                        {
                            CommandSpazz(DescendingTwin.TwinAIState.SimpleDashStart);
                            AttackNumber++;
                        }
                    }
                    else
                    {
                        if (RetinaAwaitingCommand)
                        {
                            CommandRetina(DescendingTwin.TwinAIState.SimpleDashStart);
                            AttackNumber++;
                        }
                    }
                    Timer = 0;
                }

            }
            else
            {
                SwitchState(TwinAttackState.DashDance_Part2);
            }
        }

        private void AI_DashDancePart2()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //Wait for both of them to finish and then put them into the dash dance state
            if (SpazzAwaitingCommand && RetinaAwaitingCommand)
            {
                CommandSpazz(DescendingTwin.TwinAIState.DashDanceStart);
                CommandRetina(DescendingTwin.TwinAIState.DashDanceStart);
                SwitchState(TwinAttackState.Idle);
            }
        }

        private void AI_TwinFlameSword()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.FlameSwordStart);
                CommandRetina(DescendingTwin.TwinAIState.FlameSwordStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }
        private void AI_HighSpeedCrash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.HighSpeedCrashStart);
                CommandRetina(DescendingTwin.TwinAIState.HighSpeedCrashStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }

        }
        private void AI_BouncingDash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.BouncingDashStartAnchor);
                CommandRetina(DescendingTwin.TwinAIState.BouncingDashStart);
            }

            if (Timer >= 60)
            {
                if (RetinaAwaitingCommand)
                {
                    CommandSpazz(DescendingTwin.TwinAIState.BouncingDashEnd);
                    SwitchState(TwinAttackState.Idle);
                }
            }

        }

        private void AI_NodeLay()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.SpazzNodeLayWindup);
                CommandRetina(DescendingTwin.TwinAIState.RetinaNodeLayWindup);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }


        private void AI_FlameTornado()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.FlameTornadoStart);
                CommandRetina(DescendingTwin.TwinAIState.FlameTornadoStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }
    }

    public class DescendingBigBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[1];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            ShakeModSystem.Shake = 10;
            if (Timer == 1)
            {
                SoundStyle boomSound = AssetRegistry.Sounds.SteamPunking.DescendingBoom;
                boomSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(boomSound, Projectile.position);

                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.White;
                    spark.outerColor = GetTwinColor();
                    spark.fadeToColor = Color.Blue;
                }

                var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.Scale *= 2;
                part.noStretch = true;
                part.innerColor = GetTwinColor();
                part.outerColor = Color.Lerp(GetTwinColor(), Color.Blue, 0.25f);
                part.fadeToColor = Color.Lerp(GetTwinColor(), Color.Black, 0.5f);
                for (float f = 0; f < 8; f++)
                {
                    float radius = 800;
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                    Vector2 velocity = Projectile.Center - spawnPos;
                    velocity = velocity.SafeNormalize(Vector2.Zero);
                    velocity *= Main.rand.NextFloat(8, 32);
                    var p = FXUtil.GlowStretch(spawnPos, velocity);
                    p.InnerColor = GetTwinColor();
                    p.GlowColor = Color.Lerp(GetTwinColor(), Color.Blue, 0.25f);
                    p.OuterGlowColor = Color.Lerp(GetTwinColor(), Color.Black, 0.5f);
                    p.Scale *= 3f;
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 10);
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                    pVelocity *= Main.rand.NextFloat(0.5f, 8f);
                    var spark = Particle.NewParticle<EmberParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }

                float numDust = 16;
                for (float n = 0; n < numDust; n++)
                {
                    SpawnFlameDust(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
                    SpawnGlowDust(Projectile.Center, Main.rand.NextVector2Circular(64, 64));
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: GetTwinColor(),
                        outerGlowColor: Color.Lerp(GetTwinColor(), Color.DarkBlue, 0.5f),
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 4f;
                }
            }
        }

        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Red;
            }
        }

        private void SpawnFlameDust(Vector2 position, Vector2 velocity)
        {
            var p = Particle.NewParticle<GlowFragmentParticle>(position, velocity, Color.White, Scale: 4f);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }
        private void SpawnGlowDust(Vector2 position, Vector2 velocity)
        {
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), velocity, newColor: GetTwinColor(), Scale: 2f);
        }
    }

    public class DescendingTornadoBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[1];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            ShakeModSystem.Shake = 15;
            if (Timer == 1)
            {
                SoundStyle boomSound = AssetRegistry.Sounds.SteamPunking.DescendingBoom;
                boomSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(boomSound, Projectile.position);

                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.White;
                    spark.outerColor = GetTwinColor();
                    spark.fadeToColor = Color.Blue;
                }

                var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.Scale *= 2;
                part.noStretch = true;
                part.innerColor = GetTwinColor();
                part.outerColor = Color.Lerp(GetTwinColor(), Color.Blue, 0.25f);
                part.fadeToColor = Color.Lerp(GetTwinColor(), Color.Black, 0.5f);
                for (float f = 0; f < 8; f++)
                {
                    float radius = 800;
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                    Vector2 velocity = Projectile.Center - spawnPos;
                    velocity = velocity.SafeNormalize(Vector2.Zero);
                    velocity *= Main.rand.NextFloat(8, 32);
                    var p = FXUtil.GlowStretch(spawnPos, velocity);
                    p.InnerColor = GetTwinColor();
                    p.GlowColor = Color.Lerp(GetTwinColor(), Color.Blue, 0.25f);
                    p.OuterGlowColor = Color.Lerp(GetTwinColor(), Color.Black, 0.5f);
                    p.Scale *= 3f;
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 10);
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                    pVelocity *= Main.rand.NextFloat(0.5f, 8f);
                    var spark = Particle.NewParticle<EmberParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }

                float numDust = 16;
                for (float n = 0; n < numDust; n++)
                {
                    SpawnFlameDust(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
                    SpawnGlowDust(Projectile.Center, Main.rand.NextVector2Circular(64, 64));
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: GetTwinColor(),
                        outerGlowColor: Color.Lerp(GetTwinColor(), Color.DarkBlue, 0.5f),
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 8f;
                }
            }
        }

        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Red;
            }
        }

        private void SpawnFlameDust(Vector2 position, Vector2 velocity)
        {
            var p = Particle.NewParticle<GlowFragmentParticle>(position, velocity, Color.White, Scale: 4f);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }
        private void SpawnGlowDust(Vector2 position, Vector2 velocity)
        {
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), velocity, newColor: GetTwinColor(), Scale: 2f);
        }
    }

    public class DescendingRisingTornado : ScarletProjectile
    {
        private Vector2 InitialVelocity;
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[2];
        public Vector2 ReTargetPosition;
        public override string Texture => TextureRegistry.EmptyTexture;


        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 64;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(OldCenterPos, projHitbox, targetHitbox);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(ReTargetPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            ReTargetPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
            }

            if(Timer > 120f)
            {
                float completionRatio = (Timer - 120f) / 120f;
                float ease = EasingFunction.InOutSine(completionRatio);
                Vector2 targetVelocity = (ReTargetPosition - Projectile.Center);
                Projectile.velocity = Vector2.Lerp(InitialVelocity, targetVelocity, ease);
            }

            if(Timer == 240)
            {
                if(Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                        ModContent.ProjectileType<DescendingTornadoBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Variant);
                    Projectile.Kill();
                }
            }
            float numFlamePos = OldCenterPos.Length;
            for (int n = 0; n < numFlamePos; n++)
            {
                if (Main.rand.NextBool(32))
                {
                    SpawnFlameDust(OldCenterPos[n]);
                }
            }
        }

        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Red;
            }
        }

        private void SpawnFlameDust(Vector2 position)
        {
            var p = Particle.NewParticle<GlowFragmentParticle>(position, Projectile.velocity.SafeNormalize(Vector2.Zero) * 5f, Color.White);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }


        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.White, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
        }

        private float GetTrailWidth(float completionRatio)
        {
            float outScale = (float)Projectile.timeLeft / 30f;
            float inScale = EasingFunction.InOutSine(Timer / 30f);
            float ease = EasingFunction.InOutSine(outScale);
            return MathHelper.SmoothStep(32, 0, completionRatio) * ease * inScale * 2 * MathHelper.Lerp(8, 1f, EasingFunction.InOutSine(Timer / 30f));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DescendingFlameTrailShader flameTrailShader = DescendingFlameTrailShader.Instance;
            flameTrailShader.LaserTexture = AssetRegistry.Textures.Noise.JungleWaterCaustics;

            flameTrailShader.Tiling = Vector2.One * new Vector2(4, 0.85f);
            Color innerColor;
            Color outerColor;
            switch (Variant)
            {
                default:
                case 0:
                    innerColor = Color.GreenYellow;
                    outerColor = Color.Green;
                    break;
                case 1:
                    innerColor = Color.Yellow;
                    outerColor = Color.Red;
                    break;
            }


            float lerp = EasingFunction.InOutSine(Timer / 20f);
            flameTrailShader.InnerColor = Color.Lerp(Color.White, innerColor, lerp);
            flameTrailShader.OuterColor = Color.Lerp(Color.White, outerColor, lerp);
            flameTrailShader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, flameTrailShader);

            flameTrailShader.BlendState = BlendState.Additive;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, flameTrailShader);
            return false;
        }

    }
    public class DescendingFlameSword : ModProjectile
    {
        private Vector2[] FlamePos = new Vector2[64];
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent => Main.npc[(int)Projectile.ai[1]];
        private int Variant => (int)Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(FlamePos, projHitbox, targetHitbox);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                ShakeModSystem.Shake = 2;
                FXUtil.ShakeCamera(Projectile.position, 1024, 6);


            }

            float numFlamePos = FlamePos.Length;
            for (int n = 0; n < numFlamePos; n++)
            {
                float completionRatio = (float)n / numFlamePos;
                FlamePos[n] = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
                if (Main.rand.NextBool(32))
                {
                    SpawnFlameDust(FlamePos[n]);

                }

            }
            Projectile.Center = Parent.Center;
        }
        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Red;
            }
        }
        private void SpawnFlameDust(Vector2 position)
        {
            var p = Particle.NewParticle<GlowFragmentParticle>(position, Projectile.velocity.SafeNormalize(Vector2.Zero) * 5f, Color.White);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }


        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.White, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
        }

        private float GetTrailWidth(float completionRatio)
        {
            float outScale = (float)Projectile.timeLeft / 30f;
            float inScale = EasingFunction.InOutSine(Timer / 30f);
            float ease = EasingFunction.InOutSine(outScale);
            return MathHelper.SmoothStep(0, 32, completionRatio) * ease * inScale * 2 * MathHelper.Lerp(8, 1f, EasingFunction.InOutSine(Timer / 30f));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DescendingFlameTrailShader flameTrailShader = DescendingFlameTrailShader.Instance;
            flameTrailShader.LaserTexture = AssetRegistry.Textures.Noise.JungleWaterCaustics;

            flameTrailShader.Tiling = Vector2.One * new Vector2(4, 0.85f);
            Color innerColor;
            Color outerColor;
            switch (Variant)
            {
                default:
                case 0:
                    innerColor = Color.GreenYellow;
                    outerColor = Color.Green;
                    break;
                case 1:
                    innerColor = Color.Yellow;
                    outerColor = Color.Red;
                    break;
            }


            float lerp = EasingFunction.InOutSine(Timer / 20f);
            flameTrailShader.InnerColor = Color.Lerp(Color.White, innerColor, lerp);
            flameTrailShader.OuterColor = Color.Lerp(Color.White, outerColor, lerp);
            flameTrailShader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, FlamePos, GetTrailColor, GetTrailWidth, flameTrailShader);

            flameTrailShader.BlendState = BlendState.Additive;
            TrailDrawer.Draw(Main.spriteBatch, FlamePos, GetTrailColor, GetTrailWidth, flameTrailShader);
            return false;
        }

    }
    //The thing with this boss is that it's a dual synced boss
    //I think the easiest way to do that is to have a single twin npc, and a controller npc
    //That basically sends commands to them telling them what to do
    //In that case, let's create a base class
    //I'm also going to use partial classing here to see how I feel about organizing with it

    public class DescendingTwin : ModNPC,
        IDrawOutlines
    {
        public enum TwinAIState
        {
            SpawnSpazz,
            SpawnRetina,

            Idle,


            SimpleDashStart,
            SimpleDash,
            SimpleDashEnd,

            DashDanceStart,
            DashDancePrepare,
            DashDance,
            DashDanceTwirl,
            DashDanceEnd,


            FlameSwordStart,
            FlameSwordWindup,
            FlameSwordContinuous,
            FlameSwordEnd,

            HighSpeedCrashStart,
            HighSpeedCrashQuickStart,
            HighSpeedCrashPreDash,
            HighSpeedCrashWindup,
            HighSpeedCrashCrash,
            HIghSpeedCrashEnd,

            BouncingDashStartAnchor,
            BouncingDashStart,
            BouncingDashIn,
            BouncingDashOut,
            BouncingDashEnd,

            SpazzNodeLayWindup,
            SpazzNodeLayShoot,
            RetinaNodeLayWindup,
            RetinaNodeLayShoot,
            NodeEnd,

            FlameTornadoStart,
            FlameTornadoWindup,
            FlameTornadoShoot,
            FlameTornadoEnd
        }


        private enum TwinVariant
        {
            Spazz,
            Retina
        }

        private bool _contactDamage;
        private float _rotationTimer;
        private int _parentIndex;
        private ref float Timer => ref NPC.ai[0];
        private TwinAIState State
        {
            get => (TwinAIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private TwinAIState NextCommandState
        {
            get => (TwinAIState)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[3];
        private TwinVariant Variant;
        private int FlameSwordDamage => 20;
        private int DescendingBigBoomDamage => 30;

        private int DescendingFireDamage => 15;
        private int DescendingNodeLaserDamage => 15;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 20;
            NPC.lifeMax = 18000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss6");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_simpleDashNormal);
            writer.WriteVector2(_highSpeedTargetPosition);
            writer.Write((float)Variant);
            writer.Write(_parentIndex);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _simpleDashNormal = reader.ReadVector2();
            _highSpeedTargetPosition = reader.ReadVector2();
            Variant = (TwinVariant)reader.ReadSingle();
            _parentIndex = reader.ReadInt32();
        }

        private void SwitchState(TwinAIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();

            //If we don't have a valid target automatically retarget.
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }

            _contactDamage = false;
            switch (State)
            {
                case TwinAIState.SpawnSpazz:
                    AI_SpawnSpazz();
                    break;
                case TwinAIState.SpawnRetina:
                    AI_SpawnRetina();
                    break;

                case TwinAIState.Idle:
                    AI_Idle();
                    break;


                case TwinAIState.SimpleDashStart:
                    AI_SimpleDashStart();
                    break;
                case TwinAIState.SimpleDash:
                    AI_SimpleDash();
                    break;
                case TwinAIState.SimpleDashEnd:
                    AI_SimpleDashEnd();
                    break;

                case TwinAIState.DashDanceStart:
                    AI_DashDanceStart();
                    break;
                case TwinAIState.DashDancePrepare:
                    AI_DashDancePrepare();
                    break;
                case TwinAIState.DashDanceTwirl:
                    AI_DashDanceTwirl();
                    break;
                case TwinAIState.DashDance:
                    AI_DashDance();
                    break;
                case TwinAIState.DashDanceEnd:
                    AI_DashDanceEnd();
                    break;

                case TwinAIState.FlameSwordStart:
                    AI_FlameSwordStart();
                    break;
                case TwinAIState.FlameSwordWindup:
                    AI_FlameSwordAim();
                    break;
                case TwinAIState.FlameSwordContinuous:
                    AI_FlameSwordContinuous();
                    break;
                case TwinAIState.FlameSwordEnd:
                    AI_FlameSwordEnd();
                    break;

                case TwinAIState.HighSpeedCrashStart:
                    AI_HighSpeedCrashStart();
                    break;
                case TwinAIState.HighSpeedCrashQuickStart:
                    AI_HighSpeedCrashQuickStart();
                    break;
                case TwinAIState.HighSpeedCrashPreDash:
                    AI_HighSpeedCrashPreDash();
                    break;
                case TwinAIState.HighSpeedCrashWindup:
                    AI_HighSpeedCrashWindup();
                    break;
                case TwinAIState.HighSpeedCrashCrash:
                    AI_HighSpeedCrashCrash();
                    break;
                case TwinAIState.HIghSpeedCrashEnd:
                    AI_HighSpeedCrashEnd();
                    break;

                case TwinAIState.BouncingDashStart:
                    AI_BouncingDashStart();
                    break;
                case TwinAIState.BouncingDashStartAnchor:
                    AI_BouncingDashAnchor();
                    break;
                case TwinAIState.BouncingDashIn:
                    AI_BouncingDashIn();
                    break;
                case TwinAIState.BouncingDashOut:
                    AI_BouncingDashOut();
                    break;
                case TwinAIState.BouncingDashEnd:
                    AI_BouncingDashEnd();
                    break;

                case TwinAIState.SpazzNodeLayWindup:
                    AI_SpazzNodeLayWindup();
                    break;
                case TwinAIState.SpazzNodeLayShoot:
                    AI_SpazzNodeLayShoot();
                    break;
                case TwinAIState.RetinaNodeLayWindup:
                    AI_RetinaNodeLayWindup();
                    break;
                case TwinAIState.RetinaNodeLayShoot:
                    AI_RetinaNodeLayShoot();
                    break;
                case TwinAIState.NodeEnd:
                    AI_NodeEnd();
                    break;

                case TwinAIState.FlameTornadoStart:
                    AI_FlameTornadoStart();
                    break;
                case TwinAIState.FlameTornadoWindup:
                    AI_FlameTornadoWindup();
                    break;
                case TwinAIState.FlameTornadoShoot:
                    AI_FlameTornadoShoot();
                    break;
                case TwinAIState.FlameTornadoEnd:
                    AI_FlameTornadoEnd();
                    break;

            }
            Lighting.AddLight(NPC.Center, Variant == TwinVariant.Spazz ? TorchID.Cursed : TorchID.Red);
            UpdateDraw();
        }

        private Player Target => Main.player[NPC.target];
        private Vector2 TargetNormal => NPC.DirectionTo(Target.Center);
        private DescendingTwins Commander => (DescendingTwins)Main.npc[_parentIndex].ModNPC;

        #region Flame Tornado
        private Vector2 GetFlameTornadoStartOffset()
        {
            float distanceOffset = 450f;
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return -Vector2.UnitX * distanceOffset;
                case TwinVariant.Retina:
                    return Vector2.UnitX * distanceOffset;
            }
        }

        private void AI_FlameTornadoStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                _simpleDashNormal = NPC.velocity;
            }

            if(Timer % 5 == 0)
            {
                SpawnSteamParticle();
                SpawnFlameDust();
            }

            /*
             * 
             * Both of them aim above you, shooting a type of fire (Descender Retina), shoots a red fire,
             * while Descender Spazz, shoots a green flame, and they make a crossing sword, and continuously going downwards, making you dodge
             */

            //So first we need to get them ina  good position for doing this attack
            //I think it'd be best if they position themselves on opposite sides of you
            //Alright so
            //First let's get that position and move to it
            Vector2 flameSwordOffset = GetFlameTornadoStartOffset();
            Vector2 positionToMoveTo = Target.Center + flameSwordOffset;


            float windupTime = 80f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, movementVelocity, completionRatio);

            //Look at the player
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.FlameTornadoWindup);
            }
        }

        private void AI_FlameTornadoWindup()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle beep = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beep.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beep, NPC.position);

                _simpleDashNormal = TargetNormal;
            }

            NPC.velocity.Y -= 1;
            NPC.velocity *= 0.9f;

            //We need to look up at a 30 degree angle, shoot, and then move downward
            //Alright
            float windupTime = 30f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.Anticipation(completionRatio);
            float directionToRotate = _simpleDashNormal.X > 0 ? 1f : -1f;
            float radiansOffset = MathHelper.Lerp(0f, -MathHelper.PiOver4 / 2f * directionToRotate, ease);

            //That new direction that we are facing
            Vector2 newNormal = _simpleDashNormal.RotatedBy(radiansOffset);
            NPC.rotation = newNormal.ToRotation();
            TargetOutlineColor = Color.Yellow;

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = NPC.rotation;
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.FlameTornadoShoot);
            }
        }

        private void AI_FlameTornadoShoot()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle flamethrower = AssetRegistry.Sounds.SteamPunking.DescendingFlamethrower;
                flamethrower.PitchVariance = 0.3f;
                SoundEngine.PlaySound(flamethrower, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 15;
                    DescendingRisingTornado tornado = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<DescendingRisingTornado>(), FlameSwordDamage, 1, Main.myPlayer, ai2: (int)Variant).ModProjectile as DescendingRisingTornado;
                    tornado.ReTargetPosition = Target.Center;
                }
                SpawnFlameDonut();
            }


            //Move downward whiel shooting
            float continuosTime = 100f;
            float completionRatio = Timer / continuosTime;
            float ease = EasingFunction.Anticipation2(completionRatio / 0.5f);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, -NPC.rotation.ToRotationVector2() * 10f, ease);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            TargetOutlineColor = Color.Yellow;
            ShakeModSystem.Shake = 4;
            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
                SpawnSteamParticle();
            }

            if (Timer >= continuosTime)
            {
                SwitchState(TwinAIState.FlameTornadoEnd);
            }
        }

        private void AI_FlameTornadoEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if(Timer >= 15)
            {
                SwitchState(TwinAIState.Idle);
            }
        }

        #endregion

        #region Node Lay
        private NPC GetNextNode()
        {
            int type = ModContent.NPCType<DescendingNode>();
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == type)
                {
                    if (npc.ai[3] == 0)
                        return npc;
                }
            }
            return null;
        }
        private void AI_RetinaNodeLayWindup()
        {
            if(Timer < 1)
            {
                Timer++;
            }
       
            if (Timer == 1)
            {

                SoundStyle beepSound = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beepSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beepSound, NPC.position);
            }

            NPC nextNode = GetNextNode();
            if (nextNode == null && Commander.StopFiringAtNodes)
            {
                SwitchState(TwinAIState.NodeEnd);
            }
            if (nextNode != null)
            {
                Timer++;
                Vector2 targetNormal = (nextNode.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                Vector2 myNormal = NPC.rotation.ToRotationVector2();
                float dp = Vector2.Dot(myNormal, targetNormal);
                if(dp > 0.99f && Timer > 15)
                {
                    SwitchState(TwinAIState.RetinaNodeLayShoot);
                }
                NPC.rotation = Utils.AngleTowards(NPC.rotation, targetNormal.ToRotation(), 0.1f);

                //Aim the telegraph

                _afterImageAlpha = 0f;
                _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 1f, 0.01f);
                _telegraphLineRot = NPC.rotation;
                TargetOutlineColor = Color.Yellow;
            }

            LayMovement();
        }

        private void AI_RetinaNodeLayShoot()
        {
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    NPC nextNode = GetNextNode();
                    if(nextNode != null)
                    {
                        nextNode.ai[3] = 1;
                        nextNode.netUpdate = true;

                        Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 8;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                            ModContent.ProjectileType<DescendingNodeTriggeringBeam>(), DescendingNodeLaserDamage, 1, Main.myPlayer, ai1: nextNode.whoAmI);
                    }
                }
            }
            float shootTime = 5;
            float completionRatio = Timer / shootTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            _afterImageAlpha = 0f;
            TargetOutlineColor = Color.Yellow;

            IdleMovement();
            if (Timer >= shootTime)
            {
                SwitchState(TwinAIState.RetinaNodeLayWindup);
            }
        }

        private Vector2 _layStartCenter;
        private void LayMovement()
        {

            //So we should slowly move towards the player if they're far, if not we'll just hover in place.
            //Step 1. Look towards the player, we can do this by calculating a target normal, calculating an angle and then lerping to it

            //So how do we want this attack to look?
            //I think the twins should orbit around a circle for a bit, on opposite points
            //Then after a while, they look towards you and dash to the point, when they touch each other
            //They'll burst into the dash
            //Alright so

            //First we need to create a circle around our target
            _rotationTimer++;
            if(_rotationTimer == 1)
            {
                _layStartCenter = Target.Center;
                _simpleDashNormal = NPC.velocity;
            }

            float circleRadius = 250f;
            Vector2 initialDirection = -Vector2.UnitY;
            Vector2 dashVector = initialDirection * circleRadius;

            //Get an offset based on the variant that this goober is
            float radiansOffset = Variant == TwinVariant.Spazz ? MathHelper.Pi : 0;
            radiansOffset -= MathHelper.PiOver2;
            radiansOffset += _rotationTimer * 0.05f;

            Vector2 positionToMoveTo = _layStartCenter + dashVector.RotatedBy(radiansOffset);
            Vector2 velThere = positionToMoveTo - NPC.Center;
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, velThere, EasingFunction.InOutSine(_rotationTimer / 120f));
        }

        private void AI_SpazzNodeLayWindup()
        {
            //For this attack we'll use an NPC for the nodes, it'll shoot a node NPC
            //Then retina will look for these npcs as long as they exist he'll be shooting them
            //Yeah, ok
            //SO first we choose a random direction
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    _simpleDashNormal = TargetNormal.RotatedByRandom(1.5f);
                    NPC.netUpdate = true;
                }
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
            }

            float targetAngle = _simpleDashNormal.ToRotation();

            LayMovement();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            float windUpTime = 15f;
            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, ease);
            _telegraphLineRot = NPC.rotation;
            _afterImageAlpha = 0f;

            TargetOutlineColor = Color.Yellow;
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.SpazzNodeLayShoot);
            }
        }

        private void AI_SpazzNodeLayShoot()
        {
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int x = (int)NPC.Center.X;
                    int y = (int)NPC.Center.Y;
                    float fireRotation = NPC.rotation;
                    NPC.NewNPC(NPC.GetSource_FromThis(), x, y,
                        ModContent.NPCType<DescendingNode>(), ai1: fireRotation);
                }
            }
            float shootTime = 5f;
            float completionRatio = Timer / shootTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            LayMovement();
            TargetOutlineColor = Color.Yellow;
            if (Timer >= shootTime)
            {
                AttackNumber++;
                if (AttackNumber >= 12f)
                {
                    SwitchState(TwinAIState.NodeEnd);
                }
                else
                {
                    SwitchState(TwinAIState.SpazzNodeLayWindup);
                }
            }
        }

        private void AI_NodeEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;

            //Rotate towards the twarget
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);
            _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 0f, 0.1f);
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 15)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
        #endregion

        #region BouncingDash
        private Vector2 GetBounceDashAnchorPoint()
        {
            return Commander.GetBouncingDashAnchorPoint();
        }


        private void AI_BouncingDashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();

            }

            float windupTime = 30f;
            float completionRatio = Timer / windupTime;

            Vector2 anchorPoint = GetBounceDashAnchorPoint();
            Vector2 velocityThere = (anchorPoint - NPC.Center).SafeNormalize(Vector2.Zero);
            NPC.velocity = Vector2.Lerp(NPC.velocity, velocityThere, 0.1f);

            _simpleDashNormal = (NPC.Center - anchorPoint).SafeNormalize(Vector2.Zero);
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.BouncingDashIn);
            }
        }


        private void AI_BouncingDashAnchor()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _afterImageAlpha = 1f;
            //So we should slowly move towards the player if they're far, if not we'll just hover in place.
            //Step 1. Look towards the player, we can do this by calculating a target normal, calculating an angle and then lerping to it
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            //Step 2. Check the distance between this current twin and the player
            //If the distance is too far we'll move closer to them, if not we just slow down/sit there
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float maxDistance = 450;
            if (distanceToTarget > maxDistance)
            {
                //We should scale the movement velocity based on the distance, so the farther they are the faster we'll move
                Vector2 movementVelocity = targetNormal * distanceToTarget / 32f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.05f);
            }
            else
            {
                //Otherwise, we'll just slow down
                //We want to keep a little bit of movement velocity so it's not just completely static
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetNormal * 3f, 0.1f);

                //Stpe 3. Add a little bit of hovering velocity for a cool effect
                float yHover = MathF.Sin(Timer * 0.1f) * 0.5f;
                NPC.velocity.Y += yHover;
            }

            NPC.rotation += MathHelper.Lerp(0f, 0.2f, EasingFunction.InOutSine(Timer / 120f));
            TargetOutlineColor = Color.Yellow;
            //Receive the next command state.
            //This should be automatically netcoded btw
            if (NextCommandState == TwinAIState.BouncingDashEnd)
            {
                SwitchState(NextCommandState);
                NextCommandState = TwinAIState.Idle;
            }
        }

        private void AI_BouncingDashIn()
        {
            _rotationTimer++;
            Timer++;
            if (Timer == 1)
            {
                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            float inTime = 30f;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.InExpo(completionRatio);
            Vector2 anchorPoint = GetBounceDashAnchorPoint();
            float offsetDistance = MathHelper.Lerp(300f, 0f, ease);


            Vector2 bounceOffset = _simpleDashNormal * offsetDistance;
            bounceOffset = bounceOffset.RotatedBy(_rotationTimer * 0.05f);

            Vector2 targetPosition = anchorPoint + bounceOffset;
            Vector2 targetVelocity = (targetPosition - NPC.Center);
            NPC.velocity = targetVelocity;
            NPC.rotation = NPC.velocity.ToRotation();

            _afterImageAlpha = 1f;
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= inTime)
            {
                SwitchState(TwinAIState.BouncingDashOut);
            }
        }

        private void AI_BouncingDashOut()
        {
            _rotationTimer++;
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DescendingBigBoom>(),
                        DescendingBigBoomDamage, 1, Main.myPlayer, ai1: (int)Variant);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, TargetNormal * 12f, ModContent.ProjectileType<DescendingFire>(),
                        DescendingFireDamage, 1, Main.myPlayer, ai1: (int)(1 - Variant));
                }
            }

            float inTime = 30f;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 anchorPoint = GetBounceDashAnchorPoint();

            float offsetDistance = MathHelper.Lerp(0f, 300f, ease);

            Vector2 bounceOffset = _simpleDashNormal * offsetDistance;
            bounceOffset = bounceOffset.RotatedBy(_rotationTimer * 0.05f);

            Vector2 targetPosition = anchorPoint + bounceOffset;
            Vector2 targetVelocity = (targetPosition - NPC.Center);
            NPC.velocity = targetVelocity;
            NPC.rotation = NPC.velocity.ToRotation();

            _afterImageAlpha = 1f;
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= inTime)
            {
                AttackNumber++;
                if (AttackNumber >= 16)
                {
                    SwitchState(TwinAIState.BouncingDashEnd);
                }
                else
                {
                    SwitchState(TwinAIState.BouncingDashIn);
                }
            }
        }

        private void AI_BouncingDashEnd()
        {
            Timer++;
            NPC.velocity *= 0.8f;
            if (Timer >= 15f)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
        #endregion

        #region High Speed Crash
        private Vector2 _highSpeedTargetPosition;
        private Vector2 GetHighSpeedCrashStartOffset()
        {
            float distanceOffset = 200;
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return -Vector2.UnitX * distanceOffset;
                case TwinVariant.Retina:
                    return Vector2.UnitX * distanceOffset;
            }
        }

        private void AI_HighSpeedCrashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }            /*
             * 
             * Both of them aim above you, shooting a type of fire (Descender Retina), shoots a red fire,
             * while Descender Spazz, shoots a green flame, and they make a crossing sword, and continuously going downwards, making you dodge
             */

            //So first we need to get them ina  good position for doing this attack
            //I think it'd be best if they position themselves on opposite sides of you
            //Alright so
            //First let's get that position and move to it
            Vector2 highSpeedCrashStartOffset = GetHighSpeedCrashStartOffset();
            Vector2 positionToMoveTo = Target.Center + highSpeedCrashStartOffset;


            float windupTime = 80f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, movementVelocity, completionRatio);

            //Look at the player
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = NPC.rotation;

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.HighSpeedCrashWindup);
            }
        }
        private void AI_HighSpeedCrashQuickStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }            /*
             * 
             * Both of them aim above you, shooting a type of fire (Descender Retina), shoots a red fire,
             * while Descender Spazz, shoots a green flame, and they make a crossing sword, and continuously going downwards, making you dodge
             */

            //So first we need to get them ina  good position for doing this attack
            //I think it'd be best if they position themselves on opposite sides of you
            //Alright so
            //First let's get that position and move to it
            Vector2 highSpeedCrashStartOffset = GetHighSpeedCrashStartOffset();
            highSpeedCrashStartOffset = highSpeedCrashStartOffset.RotatedBy(AttackNumber * MathHelper.PiOver4);
            Vector2 positionToMoveTo = Target.Center + highSpeedCrashStartOffset;


            float windupTime = 25f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, movementVelocity, completionRatio);

            //Look at the player
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = NPC.rotation;

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.HighSpeedCrashWindup);
            }
        }

        private void AI_HighSpeedCrashWindup()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = NPC.rotation.ToRotationVector2();
                _highSpeedTargetPosition = Target.Center;
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
            }
            //High speed crash set the rotation
            const float windupTime = 30f;
            float completionRatio = Timer / windupTime;
            float directionToRotate = _simpleDashNormal.X > 0 ? 1f : -1f;
            float radiansToRotateBy = MathHelper.Lerp(0f, -MathHelper.Pi * directionToRotate, completionRatio);

            Vector2 newDashNormal = _simpleDashNormal.RotatedBy(radiansToRotateBy);
            NPC.rotation = newDashNormal.ToRotation();
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, completionRatio);

            float speed = MathHelper.Lerp(4f, 50f, completionRatio);
            NPC.velocity = newDashNormal * speed;
            TargetOutlineColor = Color.Yellow;
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.HighSpeedCrashPreDash);
            }
        }

        private void AI_HighSpeedCrashPreDash()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = (_highSpeedTargetPosition - NPC.Center).SafeNormalize(Vector2.Zero);
                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }
            const float windUpTime = 15f;
            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, _simpleDashNormal * 5f, ease);
            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.HighSpeedCrashCrash);
            }
        }

        private void AI_HighSpeedCrashCrash()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = (_highSpeedTargetPosition - NPC.Center).SafeNormalize(Vector2.Zero);
                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            _afterImageAlpha = 1f;
            ShakeModSystem.Shake = 2;

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            if (Timer % 2 == 0)
            {

                SpawnFlameDonut();
                SpawnFlameDust();
            }

            //We need to zoom really quickly to the target position
            //Not sure how to do that tbh
            float dashTime = 25f;
            float completionRatio = Timer / dashTime;
            float dashSpeed = MathHelper.Lerp(25f, 65, completionRatio);
            NPC.velocity = _simpleDashNormal * dashSpeed;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.5f);

            if (Timer == (int)(dashTime - 5))
            {

                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DescendingBigBoom>(),
                        DescendingBigBoomDamage, 1, Main.myPlayer, ai1: (int)Variant);
                }
            }
            //Enable the contact damage as per usual
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.HIghSpeedCrashEnd);
            }
        }

        private void AI_HighSpeedCrashEnd()
        {
            Timer++;
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //Step 2. Check the distance between this current twin and the player
            //If the distance is too far we'll move closer to them, if not we just slow down/sit there
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float maxDistance = 400;
            if (distanceToTarget > maxDistance)
            {
                //We should scale the movement velocity based on the distance, so the farther they are the faster we'll move
                Vector2 movementVelocity = targetNormal * distanceToTarget / 32f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.1f);
            }
            else
            {
                //Otherwise, we'll just slow down
                //We want to keep a little bit of movement velocity so it's not just completely static
                NPC.velocity *= 0.8f;

                //Stpe 3. Add a little bit of hovering velocity for a cool effect
                float yHover = MathF.Sin(Timer * 0.1f) * 0.5f;
                NPC.velocity.Y += yHover;
            }

            if (Timer >= 42)
            {
                AttackNumber++;
                if (AttackNumber < 6)
                {
                    SwitchState(TwinAIState.HighSpeedCrashQuickStart);
                }
                else
                {
                    SwitchState(TwinAIState.Idle);
                }

            }
        }

        #endregion

        #region Flame Sword
        private Vector2 GetFlameSwordStartOffset()
        {
            float distanceOffset = 300f;
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return -Vector2.UnitX * distanceOffset;
                case TwinVariant.Retina:
                    return Vector2.UnitX * distanceOffset;
            }
        }
        private void SpawnSteamParticle()
        {
            Vector2 spawnPosition = NPC.Top;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle.NewParticle<BlackSmokeParticle>(spawnPosition, spawnVelocity, Scale: spawnScale);
            steamParticle.innerColor = Color.DarkGray;
            steamParticle.outerColor = Color.Black;
            steamParticle.fadeToColor = Color.Black;
        }
        private void AI_FlameSwordStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                _simpleDashNormal = NPC.velocity;
            }

            /*
             * 
             * Both of them aim above you, shooting a type of fire (Descender Retina), shoots a red fire,
             * while Descender Spazz, shoots a green flame, and they make a crossing sword, and continuously going downwards, making you dodge
             */

            //So first we need to get them ina  good position for doing this attack
            //I think it'd be best if they position themselves on opposite sides of you
            //Alright so
            //First let's get that position and move to it
            Vector2 flameSwordOffset = GetFlameSwordStartOffset();
            Vector2 positionToMoveTo = Target.Center + flameSwordOffset;


            float windupTime = 80f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, movementVelocity, completionRatio);

            //Look at the player
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.FlameSwordWindup);
            }

        }

        private void AI_FlameSwordAim()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle beep = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beep.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beep, NPC.position);

                _simpleDashNormal = TargetNormal;
            }

            NPC.velocity.Y -= 1;
            NPC.velocity *= 0.9f;

            //We need to look up at a 30 degree angle, shoot, and then move downward
            //Alright
            float windupTime = 30f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.Anticipation(completionRatio);
            float directionToRotate = _simpleDashNormal.X > 0 ? 1f : -1f;
            float radiansOffset = MathHelper.Lerp(0f, -MathHelper.PiOver4 / 2f * directionToRotate, ease);

            //That new direction that we are facing
            Vector2 newNormal = _simpleDashNormal.RotatedBy(radiansOffset);
            NPC.rotation = newNormal.ToRotation();
            TargetOutlineColor = Color.Yellow;

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = NPC.rotation;
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.FlameSwordContinuous);
            }
        }
        private void AI_FlameSwordContinuous()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle flamethrower = AssetRegistry.Sounds.SteamPunking.DescendingFlamethrower;
                flamethrower.PitchVariance = 0.3f;
                SoundEngine.PlaySound(flamethrower, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 800;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<DescendingFlameSword>(), FlameSwordDamage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: (int)Variant);
                }
                SpawnFlameDonut();
            }


            //Move downward whiel shooting
            float continuosTime = 100f;
            float completionRatio = Timer / continuosTime;
            float ease = EasingFunction.Anticipation2(completionRatio / 0.5f);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 10f, ease);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            TargetOutlineColor = Color.Yellow;
            ShakeModSystem.Shake = 4;
            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
                SpawnSteamParticle();
            }

            if (Timer >= continuosTime)
            {
                SwitchState(TwinAIState.FlameSwordEnd);
            }
        }

        private void AI_FlameSwordEnd()
        {
            float endTime = 15f;
            Timer++;

            NPC.velocity *= 0.9f;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= endTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
        #endregion
        private void AI_SpawnRetina()
        {
            Variant = TwinVariant.Retina;
            _parentIndex = (int)NPC.ai[2];
            NPC.ai[2] = (float)TwinAIState.Idle;
            SwitchState(TwinAIState.Idle);
        }

        private void AI_SpawnSpazz()
        {
            Variant = TwinVariant.Spazz;
            _parentIndex = (int)NPC.ai[2];
            NPC.ai[2] = (float)TwinAIState.Idle;
            SwitchState(TwinAIState.Idle);
        }

        private void IdleMovement()
        {

            //So we should slowly move towards the player if they're far, if not we'll just hover in place.
            //Step 1. Look towards the player, we can do this by calculating a target normal, calculating an angle and then lerping to it
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //Step 2. Check the distance between this current twin and the player
            //If the distance is too far we'll move closer to them, if not we just slow down/sit there
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float maxDistance = 400;
            if (distanceToTarget > maxDistance)
            {
                //We should scale the movement velocity based on the distance, so the farther they are the faster we'll move
                Vector2 movementVelocity = targetNormal * distanceToTarget / 32f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.05f);
            }
            else
            {
                //Otherwise, we'll just slow down
                //We want to keep a little bit of movement velocity so it's not just completely static
                NPC.velocity *= 0.8f;

                //Stpe 3. Add a little bit of hovering velocity for a cool effect
                float yHover = MathF.Sin(Timer * 0.1f) * 0.5f;
                NPC.velocity.Y += yHover;
            }
        }

        private void AI_Idle()
        {
            _rotationTimer = 0f;

            //Ok, so in the idle state, the goober is basically waiting on a command from the commander
            //So it should just slowly wander around and target the player
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }


            //Reset draw variables
            _scale = Vector2.One;
            _afterImageAlpha = 0f;
            IdleMovement();

            //Remember, we're just waiting on a command from up above, so we don't actually need to do anything else here
            //However, we will create a few steam particles just for funsies
            if (Timer % 10 == 0)
            {
                Particle.NewParticle<BlackSmokeParticle>(
                    NPC.Center + Main.rand.NextVector2Circular(64, 64),
                    -Vector2.UnitY * Main.rand.NextFloat(0.2f, 0.5f), newColor: Color.White);
            }

            TargetOutlineColor = Color.Transparent;
            AttackNumber = 0f;

            //Receive the next command state.
            //This should be automatically netcoded btw
            if (NextCommandState != TwinAIState.Idle)
            {
                SwitchState(NextCommandState);
                NextCommandState = TwinAIState.Idle;
            }
        }

        #region Simple Dash
        //Both dash at you multiple times, crossing each other in the middle, making like a swirl dance
        //Alright, this attack is kinda like that one silksong attack from the cogwork dancers
        //We're going to need to make some really cool movement and visuals for this
        //We'll split this into two attacks
        private Vector2 _simpleDashNormal;
        private void AI_SimpleDashStart()
        {
            //The first attack is a basic dash where the eye looks at you
            //A telegraph line appears, and after a bit of anticipation, they go backward and then forward and do a quick dash
            //Alright so
            //Step 1. target a player, look at them
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                _simpleDashNormal = TargetNormal;
            }


            float targetAngle = _simpleDashNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //2. Calculate anticipation
            float windUpTime = 20f;
            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 movementNormal = Vector2.Lerp(-_simpleDashNormal * 0.5f, _simpleDashNormal, ease);
            Vector2 anticipationVelocity = movementNormal * 10f;
            NPC.velocity = anticipationVelocity;

            //3. Draw the telegraph line
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio / 0.5f);
            _telegraphLineRot = _simpleDashNormal.ToRotation();

            TargetOutlineColor = Color.Yellow;

            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.SimpleDash);
            }
        }

        private int GetDustType()
        {
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return DustID.CursedTorch;
                case TwinVariant.Retina:
                    return DustID.RedTorch;
            }
        }
        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return Color.Green;
                case TwinVariant.Retina:
                    return Color.Red;
            }
        }
        private void SpawnFlameDust()
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, GetDustType(), Scale: Main.rand.NextFloat(1f, 2f));
            var p = Particle.NewParticle<GlowFragmentParticle>(NPC.Center, Vector2.Zero, Color.White);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }

        private void SpawnFlameDonut()
        {
            //movement donut particles
            var donut = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.White);
            Color twinColor = GetTwinColor();
            donut.innerColor = twinColor;
            donut.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            donut.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }
        private void AI_SimpleDash()
        {
            Timer++;
            if (Timer == 1)
            {
                AttackNumber++;

                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDonut();
            }

            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }
            //Fade out the dash line and just move in the direction that we were moving
            //We can just multiply the velocity
            float dashTime = 20f;
            float completionRatio = Timer / dashTime;

            float dashSpeed = 35f;
            if (NPC.velocity.Length() < dashSpeed)
            {
                NPC.velocity *= 1.5f;
            }

            NPC.rotation = NPC.velocity.ToRotation();

            //Fade out the dash line
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, completionRatio);
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(completionRatio));

            //Stretch the sprite a little bit to give a bit of a motion blurring effect
            _scale = Vector2.Lerp(new Vector2(1.5f, 1f), Vector2.One, completionRatio);

            //Set contact damage to be true
            //Make sure we telegraph this properly with red outlines.
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.SimpleDashEnd);
            }
        }

        private void AI_SimpleDashEnd()
        {
            Timer++;

            //Simply just slow down
            TargetOutlineColor = Color.Transparent;
            float endDashTime = 15f;
            NPC.velocity = NPC.velocity.RotatedBy(-0.05f);
            NPC.velocity *= 0.95f;
            NPC.rotation = NPC.velocity.ToRotation();
            if (Timer >= endDashTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
        #endregion


        #region Dash Dance
        private void AI_DashDanceStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle circlePrepareSound = AssetRegistry.Sounds.SteamPunking.DescendingCircle;
                circlePrepareSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(circlePrepareSound, NPC.position);
                _simpleDashNormal = NPC.velocity;
            }

            //So how do we want this attack to look?
            //I think the twins should orbit around a circle for a bit, on opposite points
            //Then after a while, they look towards you and dash to the point, when they touch each other
            //They'll burst into the dash
            //Alright so

            //First we need to create a circle around our target
            float windUpTime = 80f;
            float circleRadius = 300f;
            Vector2 initialDirection = -Vector2.UnitY;
            Vector2 dashVector = initialDirection * circleRadius;

            //Get an offset based on the variant that this goober is
            float radiansOffset = Variant == TwinVariant.Spazz ? MathHelper.Pi : 0;
            radiansOffset -= MathHelper.PiOver2;

            //get a ratio of how far we are into this prepation state
            float completionRatio = Timer / windUpTime;
            float rads = (MathHelper.TwoPi * 2);
            float radiansToRotateBy = MathHelper.Lerp(0f, rads, completionRatio);
            Vector2 rotatedVector = dashVector.RotatedBy(radiansToRotateBy + radiansOffset);
            Vector2 positionToMoveTo = Target.Center + rotatedVector;
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;

            float inLerp = EasingFunction.InOutSine(completionRatio / 0.5f);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, targetVelocity, completionRatio);

            //We also need to rotate towards the target, we are facing them after all!
            Vector2 targetNormal = TargetNormal;
            float targetAngle = TargetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, completionRatio);

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = targetAngle;
            TargetOutlineColor = Color.Yellow;
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.DashDancePrepare);
            }
        }

        private void AI_DashDancePrepare()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = NPC.rotation.ToRotationVector2();
                SoundStyle windupPrepareSound = AssetRegistry.Sounds.SteamPunking.DescendingWindup;
                windupPrepareSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(windupPrepareSound, NPC.position);
            }
            _telegraphLineAlpha *= 0.5f;
            //Make sure there's a bit of preparation time
            float prepareTime = 30f;
            float completionRatio = Timer / prepareTime;
            float anticipationEase = EasingFunction.Anticipation2(completionRatio);
            Vector2 anticipationVelocity = Vector2.Lerp(-_simpleDashNormal * 5f, _simpleDashNormal * 5f, anticipationEase);
            NPC.velocity = anticipationVelocity;

            //So we build up some anticipation before the dash happens
            //And also fade out the dash line
            TargetOutlineColor = Color.Yellow;
            if (Timer >= prepareTime)
            {
                SwitchState(TwinAIState.DashDance);
            }
        }

        private void AI_DashDance()
        {
            Timer++;
            float dashTime = 15f;

            //Speed up the dash speed
            float dashSpeed = 30f;
            if (NPC.velocity.Length() < dashSpeed)
            {
                NPC.velocity *= 1.5f;
            }


            if (Timer % 5 == 0)
            {
                SpawnFlameDonut();
            }

            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }

            //Create a cool little effect for have motion blurring
            float completionRatio = Timer / dashTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _scale = Vector2.Lerp(new Vector2(1.5f, 1f), Vector2.One, ease);

            //Add an after image
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, completionRatio / 0.5f);

            //Enable the contact damage
            _contactDamage = true;
            TargetOutlineColor = Color.Red;

            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.DashDanceTwirl);
            }
        }

        private void AI_DashDanceTwirl()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle twirlSound = AssetRegistry.Sounds.SteamPunking.DescendingTwirl;
                twirlSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(twirlSound, NPC.position);
            }
            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }

            //In this state, the twins rotate their velocity and sin a bit upwards
            //Alright so
            float twirlTime = 30f;
            float radiansToRotateVelocityBy = (MathHelper.TwoPi + MathHelper.Pi) / twirlTime;

            //We need to calculate the direction to rotate by, whether clockwise or counter clockwise
            //This is based on the way the twin
            float direction = Variant == TwinVariant.Spazz ? -1f : 1f;
            radiansToRotateVelocityBy *= direction;

            NPC.velocity = NPC.velocity.RotatedBy(-radiansToRotateVelocityBy);
            NPC.rotation = NPC.velocity.ToRotation();

            //By this point we already smoothed into this, so we can just set the draw variables
            _scale = Vector2.One;
            _afterImageAlpha = 1f;
            if (Timer >= twirlTime)
            {
                SwitchState(TwinAIState.DashDanceEnd);
            }

            //Enable contact damage
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
        }

        private void AI_DashDanceEnd()
        {
            Timer++;
            float endTime = 45f;
            NPC.velocity *= 0.9f;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, TargetNormal.ToRotation(), 0.1f);

            //Fade out the after image
            float completionRatio = Timer / endTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _afterImageAlpha = MathHelper.Lerp(1f, 0f, ease);
            if (Timer >= endTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
        #endregion


        //telegraph line
        #region Draw Code
        private float _telegraphLineAlpha;
        private float _telegraphLineRot;


        private float _afterImageAlpha;
        private Vector2 _scale;

        private Color _outlineColor;
        private Color TargetOutlineColor;
        private void UpdateDraw()
        {
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
        }

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(bloomLineTexture.Width / 2f, 0f);
            Vector2 drawScale = Vector2.One;
            drawScale.Y *= 2f;
            drawScale.X *= 0.5f;

            Color telegraphLineColor = Variant == TwinVariant.Spazz ? Color.Green : Color.Red;
            telegraphLineColor.A = 0;
            telegraphLineColor *= _telegraphLineAlpha;
            spriteBatch.Draw(bloomLineTexture, NPC.Center - screenPos, null, telegraphLineColor, _telegraphLineRot - MathHelper.PiOver2, drawOrigin, drawScale, SpriteEffects.None, 0);
        }


        private Texture2D GetTwinTexture()
        {
            if (Variant == TwinVariant.Spazz)
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture + "_Spazz").Value;
                return twinTexture;
            }
            else
            {
                Texture2D twinTexture = ModContent.Request<Texture2D>(Texture).Value;
                return twinTexture;
            }
        }

        private Color GetFlamingTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _afterImageAlpha;
        }

        private float GetFlamingTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(222, 222, completionRatio);
        }


        private void DrawFlamingTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = BlackFireShader.Instance;
            shader.Time = Main.GlobalTimeWrappedHourly * 16;
            shader.InnerColor = Variant == TwinVariant.Spazz ? Color.Green : Color.Red;
            shader.OuterColor = Variant == TwinVariant.Spazz ? Color.DarkGreen : Color.DarkRed;
            TrailDrawer.Draw(spriteBatch, NPC.oldPos, GetFlamingTrailColor, GetFlamingTrailWidth, shader, offset: NPC.Size / 2f);
        }
        private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D twinTexture = GetTwinTexture();
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float trailLength = NPC.oldPos.Length;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawCenter = NPC.oldPos[i] + NPC.Size / 2f - screenPos;
                float f = i;
                float completionRatio = f / trailLength;

                //After image
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= _afterImageAlpha;

                drawColor *= 0.5f;
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipVertically;
                }
                spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.oldRot[i], drawOrigin, _scale, spriteEffects, 0f);
            }
        }


        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D twinTexture = GetTwinTexture();
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            spriteBatch.Draw(twinTexture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawAfterImages(spriteBatch, screenPos);
            DrawFlamingTrail(spriteBatch, screenPos, drawColor);
            DrawTelegraphLine(spriteBatch, screenPos);
            DrawSprite(spriteBatch, screenPos, drawColor);

            //This is just to create a nice little glowy effect
            drawColor *= ExtraMath.Osc(0f, 0.5f, speed: 3f);
            drawColor.A = 0;
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            DrawSprite(spriteBatch, screenPos + Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitX * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos + Vector2.UnitY * outlineOffset, _outlineColor);
            DrawSprite(spriteBatch, screenPos - Vector2.UnitY * outlineOffset, _outlineColor);
        }
        #endregion
    }
}
