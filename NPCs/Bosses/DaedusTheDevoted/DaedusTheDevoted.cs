using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles;
using Stellamod.NPCs.Town;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Conditions = Terraria.GameContent.ItemDropRules.Conditions;
using Gambit = Stellamod.Items.Consumables.Gambit;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted
{
    internal class BaseDaedusSegment
    {
        public BaseDaedusSegment(NPC npc)
        {
            NPC = npc;
        }

        public float frameCounter;
        public int frame;
        public NPC NPC { get; init; }
        public string BaseTexturePath => "Stellamod/NPCs/Bosses/DaedusTheDevoted/";
        public virtual void AI() { }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { }
    }

    internal class DaedusTopSegment : BaseDaedusSegment
    {
        public DaedusTopSegment(NPC npc) : base(npc) { }
        public Rectangle AnimationFrame { get; set; }
        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusTop").Value;
            frameCounter += 0.5f;
            if (frameCounter >= 1f)
            {
                frameCounter = 0;
                frame++;
                if (frame >= 60)
                {
                    frame = 0;
                }
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 60);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusTop").Value;
            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    internal class DaedusFaceSegment : BaseDaedusSegment
    {
        public enum AnimationState
        {
            Laughing,
            Smile,
            Scared
        }

        public DaedusFaceSegment(NPC npc) : base(npc) { Animation = AnimationState.Smile; }
        public AnimationState Animation { get; set; }
        public Rectangle AnimationFrame { get; set; }
        public bool Glow { get; set; }
        public float GlowTimer { get; set; }

        public float BlackTimer { get; set; }
        private bool CheckCurrentAnimation(params AnimationState[] animations)
        {
            for (int i = 0; i < animations.Length; i++)
            {
                AnimationState animation = animations[i];
                if (Animation == animation)
                    return true;
            }
            return false;
        }

        public void SwitchToAnimation(AnimationState newState, params AnimationState[] animations)
        {
            if (!CheckCurrentAnimation(animations))
            {
                Animation = newState;
            }
        }

        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusFace").Value;
            frameCounter += 0.35f;
            if (frameCounter >= 1f)
            {
                frameCounter = 0;
                frame++;
            }


            switch (Animation)
            {
                default:
                case AnimationState.Laughing:
                    if (frame >= 3)
                    {
                        frame = 0;
                    }
                    break;
                case AnimationState.Smile:
                    frame = 3;
                    break;
                case AnimationState.Scared:
                    if (frame < 4 || frame >= 6)
                    {
                        frame = 4;
                    }
                    break;
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 6);
            if (Glow)
            {
                GlowTimer = MathHelper.Lerp(GlowTimer, 1f, 0.01f);
            }
            else
            {
                GlowTimer = MathHelper.Lerp(GlowTimer, 0f, 0.01f);
            }
            BlackTimer = MathHelper.Lerp(BlackTimer, 0f, 0.1f);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusFace").Value;
            Vector2 drawPos = NPC.Center - screenPos;


            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);

            //Ok so we need some glowing huhh
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float f = 0; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f, speed: 3);
                Vector2 glowDrawPos = drawPos + offset;
                spriteBatch.Draw(texture, glowDrawPos, AnimationFrame, drawColor * GlowTimer, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(texture, drawPos, AnimationFrame, Color.Black * BlackTimer, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);

        }
    }

    internal class DaedusArmSegment : BaseDaedusSegment
    {
        private AnimationState _animationState;
        public enum AnimationState
        {
            Raise,
            Hold_Up,
            Lower,
            Hold_Down
        }

        public DaedusArmSegment(NPC npc) : base(npc) { Animation = AnimationState.Hold_Down; }
        public AnimationState Animation
        {
            get
            {
                return _animationState;
            }
            set
            {
                switch (value)
                {
                    case AnimationState.Raise:
                        if (CheckCurrentAnimation(AnimationState.Raise, AnimationState.Hold_Up))
                            return;
                        break;
                    case AnimationState.Lower:
                        if (CheckCurrentAnimation(AnimationState.Lower, AnimationState.Hold_Down))
                            return;
                        break;
                }
                _animationState = value;
            }
        }
        public Rectangle AnimationFrame { get; set; }
        public bool Fast { get; set; }

        private bool CheckCurrentAnimation(params AnimationState[] animations)
        {
            for (int i = 0; i < animations.Length; i++)
            {
                AnimationState animation = animations[i];
                if (Animation == animation)
                    return true;
            }
            return false;
        }

        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusArms").Value;
            frameCounter += 0.5f;
            if (Fast)
            {
                frameCounter += 0.5f;
            }

            if (frameCounter >= 1f)
            {
                frameCounter = 0;
                frame++;
            }

            switch (Animation)
            {
                default:
                case AnimationState.Raise:
                    if (frame > 9)
                    {
                        Animation = AnimationState.Hold_Up;
                    }
                    break;
                case AnimationState.Hold_Up:
                    frame = 10;
                    break;
                case AnimationState.Lower:
                    if (frame < 11)
                    {
                        frame = 11;
                    }
                    if (frame > 17)
                    {
                        frame = 0;
                        Animation = AnimationState.Hold_Down;
                    }
                    break;
                case AnimationState.Hold_Down:
                    frame = 0;
                    break;
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 17);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusArms").Value;
            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    internal class DaedusBackSegment : BaseDaedusSegment
    {
        public DaedusBackSegment(NPC npc) : base(npc) { }
        public Rectangle AnimationFrame { get; set; }
        public override void AI()
        {
            base.AI();
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusBack").Value;
            frameCounter += 0.5f;
            if (frameCounter >= 1f)
            {
                frame++;
                if (frame >= 60)
                {
                    frame = 0;
                }
            }

            AnimationFrame = texture.GetFrame(frame, totalFrameCount: 60);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusBack").Value;
            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = AnimationFrame.Size() / 2f;
            //Ok so we need some glowing huhh
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
           
            
            for(float f = 0; f < 1f; f += 0.1f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(4f, 8f, speed: 3);
                Vector2 glowDrawPos = drawPos + offset;
                spriteBatch.Draw(texture, glowDrawPos, AnimationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(texture, drawPos, AnimationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    internal class DaedusRobeSegment : BaseDaedusSegment
    {
        public DaedusRobeSegment(NPC npc) : base(npc) { }
        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusRobe").Value;
            MiscShaderData shaderData = GameShaders.Misc["LunarVeil:DaedusRobe"];
            shaderData.Shader.Parameters["windNoiseTexture"].SetValue(TextureRegistry.CloudNoise.Value);

            float speed = 1;
            shaderData.Shader.Parameters["uImageSize0"].SetValue(texture.Size());
            shaderData.Shader.Parameters["startPixel"].SetValue(60);
            shaderData.Shader.Parameters["endPixel"].SetValue(115);
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * speed);
            shaderData.Shader.Parameters["distortionStrength"].SetValue(0.075f);


            Vector2 vel = -NPC.velocity * 0.1f;
            vel.Y *= 0.25f;
            shaderData.Shader.Parameters["movementVelocity"].SetValue(vel);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, shaderData.Shader, Main.GameViewMatrix.TransformationMatrix);


            Vector2 drawPos = NPC.Center - screenPos;

            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, null, drawColor, 0f, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    [AutoloadBossHead]
    internal class DaedusTheDevoted : ModNPC
    {
        private enum AIState
        {
            Idle,
            Lightning_Strike, // P1
            Conjure_Ball_Lightning, // P1
            Conjure_Ball_Lightning_Mega,
            Electric_Tentacle, // P1
            Electric_Field,
            Ground_Explosion, // P1
            Singularity, // P1
            Thunderslap,
            Jack_Fire, // P1
            Phase_2_Transition,
            Death,
        }

        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackCounter => ref NPC.ai[2];
        private ref float AttackCycle => ref NPC.ai[3];

        private bool InPhase2 => NPC.life < NPC.lifeMax / 2f;
        private bool Phase2Transition;
        private float Phase2WingsProgress;


        private Vector2 TargetMovePos;
        private Vector2 TeleportTarget;
        private Vector2 BigTeleportTarget;
        private float TeleportCount;
        private Vector2[] _lightningZaps = new Vector2[12];
        private Vector2[] _blackLightningZaps = new Vector2[12];
        private Vector2[] _blackLightningZaps2 = new Vector2[12];
        private Player Target => Main.player[NPC.target];


        //For Draw Code
        public DaedusTopSegment TopSegment { get; set; }
        public DaedusFaceSegment FaceSegment { get; set; }
        public DaedusBackSegment BackSegment { get; set; }
        public DaedusArmSegment ArmSegment { get; set; }
        public DaedusRobeSegment RobeSegment { get; set; }
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public CommonLightning BlackLightning { get; set; } = new CommonLightning();
        public float LightningBallTimer { get; set; }

        //Damage Numbers
        private int LightningStrikeDamage => 21;
        private int MiniLightningBallDamage => 21;
        private int ConjureBallLightningDamage => 42;
        private int ElectricTentacleDamage => 20;
        private int ElectricFieldDamage => 16;
        private int SingularityDamage => 20;
        private int ThunderslapDamage => 20;
        private int JackFireDamage => 12;
        private int GroundFireDamage => 20;


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(TeleportTarget);
            writer.WriteVector2(BigTeleportTarget);
            writer.Write(Phase2Transition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TeleportTarget = reader.ReadVector2();
            BigTeleportTarget = reader.ReadVector2();
            Phase2Transition = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            Main.npcFrameCount[NPC.type] = 46;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Stellamod/NPCs/Bosses/DaedusRework/DaedusBestiary",
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 14;
            NPC.defense = 10;
            NPC.lifeMax = 2600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.takenDamageMultiplier = 0.9f;
            NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();

            NPC.aiStyle = 0;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Daedus");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            //Teleport Go!!!
            if(TeleportTarget != Vector2.Zero)
            {
                NPC.Center = TeleportTarget;
                for(int i =0; i < 24; i++)
                {
                    float progress = i / 24f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 velocity = rot.ToRotationVector2() * 16;
                    Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, Velocity: velocity);
                }

                TeleportTarget = Vector2.Zero;

                SoundStyle teleportStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
                teleportStyle.PitchVariance = 0.05f;
                teleportStyle.Pitch = TeleportCount * 0.05f;
                SoundEngine.PlaySound(teleportStyle, NPC.position);
                TeleportCount++;
            }


            //HUGE AHH TELEPORT
            if (BigTeleportTarget != Vector2.Zero)
            {
                NPC.Center = BigTeleportTarget;
                for (int i = 0; i < 48f; i++)
                {
                    float progress = i / 48f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 velocity = rot.ToRotationVector2() * 16;
                    Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, Velocity: velocity);
                }

                BigTeleportTarget = Vector2.Zero;
                SoundStyle teleportStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3");
                SoundEngine.PlaySound(teleportStyle, NPC.position);
            }

            Lightning.WidthMultiplier = LightningBallTimer;
            if (Timer % 3 == 0)
            {
                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    float progress = (float)i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + (Timer * 0.05f);
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(0, 32, speed: 3);
                    _lightningZaps[i] = NPC.Center + offset + new Vector2(0, -64);
                }

                float flapSpeed = 1.25f;
                for (int i = 0; i < _blackLightningZaps.Length; i++)
                {
                    float progress = (float)i / (float)_lightningZaps.Length;
                    float offset = progress * MathHelper.ToRadians(120);
                    Vector2 rotatedOffset = Vector2.UnitY.RotatedBy(offset + (VectorHelper.Osc(0.9f, 1.0f, 9))).RotatedByRandom(MathHelper.PiOver4 / 24f);
                    rotatedOffset = rotatedOffset.RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * 5f * flapSpeed) * 0.5f - 0.25f);
                    rotatedOffset.X *= VectorHelper.Osc(0.2f, 1f, speed: 5 * flapSpeed);
                    Vector2 rotatedVector = (rotatedOffset * 128 * VectorHelper.Osc(0.9f, 1f, 9));
                 
                    if (i % 8 == 0)
                    {
                        _blackLightningZaps[i] = NPC.Center + rotatedVector * 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly * 4) * Phase2WingsProgress;

                        rotatedOffset.X = -rotatedOffset.X;
                        Vector2 invert = (rotatedOffset * 128 * VectorHelper.Osc(0.9f, 1f, 9));
                        _blackLightningZaps2[i] = NPC.Center + invert * 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly * 4) * -1f * Phase2WingsProgress;
                    }
                    else
                    {
                        _blackLightningZaps[i] = NPC.Center + rotatedVector * -1f * Phase2WingsProgress;

                        rotatedOffset.X = -rotatedOffset.X;
                        Vector2 invert = (rotatedOffset * 128 * VectorHelper.Osc(0.9f, 1f, 9));
                        _blackLightningZaps2[i] = NPC.Center + invert * -1f * Phase2WingsProgress;
                    }
                }
                Lightning.RandomPositions(_lightningZaps);
                BlackLightning.RandomPositions(_blackLightningZaps);
            }

            //Animations
            TopSegment ??= new DaedusTopSegment(NPC);
            TopSegment.AI();

            FaceSegment ??= new DaedusFaceSegment(NPC);
            FaceSegment.AI();

            BackSegment ??= new DaedusBackSegment(NPC);
            BackSegment.AI();

            ArmSegment ??= new DaedusArmSegment(NPC);
            ArmSegment.AI();

            RobeSegment ??= new DaedusRobeSegment(NPC);
            RobeSegment.AI();

            if (InPhase2 && Phase2Transition)
            {
                Phase2WingsProgress = MathHelper.Lerp(Phase2WingsProgress, 1f, 0.01f);
            }

            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Lightning_Strike:
                    AI_LightningStrike();
                    break;
                case AIState.Conjure_Ball_Lightning:
                    AI_ConjureBallLightning();
                    break;
                case AIState.Conjure_Ball_Lightning_Mega:
                    AI_ConjureBallLightningMega();
                    break;
                case AIState.Electric_Tentacle:
                    AI_ElectricTentacle();
                    break;
                case AIState.Electric_Field:
                    AI_ElectricField();
                    break;
                case AIState.Singularity:
                    AI_Singularity();
                    break;
                case AIState.Thunderslap:
                    AI_Thunderslap();
                    break;
                case AIState.Jack_Fire:
                    AI_JackFire();
                    break;
                case AIState.Ground_Explosion:
                    AI_GroundExplosion();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
                case AIState.Phase_2_Transition:
                    AI_Phase2Transition();
                    break;
            }

            float targetRotation = NPC.velocity.X * 0.025f;
            float lerpedRotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.2f);
            NPC.rotation = lerpedRotation;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw the segments
            if (InPhase2)
            {
                BlackLightning.SetBoltDefaults();
                BlackLightning.WidthMultiplier = 1;
                BlackLightning.Draw(spriteBatch, _blackLightningZaps, null);
                BlackLightning.Draw(spriteBatch, _blackLightningZaps2, null);
            }
          
            BackSegment.Draw(spriteBatch, screenPos, drawColor);
            ArmSegment.Draw(spriteBatch, screenPos, drawColor);
            TopSegment.Draw(spriteBatch, screenPos, drawColor);
            RobeSegment.Draw(spriteBatch, screenPos, drawColor);
            FaceSegment.Draw(spriteBatch, screenPos, drawColor);
            Lightning.Draw(spriteBatch, _lightningZaps, NPC.oldRot);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);
        }

        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                AttackCounter = 0;
                NPC.netUpdate = true;
            }
        }

        private void AI_Idle()
        {
            Timer++;
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -8), 0.025f);
                NPC.EncourageDespawn(60);
                return;
            }

            ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
            FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;


            Vector2 offset = new Vector2(0, -252);
            Vector2 targetPos = Target.Center + offset;
            Vector2 velocityToTarget = targetPos - NPC.Center;
            float m = InPhase2 ? 0.06f : 0.03f;
            Vector2 targetVelocity = velocityToTarget * m;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

            //Lighting strike attack - He strikes the players specifically making the player dodge

            //Conjure ball lighting - He raises his arms and summons a giant ball of lighting that hits the ground and explodes

            //His normal ground explosion thing

            //Electric tentacle that circles around the arena making the player dodge

            //Little electric fields that hurt the player when moving in them(ph2)

            //He brings out a small singularity and small black electricity shoots out from it to the player and random other positions

            //He can hover over the player and do a thunderslap and the player has to dodge, the lower health he can do it more(ph2)

            //Jack summon fire but slightly bigger
            float timeToWait = 120;

            //FAST
            if (InPhase2)
                timeToWait = 90;
            if (Timer >= timeToWait)
            {
                //How we choosing attack uhh, oh i know
                if (StellaMultiplayer.IsHost)
                {
                    AIState nextAttack = AIState.Lightning_Strike;
                    switch (AttackCycle)
                    {
                        case 0:
                            if (Main.rand.NextBool(2))
                            {
                                nextAttack = AIState.Lightning_Strike;
                            }
                            else
                            {
                                nextAttack = AIState.Conjure_Ball_Lightning;
                            }

                      
                            break;
                        case 1:
                            if (Main.rand.NextBool(2))
                            {
                                nextAttack = AIState.Electric_Tentacle;
                            }
                            else
                            {
                                nextAttack = AIState.Ground_Explosion;
                            }
                            break;
                        case 2:
                            if (Main.rand.NextBool(2))
                            {
                                nextAttack = AIState.Singularity;
                            }
                            else
                            {
                                nextAttack = AIState.Jack_Fire;
                            }
                            break;
                        case 3:
                            if (InPhase2)
                            {
                                nextAttack = AIState.Electric_Field;
                            }
                            else
                            {
                                nextAttack = AIState.Conjure_Ball_Lightning_Mega;
                            }
                          
                            break;
                        case 4:
                            if (Main.rand.NextBool(2))
                            {
                                nextAttack = AIState.Thunderslap;
                            }
                            else
                            {
                                nextAttack = AIState.Conjure_Ball_Lightning;
                            }

                            break;
                        case 5:
                            nextAttack = AIState.Conjure_Ball_Lightning_Mega;
                            break;
                    }
                    AttackCycle++;
                    if (InPhase2)
                    {
                        if (AttackCycle >= 6)
                        {
                            AttackCycle = 0;
                        }

                    }
                    else
                    {
                        if (AttackCycle >= 4)
                        {
                            AttackCycle = 0;
                        }

                    }


                    if (!Phase2Transition && InPhase2)
                    {
                        nextAttack = AIState.Phase_2_Transition;

                    }

                    SwitchState(nextAttack);
                }
            }
        }

        private void AI_Phase2Transition()
        {
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    NPC.velocity.X *= 0.98f;
                    if(NPC.velocity.Y < 11)
                    {
                        NPC.velocity.Y += 0.33f;
                    }
               

                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Scared;
                    if(Timer >= 90)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;
                case 1:
                    Timer++;
                    FaceSegment.Glow = true;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);

                    float transitionProgress = Timer / 300f;
                    float divisor = (int)MathHelper.Lerp(60, 20, transitionProgress);
                    if(Timer % divisor == 0)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);
                        if (StellaMultiplayer.IsHost)
                        {
                            TeleportTarget = Target.Center + Main.rand.NextVector2Circular(256, 256);
                            NPC.velocity = Main.rand.NextVector2Circular(8, 8);
                            NPC.netUpdate = true;
                        }
                    }
                    if(Timer >= 300f)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;
                case 2:
                    Timer++;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    if(Timer == 30)
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                            BigTeleportTarget = Target.Center + new Vector2(0, -128);
                            NPC.velocity = Vector2.Zero;
                            NPC.netUpdate = true;
                        }
                    }

                    if(Timer >= 90)
                    {
                        Phase2Transition = true;
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_GroundExplosion()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;

                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    Vector2 offset = new Vector2(0, -252);
                    Vector2 targetPos = Target.Center + offset;
                    Vector2 v = targetPos - NPC.Center;
                    Vector2 tv = v * 0.07f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);
                    LightningBallTimer += 1 / 30f;
                    if (Timer % 12 == 0)
                    {
                        SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                        soundStyle.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                    }

                    if (Timer > 120)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        Timer = 0;
                        AttackCounter++;
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = 21;
                            int knockback = 1; 
                            Vector2 startPos = Target.Center;
                            startPos.Y -= 128;

                            for (int i = 0; i < 10; i++)
                            {
                                Vector2 firePos = startPos;
                                firePos.X += MathHelper.Lerp(-700, 700, (float)i / 10f);
                                float length = ProjectileHelper.PerformBeamHitscan(firePos, Vector2.UnitY, maxBeamLength: 2400);
                                firePos.Y += length;

                                Vector2 fireVelocity = -Vector2.UnitY * 7;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, fireVelocity,
                                    ModContent.ProjectileType<FireRiseWarn>(), damage, knockback, Main.myPlayer);
                            }
                        }
                    }

                    break;

                case 2:
                    Timer++;

                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_LightningStrike()
        {    
            //Lighting strike attack - He strikes the players specifically making the player dodge
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        if (StellaMultiplayer.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), 
                                Target.Center - new Vector2(0, 128), Vector2.UnitY * 64,
                                ModContent.ProjectileType<LightningStrikeWarn>(), 0, 0, Main.myPlayer, ai1: Target.whoAmI);
                        }
                        TargetMovePos = Target.Center - new Vector2(0, 512);
                    }

                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;

                    NPC.velocity *= 0.96f;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;

                    if (Timer > 20 && Timer % 30 == 0)
                    {
                        FaceSegment.BlackTimer = 1f;
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = LightningStrikeDamage;
                            int knockback = 1;
                            Vector2 firePos = Target.Center - new Vector2(0, 512);
     
                            float charge = Timer / 90f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, Vector2.UnitY,
                                ModContent.ProjectileType<LightningStrike>(), damage, knockback, Main.myPlayer);
                        }
                    }

                    if (Timer >= 90)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 2:
                    Timer++;
                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_ConjureBallLightning()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if(Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if(Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if(Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;
                    NPC.velocity *= 0.96f;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    LightningBallTimer += 1 / 5f;
                    if (Timer > 20 && Timer % 30 == 0)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = MiniLightningBallDamage;
                            int knockback = 1;
                            Vector2 firePos = lightningSpawnPos;
                            Vector2 fireVelocity = (Target.Center - firePos).SafeNormalize(Vector2.Zero);
                            fireVelocity *= 7;
                            float charge = Timer / 90f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, fireVelocity,
                                ModContent.ProjectileType<ConjureBallLightning>(), damage, knockback, Main.myPlayer,
                                ai1: charge);
                        }
                    }

                    if(Timer >= 90)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 2:
                    Timer++;

                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if(Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_ElectricTentacle()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;
                  
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    Vector2 offset = new Vector2(0, -252);
                    Vector2 targetPos = Target.Center + offset;
                    Vector2 v = targetPos - NPC.Center;
                    Vector2 tv = v * 0.07f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);
                    LightningBallTimer += 1 / 30f;
                    if(Timer % 12 == 0)
                    {
                        SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                        soundStyle.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                    }

                    if (Timer > 120)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        Timer = 0;
                        AttackCounter++;
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = 21;
                            int knockback = 1;
                            Vector2 firePos = lightningSpawnPos;
                            Vector2 fireVelocity = Vector2.UnitX * 7;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, fireVelocity,
                                ModContent.ProjectileType<ElectricTentacle>(), damage, knockback, Main.myPlayer);
                        }
                    }

                    break;

                case 2:
                    Timer++;

                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_ConjureBallLightningMega()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;
                    if (Timer == 1)
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = ConjureBallLightningDamage;
                            int knockback = 1;
                            Vector2 firePos = lightningSpawnPos;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, Vector2.Zero,
                                ModContent.ProjectileType<MegaConjureBallLightning>(), damage, knockback, Main.myPlayer,
                                ai2: NPC.whoAmI);
                        }
                    }

                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    Vector2 offset = new Vector2(0, -252);
                    Vector2 targetPos = Target.Center + offset;
                    Vector2 v = targetPos - NPC.Center;
                    Vector2 tv = v * 0.07f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);

  
                    if (Timer % 12 == 0)
                    {
                        SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                        soundStyle.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                    }

                    if (Timer > 300)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        Timer = 0;
                        AttackCounter++;
       
                    }

                    break;

                case 2:
                    Timer++;

                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }
  
        private void AI_ElectricField()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;

                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    Vector2 offset = new Vector2(0, -252);
                    Vector2 targetPos = Target.Center + offset;
                    Vector2 v = targetPos - NPC.Center;
                    Vector2 tv = v * 0.07f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);
                    LightningBallTimer += 1 / 30f;
                    if (Timer % 12 == 0)
                    {
                        SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                        soundStyle.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                    }

                    if (Timer % 60 == 0)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = ElectricFieldDamage;
                            int knockback = 1;
          
         
                            Vector2 fireVelocity = (Target.Center - lightningSpawnPos).SafeNormalize(Vector2.Zero);
                            fireVelocity = fireVelocity.RotatedByRandom(MathHelper.PiOver4);
                            fireVelocity *= Main.rand.NextFloat(15, 18);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPos, fireVelocity,
                                ModContent.ProjectileType<ElectricNode>(), damage, knockback, Main.myPlayer);
                        }
                    }

                    if(Timer >= 300)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 2:
                    Timer++;

                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_Singularity()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;

                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    Vector2 offset = new Vector2(0, -252);
                    Vector2 targetPos = Target.Center + offset;
                    Vector2 v = targetPos - NPC.Center;
                    Vector2 tv = v * 0.07f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);
                    LightningBallTimer += 1 / 30f;
                    if (Timer % 12 == 0)
                    {
                        SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                        soundStyle.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                    }

                    if (Timer > 120)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        Timer = 0;
                        AttackCounter++;
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = SingularityDamage;
                            int knockback = 1;
                            Vector2 firePos = lightningSpawnPos;
                            Vector2 fireVelocity = Vector2.UnitX * 7;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, fireVelocity,
                                ModContent.ProjectileType<ElectricSingularity>(), damage, knockback, Main.myPlayer);
                        }
                    }

                    break;

                case 2:
                    Timer++;

                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_Thunderslap()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;

      
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    if(Timer < 230)
                    {
                        Vector2 offset = new Vector2(0, -252);
                        Vector2 targetPos = Target.Center + offset;
                        Vector2 v = targetPos - NPC.Center;
                        Vector2 tv = v * 0.25f;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);
                    }
      
                    if (Timer % 12 == 0)
                    {
                        SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                        soundStyle.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                    }

                    if(Timer % 60 == 0 && Timer < 240)
                    {
                        int damage = ThunderslapDamage;
                        int knockback = 1;
                        Vector2 firePos = Target.Center - new Vector2(0, 512);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, Vector2.UnitY,
                            ModContent.ProjectileType<ThunderSlapWarn>(), damage, knockback, Main.myPlayer);
                    }

                    if(Timer >= 230)
                    {
                        NPC.velocity.Y -= 0.5f;
                        ArmSegment.Fast = true;
                        ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                        FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    }
                    else if(Timer >= 210)
                    {
                        FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    }
                    else
                    {
                        ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                        FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    }

                    if (Timer > 240)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        Timer = 0;
                        AttackCounter++;
     
                        if (StellaMultiplayer.IsHost)
                        {
                            int damage = ThunderslapDamage;
                            int knockback = 1;
                            Vector2 firePos = Target.Center - new Vector2(0, 512);

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, Vector2.UnitY,
                                ModContent.ProjectileType<ThunderSlap>(), damage, knockback, Main.myPlayer);
                        }
                    }

                    break;

                case 2:
                    Timer++;
                    if(Timer == 1)
                    {
                        NPC.velocity.Y -= 15;
                    }
                    NPC.velocity *= 0.9f;
                    ArmSegment.Fast = false;
                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_JackFire()
        {
            Vector2 lightningSpawnPos = NPC.Center;
            lightningSpawnPos.Y -= 48;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 0.1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);

                        TargetMovePos = Target.Center - new Vector2(0, 128);
                    }

                    //Slow down movement and summon ball lightnings
                    //I think two?
                    //Raise arms and prepare
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 256;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

                    FaceSegment.Glow = true;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 dustVelocity = (lightningSpawnPos - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                        dustVelocity *= 4;
                        float progress = Timer / 80f;

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                        d.noGravity = true;
                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 1:
                    Timer++;

                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    Vector2 offset = new Vector2(0, -252);
                    Vector2 targetPos = Target.Center + offset;
                    Vector2 v = targetPos - NPC.Center;
                    Vector2 tv = v * 0.07f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);
                    LightningBallTimer += 1 / 12f;
                    if (Timer % 12 == 0)
                    {
                        SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
                        soundStyle.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                    }

                    float p = (Timer / 120f);
                    Vector2 pos = NPC.Center + (p * MathHelper.TwoPi).ToRotationVector2() * 80;
                    Vector2 pos2 = NPC.Center + (p * MathHelper.TwoPi + MathHelper.Pi).ToRotationVector2() * 80;
                    Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: 1f);
                    Dust.NewDustPerfect(pos2, DustID.Torch, Vector2.Zero, Scale: 1f);

                    if (Timer % 12 == 0)
                    {
                        FaceSegment.BlackTimer = 1f;
                        LightningBallTimer = 0;
                        Vector2 spawnPoint = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                        Vector2 startVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                        int projType = ModContent.ProjectileType<ElectricFire>();
                        int damage = JackFireDamage;
                        int knockback = 1;
                        if (StellaMultiplayer.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, startVelocity, projType, damage, knockback, Main.myPlayer);
                        }
                    }

                    if(Timer >= 240)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 2:
                    Timer++;

                    FaceSegment.Glow = false;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
            }
        }

        private void AI_Death()
        {

        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GothiviasSeal>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DaedusBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.DaedusBossRel>()));


            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            int numResults = 5;


            notExpertRule.OnSuccess(ItemDropRule.AlwaysAtleastOneSuccess(
                ItemDropRule.Common(ModContent.ItemType<BearBroochA>(), chanceDenominator: numResults),
                ItemDropRule.Common(ModContent.ItemType<VixedBroochA>(), chanceDenominator: numResults),
                ItemDropRule.Common(ModContent.ItemType<HeatGlider>(), chanceDenominator: numResults),
                 ItemDropRule.Common(ModContent.ItemType<DaedCard>(), chanceDenominator: numResults)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Plate>(), minimumDropped: 200, maximumDropped: 1300));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 4, maximumDropped: 55));
            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
        }


        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDaedusBoss, -1);
        }
    }
}
