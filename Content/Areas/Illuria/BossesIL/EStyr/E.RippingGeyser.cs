using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class GeyserStar : ScarletProjectile,
        IDrawBlackStar
    {
        private float _telegraphRotation;
        private float _telegraphAlpha;
        private ref float Timer => ref Projectile.ai[0];
        private bool IsSmall => Projectile.ai[1] == 1;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 1200, 
                        ModContent.ProjectileType<BlackSplash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
              //  strike.rotOffset += MathHelper.PiOver2;
            }
            if(Timer == 60)
            {
                SoundStyle starSound = new SoundStyle("Stellamod/Assets/Sounds/Starrer");
                starSound.PitchVariance = 0.5f;
                SoundEngine.PlaySound(starSound, Projectile.position);
            }
            if (Timer > 60)
            {
                if (Projectile.velocity.Length() < 10)
                {
                    Projectile.velocity *= 1.065f;
                }
            }

            _telegraphRotation = Projectile.velocity.ToRotation();
            _telegraphAlpha = EasingFunction.QuadraticBump(Timer / 120f);

            //Only home towards the player slightly
            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = MathHelper.Lerp(0.5f, 1f, Projectile.velocity.Length() / 10f) * outScale;
            Projectile.scale += ExtraMath.Osc(-0.1f, 0.1f, offset: Projectile.whoAmI);
            if (IsSmall)
                Projectile.scale *= 0.5f;
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
            if (player != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 0.125f);
            }
            Projectile.rotation += 0.01f;
            Projectile.rotation += Projectile.velocity.ToRotation() * 0.02f;
        }

        private void DrawSprite(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale;
            Vector2 drawScale = Vector2.One * scale;
            spriteBatch.Draw(texture, drawCenter, null, Color.White, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private void DrawAfterImages(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                float completionRatio = (float)i / (float)TrailCacheLength;

                Vector2 drawCenter = OldCenterPos[i] - Main.screenPosition;
                float rotation = OldCenterRot[i];
                float scale = Projectile.scale;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= 0.15f;
                Vector2 drawScale = Vector2.One * scale;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        private void DrawTelegraphLine(SpriteBatch spriteBatch)
        {
            Texture2D bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(bloomLine.Width / 2f, 0f);
            float rotation = _telegraphRotation - MathHelper.PiOver2;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= _telegraphAlpha;
            drawColor *= 0.5f;
            Vector2 scale = Vector2.One;
            scale.X *= 0.3f;
            scale.Y *= 2 * EasingFunction.QuadraticBump(Timer / 120f);
            spriteBatch.Draw(bloomLine, drawCenter, null, drawColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
         //   DrawTelegraphLine(Main.spriteBatch);
            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {

            DrawAfterImages(spriteBatch);
            DrawSprite(spriteBatch);
        }
    }


    public class BlackSplash : ModProjectile,
    IDrawBlackStar
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float Multiplier => ref Projectile.ai[1];
        private Vector2[] SplashPoints = new Vector2[32];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
        }

        private float GetTrailWidth(float completionRatio)
        {
            return 1500;
        }

        private Color GetTrailColor(float completionRatio)
        {
            float ease = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / 30f));
            return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(completionRatio)) * ease;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D blacksplashTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(0, blacksplashTexture.Height / 2f);
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / 30f));
            Vector2 drawScale = Vector2.One;
            drawScale.X *= 6;
            if (Multiplier != 0)
                drawScale.X *= Multiplier;
            drawScale.Y = 0.5f;
            spriteBatch.Draw(blacksplashTexture, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, drawScale, SpriteEffects.None, 0);
            /*
            var laserShader = BasicLaserAlphaShader.Instance;
            laserShader.LaserTexture = TrailRegistry.SimpleTrail;
            TrailDrawer.Draw(Main.spriteBatch, SplashPoints, GetTrailColor, GetTrailWidth, laserShader);
            TrailDrawer.Draw(Main.spriteBatch, SplashPoints, GetTrailColor, GetTrailWidth, laserShader);*/
            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            /*
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.Black;
            flamingTrailShader.InnerColor = Color.White;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 2;
            flamingTrailShader.Tiling = new Vector2(1, 1);
            flamingTrailShader.BlendState = BlendState.Additive;
            flamingTrailShader.PrimaryTexture = TrailRegistry.LightningTrail2;
            flamingTrailShader.NoiseTexture = TrailRegistry.LightningTrail2;
            TrailDrawer.Draw(Main.spriteBatch, SplashPoints, GetTrailColor, GetTrailWidth, flamingTrailShader);*/
        }
    }


    public class GeyserBlast : ModProjectile,
        IDrawBlackStar
    {
        private float _inScale;
        private float _outScale;
        private Vector2[] LinePos = new Vector2[4];
        private TexturedQuad _quadBackingField;
        private TexturedQuad TexturedQuad
        {
            get
            {
                if (_quadBackingField == null)
                    _quadBackingField = new TexturedQuad();
                return _quadBackingField;
            }
        }
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 500;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(LinePos, projHitbox, targetHitbox, 1000);
        }
        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target) && Timer >= 30;
        }

        public override void AI()
        {
            base.AI();
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
            Timer++;
            if (Timer == 1)
            {
                SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER");
                shootSound.Pitch = -0.8f;
                SoundEngine.PlaySound(shootSound, Projectile.position);
            }
            _inScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(Timer / 60f));
            _outScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo((float)Projectile.timeLeft / 100));
            ShakeModSystem.Shake = MathHelper.Lerp(0, 9, _outScale);
            LinePos[0] = Projectile.Center;
            LinePos[1] = Projectile.Center;
            LinePos[2] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 8000;
            LinePos[3] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 8000;
            if (Timer > 60 && Timer < 120 && this.OwnedByLocalClient())
            {
                if (Timer % 10 == 0)
                {
                    Vector2 spawnCenterStart = Projectile.Center;
                    Vector2 spawnCenterEnd = spawnCenterStart + Projectile.velocity;
                    Vector2 spawnCenter = Vector2.Lerp(spawnCenterStart, spawnCenterEnd, Main.rand.NextFloat(0f, 1f));
                    Vector2 spawnVelocity = Vector2.UnitY * 5;

                    Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 16000);
                    if (player != null)
                    {
                        spawnVelocity = (player.Center - spawnCenter).SafeNormalize(Vector2.Zero) * 5;
                    }
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnCenter, spawnVelocity,
                        ModContent.ProjectileType<GeyserStar>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }
            }

            if(Timer > 120 && Timer % 10 == 0 && this.OwnedByLocalClient())
            {
                ScreenSmearEffectManager.NewParticle(Projectile.Center, Vector2.UnitY, 1000, 25);
                Vector2 spawnCenterStart = Projectile.Center;
                Vector2 spawnCenterEnd = spawnCenterStart + Projectile.velocity;
                Vector2 spawnCenter = Vector2.Lerp(spawnCenterStart, spawnCenterEnd, Main.rand.NextFloat(0f, 1f));

                Vector2 spawnVelocity = -Vector2.UnitY * 2;
                Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 16000);
                if (player != null)
                {
                    spawnVelocity = (player.Center - spawnCenter).SafeNormalize(Vector2.Zero) * 2;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnCenter, spawnVelocity,
                  ModContent.ProjectileType<GeyserStar>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai1: 1);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.Black;
            flamingTrailShader.InnerColor = Color.White;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = new Vector2(1, 3);
            flamingTrailShader.BlendState = BlendState.Additive;

            float smooth = _inScale * _outScale;
            float width = MathHelper.SmoothStep(0f, 1250, smooth);
            TexturedQuad.CalculateVertices(Projectile.Center, Projectile.velocity,
                8000, width);
           // TexturedQuad.DrawWithShader(flamingTrailShader);
            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.Black;
            flamingTrailShader.InnerColor = Color.White;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = new Vector2(1, 3);
            flamingTrailShader.BlendState = BlendState.Additive;

            float smooth = _inScale * _outScale;
            float width = MathHelper.SmoothStep(0f, 1250, smooth);
            TexturedQuad.CalculateVertices(Projectile.Center, Projectile.velocity,
                8000, width);
            TexturedQuad.DrawWithShader(flamingTrailShader);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
            BlackStars.AddBuff(target, 500);
        }
    }

    public class RippingGeyser : ModProjectile,
        IDrawBlackStar
    {
        private float _easeInTimer;
        private float _inFlash;
        private float _outScale = 1f;
        private enum AIState
        {
            Spawn,
            Gravity,
            LaserBlast
        }
        private Vector2[] Points = new Vector2[64];
        private Vector2 InitialPosition;
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }

        private AIState State
        {
            get => (AIState)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 450;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(InitialPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            InitialPosition = reader.ReadVector2();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle primRay = new SoundStyle("Stellamod/Assets/Sounds/PrimRay");
                primRay.Pitch = -0.7f;
                SoundEngine.PlaySound(primRay, Projectile.position);
                InitialPosition = Projectile.Center;
            }

            if (Points == null)
                return;

            for (int i = 0; i < Points.Length; i++)
            {
                float completionRatio = (float)i / (float)Points.Length;
                float ease = EasingFunction.InOutSine(completionRatio);
                Vector2 start = Projectile.Center;
                Vector2 end = Projectile.Center + Projectile.velocity;
                Vector2 interpolatedPoint = Vector2.Lerp(start, end, ease);
                Points[i] = interpolatedPoint;
            }

            if (Timer >= 60)
            {
                SwitchState(AIState.Gravity);
            }
        }

        private void AI_Gravity()
        {
            Timer++;
            float gravityTime = 120f;
            ShakeModSystem.Shake = MathHelper.Lerp(10, 0, Timer / gravityTime);
            for (int i = 0; i < 3; i++)
            {
                Smear();
            }
            if (Timer >= gravityTime)
            {
                SwitchState(AIState.LaserBlast);
            }


        }

        private float GetDirection()
        {
            if (Projectile.velocity.X > 0)
                return 1;
            return -1;
        }

        private void AI_LaserBlast()
        {
            Timer++;
            if (Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    Vector2 start = Projectile.Center;
                    Vector2 end = Projectile.Center + Projectile.velocity;
                    Vector2 midPosition = Vector2.Lerp(start, end, 0.5f);


                    Vector2 velocity = Projectile.velocity;
                    velocity = velocity.RotatedBy(MathHelper.PiOver2 * GetDirection());

                    int projType = ModContent.ProjectileType<GeyserBlast>();
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), midPosition, velocity, projType,
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }


            for (int i = 0; i < 2; i++)
            {
                Smear2();
            }
            _outScale = MathHelper.Lerp(1f, 0f, Timer / 240f);
            if (Timer >= 240)
            {
                Projectile.Kill();
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
        public override void AI()

        {
            base.AI();
            _easeInTimer++;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Gravity:
                    AI_Gravity();
                    break;
                case AIState.LaserBlast:
                    AI_LaserBlast();
                    break;
            }

        }
        private void Smear2()
        {
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity;
            Vector2 spawnPosition = Vector2.Lerp(start, end, Main.rand.NextFloat(0f, 1f));


            float length = 1200;
            float strength = 0.1f;

            Vector2 smearVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 * GetDirection());
            ScreenSmearEffectManager.NewParticle(spawnPosition, smearVelocity, length, 15, strength);
        }

        private void Smear()
        {
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity;
            Vector2 spawnPosition = Vector2.Lerp(start, end, Main.rand.NextFloat(0f, 1f));

            float length = MathHelper.SmoothStep(800, 1200, Timer / 120f);
            float strength = 0.3f;

            Vector2 smearVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 * GetDirection());
            ScreenSmearEffectManager.NewParticle(spawnPosition, smearVelocity, length, 15, strength);
        }

        private float GetTrailWidth(float completionRatio)
        {
            float w = MathHelper.Lerp(0f, 32, EasingFunction.QuadraticBump(completionRatio));
            w *= MathHelper.Lerp(2f, 1f, _inFlash);

            float inScale = EasingFunction.InOutSine(_easeInTimer / 30f);
            w *= inScale;
            w *= _outScale;
            return w;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Black, _inFlash);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            if (Points == null)
                return;
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.BeamTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 2;
            shader.Repeats = 1f;

            _inFlash = 0f;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);

            _inFlash = 1f;
            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);
        }
    }
    public partial class E
    {
        private int RippingGeyserDaamge => 100;
        private void AI_RippingGeysterStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
                _forwardVector = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
                _forwardVector.Y = 0;
            }

            Animator.PlayAnimation(Anim_Holding);

            //Position yourself on the right or left of the target player
            float startupTime = 100;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 targetStartPosition = MyTarget.Center + _forwardVector * 700;
            targetStartPosition.Y -= 300;
            Vector2 targetVelocity = (targetStartPosition - NPC.Center);
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            NPC.direction = TargetDirection;

            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -250);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= startupTime)
            {
                SwitchState(AIState.RippingGeyser_Dash);
            }
        }

        private void AI_RippingGeyserDash()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.Center;
                _forwardVector = Vector2.UnitX * (MyTarget.Center.X > NPC.Center.X ? 1 : -1);
                NPC.direction = TargetDirection;


                float distance = 1000f;
                Vector2 cutVelocity = new Vector2(NPC.direction * distance, -distance / 2f);
                if (MultiplayerHelper.IsHost)
                { 
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, cutVelocity,
                        ModContent.ProjectileType<RippingGeyser>(), RippingGeyserDaamge, 1, Main.myPlayer);
                }

                Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                Vector2 startPosition = NPC.Center;
                ScreenSmearEffectManager.NewParticle(startPosition, cutVelocity, 2400, 45);

                for (float i = 0; i < 3; i++)
                {
                    var donutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center + cutVelocity * 0.5f, cutVelocity.SafeNormalize(Vector2.Zero));
                    donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);
                    donutParticle.rotOffset += MathHelper.PiOver2;
                    donutParticle.xMult = 12;

                }

                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurriSlash, NPC.position);
                ShakeModSystem.Shake = 24;
            }

            Animator.PlayAnimation(Anim_BigSlash);

            //Dash forward, we need to create a projectile here
            float dashTime = 35f;
            float completionRatio = Timer / dashTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 startCenter = TargetVector;
            Vector2 endCenter = startCenter - _forwardVector * 512;
            Vector2 positionToMoveTo = Vector2.Lerp(startCenter, endCenter, ease);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            NPC.velocity = targetVelocity;


            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -100);
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= dashTime)
            {
                SwitchState(AIState.RippingGeyser_AuraFarm);
            }
        }

        private void AI_RippingGeyserAuraFarm()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            Animator.PlayAnimation(Anim_BattleIdle);

            //Bounce back to the player
            float inTime = 120;
            float inCompletionRatio = Timer / inTime;
            float ease = EasingFunction.InOutSine(inCompletionRatio);
            Vector2 positionToMoveTo = MyTarget.Center + new Vector2(0, -256);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;

            //The geyser has been created so don't really do anything
            //We can have bro just lerp back to you maybe?
            float auraTime = 500;
            if (Timer >= auraTime)
            {
                SwitchState(AIState.RippingGeyser_End);
            }
        }



        private void AI_RippingGeyserEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
