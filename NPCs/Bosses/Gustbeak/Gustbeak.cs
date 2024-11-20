using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.DrawEffects;
using Stellamod.Common.Lights;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.Gustbeak.Projectiles;
using Stellamod.NPCs.Bosses.SunStalker;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Gustbeak
{
    [AutoloadBossHead]
    internal partial class Gustbeak : ModNPC
    {

        private enum AIState
        {
            Idle,

            WindBlast_Start,
            WindBlast,
            WindBlast_End,

            Average_Magic_Ball_Start,
            Average_Magic_Ball,
            Average_Magic_Ball_End,

            Wing_Airblast_Start,
            Wing_Airblast,
            Wing_Airblast_End,

            Wing_Tornado_Start,
            Wing_Tornado,
            Wing_Tornado_End,

            Phase_2_Transition,

            Wind_Dash_Start,
            Wind_Dash,
            Wind_Dash_End,

            Wind_Crash_Start,
            Wind_Crash,
            Wind_Crash_End,

            Death
        }

        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float AttackCycle => ref NPC.ai[2];

        private BaseGustbeakSegment[] _segments;
        private BaseGustbeakSegment[] Segments
        {
            get
            {
                if (_segments == null)
                {
                    List<BaseGustbeakSegment> segments = new List<BaseGustbeakSegment>();
                    segments.Add(Head);
                    BodyFront.children = new BaseGustbeakSegment[4]
                    {
                        WingFront,
                        WingBack,
                        FrontLegFront,
                        FrontLegBack
                    };

                    segments.Add(BodyFront);
                    segments.Add(BodyMiddle);


                    BodyBack.children = new BaseGustbeakSegment[2]
                    {
                        BackLegFront,
                        BackLegBack
                    };
                    segments.Add(BodyBack);
                    segments.Add(Tail);
                    _segments = segments.ToArray();
                }

                return _segments;
            }
        }

        private GustbeakHead _head;
        private GustbeakHead Head
        {
            get
            {
                _head ??= new GustbeakHead();
                return _head;
            }
        }

        private GustbeakBodyFront _bodyFront;
        private GustbeakBodyFront BodyFront
        {
            get
            {
                _bodyFront ??= new GustbeakBodyFront();
                return _bodyFront;
            }
        }

        private GustbeakBodyMiddle _bodyMiddle;
        private GustbeakBodyMiddle BodyMiddle
        {
            get
            {
                _bodyMiddle ??= new GustbeakBodyMiddle();
                return _bodyMiddle;
            }
        }

        private GustbeakBodyBack _bodyBack;
        private GustbeakBodyBack BodyBack
        {
            get
            {
                _bodyBack ??= new GustbeakBodyBack();
                return _bodyBack;
            }
        }

        private GustbeakTail _tail;
        private GustbeakTail Tail
        {
            get
            {
                _tail ??= new GustbeakTail();
                return _tail;
            }
        }

        private GustbeakWingFront _wingFront;
        private GustbeakWingFront WingFront
        {
            get
            {
                _wingFront ??= new GustbeakWingFront();
                return _wingFront;
            }
        }

        private GustbeakWingBack _wingBack;
        private GustbeakWingBack WingBack
        {
            get
            {
                _wingBack ??= new GustbeakWingBack();
                return _wingBack;
            }
        }

        private GustbeakFrontLegFront _frontLegFront;
        private GustbeakFrontLegFront FrontLegFront
        {
            get
            {
                _frontLegFront ??= new GustbeakFrontLegFront();
                return _frontLegFront;
            }
        }

        private GustbeakFrontLegBack _frontLegBack;
        private GustbeakFrontLegBack FrontLegBack
        {
            get
            {
                _frontLegBack ??= new GustbeakFrontLegBack();
                return _frontLegBack;
            }
        }

        private GustbeakBackLegFront _backLegFront;
        private GustbeakBackLegFront BackLegFront
        {
            get
            {
                _backLegFront ??= new GustbeakBackLegFront();
                return _backLegFront;
            }
        }

        private GustbeakBackLegBack _backLegBack;
        private GustbeakBackLegBack BackLegBack
        {
            get
            {
                _backLegBack ??= new GustbeakBackLegBack();
                return _backLegBack;
            }
        }

        private CommonWind _wind;
        private CommonWind Wind
        {
            get
            {
                _wind ??= new CommonWind();
                return _wind;
            }
        }

        private float WindCharge;
        private Vector2 TailPosition;
        private Player Target => Main.player[NPC.target];
        private Vector2 TargetCenter => Target.Center;
        private Vector2 DashStartCenter;
        private Vector2 DashVelocity;
        private float Invisibility = 1f;
        private bool DrawHelmet = true;

        private int WindBlastDamage => 12;
        private int AverageBallDamage => 24;
        private int WingAirBlastDamage => 18;
        private int TornadoDamage => 30;
        private int WindDashDamage => 12;
        private int WindCrashDamage => 18;

        private bool Phase2Transition;
        private bool InPhase2 => NPC.life < NPC.lifeMax / 2;

        private float FlipValue;
        private float DirToPlayer => Target.Center.X < NPC.Center.X ? -1 : 1;


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(DashStartCenter);
            writer.WriteVector2(DashVelocity);
            writer.Write(Invisibility);
            writer.Write(DrawHelmet);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            DashStartCenter = reader.ReadVector2();
            DashVelocity = reader.ReadVector2();
            Invisibility = reader.ReadSingle();
            DrawHelmet = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Stellamod/NPCs/Bosses/Gustbeak/Gustbeak",
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 14;
            NPC.defense = 12;
            NPC.lifeMax = 1300;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.DD2_WyvernScream;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.takenDamageMultiplier = 0.9f;
            NPC.BossBar = ModContent.GetInstance<GustbeakBossBar>();
            NPC.aiStyle = -1;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SunStalker");
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        private void DrawGustbeak(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            WingBack.Draw(spriteBatch, screenPos, drawColor);
            BackLegBack.Draw(spriteBatch, screenPos, drawColor);
            FrontLegBack.Draw(spriteBatch, screenPos, drawColor);

            //Back Body Sprites
            Tail.Draw(spriteBatch, screenPos, drawColor);
            BodyBack.Draw(spriteBatch, screenPos, drawColor);
            BodyMiddle.Draw(spriteBatch, screenPos, drawColor);
            BodyFront.Draw(spriteBatch, screenPos, drawColor);

            //Other Sprites
            BackLegFront.Draw(spriteBatch, screenPos, drawColor);
            FrontLegFront.Draw(spriteBatch, screenPos, drawColor);
            Head.Draw(spriteBatch, screenPos, drawColor);
            WingFront.Draw(spriteBatch, screenPos, drawColor);

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Restart(blendState: BlendState.Additive);
            //Draw Outline stuff
            for (int i = 0; i < Segments.Length; i++)
            {
                var segment = Segments[i];
                segment.drawArmored = false;
            }




            for (float f = 0.0f; f < 1.0f; f += 1f)
            {
                float rot = f * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly * 0.5f;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(3f, 5f);
                DrawGustbeak(spriteBatch, screenPos + offset, drawColor);
            }

            spriteBatch.RestartDefaults();


            //Draw back to front i think
            //Back sprites
            for (int i = 0; i < Segments.Length; i++)
            {
                var segment = Segments[i];
                segment.drawArmored = true;
            }
            DrawGustbeak(spriteBatch, screenPos, drawColor);

            spriteBatch.RestartDefaults();
            Wind.Draw(spriteBatch, drawColor);
            return false;
        }


        public override void AI()
        {
            base.AI();

            Head.position = NPC.Center;
            TailPosition = Head.position + (new Vector2(-48 * FlipValue, 80)).RotatedBy(NPC.rotation);
            TailPosition.X -= NPC.velocity.X * 8;

            float rotToTarget = (TargetCenter - NPC.Center).ToRotation();
            TailPosition -= rotToTarget.ToRotationVector2() * 24;
            float flipSpeed = 0.05f;
            if (Target.Center.X < NPC.Center.X)
            {
                //FLip
                FlipValue -= flipSpeed;
            }
            else
            {
                FlipValue += flipSpeed;
            }
            FlipValue = MathHelper.Clamp(FlipValue, -1f, 1f);
            Head.drawHelmet = DrawHelmet;

            Vector2[] curvePositions = CalculateCurve();
            for (int c = 1; c < curvePositions.Length; c++)
            {
                Vector2 curvePoint = curvePositions[c];
                Vector2 prevCurvePoint = curvePositions[c - 1];
                float rotation = (prevCurvePoint - curvePoint).ToRotation();

                //Update AI
                var segment = Segments[c - 1];
                segment.rotation = rotation;
                segment.globalRotation = NPC.rotation;
                segment.position = curvePoint;
                segment.spriteEffects = FlipValue < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                segment.invisibility = Invisibility;
                segment.AI();
            }

            Head.rotation = (Target.Center - Head.position).ToRotation();

            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }

            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;

                case AIState.WindBlast_Start:
                    AI_WindBlastStart();
                    break;
                case AIState.WindBlast:
                    AI_WindBlast();
                    break;
                case AIState.WindBlast_End:
                    AI_WindBlastEnd();
                    break;

                case AIState.Average_Magic_Ball_Start:
                    AI_AverageMagicBallStart();
                    break;
                case AIState.Average_Magic_Ball:
                    AI_AverageMagicBall();
                    break;
                case AIState.Average_Magic_Ball_End:
                    AI_AverageMagicBallEnd();
                    break;

                case AIState.Wing_Airblast_Start:
                    AI_WingAirblastStart();
                    break;
                case AIState.Wing_Airblast:
                    AI_WingAirblast();
                    break;
                case AIState.Wing_Airblast_End:
                    AI_WingAirblastEnd();
                    break;

                case AIState.Wing_Tornado_Start:
                    AI_WingTornadoStart();
                    break;
                case AIState.Wing_Tornado:
                    AI_WingTornado();
                    break;
                case AIState.Wing_Tornado_End:
                    AI_WingTornadoEnd();
                    break;

                case AIState.Phase_2_Transition:
                    AI_Phase2Transition();
                    break;

                case AIState.Wind_Dash_Start:
                    AI_WindDashStart();
                    break;
                case AIState.Wind_Dash:
                    AI_WindDash();
                    break;
                case AIState.Wind_Dash_End:
                    AI_WindDashEnd();
                    break;

                case AIState.Wind_Crash_Start:
                    AI_WindCrashStart();
                    break;
                case AIState.Wind_Crash:
                    AI_WindCrash();
                    break;
                case AIState.Wind_Crash_End:
                    AI_WindCrashEnd();
                    break;

                case AIState.Death:
                    AI_Death();
                    break;
            }

            float targetRot = NPC.velocity.X * 0.1f;
            float newRotation = MathHelper.Lerp(NPC.rotation, targetRot, 0.1f);
            NPC.rotation = newRotation;

            float rotationToPlayer = (Target.Center - NPC.Center).ToRotation();
            Wind.AI(NPC.Center + rotationToPlayer.ToRotationVector2() * 12);
            if (Timer % 4 == 0)
            {

                Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(24, 24), DustID.GemDiamond, Vector2.Zero, Scale: 1f);
                d.noGravity = true;
            }
        }


        private void AI_Idle()
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -8), 0.025f);
                NPC.EncourageDespawn(60);
                return;
            }

            //OK, Let's plan out the attacks
            //Gustbeak will have a few attacks
            //1. Opens mouth and fires compressed wind blasts, like slicing wind type shader that violently expands, I think I know how to do this
            //2. Opens mouth and fires a large and small ball like in the gif zemmie sent, it slowly moves in a direction before beelining for the player
            //3. Flaps wings slower and has synced air ball blasts with each time he closes his wings
            //4. Flaps wings very fast and creates a really cool tornado, most deadly attack, it sucks you in and pulls in rubble
            //Crowd screams during this attack very funnny
            //5. PHASE 2 - Triggers at half health, he screams and there's a shockwave and blur effect over the screen
            //6. He becomes a bit faster generally in phase 2
            //7. Hovers above around in the center of the arena and flaps around violently randomly sending air blasts and weak projectiles and debris/rocks everywhere
            //-Very panicky attack
            //8. Opens mouth and charges up a compressed and large airball for an extended period of time before firing it at the player, this is extremely deadly

            Timer++;
            Vector2 targetVelocity = new Vector2(0, MathF.Sin(Timer * 0.1f)) * 0.2f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
            if (Timer % 7 == 0)
            {
                SoundStyle wingFlap = new SoundStyle($"Stellamod/Assets/Sounds/NiiviWingFlap");
                wingFlap.PitchVariance = 0.15f;
                SoundEngine.PlaySound(wingFlap, NPC.position);
            }

            if (Timer >= 15 && StellaMultiplayer.IsHost)
            {
                if (!Phase2Transition && InPhase2)
                {
                    SwitchState(AIState.Phase_2_Transition);
                    Phase2Transition = true;
                }
                else
                {
                    if (InPhase2)
                    {
                        switch (AttackCycle)
                        {
                            case 0:
                                SwitchState(AIState.WindBlast_Start);
                                break;
                            case 1:
                                SwitchState(AIState.Average_Magic_Ball_Start);
                                break;
                            case 2:
                                SwitchState(AIState.Wing_Airblast_Start);
                                break;
                            case 3:
                                SwitchState(AIState.Wind_Dash_Start);
                            
                                break;
                            case 4:
                                SwitchState(AIState.Wing_Tornado_Start);
                                break;
                            case 5:
                                SwitchState(AIState.Wind_Crash_Start);
                                break;
                        }

                        AttackCycle++;
                        if (AttackCycle >= 6)
                        {
                            AttackCycle = 0;
                        }
                    }
                    else
                    {
                        switch (AttackCycle)
                        {
                            case 0:
                                SwitchState(AIState.WindBlast_Start);
                                break;
                            case 1:
                                SwitchState(AIState.Average_Magic_Ball_Start);
                                break;
                            case 2:
                                SwitchState(AIState.Wing_Airblast_Start);
                                break;
                            case 3:
                                SwitchState(AIState.Wing_Tornado_Start);
                                break;
                        }

                        AttackCycle++;
                        if (AttackCycle >= 4)
                        {
                            AttackCycle = 0;
                        }
                    }
                  
                }
       
            
            }
        }

        private void AI_WindBlastStart()
        {
            //Well first we need to get the rigging and animation setup
            //Let's make a list of tasks

            //1. Setup Gust Beak Rigging/Animation
            //2. Setup Gust Beak Armor/Invis Shader
            //3. Setup Wind Shader Stuff (gonna spam prims and do some cool little things with ovals and offsetting movement
            //4. Make Wind Blast Projectile
            //5. Make Gust Beak Wind Blast Attack
            //6. Make Average Magic Ball Projectile
            //7. Make Gust Beak Average Magic Ball Attack
            //8. Make Wing Airblast Projectile
            //9. Make Gust Beak Wing Airblast Attack
            //10. Make Tornado Projectile
            //11. Make Gust Beak Tornado Attak
            //12. Make Phase 2 Transition
            //13. Make Gust Beak Rampage Attack and Rubble Projectiles
            //14. Make Gust Beak Vacuum Blast Attack
            //15. Make Gust Beak death animation (sadness)
            //16. Make Gust Beak Relic


            //Kay

            //HE SHOULD
            //1. Open Mouth
            //2. Start flying around/towards the player
            //3. Firing Wind Blasts
            Timer++;
            Head.Animation = GustbeakHead.AnimationState.Open_Mouth;

            float dir = Target.Center.X < NPC.Center.X ? -1 : 1;
            Vector2 pointAbovePlayer = Target.Center + new Vector2(-252 * dir, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);


            //Home to this point
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * 8, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if (Timer > 60)
            {
                SwitchState(AIState.WindBlast);
            }
        }

        private void AI_WindBlast()
        {
            Timer++;
            if (Timer == 1)
            {
                WindCharge = 0;
            }
            float dir = Target.Center.X < NPC.Center.X ? -1 : 1;
            Vector2 pointAbovePlayer = Target.Center + new Vector2(-252 * dir, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);


            //Home to this point
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * 8, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;

            if (Timer % 4 == 0)
            {
                Vector2 offset = Main.rand.NextVector2Circular(4, 4);
                float rotation = offset.ToRotation();
                rotation += Main.rand.NextFloat(-1f, 1f);
                offset -= NPC.Size / 2f;
                Wind.NewSlash(offset, rotation);
            }
            WindCharge++;
            float windChargeProgress = WindCharge / 60f;
            float easedWindChargeProgress = Easing.OutCirc(windChargeProgress);
            Wind.ExpandMultiplier = MathHelper.Lerp(4f, 0.2f, easedWindChargeProgress);
            if (Timer % 60 == 0)
            {
                WindCharge = 0;
                SoundStyle windCast = new SoundStyle($"Stellamod/Assets/Sounds/WindCast", variantSuffixesStart: 1, numVariants: 2);
                windCast.PitchVariance = 0.15f;
                SoundEngine.PlaySound(windCast, NPC.position);

                //Some dust circle
                for (float f = 0; f < 24; f++)
                {
                    float rot = (f / 24f) * MathHelper.TwoPi;
                    Vector2 velOffset = rot.ToRotationVector2() * 6;
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, velOffset, Scale: 1f);
                    d.noGravity = true;
                }

                if (StellaMultiplayer.IsHost)
                {
                    int damage = WindBlastDamage;
                    int knockback = 32;
                    Vector2 fireVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    fireVelocity *= 8;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<WindBlast>(), damage, knockback, Main.myPlayer);
                }
            }

            if (Timer > 238)
            {
                SwitchState(AIState.WindBlast_End);
            }
        }

        private void AI_WindBlastEnd()
        {
            Timer++;
            Head.Animation = GustbeakHead.AnimationState.Close_Mouth;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_AverageMagicBallStart()
        {
            Timer++;
            Head.Animation = GustbeakHead.AnimationState.Open_Mouth;

            float dir = Target.Center.X < NPC.Center.X ? -1 : 1;
            Vector2 pointAbovePlayer = Target.Center + new Vector2(-252 * dir, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);


            //Home to this point
            float maxSpeed = MathHelper.Lerp(8, 2f, Timer / 60f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * maxSpeed, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if (Timer > 60)
            {
                SwitchState(AIState.Average_Magic_Ball);
            }
        }

        private void AI_AverageMagicBall()
        {
            Timer++;
            WindCharge++;
            if (Timer == 1)
            {
                WindCharge = 0;
            }

            //Yeah Yeah Yeah
            float dir = Target.Center.X < NPC.Center.X ? -1 : 1;
            Vector2 pointAbovePlayer = Target.Center + new Vector2(-252 * dir, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * 8, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;

            //Create Slashes
            if (Timer % 4 == 0)
            {
                Vector2 offset = Main.rand.NextVector2Circular(4, 4);
                float rotation = offset.ToRotation();
                rotation += Main.rand.NextFloat(-1f, 1f);
                offset -= NPC.Size / 2f;
                Wind.NewSlash(offset, rotation);
            }

            //Compress Air
            float windChargeProgress = WindCharge / 150f;
            float easedWindChargeProgress = Easing.OutCirc(windChargeProgress);
            Wind.ExpandMultiplier = MathHelper.Lerp(4f, 0.5f, easedWindChargeProgress);

            //Shoot Air
            if (Timer == 150)
            {
                WindCharge = 0;
                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                //Some dust circle
                for (float f = 0; f < 24; f++)
                {
                    float rot = (f / 24f) * MathHelper.TwoPi;
                    Vector2 velOffset = rot.ToRotationVector2() * 6;
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, velOffset, Scale: 1f);
                    d.noGravity = true;
                }

                if (StellaMultiplayer.IsHost)
                {
                    int damage = AverageBallDamage;
                    int knockback = 32;
                    Vector2 fireVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    fireVelocity *= 8;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<AverageWindBall>(), damage, knockback, Main.myPlayer);
                }
            }

            if (Timer > 180)
            {
                SwitchState(AIState.Average_Magic_Ball_End);
            }
        }

        private void AI_AverageMagicBallEnd()
        {
            Timer++;

            //Kinda just want him to fly opposite way he's facing for a bit
            Vector2 pointAbovePlayer = Target.Center + new Vector2(-512 * DirToPlayer, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * 4, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if (Timer > 30)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_WingAirblastStart()
        {
            Timer++;
            Head.Animation = GustbeakHead.AnimationState.Open_Mouth;

            Vector2 pointAbovePlayer = Target.Center + new Vector2(-512 * DirToPlayer, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * 1, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if (Timer > 60)
            {
                SwitchState(AIState.Wing_Airblast);
            }
        }

        private void AI_WingAirblast()
        {
            Timer++;
            if (Timer == 1)
            {
                WindCharge = 0;
                SoundStyle screamStyle = SoundID.DD2_WyvernScream;
                screamStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(screamStyle, NPC.position);
            }
            float dir = Target.Center.X < NPC.Center.X ? -1 : 1;
            Vector2 pointAbovePlayer = Target.Center + new Vector2(-252 * dir, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Cool little effect
            float p = (Timer / 120f);
            Vector2 pos = NPC.Center + (p * MathHelper.TwoPi).ToRotationVector2() * 80;
            Vector2 pos2 = NPC.Center + (p * MathHelper.TwoPi + MathHelper.Pi).ToRotationVector2() * 80;
            Dust.NewDustPerfect(pos, DustID.GemDiamond, Vector2.Zero, Scale: 1f);
            Dust.NewDustPerfect(pos2, DustID.GemDiamond, Vector2.Zero, Scale: 1f);


            //Home to this point
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * 1, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;

            if (Timer % 4 == 0)
            {
                Vector2 offset = Main.rand.NextVector2Circular(4, 4);
                float rotation = offset.ToRotation();
                rotation += Main.rand.NextFloat(-1f, 1f);
                offset -= NPC.Size / 2f;
                Wind.NewSlash(offset, rotation);
            }

            WindCharge++;
            float windChargeProgress = WindCharge / 60f;
            float easedWindChargeProgress = Easing.OutCirc(windChargeProgress);
            Wind.ExpandMultiplier = MathHelper.Lerp(4f, 2f, easedWindChargeProgress);
            if (Timer % 20 == 0)
            {
                WindCharge = 0;
                SoundStyle windCast = new SoundStyle($"Stellamod/Assets/Sounds/WindCast", variantSuffixesStart: 1, numVariants: 2);
                windCast.PitchVariance = 0.15f;
                SoundEngine.PlaySound(windCast, NPC.position);

                //Some dust circle
                for (float f = 0; f < 24; f++)
                {
                    float rot = (f / 24f) * MathHelper.TwoPi;
                    Vector2 velOffset = rot.ToRotationVector2() * 6;
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, velOffset, Scale: 1f);
                    d.noGravity = true;
                }

                if (StellaMultiplayer.IsHost)
                {
                    int damage = WingAirBlastDamage;
                    int knockback = 32;
                    Vector2 firePoint = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
                    Vector2 fireVelocity = (Target.Center - firePoint).SafeNormalize(Vector2.Zero);
                    fireVelocity *= 8;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), firePoint, fireVelocity,
                        ModContent.ProjectileType<WingAirblast>(), damage, knockback, Main.myPlayer);
                }
            }

            if (Timer > 238)
            {
                SwitchState(AIState.Wing_Airblast_End);
            }
        }

        private void AI_WingAirblastEnd()
        {
            Timer++;
            NPC.velocity *= 0.99f;
            Head.Animation = GustbeakHead.AnimationState.Close_Mouth;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }


        private void AI_WingTornadoStart()
        {
            Timer++;
            Head.Animation = GustbeakHead.AnimationState.Open_Mouth;
            Vector2 pointAbovePlayer = Target.Center + new Vector2(-512 * DirToPlayer, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            float maxSpeed = MathHelper.Lerp(8, 2f, Timer / 60f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * maxSpeed, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if (Timer > 60)
            {
                SwitchState(AIState.Wing_Tornado);
            }
        }

        private void AI_WingTornado()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            float progress = MathHelper.Clamp(Timer / 100f, 0f, 1f);
            WingBack.WingAnimationSpeedMult = MathHelper.Lerp(1f, 2f, progress);
            WingFront.WingAnimationSpeedMult = MathHelper.Lerp(1f, 2f, progress);

            Vector2 pointAbovePlayer = Target.Center + new Vector2(-512 * DirToPlayer, -128);
            Vector2 velToPlayer = pointAbovePlayer - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToPlayer * 1, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;

            if (Timer % 8 == 0)
            {
                Vector2 offset = Main.rand.NextVector2Circular(4, 4);
                float rotation = offset.ToRotation();
                rotation += Main.rand.NextFloat(-1f, 1f);
                offset -= NPC.Size / 2f;
                Wind.NewSlash(offset, rotation);
            }

            if (Timer == 70)
            {
                if (StellaMultiplayer.IsHost)
                {
                    int tornadoDamage = TornadoDamage;
                    int knockback = 2;
                    Vector2 tornadoPos = NPC.Center + new Vector2(DirToPlayer * 48, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), tornadoPos, Vector2.Zero,
                        ModContent.ProjectileType<WindStorm>(), tornadoDamage, knockback, Main.myPlayer);
                }
            }

            if (Timer > 70)
            {
                ShakeModSystem.Shake = 1.5f;
            }
            if (Timer > 370)
            {
                ShakeModSystem.Shake = 0f;
            }
            if (Timer > 370)
            {
                SwitchState(AIState.Wing_Tornado_End);
            }
        }

        private void AI_WingTornadoEnd()
        {
            Timer++;
            float progress = MathHelper.Clamp(Timer / 60f, 0f, 1f);
            WingBack.WingAnimationSpeedMult = MathHelper.Lerp(2f, 1f, progress);
            WingFront.WingAnimationSpeedMult = MathHelper.Lerp(2f, 1f, progress);

            NPC.velocity *= 0.99f;
            Head.Animation = GustbeakHead.AnimationState.Close_Mouth;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Phase2Transition()
        {
            Timer++;
          
            //He also slows down
            NPC.velocity *= 0.99f;

            if(Timer > 115)
            {
                Head.Animation = GustbeakHead.AnimationState.Open_Mouth;
            }

            //Gustbeak screams
            if (Timer == 120)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernDeath;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.position, 2048, 12);
                DrawHelmet = false;
            }

            if(Timer > 120 && Timer < 240)
            {
                SpecialEffectsPlayer specialEffectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                specialEffectsPlayer.blurStrength = 1f;
                NPC.velocity = NPC.velocity.RotatedBy(0.01f);
            }

            if(Timer > 360)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_WindDashStart()
        {
            //Align directly right or left of player
            Timer++;
            if(Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if(Timer < 120)
            {
                DashStartCenter = Target.Center + new Vector2(-512 * DirToPlayer, 0);
                DashVelocity = new Vector2(DirToPlayer * 16, 0);
            }

            Vector2 velToPlayer = DashStartCenter - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);
            Invisibility = MathHelper.Lerp(1f, 0f, Timer / 60f);

            //Home to this point
            float maxSpeed = 24f;
            Vector2 targetVelocity = velToPlayer;
            float distance = Vector2.Distance(NPC.Center, DashStartCenter);
            if (distance < maxSpeed)
            {
                targetVelocity *= distance;
            }
            else
            {
                targetVelocity *= maxSpeed;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if(Timer > 180f)
            {
                SwitchState(AIState.Wind_Dash);
            }
        }

        private void AI_WindDash()
        {
            Timer++;
            WingFront.Animation = BaseGustbeakWingSegment.AnimationState.Hold_Up;
            WingBack.Animation = BaseGustbeakWingSegment.AnimationState.Hold_Up;
            if(Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernScream;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position); 
               
                for (int i = 0; i < 12; i++)
                {
                    float progress = (float)i / 12f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }

                if (StellaMultiplayer.IsHost)
                {
                    int damage = WindDashDamage;
                    int knockback = 4;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<WindAura>(), damage, knockback, Main.myPlayer, 
                        ai1: NPC.whoAmI);
                }
            }

            Invisibility = MathHelper.Lerp(0f, 1f, MathHelper.Clamp(Timer / 10f, 0f, 1f));
            NPC.velocity = Vector2.Lerp(NPC.velocity, DashVelocity, 0.1f);
            if(Timer % 24 == 0)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                for (int i = 0; i < 12; i++)
                {
                    float progress = (float)i / 12f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }

                if (StellaMultiplayer.IsHost)
                {
                    Vector2 vel = Vector2.UnitY * 3;
                    if (Target.Center.Y < NPC.Center.Y)
                    {
                        vel = -vel;
                    }

                    int damage = WindDashDamage;
                    int knockback = 4;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, vel,
                        ModContent.ProjectileType<WingAirblast>(), damage, knockback, Main.myPlayer);
                }
            }
      
            if(Timer == 40)
            {
                DashVelocity *= 0.5f;
            }
            if(Timer > 80)
            {
                DashVelocity *= 0.9f;
            }

            if(Timer > 140)
            {
                SwitchState(AIState.Wind_Dash_End);
            }
        }

        private void AI_WindDashEnd()
        {
            Timer++;
            WingFront.Animation = BaseGustbeakWingSegment.AnimationState.Flap;
            WingBack.Animation = BaseGustbeakWingSegment.AnimationState.Flap;
            NPC.velocity *= 0.99f;
            Head.Animation = GustbeakHead.AnimationState.Close_Mouth;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_WindCrashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if (Timer < 30)
            {
                DashStartCenter = Target.Center + new Vector2(0, -300);
                DashVelocity = new Vector2(0, 16);
            }

            Vector2 velToPlayer = DashStartCenter - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);
            Invisibility = MathHelper.Lerp(1f, 0f, Timer / 60f);

            //Home to this point
            float maxSpeed = 24f;
            Vector2 targetVelocity = velToPlayer;
            float distance = Vector2.Distance(NPC.Center, DashStartCenter);
            if(distance < maxSpeed)
            {
                targetVelocity *= distance;
            }
            else
            {
                targetVelocity *= maxSpeed;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;
            if (Timer > 120f)
            {
                SwitchState(AIState.Wind_Crash);
            }
        }

        private void AI_WindCrash()
        {
            Timer++;
            WingFront.Animation = BaseGustbeakWingSegment.AnimationState.Hold_Up;
            WingBack.Animation = BaseGustbeakWingSegment.AnimationState.Hold_Up;
            if (Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernScream;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                for (int i = 0; i < 12; i++)
                {
                    float progress = (float)i / 12f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }

                if (StellaMultiplayer.IsHost)
                {
                    int damage = WindCrashDamage;
                    int knockback = 4;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<WindAura>(), damage, knockback, Main.myPlayer,
                        ai1: NPC.whoAmI);
                }
            }

            Invisibility = MathHelper.Lerp(0f, 1f, MathHelper.Clamp(Timer / 10f, 0f, 1f));
            NPC.velocity = Vector2.Lerp(NPC.velocity, DashVelocity, 0.1f);

            if(Timer == 5)
            {
                DashVelocity *= 0.5f;
            }

            if(Timer % 30 == 0)
            {
                DashVelocity *= 0.5f;
                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
                if (StellaMultiplayer.IsHost)
                {
                    int damage = WindCrashDamage;
                    int knockback = 4;
                    Vector2 vel = Vector2.UnitX * 2;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, vel,
                        ModContent.ProjectileType<WingAirblast>(), damage, knockback, Main.myPlayer);

                    vel = -vel;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, vel,
                        ModContent.ProjectileType<WingAirblast>(), damage, knockback, Main.myPlayer);
                }
            }

            if (Timer > 30)
            {
                DashVelocity *= 0.99f;
            }

            if(Timer > 120)
            {
                SwitchState(AIState.Wind_Crash_End);
            }
        }

        private void AI_WindCrashEnd()
        {
            Timer++;
            WingFront.Animation = BaseGustbeakWingSegment.AnimationState.Flap;
            WingBack.Animation = BaseGustbeakWingSegment.AnimationState.Flap;
            NPC.velocity *= 0.99f;
            Head.Animation = GustbeakHead.AnimationState.Close_Mouth;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Death()
        {
            Timer++;
            NPC.velocity *= 0.99f;
            WingFront.Animation = BaseGustbeakWingSegment.AnimationState.Hold_Out;
            WingBack.Animation = BaseGustbeakWingSegment.AnimationState.Hold_Out;
            if(Timer == 120)
            {
                Head.Animation = GustbeakHead.AnimationState.Open_Mouth;
                SoundStyle soundStyle = SoundID.DD2_WyvernScream;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.position, 2048, 8);
            }
            if (Timer > 120 && Timer < 180)
            {
                SpecialEffectsPlayer specialEffectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                specialEffectsPlayer.blurStrength = 1f;
                NPC.velocity = NPC.velocity.RotatedBy(0.01f);
            }

            if (Timer > 180)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernDeath;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
                for (int i = 0; i < 32; i++)
                {
                    float progress = (float)i / 32f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 8;
                    Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
                }

                var entitySource = NPC.GetSource_Death();
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakBackLegBackGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakBackLegFrontGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakFrontLegBackGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakFrontLegFrontGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakBodyBackGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakBodyMiddleGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakBodyFrontGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakTailGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakHeadGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakWingsBackGore>());
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, ModContent.GoreType<GustbeakWingsFrontGore>());
                NPC.Kill();
            }
        }

        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
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

        private Vector2[] CalculateCurve()
        {
            Vector2 ownerPos = Head.position;
            List<Vector2> controlPoints = new List<Vector2>();
            controlPoints.Add(Head.position);

            Vector2 controlPoint1 = Vector2.Lerp(Head.position, TailPosition, 0.3f);
            controlPoint1.Y -= 8;
            controlPoint1.X -= 32 * FlipValue;
            controlPoints.Add(controlPoint1);
            controlPoints.Add(TailPosition);

            int numPoints = Segments.Length;
            Vector2[] chainPositions = GetBezierApproximation(controlPoints.ToArray(), numPoints);
            return chainPositions;
        }

        private Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount)
        {
            Vector2[] points = new Vector2[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                float t = (float)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return points;
        }

        private Vector2 GetBezierPoint(float t, Vector2[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Vector2((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<GustbeakBossRel>()));
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSunsBoss, -1);
        }
    }
}
