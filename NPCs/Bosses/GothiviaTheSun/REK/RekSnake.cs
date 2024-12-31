using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Wings;
using Stellamod.Items.Consumables;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles;
using Stellamod.Projectiles.Visual;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK
{
    internal class RekSegment
    {
        public string TexturePath;
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath).Value;
        public Texture2D GlowTexture => ModContent.Request<Texture2D>(TexturePath + "_Glow").Value;
        public Color GlowWhiteColor;
        public bool GlowWhite;
        public float GlowTimer;
        public Vector2 Size;
        public Vector2 Position;
        public Vector2 Center => Position + Size / 2;
        public Vector2 Velocity;
        public float Rotation;
        public float Scale = 1f;
        public bool Eaten;

        public RekSegment(NPC npc)
        {
            Position = npc.position;
            Rotation = 0;
            Velocity = Vector2.Zero;
            Eaten = false;
        }
    }

    [AutoloadBossHead]
    internal class RekSnake : ModNPC
    {
        //Draw Code
        private string BaseTexturePath => "Stellamod/NPCs/Bosses/GothiviaTheSun/REK/";
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float SegmentStretch = 0.66f;
        private float ChargeTrailOpacity;
        private bool DrawChargeTrail;

        //Segments
        private RekSegment Head => Segments[0];
        private RekSegment[] Segments;
        private Vector2 HitboxFixer => new Vector2(NPC.width, NPC.height) / 2;

        //AI
        private bool _resetTimers;
        private enum ActionState
        {
            Idle,
            Dash,
            Clappback,
            EyePopout,
            Beamer,
            Ouroboros,
            Crystal,
            Death
        }

   
        private ActionState State
        {
            get =>      (ActionState) NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        //Damage Numbers
        private int DamageBeamerFireBall => 60;
        private int DamagePopoutEyeMiniLaser => 70;
        private int DamageFireShockWave => 150;
        private int DamageBlowtorch => 90;
        private int DamageBlowtorchBlast => 90;
        private int DamageBlowtorchExplosion => 150;

        private ref float Timer => ref NPC.ai[1];
        private ref float AttackTimer => ref NPC.ai[2];
        private ref float AttackCycle => ref NPC.ai[3];
        private bool PoppedOutEye;
        private bool InPhase2 => NPC.life < NPC.lifeMax / 2f;
        private Player Target => Main.player[NPC.target];
        private IEntitySource EntitySource => NPC.GetSource_FromThis();
        private Vector2 CrystalPosition;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 32;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;


            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,

                    BuffID.Confused // Most NPCs have this
				}
            };
     
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/GothiviaTheSun/REK/RekBestiary";
            drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A special snake from the Cinderspark that was atop it's food chain and took a liking to Gothivia. Eventually it became her guardian.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Rek, The Sun Serpent", "2"))
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 90;
            NPC.lifeMax = 108000;
           
            NPC.damage = 900;
            NPC.defense = 135;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<RekBossBar>();
            NPC.takenDamageMultiplier = 0.8f;

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Rek");
            }
        }

        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                Timer = 0;
                AttackTimer = 0;
                AttackCycle = 0;
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

        private void ResetState(ActionState state)
        {
            State = state;
            ResetTimers();
            if (StellaMultiplayer.IsHost)
            {
                NPC.netUpdate = true;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return State == ActionState.Dash;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(CrystalPosition);
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
            CrystalPosition = reader.ReadVector2();
            bool hasSegments = reader.ReadBoolean();
            if (hasSegments)
            {
                int length = reader.ReadInt32();
                for (int i = 0; i < length; i++)
                {
                    Vector2 pos = reader.ReadVector2();
                    Vector2 vel = reader.ReadVector2();
                    if(Segments != null && Segments.Length <= length)
                    {
                        Segments[i].Position = pos;
                        Segments[i].Velocity = vel;
                    }
                }
            }
        }

        public override void AI()
        {
            FinishResetTimers();

            if (Segments == null)
            {
                //Initialize Segments
                int bodySegments = 4;
                int bodFrontExtraSegments = 7;
                int bodyExtraSegments = 6;
                int tailSegments = 8;
                List<RekSegment> segments = new List<RekSegment>();
                //Set the textures

                //Head
                float offset = 0.1f;
                RekSegment segment = new RekSegment(NPC);
                segment.TexturePath = Texture;
                segment.Size = new Vector2(204, 204);
                segment.Position = segment.Position + new Vector2(offset, 0);
                segments.Add(segment);
                offset += 0.1f;

                //Neck
                segment = new RekSegment(NPC);
                segment.TexturePath = $"{BaseTexturePath}RekNeck";
                segment.Size = new Vector2(74, 90);
                segment.Position = segment.Position + new Vector2(offset, 0);
                segments.Add(segment);
                offset += 0.1f;
                for (int i = 0; i < bodySegments; i++)
                {
                    segment = new RekSegment(NPC);
                    segment.TexturePath = $"{BaseTexturePath}RekBody{i + 1}";
                    segment.Position = segment.Position + new Vector2(offset, 0);
                    offset += 0.1f;
                    switch (i)
                    {
                        case 0:
                            segment.Size = new Vector2(118, 106);
                            break;
                        case 1:
                            segment.Size = new Vector2(118, 106);
                            break;
                        case 2:
                            segment.Size = new Vector2(84, 98);
                            break;
                        case 3:
                            segment.Size = new Vector2(68, 58);
                            break;
                        case 4:
                            segment.Size = new Vector2(62, 50);
                            break;
                        case 5:
                            segment.Size = new Vector2(36, 32);
                            break;
                        case 6:
                            segment.Size = new Vector2(36, 46);
                            break;
                    }

                    segments.Add(segment);
                }

                for (int i = 0; i < bodFrontExtraSegments; i++)
                {
                    segment = new RekSegment(NPC);
                    if(i % 2 == 0)
                    {
                        segment.TexturePath = $"{BaseTexturePath}RekBody2";
                        segment.Size = new Vector2(118, 106);
                    }
                    else
                    {
                        segment.TexturePath = $"{BaseTexturePath}RekBody3";
                        segment.Size = new Vector2(84, 98);
                    }
                    segment.Position = segment.Position + new Vector2(offset, 0);
                    offset += 0.1f;
                    segments.Add(segment);
                }

                //Front Tail Segments
                for (int i = 0; i < bodyExtraSegments; i++)
                {
                    segment = new RekSegment(NPC);
                    string texturePath = $"{BaseTexturePath}RekBody4";
                    segment.Size = new Vector2(68, 58);
                    if (i > bodyExtraSegments / 2)
                    {
                        texturePath = $"{BaseTexturePath}RekBody5";
                        segment.Size = new Vector2(62, 50);
                    }
                    segment.Position = segment.Position + new Vector2(offset, 0);
                    offset += 0.1f;
                    segment.TexturePath = texturePath;
                    segments.Add(segment);
                }

                //Tail Segments
                for (int i = 0; i < tailSegments; i++)
                {

                    float p = i;
                    float progress = p / tailSegments;
                    progress = 1 - progress;
                    string texturePath = $"{BaseTexturePath}RekBody6";
                    segment.Size = new Vector2(36, 32);
                    if (i > 2)
                    {
                        texturePath = $"{BaseTexturePath}RekBody7";
                        segment.Size = new Vector2(36, 46);
                    }
                    segment = new RekSegment(NPC);
                    segment.Position = segment.Position + new Vector2(offset, 0);
                    offset += 0.1f;
                    segment.TexturePath = texturePath;
                    segment.Scale = Math.Max(0.5f, progress);
                    segments.Add(segment);
                }

                segment = new RekSegment(NPC);
                segment.Position = segment.Position + new Vector2(offset, 0);
                offset += 0.1f;
                segment.TexturePath = $"{BaseTexturePath}RekTail";
                segment.Size = new Vector2(88, 18);
                segments.Add(segment);
                Segments = segments.ToArray();

                NPC.TargetClosest();
                return;
            }


            if (InPhase2 && !PoppedOutEye)
            {
                Head.TexturePath = $"{BaseTexturePath}RekSnakeNoEye";
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(EntitySource, (int)NPC.Center.X, (int)NPC.Center.Y,
                        ModContent.NPCType<RekFireEye>(), ai1: NPC.whoAmI);
                }
                PoppedOutEye = true;
            }


            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    NPC.EncourageDespawn(120);
                    NPC.velocity += -Vector2.UnitY * 0.03f;
                    NPC.rotation = NPC.velocity.ToRotation();
                    MakeLikeWorm();
                    return;
                }
            }

            switch (State)
            {
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Dash:
                    AI_Dash();
                    break;
                case ActionState.Clappback:
                    AI_Clappback();
                    break;
                case ActionState.EyePopout:
                    AI_EyePopout();
                    break;
                case ActionState.Beamer:
                    AI_Beamer();
                    break;
                case ActionState.Ouroboros:
                    AI_Ouroboros();
                    break;
                case ActionState.Crystal:
                    AI_Crystal();
                    break;
                case ActionState.Death:
                    AI_Death();
                    break;
            }
        }
       

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8, float accel = 16)
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
                NPC.velocity.X+= accel;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X-= accel;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y+= accel;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y-= accel;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        private void AI_Idle()
        {
            Timer++;

            NPC.velocity *= 0.98f;
            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if (Timer >= 15)
            {
                ResetState(ActionState.Dash);
            }
        }

        private void AI_Dash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HavocCharge"), NPC.position);

                Vector2 directionToPlayer = NPC.Center.DirectionTo(Target.Center);
                Vector2 targetVelocity = directionToPlayer * 1;
                NPC.velocity = NPC.rotation.ToRotationVector2() * 2;

                //Let's start glowing
                Color color;
                switch (AttackTimer)
                {
                    default:
                        color = Color.White;
                        break;
                    case 1:
                        color = Color.Orange;
                        break;
                    case 2:
                        color = Color.White;
                        break;
                }

                StartSegmentGlow(color);
            }

            //Glowing white
            if(Timer < 60)
            {
                SegmentStretch -= 0.01f;
                if(NPC.velocity.Length() < 32)
                {
                    NPC.velocity *= 1.08f;
                }
     
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi*2/60);
                //Some smoky dusts while charging up
                for (int i = 0; i < Segments.Length; i++)
                {
                    RekSegment segment = Segments[i];
                    if (segment.GlowWhite)
                    {
                        if (Main.rand.NextBool(8))
                        {
                            Vector2 randOffset = Main.rand.NextVector2Circular(32, 32);
                            Vector2 velocity = -(Vector2.UnitY * 5).RotatedByRandom(MathHelper.PiOver4 / 2);
                            Dust.NewDustPerfect(segment.Center + randOffset, ModContent.DustType<TSmokeDust>(), velocity, 0, Color.DarkGray, 0.5f).noGravity = true;
                        }

                        if (Main.rand.NextBool(32))
                        {
                            Dust.NewDustPerfect(segment.Center, ModContent.DustType<GlowDust>(),
                            (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                        }
                    }
                }

                float glowSpeed = 1 / 60f;
                GlowWhite(glowSpeed);
            }
            if(Timer > 60 && Timer < 70)
            {
                NPC.velocity *= 0.92f;
            }

            if (Timer == 70)
            {
                SegmentStretch = 0.66f;
                float attackProgress = (AttackTimer) / 3;
                float speed = 80 + attackProgress * 48;

                Vector2 directionToPlayer = NPC.Center.DirectionTo(Target.Center);
                Vector2 targetVelocity = directionToPlayer * speed;
                NPC.velocity = targetVelocity;

                //Turn on the trail and roar!!!
                DrawChargeTrail = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 64f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SNAKEROAR"), NPC.position);
                StopSegmentGlow();
            }

            if(Timer > 60)
            {
                float glowSpeed = 1 / 40f;
                GlowWhite(glowSpeed);
                NPC.velocity *= 0.98f;
       
            }

            if(Timer > 80)
            {
                if(NPC.velocity.Length() > 4)
                {
                    NPC.velocity *= 0.8f;
                }
             
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.PiOver4 / 40f);
            }

            if(Timer >= 100)
            {
                DrawChargeTrail = false;
                Timer = 0;
                AttackTimer++;
            }

            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if (AttackTimer >= 3)
            {
                ResetSegmentGlow();
                ResetState(ActionState.Clappback);
            }
        }

        private void AI_Clappback()
        {
            Timer++;
            NPC.velocity *= 0.98f;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HavocCharge"), NPC.position);

                Vector2 directionToPlayer = NPC.Center.DirectionTo(Target.Center);
                Vector2 targetVelocity = directionToPlayer * 1;
                NPC.velocity = NPC.rotation.ToRotationVector2() * 2;

                //Let's start glowing
                Color color;
                switch (AttackTimer)
                {
                    default:
                        color = Color.White;
                        break;
                    case 1:
                        color = Color.Orange;
                        break;
                    case 2:
                        color = Color.White;
                        break;
                }

                StartSegmentGlow(color);
            }

            //Glowing white
            if (Timer < 60)
            {
                SegmentStretch -= 0.01f;
                if (NPC.velocity.Length() < 32)
                {
                    NPC.velocity *= 1.08f;
                }

                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi * 2 / 60);
                //Some smoky dusts while charging up
                for (int i = 0; i < Segments.Length; i++)
                {
                    RekSegment segment = Segments[i];
                    if (segment.GlowWhite)
                    {
                        if (Main.rand.NextBool(8))
                        {
                            Vector2 randOffset = Main.rand.NextVector2Circular(32, 32);
                            Vector2 velocity = -(Vector2.UnitY * 5).RotatedByRandom(MathHelper.PiOver4 / 2);
                            Dust.NewDustPerfect(segment.Center + randOffset, ModContent.DustType<TSmokeDust>(), velocity, 0, Color.DarkGray, 0.5f).noGravity = true;
                        }

                        if (Main.rand.NextBool(32))
                        {
                            Dust.NewDustPerfect(segment.Center, ModContent.DustType<GlowDust>(),
                            (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                        }
                    }
                }

                float glowSpeed = 1 / 60f;
                GlowWhite(glowSpeed);
            }

            if (Timer > 60 && Timer < 90)
            {
                NPC.velocity *= 0.92f;
            }

            if(Timer < 90)
            {
                NPC.rotation = NPC.velocity.ToRotation();
                MakeLikeWorm();
            }

            if (Timer == 90)
            {
                //Turn on the trail and roar!!!
                MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                myPlayer.ShakeAtPosition(NPC.Center, 1024f, 64f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SNAKEROAR"), NPC.position);
            
                //Explode
                for (int i = 0; i < Segments.Length; i++)
                {
                    var segment = Segments[i];
                    segment.Velocity = Main.rand.NextVector2Circular(32, 32);
                    segment.Velocity.Y -= 24;
                }
            }

            if(Timer > 90 && Timer < 120)
            {
                ResetSegmentGlow();
                for (int i = 0; i < Segments.Length; i++)
                {
                    var segment = Segments[i];
                    segment.Position += segment.Velocity;
                    segment.Velocity.Y += 0.5f;
                    segment.Rotation += segment.Velocity.Length() * 0.025f;
                }
            }
   
            if(Timer > 120 && Timer < 240)
            {
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                NPC.velocity += directionToTarget;
                for (int i = 0; i < Segments.Length; i++)
                {
                    var segment = Segments[i];
                    segment.Position += segment.Velocity;
                    segment.Velocity *= 0.98f;
                    segment.Rotation += segment.Velocity.Length() * 0.025f;
                }
            }

            if(Timer == 240)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekClappbackStart"), NPC.position);
                StartSegmentGlow(Color.White);
            }

            if(Timer > 240 && Timer < 300)
            {
                GlowWhite(1 / 60f);
                float progress = (Timer - 240) / 30f;
                progress = Easing.InExpo(progress);
                for (int i = 0; i < Segments.Length; i++)
                {
                    float recoverSpeed = 64 * progress;
                    var segment = Segments[i];
                    float distanceToCenter = Vector2.Distance(segment.Center, NPC.Center);
                    if(distanceToCenter < recoverSpeed)
                    {
                        recoverSpeed = distanceToCenter;
                    }

                    segment.Position += segment.Center.DirectionTo(NPC.Center) * recoverSpeed;
                    segment.Rotation += segment.Velocity.Length() * 0.025f;
                }
            }

            if(Timer == 300)
            {
                //Explosion
                ResetSegmentGlow();
                MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                myPlayer.ShakeAtPosition(NPC.Center, 3000, 128);
                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.2f, 0.2f), blend: 0.15f, timer: 30);
                screenShaderSystem.TintScreen(Color.Red, 0.3f, timer: 30);

                if (StellaMultiplayer.IsHost)
                {
                    float knockback = 1;
                    Projectile.NewProjectile(EntitySource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<RekFireShockWave>(),
                        DamageFireShockWave, knockback);
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SNAKEROAR"), NPC.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekShockwave"), NPC.position);
                NPC.velocity = -Vector2.UnitY;
            }


            if(Timer > 300 & Timer < 310)
            {
                float maxSpeed = 45f;
                if(NPC.velocity.Length() < maxSpeed)
                {
                    NPC.velocity *= 2f;
                }
            }

            if(Timer > 310 && Timer < 370)
            {
                SegmentStretch = MathHelper.Lerp(0.1f, 0.66f, (Timer - 310f)/60f);
                NPC.velocity *= 0.99f;
                NPC.velocity = NPC.velocity.RotatedBy((MathHelper.TwoPi) / 60f);
            }

            if (Timer >= 300)
            {
                NPC.rotation = NPC.velocity.ToRotation();
                MakeLikeWorm();
            }

            if (Timer >= 370)
            {
                Timer = 0;
                StopSegmentGlow();
                ResetState(ActionState.EyePopout);
            }
        }

        private void AI_EyePopout()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                Vector2 laserDirection = NPC.Center.DirectionTo(Target.Center);
                if (InPhase2 && StellaMultiplayer.IsHost)
                {

                    float knockback = 1;
                    Projectile.NewProjectile(EntitySource, NPC.Center, laserDirection, ModContent.ProjectileType<RekFireBlowtorchProj>(),
                        DamageBlowtorch, knockback, ai1: NPC.whoAmI);

                }
            }

            if(Timer % (int)((720 * 2) / (float)Segments.Length) == 0)
            {
                Vector2 laserDirection = NPC.Center.DirectionTo(Target.Center);
                Vector2 laserVelocity = laserDirection * 40;
      
                if (!InPhase2 && StellaMultiplayer.IsHost)
                {
                    float knockback = 1;
                    Projectile.NewProjectile(EntitySource, NPC.Center, laserVelocity, ModContent.ProjectileType<RekFireEyeLaserMiniProj>(),
                        DamagePopoutEyeMiniLaser, knockback);

                    Projectile.NewProjectile(EntitySource, NPC.Center, laserVelocity * 0, ModContent.ProjectileType<CircleExplosionProj>(),
                        DamagePopoutEyeMiniLaser, knockback);
                }

                StartSegmentGlow((int)AttackTimer, Color.White);
                AttackTimer++;
                if (AttackTimer >= Segments.Length)
                    AttackTimer = 0;
            }

            GlowWhite(0.02f);
            float distance = 16;
            Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
            Vector2 initialSpeed = directionToTarget * 8;
            Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
            offset.Normalize();
            offset *= (float)(Math.Cos(Timer * 3 * (Math.PI / 180)) * (distance / 3));

            NPC.velocity = initialSpeed + offset;
            if (InPhase2)
            {
                NPC.velocity *= 1.25f;
            }

            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if(Timer >= 720)
            {
                Timer = 0;
                ResetSegmentGlow();
                ResetState(ActionState.Beamer);
            }
        }

        private void AI_Beamer()
        {
            Timer++;
            float rotationProgress = Timer / 90;
            rotationProgress = MathHelper.Clamp(rotationProgress, 0, 1);
            float rotationSpeed = MathHelper.Lerp(30, 180, rotationProgress);
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            float time = 12;
            if (Timer % (time * 2) == 0 && Timer != 0)
            {
                var segment = Segments[(int)AttackTimer];
                Vector2 directionToTarget = segment.Center.DirectionTo(Target.Center);
                Vector2 velocity = directionToTarget * 12;
                if (StellaMultiplayer.IsHost)
                {
                    if(AttackTimer % 3 == 0)
                    {
                        float knockback = 1;
                        Projectile.NewProjectile(EntitySource, segment.Center, velocity,
                            ModContent.ProjectileType<RekFireBlowtorchBlastProj>(), DamageBlowtorchBlast, knockback, Main.myPlayer);
                    }
                    else
                    {
                        float knockback = 1;
                        Projectile.NewProjectile(EntitySource, segment.Center, velocity,
                            ModContent.ProjectileType<RekFireBallProj>(), DamageBeamerFireBall, knockback, Main.myPlayer);

                        Projectile.NewProjectile(EntitySource, segment.Center, velocity * 0,
                            ModContent.ProjectileType<SmallCircleExplosionProj>(), DamageBeamerFireBall, knockback, Main.myPlayer);
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector2 dustSpeed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Dust d = Dust.NewDustPerfect(segment.Center + dustSpeed * 8, ModContent.DustType<GlowDust>(), dustSpeed * 5, Scale: 0.6f, newColor: Color.Orange); ;
                    d.noGravity = true;
                }

                StopSegmentGlow((int)AttackTimer);
                if (StellaMultiplayer.IsHost)
                {
                    AttackTimer = Main.rand.Next(0, Segments.Length);
                    NPC.netUpdate = true;
                }
            }

            if (Timer % time == 0)
            {
                StartSegmentGlow((int)AttackTimer, Color.White);
            }

            float glowSpeed = 1 / time;
            GlowWhite(glowSpeed);

            //Movement
            Vector2 direction = Target.Center.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / rotationSpeed);
            Vector2 targetCenter = Target.Center + direction * 444;
            float speed = 96;
            float accel = 16;
            AI_MoveToward(targetCenter, speed, accel);

            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if(Timer >= 720)
            {
                Timer = 0;
                ResetSegmentGlow();
                ResetState(ActionState.Ouroboros);
            }
        }

        private void AI_Ouroboros()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                StartSegmentGlow(Color.Orange);
            }


            if (AttackCycle == 0)
            {
                var targetSegment = Segments[Segments.Length - (int)AttackTimer - 1];
                float endSegment = Segments.Length - 5;
                float progress = AttackTimer / endSegment;
                float speed = 1 + (7f * (1f - progress));
                float accel = 1;
                AI_MoveToward(targetSegment.Center, speed, accel);
                NPC.rotation = NPC.velocity.ToRotation();

                if (AttackTimer < endSegment)
                {
                    if (Vector2.Distance(NPC.Center, targetSegment.Center) <= 32)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/REKEAT"), NPC.position);
                        if (StellaMultiplayer.IsHost)
                        {
                            float rot = targetSegment.Rotation - MathHelper.PiOver2;
                            Vector2 velocity = rot.ToRotationVector2();
                            float knockback = 1;
                            Projectile.NewProjectile(EntitySource, targetSegment.Center, velocity,
                                ModContent.ProjectileType<RekFireBlowtorchBlastProj>(), DamageBlowtorchBlast, knockback, Main.myPlayer);
                        }
                      
                        targetSegment.Eaten = true;
                        AttackTimer++;
                    }
                }
                if (AttackTimer >= endSegment)
                {
                    AttackTimer = 0;
                    AttackCycle++;
                }
            }
            else if (AttackCycle == 1)
            {
                int explosionTime = 120;
                AttackTimer++;
                if (AttackTimer  == 1)
                {
                    StopSegmentGlow();
                }

                if(AttackTimer == explosionTime / 2)
                {
                    StartSegmentGlow(Color.White);           
                }


                GlowWhite(1 / 60f);
                if (AttackTimer == explosionTime)
                {
                    for (int i = 0; i < Segments.Length; i++)
                    {
                        var segment = Segments[i];
                        segment.Eaten = false;
                    }

                    ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    screenShaderSystem.TintScreen(Color.Orange, 0.3f, timer: 30f);
                    screenShaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.5f, 0.5f), timer: 30);
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 3000f, 48);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekOuroborosExplosion"), NPC.Center);
                    float num = 8;
                    for (int i = 0; i < num; i++)
                    {
                        float progress = i / num;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 velocity = rot.ToRotationVector2();
                        if (StellaMultiplayer.IsHost)
                        {
                            float knockback = 1;
                            Projectile.NewProjectile(EntitySource, NPC.Center, velocity,
                                ModContent.ProjectileType<RekFireOuroborosProj>(), DamageBlowtorchExplosion, knockback, Main.myPlayer);
                        }
                    }

                    NPC.velocity = -Vector2.UnitY;
                }

                if (AttackTimer > explosionTime && AttackTimer < explosionTime + 10)
                {
                    float maxSpeed = 45f;
                    if (NPC.velocity.Length() < maxSpeed)
                    {
                        NPC.velocity *= 2f;
                    }
                }

                if (AttackTimer > explosionTime + 10 && AttackTimer < explosionTime + 70)
                {
                    NPC.velocity *= 0.99f;
                    NPC.velocity = NPC.velocity.RotatedBy((MathHelper.TwoPi) / 60f);
                }
        
                if(AttackTimer >= explosionTime + 70)
                {
                    AttackTimer = 0;
                    AttackCycle++;
                }
            }
    
    
            MakeLikeWorm(); 
            if (AttackCycle == 2)
            {
                ResetState(ActionState.Crystal);
            }
        }

        private void AI_Crystal()
        {
            Timer++;
            float rotationProgress = Timer / 180f;
            rotationProgress = MathHelper.Clamp(rotationProgress, 0, 1);
            rotationProgress = Easing.SpikeInOutExpo(rotationProgress);
            float rotationSpeed = MathHelper.Lerp(180, 30, rotationProgress);
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CrystalPosition = Target.Center;
                StartSegmentGlow(Color.White);
                NPC.netUpdate = true;
            }


            if (Timer > 30 && Timer < 90)
            {
                Vector2 offset = new Vector2(0, -61);
                int dustType = ModContent.DustType<GlowDust>();
                if (Main.rand.NextBool(3))
                {
      
                    Vector2 randPos = CrystalPosition + offset + Main.rand.NextVector2CircularEdge(196, 196);
                    Vector2 velocity = randPos.DirectionTo(CrystalPosition + offset) * 4;
                    float scale = Main.rand.NextFloat(0.5f, 1f);
                    Dust.NewDustPerfect(randPos, dustType, velocity, Alpha: 0, newColor: Color.White, Scale: scale);

                }

                float dustProgress = (Timer - 30f) / 60f;

                float centerScale = dustProgress * 4f;
                Dust.NewDustPerfect(CrystalPosition + offset, dustType, Vector2.Zero, Alpha: 0, newColor: Color.White, Scale: centerScale);
            }

            if(Timer == 90)
            {
                StopSegmentGlow();
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(EntitySource, (int)CrystalPosition.X, (int)CrystalPosition.Y, ModContent.NPCType<RekFireCrystal>());
                }
            }

            float glowSpeed = 1 / 90f;
            GlowWhite(glowSpeed);

            //Movement
            Vector2 direction = CrystalPosition.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / rotationSpeed);
            Vector2 targetCenter = CrystalPosition + direction * 444;
            float speed = 96;
            float accel = 16;
            AI_MoveToward(targetCenter, speed, accel);

            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if (Timer >= 720)
            {
                Timer = 0;
                StopSegmentGlow();
                ResetState(ActionState.Idle);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (NPC.life <= 0)
            {
                NPC.life = 1;
                if(State != ActionState.Death)
                {
                    ResetState(ActionState.Death);
                }
            }
        }

        private void AI_Death()
        {
            Timer++;
            if(Timer % 2 == 0)
            {
                int randSegment = Main.rand.Next(0, Segments.Length);
                Color color;
                switch (Main.rand.Next(2))
                {
                    default:
                    case 0:
                        color = Color.White;
                        break;
                    case 1:
                        color = Color.Orange;
                        break;
                }
                StartSegmentGlow(randSegment, color);
            }

            SegmentStretch = MathHelper.Lerp(0.66f, 0.1f, Timer / 300f);
            GlowWhite(0.5f);
            NPC.velocity *= 0.98f;
            NPC.rotation = NPC.velocity.ToRotation();
            MakeLikeWorm();
            if(Timer >= 300)
            {
                //DIE NOWWWW!!!
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekDeath"), NPC.position);

                MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                myPlayer.ShakeAtPosition(NPC.position, 6000, 128);

                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Orange, 0.3f, timer: 5);
                screenShaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.5f, 0.5f), timer: 15);

                if (StellaMultiplayer.IsHost)
                {
                    for(float i = 0; i < 8; i++)
                    {
                        float progress = i / 8f;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 direction = rot.ToRotationVector2();
                        Projectile.NewProjectile(EntitySource, NPC.Center, direction,
                            ModContent.ProjectileType<RekFireBlowtorchBlastDeathProj>(), 0, 0, Main.myPlayer);
                    }
                }
                NPC.Kill();
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            //	npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));




            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<RekBossRel>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 13, 25));    
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SerpentWings>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<SerpentStaff>(),
                ModContent.ItemType<Incinerator>(),
                ModContent.ItemType<YourFired>(),
                ModContent.ItemType<BlackEye>(),
                ModContent.ItemType<VulcanBreaker>()
                ));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));



            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added

            /*
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<BurningGBroochA>(),
                ModContent.ItemType<Gothinstein>(),
                ModContent.ItemType<BurnBlast>(),
                ModContent.ItemType<WeddingDay>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Plate>(), minimumDropped: 200, maximumDropped: 1300));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 4, maximumDropped: 55));
           
            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

            // This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
            // We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
            // which requires these parameters to be defined
            //int itemType = ModContent.ItemType<Gambit>();
            //var parameters = new DropOneByOne.Parameters()
            //{
            //	ChanceNumerator = 1,
            //	ChanceDenominator = 1,
            //	MinimumStackPerChunkBase = 1,
            //	MaximumStackPerChunkBase = 1,
            //	MinimumItemDropsCount = 1,
            //	MaximumItemDropsCount = 3,
            //};

            //notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
             */
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedRekBoss, -1);
        }

        #region Draw Code

        private void GlowWhite(float speed)
        {
            if (Segments == null)
                return;
            for(int i = 0; i < Segments.Length; i++)
            {
                RekSegment segment = Segments[i];
                if (segment.GlowWhite)
                {
                    segment.GlowTimer += speed;
                    if (segment.GlowTimer >= 1f)
                        segment.GlowTimer = 1f;
                }
                else
                {
                    segment.GlowTimer -= speed;
                    if (segment.GlowTimer <= 0)
                        segment.GlowTimer = 0;
                }
            }
        }

        private void MakeLikeWorm()
        {
            if (Segments == null)
                return;

            //Segments
            Head.Position = NPC.position;
            Head.Rotation = NPC.rotation;
            MoveSegmentsLikeWorm();
        }

        private void StartSegmentGlow( Color color)
        {
            if (Segments == null)
                return;
            for(int i = 0; i < Segments.Length; i++)
            {
                StartSegmentGlow(i, color);
            }
        }

        private void StartSegmentGlow(int index, Color color)
        {
            if (Segments == null)
                return;
            RekSegment segment = Segments[index];
            segment.GlowWhiteColor = color;
            segment.GlowWhite = true;
        }

        private void StopSegmentGlow()
        {
            if (Segments == null)
                return;
            for (int i = 0; i < Segments.Length; i++)
            {
                StopSegmentGlow(i);
            }
        }

        private void StopSegmentGlow(int index)
        {
            if (Segments == null)
                return;
            RekSegment segment = Segments[index];
            segment.GlowWhite = false;
        }

        private void ResetSegmentGlow()
        {
            if (Segments == null)
                return;
            for (int i = 0; i < Segments.Length; i++)
            {
                ResetSegmentGlow(i);
            }
        }

        private void ResetSegmentGlow(int index)
        {
            if (Segments == null)
                return;
            RekSegment segment = Segments[index];
            segment.GlowWhite = false;
            segment.GlowTimer = 0;
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

            ref var segment = ref Segments[index];
            ref var frontSegment = ref Segments[index - 1];

            // Follow behind the segment "in front" of this NPC
            // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.
            float dirX = frontSegment.Position.X - segment.Position.X;
            float dirY = frontSegment.Position.Y - segment.Position.Y;

            // We then use Atan2 to get a correct rotation towards that parent NPC.
            // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
            segment.Rotation = (float)Math.Atan2(dirY, dirX);
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            if (length == 0)
                length = 1;

            // We calculate a new, correct distance.

            float fixer = 1;
            if (index == Segments.Length - 1)
            {
                fixer /= 1.75f;
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


        public float WidthFunctionCharge(float completionRatio)
        {
            return NPC.width * NPC.scale * (1f - completionRatio) * 2f;
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

            return Color.Lerp(Color.Orange, Color.Orange, (1f - completionRatio)) * ChargeTrailOpacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Segments == null)
                return false;
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            Vector2 size = new Vector2(90, 90);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:BasicTrail"];
            TrailDrawer.DrawPrims(NPC.oldPos, size * 0.5f - screenPos, 155);

            //Draw all the segments
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                RekSegment segment = Segments[i];
                if (segment.Eaten)
                    continue;

                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale * segment.Scale;
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
                RekSegment segment = Segments[i];
                if (segment.Eaten)
                    continue;

                if (!ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_Glow", out var asset))
                    continue;

                Vector2 drawPosition = segment.Position - screenPos + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = NPC.scale;

                float osc = VectorHelper.Osc(0, 1);
   
                spriteBatch.Draw(asset.Value, drawPosition, null, drawColor * osc, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

                for (float j = 0f; j < 1f; j += 0.25f)
                {
                    float radians = (j + osc) * MathHelper.TwoPi;
                    spriteBatch.Draw(segment.GlowTexture, drawPosition + new Vector2(0f, 8f).RotatedBy(radians) * osc,
                        null, Color.White * osc * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }

                if (segment.GlowTimer > 0 && ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_White", out var whiteAsset))
                {
                    spriteBatch.Draw(whiteAsset.Value, drawPosition, null, segment.GlowWhiteColor * segment.GlowTimer, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }
        }
        #endregion
    }
}
