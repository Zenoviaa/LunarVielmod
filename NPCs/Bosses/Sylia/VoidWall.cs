using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Sylia.Projectiles;
using Stellamod.Particles;
using Stellamod.Projectiles.Summons.VoidMonsters;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    internal class VoidWall : ModNPC
    {
        private int _particleCounter = 0;
        private int _tooFarCounter = 0;
        private float _projSpeed = 5.5f;
        private bool _tooFar;
        private bool _spawned;

        private enum AttackState
        {
            Idle,
            Void_Vomit_Telegraph,
            Void_Vomit,
            Void_Blast_Telegraph,
            Void_Blast,
            Void_Laser_Telegraph,
            Void_Laser,
            Void_Suck_Telegraph,
            Void_Suck
        }

        private const int Body_Radius = 64;
        private const int Body_Particle_Count = 10;
        private const int Body_Particle_Rate = 2;
        private const int Check_For_Pull_Radius = 2048;

        //AI
        private const int Idle_Time = 240;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Size = new Vector2(Body_Radius, Body_Radius * 16);
            NPC.damage = 100;
            NPC.defense = 66;
            NPC.lifeMax = 6666;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
  
            NPC.scale = 1f;
  
            // Take up open spawn slots, preventing random NPCs from spawning during the fight
            NPC.npcSlots = 10f;

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;
            NPC.boss = true;
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.SpawnWithHigherTime(30);
        }

        private int _frameCounter;
        private int _frameTick;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {        
            //Visual Stuffs
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
               new Vector3(60, 0, 118),
               new Vector3(117, 1, 187),
               new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, new Color(60, 0, 118), drawColor, 1);
            SpriteEffects effects = SpriteEffects.None;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 origin = new Vector2(58 / 2, 88 / 2);

            Texture2D voidMouthTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/Projectiles/VoidMouth").Value;
            int frameSpeed = 2;
            int frameCount = 6;
            Main.EntitySpriteDraw(voidMouthTexture, drawPosition,
                voidMouthTexture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, true),
                Color.White, 0, origin, 1f, effects, 0f);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        private void PullPlayer()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                bool isBehindWall = NPC.Center.X + Body_Radius > player.Center.X;
                if (isBehindWall && Vector2.Distance(NPC.Center, player.Center) <= Check_For_Pull_Radius)
                {
                    player.velocity.X += 1;
                }
            }
        }

        private void Movement()
        {

            if (!NPC.HasValidTarget)
                return;

            Player target = Main.player[NPC.target];

            ref float ai_State = ref NPC.ai[1];
            ref float ai_Cycle = ref NPC.ai[2];

            Vector2 targetCenter = target.Center;
            AttackState attackState = (AttackState)ai_State;
            Vector2 homingVelocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * _projSpeed;
            if(attackState == AttackState.Void_Suck)
            {
                homingVelocity = new Vector2(homingVelocity.X * 0.4f, homingVelocity.Y);
            }

            float tooFarDistance = 16 * 64;
            float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
            if (!_tooFar)
            {
                if (distanceToTarget > tooFarDistance)
                {
                    _tooFarCounter++;
                    if (_tooFarCounter > 30)
                    {
                        SoundEngine.PlaySound(SoundID.Roar);
                        _tooFar = true;
                    }
                }

                NPC.velocity = homingVelocity;
            }
            else
            {
                if (distanceToTarget < tooFarDistance)
                {
                    _tooFarCounter--;
                    if (_tooFarCounter <= 0)
                    {
                        _tooFar = false;
                    }
                }

                NPC.velocity = new Vector2(homingVelocity.X * 4f, homingVelocity.Y);
            }

            if (!NPC.AnyNPCs(ModContent.NPCType<Sylia>()) && NPC.active)
            {
                NPC.Kill();
            }
        }

        private void SuckVisuals()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 voidAbsorbPosition = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
                Vector2 speed = (NPC.Center - voidAbsorbPosition).SafeNormalize(Vector2.Zero) * 8;
                Particle p = ParticleManager.NewParticle(voidAbsorbPosition, speed, ParticleManager.NewInstance<VoidParticle>(),
                    default(Color), 0.25f);
                p.layer = Particle.Layer.BeforeProjectiles;

                float distance = 128;
                float particleSpeed = 4;
                Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                Vector2 dustSpeed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
                Dust.NewDustPerfect(position, DustID.GemAmethyst, dustSpeed, Scale: 0.5f);
            }
        }

        private void SwitchState(AttackState attackState)
        {
            ref float ai_Counter = ref NPC.ai[0];
            ref float ai_State = ref NPC.ai[1];
            ai_Counter = 0;
            ai_State = (float)attackState;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            ref float ai_Counter = ref NPC.ai[0];
            ref float ai_State = ref NPC.ai[1];
            ref float ai_Cycle = ref NPC.ai[2];

            if (!_spawned)
            {
                ai_Counter += Idle_Time;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<VoidTrailProj>(), 0, 0, Main.myPlayer, ai1: NPC.whoAmI);
                }

                _spawned = true;
            }

            Movement();
            PullPlayer();
            Visuals();


            if (!NPC.HasValidTarget)
            {
                return;
            }

            Player target = Main.player[NPC.target];
            Vector2 targetCenter = target.Center;
            AttackState attackState = (AttackState)ai_State;
            switch (attackState)
            {
                case AttackState.Idle:
                    ai_Counter++;
                    if (ai_Counter >= Idle_Time)
                    {
                        //Do thingy
                        if(ai_Cycle == 0)
                        {
                            SwitchState(AttackState.Void_Vomit_Telegraph);
                        } else if (ai_Cycle == 1)
                        {
                            SwitchState(AttackState.Void_Blast_Telegraph);
                        }
                        else if (ai_Cycle == 2)
                        {
                            SwitchState(AttackState.Void_Vomit_Telegraph);
                        } else if (ai_Cycle == 3)
                        {
                            SwitchState(AttackState.Void_Suck_Telegraph);
                        }

                        ai_Cycle++;
                        if(ai_Cycle > 3)
                        {
                            ai_Cycle = 0;
                        }
                    }

                    break;

                case AttackState.Void_Vomit_Telegraph:
                    //SUCK IN VOID
                    SuckVisuals();

                    ai_Counter++;
                    if(ai_Counter >= 60)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath13);
                        SwitchState(AttackState.Void_Vomit);
                    }

                    break;

                case AttackState.Void_Vomit:
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 velocity = (targetCenter - NPC.Center).SafeNormalize(Vector2.Zero) * 20;
                        int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                            ModContent.ProjectileType<VoidBall>(), 20, 1, Owner: Main.myPlayer);
                        Main.projectile[p].timeLeft *= 2;
                    }
    
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 512f, 32f);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash1"), NPC.Center);
                    SwitchState(AttackState.Idle);
                    break;

                case AttackState.Void_Blast_Telegraph:
                    SuckVisuals();

                    ai_Counter++;
                    if (ai_Counter >= 60)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath13);
                        SwitchState(AttackState.Void_Blast);
                    }
                    break;

                case AttackState.Void_Blast:
                    ai_Counter++;
                    if(ai_Counter % 30 == 0)
                    {
                        Vector2 voidBlastVelocity = (targetCenter - NPC.Center).SafeNormalize(Vector2.Zero) * 9.5f;
                        if (StellaMultiplayer.IsHost)
                        {
                            if (Main.rand.NextBool(2))
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, voidBlastVelocity,
                                    ModContent.ProjectileType<VoidWallEater>(), 60, 1, Owner: Main.myPlayer);
                            }
                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, voidBlastVelocity,
                                    ModContent.ProjectileType<VoidWallEaterMini>(), 60, 1, Owner: Main.myPlayer);
                            }
                        }

                        SoundEngine.PlaySound(SoundID.NPCDeath12);
                    }

                    if(ai_Counter >= 150)
                    {
                        SwitchState(AttackState.Idle);
                    }
              
                    break;

                case AttackState.Void_Laser_Telegraph:
                    SuckVisuals();

                    ai_Counter++;
                    if (ai_Counter >= 60)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath13);
                        SwitchState(AttackState.Void_Laser);
                    }
                    break;

                case AttackState.Void_Laser:
                    SwitchState(AttackState.Idle);
                    break;
                case AttackState.Void_Suck_Telegraph:
                    SuckVisuals();

                    ai_Counter++;
                    if (ai_Counter >= 60)
                    {
                        SoundEngine.PlaySound(SoundID.Item117);
                        SwitchState(AttackState.Void_Suck);
                    }
                    break;

                case AttackState.Void_Suck:
                    if (StellaMultiplayer.IsHost && ai_Counter == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                            ModContent.ProjectileType<VoidVortex>(), 0, 0, Owner: Main.myPlayer, NPC.whoAmI);
                    }
                
                    ai_Counter++;
                    if(ai_Counter >= 240)
                    {
                        SwitchState(AttackState.Idle);
                    }
                    break;
            }
        }


        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {
                for (int i = 0; i < Body_Particle_Count; i++)
                {
                    Vector2 position = NPC.Center + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
                    float size = Main.rand.NextFloat(0.75f, 1f);
                    Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), size);

                    p.layer = Particle.Layer.BeforeNPCs;
                    Particle tearParticle = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidTearParticle>(),
                        default(Color), size + 0.025f);

                    tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
                }

                for (int i = 0; i < Body_Particle_Count; i++)
                {
                    Vector2 position = NPC.RandomPositionWithinEntity();
                    float size = Main.rand.NextFloat(0.75f, 1f);
                    Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), size);

                    p.layer = Particle.Layer.BeforeNPCs;
                }

                _particleCounter = 0;
            }
        }
    }
}
