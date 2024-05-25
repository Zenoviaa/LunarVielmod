using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Trails;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Sylia.Projectiles;
using Stellamod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Shaders;
using System;
using Terraria.Graphics.Effects;

namespace Stellamod.NPCs.Bosses.Sylia
{
    internal class VoidWall : ModNPC,
        IPixelPrimitiveDrawer
    {
        internal PrimitiveTrail BeamDrawer;

        private int _tooFarCounter = 0;
        private float _projSpeed = 3.5f;
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
        private const int Check_For_Pull_Radius = 2048;

        //AI
        private const int Idle_Time = 240;
        private const float Wall_Length = 2048;
        private const float Wall_Width = 2048;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = Body_Radius;
            NPC.height = Body_Radius * 16;
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
        private int _frameCounter2;
        private int _frameTick2;
        private float _blackProgress;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
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
                        SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                        _tooFar = true;
                    }
                }

                homingVelocity.Y = 0;
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


            _blackProgress += 0.01f;
            if(Main.netMode != NetmodeID.Server)
            {
                FilterManager filterManager = Terraria.Graphics.Effects.Filters.Scene;
                if (_blackProgress >= 1 && filterManager[ShaderRegistry.Screen_Black].IsActive())
                {
                    filterManager.Deactivate(ShaderRegistry.Screen_Black);
                }
                else
                {
                    filterManager[ShaderRegistry.Screen_Black].GetShader()
                     .UseProgress(_blackProgress);
                }
            }

            Movement();
            PullPlayer();


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
                        SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
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
                        SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
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

                        SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.position);
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
                        SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.position);
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
                        SoundEngine.PlaySound(SoundID.Item117, NPC.position);
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


        public float WidthFunction(float completionRatio)
        {
            return Wall_Width * MathF.Sin(completionRatio * 2 * VectorHelper.Osc(0.9f, 1f, 0.3f));
        }
        public float WidthFunction2(float completionRatio)
        {
            return Wall_Width * MathF.Sin(completionRatio * 1 * VectorHelper.Osc(0.9f, 1f, 0.3f)) * 0.68f;
        }


        public Color ColorFunction(float completionRatio)
        {
            if(completionRatio < 0.1f)
            {
                return ColorFunctions.MiracleVoid * ((1f - completionRatio) / 0.1f);
            }
            return ColorFunctions.MiracleVoid * MathF.Sin(completionRatio);
        }
        public Color ColorFunction2(float completionRatio)
        {
            if (completionRatio < 0.1f)
            {
                return Color.Black * ((1f - completionRatio) / 0.1f);
            }

            return ColorFunctions.MiracleVoid;
        }


        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Black);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.FadedStreak);

            List<Vector2> points = new();
            Vector2 velocity = Vector2.Zero;
            velocity.X = 0;
            velocity.Y = 1;

            Vector2 startPos = NPC.Center + new Vector2(0, Wall_Length / 2);
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(startPos, startPos - velocity * Wall_Length, i / 8f));
            }
            BeamDrawer.WidthFunction = WidthFunction2;
            BeamDrawer.ColorFunction = ColorFunction2;
            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);

            BeamDrawer.WidthFunction = WidthFunction;
            BeamDrawer.ColorFunction = ColorFunction;
            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);

            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 origin = new Vector2(58 / 2, 88 / 2);

            Texture2D voidMouthTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/Projectiles/VoidMouth").Value;
            int frameSpeed = 2;
            int frameCount = 6;
            Rectangle animationFrame = voidMouthTexture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, true);
            Main.EntitySpriteDraw(voidMouthTexture, drawPosition, animationFrame,
                Color.White, 0, origin, 2f, SpriteEffects.None, 0f);

            Vector2 drawOffset = new Vector2(-48, -128);
            Texture2D voidEaterTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/Projectiles/VoidWallEaterMini").Value;
            animationFrame = voidEaterTexture.AnimationFrame(ref _frameCounter2, ref _frameTick2, frameSpeed, frameCount, true);
            Vector2 voidEaterSize = new Vector2(52, 38);
            Vector2 voidEaterOrigin = voidEaterSize / 2;

            Main.EntitySpriteDraw(voidEaterTexture, drawPosition + drawOffset, animationFrame,
                Color.White, 0, voidEaterOrigin, VectorHelper.Osc(0.5f, 1f, 6f), SpriteEffects.None, 0f);

            drawOffset.Y *= -1;
            Main.EntitySpriteDraw(voidEaterTexture, drawPosition + drawOffset, animationFrame,
                Color.White, 0, origin, VectorHelper.Osc(0.5f, 1f, 6f), SpriteEffects.None, 0f);



            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
