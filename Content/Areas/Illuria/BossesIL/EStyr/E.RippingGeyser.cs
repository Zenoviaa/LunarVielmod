using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private ref float Timer => ref Projectile.ai[0];
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
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            if (Projectile.velocity.Length() < 20)
            {
                Projectile.velocity *= 1.065f;
            }

            if (Timer < 60)
            {
                Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
                if (player != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 1);
                }
            }

        }

        private void DrawSprite(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale;
            Vector2 drawScale = Vector2.One;
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
                drawColor *= 0.6f;
                Vector2 drawScale = Vector2.One;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawAfterImages(spriteBatch);
            DrawSprite(spriteBatch);
        }
    }
    public class GeyserBlast : ModProjectile
    {
        private float _inScale;
        private float _outScale;
        private Vector2[] LinePos = new Vector2[2];
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
            LinePos[1] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 8000;
            if (Timer > 60 && Timer < 120 && this.OwnedByLocalClient())
            {
                if (Timer % 10 == 0)
                {
                    Vector2 spawnCenter = Projectile.Center;
                    spawnCenter.Y += Main.rand.NextFloat(0f, 1590);
                    spawnCenter.X += Main.rand.NextFloat(-500, 500);
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
            TexturedQuad.DrawWithShader(flamingTrailShader);
            TexturedQuad.DrawWithShader(flamingTrailShader);
            return false;
        }
    }

    public class RippingGeyser : ModProjectile
    {
        private float _inFlash;
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

            if (Timer < 30)
            {
                Projectile.Center = Parent.Center;
            }

            if (Points == null)
                return;

            for (int i = 0; i < Points.Length; i++)
            {
                float completionRatio = (float)i / (float)Points.Length;
                float ease = EasingFunction.InOutSine(completionRatio);
                Vector2 start = InitialPosition;
                Vector2 end = Projectile.Center;
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

        private void AI_LaserBlast()
        {
            Timer++;
            if (Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    Vector2 midPosition = InitialPosition + Projectile.Center;
                    midPosition /= 2f;

                    Vector2 velocity = Vector2.UnitY;

                    int projType = ModContent.ProjectileType<GeyserBlast>();
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), midPosition, velocity, projType,
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }


            for (int i = 0; i < 2; i++)
            {
                Smear2();
            }
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
            float x = Main.rand.NextFloat(InitialPosition.X, Projectile.Center.X);
            Vector2 spawnPosition = new Vector2();
            spawnPosition.X = x;
            spawnPosition.Y = InitialPosition.Y;

            float length = 1000;
            float strength = 0.1f;
            ScreenSmearEffectManager.NewParticle(spawnPosition, Vector2.UnitY, length, 15, strength);
        }

        private void Smear()
        {
            float x = Main.rand.NextFloat(InitialPosition.X, Projectile.Center.X);
            Vector2 spawnPosition = new Vector2();
            spawnPosition.X = x;
            spawnPosition.Y = InitialPosition.Y;

            float length = MathHelper.SmoothStep(1000, 0, Timer / 120f);
            float strength = 0.3f;
            ScreenSmearEffectManager.NewParticle(spawnPosition, Vector2.UnitY, length, 15, strength);
        }

        private float GetTrailWidth(float completionRatio)
        {
            float w = MathHelper.Lerp(0f, 10, EasingFunction.QuadraticBump(completionRatio));
            w = MathHelper.Lerp(w, 5f, _inFlash);
            return w;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Black, _inFlash);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (Points == null)
                return false;
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            _inFlash = 0f;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);

            _inFlash = 1f;
            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);
            return false;
        }
    }
    public partial class E
    {
        private int RippingGeyserDaamge => 30;
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


            //Position yourself on the right or left of the target player
            float startupTime = 100;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 targetStartPosition = MyTarget.Center + _forwardVector * 700;
            targetStartPosition.Y -= 600;
            Vector2 targetVelocity = (targetStartPosition - NPC.Center);
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            NPC.direction = TargetDirection;

            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -300);
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
                TargetVector = NPC.velocity;
                _forwardVector = Vector2.UnitX * (MyTarget.Center.X > NPC.Center.X ? 1 : -1);
                NPC.direction = TargetDirection;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<RippingGeyser>(), RippingGeyserDaamge, 1, Main.myPlayer, ai1: NPC.whoAmI);
                }


                var strike = Particle.NewParticle<GlowDonutParticle>(NPC.Center, _forwardVector);
                strike.xMult = 6;
                strike.rotOffset += MathHelper.PiOver2;
            }

            //Dash forward, we need to create a projectile here
            float dashTime = 30;
            float completionRatio = Timer / dashTime;
            float ease = EasingFunction.InOutExpo(completionRatio);
            float ease2 = EasingFunction.QuadraticBump(completionRatio);

            Vector2 dashSpeed = _forwardVector * 140;
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, dashSpeed, ease);
            Vector2 lerp2 = Vector2.Lerp(TargetVector, interpolatedVelocity, ease2);
            NPC.velocity = lerp2;
            NPC.velocity.Y = 0;

            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -300);
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
            TargetOutlineColor = Color.Transparent;
            float auraTime = 300;
            if (Timer >= auraTime)
            {
                SwitchState(AIState.RippingGeyser_End);
            }
        }



        private void AI_RippingGeyserEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
