using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Animations;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc
{
    internal class HavocSegment
    {
        public string TexturePath;
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath).Value;
        public Texture2D GlowTexture => ModContent.Request<Texture2D>(TexturePath + "_Glow").Value;
        public Vector2 Size;
        public Vector2 Position;
        public Vector2 Center => Position + Size / 2;
        public Vector2 Velocity;
        public float Rotation;
    }

    internal class Havoc : ModNPC
    {
        //Damage Values
        private int ChargeDamage => 300;
        private int LaserMiniDamage => 200;
        private int LaserBigDamage => 500;

        public enum ActionState
        {
            Idle,
            Charge,
            Laser,
            Laser_Big
        }

        //AI
        ActionState State
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

        float Timer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        float AttackTimer
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        float LaserAttackDistance;
        float OrbitDistance = 500;
        Vector2 ArenaCenter;

        //Draw Code
        //Segment Positions;
        HavocSegment[] Segments;

        float SegmentStretch = 2;
        float TargetSegmentStretch = 2;
        float ChargeTrailOpacity;
        bool DrawChargeTrail;


        //Attacks
        //Charge
        Vector2 ChargeDirection;

        //Textures
        public const string BaseTexturePath = "Stellamod/NPCs/Bosses/IrradiaNHavoc/Havoc/";
        private HavocSegment Head => Segments[0];
        private HavocSegment BodyFront => Segments[1];
        private HavocSegment BodyMiddle => Segments[2];
        private HavocSegment BodyBack => Segments[3];
        private HavocSegment BodyTail => Segments[4];

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 32;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 90;
            NPC.lifeMax = 1000;
            NPC.damage = ChargeDamage;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            if(Segments == null)
            {
                //Initialize Segments
                Segments = new HavocSegment[5];
                for (int i = 0; i < Segments.Length; i++)
                {
                    Segments[i] = new HavocSegment();
                    Segments[i].Position = NPC.position;
                    Segments[i].Rotation = 0;
                    Segments[i].Velocity = Vector2.Zero;
                }

                //Set the textures
                Segments[0].TexturePath = Texture;
                Segments[0].Position = Segments[0].Position + new Vector2(0.98f, 0);
                Segments[0].Size = new Vector2(170, 90);
                Segments[1].TexturePath = $"{BaseTexturePath}HavocSegmentFront";
                Segments[1].Position = Segments[1].Position + new Vector2(1, 0);
                Segments[1].Size = new Vector2(54, 74);
                Segments[2].TexturePath = $"{BaseTexturePath}HavocSegmentMiddle";
                Segments[2].Position = Segments[2].Position + new Vector2(1, 0);
                Segments[2].Size = new Vector2(54, 74);
                Segments[3].TexturePath = $"{BaseTexturePath}HavocSegmentBack";
                Segments[3].Position = Segments[3].Position + new Vector2(1, 0);
                Segments[3].Size = new Vector2(54, 78);
                Segments[4].TexturePath = $"{BaseTexturePath}HavocTail";
                Segments[4].Position = Segments[4].Position + new Vector2(1, 0);
                Segments[4].Size = new Vector2(136, 58);
                return;
            }

            if (ArenaCenter == default(Vector2) && StellaMultiplayer.IsHost)
            {
                ArenaCenter = NPC.position;
                NPC.netUpdate = true;
            }

            switch (State)
            {
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Charge:
                    AI_Charge();
                    break;
                case ActionState.Laser:
                    AI_Laser();
                    break;
                case ActionState.Laser_Big:
                    AI_LaserBig();
                    break;
            }

            //This controls how far apart the segments are
            //Set to 1 if you want them to be touching each other, any number bigger than 1 spaces them out,
            //Smaller than 1 makes them overlap


            //Set head position and rotation
            //If using the worm like movement, don't forget to set the head position and rotation before calling that MoveSegments function
            UpdateSegmentStretch();
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(LaserAttackDistance);
            writer.WriteVector2(ArenaCenter);
            writer.Write(OrbitDistance);
            if (Segments == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(Segments.Length);
                for (int i = 0; i < Segments.Length; i++)
                {
                    writer.WriteVector2(Segments[i].Position);
                    writer.WriteVector2(Segments[i].Velocity);
                }
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            LaserAttackDistance = reader.ReadSingle();
            ArenaCenter = reader.ReadVector2();
            OrbitDistance = reader.ReadSingle();
            bool hasSegments = reader.ReadBoolean();
            if (hasSegments)
            {
                int length = reader.ReadInt32();
                for (int i = 0; i < length; i++)
                {
                    Vector2 pos = reader.ReadVector2();
                    Vector2 vel = reader.ReadVector2();
                    if (Segments != null && Segments.Length <= length)
                    {
                        Segments[i].Position = pos;
                        Segments[i].Velocity = vel;
                    }
                }
            }
        }

        private void ResetState(ActionState state)
        {
            State = state;
            Timer = 0;
            AttackTimer = 0;
        }

        private ActionState ReceiveSignal()
        {
            foreach(NPC npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<HavocSignal>())
                    continue;

                float signal = npc.ai[0];
                ActionState actionState = (ActionState)signal;
                npc.Kill();
                return actionState;
            }

            return ActionState.Idle;
        }

        private void AI_HandleSignal()
        {
            ActionState signal = ReceiveSignal();
            if (signal != ActionState.Idle)
            {
                ResetState(signal);
                NPC.netUpdate = true;
            }
        }

        private void AI_Idle()
        {
            DrawChargeTrail = false;
            NPC.TargetClosest();
            
            OrbitDistance+=100;
            if(OrbitDistance >= 500)
            {
                OrbitDistance = 500;
            }

            AI_MoveInOrbit();
            AI_HandleSignal();

            NPC.rotation = NPC.velocity.ToRotation();
            TargetSegmentStretch = 2;
            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        private void AI_MoveInOrbit()
        {
            //Orbit Around
            Vector2 direction = ArenaCenter.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / 720);
            Vector2 targetCenter = ArenaCenter + direction * OrbitDistance;
            AI_MoveToward(targetCenter, 4);
        }

        private void UpdateSegmentStretch()
        {
            SegmentStretch = MathHelper.Lerp(SegmentStretch, TargetSegmentStretch, 0.04f);
        }


        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return State == ActionState.Charge;
        }

        private void AI_Charge()
        {
            Player target = Main.player[NPC.target];
            Timer++;
            if (Timer < 100)
            {
                if (Timer == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HavocCharge"), NPC.position);
                    NPC.TargetClosest();
                }

                //Ease in
                TargetSegmentStretch = 1f;
                ChargeDirection = NPC.Center.DirectionTo(target.Center);

                AI_MoveInOrbit();
                NPC.velocity *= 0.8f;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, ChargeDirection.ToRotation(), 0.08f);
            }
            else if (Timer < 150)
            {
                ChargeDirection = NPC.Center.DirectionTo(target.Center);
                AI_MoveInOrbit();
                NPC.velocity *= 0.3f;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, ChargeDirection.ToRotation(), 0.08f);
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    NPC.oldPos[i] = NPC.position;
                }
            }
            else if (Timer < 180)
            {
                DrawChargeTrail = true;
          

                if (Timer == 151)
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RekRoar");
                    SoundEngine.PlaySound(soundStyle, NPC.position);
                }
                NPC.velocity = ChargeDirection * 40;
            }
            else if (Timer < 240)
            {
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / 60);
                NPC.velocity *= 0.96f;
                NPC.rotation = NPC.velocity.ToRotation();
                TargetSegmentStretch = 1;
            }
            else
            {
  
                ResetState(ActionState.Idle);
            }

            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
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

        private void AI_Laser()
        {
            Vector2 arenaLeft = ArenaCenter + new Vector2(-4000, 0);
            if (AttackTimer == 0)
            {  
                float distanceToLeft = Vector2.Distance(NPC.Center, arenaLeft);
                AI_MoveToward(arenaLeft, 32);
                if (distanceToLeft <= 16)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        LaserAttackDistance = Main.rand.NextFloat(16, 750);
                        TargetSegmentStretch = Main.rand.NextFloat(2, 5);
                        NPC.netUpdate = true;
                    }
        
            
                    AttackTimer++;
                }
            } 
            else if (AttackTimer == 1)
            {
                //Delay before he charges
                NPC.velocity *= 0.8f;
                Timer++;
                if(Timer >= 45)
                {
                    AttackTimer++;
                    Timer = 0;
                }
            } 
            else if (AttackTimer == 2)
            {
                float speed = 32;
                Vector2 arenaCenterOffset = ArenaCenter + new Vector2(288, 0);
                float distanceToTarget = Vector2.Distance(NPC.Center, arenaCenterOffset);
                AI_MoveToward(arenaCenterOffset, speed);
                if(distanceToTarget <= LaserAttackDistance)
                {
                    AttackTimer++;
                    Timer = 0;
                }
            } 
            else if (AttackTimer == 3)
            {
                if(NPC.velocity.Length() <= 1f)
                {
                    NPC.velocity = Vector2.UnitX;
                }
                else
                {
                    NPC.velocity *= 0.94f;
                }
                
            
                Timer++;

                if(Timer == 1)
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_WaveCharge");
                    SoundEngine.PlaySound(soundStyle, NPC.position);


                }
      
                //Visuals
                for (int i = 1; i < Segments.Length- 1; i++)
                {
                    var segment = Segments[i];
                    float progress = Timer / 60;
                    float minParticleSpawnSpeed = 8;
                    float maxParticleSpawnSpeed = 2;
                    int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
                    if (Timer % particleSpawnSpeed == 0)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            Vector2 pos = segment.Center + Main.rand.NextVector2CircularEdge(168, 168);
                            Vector2 vel = (segment.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                            Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, 0, Color.OrangeRed);
                        }
                    }
                }

                if(Timer >= 60)
                {
                    AttackTimer++;
                    Timer = 0;
                }
            } 
            else if (AttackTimer == 4)
            {
                Timer++;
                if(Timer == 0)
                {
                    NPC.velocity = Vector2.UnitX;
        
                }

                if(Timer == 1)
                {
                    for (int i = 1; i < Segments.Length - 1; i++)
                    {
                        var segment = Segments[i];
                        Vector2 velocity = Vector2.UnitY;
                        if (StellaMultiplayer.IsHost)
                        {
                            int type = ModContent.ProjectileType<HavocLaserWarnProj>();
                            Vector2 pos = segment.Center;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, velocity,
                                type, LaserMiniDamage, 0, Main.myPlayer);
                        }
                    }
                }

               
                NPC.velocity *= 1.02f;
                if(Timer >= 120)
                {
                    ResetState(ActionState.Idle);
                }
            }

            float velocityRotation = NPC.velocity.ToRotation();
            NPC.rotation = MathHelper.Lerp(NPC.rotation, velocityRotation, 0.08f);
            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        private void AI_LaserBig()
        {
            Player target = Main.player[NPC.target];
            if (AttackTimer == 0)
            {
                NPC.TargetClosest();
                Timer++;
                if(Timer == 1)
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Charge2");
                    SoundEngine.PlaySound(soundStyle, NPC.position);
                }

                TargetSegmentStretch = 1;
                AI_MoveInOrbit();
                NPC.rotation = NPC.velocity.ToRotation();

                //Charge Particles
                float progress = OrbitDistance / 500;
                float minParticleSpawnSpeed = 8;
                float maxParticleSpawnSpeed = 2;
                int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
                if (Timer % particleSpawnSpeed == 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(168, 168);
                        Vector2 vel = (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                        Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, 0, Color.OrangeRed);
                    }
                }

                OrbitDistance-=2;
                if (OrbitDistance <= 0)
                {
                    Timer = 0;
                    AttackTimer++;
                }
            } 
            else if (AttackTimer == 1)
            {
                NPC.velocity *= 0.98f;
                if(Timer == 1)
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER");
                    SoundEngine.PlaySound(soundStyle, NPC.position);

        
                    if (StellaMultiplayer.IsHost)
                    {
                        int type = ModContent.ProjectileType<HavocLaserBigProj>();
                        int damage = LaserBigDamage;
                        int knockback = 1;
                        Vector2 pos = NPC.Center;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, NPC.rotation.ToRotationVector2(),
                            type, damage, knockback, Main.myPlayer, 0, ai1: NPC.whoAmI);
                    }
                }
                
                if(Timer % 2 == 0)
                {
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.position, 1024, 16);
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(64, 64);
                    Vector2 vel = NPC.rotation.ToRotationVector2() * 8;
                    float scale = Main.rand.NextFloat(2.5f, 3.75f);
                    Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, 0, Color.OrangeRed, scale).noGravity = true;
                    if (Main.rand.NextBool(10))
                    {
                        Dust.NewDustPerfect(pos, ModContent.DustType<TSmokeDust>(), vel, 0, Color.OrangeRed, scale / 2).noGravity=true;
                    }
                }

                NPC.rotation += MathHelper.TwoPi / 360;
                Timer++;
                if(Timer >= 360)
                {
                    ResetState(ActionState.Idle);
                }
            }

            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        public override void PostAI()
        {
            if (Segments == null)
                return;
            //Move segments according to velocity
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].Position += Segments[i].Velocity;
            }
        }

        private void MoveSegmentsLikeWorm()
        {
            if (Segments == null)
                return;
            for (int i = 1; i < Segments.Length; i++)
            {
                MoveSegmentLikeWorm(i);
            }
        }

        private void MoveSegmentLikeWorm(int index)
        {
            if (Segments == null)
                return;
            int inFrontIndex = index - 1;
            if (inFrontIndex < 0)
                return;

            ref HavocSegment segment = ref Segments[index];
            ref HavocSegment frontSegment = ref Segments[index - 1];

            // Follow behind the segment "in front" of this NPC
            // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
            float dirX = frontSegment.Position.X - segment.Position.X;
            float dirY = frontSegment.Position.Y - segment.Position.Y;

            // We then use Atan2 to get a correct rotation towards that parent NPC.
            // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
            segment.Rotation = (float)Math.Atan2(dirY, dirX) * 0.33f;
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            if (length == 0)
                length = 1;

            // We calculate a new, correct distance.

            float fixer = 1;
            if (index == Segments.Length - 1)
            {
                fixer /= 1.75f;

                //Unbreak that rotation
                segment.Rotation *= 3;
            }

            float dist = (length - segment.Size.X * SegmentStretch * fixer) / length;

            float posX = dirX * dist;
            float posY = dirY * dist;

            //reset the velocity
            segment.Velocity = Vector2.Zero;


            // And set this NPCs position accordingly to that of this NPCs parent NPC.
            segment.Position.X += posX;
            segment.Position.Y += posY;
        }

        public float WidthFunction(float completionRatio)
        {
            return NPC.width * NPC.scale / 4.2f;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Orange, Color.Red, 0.2f);
            return color * NPC.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }


        public float WidthFunctionCharge(float completionRatio)
        {
            return NPC.width * NPC.scale / 0.75f * (1f- completionRatio);
        }

        public Color ColorFunctionCharge(float completionRatio)
        {
            if (!DrawChargeTrail)
            {
                ChargeTrailOpacity -= 0.05f;
                if (ChargeTrailOpacity <= 0)
                    ChargeTrailOpacity = 0;
            }
            else
            {
                ChargeTrailOpacity += 0.05f;
                if (ChargeTrailOpacity >= 1)
                    ChargeTrailOpacity = 1;
            }

            Color color = Color.Lerp(Color.Orange, Color.Red, 0.2f);
            return color * NPC.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f) * ChargeTrailOpacity * (1f - completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        internal PrimitiveTrail BeamDrawer;
        Vector2 HitboxFixer = new Vector2(90, 90) / 2;
        int warningFrameCounter;
        int warningFrameTick;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Segments == null)
                return false;
            if (State == ActionState.Charge)
            {
                Texture2D warningTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/IrradiaNHavoc/Havoc/Projectiles/HavocWarnChargeProj").Value;
                Player target = Main.player[NPC.target];
                Vector2 drawPosition = target.Center - screenPos;
                Vector2 drawOffset = new Vector2(0, -16);

                spriteBatch.Draw(warningTexture, drawPosition + drawOffset, warningTexture.AnimationFrame(ref warningFrameCounter, ref warningFrameTick, 4, 4, true), Color.White, 0, warningTexture.Size() / 2, 1f, SpriteEffects.None, 0);
                Lighting.AddLight(target.Center + drawOffset, Color.Red.ToVector3() * 2.25f);
            }

            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            Vector2 size = new Vector2(90, 90);
            TrailDrawer.DrawPrims(NPC.oldPos, size * 0.5f - screenPos, 155);

            //Draw Chain
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailRegistry.LaserShader.UseColor(Color.LightGoldenrodYellow);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);
            for (int i = 1; i < Segments.Length; i++)
            {
                HavocSegment segment = Segments[i - 1];
                HavocSegment nextSegment = Segments[i];
                List<Vector2> points = new();
                for (int j = 0; j <= 8; j++)
                {
                    points.Add(Vector2.Lerp(segment.Position, nextSegment.Position, j / 8f));
                }
                BeamDrawer.Draw(points, -Main.screenPosition + HitboxFixer, 32);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw all the segments
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                HavocSegment segment = Segments[i];
                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale;
                spriteBatch.Draw(segment.Texture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Segments == null)
                return;
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                HavocSegment segment = Segments[i];
              /*  if (!segment.HasGlowTexture)
                    continue;
              */
                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale;

                float osc = VectorHelper.Osc(0, 1);
                spriteBatch.Draw(segment.GlowTexture, drawPosition, null, drawColor * osc, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                for (float j = 0f; j < 1f; j += 0.25f)
                {
                    float radians = (j + osc) * MathHelper.TwoPi;
                    spriteBatch.Draw(segment.GlowTexture, drawPosition + new Vector2(0f, 8f).RotatedBy(radians) * osc,
                        null, Color.White * osc * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
