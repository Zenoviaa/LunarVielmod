using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted
{
    public class BaseDaedusSegment
    {
        public BaseDaedusSegment(NPC npc)
        {
            NPC = npc;
        }

        public float frameCounter;
        public int frame;
        public Color outlineColor;
        public NPC NPC { get; init; }
        public string BaseTexturePath => GetType().DirectoryHere() + "/";
        public virtual void AI() { }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { }
    }

    public class DaedusTopSegment : BaseDaedusSegment
    {
        public DaedusTopSegment(NPC npc) : base(npc)
        {

        }

        public Rectangle AnimationFrame { get; set; }
        public override void AI()
        {
            base.AI();
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
        }
        public void Outline(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusTop").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 60);
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos.Y -= 0;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;

            Vector2 drawOrigin = animationFrame.Size() / 2;
            spriteBatch.Draw(texture, drawPos + left, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusTop").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 60);
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = animationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, animationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    public class DaedusFaceSegment : BaseDaedusSegment
    {
        public enum AnimationState
        {
            Laughing,
            Smile,
            Scared
        }
        public DaedusFaceSegment(NPC npc) : base(npc)
        {
            Animation = AnimationState.Smile;

        }
        public AnimationState Animation { get; set; }
        public bool Glow { get; set; }
        public float GlowTimer { get; set; }

        public float BlackTimer { get; set; }
        public override void AI()
        {
            base.AI();
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
        public void Outline(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusFace").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 6);
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos.Y -= 0;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;

            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            Vector2 drawOrigin = animationFrame.Size() / 2;
            spriteBatch.Draw(texture, drawPos + left, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusFace").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 6);
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = animationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, animationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);

            //Ok so we need some glowing huhh
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float f = 0; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f, speed: 3);
                Vector2 glowDrawPos = drawPos + offset;
                spriteBatch.Draw(texture, glowDrawPos, animationFrame, drawColor * GlowTimer, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(texture, drawPos, animationFrame, Color.Black * BlackTimer, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);

        }
    }

    public class DaedusArmSegment : BaseDaedusSegment
    {
        private AnimationState _animationState;
        public enum AnimationState
        {
            Raise,
            Hold_Up,
            Lower,
            Hold_Down
        }


        public DaedusArmSegment(NPC npc) : base(npc)
        {
            Animation = AnimationState.Hold_Down;
        }

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

        }
        public void Outline(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusArms").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 17);
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos.Y -= 0;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;

            Vector2 drawOrigin = animationFrame.Size() / 2;
            spriteBatch.Draw(texture, drawPos + left, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusArms").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 17);
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = animationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, animationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    public class DaedusBackSegment : BaseDaedusSegment
    {
        public DaedusBackSegment(NPC npc) : base(npc)
        {

        }

        public override void AI()
        {
            base.AI();
            frameCounter += 0.5f;
            if (frameCounter >= 1f)
            {
                frame++;
                if (frame >= 60)
                {
                    frame = 0;
                }
            }
        }

        private void DrawOutline(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusBack").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 60);
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos.Y -= 0;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;

            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            Vector2 drawOrigin = animationFrame.Size() / 2;
            spriteBatch.Restart(effect: whiteShader.Effect);

            spriteBatch.Draw(texture, drawPos + left, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);

            spriteBatch.RestartDefaults();
        }
        public void Outline(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusBack").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 60);
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos.Y -= 0;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;

            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            Vector2 drawOrigin = animationFrame.Size() / 2;
            spriteBatch.Draw(texture, drawPos + left, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, animationFrame, outlineColor, NPC.rotation, drawOrigin, NPC.scale * 2, spriteEffects, 0);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawOutline(spriteBatch, screenPos, drawColor);
            Texture2D texture = ModContent.Request<Texture2D>(BaseTexturePath + "DaedusBack").Value;
            Rectangle animationFrame = texture.GetFrame(frame, totalFrameCount: 60);
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = animationFrame.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, animationFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        }
    }

    public class DaedusRobeSegment : BaseDaedusSegment
    {
        public DaedusRobeSegment(NPC npc) : base(npc)
        {

        }

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

    public class DaedusTheDevoted : ScarletBoss
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
            Tired
        }

        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackCounter => ref NPC.ai[2];
        private ref float AttackCycle => ref NPC.ai[3];

        private PatternManager<AIState> _patternManager;
        private float _attackNum;
        private float _hitDirection;
        private float _deathRotation;
        private bool _showNamePlate;
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
        private DaedusTopSegment _topSegment;
        private DaedusFaceSegment _faceSegment;
        private DaedusBackSegment _backSegment;
        private DaedusArmSegment _armSegment;
        private DaedusRobeSegment _robeSegment;
        public DaedusTopSegment TopSegment
        {
            get
            {
                _topSegment ??= new DaedusTopSegment(NPC);
                return _topSegment;
            }
        }

        public DaedusFaceSegment FaceSegment
        {
            get
            {
                _faceSegment ??= new DaedusFaceSegment(NPC);
                return _faceSegment;
            }
        }

        public DaedusBackSegment BackSegment
        {
            get
            {
                _backSegment ??= new DaedusBackSegment(NPC);
                return _backSegment;
            }
        }

        public DaedusArmSegment ArmSegment
        {
            get
            {
                _armSegment ??= new DaedusArmSegment(NPC);
                return _armSegment;
            }
        }

        public DaedusRobeSegment RobeSegment
        {
            get
            {
                _robeSegment ??= new DaedusRobeSegment(NPC);
                return _robeSegment;
            }
        }
        public CoreLightning Lightning { get; set; } = new CoreLightning();
        public CoreLightning BlackLightning { get; set; } = new CoreLightning();
        public float LightningBallTimer { get; set; }

        public Vector2 ArenaCenter { get; set; }

        public bool Enraged
        {
            get
            {
                //Don't go outside the arena smh
                float distanceToTarget = Vector2.Distance(ArenaCenter, Target.Center);
                return distanceToTarget > 1440;
            }
        }

        //Damage Numbers
        private int LightningStrikeDamage => 21;
        private int MiniLightningBallDamage => 21;
        private int ConjureBallLightningDamage => 42;
        private int ElectricFieldDamage => 16;
        private int ThunderslapDamage => 20;
        private int JackFireDamage => 12;

        private Color _outlineColor;
        public Color TargetOutlineColor;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(TeleportTarget);
            writer.WriteVector2(BigTeleportTarget);
            writer.Write(Phase2Transition);
            writer.WriteVector2(ArenaCenter);
            writer.Write(_attackNum);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TeleportTarget = reader.ReadVector2();
            BigTeleportTarget = reader.ReadVector2();
            Phase2Transition = reader.ReadBoolean();
            ArenaCenter = reader.ReadVector2();
            _attackNum = reader.ReadInt32();
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            Main.npcFrameCount[NPC.type] = 46;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 14;
            NPC.defense = 2;
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
            NPC.aiStyle = 0;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Daedus");
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (ArenaCenter == default)
            {
                ArenaCenter = NPC.Center;
            }
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);


            //Teleport Go!!!
            if (TeleportTarget != Vector2.Zero)
            {
                NPC.Center = TeleportTarget;
                for (int i = 0; i < 24; i++)
                {
                    float progress = i / 24f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 velocity = rot.ToRotationVector2() * 16;
                    Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, Velocity: velocity);
                }


                SoundStyle teleportStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
                teleportStyle.PitchVariance = 0.05f;
                teleportStyle.Pitch = TeleportCount * 0.05f;
                SoundEngine.PlaySound(teleportStyle, NPC.position);

                TeleportTarget = Vector2.Zero;
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
                    float progress = i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + Timer * 0.05f;
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(0, 32, speed: 3);
                    _lightningZaps[i] = NPC.Center + offset + new Vector2(0, -64);
                }

                float flapSpeed = 1.25f;
                for (int i = 0; i < _blackLightningZaps.Length; i++)
                {
                    float progress = i / (float)_lightningZaps.Length;
                    float offset = progress * MathHelper.ToRadians(120);
                    Vector2 rotatedOffset = Vector2.UnitY.RotatedBy(offset + VectorHelper.Osc(0.9f, 1.0f, 9)).RotatedByRandom(MathHelper.PiOver4 / 24f);
                    rotatedOffset = rotatedOffset.RotatedBy(MathF.Sin(Main.GlobalTimeWrappedHourly * 5f * flapSpeed) * 0.5f - 0.25f);
                    rotatedOffset = rotatedOffset.RotatedBy(-NPC.rotation);
                    rotatedOffset.X *= VectorHelper.Osc(0.2f, 1f, speed: 5 * flapSpeed);
                    Vector2 rotatedVector = rotatedOffset * 128 * VectorHelper.Osc(0.9f, 1f, 9);

                    if (i % 8 == 0)
                    {
                        _blackLightningZaps[i] = NPC.Center + rotatedVector * 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly * 4) * Phase2WingsProgress;

                        rotatedOffset.X = -rotatedOffset.X;
                        Vector2 invert = rotatedOffset * 128 * VectorHelper.Osc(0.9f, 1f, 9);
                        _blackLightningZaps2[i] = NPC.Center + invert * 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly * 4) * -1f * Phase2WingsProgress;
                    }
                    else
                    {
                        _blackLightningZaps[i] = NPC.Center + rotatedVector * -1f * Phase2WingsProgress;

                        rotatedOffset.X = -rotatedOffset.X;
                        Vector2 invert = rotatedOffset * 128 * VectorHelper.Osc(0.9f, 1f, 9);
                        _blackLightningZaps2[i] = NPC.Center + invert * -1f * Phase2WingsProgress;
                    }
                }
                Lightning.RandomPositions(_lightningZaps);
                BlackLightning.RandomPositions(_blackLightningZaps);
            }

            //Animations
            TopSegment.AI();
            FaceSegment.AI();
            BackSegment.AI();
            ArmSegment.AI();
            RobeSegment.AI();

            if (State == AIState.Death)
            {
                Phase2WingsProgress = MathHelper.Lerp(Phase2WingsProgress, 0f, 0.01f);
            }
            else if (InPhase2 && Phase2Transition)
            {
                Phase2WingsProgress = MathHelper.Lerp(Phase2WingsProgress, 1f, 0.1f);
            }

            //Retarget if can't attack current bro
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }

            switch (State)
            {
                case AIState.Tired:
                    AI_Tired();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    if (!_showNamePlate)
                    {
                        ShowNamePlate();
                        _showNamePlate = true;
                    }
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
            NPC.rotation = lerpedRotation + _deathRotation;
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

            //Draw Outlines
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
            spriteBatch.Restart(effect: whiteShader.Effect);
            BackSegment.outlineColor = _outlineColor;
            BackSegment.Outline(spriteBatch, screenPos, drawColor);

            ArmSegment.outlineColor = _outlineColor;
            ArmSegment.Outline(spriteBatch, screenPos, drawColor);

            TopSegment.outlineColor = _outlineColor;
            TopSegment.Outline(spriteBatch, screenPos, drawColor);

            // RobeSegment.outlineColor = _outlineColor;
            // RobeSegment.Outline(spriteBatch, screenPos, drawColor);

            FaceSegment.outlineColor = _outlineColor;
            FaceSegment.Outline(spriteBatch, screenPos, drawColor);
            spriteBatch.RestartDefaults();

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
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                AttackCounter = 0;
                NPC.netUpdate = true;
            }
        }

        private void AI_Tired()
        {
            Timer++;
         
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
              
                if (!NPC.HasValidTarget)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -8), 0.025f);
                    NPC.EncourageDespawn(60);
                    return;
                }
              
            }

            ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
            FaceSegment.Animation = DaedusFaceSegment.AnimationState.Scared;
            TargetOutlineColor = Color.Transparent;

            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            if (distanceToTarget > 128)
            {
                Vector2 offset = new Vector2(0, -64);
                Vector2 targetPos = Target.Center + offset;
                Vector2 velocityToTarget = targetPos - NPC.Center;
                Vector2 targetVelocity = velocityToTarget * 0.01f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
            }
            else
            {
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer) * 0.02f, 0.1f);
            }

            if (Timer >= 320)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void SetupPatternManager()
        {
            if (_patternManager == null)
            {
                if (InPhase2)
                {
                    _patternManager = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.Lightning_Strike, 1.0f),
                        new Tuple<AIState, float>(AIState.Electric_Field, 1.0f),
                        new Tuple<AIState, float>(AIState.Conjure_Ball_Lightning, 1.0f),
                        new Tuple<AIState, float>(AIState.Electric_Tentacle, 1.0f),
                        new Tuple<AIState, float>(AIState.Ground_Explosion, 1.0f),
                        new Tuple<AIState, float>(AIState.Jack_Fire, 1.0f),
                        new Tuple<AIState, float>(AIState.Singularity, 1.0f),
                        new Tuple<AIState, float>(AIState.Conjure_Ball_Lightning_Mega, 0.2f),
                        new Tuple<AIState, float>(AIState.Thunderslap, 0.2f));
                }
                else
                {
                    _patternManager = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.Lightning_Strike, 1.0f),
                        new Tuple<AIState, float>(AIState.Conjure_Ball_Lightning, 1.0f),
                        new Tuple<AIState, float>(AIState.Electric_Tentacle, 1.0f),
                        new Tuple<AIState, float>(AIState.Ground_Explosion, 1.0f),
                        new Tuple<AIState, float>(AIState.Jack_Fire, 1.0f),
                        new Tuple<AIState, float>(AIState.Singularity, 1.0f),
                        new Tuple<AIState, float>(AIState.Conjure_Ball_Lightning_Mega, 0.2f));
                }
            }



        }
        private void AI_Idle()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }
          
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -8), 0.025f);
                    NPC.EncourageDespawn(60);
                    return;
                }
            }

            ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
            FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;

            TargetOutlineColor = Color.Transparent;

            Vector2 offset = new Vector2(0, -128);
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
            float timeToWait = 100;

            //FAST
            if (InPhase2)
                timeToWait = 30;
            if (Enraged)
                timeToWait = 0;

            if (Timer >= timeToWait)
            {
                //How we choosing attack uhh, oh i know
                if (MultiplayerHelper.IsHost)
                {
                    if (_attackNum >= 3)
                    {
                        _attackNum = 0;
                        SwitchState(AIState.Tired);
                        return;
                    }

                    SetupPatternManager();
                    AIState nextAttack = _patternManager.NextPattern();
                    if (!Phase2Transition && InPhase2)
                    {
                        nextAttack = AIState.Phase_2_Transition;
                    }

                    _attackNum++;
                    SwitchState(nextAttack);


                }
            }
        }

        private void AI_Phase2Transition()
        {
            _patternManager = null;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        NPC.velocity.Y -= 15;
                    }

                    NPC.velocity.X *= 0.98f;
                    if (NPC.velocity.Y < 12)
                    {
                        NPC.velocity.Y += 0.33f;
                    }


                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Scared;
                    if (Timer >= 180)
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
                    if (Timer % divisor == 0)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);
                        if (MultiplayerHelper.IsHost)
                        {
                            float range = MathHelper.Lerp(1024, 64, Timer / 300f);
                            TeleportTarget = Target.Center + Main.rand.NextVector2CircularEdge(range, range);
                            NPC.velocity = Main.rand.NextVector2Circular(8, 8);
                            NPC.netUpdate = true;
                        }
                    }
                    if (Timer >= 300f)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;
                case 2:
                    Timer++;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    if (Timer == 30)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            BigTeleportTarget = Target.Center + new Vector2(0, -256);
                            NPC.velocity = Vector2.Zero;
                            NPC.netUpdate = true;
                        }
                    }

                    if (Timer >= 90)
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
                        if (MultiplayerHelper.IsHost)
                        {
                            int damage = 21;
                            int knockback = 1;
                            Vector2 startPos = Target.Center;
                            startPos.Y -= 128;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), startPos, Vector2.Zero,
                                ModContent.ProjectileType<RadiantBall>(), damage, knockback, Main.myPlayer, ai2: NPC.whoAmI);
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

                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(),
                                Target.Center - new Vector2(0, 128), Vector2.UnitY * 64,
                                ModContent.ProjectileType<LightningStrikeWarn>(), 0, 0, Main.myPlayer, ai1: Target.whoAmI);
                        }
                        TargetMovePos = Target.Center - new Vector2(0, 512);
                    }
                    TargetOutlineColor = Color.Yellow;
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 128;
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
                    TargetOutlineColor = Color.Red;
                    NPC.velocity *= 0.96f;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;

                    if (Timer > 20 && Timer % 30 == 0)
                    {
                        FaceSegment.BlackTimer = 1f;
                        if (MultiplayerHelper.IsHost)
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
                    TargetOutlineColor = Color.Transparent;
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
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 128;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
                    TargetOutlineColor = Color.Yellow;
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
                    TargetOutlineColor = Color.Red;
                    LightningBallTimer += 1 / 10f;
                    if (Timer < 80)
                    {
                        if (Timer % 10 == 0)
                        {
                            FXUtil.GlowCircleBoom(lightningSpawnPos, Color.Yellow, Color.Orange, Color.Blue);
                        }
                    }
                    if (Timer > 80 && Timer % 30 == 0)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        if (MultiplayerHelper.IsHost)
                        {
                            int damage = MiniLightningBallDamage;
                            int knockback = 1;
                            Vector2 firePos = lightningSpawnPos;
                            Vector2 fireVelocity = (Target.Center - firePos).SafeNormalize(Vector2.Zero);
                            fireVelocity *= 2;
                            float charge = Timer / 90f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, fireVelocity,
                                ModContent.ProjectileType<ConjureBallLightning>(), damage, knockback, Main.myPlayer,
                                ai1: charge);
                        }
                    }

                    if (Timer >= 150)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 2:
                    Timer++;
                    TargetOutlineColor = Color.Transparent;
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
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 128;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
                    TargetOutlineColor = Color.Yellow;
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
                    TargetOutlineColor = Color.Red;
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
                        if (MultiplayerHelper.IsHost)
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
                    TargetOutlineColor = Color.Transparent;
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
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 128;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
                    TargetOutlineColor = Color.Yellow;
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
                        if (MultiplayerHelper.IsHost)
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
                    TargetOutlineColor = Color.Red;

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
                    TargetOutlineColor = Color.Transparent;
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
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 128;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
                    TargetOutlineColor = Color.Yellow;
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
                    TargetOutlineColor = Color.Red;
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
                        if (MultiplayerHelper.IsHost)
                        {
                            int damage = ElectricFieldDamage;
                            int knockback = 1;


                            float rot = Main.rand.NextFloat(0f, 3.14f);
                            Vector2 fireVelocity = rot.ToRotationVector2() * Main.rand.NextFloat(10, 18);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), lightningSpawnPos, fireVelocity,
                                ModContent.ProjectileType<ElectricNode>(), damage, knockback, Main.myPlayer);
                        }
                    }

                    if (Timer >= 300)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 2:
                    Timer++;
                    TargetOutlineColor = Color.Transparent;
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
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 128;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
                    TargetOutlineColor = Color.Yellow;
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
                    TargetOutlineColor = Color.Red;
                    if (Timer > 120)
                    {
                        LightningBallTimer = 0;
                        FaceSegment.BlackTimer = 1f;
                        Timer = 0;
                        AttackCounter++;
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 firePos = lightningSpawnPos;
                            NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)firePos.X, (int)firePos.Y,
                                ModContent.NPCType<ElectricSingularity>());
                        }
                    }

                    break;

                case 2:
                    Timer++;
                    TargetOutlineColor = Color.Transparent;
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
                    Vector2 movePos = TargetMovePos + Vector2.UnitY.RotatedBy(0.025f * Timer * (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero).X) * 128;
                    Vector2 velocityToTarget = movePos - NPC.Center;
                    Vector2 targetVelocity = velocityToTarget * 0.03f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
                    TargetOutlineColor = Color.Yellow;
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
                    TargetOutlineColor = Color.Red;

                    if (Timer % 4 == 0)
                    {
                        Vector2 dustSpawnPoint = lightningSpawnPos;
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(4, 4);

                        Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GemTopaz, Velocity: dustVelocity, Scale: 0.5f);
                        d.noGravity = true;
                    }

                    if (Timer < 230)
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

                    if (Timer % 60 == 0 && Timer < 240)
                    {
                        int damage = ThunderslapDamage;
                        int knockback = 1;
                        Vector2 firePos = Target.Center - new Vector2(0, 512);

                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Ticking");
                        laughSound.PitchVariance = 0.03f;
                        laughSound.Pitch = Timer / 180f;
                        SoundEngine.PlaySound(laughSound, NPC.position);


                        Projectile.NewProjectile(NPC.GetSource_FromThis(), firePos, Vector2.UnitY,
                            ModContent.ProjectileType<ThunderSlapWarn>(), damage, knockback, Main.myPlayer);
                    }

                    if (Timer >= 230)
                    {
                        NPC.velocity.Y -= 0.5f;
                        ArmSegment.Fast = true;
                        ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                        FaceSegment.Animation = DaedusFaceSegment.AnimationState.Laughing;
                    }
                    else if (Timer >= 210)
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

                        if (MultiplayerHelper.IsHost)
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
                    if (Timer == 1)
                    {
                        NPC.velocity.Y -= 15;
                    }
                    TargetOutlineColor = Color.Transparent;
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
                    TargetOutlineColor = Color.Yellow;
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
                    TargetOutlineColor = Color.Red;
                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Raise;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer % 128 == 0)
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

                    float p = Timer / 120f;
                    Vector2 pos = NPC.Center + (p * MathHelper.TwoPi).ToRotationVector2() * 80;
                    Vector2 pos2 = NPC.Center + (p * MathHelper.TwoPi + MathHelper.Pi).ToRotationVector2() * 80;
                    Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: 1f);
                    Dust.NewDustPerfect(pos2, DustID.Torch, Vector2.Zero, Scale: 1f);

                    if (Timer % 42 == 0)
                    {
                        FaceSegment.BlackTimer = 1f;
                        LightningBallTimer = 0;
                        Vector2 spawnPoint = NPC.Center + new Vector2(0, -96);
                        spawnPoint.X += Main.rand.NextFloat(-300, 300);
                        Vector2 startVelocity = (Target.Center - spawnPoint).SafeNormalize(Vector2.Zero) * 10;
                        int projType = ModContent.ProjectileType<ElectricFire>();
                        int damage = JackFireDamage;
                        int knockback = 1;
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, startVelocity, projType, damage, knockback, Main.myPlayer);
                        }
                    }

                    if (Timer >= 240)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;

                case 2:
                    Timer++;
                    TargetOutlineColor = Color.Transparent;
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
        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);

            if (NPC.life <= 0 && State != AIState.Death)
            {
                _hitDirection = hit.HitDirection;
                NPC.life = 1;
                SwitchState(AIState.Death);
            }


            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
        }

        private void AI_Death()
        {
            TargetOutlineColor = Color.Transparent;
            switch (AttackCounter)
            {
                case 0:
                    Timer++;
                    if (Timer == 1)
                    {
                        NPC.velocity.X = _hitDirection * 7;
                        NPC.velocity.Y -= 15;
                    }

                    if (!NPC.collideY)
                    {
                        _deathRotation += _hitDirection * 0.0025f;
                    }

                    NPC.noGravity = false;


                    ArmSegment.Animation = DaedusArmSegment.AnimationState.Lower;
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Scared;
                    if (Timer >= 180f)
                    {
                        Timer = 0;
                        AttackCounter++;
                    }
                    break;
                case 1:
                    Timer++;
                    _deathRotation = 0;
                    NPC.noGravity = true;
                    if (Timer == 1)
                    {
                        SoundStyle laughSound = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                        laughSound.PitchVariance = 1f;
                        laughSound.Pitch = 0.75f;
                        SoundEngine.PlaySound(laughSound, NPC.position);
                        if (MultiplayerHelper.IsHost)
                        {
                            BigTeleportTarget = Target.Center + new Vector2(0, -256);
                            NPC.velocity = Vector2.Zero;
                            NPC.netUpdate = true;
                        }
                    }


                    Vector2 offset = new Vector2(0, -64f);
                    Vector2 targetPos = Target.Center + offset;
                    Vector2 v = targetPos - NPC.Center;
                    Vector2 tv = v * 0.007f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, tv, 0.2f);
                    FaceSegment.Animation = DaedusFaceSegment.AnimationState.Smile;
                    if (Timer >= 120)
                    {
                        NPC.Kill();
                    }

                    break;

            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedDaedusBoss, -1);
        }
    }
}
