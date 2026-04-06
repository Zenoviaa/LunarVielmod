using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.InverseKinematics;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Assets.AssetRegistry;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public struct PunkerPrimeDraw
    {
        public Color outlineColor;
        public Vector2 scale;
        public Vector2 shakeOffset;
        public float afterImageStrength;

        public void SetDefaults()
        {
            scale = Vector2.One;
            outlineColor = Color.Transparent;
            afterImageStrength = 0f;
        }
    }

    //On top of that we need to make the custom draw code for the armrs
    //We're just going to do this with forward kinematics since the arms don't need to be super precisely reaching for something, just coming out of the body really
    //So let's make a simple system
    public class PunkerPrimeArmPart
    {
        public PunkerPrimeArmPart(PunkerPrimeArmPart parent, Texture2D texture, float initialAngle)
        {
            this.parent = parent;
            this.texture = texture;
            this.drawOrigin = new Vector2(0f, texture.Height / 2f);
            this.angle = initialAngle;
            this.length = texture.Width;
            this.color = Color.White;
        }
        public PunkerPrimeArmPart parent;
        public Texture2D texture;
        public Vector2 drawOrigin;
        public Vector2 rootPosition;
        public Vector2 endPosition;
        public float angle;
        public float length;
        public Color color;

        public void Update()
        {
            if (parent != null)
            {
                rootPosition = parent.endPosition;
            }
            endPosition = rootPosition + angle.ToRotationVector2() * length;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawPosition = rootPosition - screenPos;
            Color finalColor = color.MultiplyRGB(drawColor);
            Vector2 drawScale = Vector2.One;
            spriteBatch.Draw(texture, drawPosition, null, finalColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }

    //ALright so now we need to think about punker prime's arms
    //The arms are able to operate individually of the boss
    //The easiest way to do this is to make separate NPCs for each of them
    //So we should probably have a base NPC for PunkerPrime's arms

    public abstract class PunkerPrimeArm : ModNPC,
        IDrawOutlines
    {
        protected float _flashAlpha;
        protected Color _outlineColor;
        protected Color TargetOutlineColor;
        protected PunkerPrimeArmPart[] _segmentsBackingField;

        protected PunkerPrimeArmPart[] Segments
        {
            get
            {
                if (_segmentsBackingField == null)
                {
                    Texture2D[] armTextures = RequestArmTextures();
                    _segmentsBackingField = new PunkerPrimeArmPart[armTextures.Length];

                    for (int a = 0; a < armTextures.Length; a++)
                    {
                        PunkerPrimeArmPart parent = a == 0 ? null : _segmentsBackingField[a - 1];
                        PunkerPrimeArmPart armPart = new PunkerPrimeArmPart(parent, armTextures[a], 0);
                        _segmentsBackingField[a] = armPart;
                    }
                }

                return _segmentsBackingField;
            }
        }

        private VerletChain _vchain;
        protected VerletChain VChain
        {
            get
            {
                if (_vchain == null)
                {
                    _vchain = new VerletChain(NPC.Center, NPC.Center + Vector2.UnitY * 360, 20);
                }
                return _vchain;
            }
        }
        public bool isAttacking;
        public float superChargeTimer;
        public float afterImageStrength;
        public Color telegraphLineColor;
        public float heldLightningScale;
        protected bool DoAttack
        {
            get => NPC.ai[0] == 1;
            set => NPC.ai[0] = value ? 1 : 0;
        }

        protected NPC Parent
        {
            get => Main.npc[(int)NPC.ai[1]];
            set => NPC.ai[1] = value.whoAmI;
        }

        protected ref float Timer => ref NPC.ai[2];
        protected Player Target => Main.player[NPC.target];
        protected Texture2D RequestSubTexture(string spriteName)
        {
            string texturePath = ModContent.GetInstance<PunkerPrime>().Texture;
            string subTexturePath = texturePath + "_" + spriteName;
            Texture2D texture = ModContent.Request<Texture2D>(subTexturePath, AssetRequestMode.ImmediateLoad).Value;
            return texture;
        }

        protected Texture2D[] RequestArmTextures()
        {
            Texture2D[] textures = new Texture2D[4];
            textures[0] = RequestSubTexture("Shoulder");
            textures[1] = RequestSubTexture("Arm");
            textures[2] = RequestSubTexture("Elbow");
            textures[3] = RequestSubTexture("ForeArm");
            return textures;
        }

        protected Vector2 GetGunHoldCenter()
        {
            return Segments[Segments.Length - 1].endPosition;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(isAttacking);
            writer.Write(superChargeTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            isAttacking = reader.ReadBoolean();
            superChargeTimer = reader.ReadSingle();
        }


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 100;
            NPC.defense = 14;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        //Sealing this just so don't accidentally override it, we don't want to remove the base functionailty
        public sealed override void AI()
        {
            base.AI();

            if (!Parent.active)
                NPC.active = false;


            float s = 4;
            Vector2 rootPosition = Parent.Center + Vector2.UnitY * 150;
            Vector2 targetPosition = rootPosition + Vector2.UnitY * 200;
            targetPosition.X += ExtraMath.Osc(-200, 200, speed: s, offset: NPC.whoAmI *4);
            targetPosition.Y += ExtraMath.Osc(-50, 0, speed: 2, offset: NPC.whoAmI*4);
            VChain.noTileCollide = true;
            VChain.points[0].pinned = true;
            VChain.points[0].position = Parent.Center;
            VChain.points[VChain.points.Length-1].pinned = true;
            VChain.points[VChain.points.Length - 1].position = GetGunHoldCenter();
            VChain.gravity = 0;
            VChain.Update();
            //Arm.IK(rootPosition, targetPosition);

            ArmAI();
            if(superChargeTimer > 0)
            {
                if(superChargeTimer % 2 == 0)
                {
                    ArmAI();
                }
         
                superChargeTimer--;
            }
            _flashAlpha *= 0.92f;
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            for (int i = 0; i < Segments.Length; i++)
            {
                PunkerPrimeArmPart segment = Segments[i];
                segment.Update();
            }
            if (isAttacking)
            {
                if (Main.rand.NextBool(16))
                {
                    Vector2 gunHoldCenter = GetGunHoldCenter();
                    Vector2 spawnPos = gunHoldCenter;
                    spawnPos += Main.rand.NextVector2Circular(8, 8);
                    var zapParticle = LegacyParticle.NewParticle<SparkParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    zapParticle.innerColor = Color.White;
                    zapParticle.outerColor = Color.Red;
                    zapParticle.fadeToColor = Color.Yellow;
                }
           

            }
            Lighting.AddLight(NPC.Center, TorchID.Red);
        }


        public virtual void ArmAI()
        {

        }
        public void SuperchargeAttack()
        {
            DoAttack = true;
            superChargeTimer = 300;
            NPC.netUpdate = true;
            SoundStyle superCharge = AssetRegistry.Sounds.SteamPunking.MechSupercharge;
            superCharge.PitchVariance = 0.3f;
            SoundEngine.PlaySound(superCharge, NPC.position);
        }


        public void Attack()
        {
            DoAttack = true;
            NPC.netUpdate = true;
        }

        protected void SetRootToParentCenter()
        {
 

            Segments[0].rootPosition = Parent.Bottom;
        }
        protected void AimGunTowardTarget()
        {
            Vector2 holdCenter = GetGunHoldCenter();
            Vector2 targetVelocity = (holdCenter - NPC.Center);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, EasingFunction.InOutSine(Timer / 60f));

            float targetAngle = Segments[Segments.Length - 1].angle;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);
        }

        protected void CreateMuzzleFlash()
        {
            _flashAlpha = 1f;
            var bigPart = FXUtil.GlowCircleBoom(GetGunHoldCenter(), Color.White, Color.Red, Color.DarkRed);
            var littlePart = FXUtil.GlowCircleBoom(GetGunHoldCenter(), Color.White, Color.Red, Color.DarkRed);
            littlePart.Scale *= 0.6f;

            float numParticles = 4;
            for(float n = 0; n < numParticles; n++)
            {
                Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 5f;
                fireVelocity *= Main.rand.NextFloat(0.5f, 1f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), fireVelocity, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }

        private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(bloomLineTexture.Width / 2f, 0f);
            Vector2 drawCenter = NPC.Center - screenPos;
            Vector2 scale = Vector2.One;
            scale.X = 0.35f;
            scale.Y = 2;

            Color color = telegraphLineColor;
            color.A = 0;
            color *= 0.35f;
            float rotation = NPC.rotation - MathHelper.ToRadians(90);
            spriteBatch.Draw(bloomLineTexture, drawCenter, null, color, rotation, drawOrigin, scale, SpriteEffects.None, 0);
        }

        private void DrawTentacleArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture2 = RequestSubTexture("ArmSmallGlow");
            var texture = RequestSubTexture("ArmSmall");
            for (int i = 0; i < VChain.points.Length - 1; i++)
            {

                var point = VChain.points[i];
                Vector2 drawPosition = point.position - Main.screenPosition;
                Vector2 drawOrigin = new Vector2(0f, texture.Height / 2f);
                Vector2 drawScale = Vector2.One;
                drawScale.Y *= 0.2f;
                drawScale.X *= 0.45f;
                var nextPoint = VChain.points[i + 1];
                float angle = (nextPoint.position - point.position).ToRotation();
                spriteBatch.Draw(texture, drawPosition, null, drawColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
                if (isAttacking)
                {
                    Color glowColor = Color.Yellow;
                    glowColor *= ExtraMath.Osc(0f, 0.5f, speed: 8, offset: i * 4);
                    glowColor.A = 0;
                    spriteBatch.Draw(texture2, drawPosition, null, glowColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }
        }

        public void DrawPowerCord(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawTentacleArm(spriteBatch, screenPos, drawColor);
        }

        public void DrawArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < Segments.Length; i++)
            {

                PunkerPrimeArmPart segment = Segments[i];
                segment.Draw(spriteBatch, screenPos, drawColor);
            }
        }

        private void DrawGunAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawScale = Vector2.One;
            float length = NPCID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < length; i++)
            {
                float f = i;
                float completionRatio = f / length;
                Vector2 oldPosition = NPC.oldPos[i];
                Vector2 oldCenter = oldPosition + NPC.Size / 2f - screenPos;
                Color color = Color.Red;
                color *= 0.1f;
                color *= afterImageStrength;
                color *= MathHelper.SmoothStep(1f, 0f, completionRatio);
                spriteBatch.Draw(texture, oldCenter, frame, color, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Gray, EasingFunction.QuadraticBump(completionRatio)) * heldLightningScale * 0.35f;
        }

        private float WidthFunction(float completionRatio)
        {
            return 8 * heldLightningScale;
        }
        private void DrawHeldLightning(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (heldLightningScale <= 0.02f)
                return;

            List<Vector2> conjureLightningPositions = new List<Vector2>();
            float numPoints = 32;
            for(float n = 0; n < numPoints; n++)
            {
                float completionRatio = n / numPoints;
                Vector2 position = Vector2.Lerp(GetGunHoldCenter(), NPC.Center, completionRatio);
                conjureLightningPositions.Add(position);
            }

            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.LightningTrail2;
            shader.PrimaryTexture2 = TrailRegistry.LightningTrail;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.Red;
            shader.Distortion = 0.2f;
            shader.Time = Main.GlobalTimeWrappedHourly * 16;
            TrailDrawer.Draw(spriteBatch, conjureLightningPositions.ToArray(), ColorFunction, WidthFunction, shader);

           
        }
        public void DrawGun(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = NPC.Center - screenPos;


            Color baseColor = isAttacking ? Color.White : Color.Lerp(Color.White, Color.Black, 0.8f);
            Color finalColor = baseColor.MultiplyRGB(drawColor);
            Vector2 drawScale = Vector2.One;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, NPC.frame, finalColor, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            Color glowColor = Color.Red;
            glowColor.A = 0;
            glowColor *= _flashAlpha;
            spriteBatch.Draw(texture, drawPosition, NPC.frame, glowColor, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        public void DrawGunEffects(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawHeldLightning(spriteBatch, screenPos, drawColor);
            DrawTelegraphLine(spriteBatch, screenPos, drawColor);
            DrawGunAfterImage(spriteBatch, screenPos, drawColor);
        }

        public void DrawGunArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (superChargeTimer > 0f)
            {
                DrawSuperchargedArm(spriteBatch, screenPos, drawColor);
            }
            else
            {
                DrawArm(spriteBatch, screenPos, drawColor);
            }
        }

        //Drawing is handled by parent npc for proper layering
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        private void DrawSuperchargedArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                PunkerPrimeArmPart segment = Segments[i];
                Color finalColor = Color.Red;
                finalColor = Color.Lerp(finalColor, drawColor, ExtraMath.Osc(0f, 1f, speed: 32f));
                segment.Draw(spriteBatch, screenPos, finalColor);
            }
        }
        public void DrawGlowBall(SpriteBatch spriteBatch, Vector2 screen, Color drawColor)
        {
            if (heldLightningScale <= 0.02f)
                return;
            Texture2D glowballTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_56").Value;
            Vector2 drawCenter = GetGunHoldCenter() - Main.screenPosition;
            Vector2 drawOrigin = glowballTexture.Size() / 2f;
            Color glowColor = Color.Lerp(Color.Red, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 16));
            glowColor.A = 0;
            glowColor *= ExtraMath.Osc(0.5f, 1f, speed: 64);
            spriteBatch.Draw(glowballTexture, drawCenter, null, glowColor, 0, drawOrigin, heldLightningScale * 0.35f * ExtraMath.Osc(0.9f, 1f, speed: 64), SpriteEffects.None, 0);
        }


        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (_outlineColor == Color.Transparent)
                return;

            float outlineOffset = 2;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            DrawArm(spriteBatch, screenPos + h, _outlineColor);
            DrawArm(spriteBatch, screenPos - h, _outlineColor);
            DrawArm(spriteBatch, screenPos + v, _outlineColor);
            DrawArm(spriteBatch, screenPos - v, _outlineColor);

            DrawGun(spriteBatch, screenPos + h, _outlineColor);
            DrawGun(spriteBatch, screenPos - h, _outlineColor);
            DrawGun(spriteBatch, screenPos + v, _outlineColor);
            DrawGun(spriteBatch, screenPos - v, _outlineColor);
        }
    }

    public class Metronome
    {
        public Metronome(float bpm)
        {
            this.bpm = bpm;
        }
        public float bpm;
        public bool beatHit;
        public float beatCounter;
        public float localBeatCounter;
        public float beatTimer;

        public void Update()
        {
            float beatsPerTick = 150f / 60f / 60f;
            beatTimer += beatsPerTick;

            beatHit = false;
            while (beatTimer >= 1f)
            {
                beatTimer -= 1f;
                beatCounter++;
                localBeatCounter++;
                beatHit = true;
            }
        }
    }
    public class Boombox : ModNPC
    {
        private enum AIState
        {
            IdleFollow,
            Warn
        }
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private Metronome _metronome;
        private Metronome Metronome
        {
            get
            {
                _metronome ??= new Metronome(150);
                return _metronome;
            }
        }
        private NPC Parent => Main.npc[(int)NPC.ai[2]];
        private bool ShouldWarn => NPC.ai[3] == 1;
        private float _upDown;
        private Vector2 _upDownOffset;
        private Vector2 _bounceOffset;
        private float _rotOffset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 100;
            NPC.defense = 14;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
        }

        public override void AI()
        {
            base.AI();
            if (!Parent.active)
            {
                NPC.active = false;
            }

            Metronome.Update();
            if (_upDown == 0)
                _upDown = 1;
            if (Metronome.beatHit)
            {
                _upDown *= -1;
            }
            _rotOffset = MathHelper.Lerp(_rotOffset, 0.5f * _upDown, 0.2f);
            _upDownOffset = Vector2.Lerp(_upDownOffset, Vector2.UnitY * _upDown * 8, 0.2f);
            switch (State)
            {
                case AIState.IdleFollow:
                    AI_IdleFollow();
                    break;
                case AIState.Warn:
                    AI_Warn();
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
        private void AI_IdleFollow()
        {
            Timer++;
            Chase();
            if (ShouldWarn)
            {
                SwitchState(AIState.Warn);
            }
        }

        private void Chase()
        {
            //Crazy movement code
            Vector2 targetPosition = Parent.Center;
            targetPosition.Y -= 8;
            Vector2 velocityToPlayer = (targetPosition - NPC.Center);
            velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
            float dist = Vector2.Distance(NPC.Center, targetPosition);
            if (dist <= 0)
                dist = 1;

            float interp = dist / 384;
            interp = EasingFunction.InOutSine(interp);
            float speed = MathHelper.Lerp(6, 20, interp);

            float xDist = MathF.Abs(targetPosition.X - NPC.Center.X);
            if (xDist < 256)
                velocityToPlayer.Y -= 0.5f;

            if (dist < speed)
                speed = dist;
            velocityToPlayer *= speed;
            velocityToPlayer *= ExtraMath.Osc(0.5f, 1f, speed: 2);
            velocityToPlayer.Y += ExtraMath.Osc(-5, 5, speed: 2);
            NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToPlayer, 0.02f);
            NPC.rotation = NPC.velocity.X * 0.02f + ExtraMath.Osc(-0.05f, 0.05f, speed: 2);
        }

        private void AI_Warn()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if(Timer == 2)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Kabloowie>(), 1, 1, Main.myPlayer);
                }
            }
            _bounceOffset = Vector2.Lerp(Vector2.UnitY * -64, Vector2.UnitY * 64, ExtraMath.Osc(0f, 1f, speed: 4)) * MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / 60f));
            NPC.ai[3] = 0;
            if(Timer >= 60)
            {
                SwitchState(AIState.IdleFollow);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
            Vector2 offset = _upDownOffset;
            offset.Y += ExtraMath.Osc(-8f, 8f, 8f);
            offset += _bounceOffset;
            drawer.worldPosition += offset;
            drawer.rotation += _rotOffset;
            spriteBatch.Draw(drawer);
            return false;
        }
    }
    public class PunkerPrime : ScarletBoss,
        IDrawOutlines
    {
        private Vector2 _teleportPosition;
        private enum AIState
        {
            Spawn,
            Despawn,
            Idle,
            Flurry,
            Death,
            Warning_Prepare_Attacks,
            SummonArms,
            Special_Start,
            Special_Loop,
            Special_End
        }
        private const string Anim_Bouncing_Fast = "bouncefast";
        private const string Anim_Bouncing_Slow = "bounceslow";
        private const string Anim_Idle = "idle";
        private Animator _animator;
        private Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = new Animator();
                    var bounceFast = new SpriteAnimation(0, 9, isLooping: true, frameSpeed: 0.4f);
                    _animator.AddAnimation(Anim_Bouncing_Fast, bounceFast);

                    var running = new SpriteAnimation(11, 15, isLooping: true, frameSpeed: 0.1f);
                    _animator.AddAnimation(Anim_Bouncing_Slow, running);

                    var idle = new SpriteAnimation(10, 10, isLooping: true, frameSpeed: 0.35f);
                    _animator.AddAnimation(Anim_Idle, idle);
                }

                return _animator;
            }
        }
        private PunkerPrimeDraw _draw;
        private Vector2 _startCenter;
        private Vector2 _hoverCenter;
        private Color TargetOutlineColor;
        private bool[] _disabledArms;
        private bool _showNamePlate;
        private bool _phaseTransition;
        private float _flurryTimer;

        private float _upDown;
        private float _rotOffset;
        private Vector2 _upDownOffset;
        private Vector2 _bounceOffset;
        private string _animationToPlay = string.Empty;
        private Queue<int> _armQueueBacking;
        private Queue<int> ArmQueue
        {
            get
            {
                if(_armQueueBacking == null)
                {
                    _armQueueBacking = new Queue<int>();
                }
                if(_armQueueBacking.Count <= 0)
                {
                    while(_armQueueBacking.Count < 5)
                    {
                        int armToSummon = Main.rand.Next(0, 5);
                        if (InPhase2)
                        {
                            armToSummon = Main.rand.Next(0, 8);
                        }
                        if (_armQueueBacking.Contains(armToSummon))
                            continue;
                        _armQueueBacking.Enqueue(armToSummon);
                    }
                }
                return _armQueueBacking;
            }
        }
        private Metronome _metronome;
        private Metronome Metronome
        {
            get
            {
                _metronome ??= new Metronome(150);
                return _metronome;
            }
        }
        private ref float Timer => ref NPC.ai[0];

        private PunkerPrimeArm[] _arms;
        private NPC _boomBoxNPC;
        private ref PunkerPrimeArm Chainsaw1 => ref _arms[0];
        private ref PunkerPrimeArm Chainsaw2 => ref _arms[1];
        private ref PunkerPrimeArm Drill => ref _arms[2];
        private ref PunkerPrimeArm Pincher => ref _arms[3];
        private ref PunkerPrimeArm SawbladeLauncher => ref _arms[4];

        public bool InPhase2 => NPC.life < NPC.lifeMax / 2;
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float SuperchargeTimer => ref NPC.ai[2];
        private ref float SpecialTimer => ref NPC.ai[3];
        private int PrimeSawbladeDamage => 30;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_teleportPosition);
            writer.WriteVector2(_hoverCenter);
            writer.WriteVector2(_startCenter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _teleportPosition = reader.ReadVector2();
            _hoverCenter = reader.ReadVector2();
            _startCenter = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 16;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _draw.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 100;
            NPC.defense = 28;
            NPC.lifeMax = 18000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/PunkerPrime");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            Animator.Update();
            NPC.frame.Y = Animator.GetFrameY(frameHeight);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && false;
        }

        private void ManageMetronome()
        {
            Metronome.Update();
            if (_upDown == 0)
                _upDown = 1;
            if (Metronome.beatHit)
            {
                if(!string.IsNullOrEmpty(_animationToPlay))
                    Animator.PlayAnimation(_animationToPlay);
                _upDown *= -1;
            }
            _rotOffset = MathHelper.Lerp(_rotOffset, 0.1f * _upDown, 0.2f);
            _upDownOffset = Vector2.Lerp(_upDownOffset, Vector2.UnitY * _upDown * 8, 0.2f);
        }
        public override void AI()
        {
            base.AI();
       
            _draw.outlineColor = Color.Lerp(_draw.outlineColor, TargetOutlineColor, 0.1f);
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }

            if (_teleportPosition != Vector2.Zero)
            {
                NPC.position = _teleportPosition;
                _teleportPosition = Vector2.Zero;
            }

            if(!_phaseTransition && InPhase2)
            {
                float numDust = 16;
                for(float d = 0; d < numDust; d++)
                {
                    Vector2 spawnPosition = NPC.Top;
                    spawnPosition.X += Main.rand.NextFloat(-64, 64);

                    Vector2 spawnVelocity = Vector2.Zero;
                    spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);
                    spawnVelocity += Main.rand.NextVector2Circular(8, 8);
                    
                    
                    float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                    var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
                }

                var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.Yellow, Color.Red, Color.DarkRed);
                boom.Scale *= 2f;

                float numGlowDust = 16f;
                for(float d = 0; d < numGlowDust; d++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                SoundStyle mechSteaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
                SoundEngine.PlaySound(mechSteaming, NPC.position);
                _phaseTransition = true;
            }
            ManageMetronome();
            MoveSlightlyTowardMe();
            Lighting.AddLight(NPC.Center, TorchID.Red);
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Flurry:
                    AI_Flurry();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
                case AIState.SummonArms:
                    AI_SummonArms();
                    break;
                case AIState.Warning_Prepare_Attacks:
                    AI_WarningPrepareAttacks();
                    break;
                case AIState.Special_Start:
                    AI_Special();
                    break;
                case AIState.Special_Loop:
                    AI_SpecialLoop();
                    break;
                case AIState.Special_End:
                    AI_SpecialEnd();
                    break;
            }

        }

        private bool CanUseArm(int armIndex)
        {
            return !_disabledArms[armIndex];
        }

        private void MoveSlightlyTowardMe()
        {
            Player player = Main.LocalPlayer;
            Vector2 vectorHere = (NPC.Center - player.Center);
            vectorHere *= 0.2f;
            OffsetCameraModifier.FocusTargetOffset = vectorHere;
        }
        private void AI_SummonArms()
        {
            int armToSummon = ArmQueue.Dequeue();
            
            //Just recall this function until you get to an arm that you can summon
            if (!CanUseArm(armToSummon))
            {
                AI_SummonArms();
                return;
            }
            if (MultiplayerHelper.IsHost)
            {
                PunkerPrimeArm arm = _arms[armToSummon];
     
                if (InPhase2 && SuperchargeTimer > 600)
                {
                    SuperchargeTimer = 0f;
                    arm.SuperchargeAttack();
                }
                else
                {
                    arm.Attack();
                }
            }

            SwitchState(AIState.Flurry);
        }

        private void SummonArm()
        {
            int armToSummon = ArmQueue.Dequeue();

            //Just recall this function until you get to an arm that you can summon
            if (!CanUseArm(armToSummon))
            {
                AI_SummonArms();
                return;
            }
            if (MultiplayerHelper.IsHost)
            {
                PunkerPrimeArm arm = _arms[armToSummon];

                if (InPhase2 && SuperchargeTimer > 600)
                {
                    SuperchargeTimer = 0f;
                    arm.SuperchargeAttack();
                }
                else
                {
                    arm.Attack();
                }
            }
        }

        private void AI_WarningPrepareAttacks()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }
        }

        private void AI_Special()
        {
            //This is the saw attack that this goober has
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                _startCenter = NPC.Center;
                _hoverCenter = MyTarget.Center + new Vector2(0, -300);
                SoundStyle prepSound = AssetRegistry.Sounds.SteamPunking.MechSaw;
                prepSound.PitchVariance = 0.3f;
                prepSound.Pitch = -0.5f;
                SoundEngine.PlaySound(prepSound, NPC.position);
            }

            TargetOutlineColor = Color.Yellow;
            float revTime = 60f;
            float completionRatio = Timer / revTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 targetCenter = Vector2.Lerp(_startCenter, _hoverCenter, ease);
            Vector2 velocity = (targetCenter - NPC.Center);
            NPC.velocity = velocity;
            NPC.rotation = NPC.velocity.X * 0.02f;

            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 1f, 0.1f);
            if(Timer >= revTime)
            {
                SwitchState(AIState.Special_Loop);
            }
        }

        private void AI_SpecialLoop()
        {
            Timer++;
            if(Timer == 1)
            {
                _startCenter = _hoverCenter;
                _hoverCenter = MyTarget.Center + new Vector2(0, -64);

                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero,
                        ModContent.ProjectileType<PrimeMegaSaw>(), PrimeSawbladeDamage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                }
            }

            TargetOutlineColor = Color.Red;
            float revTime = 60f;
            float completionRatio = Timer / revTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 targetCenter = Vector2.Lerp(_startCenter, _hoverCenter, ease);
            Vector2 velocity = (targetCenter - NPC.Center);
            NPC.velocity = velocity;
            _draw.shakeOffset = Main.rand.NextVector2Circular(2, 2);
            NPC.rotation = _draw.shakeOffset.ToRotation() * 0.02f;

            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 1f, 0.1f);
            if(Timer >= revTime)
            {
                SwitchState(AIState.Special_End);
            }
        }

        private void AI_SpecialEnd()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }

            if(Timer % 5 == 0)
            {
                Dust.NewDust(NPC.BottomLeft, NPC.width, 2, DustID.FireworkFountain_Red);
            }

            if(Timer % 6 == 0)
            {
                LegacyParticle.NewParticle<SparkParticle>(NPC.Bottom + Main.rand.NextVector2Circular(16, 16), 
                    Main.rand.NextVector2Circular(4, 4), Color.Red);
            }
            if(Timer % 3 == 0)
            {
                SpawnSteamParticle();
            }

            float endTime = 240;
            Vector2 velToTarget = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            velToTarget *= 8f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, velToTarget, 0.1f);
            _draw.shakeOffset = Main.rand.NextVector2Circular(2, 2);
            NPC.rotation = _draw.shakeOffset.ToRotation() * 0.02f;
            if(Timer >= endTime)
            {
                SwitchState(AIState.Idle);
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

        private void TeleportTo(Vector2 teleportCenter)
        {
            if (MultiplayerHelper.IsHost)
            {
                NPC.Center = teleportCenter;
                _teleportPosition = NPC.position;
                NPC.netUpdate = true;
            }
        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();

                //Start from above the player and come down
                Vector2 teleportCenter = MyTarget.Center + new Vector2(0, -500);
                TeleportTo(teleportCenter);

                SoundStyle mechTurnSound = AssetRegistry.Sounds.SteamPunking.MechTurn;
                mechTurnSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(mechTurnSound, NPC.position);

                SummonArms();
            }

            TargetOutlineColor = Color.Transparent;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;

            float time = 120f;
            float completionRatio = Timer / time;
            float ease = EasingFunction.InOutSine(completionRatio);
            float yVelocity = MathHelper.Lerp(3f, 0f, ease);
            NPC.velocity.Y = yVelocity;
            NPC.velocity.X = 0;
            NPC.rotation = 0;
            if (Timer >= time)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Despawn()
        {
            TargetOutlineColor = Color.Transparent;
            //Just fly up and despawn, very simepl
            Timer++;
            float despawnTime = 90;
            NPC.velocity.Y -= 0.5f;
            NPC.velocity.X *= 0.5f;
            NPC.rotation = 0;
            if (Timer >= despawnTime)
            {
                NPC.active = false;
            }
        }

        private bool CanAttack()
        {
            //This is going to check all of the arms and check how many of them are moving
            int attackingArmCount = 0;
            for(int i = 0; i < _arms.Length; i++)
            {
                PunkerPrimeArm arm = _arms[i];

                //wait
                if (arm.isAttacking)
                    attackingArmCount++;
            }
            return attackingArmCount < 2;
        }

        private T SummonArm<T>() where T  : PunkerPrimeArm
        {
            T t = ModContent.GetInstance<T>();
            int type = t.Type;
            int x = (int)NPC.Center.X;
            int y = (int)NPC.Center.Y;
            int npcIndex = NPC.NewNPC(SourceFromThis, x, y, type, ai1: NPC.whoAmI);
            T arm = Main.npc[npcIndex].ModNPC as T;
            return arm;
        }

        private void SummonArms()
        {
            if (!MultiplayerHelper.IsHost)
                return;

            _arms = new PunkerPrimeArm[8];
            _disabledArms = new bool[8];
            _arms[0] = SummonArm<Chainsaw>();
            _arms[1] = SummonArm<Chainsaw2>();
            _arms[2] = SummonArm<Drill>();
            _arms[3] = SummonArm<Pincher>();
            _arms[4] = SummonArm<SawbladeLauncher>();
            _arms[5] = SummonArm<AssaultRifle>();
            _arms[6] = SummonArm<LaserRifle>();
            _arms[7] = SummonArm<ElectroFieldLauncher>();

            int x = (int)NPC.Center.X;
            int y = (int)NPC.Center.Y;
            _boomBoxNPC = NPC.NewNPCDirect(SourceFromThis, x, y, ModContent.NPCType<Boombox>(), ai2: NPC.whoAmI);
        }

        private void AI_Idle()
        {
            _draw.afterImageStrength *= 0.5f;
            _animationToPlay = Anim_Idle;
    

            //Steampunker prime is just going to hover around and above you most of the time for the most part
            //If you get far from him he'll track you, but otherwise he's mostly stationary and doesn't move too much
            //Which should be easy to deal with?
            //The extension from melee should make it easy to hit the cores
            //Hopefully;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            if (InPhase2)
            {
                SuperchargeTimer++;
            }

            SpecialTimer++;

            //Starts slow and gets faster over time
            float idleTime = 240;
            if (Timer % 15 == 0)
            {
                SpawnSteamParticle();
                if (Main.rand.NextBool(3))
                {
                    var d = Dust.NewDustPerfect(NPC.Top, ModContent.DustType<TSmokeDust>(), Scale: Main.rand.NextFloat(0.5f, 1.2f));
                }
            }

            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 0f, 0.1f);
            if (!_showNamePlate)
            {
                ShowNamePlate();
                _showNamePlate = true;
            }
            Chase(speedMult: 0.25f);

            if(Timer > idleTime / 2)
            {
                _animationToPlay = Anim_Bouncing_Slow;
            }

            TargetOutlineColor = Color.Transparent;
            NPC.rotation = NPC.velocity.X * 0.02f;
            if (Timer >= idleTime)
            {
                if (SpecialTimer >= 1000)
                {
                    SpecialTimer = 0f;
                    SwitchState(AIState.Special_Start);
                }
                else
                {
                    SwitchState(AIState.Flurry);
                }

            }
        }

        private void AI_Flurry()
        {
            Timer++;
            _animationToPlay = Anim_Bouncing_Fast;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _boomBoxNPC.ai[3] = 1;
                    _boomBoxNPC.netUpdate = true;
                }

                NPC.TargetClosest();
            }

            if (Timer % 15 == 0)
            {
                SpawnSteamParticle();
                if (Main.rand.NextBool(3))
                {
                    var d = Dust.NewDustPerfect(NPC.Top, ModContent.DustType<TSmokeDust>(), Scale: Main.rand.NextFloat(0.5f, 1.2f));
                }
            }

            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 1f, 0.1f);
            Chase();
            NPC.velocity *= 0.94f;
            TargetOutlineColor = Color.Transparent;
            NPC.rotation = NPC.velocity.X * 0.02f;
            if(Timer > 100 && Timer % 90 == 0 && Timer < 400)
            {
                SummonArm();
            }

            if(Timer >= 500)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void Chase(float speedMult = 1f)
        {
            //Crazy movement code
            Vector2 targetPosition = MyTarget.Center;
            targetPosition.Y -= 128;
            Vector2 velocityToPlayer = (targetPosition - NPC.Center);
            velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
            float dist = Vector2.Distance(NPC.Center, targetPosition);
            if (dist <= 0)
                dist = 1;

            float interp = dist / 384;
            interp = EasingFunction.InOutSine(interp);
            float speed = MathHelper.Lerp(6, 20, interp);
            speed *= speedMult;

            float xDist = MathF.Abs(targetPosition.X - NPC.Center.X);
            if (xDist < 256)
                velocityToPlayer.Y -= 0.5f;

            if (dist < speed)
                speed = dist;
            velocityToPlayer *= speed;
            velocityToPlayer *= ExtraMath.Osc(0.5f, 1f, speed: 2);
            velocityToPlayer.Y += ExtraMath.Osc(-5, 5, speed: 2);
            NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToPlayer, 0.04f);
        }

        private void ChooseAttack()
        {
            if (!CanAttack())
                return;
            if(SpecialTimer >= 1000)
            {
                SpecialTimer = 0f;
                SwitchState(AIState.Special_Start);
            }
            else
            {
                SwitchState(AIState.SummonArms);
            }

        }

        private void AI_Death()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            float deathTime = 300f;
            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            if (Timer % 2 == 0)
            {
                _draw.shakeOffset = Main.rand.NextVector2Circular(16, 16);
                NPC.rotation = _draw.shakeOffset.X * 0.05f;
            }

            if (Timer % 12 == 0)
            {
                Vector2 spawnPoint = NPC.Top;
                spawnPoint.X += Main.rand.NextFloat(-64f, 64f);
                var fireDust = Dust.NewDustPerfect(spawnPoint, DustID.FireworkFountain_Red, Scale: Main.rand.NextFloat(0.5f, 1f));
                fireDust.noGravity = false;
            }

            NPC.velocity = Vector2.Zero;
            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 0f, 0.1f);
            _draw.outlineColor = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12f));
            if (Timer >= deathTime)
            {
                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(),
                        (Vector2.One * Main.rand.Next(5, 15)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
                for (float f = 0; f < 12; f++)
                {
                    Vector2 v = Main.rand.NextVector2Circular(128, 128);
                    FXUtil.GlowStretch(NPC.Center, v);
                }

                float numDust = 32;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(32, 32);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), dustVelocity,
                        newColor: Color.Red,
                        Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/GlocketRouncher");
                explosionSound.Pitch = -0.5f;
                SoundEngine.PlaySound(explosionSound, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Yellow, Color.Red);
                boom.Scale *= 3f;
                ShakeModSystem.Shake = 16;
                var p = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Red, Color.Black);
                NPC.Kill();
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.PunkerPrime);
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
        private void SpawnSteamParticle()
        {
            Vector2 spawnPosition = NPC.Top;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (_arms == null)
                return false;

            for(int i = 0; i < _arms.Length; i++)
            {
                var arm = _arms[i];
                arm.DrawPowerCord(spriteBatch, screenPos, drawColor);
            }

            for (int i = 0; i < _arms.Length; i++)
            {
                var arm = _arms[i];
                arm.DrawGunArm(spriteBatch, screenPos, drawColor);
            }

            for (int i = 0; i < _arms.Length; i++)
            {
                var arm = _arms[i];
                arm.DrawGunEffects(spriteBatch, screenPos, drawColor);
            }

            for (int i = 0; i < _arms.Length; i++)
            {
                var arm = _arms[i];
                arm.DrawGun(spriteBatch, screenPos, drawColor);
            }
            for (int i = 0; i < _arms.Length; i++)
            {
                var arm = _arms[i];
                arm.DrawGlowBall(spriteBatch, screenPos, drawColor);
            }
            DrawBodyAfterImage(spriteBatch, screenPos);
            DrawBodySprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawBodyAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float length = NPCID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < length; i++)
            {
                float f = i;
                float completionRatio = f / length;
                Vector2 oldPosition = NPC.oldPos[i];
                Vector2 oldCenter = oldPosition + NPC.Size / 2f - screenPos;
                Color color = Color.Red;
                color *= 0.1f;
                color *= _draw.afterImageStrength;
                color *= MathHelper.SmoothStep(1f, 0f, completionRatio);
                spriteBatch.Draw(texture, oldCenter, frame, color, NPC.rotation, drawOrigin, _draw.scale, SpriteEffects.None, 0);
            }
        }

        private void DrawBodySprite(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle frame = NPC.frame;
            Vector2 drawCenter = NPC.Center - screenPos;
            Vector2 drawOrigin = frame.Size() / 2f;
            drawCenter += _draw.shakeOffset;
            spriteBatch.Draw(texture, drawCenter + _upDownOffset, frame, color, NPC.rotation + _rotOffset, drawOrigin, _draw.scale, SpriteEffects.None, 0);
        }


        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2f;
            if (_draw.outlineColor == Color.Transparent)
                return;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            DrawBodySprite(spriteBatch, screenPos + h, _draw.outlineColor);
            DrawBodySprite(spriteBatch, screenPos - h, _draw.outlineColor);
            DrawBodySprite(spriteBatch, screenPos + v, _draw.outlineColor);
            DrawBodySprite(spriteBatch, screenPos - v, _draw.outlineColor);
        }
    }
}
