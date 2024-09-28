using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal class NiiviPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (NPC.AnyNPCs(ModContent.NPCType<Niivi>()))
            {
                ActivateSkye();

            }
            else
            {
                DeActivateSkye();
            }
        }

        private void ActivateSkye()
        {
            if (!SkyManager.Instance["Stellamod:NiiviSky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:NiiviSky", targetCenter);
                Main.shimmerDarken = 0.1f;
                Main.shimmerAlpha = 0.5f;
            }
 
        }

        private void DeActivateSkye()
        {
            if (SkyManager.Instance["Stellamod:NiiviSky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Deactivate("Stellamod:NiiviSky", targetCenter);
                Main.shimmerDarken = 0f;
                Main.shimmerAlpha = 0f;
            }

        }
    }

    [AutoloadBossHead]
    internal partial class Niivi : ModNPC
    {
        public enum ActionState
        {
            Spawn,
            Idle,
            Frost_Breath,
            Laser_Blast,
            Star_Wrath,
            Star_Storm,
            Thunderstorm,
            Baby_Dragons,
            Swoop_Out,
            PrepareAttack,
            Calm_Down,
            Transition_P2,
            Space_Circle,
            Laser_Blast_V2,
            Star_Wrath_V2,      
            Spare_Me,
            Spared
        }

        public ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = (float)value;
            }
        }


        //Damage Values
        private int P1_LightningDamage => 240;
        private int P1_FrostBreathDamage => 120;
        private int P1_StarWrathDamage => 70;
        private int P1_LaserDamage => 500;
        private int P1_StarStormDamage => 62;
        private int P1_CosmicBombDamage => 500;
        private int P2_VoidField => 72;

        //AI
        private bool _resetTimers;
        ref float Timer => ref NPC.ai[1];
        ref float AttackTimer => ref NPC.ai[2];
        public ActionState NextAttack
        {
            get => (ActionState)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }

        private float CrystalTimer;
        private float SpecialTimer;
        private int SpecialCycle;
        int ScaleDamageCounter;
        int AggroDamageCounter;
        int SpecialDamageCounter;

        private Player Target => Main.player[NPC.target];
        private IEntitySource EntitySource => NPC.GetSource_FromThis();
        private float DirectionToTarget
        {
            get
            {
                if (Target.position.X < NPC.position.X)
                    return -1;
                return 1;
            }
        }

        //Phase Switches
        private bool InPhase2 => NPC.life <= (NPC.lifeMax * 0.66f);
        private bool TriggeredPhase2;

        private int AttackCount;
        private int AttackSide;
        private int BreathingTimer;
        private bool DoAttack;
        private bool IsCharging;
        private Vector2 AttackPos;
        private Vector2 ChargeDirection;
        private Vector2 LaserAttackPos;

        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                Timer = 0;
                AttackTimer = 0;
                AttackCount = 0;
                _resetTimers = false;
            }
        }

        private void ResetTimers()
        {
            if (StellaMultiplayer.IsHost)
            {
                _resetTimers = true;
                NPC.netUpdate = true;
            }
        }


        private void ResetState(ActionState bossActionState)
        {
            State = bossActionState;
            ResetTimers();
            NPC.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(_resetTimers);
            writer.Write(ScaleDamageCounter);
            writer.Write(AggroDamageCounter);
            writer.Write(SpecialDamageCounter);
            writer.Write(CrystalTimer);
            writer.Write(SpecialTimer);
            writer.Write(SpecialCycle);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            _resetTimers = reader.ReadBoolean();
            ScaleDamageCounter = reader.ReadInt32();
            AggroDamageCounter = reader.ReadInt32();
            SpecialDamageCounter = reader.ReadInt32();
            CrystalTimer = reader.ReadSingle();
            SpecialTimer = reader.ReadSingle();
            SpecialCycle = reader.ReadInt32();
        }

        public override void SetStaticDefaults()
        {
            //Don't want her to be hit by any debuffs
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = Total_Segments;
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Niivi/NiiviPreview";
            drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Niivi, The First Dragon"))
            });
        }

        public override void SetDefaults()
        {
            //Stats
            NPC.lifeMax = 292000;
            NPC.defense = 160;
            NPC.damage = 290;
            NPC.width = (int)NiiviHeadSize.X;
            NPC.height = (int)NiiviHeadSize.Y;
            //It won't be considered a boss or take up slots until the fight actually starts
            //So the values are like this for now
            NPC.boss = true;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;

            //She'll tile collide and have gravity while on the ground, but not while airborne.
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.takenDamageMultiplier = 0.6f;

            NPC.BossBar = ModContent.GetInstance<NiiviBossBar>();
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Niivi");
            }

            Crystals = new NiiviCrystalDraw[3];
            Crystals[0] = new NiiviCrystalDraw(NPC, ModContent.Request<Texture2D>($"{BaseProjectileTexturePath}NiiviCrystalFrost").Value);
            Crystals[1] = new NiiviCrystalDraw(NPC, ModContent.Request<Texture2D>($"{BaseProjectileTexturePath}NiiviCrystalLightning").Value);
            Crystals[2] = new NiiviCrystalDraw(NPC, ModContent.Request<Texture2D>($"{BaseProjectileTexturePath}NiiviCrystalStars").Value);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //Return false for no Contact Damage
            if (State == ActionState.Star_Storm)
                return IsCharging;
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (InPhase2)
            {
                SpecialDamageCounter += hit.Damage;
            }

            int lifeToGiveIllurineScaleInBoss = NPC.lifeMax / 100;
            if (StellaMultiplayer.IsHost)
            {
                ScaleDamageCounter += hit.Damage;
                if (ScaleDamageCounter >= lifeToGiveIllurineScaleInBoss)
                {
                    Vector2 velocity = -Vector2.UnitY;
                    velocity *= Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, velocity,
                        ModContent.ProjectileType<NiiviScaleProj>(), 0, 1, Main.myPlayer);
                    ScaleDamageCounter = 0;
                }
            }
            if(State == ActionState.Spare_Me)
            {
                Timer = 0;
            }
            if(State != ActionState.Spare_Me && NPC.life <= NPC.lifeMax / 100f)
            {
                //Dying
                NPC.life = NPC.lifeMax / 100;
                SpecialTimer = 0;
                Black = false;
                ChargeCrystals = false;
                ResetShaders();
                AI_Phase3_Reset();
                ResetState(ActionState.Spare_Me);
            }
            if(NPC.life <= 0)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 2000, 32);
                SoundEngine.PlaySound(SoundRegistry.Niivi_Death, NPC.position);
                var entitySource = NPC.GetSource_Death();
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi1);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi2);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi3);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi2);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi3);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi4);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi5);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi6);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, GoreHelper.Niivi7);

                for (int i = 0; i < 150; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, speed * 17, Scale: 5f);
                    d.noGravity = true;

                    Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var da = Dust.NewDustPerfect(NPC.Center, DustID.WhiteTorch, speeda * 11, Scale: 5f);
                    da.noGravity = false;

                    Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
                    var dab = Dust.NewDustPerfect(NPC.Center, DustID.HallowedTorch, speeda * 30, Scale: 5f);
                    dab.noGravity = false;
                }

                if (StellaMultiplayer.IsHost)
                {
                    for(int i = 0; i < 8; i++)
                    {
                        Vector2 velocity = -Vector2.UnitY;
                        velocity *= Main.rand.NextFloat(8, 16);
                        velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, velocity,
                            ModContent.ProjectileType<NiiviScaleProj>(), 0, 1, Main.myPlayer);
                    }
                }
            }
        }

        public override void AI()
        {
            FinishResetTimers();
            AI_PhaseSwaps();

            if (!NPC.HasValidTarget)
            {
                //Despawn basically
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != ActionState.Calm_Down)
                {
                    ResetState(ActionState.Calm_Down);
                }
            }

            if(State != ActionState.Spawn)
            {
                Crystals[0].Draw = !NPC.AnyNPCs(ModContent.NPCType<NiiviCrystalFrost>());
                Crystals[1].Draw = !NPC.AnyNPCs(ModContent.NPCType<NiiviCrystalLightning>());
                Crystals[2].Draw = !NPC.AnyNPCs(ModContent.NPCType<NiiviCrystalStars>());
            }
            CrystalTimer++; 
            if (ChargeCrystals == false)
            {
                ChargeCrystalTimer -= 1 / 60f;
            }
            else
            {
                ChargeCrystalTimer += 1 / 60f;
            }
            ChargeCrystalTimer = MathHelper.Clamp(ChargeCrystalTimer, 0f, 1f);


            if (InPhase2)
            {
                SpecialTimer++;
                if (SpecialTimer == 2500)
                {
                    AI_Phase2_SpecialReset();
                }

                if(SpecialDamageCounter > (NPC.lifeMax / 2 / 3))
                {
                    SpecialTimer = 2500;
                    SpecialDamageCounter = 0;
                }
            }

            switch (State)
            {
                case ActionState.Spawn:
                    AI_Spawn();
                    break;
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Swoop_Out:
                    AI_SwoopOut();
                    break;
                case ActionState.PrepareAttack:
                    AI_PrepareAttack();
                    break;
                case ActionState.Frost_Breath:
                    AI_FrostBreath();
                    break;
                case ActionState.Laser_Blast:
                    AI_LaserBlast();
                    break;
                case ActionState.Star_Wrath:
                    AI_StarWrath();
                    break;
                case ActionState.Star_Storm:
                    AI_StarStorm();
                    break;
                case ActionState.Thunderstorm:
                    AI_Thunderstorm();
                    break;
                case ActionState.Calm_Down:
                    AI_CalmDown();
                    break;

                //Phase 2
                case ActionState.Transition_P2:
                    AI_Transition_P2();
                    break;
                case ActionState.Space_Circle:
                    AI_SpaceCircle();
                    break;
                case ActionState.Laser_Blast_V2:
                    AI_LaserBlast_V2();
                    break;
                case ActionState.Star_Wrath_V2:
                    AI_StarWrath_V2();
                    break;

                //Phase 3
                case ActionState.Spare_Me:
                    AI_SpareMe();
                    break;
                case ActionState.Spared:
                    AI_Spared();
                    break;
            }

            UpdateOrientation();
        }

        private void AI_PhaseSwaps()
        {
            //Trigger Phase 2
            if (InPhase2 && !TriggeredPhase2)
            {
                AI_Phase2_Reset();
                TriggeredPhase2 = true;
                return;
            }
        }


        private void ChargeVisuals<T>(float timer, float maxTimer) where T : Particle
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    ParticleManager.NewParticle<T>(pos, vel, Color.White, 1f);
                }
            }
        }

        #region Phase 1
        private void AI_Spawn()
        {
            NPC.TargetClosest();
            Timer++;
            if (Timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(EntitySource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<NiiviSpawnExplosionProj>(),
                        0, 0, Main.myPlayer);
                }

                for (int i = 0; i < Crystals.Length; i++)
                {
                    Crystals[i].Draw = true;
                }
                NPC.velocity = -Vector2.UnitY;
            }

            OrientArching();
            UpdateOrientation();
            LookAtTarget();
            FlipToDirection();
            //Slowdown over time
            float length = 720;
            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

            if (Timer >= 60)
            {
                NextAttack = ActionState.Frost_Breath;
                ResetState(ActionState.Idle);

            }
        }


        private void DefaultOrientation()
        {
            OrientArching();
            UpdateOrientation();
            FlipToDirection();
        }

        private void AI_Idle()
        {
            NPC.TargetClosest();
            Timer++;
            if (Timer >= 1)
            {
                ResetState(ActionState.PrepareAttack);
            }

            DefaultOrientation();
            NPC.velocity *= 0.98f;
        }

        private void AI_CalmDown()
        {
            Timer++;
            if (Timer >= 60)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NiiviRoaming>());
                }

                ResetShaders();
                NPC.active = false;
            }

            DefaultOrientation();
            NPC.velocity *= 0.98f;
        }

        private void AI_SwoopOut()
        {
            Timer++;
            if (Timer == 1)
            {
                OrientationSpeed = 0.03f;
                NPC.velocity = -Vector2.UnitY * 0.02f;
                SoundEngine.PlaySound(SoundRegistry.Niivi_WingFlap, NPC.position);
            }

            LookDirection = DirectionToTarget;
            DefaultOrientation();
            LookAtTarget();

            Vector2 targetCenter = Target.Center + new Vector2(DirectionToTarget * -256, -256);
            Vector2 idlePosition = targetCenter;

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10) * -DirectionToTarget;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            Vector2 vectorToIdlePosition = idlePosition - NPC.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();

            if (distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
         //       NPC.position = idlePosition;
           //     NPC.velocity *= 0.1f;
                //Projectile.netUpdate = true;
            }


            float speed;
            float inertia;

            // Minion doesn't have a target: return to player and idle
            if (distanceToIdlePosition > 100f)
            {
                // Speed up the minion if it's away from the player
                speed = 128;
                inertia = 200f;
            }
            else
            {
                // Slow down the minion if closer to the player
                speed = 3f;
                inertia = 100f;
            }

            if (distanceToIdlePosition > 20f)
            {
                // The immediate range around the player (when it passively floats about)
                // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                NPC.velocity = (NPC.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
            }
            else if (NPC.velocity == Vector2.Zero)
            {
                // If there is a case where it's not moving at all, give it a little "poke"
                NPC.velocity.X = -0.28f;
                NPC.velocity.Y = -0.14f;
            }

            if (Timer >= 30)
            {
                ResetState(ActionState.Idle);
            }
        }

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8)
        {
            //chase target
            Vector2 directionToTarget = NPC.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(NPC.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (NPC.velocity.X < targetVelocity.X)
            {
                NPC.velocity.X++;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X--;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y--;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        private void AI_PrepareAttack()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_WingFlap, NPC.position);
                DoAttack = false;
            }

            if (Timer % 60 == 0)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_WingFlap, NPC.position);
            }

            //Initialize Attack
            NPC.TargetClosest();
            LookDirection = DirectionToTarget;
            OrientArching();
            FlipToDirection();
            if (NPC.position.X > Target.position.X)
            {
                AttackSide = 1;
            }
            else
            {
                AttackSide = -1;
            }

            //Rotate Head
            TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

            if (AttackTimer == 0)
            {
                Vector2 targetCenter = AttackPos = Target.Center + new Vector2(DirectionToTarget * -256, -256);
                Vector2 idlePosition = targetCenter;

                // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
                // The index is projectile.minionPos
                float minionPositionOffsetX = (10) * -DirectionToTarget;
                idlePosition.X += minionPositionOffsetX; // Go behind the player

                // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

                // Teleport to player if distance is too big
                Vector2 vectorToIdlePosition = idlePosition - NPC.Center;
                float distanceToIdlePosition = vectorToIdlePosition.Length();

                if (distanceToIdlePosition > 2000f)
                {
                    // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                    // and then set netUpdate to true
                    //       NPC.position = idlePosition;
                    //     NPC.velocity *= 0.1f;
                    //Projectile.netUpdate = true;
                }


                float speed;
                float inertia;

                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 100f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 80;
                    inertia = 150;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 3f;
                    inertia = 100f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)
                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    NPC.velocity = (NPC.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (NPC.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    NPC.velocity.X = -0.28f;
                    NPC.velocity.Y = -0.14f;
                }
                if (Timer >= 360 || Vector2.Distance(NPC.Center, AttackPos) <= 128)
                {
                    DoAttack = true;
                }
            }


            if (DoAttack)
            {
                AttackTimer++;
                NPC.velocity *= 0.6f;
                if (SpecialTimer >= 2500 && AttackTimer >= 30)
                {
                    switch (SpecialCycle)
                    {
                        case 0:
                            ResetState(ActionState.Laser_Blast_V2);
                            SpecialCycle = 1;
                            break;
                        case 1:
                            ResetState(ActionState.Star_Wrath_V2);
                            SpecialCycle = 2;
                            break;
                        case 2:
                            ResetState(ActionState.Space_Circle);
                            SpecialCycle = 0;
                            break;
                    }
                    SpecialTimer = 0;
                }
                else if (AttackTimer >= 30)
                {
                    ResetState(NextAttack);
                }
            }
        }

        private void AI_LaserBlast()
        {
            /*
             * Step 1: Look at target for a bit
             * 
             * Step 2: Charge begin charging massive laser, white particles come in and slowly build up
             * 
             * Step 3: Fire the laser, twice?, Nah maybe three times
             */

            LookDirection = DirectionToTarget;
            FlipToDirection();
            if (AttackTimer == 0)
            {
                Timer++;
                NPC.velocity *= 0.8f;
                              //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                if(Timer == 1)
                {
                    ChargeCrystals = true;
                    SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlastReady, NPC.position);
                }

                float progress = Timer / 60;
                progress = MathHelper.Clamp(progress, 0, 1);
                float sparkleSize = MathHelper.Lerp(0f, 4f, progress);
                if (Timer % 4 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle p = ParticleManager.NewParticle(NPC.Center, Vector2.Zero,
                            ParticleManager.NewInstance<GoldSparkleParticle>(), Color.White, sparkleSize);
                        p.timeLeft = 8;
                    }
                }
                
                if(Timer > 61)
                {
                    NPC.velocity *= 0.98f;
                }

                //Rotate head 90-ish degrees upward
                if (Timer < 60)
                {
              
                   
                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    LaserAttackPos = Target.Center + directionToTarget * 384;
                    TargetHeadRotation = directionToTarget.ToRotation();

                    float distanceToTarget = Vector2.Distance(Target.Center, NPC.Center);
                    if(distanceToTarget > 128)
                    {
                        AI_MoveToward(Target.Center, 3);
                        //Slowly accelerate up while charging
                        NPC.velocity *= 1.002f;
                    }
                    else
                    {
                        NPC.velocity *= 0.98f;
                    }
         

                    //Charge up
                    ChargeVisuals<StarParticle2>(Timer, 60);
                }
                else if (Timer == 61)
                {
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, LaserAttackPos, velocity, type,
                            0, 0, Main.myPlayer);
                    }
                }
                else if (Timer >= 120)
                {
                    ChargeCrystals = false;
                    //SHOOT LOL
                    Vector2 fireDirection = TargetHeadRotation.ToRotationVector2();
                    float distance = Vector2.Distance(NPC.Center, LaserAttackPos);

                    int type = ModContent.ProjectileType<NiiviLaserBlastProj>();
                    int damage = P1_LaserDamage;
                    float knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        float size = 5.5f;
                        float beamLength = distance;
                        Projectile.NewProjectile(EntitySource, NPC.Center, fireDirection, type,
                        damage, knockback, Main.myPlayer,
                            ai0: size,
                            ai1: beamLength);
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 particleVelocity = fireDirection * 16;
                        particleVelocity = particleVelocity.RotatedByRandom(MathHelper.PiOver4 / 3);
                        particleVelocity *= Main.rand.NextFloat(0.3f, 1f);
                        ParticleManager.NewParticle(NPC.Center, particleVelocity,
                            ParticleManager.NewInstance<StarParticle>(), Color.White, sparkleSize);
                    }

                    Timer = 0;
                    AttackCount++;
                }

                if (AttackCount >= 3)
                {
                    ChargeCrystals = false;
                    NextAttack = ActionState.Star_Wrath;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_FrostBreath()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            /*
            //Ok so how is this attack going to work
            Step 1: Niivi takes aim at you for a few seconds, then rotates her head 135 degrees upward
            
            Step 2: Snowflake and snow particles circle around into her magic sigil thing
            
            Step 3: Then they form a frosty rune/sigil thingy
            
            Step 4: A second or two later, Niivi starts breathing the ice breath while slowly rotating her head
            
            Step 5: The attack goes 180 degrees, or a little more, so you'll need to move behind her
            
            Step 6: The breath spews out a windy looking projectile that quickly expands and dissipates
            
            Step 7: Stars and snowflake particles also come out
            
            Step 8: The screen is tinted blue/white during this attack (shader time!)
            
            Step 9: When the breath collides with tiles (including platforms if possible), 
                there is a chance for it to form large icicles, these are NPCs, you can break them
                they have a snowy aura

            Step 10: Niivi stops breathing once she reaches the edge of her range,
                she turns around towards you and fires three frost blasts while slowly flying away

            Step 11: The frost blasts travel a short distance, (slowing down over time)
                when they come to complete stop, they explode into icicles that are affected by gravity

            Step 12: Niivi flies away to decide a new attack
           
            In Phase 2:
                Niivi does frost balls before doing the breath, and rotates her head slightly faster
            */


            if (AttackTimer == 0)
            {
                //Taking aim
                Timer++;
                ChargeCrystals = true;
                shaderSystem.VignetteScreen(-0.5f, timer: 60);
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                //Rotate head 90-ish degrees upward
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);

                float targetRotation = MathHelper.PiOver2 * -AttackSide;
                Vector2 rotatedDirection = directionToTarget.RotatedBy(targetRotation);
                TargetHeadRotation = rotatedDirection.ToRotation();

                //Slowly accelerate up while charging
                NPC.velocity *= 1.002f;

                Timer++;
                if (Timer == 1)
                {
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                        0, 0, Main.myPlayer);
                    }
                }

                //Charge up
                ChargeVisuals<SnowFlakeParticle>(Timer, 60);
                if (Timer >= 60)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        int type = ModContent.ProjectileType<NiiviFrostCircleProj>();
                        int damage = 0;
                        int knockback = 0;
                        Projectile.NewProjectile(EntitySource, NPC.Center, Vector2.Zero,
                            type, damage, knockback, Main.myPlayer);

                        NPC.NewNPC(EntitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NiiviCrystalFrost>());
                    }

                    Timer = 0;
                    AttackTimer++;
                    NPC.velocity = -Vector2.UnitY;
                }
            }
            else if (AttackTimer == 2)
            {
                ChargeCrystals = false;

                //Get the shader system!

                shaderSystem.TintScreen(Color.Cyan, 0.1f, timer: 720);
                shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f, timer: 720);
                shaderSystem.VignetteScreen(-1f, timer: 720);

                //Slowdown over time
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

                //Re-orient incase target went behind
                LookDirection = DirectionToTarget;
                OrientArching();
                FlipToDirection();

                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                TargetHeadRotation = directionToTarget.ToRotation();
                Timer++;
                if (Timer >= length)
                {

                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 3)
            {
                //Untint the screen
                ResetShaders();

                Timer++;
                if (Timer == 1)
                {
                    //Retarget, just incase target died ya know
                    NPC.TargetClosest();

                    //Re-orient incase target went behind
                    LookDirection = DirectionToTarget;
                    OrientArching();
                    FlipToDirection();

                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();
                }

                if (Timer >= 30)
                {
                    float speed = 24;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * speed;

                    //Add some random offset to the attack
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    int type = ModContent.ProjectileType<NiiviFrostBombProj>();

                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                    velocity *= Main.rand.NextFloat(0.5f, 1f);

                    int damage = P1_FrostBreathDamage;
                    float knockback = 1;

                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, spawnPos, velocity, type,
                        damage, knockback, Main.myPlayer);
                    }

                    Timer = 0;
                    AttackCount++;
                }

                if (AttackCount >= 6)
                {
                    Timer = 0;
                    AttackCount = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 4)
            {
                NextAttack = ActionState.Laser_Blast;
                ResetState(ActionState.Swoop_Out);    
            }
        }

        private void AI_StarWrath()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            LookDirection = DirectionToTarget;
            DefaultOrientation();
            if (AttackTimer == 0)
            {
                Timer++;
                if (Timer == 1)
                {
                    shaderSystem.VignetteScreen(1);
                }
                ChargeCrystals = true;
                NPC.velocity *= 0.8f;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnCenter = Target.Center + new Vector2(0, -128);
                        NPC.NewNPC(EntitySource, (int)spawnCenter.X, (int)spawnCenter.Y, ModContent.NPCType<NiiviCrystalStars>());
                    }
                }
            }
            else if (AttackTimer == 1)
            {
                ChargeCrystals = false;
                Timer++;
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                //Slowdown over time
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

                if (Timer >= 360)
                {
                    NextAttack = ActionState.Star_Storm;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarStorm()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            NPC.rotation = 0;
            LookDirection = DirectionToTarget;
            DefaultOrientation();
            LookAtTarget();
            if (AttackTimer == 0)
            {
                Timer++;
                if(Timer == 1)
                {
                    shaderSystem.VignetteScreen(1, timer: 60);
                }

                //Slowdown over time
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

                ChargeCrystals = true;
                if(Timer >= 60)
                {
      
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                if(Timer % 30 == 0)
                {
                    shaderSystem.TintScreen(Color.White, 0.2f, timer: 15);
                    NPC.velocity = -NPC.Center.DirectionTo(Target.Center) * 16;
                    if (StellaMultiplayer.IsHost)
                    {
                        float speed = Main.rand.NextFloat(24, 42);
                        Vector2 spawnCenter = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                        Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                        Vector2 velocityToTarget = directionToTarget * speed;
                        Projectile.NewProjectile(EntitySource, spawnCenter, velocityToTarget, 
                            ModContent.ProjectileType<NiiviStarBounceProj>(), P1_StarStormDamage, 1, Main.myPlayer);
                    }
                }
                else
                {
                    NPC.velocity *= 0.92f;
                }

                if(Timer >= 120)
                {
                    Timer = 0;
                    AttackTimer++;
                }
            }     
            else if (AttackTimer == 2)
            {
                ChargeCrystals = false;
                Timer++;
                if(Timer >= 60)
                {
                    NextAttack = ActionState.Thunderstorm;
                    ResetState(ActionState.Swoop_Out);
                }
            }     
        }

        private void AI_Thunderstorm()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            LookDirection = DirectionToTarget;
            DefaultOrientation();
            //Aight, this shouldn't be too hard to do
            //She flies up and rains down lightning
            if (AttackTimer == 0)
            {
                Timer++;
                ChargeCrystals = true;
                NPC.velocity *= 0.8f;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                shaderSystem.TintScreen(Color.Black, 0.5f, timer: 450);
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                NPC.velocity *= 1.05f;
                if (Timer >= 60)
                {
                    Timer = 0;
                    AttackTimer++;
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnCenter = Target.Center + new Vector2(0, -128);
                        NPC.NewNPC(EntitySource, (int)spawnCenter.X, (int)spawnCenter.Y, ModContent.NPCType<NiiviCrystalLightning>());
                    }

                }
            }
            else if (AttackTimer == 2)
            {
                ChargeCrystals = false;
                NPC.velocity *= 0.98f;
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                if (Timer == 1)
                {
                    LaserAttackPos = Target.Center;
                }

                if (Timer >= 90)
                {
                    AttackCount++;
                    Timer = 0;
                }
                if (AttackCount >= 4)
                {
                    AttackTimer++;
                    Timer = 0;
                }
            }
            else if (AttackTimer == 3)
            {
                NPC.velocity *= 0.98f;
                Timer++;
                if (Timer >= 90)
                {
                    shaderSystem.UnTintScreen();
                    NextAttack = ActionState.Frost_Breath;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        #endregion

        #region Phase 2
        private void AI_Phase2_Reset()
        {
            ResetShaders();
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.TintScreen(Color.White, 0.5f, timer: 15);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetState(ActionState.Transition_P2);
            SpecialTimer = 0;
            SpecialCycle = 0;
        }

        private void AI_Phase2_SpecialReset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.TintScreen(Color.White, 0.5f, timer: 15);
            SoundStyle soundStyle = SoundRegistry.Niivi_PrismaticCharge;
            SoundEngine.PlaySound(soundStyle, NPC.position);
        }

        private void AI_Transition_P2()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.velocity = Vector2.Zero;
                NPC.defense *= 8;
            }
            ChargeCrystals = false;
            NPC.velocity.Y += 0.002f;
            OrientArching();
            UpdateOrientation();
            if(Timer >= 450)
            {
                for (int i = 0; i < 150; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, speed * 17, Scale: 5f);
                    d.noGravity = true;

                    Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var da = Dust.NewDustPerfect(NPC.Center, DustID.WhiteTorch, speeda * 11, Scale: 5f);
                    da.noGravity = false;

                    Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
                    var dab = Dust.NewDustPerfect(NPC.Center, DustID.HallowedTorch, speeda * 30, Scale: 5f);
                    dab.noGravity = false;
                }

                NPC.defense /= 8;
                ResetState(ActionState.Swoop_Out);        
            }
        }

        private void AI_SpaceCircle()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
      
            if (AttackTimer == 0)
            {
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);
                TargetHeadRotation = -MathHelper.PiOver4;
                OrientArching();
                if (FlightDirection == -1)
                {
                    TargetHeadRotation = -MathHelper.PiOver4 * 3;
                }

                UpdateOrientation();
                FlipToDirection();
                ChargeCrystals = true;
                Black = true;
                Timer++;
                if (Timer == 1)
                {
                    NPC.velocity = -Vector2.UnitY;
                }

                float progress = Timer / 120f;
                progress = MathHelper.Clamp(progress, 0, 1);
                float sparkleSize = MathHelper.Lerp(0f, 4f, progress);
                Vector2 pos = NPC.Center + HeadRotation.ToRotationVector2() * 256;
                if (Timer % 4 == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Particle p = ParticleManager.NewParticle(pos, Vector2.Zero,
                            ParticleManager.NewInstance<GoldSparkleParticle>(), Color.White, sparkleSize);
                        p.timeLeft = 8;
                    }
                }

                if (Timer % 16 == 0)
                {
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(pos, 1024, 16);
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, Vector2.Zero,
                            ModContent.ProjectileType<NiiviCosmicBombAbsorbProj>(), 0, 0, Main.myPlayer);
                    }
      
                    screenShaderSystem.TintScreen(Color.Black, 0.3f, timer: 7);
                }

                if (Timer >= 120)
                {
                    screenShaderSystem.VignetteScreen(1f, timer: 560);
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviStarFieldProj>();
                    int damage = P2_VoidField;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center + HeadRotation.ToRotationVector2() * 256, velocity, type,
                            damage, 0, Main.myPlayer);
                    }
                    AttackTimer++;
                    Timer = 0;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                OrientArching();
                UpdateOrientation();
                LookAtTarget();
                FlipToDirection();
                //Slowdown over time
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);
                if (Timer >= 680)
                {
                    ResetShaders();
                    NPC.velocity = -Vector2.UnitY;
                    AttackTimer++;
                    Timer = 0;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_LaserBlast_V2()
        {
            /*
            * Step 1: Look at target for a bit
            * 
            * Step 2: Charge begin charging massive laser, white particles come in and slowly build up
            * 
            * Step 3: Fire the laser, twice?, Nah maybe three times
            */

            if (AttackTimer == 0)
            {
                Timer++;
                NPC.velocity *= 0.8f;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                float progress = Timer / 60;
                progress = MathHelper.Clamp(progress, 0, 1);
                float sparkleSize = MathHelper.Lerp(0f, 4f, progress);
                if (Timer % 4 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle p = ParticleManager.NewParticle(NPC.Center, Vector2.Zero,
                            ParticleManager.NewInstance<GoldSparkleParticle>(), Color.White, sparkleSize);
                        p.timeLeft = 8;
                    }
                }

                //Rotate head 90-ish degrees upward
                if (Timer < 60)
                {
                    LaserAttackPos = Target.Center;
                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();

                    //Slowly accelerate up while charging
                    NPC.velocity *= 1.002f;

                    //Charge up
                    ChargeVisuals<StarParticle2>(Timer, 60);
                }
                else if (Timer == 61)
                {
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, LaserAttackPos, velocity, type,
                            0, 0, Main.myPlayer);
                    }
                }
                else if (Timer == 120)
                {
                    //SHOOT LOL
                    Vector2 fireDirection = TargetHeadRotation.ToRotationVector2();
                    int type = ModContent.ProjectileType<NiiviLaserContinuousProj>();
                    int damage = P1_LaserDamage;
                    float knockback = 1;
                    NPC.rotation = HeadRotation;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, fireDirection, type,
                        damage, knockback, Main.myPlayer, ai1: NPC.whoAmI);
                    }


                    ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    shaderSystem.TintScreen(Color.White, 0.3f, timer: 600);
                }
                else if (Timer >= 120)
                {
                    NPC.velocity *= 0.98f;
                  //  TargetHeadRotation += 0.02f;
                    NPC.rotation += 0.02f;
                }

                if (Timer >= 720)
                {
                    ResetShaders();
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarWrath_V2()
        {
            if(AttackTimer == 0)
            {
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);
                TargetHeadRotation = -MathHelper.PiOver4;
                OrientArching();
                UpdateOrientation();
                FlipToDirection();

                Timer++;
                if(Timer == 1)
                {
                    NPC.velocity = -Vector2.UnitY;
                }
                if(Timer >= 60)
                {
                    AttackTimer++;
                    Timer = 0;
                }
            } else if(AttackTimer == 1)
            {
                ChargeCrystals = true;
                Black = true;

                NPC.velocity = -Vector2.UnitY * 0.02f;
                NPC.velocity *= 0.8f;
                Timer++;
                OrientArching();
                if(FlightDirection == -1)
                {
                    TargetHeadRotation = -MathHelper.PiOver4 * 3;
                }
         
                UpdateOrientation();
                FlipToDirection();
                if(Timer == 1) 
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center + HeadRotation.ToRotationVector2() * 256, Vector2.Zero,
                            ModContent.ProjectileType<NiiviCosmicBombProj>(), P1_CosmicBombDamage, 1, Main.myPlayer);
                    }
                }

                if(Timer >= 480)
                {
                    NPC.velocity = -Vector2.UnitY;
                    AttackTimer++;
                    Timer = 0;
                }
            } else if (AttackTimer == 2)
            {
                ChargeCrystals = false;
                Black = false;

                Timer++;
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

                if(Timer >= 600)
                {
                    ResetShaders();
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }
        #endregion

        #region Phase 3
        private void AI_Phase3_Reset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.TintScreen(Color.White, 0.3f, timer: 15);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetShaders();
            NextAttack = ActionState.Spare_Me;
            ResetState(ActionState.Swoop_Out);
      
        }

        private void AI_SpareMe()
        {
            //Vignette
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float progress = distanceToTarget / 2000f;
            progress = 1f - progress;

            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.VignetteScreen(progress * 2.5f, timer: 450);

            float length = 450;
            SpecialTimer++;
            if(SpecialTimer < length)
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage  = false;
                SpecialTimer = length;
            }

            BreathingTimer++;
            if(BreathingTimer == 1)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_Tired, NPC.position);
            }

            if(BreathingTimer % 150 == 0)
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_HeavyBreathing1, NPC.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_HeavyBreathing2, NPC.position);
                        break;
                }
            }

            Timer++;
            if(Timer == 1)
            {

            }
            NPC.velocity *= 0.8f;
            LookDirection = DirectionToTarget;
            DefaultOrientation();
            //Put huffing and puffing sounds here
            if(Timer >= 1350)
            {
                ResetShaders();
                ResetState(ActionState.Spared);
            }
        }

        private void AI_Spared()
        {
            LookDirection = DirectionToTarget;
            DefaultOrientation();
            Timer++;
            if(Timer == 1)
            {
                NPC.velocity = -Vector2.UnitY;
                if (StellaMultiplayer.IsHost)
                {
                    int itemIndex = Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(),
                        ModContent.ItemType<IridineNecklace>(), Main.rand.Next(1, 1));
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
                }

            }
            NPC.dontTakeDamage = true;
            NPC.velocity *= 1.05f;
            TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
            if(Timer >= 120)
            {
                if (!DownedBossSystem.downedNiiviBoss)
                {
                    NPC.SetEventFlagCleared(ref DownedBossSystem.downedNiiviBoss, -1);
                }

    
                NPC.active = false;
            }
        }
        #endregion

        public override void OnKill()
        {
            ResetShaders();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedNiiviBoss, -1);
        }

        private void ResetShaders()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.UnTintScreen();
            shaderSystem.UnDistortScreen();
            shaderSystem.UnVignetteScreen();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PureHeart>(), minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PureHeart>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<NiiviBossRel>()));
        }
    }
}
