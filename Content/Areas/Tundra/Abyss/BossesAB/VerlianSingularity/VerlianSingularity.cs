using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Wings;
using Stellamod.Skies;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity
{

    public class SingularityHitbox : VSProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
        }
        public override void AI()
        {
            base.AI();
            Projectile.Center = GetParentNPC().Center;
            if (NPC.AnyNPCs(ModContent.NPCType<VerlianSingularity>()))
            {
                Projectile.timeLeft = 2;
            }
        }
    }
    [AutoloadEquip(EquipType.Wings)]
    public class MagicWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            // These wings use the same values as the solar wings
            // Fly time: 180 ticks = 3 seconds
            // Fly speed: 9
            // Acceleration multiplier: 2.5
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f; // Falling glide speed
            ascentWhenRising = 0.15f; // Rising speed
            maxCanAscendMultiplier = 2;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

    }

    public class SingularitySuckPlayer : ModPlayer
    {
        private float _frameCounter;
        private int _frame;
        private float _frameSpeed;
        private float _wingTimer;

        public Vector2? pullVelocity;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (pullVelocity.HasValue)
            {
                Vector2 velocity = pullVelocity.Value;
                Player.velocity = velocity;
                for(float f = 0; f < 3; f++)
                {
                    FXUtil.GlowStretch(Player.Center, velocity.RotatedByRandom(MathHelper.ToRadians(15)));
                }

                pullVelocity = null;
            }
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (!fallSystem.inSpace)
                return;
            if (fallSystem.noWings)
                return;


            _wingTimer++;
            if (_wingTimer % 7 == 0)
            {
                Dust.NewDustPerfect(Player.Center, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.White, Scale: 0.5f);
            }

            int wingSlot = EquipLoader.GetEquipSlot(Mod, "MagicWings", EquipType.Wings);
            Player.wings = wingSlot;
            Player.wingsLogic = wingSlot;
            Player.wingTime = 1000;
            Player.wingTimeMax = 1000;
            Player.noFallDmg = true;
            Player.equippedWings = Player.armor[1];

            if (IsFlying())
            {
                _frameSpeed = 4;
                _frameCounter++;
                if (_frameCounter >= _frameSpeed)
                {
                    _frameCounter = 0;
                    _frame++;
                    if (_frame >= 8)
                    {
                        _frame = 0;
                    }
                }
            }
            else
            {
                if(_frame > 0)
                {
                    _frameCounter--;
                    if (_frameCounter <= 0)
                    {
                        _frameCounter = _frameSpeed;
                        _frame--;
                    }
                }
          
              
            }
        }

        private bool IsFlying()
        {
            return Player.controlJump && !Player.mount.Active && Player.wingTime > 0;
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (!fallSystem.inSpace)
                return;
            if (drawInfo.shadow != 0f)
                return;
            float alpha = EasingFunction.InOutSine(_wingTimer / 60f);
            Texture2D wingsTexture = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/MagicWingsProj").Value;
            Rectangle frame = wingsTexture.GetFrame(_frame, 8);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color glowColor = Color.White;
            glowColor *= alpha;
            glowColor.A = 0;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawScale = Vector2.One;

            Vector2 drawPosition = Player.Center - Main.screenPosition;
            drawPosition.Y -= 12;
            Texture2D zuiTexyt = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            spriteBatch.Draw(zuiTexyt, drawPosition, null, glowColor, 0, zuiTexyt.Size() / 2f, drawScale * 0.75f, SpriteEffects.None, 0);
            spriteBatch.Draw(wingsTexture, drawPosition, frame, glowColor, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }

    public class VerlianSingularity : ScarletBoss
    {
        private float _incresionDiskFrameBottom;
        private float _incresionDiskFrameTop;

        private float _spawnScale;
        private float _spazzingTimer;
        private float _hitTimer;
        private float _hitScale;
        private bool _focusOn;
        private bool _spawnedCrescentMoon;
        private bool _starField;
        private bool _spawnedHitbox;
        private bool _ragingGlowCircle;
        private bool _warning;
        private float _bloomLine;
        private Vector2 _shakeOffset;
        private Vector2 _hitOffset;
        private Color _chargeColor;
        private enum AIState
        {
            Spawn,
            Idle,
            OrbitingStarPull,
            SpiralStarPull,
            ZigzagStorm,
            SlowFallingStars,
            SingularityBoom,
            Phase2Transition,
            BlackLightning,
            BerserkLaser,
            Death,
            Despawn
        }

        private int ShootingStarDamage => 24;
        private int SpiralStarDamage => 16;
        private int SingularityBoom => 32;
        private int BlackLightningDamage => 20;
        private int BerserkLaserDamage => 50;

        private float _spinTimer;
        private ref float Timer => ref NPC.ai[0];
        private ref float AttackCounter => ref NPC.ai[1];
        private AIState State
        {
            get => (AIState)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }
        private ref float AttackCycle => ref NPC.ai[3];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_spinTimer);
            writer.Write(_starField);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _spinTimer = reader.ReadSingle();
            _starField = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _chargeColor = Color.White;
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 100;
            NPC.defense = 14;
            NPC.lifeMax = 6000;
            NPC.scale = 1f;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SingularityFragment");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/VoidHit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/VoidDead1") with { PitchVariance = 0.1f };
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            DifficultyChanges.ApplyDifficultyAndScaling(NPC, numPlayers);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
            DrawHelper.UpdateFrame(ref _incresionDiskFrameTop, 0.8f, 1, 76);
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                if (State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }

            _spinTimer++;
            if (_starField)
            {


                DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
                fallSystem.inSpace = true;
                fallSystem.noProjTileCollide = true;
                RoyalCapitalStars stars = ModContent.GetInstance<RoyalCapitalStars>();
                stars.inStarField = true;
            }

            if (NPC.rotation == 0f)
                NPC.rotation += MathHelper.ToRadians(15);
            NPC.rotation += 0.001f;
            if (!_starField)
            {

                NPC.velocity = Vector2.UnitY.RotatedBy(_spinTimer * 0.02f) * 0.5f;
                NPC.velocity.X = 0;


            }
            else
            {
                NPC.velocity = Vector2.UnitY.RotatedBy(_spinTimer * 0.01f) * 2.2f;
            }

            if (_spazzingTimer > 0)
            {
                if (_spazzingTimer % 10 == 0)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.White;
                    spark.outerColor = Color.Cyan;
                    spark.fadeToColor = Color.Blue;
                    spark.Scale *= 0.5f;

                }
                _shakeOffset = Main.rand.NextVector2Circular(4, 4);
                _spazzingTimer--;
            }

            if (_hitTimer > 0)
            {
                _hitOffset = Main.rand.NextVector2Circular(2, 2);
                _hitTimer--;
            }
            _hitScale = MathHelper.Lerp(_hitScale, 1f, 0.1f);
            _warning = false;
            _ragingGlowCircle = false;
            SuckNearbyPlayers();
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    if (!_spawnedHitbox)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<SingularityHitbox>(), 50, 1, Main.myPlayer, ai0: NPC.whoAmI);
                        }
                        _spawnedHitbox = true;
                    }
                    if (!_spawnedCrescentMoon)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                                ModContent.NPCType<VerlianMoon>(), ai0: NPC.whoAmI);
                        }

                        _spawnedCrescentMoon = true;
                    }
                    _spawnScale = MathHelper.Lerp(_spawnScale, 1, 0.1f);
                    SuckingParticles();
                    AI_Idle();
                    break;
                case AIState.OrbitingStarPull:
                    AI_OrbitingStarPull();
                    break;
                case AIState.SpiralStarPull:
                    AI_SpiralStarPull();
                    break;
                case AIState.ZigzagStorm:
                    AI_ZigzagStorm();
                    break;
                case AIState.SlowFallingStars:
                    AI_SlowFallingStars();
                    break;
                case AIState.SingularityBoom:
                    AI_SingularityBoom();
                    break;
                case AIState.Phase2Transition:
                    AI_Phase2Transition();
                    break;
                case AIState.BlackLightning:
                    AI_BlackLightning();
                    break;
                case AIState.BerserkLaser:
                    AI_BerserkLaser();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
            }
        }

        private void AI_Despawn()
        {
            Timer++;
            float interpolant = Timer / 120f;
            float ease = EasingFunction.InOutSine(interpolant);
            _spawnScale = MathHelper.Lerp(_spawnScale, 0f, ease);
            if (Timer >= 120f)
            {
                NPC.active = false;
            }
        }

        private void AI_BlackLightning()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge");
                SoundEngine.PlaySound(chargeSound, NPC.position);
            }
            if (Timer % 10 == 0 && Timer < 120)
            {
                _spawnScale *= 0.75f;
            }
            else if (Timer < 180)
            {
                _chargeColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 32));
            }
            else if (Timer < 500 && Timer % 15 == 0)
            {
                SpawnPulse();
                _spawnScale *= 1.25f;
                _spawnScale *= Main.rand.NextFloat(0.5f, 1f);
                if (MultiplayerHelper.IsHost)
                {
                    int blackLightningProjectileType = ModContent.ProjectileType<BlackLightning>();
                    Vector2 spawnPos = NPC.Center;
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(1, 1);
                    var source = NPC.GetSource_FromThis();
                    Projectile.NewProjectile(source, spawnPos, velocity, blackLightningProjectileType, BlackLightningDamage, 2, Main.myPlayer, ai0: NPC.whoAmI);
                    velocity = Main.rand.NextVector2CircularEdge(1, 1);
                    Projectile.NewProjectile(source, spawnPos, velocity, blackLightningProjectileType, BlackLightningDamage, 2, Main.myPlayer, ai0: NPC.whoAmI);
                    velocity = Main.rand.NextVector2CircularEdge(1, 1);
                    Projectile.NewProjectile(source, spawnPos, velocity, blackLightningProjectileType, BlackLightningDamage, 2, Main.myPlayer, ai0: NPC.whoAmI);
                }
            }
            else if (Timer >= 500)
            {
                _spawnScale = MathHelper.Lerp(_spawnScale, 1f, 0.05f);
                if(Timer >= 600)
                {
                    SwitchState(AIState.SingularityBoom);
                }
            
            }
        }

        private void AI_BerserkLaser()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge2");
                SoundEngine.PlaySound(chargeSound, NPC.position);
                FXUtil.FocusCamera(NPC.Center - Vector2.UnitY * 500, 120);
                SpawnPulse();
            }

            if (Timer < 60)
            {
                _spawnScale = MathHelper.Lerp(_spawnScale, 0.5f, 0.01f);
            }

            if(Timer < 120)
            {
                _bloomLine = MathHelper.Lerp(_bloomLine, 1f, EasingFunction.InOutSine(Timer / 120f));
                if(Timer % 5 == 0)
                {
                    Vector2 position = NPC.Center - Vector2.UnitY * Main.rand.NextFloat(0, 1000);
                    Vector2 velocity = -Vector2.UnitY;
                    var part = LegacyParticle.NewParticle<ZapParticle>(position, velocity);
                    part.Scale *= Main.rand.NextFloat(0.5f, 3f);

                    position = NPC.Center - Vector2.UnitY * Main.rand.NextFloat(0, 1000);
                    var part2 = LegacyParticle.NewParticle<SparkParticle>(position, velocity);
                    part2.Scale *= Main.rand.NextFloat(0.5f, 3f);
                }
            }
            if (Timer == 120)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<SingularityBoom>(), SingularityBoom, 2, Main.myPlayer);
                }
                if (MultiplayerHelper.IsHost)
                {
                    int damage = BerserkLaserDamage;
                    int projType = ModContent.ProjectileType<BerserkLaser>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitY * 1000,
                        projType, damage, 2, Main.myPlayer, ai0: NPC.whoAmI);
                }
            }

            if(Timer >= 120)
            {
                _bloomLine *= 0.9f;
                SuckingParticles();
            }
            if(Timer >= 120 && Timer % 45 == 0 && Timer < 1200)
            {
         
                SpawnPulse();
                if (MultiplayerHelper.IsHost)
                {
                    int blackLightningProjectileType = ModContent.ProjectileType<BlackLightning>();
                    Vector2 spawnPos = NPC.Center;
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(1, 1);
                    var source = NPC.GetSource_FromThis();
                    Projectile.NewProjectile(source, spawnPos, velocity, blackLightningProjectileType, BlackLightningDamage, 2, Main.myPlayer, ai0: NPC.whoAmI);
                    velocity = Main.rand.NextVector2CircularEdge(1, 1);
                    Projectile.NewProjectile(source, spawnPos, velocity, blackLightningProjectileType, BlackLightningDamage, 2, Main.myPlayer, ai0: NPC.whoAmI);
                    velocity = Main.rand.NextVector2CircularEdge(1, 1);
                    Projectile.NewProjectile(source, spawnPos, velocity, blackLightningProjectileType, BlackLightningDamage, 2, Main.myPlayer, ai0: NPC.whoAmI);
                }
            }

            if(Timer >= 120 && Timer < 1200)
            {
                _ragingGlowCircle = true;
            }
            if(Timer >= 120 && Timer % 2 == 0)
            {
                _shakeOffset = Main.rand.NextVector2Circular(12, 12);
            }

            if(Timer > 600)
            {
       
            }
            if (Timer >= 1320)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Death()
        {
            Timer++;
            _ragingGlowCircle = true;
            if (Timer == 1)
            {
                SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge");
                SoundEngine.PlaySound(chargeSound, NPC.position);
                FXUtil.FocusCamera(NPC.Center, 400);
            }

            if (Timer % 4 == 0)
            {
                _shakeOffset = Main.rand.NextVector2Circular(24, 24);
            }

            SuckingParticles();
            SuckingParticles();
            foreach (var proj in Main.ActiveProjectiles)
            {
                //Cheaper check than casting the mod proj
                if (proj.ai[0] == NPC.whoAmI)
                    proj.Kill();
            }

            if (Timer == 400)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<SingularityBoom>(), SingularityBoom, 2, Main.myPlayer);
                }
                NPC.Kill();
            }
        }

        private void AI_Phase2Transition()
        {
            Timer++;
            if (Timer == 1)
            {
                FXUtil.FocusCamera(NPC.Center, 400);
                SpawnPulse();
            }

            float targetScale = MathHelper.Lerp(0f, 1f, AttackCounter / 4f);
            _spawnScale = MathHelper.Lerp(_spawnScale, targetScale, 0.1f);

            if (Timer >= 102)
            {
                Timer = 0;
                AttackCounter++;

                if (AttackCounter >= 3)
                {
                    _starField = true;
                    SpawnPulse();
                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                            ModContent.ProjectileType<SingularityBoom>(), SingularityBoom, 2, Main.myPlayer);
                    }
                    SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn");
                    SoundEngine.PlaySound(crackSound, NPC.position);
                    SwitchState(AIState.Idle);
                }
            }
        }

        private void AI_SingularityBoom()
        {
            _warning = true;
            Timer++;
            if (Timer < 100)
            {
              
                float interpolant = Timer / 100;
                float ease = EasingFunction.InOutSine(interpolant);
                _spawnScale = MathHelper.Lerp(1f, 0.25f, ease);
                _chargeColor = Color.Lerp(Color.White, Color.Yellow, ease);
                if (Timer % 10 == 0)
                {
                    _shakeOffset = Main.rand.NextVector2CircularEdge(16, 16) * ease;

                }
                if (Timer % 20 == 0)
                {
                    SpawnPulse();
                }
            }
            else if (Timer < 160)
            {
                _chargeColor = Color.Lerp(Color.Yellow, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));

            }
            else if (Timer == 160)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<SingularityBoom>(), SingularityBoom, 2, Main.myPlayer);
                }
            }
            else
            {
                _spawnScale = MathHelper.Lerp(_spawnScale, 1.5f, ExtraMath.Osc(0f, 0.1f, speed: 32));
            }
            if (Timer >= 240)
            {
                _chargeColor = Color.White;
                SwitchState(AIState.Idle);
            }
        }
        private void ChooseAttack()
        {
            if (!MultiplayerHelper.IsHost)
                return;

            if (!_starField)
            {
                switch (AttackCycle)
                {
                    case 0:
                        SwitchState(AIState.OrbitingStarPull);
                        break;
                    case 1:
                        SwitchState(AIState.SpiralStarPull);
                        break;
                    case 2:
                        SwitchState(AIState.ZigzagStorm);
                        break;
                    case 3:
                        SwitchState(AIState.SlowFallingStars);
                        break;
                    case 4:
                        SwitchState(AIState.SingularityBoom);
                        break;
                }

                AttackCycle++;
                if (AttackCycle >= 5)
                {
                    AttackCycle = 0;
                }
            }
            else if (_starField)
            {
                switch (AttackCycle)
                {
                    case 0:
                        SwitchState(AIState.SingularityBoom);
                        break;
                    case 1:
                        SwitchState(AIState.OrbitingStarPull);
                        break;
                    case 2:
                        SwitchState(AIState.BlackLightning);
                        break;
                    case 3:
                        SwitchState(AIState.ZigzagStorm);
                        break;
                    case 4:
                        SwitchState(AIState.SlowFallingStars);
                        break;
                    case 5:
                        SwitchState(AIState.SpiralStarPull);
                        break;
                    case 6:
                        SwitchState(AIState.BerserkLaser);
                        break;
                }

                AttackCycle++;
                if (AttackCycle >= 7)
                {
                    AttackCycle = 0;
                }
            }

            if (NPC.life < NPC.lifeMax / 2 && !_starField)
            {
                SwitchState(AIState.Phase2Transition);
            }
        }

        private void SuckNearbyPlayers()
        {
            foreach (var player in Main.ActivePlayers)
            {
                float distanceToPlayer = NPC.DistanceFrom(player);
                if (distanceToPlayer > 1000 && distanceToPlayer < 2000)
                {
                    Vector2 pullingVelocity = player.NormalizedVelocityTo(NPC);
                    pullingVelocity *= 5;

                    SingularitySuckPlayer suckPlayer = player.GetModPlayer<SingularitySuckPlayer>();
                    suckPlayer.pullVelocity = pullingVelocity;
                }
            }
        }

        private void HitOut()
        {
            _hitScale = 0.92f;
            _hitTimer = 10;
        }

        /// <summary>
        /// Creates a pulsing effect, good way to show the singularity is summoning projectiles
        /// </summary>
        private void SpazOut()
        {
            _spawnScale *= 0.9f;
            _spazzingTimer = 120;
            FXUtil.ShakeCamera(NPC.position, 1024, 8);
            FXUtil.GlowCircleBoom(NPC.Center,
                       innerColor: Color.White,
                       glowColor: Color.LightBlue,
                       outerGlowColor: Color.Blue, duration: 25, baseSize: 0.08f);

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.innerColor = Color.White;
                spark.outerColor = Color.Cyan;
                spark.fadeToColor = Color.Blue;
            }

            SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot1");
            crackSound.PitchVariance = 0.1f;
            SoundEngine.PlaySound(crackSound, NPC.position);
        }
        private void MiniSpazOut()
        {
            _spawnScale *= 0.97f;
            _spazzingTimer = 60;

            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<SparkParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.innerColor = Color.White;
                spark.outerColor = Color.Cyan;
                spark.fadeToColor = Color.Blue;
            }

            SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot1");
            crackSound.PitchVariance = 0.1f;
            SoundEngine.PlaySound(crackSound, NPC.position);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            HitOut();
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

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.Verlian_Singularity);
        }

        private void AI_SlowFallingStars()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
                _spawnScale *= 1.01f;
            }

            if (Timer > 60 && Timer % 22 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int orbitingStarType = ModContent.ProjectileType<SlowFallingStar>();
                    float rot = Timer * 0.05f;

                    Vector2 offset = rot.ToRotationVector2();
                    offset *= 1000;

                    Vector2 spawnVelocity = Vector2.Zero;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + offset, spawnVelocity, orbitingStarType, SpiralStarDamage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - offset, spawnVelocity, orbitingStarType, SpiralStarDamage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                }
                AttackCounter++;
            }

            if (AttackCounter >= 32)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_ZigzagStorm()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }
            if (Timer == 10)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }
            if (Timer == 20)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }

            if (Timer > 60 && Timer % 20 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int orbitingStarType = ModContent.ProjectileType<ZigzaggingStar>();
                    float rot = Timer * 0.05f;

                    Vector2 offset = rot.ToRotationVector2();
                    offset *= 1000;
                    Vector2 spawnPos = NPC.Center + offset;
                    Vector2 spawnVelocity = Vector2.Zero;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, spawnVelocity, orbitingStarType, SpiralStarDamage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                }
                AttackCounter++;
            }

            if (AttackCounter >= 32)
            {
                SwitchState(AIState.Idle);
            }

        }
        private void AI_SpiralStarPull()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPOut");
                crackSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crackSound, NPC.position);
                SpazOut();
            }

            if (Timer % 10 == 0)
            {
                MiniSpazOut();

                if (MultiplayerHelper.IsHost)
                {
                    float num = 2;
                    for (float f = 0; f < num; f++)
                    {
                        int orbitingStarType = ModContent.ProjectileType<SpiralFallingStar>();
                        float interpolant = f / num;
                        float rot = MathHelper.TwoPi * interpolant;
                        rot += Timer * 0.05f;
                        Vector2 offset = rot.ToRotationVector2();
                        offset *= 1000;
                        Vector2 spawnPos = NPC.Center + offset;
                        Vector2 spawnVelocity = Vector2.Zero;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, spawnVelocity, orbitingStarType, SpiralStarDamage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                    }
                }

                AttackCounter++;
            }

            if (AttackCounter >= 32)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_OrbitingStarPull()
        {
            Timer++;
            if (Timer == 1)
            {
                SpazOut();
            }

            //Asgore attack basically
            if (Timer >= 60)
            {
                var part = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
                part.Scale *= 4;
                part.shrink = true;
                part.noStretch = true;
                if (MultiplayerHelper.IsHost)
                {
                    int orbitingStarType = ModContent.ProjectileType<OrbitingShootingStar>();

                    float num = 16;
                    float direction = AttackCounter % 2 == 0 ? 1 : -1;
                    float randOffset = Main.rand.NextFloat(-0.5f, 0.5f);
                    for (float f = 0; f < num; f++)
                    {
                        float interpolant = f / num;
                        float rot = MathHelper.TwoPi * interpolant;
                        rot += (AttackCounter / 3f) * MathHelper.TwoPi;
                        rot += randOffset;
                        Vector2 offset = rot.ToRotationVector2();
                        offset *= 1200;
                        Vector2 spawnPos = NPC.Center + offset;
                        Vector2 spawnVelocity = Vector2.Zero;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, spawnVelocity, orbitingStarType, ShootingStarDamage, 1, Main.myPlayer,
                            ai0: NPC.whoAmI,
                            ai2: direction);
                    }

                }
                Timer = 0;
                AttackCounter++;
            }

            if (AttackCounter >= 6)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void SuckingParticles()
        {
            if (_spinTimer % 7 == 0)
            {
                float radius = 800;
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                Vector2 velocity = NPC.Center - spawnPos;
                velocity = velocity.SafeNormalize(Vector2.Zero);
                velocity *= Main.rand.NextFloat(16, 64);
                FXUtil.GlowStretch(spawnPos, velocity);
            }
        }
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                AttackCounter = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void SpawnPulse()
        {
            float strength = MathHelper.Lerp(0f, 128, AttackCounter / 3f);
            FXUtil.ShakeCamera(NPC.position, 1024, strength);
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.innerColor = Color.White;
                spark.outerColor = Color.Cyan;
                spark.fadeToColor = Color.Blue;
            }

            var part = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
            part.Scale *= 4;
            part.shrink = true;
            part.noStretch = true;
            for (float f = 0; f < 3; f++)
            {
                float radius = 800;
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                Vector2 velocity = NPC.Center - spawnPos;
                velocity = velocity.SafeNormalize(Vector2.Zero);
                velocity *= Main.rand.NextFloat(16, 64);
                FXUtil.GlowStretch(spawnPos, velocity);
            }

            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                pVelocity *= Main.rand.NextFloat(0.5f, 8f);
                var spark = LegacyParticle.NewParticle<EmberParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
            }

            SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot");
            SoundEngine.PlaySound(crackSound, NPC.position);
        }
        private void AI_Spawn()
        {
            if (!_focusOn)
            {
                FXUtil.FocusCamera(NPC.Center, 400);
                _focusOn = true;
            }
            Timer++;
            if (Timer == 1)
            {
                SpawnPulse();
            }
            float targetScale = MathHelper.Lerp(0f, 1f, AttackCounter / 4f);
            _spawnScale = MathHelper.Lerp(_spawnScale, targetScale, 0.1f);

            if (Timer >= 102)
            {
                Timer = 0;
                AttackCounter++;

                if (AttackCounter >= 3)
                {
                    ShowNamePlate();
                    SpawnPulse();
                    SoundStyle crackSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_TPIn");
                    SoundEngine.PlaySound(crackSound, NPC.position);
                    SwitchState(AIState.Idle);
                }
            }
        }
        private void AI_Idle()
        {
            Timer++;
            float idleTime = _starField ? 200 : 300;
            if (Timer >= idleTime)
            {
                ChooseAttack();
            }
        }

        #region Draw Code
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawPosition = NPC.Center - screenPos;
            Texture2D celestialRing = ModContent.Request<Texture2D>(Texture + "_CelestialRing").Value;
            Vector2 ringDrawOrigin = celestialRing.Size() / 2f;
            Color ringDrawColor = Color.White;
            if (_warning)
            {
                ringDrawColor = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 24));
            } else
            {
            
            }
            ringDrawColor *= 0.15f;
            ringDrawColor *= _spawnScale;
            ringDrawColor.A = 0;
            spriteBatch.Draw(celestialRing, drawPosition, null, ringDrawColor, NPC.rotation, ringDrawOrigin, 4, SpriteEffects.None, 0);


            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = NPC.scale * Vector2.One * _spawnScale * 2 * _hitScale;
            drawPosition += _shakeOffset + _hitOffset;

            float spinRotOffset = _spinTimer * -0.01f;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Distortion = -0.15f;
            sparkyShader.Time = -Main.GlobalTimeWrappedHourly * 40;
            sparkyShader.Tiling = Vector2.One * 2;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: sparkyShader.Effect);


            var lightTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 lightDrawOrigin = lightTexture.Size() / 2f;

            float sparkyRot = NPC.rotation + spinRotOffset;
            float scaleOsc2 = ExtraMath.Osc(0.4f, 0.5f, speed: 1);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.75f, sparkyRot, lightDrawOrigin, drawScale * 3 * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.25f, sparkyRot + 0.2f, lightDrawOrigin, drawScale * 8 * scaleOsc2, SpriteEffects.None, 0);


            var shader = SingularityShader.Instance;
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPosition, null, Color.White, NPC.rotation, drawOrigin, drawScale * 1.5f * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Cyan, 0.15f), ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor = diskDrawColor.MultiplyRGB(_chargeColor);
            diskDrawColor.A = 0;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.65f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc, SpriteEffects.None, 0);

            diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF2").Value;
            for (float f = 0; f < 4; f++)
            {

                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(1.5f, 0.2f), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(3.5f, 0.2f), SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, NPC.rotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(7.5f, 0.2f), SpriteEffects.None, 0);


            Texture2D extra67 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_67").Value;
            Vector2 extra67DrawOrigin = extra67.Size() / 2f;
            Color extra67DrawColor = Color.Lerp(Color.White, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 2));
            extra67DrawColor.A = 0;
            spriteBatch.Draw(extra67, drawPosition, null, extra67DrawColor, NPC.rotation, extra67DrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            DrawIncresionDiskBottom(spriteBatch, screenPos, drawColor);
            DrawIncresionDiskTop(spriteBatch, screenPos, drawColor);
            if(_bloomLine > 0)
            {
                Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
                Vector2 bloomLineOrigin = bloomLineTexture.Size() / 2f;
                Color glowDrawColor = Color.Lerp(Color.Yellow, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 32));
                glowDrawColor *= _bloomLine;
                glowDrawColor.A = 0;
                spriteBatch.Draw(bloomLineTexture, drawPosition - new Vector2(0, (float)bloomLineTexture.Height), null, glowDrawColor, 0, bloomLineOrigin, drawScale, SpriteEffects.None, 0);
            }
            if (_ragingGlowCircle)
            {
                Texture2D glowCircleTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
                Vector2 glowCircleDrawOrigin = glowCircleTexture.Size() / 2f;
                Color glowDrawColor = Color.Lerp(Color.Yellow, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 32));
                glowDrawColor.A = 0;
                spriteBatch.Draw(glowCircleTexture, drawPosition, null, glowDrawColor, NPC.rotation, glowCircleDrawOrigin, drawScale * scaleOsc * 6, SpriteEffects.None, 0);
                spriteBatch.Draw(glowCircleTexture, drawPosition, null, glowDrawColor, NPC.rotation, glowCircleDrawOrigin, drawScale * scaleOsc * 6, SpriteEffects.None, 0);
            }
            return false;
        }
     
        private void DrawIncresionDiskBottom(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameBottom, columns: 5, frameWidth: 400, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Disk").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            float drawScale = NPC.scale  * _spawnScale * 1.75f;
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
           
            incresionDiskDrawColor = Color.Cyan;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, NPC.rotation, drawOrigin, drawScale * 1.5f, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Purple;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, NPC.rotation, drawOrigin, drawScale * 2, SpriteEffects.None, 0);
        }


        private void DrawIncresionDiskTop(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameTop, columns: 4, frameWidth: 480, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Top").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.15f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;

            float drawScale = NPC.scale * 3 * _spawnScale;
            float drawRotation = NPC.rotation;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        #endregion
    }
}
