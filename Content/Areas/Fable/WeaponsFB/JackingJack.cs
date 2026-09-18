using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class JackingJack : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToBellMinion(ModContent.ProjectileType<JackingJackSummon>(), isGuardian: true);
            Item.damage = 12;
        }
    }

    public class JackingJackSummon : AbstractBellSummon
    {
        public override string Texture => ModContent.GetInstance<JackTheScholar>().Texture;
        private enum AIState
        {
            //Movement States
            Idle,
            HopAround,
            RocketFlight,

            //Attack States
            Fireball_Start,
            Fireball,
            Fireball_End,


            FireWisp_Start,
            FireWisp,
            FireWisp_End,

            FirePulse_Start,
            FirePulse,
            FirePulse_End,
        }

        private enum AnimationState
        {
            Idle,
            Cast_Hand_Up,
            Cast_Hold_Out,
            Cast_Put_Down,
            Summon_Hand_Up,
            Summon_Hold_Out,
            Summon_Hand_Down
        }

        private float _frameTimer;
        private AnimationState _animation;

        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private PatternManager<AIState> _attackCycleBackingField;
        private PatternManager<AIState> AttackCycle
        {
            get
            {
                if(_attackCycleBackingField == null)
                {
                    _attackCycleBackingField = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.FirePulse_Start, 1.0f ),
                        new Tuple<AIState, float>(AIState.FireWisp_Start, 1.0f),
                        new Tuple<AIState, float>(AIState.Fireball_Start, 1.0f));
                }
                return _attackCycleBackingField;
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 28;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            base.AI();

            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.HopAround:
                    AI_HopAround();
                    break;
                case AIState.RocketFlight:
                    AI_RocketFlight();
                    break;

                case AIState.Fireball_Start:
                    AI_FireballStart();
                    break;
                case AIState.Fireball:
                    AI_Fireball();
                    break;
                case AIState.Fireball_End:
                    AI_FireballEnd();
                    break;


                case AIState.FireWisp_Start:
                    AI_FireWispStart();
                    break;
                case AIState.FireWisp:
                    AI_FireWisp();
                    break;
                case AIState.FireWisp_End:
                    AI_FireWispEnd();
                    break;

                case AIState.FirePulse_Start:
                    AI_FirePulseStart();
                    break;
                case AIState.FirePulse:
                    AI_FirePulse();
                    break;
                case AIState.FirePulse_End:
                    AI_FirePulseEnd();
                    break;
            }
            //Gravity
            Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Animate();
        }

        private void Animate()
        {
            _frameTimer += 0.2f;
            if (_frameTimer >= 1f)
            {
                _frameTimer = 0;
                Projectile.frame++;
            }

            switch (_animation)
            {
                default:
                case AnimationState.Idle:
                    if (Projectile.frame >= 4f)
                    {
                        Projectile.frame = 0;
                    }
                    break;
                case AnimationState.Cast_Hand_Up:
                    if (Projectile.frame >= 8f)
                    {
                        Projectile.frame = 7;
                    }
                    break;
                case AnimationState.Cast_Hold_Out:
                    if (Projectile.frame >= 12)
                    {
                        Projectile.frame = 8;
                    }
                    break;
                case AnimationState.Cast_Put_Down:
                    if (Projectile.frame >= 16)
                    {
                        Projectile.frame = 15;
                    }
                    break;
                case AnimationState.Summon_Hand_Up:
                    if (Projectile.frame >= 20)
                    {
                        Projectile.frame = 19;
                    }
                    break;
                case AnimationState.Summon_Hold_Out:
                    if (Projectile.frame >= 24)
                    {
                        Projectile.frame = 20;
                    }
                    break;
                case AnimationState.Summon_Hand_Down:
                    if (Projectile.frame >= 28)
                    {
                        Projectile.frame = 0;
                        Projectile.frame = 27;
                    }
                    break;
            }
       
        }

        private void AI_Idle()
        {
            Timer++;
            _animation = AnimationState.Idle;
            Projectile.tileCollide = true;
            Projectile.spriteDirection = Owner.Center.X < Projectile.Center.X ? 1 : -1;
            Projectile.velocity.X *= 0.9f;
            NPC target = NPCHelper.FindClosestNPC(Projectile.position, 1024);
            if (target != null)
            {
                Projectile.spriteDirection = target.Center.X < Projectile.Center.X ? 1 : -1;
            }
            if(Timer >= 60 && target != null)
            {
                if (this.OwnedByLocalClient())
                {
                    AIState nextAttack = AttackCycle.NextPattern();
                    SwitchState(nextAttack);
                }

            }

            float distanceToOwner = Vector2.Distance(Projectile.Center, Owner.Center);
            if(distanceToOwner > 300)
            {
                SwitchState(AIState.RocketFlight);
            }
        }
        private void AI_HopAround()
        {
         
            _animation = AnimationState.Idle;
            Timer++;
            if(Timer == 1)
            {
                float xDirectionToOwner = MathF.Sign(Owner.Center.X - Projectile.Center.X);

                Vector2 jumpVelocity = new Vector2(xDirectionToOwner * 2, -11);
                Projectile.velocity = jumpVelocity;
            }
            Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;

            if(Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_RocketFlight()
        {
            _animation = AnimationState.Summon_Hold_Out;
            Timer++;
            float distanceToOwner = Vector2.Distance(Projectile.Center, Owner.Center);
            if(distanceToOwner <= 16)
            {
                Projectile.velocity.Y -= 5;
                SwitchState(AIState.Idle);
            }
            else
            {
                Vector2 velocityToOwner = (Owner.Center - Projectile.Center);
                velocityToOwner = velocityToOwner.SafeNormalize(Vector2.Zero);
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocityToOwner * 8, 0.1f);
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.1f);
            }
        }

        private void AI_FireballStart()
        {
            _animation = AnimationState.Cast_Hand_Up;
            Timer++;
            Projectile.velocity.X *= 0.9f;
            if (Timer >= 15)
            {
                SwitchState(AIState.Fireball);
            }
        }

        private void AI_Fireball()
        {
            _animation = AnimationState.Cast_Hold_Out;
            Timer++;
            if(Timer % 30 == 0)
            {
                NPC target = NPCHelper.FindClosestNPC(Projectile.position, 1024);
                if(target != null)
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                    Vector2 velocity = target.Center - spawnPoint;
                    velocity = velocity.SafeNormalize(Vector2.Zero);
                    velocity *= 15;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint, velocity, 
                        ModContent.ProjectileType<JackingJackFireball>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            if (Timer >= 60)
            {
                SwitchState(AIState.Fireball_End);
            }
        }

        private void AI_FireballEnd()
        {
            _animation = AnimationState.Cast_Put_Down;
            Timer++;
            if (Timer >= 15)
            {
                SwitchState(AIState.HopAround);
            }
        }

        private void AI_FireWispStart()
        {
            _animation = AnimationState.Cast_Hand_Up;
            Timer++;
            Projectile.velocity.X *= 0.9f;
            if (Timer >= 15)
            {
                SwitchState(AIState.FireWisp);
            }
        }


        private void AI_FireWisp()
        {
            _animation = AnimationState.Cast_Hold_Out;
            Timer++;
            if (Timer % 15 == 0)
            {
                if (this.OwnedByLocalClient())
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint, -Vector2.UnitY * 8,
                        ModContent.ProjectileType<JackingJackFire>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Timer >= 60)
            {
                SwitchState(AIState.FireWisp_End);
            }
        }

        private void AI_FireWispEnd()
        {
            _animation = AnimationState.Cast_Put_Down;
            Timer++;
            if (Timer >= 15)
            {
                SwitchState(AIState.HopAround);
            }
        }

        private void AI_FirePulseStart()
        {
            _animation = AnimationState.Summon_Hand_Up;
            Timer++;
            Projectile.velocity.X *= 0.9f;
            if (Timer >= 15)
            {
                SwitchState(AIState.FirePulse);
            }
        }

        private void AI_FirePulse()
        {
            _animation = AnimationState.Summon_Hold_Out;
            Timer++;
            if (Timer == 30)
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                        ModContent.ProjectileType<JackingJackFirePulse>(), Projectile.damage, 2, Projectile.owner);
                }
            }
            if (Timer >= 60)
            {
                SwitchState(AIState.FirePulse_End);
            }
        }

        private void AI_FirePulseEnd()
        {
            _animation = AnimationState.Summon_Hand_Down;
            Timer++;
            if (Timer >= 15)
            {
                SwitchState(AIState.HopAround);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Timer >= 10 && State == AIState.HopAround)
            {
                SwitchState(AIState.Idle);
            }
            return false;
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
    }

    public class JackingJackFireball : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        break;
                }

            }

            if (Projectile.velocity.Length() > 2f)
                Projectile.velocity *= 0.99f;

            Projectile.scale = MathHelper.SmoothStep(0f, 1f, Timer / 15f);

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Projectile.rotation += 0.25f;

            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 800);
            if (nearest == null)
                return;
            if (Timer > 15)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 12);
                Projectile.velocity = homingVelocity;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sawTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = sawTexture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.Yellow;
            drawColor.A = 0;
            spriteBatch.Draw(sawTexture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.15f, SpriteEffects.None, 0);


            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.15f, SpriteEffects.None, 0);
            // spriteBatch.RestartDefaults();


            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.3f, SpriteEffects.None, 0);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color afo = glowColor;
                afo = Color.Lerp(afo, Color.Black, MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutExpo7(ratio)));
                afo.A = 0;
                afo *= 0.15f;
                spriteBatch.Draw(glowMask, oldCenter, null, afo, Projectile.oldRot[i], glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange = new Vector2(0.1f, 3f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire3, 120);
        }
        private void CreateImpactEffects()
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 0.5f).noGravity = true;
            }

            int numDust = 8;
            for (int n = 0; n < numDust; n++)
            {
                var sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY, Scale: Main.rand.NextFloat(1f, 2f));
                sp.initialColor = Color.Brown;
            }

            for (int n = 0; n < numDust; n++)
            {
                var dp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 45), Scale: Main.rand.NextFloat(1f, 2f));
            }


            ShakeScreenPosition.Shake = 3;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange = new Vector2(0.1f, 3f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            for (int n = 0; n < 3; n++)
            {
                SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY, Color.White, Scale: 1.5f);
                sp.initialColor = Color.White * 0.24f;
            }
            CreateImpactEffects();

        }
    }

    public class JackingJackFirePulse : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 196;
            Projectile.height = 196;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
           
        }
        

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, Projectile.position);

                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

                for (float f = 0; f < 4; f++)
                {
                    Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                for (float f = 0; f < 4; f++)
                {
                    var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    smoke.initialColor = Color.DarkGray;
                }


                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }
    }

    public class JackingJackFire : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];

        public override string Texture => ModContent.GetInstance<WillOWisp>().Texture;
        private float _scale;
        private Vector2 InitialVelocity;
        private Vector2 TargetVelocity;
        private NPC _target;
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
            Projectile.light = 0.278f;
            Projectile.timeLeft = 180;
        }


        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
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
            if (Timer < 30 && _target == null || _target != null && !_target.active)
            {
                _target = NPCHelper.FindClosestNPC(Projectile.position, 1024);
            }
            if (Timer < 30)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.25f, 1f), Easing.InCubic(Timer / 30f));
                Projectile.velocity *= 0.5f;
            }

            if (Timer == 30)
            {
                //Ping Sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if (Timer == 90)
            {
                if (_target != null && _target.active)
                {
                    TargetVelocity = Projectile.velocity = (_target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * InitialVelocity.Length();
                }
            }

            if (Timer > 90)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, TargetVelocity, 0.02f);
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.2f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightGoldenrodYellow * 0.1361f, Color.Transparent, completionRatio);
        }

        private void DrawPixelTrails(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Yellow;
            shader.InnerColor = Color.OrangeRed;
            shader.OuterColor = Color.Red;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelTrails);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);

            drawColor.A = 0;
            for (int i = 0; i < 4; i++)
            {
                float rot = i / 4f;
                Vector2 vel = rot.ToRotationVector2() * VectorHelper.Osc(0f, 4f, speed: 16);
                Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
                flameDrawPos -= Vector2.UnitY * 4;
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default, 1.5f);
                Dust dust = Main.dust[num];
                dust.noGravity = true;
                dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
            }
            var part = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Red);
            part.Scale *= 0.5f;
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
        }


        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            float drawScale = 1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 3; i++)
            {
                Color glowColor = new Color(85, 45, 15) * 0.5f;
                glowColor.A = 0;
                spriteBatch.Draw(dimLightTexture, Projectile.Center - Main.screenPosition, null, glowColor,
                    Projectile.rotation, dimLightTexture.Size() / 2f, drawScale * VectorHelper.Osc(0.75f, 1f, speed: 32, offset: Projectile.whoAmI), SpriteEffects.None, 0f);
            }

            //  Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 0.3f * Main.essScale);
        }
    }
}
