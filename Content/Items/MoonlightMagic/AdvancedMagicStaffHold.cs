using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class AdvancedMagicStaffHold : ModProjectile
    {
        private enum AIState
        {
            Charge,
            Release,
            PullBack,
            Swing
        }
        private AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private TexturedQuad _texturedQuad;
        private TexturedQuad TexturedQuad
        {
            get
            {
                _texturedQuad ??= new TexturedQuad();
                return _texturedQuad;
            }
        }

        private Asset<Texture2D> _ringTextureAsset;
        private OvalSwing _ovalSwing;
        private float _level;
        private float _circleTimer;

        private float _outTimer;
        private float _ringTimer1;
        private float _ringTimer2;
        private float _ringTimer3;

        private float _midringTimer1;
        private float _midringTimer2;
        private float _midringTimer3;

        private bool _hasFired;

        private float _overchargeScaleTimer;
        private float CrosshairProgress;
        private float Interpolant;
        private ref float Timer => ref Projectile.ai[1];
        private ref float ChargeProgress => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        private Player Owner => Main.player[Projectile.owner];
        private AdvancedMagicPlayer MagicPlayer => Owner.GetModPlayer<AdvancedMagicPlayer>();
        private BaseElement Element => GetElement();

        private float GetChargeTime()
        {
            const float Base_Charge_Time = 360;
            AdvancedMagicPlayer magicPlayer = Owner.GetModPlayer<AdvancedMagicPlayer>();
            float chargeTime = Base_Charge_Time;
            float decrease = chargeTime * magicPlayer.chargeTimeBonus;
            chargeTime -= decrease;
            return chargeTime;
        }

        private float GetLevelChargeTime()
        {
            return GetChargeTime() / 3;
        }

        public bool IsOvercharging()
        {
            return _level >= 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.timeLeft = int.MaxValue;
        }

        private BaseElement GetElement()
        {
            BaseStaff staff = Owner.HeldItem.ModItem as BaseStaff;
            Item elementItem = staff.GetElement();
            BaseElement element = elementItem.ModItem as BaseElement;
            return element;
        }

        private void IncreaseRingTimers()
        {
            if (_level >= 1)
            {
                _ringTimer1++;
            }
            if (_level >= 2)
            {
                _ringTimer2++;
            }
            if (_level >= 3)
            {
                _ringTimer3++;
            }
            _ringTimer1 = MathHelper.Clamp(_ringTimer1, 0f, 30f);
            _ringTimer2 = MathHelper.Clamp(_ringTimer2, 0f, 30f);
            _ringTimer3 = MathHelper.Clamp(_ringTimer3, 0f, 30f);

            if (_circleTimer >= 60)
            {
                _midringTimer1++;
            }
            if (_circleTimer >= 180)
            {
                _midringTimer2++;
            }
            if (_circleTimer >= 300)
            {
                _midringTimer3++;
            }

            _midringTimer1 = MathHelper.Clamp(_midringTimer1, 0f, 30f);
            _midringTimer2 = MathHelper.Clamp(_midringTimer2, 0f, 30f);
            _midringTimer3 = MathHelper.Clamp(_midringTimer3, 0f, 30f);
        }
        public override void AI()
        {
            base.AI();
            CrosshairProgress = MathHelper.Lerp(CrosshairProgress, 1f, 0.1f);


            _circleTimer++;
            switch (State)
            {
                case AIState.Charge:
                    AI_Charge();
                    IncreaseRingTimers();
                    break;

                case AIState.PullBack:
                    AI_Pullback();
                    break;
                case AIState.Swing:
                    AI_Swing();
                    break;
                case AIState.Release:
                    AI_Release();
                    break;


            }

    
            //        Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Vector2 vel = -Vector2.UnitY;
            Owner.itemRotation = (float)Math.Atan2(
                vel.Y * Projectile.direction,
                vel.X * Projectile.direction);

        }

        private void SwitchState(AIState state)
        {
            State = state;
            Timer = 0;
            Projectile.netUpdate = true;
        }

        private void SetHoldPosition()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                // Projectile.spriteDirection = (int)Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;

                Projectile.netUpdate = true;
            }


            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }
            float drawRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * Owner.direction;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,
               drawRotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full,
             drawRotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            //Owner.heldProj = Projectile.whoAmI;
        }

        public float GetSwingTime(float baseSwingTime)
        {
            float swingTime = baseSwingTime;
            return (int)(swingTime);
        }




        private void AI_Charge()
        {
            AdvancedMagicPlayer magicPlayer = Owner.GetModPlayer<AdvancedMagicPlayer>();
            Item heldItem = Owner.HeldItem;
            BaseStaff staff = Owner.HeldItem.ModItem as BaseStaff;
            foreach (var enchantmentItem in staff.equippedEnchantments)
            {
                if (enchantmentItem.ModItem is BaseEnchantment e)
                {

                    e.AI_Charge(magicPlayer, this);
                }
            }

            Timer++;
            if (Timer == 1)
            {
                MagicPlayer.ResetChargeEffects();
                SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/StormKnight_Rechage");
                mySound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mySound, Projectile.position);

            }
            if (IsOvercharging())
            {
                if (Timer % 60 == 0)
                {
                    _overchargeScaleTimer = 0;
                    SoundStyle mySound = AssetRegistry.Sounds.MagicWand.BasicCharge;
                    mySound.PitchVariance = 0.05f;
                    mySound.Pitch = MathHelper.Lerp(0f, 0.8f, MagicPlayer.chargeDamageBonus);
                    SoundEngine.PlaySound(mySound, Projectile.position);
                }
                _overchargeScaleTimer++;
            }
            else
            {
                _overchargeScaleTimer = 60;
            }
            if (Main.myPlayer == Projectile.owner)
            {

                Projectile.velocity = Owner.Center.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
            }
            if(Timer % 18 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Owner.Center + Projectile.velocity * 64, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60)), Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.outerColor = Element.GetElementColor();
            }
            if (Timer == GetLevelChargeTime())
            {
                if (_level < 3)
                {
                    SoundEngine.PlaySound(Element.ChargeSound, Projectile.position);
                    _level++;
                    if (_level < 3)
                    {
                        Timer = 0;
                    }
                }
            }

            ChargeProgress = Timer / GetLevelChargeTime();
            ChargeProgress = MathHelper.Clamp(ChargeProgress, 0, 1);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Owner.channel)
                {
                    SwitchState(AIState.PullBack);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Lighting.AddLight((Projectile.Center + Projectile.velocity * 64), Color.LightCyan.ToVector3() * 1.5f);
            SetHoldPosition();
        }

        private void AI_Pullback()
        {
            Timer++;
            float speed = 2;
            _ringTimer1 -= speed;
            _ringTimer2 -= speed;
            _ringTimer3 -= speed;

            _midringTimer1 -= speed;
            _midringTimer2 -= speed;
            _midringTimer3 -= speed;

            SetHoldPosition();
            if (Timer >= 15f)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    SwitchState(AIState.Swing);
                }
            }
        }

        private int CalculateFinalDamage()
        {
            float levelProgress = (_level / 3f);

            float baseDamage = MathHelper.Lerp(1, Projectile.damage, levelProgress);
            AdvancedMagicPlayer magicPlayer = Owner.GetModPlayer<AdvancedMagicPlayer>();
            float finalDamage = baseDamage * (1f + magicPlayer.chargeDamageBonus);// * (1f - magicPlayer.chargeDamagePenalty);



            int damage = (int)finalDamage;
            return damage;
        }
        private void AI_Swing()
        {
            _outTimer++;
            Timer++;
            _ovalSwing ??= new OvalSwing();
            _ovalSwing.XSwingRadius = 64;
            _ovalSwing.YSwingRadius = 24;
            _ovalSwing.SwingDegrees = 270;
            _ovalSwing.Duration = 30;
            _ovalSwing.SetDirection(1);
            float duration = _ovalSwing.GetDuration(1f / Owner.GetTotalAttackSpeed(Projectile.DamageType));

            float swingTime = GetSwingTime(duration);
            Interpolant = Timer / swingTime;
            Interpolant = MathHelper.Clamp(Interpolant, 0f, 1f);
            if (Interpolant >= 0.5f && !_hasFired && Main.myPlayer == Projectile.owner)
            {
                Item heldItem = Owner.HeldItem;
                float levelProgress = (_level / 3f);
                int damage = CalculateFinalDamage();
                float knockback = Projectile.knockBack;
                Vector2 fireVelocity = Projectile.velocity * 15;
                BaseStaff staff = Owner.HeldItem.ModItem as BaseStaff;


                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = fireVelocity;
                Projectile.damage = damage;
                Projectile.knockBack = knockback;

                Vector2 ballPosition = Owner.Center + Projectile.velocity * 64;
                AdvancedMagicUtil.NewMagicProjectile(ballPosition, staff, Projectile, levelProgress);
                Projectile.velocity = oldVelocity;

                for (int i = 0; i < 7 * levelProgress; i++)
                {
                    Vector2 velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(25f, 45f);
                    var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Element.GetElementColor();
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50) * levelProgress;
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f) * levelProgress;
                    particle.VectorScale *= 0.5f;
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 8);

                FXUtil.GlowCircleBoom(ballPosition,
                    innerColor: Color.White,
                    glowColor: Element.GetElementColor(),
                    outerGlowColor: Color.Lerp(Element.GetElementColor(), Color.Black, 0.5f), duration: 25, baseSize: 0.14f);

             
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);

                    var particle = FXUtil.GlowCircleDetailedBoom1(Owner.Center + Projectile.velocity * 64,
                        innerColor: Color.White,
                        glowColor: Element.GetElementColor(),
                        outerGlowColor: Color.Lerp(Element.GetElementColor(), Color.Black, 0.5f),
                        baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                float numDust = 4;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 15;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                    velocity *= Main.rand.NextFloat(0.3f, 2f);
                    Dust.NewDustPerfect(ballPosition, ModContent.DustType<GlowDust>(), velocity, newColor: Element.GetElementColor(), Scale: 2f);
                    SparkleParticle sp = Particle<SparkleParticle>.Spawn(ballPosition, velocity, Scale: Main.rand.NextFloat(0.6f, 1f));
                    sp.gravity = 0;
                    sp.dampening = 0.1f;
                    sp.outerColor = Element.GetElementColor();
                }
                _hasFired = true;
            }
            //For the purposes of netcode,
            //Killing the projectile manually instead of trying to sync time left is better I think.
            if (Timer >= swingTime)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    SwitchState(AIState.Release);
                }
            }

            //We now have the offset so we can apply that to the weapon
            _ovalSwing.UpdateSwing(Interpolant, Projectile.Center, Projectile.velocity, out Vector2 offset);
            Projectile.Center = Owner.Center + offset;
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;
        }

        private void AI_Release()
        {
            Projectile.Kill();
        }

        private void DrawPixelatedRings(GraphicsDevice graphicsDevice)
        {

            var element = Element;

            float ring1Ease = EasingFunction.InOutSine(_ringTimer1 / 30f);
            float ring2Ease = EasingFunction.InOutSine(_ringTimer2 / 30f);
            float ring3Ease = EasingFunction.InOutSine(_ringTimer3 / 30f);
            Vector2 ring1Scale = Vector2.Lerp(Vector2.Zero, Vector2.One * new Vector2(1, 0.35f), ring1Ease);
            Vector2 ring2Scale = Vector2.Lerp(Vector2.Zero, Vector2.One * new Vector2(1, 0.35f), ring2Ease);
            Vector2 ring3Scale = Vector2.Lerp(Vector2.Zero, Vector2.One * new Vector2(1, 0.35f), ring3Ease);

            bool overchargingVisual = IsOvercharging() && MagicPlayer.overchargingVisual;
            float chargingSpeed = overchargingVisual ? 8 : 1;
            Color chargingColor = overchargingVisual ? Color.LightPink : Color.White;


            Vector2 ring1Offset = Projectile.velocity * MathHelper.Lerp(8, 32, ExtraMath.Osc(0f, 1f, speed: chargingSpeed));
            Vector2 ring2Offset = Projectile.velocity * MathHelper.Lerp(8, 32, ExtraMath.Osc(0f, 1f, speed: chargingSpeed, offset: 3));
            Vector2 ring3Offset = Projectile.velocity * MathHelper.Lerp(8, 32, ExtraMath.Osc(0f, 1f, speed: chargingSpeed, offset: 6));
     
            if (_level >= 1)
            {
                float perspectiveRotation = Main.GlobalTimeWrappedHourly * 3;
                DrawRingInner(Owner.Center + Projectile.velocity * 64 * ring1Ease + ring1Offset, 2, ring1Scale, chargingColor * ring1Ease, perspectiveRotation);
            }
            if (_level >= 2)
            {
                float perspectiveRotation2 = Main.GlobalTimeWrappedHourly * 3;
                DrawRingInner(Owner.Center + Projectile.velocity * 100 * ring2Ease + ring2Offset, 1, ring2Scale, chargingColor * ring2Ease, perspectiveRotation2);
            }
            if (_level >= 3)
            {
                float perspectiveRotation3 = Main.GlobalTimeWrappedHourly * 3;
                DrawRingInner(Owner.Center + Projectile.velocity * 140 * ring3Ease + ring3Offset, 0, ring3Scale, chargingColor * ring3Ease, perspectiveRotation3);
            }
        }

        private void DrawRingInner(Vector2 center, int frame, Vector2 size, Color color, float perpsectiveRotation)
        {

            if (_ringTextureAsset == null)
            {
      
                string texturePath = Element.Texture + "_Ring";
                string basicElementTexturePath = ModContent.GetInstance<BasicElement>().Texture + "_Ring";
                if(!ModContent.RequestIfExists<Texture2D>(texturePath, out _ringTextureAsset, AssetRequestMode.ImmediateLoad))
                {
                    ModContent.RequestIfExists<Texture2D>(basicElementTexturePath, out _ringTextureAsset, AssetRequestMode.ImmediateLoad);
                }


                return;
            }

            MagicCircleShader magicCircleShader = MagicCircleShader.Instance;

            //Here we need to prepare the shader
            float numFrames = 3f;
            float f = frame;
            Vector2 tiling = new Vector2(1f, 1f / numFrames);
            Vector2 offset = new Vector2(0, f * 1f / numFrames);
            Vector4 tilingOffset = new Vector4(offset.X, offset.Y, tiling.X, tiling.Y);
            magicCircleShader.TilingOffset = tilingOffset;
            magicCircleShader.RingTexture = _ringTextureAsset;

            Color auraColor = Element.GetElementColor();
            auraColor = auraColor.MultiplyRGB(color);

            TexturedQuad.CalculatePerspectiveCenterVertices(center, 180 , 180 , Projectile.velocity.ToRotation(), perpsectiveRotation);
            TexturedQuad.SetColor(auraColor);
            TexturedQuad.DrawWithShader(magicCircleShader);
        }

        private void DrawRingTrail(float holdOffset, Vector2 scaleOffset, float rotateSpeed, Vector2 ringScale)
        {

            var element = Element;
            float drawRotation = Projectile.velocity.ToRotation();
            float radians = Main.GlobalTimeWrappedHourly * rotateSpeed;

            float xRadius = 4 * ringScale.X;
            float yRadius = 32 * ringScale.Y;
            List<Vector2> points = new List<Vector2>();
            List<float> rot = new List<float>();
            for (int i = 0; i < 48; i++)
            {
                float rads = radians + i * 0.05f;
                Vector2 offset = new Vector2();
                offset.X = xRadius * MathF.Cos(rads);// * scaleOffset.X;
                offset.Y = yRadius * MathF.Sin(rads);// * scaleOffset.Y;
                offset = offset.RotatedBy(drawRotation);
                Vector2 ringPos = Owner.Center + offset + Projectile.velocity * holdOffset;
                points.Add(ringPos);
                rot.Add(0);
            }

            element.DrawRingTrail(points.ToArray(), rot.ToArray());
        }
        private void DrawRingV2(ref Color lightColor)
        {

            float ring1Ease = EasingFunction.InOutSine(_midringTimer1 / 30f);
            float ring2Ease = EasingFunction.InOutSine(_midringTimer2 / 30f);
            float ring3Ease = EasingFunction.InOutSine(_midringTimer3 / 30f);
            Vector2 ring1Scale = Vector2.Lerp(Vector2.Zero, Vector2.One * new Vector2(1, 1f), ring1Ease);
            Vector2 ring2Scale = Vector2.Lerp(Vector2.Zero, Vector2.One * new Vector2(1, 1f) * 2f, ring2Ease);
            Vector2 ring3Scale = Vector2.Lerp(Vector2.Zero, Vector2.One * new Vector2(1, 1f) * 0.5f, ring3Ease);


            Vector2 ring1Offset = Projectile.velocity * MathHelper.Lerp(8, 32, ExtraMath.Osc(0f, 1f));
            Vector2 ring2Offset = Projectile.velocity * MathHelper.Lerp(8, 32, ExtraMath.Osc(0f, 1f, offset: 3));
            Vector2 ring3Offset = Projectile.velocity * MathHelper.Lerp(8, 32, ExtraMath.Osc(0f, 1f, offset: 6));
            if (_level >= 0)
            {
                DrawRingTrail(64, ring1Offset, 4, ring1Scale);
            }
            if (_level >= 1)
            {
                DrawRingTrail(100, ring2Offset, 4, ring2Scale);
            }
            if (_level >= 2)
            {
                DrawRingTrail(140, ring3Offset, 4, ring3Scale);
            }



        }



        private void DrawStaff(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos + Projectile.velocity * 24, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, drawPos + Projectile.velocity * 32, null, new Color(255, 128, 125, 0) * ChargeProgress * 0.5f, drawRotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
        }

        private void DrawEnergyBall(ref Color lightColor)
        {
            if (_hasFired)
                return;

            var element = Element;
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Owner.Center - Main.screenPosition;
            var shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            float progress = _circleTimer / 360f;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            shader.Size = MathHelper.Lerp(0f, 0.06f, Easing.OutCubic(progress));


            //Colors
            Color startInner = element.GetElementColor();
            Color startGlow = element.GetElementColor();
            Color startOuterGlow = Color.Lerp(startGlow, Color.Black, VectorHelper.Osc(0f, 1f, speed: 64));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 3.5f;
            shader.Apply();


            float drawScale = 1f + MagicPlayer.chargeDamageBonus;
            drawScale *= MathHelper.Lerp(0.5f, 1f, Easing.InOutSine(_overchargeScaleTimer / 60f));
            drawScale = MathHelper.Clamp(drawScale, 0f, 2f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos + Projectile.velocity * 64, null, startGlow, Projectile.rotation, texture.Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = element.GetElementColor();
            glowColor.A = 0;
            glowColor *= progress;
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(texture2D4, centerPos + Projectile.velocity * 64, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f) * progress * VectorHelper.Osc(0.75f, 1f, speed: 3), SpriteEffects.None, 0f);
            }
        }

        private void DrawPixelatedMuzzleFlash(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
            Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
            Vector2 drawCenter = Owner.Center + Projectile.velocity * 64  - screenPos;
            Color drawColor = Element.GetElementColor();
            drawColor.A = 0;
            drawColor *= CrosshairProgress;
            float outEase = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Interpolant));
            drawColor *= outEase;

            float width = (float)Projectile.timeLeft / 30f;
            float outWidth = EasingFunction.InOutSine(width);
            float scale = outWidth;
            Vector2 flashScale = Vector2.One;
            flashScale.X *= 0.5f;
            flashScale.Y *= 2f * ExtraMath.Osc(0.5f, 1f, speed: 3f);
            flashScale *= scale;
            spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawAimingLines(ref lightColor);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedRings, DrawLayer.OverNPCs);
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMuzzleFlash, DrawLayer.OverNPCs);
            //     DrawPixelatedRings(Main.graphics.GraphicsDevice);
            DrawRingV2(ref lightColor);

            DrawStaff(ref lightColor);
            DrawEnergyBall(ref lightColor);
            DrawCrosshair(ref lightColor);
            return false;
        }

        private void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Color lightColor = Color.White;


        }
        private Core.Effects.GlowCircleShader _shader;
        public void DrawAimingLines(ref Color lightColor)
        {
            if (_hasFired)
                return;

            Item heldItem = Owner.HeldItem;
            if (heldItem.ModItem == null)
                return;
            Asset<Texture2D> heldTexture = ModContent.Request<Texture2D>("Stellamod/Core/Bases/CrossbowCrosshairLineParticle");
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 centerPos = Owner.Center - Main.screenPosition;
            _shader ??= new();
            var shader = _shader;
            shader.Speed = 5;

            float bp = 0.5f;
            shader.BasePower = bp;

            float s = 0.3f;
            shader.Size = s;

            Color startInner = Color.White;
            Color startGlow = Color.LightGoldenrodYellow;
            Color startOuterGlow = Color.Black;

            Color endColor = startOuterGlow;


            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;


            shader.InnerColor = Color.Lerp(shader.InnerColor, Color.Black, ChargeProgress);
            shader.GlowColor = Color.Lerp(shader.GlowColor, Color.Black, ChargeProgress);
            shader.OuterGlowColor = Color.Lerp(shader.OuterGlowColor, Color.Black, ChargeProgress);
            shader.Pixelation = 0.0005f;

            shader.ApplyToEffect();

            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);

            Vector2 aimLineOrigin = new Vector2(0, heldTexture.Size().Y / 2);
            Vector2 scale = Vector2.Lerp(new Vector2(0, 1), Vector2.One, ChargeProgress);
            float rotation = Projectile.velocity.ToRotation();
            spriteBatch.Draw(heldTexture.Value, Projectile.Center - Main.screenPosition, null, Color.White, rotation,
               aimLineOrigin, scale, SpriteEffects.None, 0);


            spriteBatch.RestartDefaults();

        }
        public void DrawCrosshair(ref Color lightColor)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Stellamod/Core/Bases/CrossbowCrosshair");
            Vector2 drawPos = Main.MouseWorld - Main.screenPosition;
            Vector2 drawOrigin = tex.Size() / 2f;
            float drawScale = 1f * CrosshairProgress * ExtraMath.Osc(0.95f, 1f, speed: 12);
            float drawRotation = Main.GlobalTimeWrappedHourly;
            Color drawColor = Color.Red * CrosshairProgress;
            drawColor *= 0.73f;
            drawColor.A = 0;
            spriteBatch.Draw(tex.Value, drawPos, null, drawColor , drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

    }
}
