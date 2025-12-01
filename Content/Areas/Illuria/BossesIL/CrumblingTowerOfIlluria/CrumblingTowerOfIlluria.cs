using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander.Projectiles;
using Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
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

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria
{
    public struct TowerOfIlluraDraw
    {
        public Color outlineColor;
        public Vector2 towerDrawCenter;
        public float afterImageAlpha;
        public void SetDefaults()
        {
            outlineColor = Color.Transparent;
            afterImageAlpha = 1f;
        }
    }
    public class CrumblingTowerOfIlluria : ScarletBoss,
        IDrawOutlines
    {

        private float _enrageTimer;
        private int _heartCount;
        private float _shineTimer;
        private float _hoverTimer;
        private bool _inPhase2;
        private bool _showNamePlate;
        private bool _setTowerPosition;
        private bool _summonedHearts;
        private Vector2 _shakeOffset;
        private TowerOfIlluraDraw _draw;
        private enum AIState
        {
            Spawn,
            Idle,
            Despawn,
            Death,

            LaserBolt,
            LaserBolt_Enrage,
            PhaseTransition,

            BouncingIdle,
            WhiteWhips,
            DiscoHead,

            SpawnIdle
        }


        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float BounceTimer => ref NPC.ai[2];
        private ref float AttackCounter => ref NPC.ai[3];
        private PatternManager<AIState> _p2PatternBackingField;
        private PatternManager<AIState> P2PatternManager
        {
            get
            {
                if(_p2PatternBackingField == null)
                {
                    _p2PatternBackingField = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.WhiteWhips, 1.0f),
                        new Tuple<AIState, float>(AIState.DiscoHead, 1.0f));
                }
                return _p2PatternBackingField;
            }
        }

        private bool AllHeartsDead => !NPC.AnyNPCs(ModContent.NPCType<TowerHeart>());
        private Color TargetOutlineColor;
        private int IllurianSnipeDamage => 28;
        private int ShockwaveDamage => 20;
        private int WhiteWhipDamage => 15;
        private int HomingWhiteMothDamage => 18;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_setTowerPosition);
            writer.Write(_inPhase2);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _setTowerPosition = reader.ReadBoolean();
            _inPhase2 = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _draw.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 40;
            NPC.lifeMax = 12000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/MothlightBoss");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByProjectile(projectile, ref modifiers);
            if (_inPhase2)
                return;

            //Here we want to set the damage of the projectile to NOTHING if it's not the IllurianSoul
            modifiers.FinalDamage *= 0;
        }

        private int CountHearts()
        {
            int count = 0;
            foreach(var npc in Main.ActiveNPCs)
            {
                if(npc.type == ModContent.NPCType<TowerHeart>() && npc.ai[1] == NPC.whoAmI)
                {
                    count++;
                }
            }

            return count;
        }
        private void Shine()
        {
            if (!MultiplayerHelper.IsHost)
                return;
            _shineTimer++;
            if(_shineTimer >= 100)
            {
                float radians = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                Vector2 velocity = radians.ToRotationVector2();
                velocity *= 800;
                Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                    ModContent.ProjectileType<DiscoLight>(), WhiteWhipDamage, 1, Main.myPlayer, ai1: Main.rand.Next(2, 4), ai2: NPC.whoAmI);
                _shineTimer = 0;
            }

        }

        public override void AI()
        {
            base.AI();
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            _draw.outlineColor = Color.Lerp(_draw.outlineColor, TargetOutlineColor, 0.1f);
        
            ManageTowerPosition();
            SummonHearts();
            Shine();
            int newHeartCount = CountHearts();
            if (newHeartCount < _heartCount)
            {
             
                _heartCount = newHeartCount;
                SwitchState(AIState.LaserBolt_Enrage);
            }
            Inner_AI();
        }
        private void Inner_AI()
        {
            switch (State)
            {
                case AIState.SpawnIdle:
                    AI_SpawnIdle();
                    break;
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
                case AIState.LaserBolt:
                    AI_LaserBolt();
                    break;
                case AIState.LaserBolt_Enrage:
                    AI_LaserBoltEnrage();
                    break;
                case AIState.PhaseTransition:
                    AI_PhaseTransition();
                    break;
                case AIState.BouncingIdle:
                    AI_BouncingIdle();
                    break;
                case AIState.WhiteWhips:
                    AI_WhiteWhips();
                    break;
                case AIState.DiscoHead:
                    AI_DiscoHead();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        #region Tower of Illuria
        private void SummonHearts()
        {
            if (!MultiplayerHelper.IsHost)
                return;

            if (_summonedHearts)
                return;


            SummonHeart(0, 200, 150);
            SummonHeart(MathHelper.Pi, 200, 150);
            SummonHeart(MathHelper.PiOver2, 200, 150);
            SummonHeart(MathHelper.Pi + MathHelper.PiOver2, 200, 150);

            SummonHeart(0, 400, 100);
            SummonHeart(MathHelper.PiOver2, 400, 100);
            SummonHeart(MathHelper.PiOver2 * 2, 400, 100);
            SummonHeart(MathHelper.PiOver2 * 3, 400, 100);
            _heartCount = 8;
            _summonedHearts = true;

        }

        private void SummonHeart(float radiansOffset, float xRadius, float yRadius)
        {
            int heartType = ModContent.NPCType<TowerHeart>();


            int x = (int)NPC.Center.X;
            int y = (int)NPC.Center.Y;

            NPC.NewNPC(SourceFromThis, x, y, heartType, ai0: radiansOffset, ai1: NPC.whoAmI, ai2: xRadius, ai3: yRadius);
        }
        private void ManageTowerPosition()
        {
            if (!MultiplayerHelper.IsHost)
                return;

            if (!_setTowerPosition)
            {
                Vector2 ground = FindGround();
                ground.Y -= 125;
                _draw.towerDrawCenter = ground;
                NPC.Center = ground - new Vector2(0, 250);
                NPC.netUpdate = true;
                _setTowerPosition = true;
            }
        }

        private Vector2 FindGround()
        {
            Vector2 groundPoint = CollisionHelper.RayCast(NPC.Top, Vector2.UnitY, 2000, 3);
            return groundPoint;
        }

        #endregion
        private void AI_PhaseTransition()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                NPC.velocity.Y = -5;
            }

            TargetOutlineColor = Color.Yellow;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.velocity.X = 0;
            NPC.rotation = 0;
            if (NPC.collideY)
            {
                NPC.velocity.Y = -2;
                Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, -Vector2.UnitY, Color.White, Scale: 0.2f);
            }
            if(Timer >= 100)
            {
                SwitchState(AIState.BouncingIdle);
            }
        }

        private void CreateShockwaveParticles()
        {
            SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
            boom.PitchVariance = 0.3f;
            SoundEngine.PlaySound(boom, NPC.position);
            for (int i = 0; i < 16; i++)
            {
                float radius = 150;
                Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                offset *= Main.rand.NextFloat(1f, radius);
                offset += new Vector2(radius / 2, 0);

                Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                velocity *= Main.rand.NextFloat(1f, 2f);
                Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                    Main.rand.NextFloat(0.3f, 0.7f));
            }

            FXUtil.GlowCircleBoom(NPC.Bottom,
               innerColor: Color.White,
               glowColor: Color.Black,
               outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(240);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black,
                    baseSize: 0.24f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                var particle = FXUtil.GlowStretch(NPC.Bottom, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                particle.VectorScale *= 0.5f;
            }
        }
        private void Bounce()
        {
            BounceTimer++;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            float baseX = MathF.Sin(BounceTimer * 0.03f) * 8;
            float slightX = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, baseX + slightX, 0.1f);
            NPC.rotation += NPC.velocity.X * 0.03f;
            if (NPC.collideY)
            {
                NPC.velocity.Y = -15;
                Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, -Vector2.UnitY, Color.White, Scale: 0.2f);
                CreateShockwaveParticles();
                if (MultiplayerHelper.IsHost)
                {
                    //This is the part where you spawn the cool ahh shockwaves
                    //But we have to make cool ahh shockwaves :(
                    int knockback = 1;
                    Vector2 velocity = Vector2.UnitX;
                    velocity *= 4;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                        ModContent.ProjectileType<WindShockwave>(), ShockwaveDamage, knockback, Main.myPlayer);
                    velocity = -velocity;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                        ModContent.ProjectileType<WindShockwave>(), ShockwaveDamage, knockback, Main.myPlayer);
                }
            }
        }

        private void AI_BouncingIdle()
        {
        
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }
            AttackCounter = 0;
            TargetOutlineColor = Color.Transparent;
            Bounce();


            if(Timer % 10 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 
                    ModContent.DustType<Sparkle>());
            }

            if(Timer >= 360 && NPC.velocity.Y < -2)
            {
                ChoosePhase2Attack();
            }
        }

        private void AI_WhiteWhips()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                float numDust = 8;
                for(float d = 0; d < numDust; d++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPos = NPC.Center;
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowDust>(), velocity,
                        newColor: Color.White, 
                        Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
            }

            Bounce();
             TargetOutlineColor = Color.Yellow;

    
            float totalNumWhips = 36;
            float loops = 4;
            if(Timer % 10 == 0 && Timer < 100)
            {
                if (MultiplayerHelper.IsHost)
                {
                    float radians = (AttackCounter / totalNumWhips) * MathHelper.TwoPi * loops;
                    radians *= 2;
                    Vector2 velocity = radians.ToRotationVector2();
                    velocity *= 7;
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity, 
                        ModContent.ProjectileType<WhiteWhip>(), WhiteWhipDamage, 1, Main.myPlayer, ai2: NPC.whoAmI);
                    AttackCounter++;
                    if(AttackCounter >= totalNumWhips)
                    {
                        SwitchState(AIState.BouncingIdle);
                    }
                }
            }
            if(Timer >= 200)
            {
                Timer = 0;
            }
        }


        private void AI_DiscoHead()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                float numDust = 8;
                for (float d = 0; d < numDust; d++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPos = NPC.Center;
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowDust>(), velocity,
                        newColor: Color.White,
                        Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
            }

            if (Timer % 10 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    ModContent.DustType<Sparkle>());
            }

            if(NPC.velocity.Y > 1)
            {
                NPC.velocity.Y -= 0.5f;
            }
            Bounce();
            TargetOutlineColor = Color.Yellow;
            float totalNumLights = 36;
            if (Timer % 16 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    float radians = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                    Vector2 velocity = radians.ToRotationVector2();
                    velocity *= 800;
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                        ModContent.ProjectileType<DiscoLight>(), WhiteWhipDamage, 1, Main.myPlayer, ai1: Main.rand.Next(0, 2), ai2: NPC.whoAmI);
                    AttackCounter++;
                    if (AttackCounter >= totalNumLights)
                    {
                        SwitchState(AIState.BouncingIdle);
                    }
                }
            }

            if(Timer % 100 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(32, 32);
                    Vector2 spawnPos = NPC.Center + offset;
                    Vector2 velocity = -Vector2.UnitY * 7;

                    Projectile.NewProjectile(SourceFromThis, spawnPos, velocity, ModContent.ProjectileType<HomingWhiteMoth>(), HomingWhiteMothDamage, 1, Main.myPlayer);
                }
            }
        }

        private void AI_LaserBolt()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            TargetOutlineColor = Color.Yellow;
            Hover();

            if (Timer == 60 && MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.UnitX * 8,
                    ModContent.ProjectileType<IllurianSnipe>(), IllurianSnipeDamage, 1, Main.myPlayer, ai2: NPC.whoAmI);
            }

            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }
        private void AI_LaserBoltEnrage()
        {
            Timer++;
            if (Timer == 1)
            {
                var part = Particle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
                part.Scale *= 4;
                part.shrink = true;
                part.noStretch = true;
                NPC.TargetClosest();
            }

            TargetOutlineColor = Color.Yellow;
            Hover();
            Hover();
            ShakeModSystem.Shake = 2;
            _shakeOffset = Main.rand.NextVector2Circular(3, 3);
            if (Timer % 20 == 0 && MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.UnitX * 8,
                    ModContent.ProjectileType<IllurianSnipe>(), IllurianSnipeDamage, 1, Main.myPlayer, ai2: NPC.whoAmI);
            }

            NPC.AddBuff(BuffID.Frostburn, 2);
            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_SpawnIdle()
        {

        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            ShakeModSystem.Shake = 4;
            if (Timer % 7 == 0)
            {
                Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
            }

            Hover();
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            if (Timer >= 100)
            {
                SwitchState(AIState.Idle);
            }
        }
        private void Hover()
        {
            _hoverTimer++;
            Vector2 startPosition = _draw.towerDrawCenter;
            startPosition.Y += MathF.Sin(_hoverTimer * 0.025f) * 32;
            startPosition.Y -= 180;
            Vector2 h = startPosition - NPC.Center;
            NPC.velocity = h;
        }

        private void AI_Idle()
        {
          
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            if (!_showNamePlate)
            {
                ShowNamePlate();
                _showNamePlate = true;
            }
            //Check for all hearts dying to do the phase transition
            if (MultiplayerHelper.IsHost && !_inPhase2 && AllHeartsDead)
            {
                SwitchState(AIState.PhaseTransition);
                _inPhase2 = true;
            }
            AttackCounter = 0;
            _draw.afterImageAlpha = MathHelper.Lerp(_draw.afterImageAlpha, 1f, 0.1f);
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            Hover();

            NPC.rotation = 0;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 120)
            {
                if (!_inPhase2)
                {
                    ChoosePhase1Attack();
                }
            }

            if(MultiplayerHelper.IsHost && Timer == 60)
            {
                Vector2 offset = Main.rand.NextVector2Circular(256, 256);
                Vector2 spawnPos = NPC.Center + offset;
                Vector2 velocity = -Vector2.UnitY * 7;

                Projectile.NewProjectile(SourceFromThis, spawnPos, velocity, ModContent.ProjectileType<HomingWhiteMoth>(), HomingWhiteMothDamage, 1, Main.myPlayer);
            }
        }

        private void ChoosePhase1Attack()
        {
            SwitchState(AIState.LaserBolt);
        }

        private void ChoosePhase2Attack()
        {
            if (!MultiplayerHelper.IsHost)
                return;
            SwitchState(P2PatternManager.NextPattern());
        }

        private void AI_Despawn()
        {
            Timer++;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.velocity.X *= 0.5f;
            NPC.velocity.Y += 0.5f;
            if (Timer >= 100)
            {
                NPC.active = false;
            }
        }

        private void AI_Death()
        {
            Timer++;
            if (Timer % 16 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    float radians = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                    Vector2 velocity = radians.ToRotationVector2();
                    velocity *= 800;
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                        ModContent.ProjectileType<DiscoLight>(), WhiteWhipDamage, 1, Main.myPlayer, ai1: Main.rand.Next(0, 2), ai2: NPC.whoAmI);
                }
            }

            if (Timer % 10 == 0)
            {
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(200, 200);
                Vector2 spawnVelocity = (NPC.Center - spawnPos).SafeNormalize(Vector2.Zero);
                spawnVelocity *= 24;
                var stretch = FXUtil.GlowStretch(spawnPos, spawnVelocity);
                stretch.Scale *= Main.rand.NextFloat(0.5f, 1f);
                stretch.VectorScale.X *= Main.rand.NextFloat(0.5f, 1f);
            }

            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            ShakeModSystem.Shake = 4;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.velocity.X *= 0.95f;
            NPC.velocity.Y *= 0.95f;
            if (Timer >= 400)
            {
                FXUtil.ShakeCamera(NPC.position, 1024, 32);
                ShakeModSystem.Shake = 20;
                var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Cyan, Color.Purple);
                boom.Scale *= 2f;

                boom = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Cyan, Color.Purple);
                boom.Scale *= 1.2f;
                float numDust = 16;
                for(float n = 0; n < numDust; n++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(64, 64);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightCyan, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
                for (float f = 0; f < 42; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(128, 128);
                    FXUtil.GlowStretch(NPC.Center, velocity);
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightCyan,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 4;
                }

                for (float f = 0; f < 16; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                    pVelocity *= Main.rand.NextFloat(0.5f, 8f);
                    var spark = Particle.NewParticle<EmberParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }
        
                var donut = Particle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, newColor: Color.Cyan);
                donut.shrink = true;
                NPC.Kill();
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.CrumblingTowerOfIlluria);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (NPC.life <= 0 && State != AIState.Death)
            {
                NPC.life = 1;
                SwitchState(AIState.Death);
            }


            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
        }

        #region Draw Code
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawAfterImages(spriteBatch, screenPos + _shakeOffset, drawColor);
            DrawBase(spriteBatch, screenPos + _shakeOffset, drawColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos + _shakeOffset, drawColor);
            DrawGlow(spriteBatch, screenPos + _shakeOffset, drawColor);
            _shakeOffset = Vector2.Zero;
            return false;
        }

        private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int trailLength = NPC.oldPos.Length;
            for (int i = 0; i < trailLength; i++)
            {
                float f = i;
                float numAfterImages = trailLength;
                float completionRatio = f / numAfterImages;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                afterImageColor *= 0.2f;
                afterImageColor *= _draw.afterImageAlpha;
                Vector2 drawPosition = NPC.oldPos[i] + NPC.Size / 2f;
                DrawSprite(spriteBatch, drawPosition - screenPos, afterImageColor);
            }
        }

        private void DrawBase(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D towerTexture = ModContent.Request<Texture2D>(Texture + "_Tower").Value;
            Rectangle? frame = null;
            Vector2 drawOrigin = towerTexture.Size() / 2f;
            Vector2 drawCenter = _draw.towerDrawCenter - screenPos;
            spriteBatch.Draw(towerTexture, drawCenter, frame, drawColor, 0, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawPosition, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        private void DrawGlow(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float numAfterImages = 8;
            for (float n = 0; n < numAfterImages; n++)
            {
                float completionRatio = n / numAfterImages;
                float rot = MathHelper.TwoPi * completionRatio;
                Vector2 offset = rot.ToRotationVector2();
                offset *= ExtraMath.Osc(16, 24, speed: 2);
                Color glowColor = Color.White;
                glowColor.A = 0;
                glowColor *= 0.2f;
                glowColor *= ExtraMath.Osc(0.2f, 0.5f, speed: 1);
                DrawSprite(spriteBatch, NPC.Center - screenPos + offset, glowColor);
            }
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitX * outlineOffset;
            Vector2 h = Vector2.UnitY * outlineOffset;
            DrawSprite(spriteBatch, NPC.Center - screenPos + v, _draw.outlineColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos - v, _draw.outlineColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos + h, _draw.outlineColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos - h, _draw.outlineColor);
        }
        #endregion
    }
}
