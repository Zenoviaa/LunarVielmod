using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public class CrossbowHold : ModProjectile
    {
        public enum AIState
        {
            Take_Aim,
            Aim,
            Fire
        }

        //Texture assets that we need
        private Asset<Texture2D> _crosshairTextureAsset;
        private Asset<Texture2D> _bloomlineTextureAsset;


        protected Vector2 AimedDrawScale;
        protected Vector2 DrawScale;
        protected Vector2 HeldOffset;
        protected Vector2 DrawOriginOffset;
        protected Vector2 ArrowOffset;
        protected float AimTime;
        protected float FireTime;
        protected float GlowProgress;
        protected float AimProgress;
        protected float CrosshairProgress;
        protected float BurstCount;
        protected float ChargeStrength;
        protected ref float Timer => ref Projectile.ai[0];
        protected AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float UsingStamina => ref Projectile.ai[2];
        protected Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;
        public bool IsUsingStamina()
        {
            return UsingStamina > 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(DrawScale);
            writer.Write(GlowProgress);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            DrawScale = reader.ReadVector2();
            GlowProgress = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            HeldOffset = new Vector2(12, 0);
            DrawScale = Vector2.One;
            AimedDrawScale = new Vector2(1.2f, 1.2f);
            AimTime = 60;
            FireTime = 15;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Take_Aim:
                    AI_TakeAim();
                    break;
                case AIState.Aim:
                    AI_Aim();
                    break;
                case AIState.Fire:
                    AI_AimFire();
                    break;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
                Projectile.netUpdate = true;
            }


            GlowProgress *= 0.97f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            SetHeldPosition();
        }

        private void SwitchState(AIState state)
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }

        public virtual void AI_TakeAim()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle crossbowPull = AssetRegistry.Sounds.Bow.CrossbowPull;
                crossbowPull.PitchVariance = 0.4f;
                crossbowPull.Volume = 0.25f;
                SoundEngine.PlaySound(crossbowPull, Projectile.position);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Owner.Center.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
            }

            AimProgress = Timer / AimTime;
            CrosshairProgress = AimProgress;
            float easedScaleInProgress = EasingFunction.InOutCubic(AimProgress);
            DrawScale = Vector2.Lerp(Vector2.One, AimedDrawScale, easedScaleInProgress);
            ArrowOffset = Vector2.Lerp(new Vector2(24, 0), Vector2.Zero, easedScaleInProgress);
            if (Timer == AimTime)
            {
                //Flash Effect
                GlowProgress = 1f;
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightGray,
                        outerGlowColor: Color.Black);
                    particle.Scale *= 0.5f;
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            ChargeStrength = Timer / AimTime;
            if (Timer >= AimTime)
            {
                SwitchState(AIState.Aim);
            }

            if (Main.myPlayer == Projectile.owner && !Owner.controlUseItem && Timer >= 5 && !Main.mouseRight)
            {
                SwitchState(AIState.Fire);
            }
        }

        public virtual void AI_Aim()
        {
            Timer++;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
            }

            if (Main.myPlayer == Projectile.owner && !Owner.controlUseItem && !Main.mouseRight)
            {
                SwitchState(AIState.Fire);
            }
        }

        public virtual void AI_AimFire()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle aimSound = AssetRegistry.Sounds.Bow.Aim;
                aimSound.PitchVariance = 0.4f;
                aimSound.Volume = 0.25f;
                SoundEngine.PlaySound(aimSound, Projectile.position);
                if(Owner.HeldItem.ModItem is BaseCrossbowItem cb && Main.myPlayer == Projectile.owner)
                {
                    Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId);
                    ShootParams @params = new ShootParams
                    {
                        position = Owner.Center,
                        velocity = Projectile.velocity,
                        chargeStrength = ChargeStrength,
                        damage = damage,
                        knockBack = knockBack,
                        projToShoot = projToShoot,
                        speed = speed,
                        useAmmoItemId = usedAmmoItemId
                    };
                    var source = new Terraria.DataStructures.EntitySource_ItemUse_WithAmmo(Owner, Owner.HeldItem, usedAmmoItemId);
                 
                    if (IsUsingStamina())
                    {
                        cb.StaminaShootBow(Owner, source, @params);
                    } else
                    {
                        cb.ShootBow(Owner, source, @params);
                    }
            
                }
            }




            float scaleOutProgress = Timer / FireTime;
            float easedScaleOutProgress = EasingFunction.OutExpo(scaleOutProgress);
            DrawScale = Vector2.Lerp(AimedDrawScale, Vector2.One, easedScaleOutProgress);

            float originEasedProgress = EasingFunction.QuadraticBump(scaleOutProgress);
            DrawOriginOffset = Vector2.Lerp(Vector2.Zero, new Vector2(-8, 0), originEasedProgress).RotatedBy(Projectile.velocity.ToRotation());
            CrosshairProgress = 1f - scaleOutProgress;
            if (Timer >= FireTime * 1.5f)
            {
                Projectile.Kill();
            }
        }

        public virtual void Shoot(Vector2 position, Vector2 velocity)
        {

        }

        private void SetHeldPosition()
        {
            //This do be so goofy but it works so oh well
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= MathHelper.PiOver2;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            float handOffset = Projectile.spriteDirection == 1 ? 90 : 0;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(handOffset)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = Owner.MountedCenter;
            Projectile.Center += Projectile.velocity * 16;
            Owner.heldProj = Projectile.whoAmI;
        }

        public void DrawAimingLines(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(PixelatedAimingLineDraw, DrawLayer.OverNPCsWithOutline);
        }

        public void DrawCrosshair(ref Color lightColor)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            PixelationManager.QueueSpritebatchDrawAction(PixelatedCrosshairDraw, DrawLayer.OverNPCsWithOutline);
        }

        private void PixelatedAimingLineDraw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            _bloomlineTextureAsset ??= ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine");
            Vector2 centerPos = Owner.Center - Main.screenPosition;
            Vector2 aimLineOrigin = new Vector2(_bloomlineTextureAsset.Size().X / 2, 0);
            Vector2 scale = Vector2.Lerp(new Vector2(1, 0), Vector2.One, AimProgress);
            scale.X *= 0.2f;
            scale.Y *= 0.2f;
            float rotation = Projectile.velocity.ToRotation();
            rotation -= MathHelper.PiOver2;

            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(AimProgress));
            drawColor *= 0.15f;

            spriteBatch.Draw(_bloomlineTextureAsset.Value, Projectile.Center - Main.screenPosition, null, drawColor, rotation,
               aimLineOrigin, scale, SpriteEffects.None, 0);
        }

        private void PixelatedCrosshairDraw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            _crosshairTextureAsset ??= ModContent.Request<Texture2D>("Stellamod/Core/Bases/CrossbowCrosshair");
            Vector2 drawPos = Main.MouseWorld - Main.screenPosition;
            Vector2 drawOrigin = _crosshairTextureAsset.Size() / 2f;
            float drawScale = 1f * CrosshairProgress * ExtraMath.Osc(0.95f, 1f, speed: 12);
            float drawRotation = Main.GlobalTimeWrappedHourly;
            Color drawColor = Color.Red * CrosshairProgress;

            spriteBatch.Draw(_crosshairTextureAsset.Value, drawPos, null, drawColor * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        public virtual void DrawSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Item heldItem = Owner.HeldItem;
            if (heldItem.ModItem == null)
                return;

            Asset<Texture2D> heldTexture = ModContent.Request<Texture2D>(heldItem.ModItem.Texture);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos += DrawOriginOffset;

            Vector2 drawOrigin = heldTexture.Size() / 2f;
            //  drawPos += DrawOriginOffset;
            SpriteEffects spriteEffects = SpriteEffects.None;
            float drawRotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
                drawRotation += MathHelper.ToRadians(90);
            if (Owner.direction == -1)
                spriteEffects |= SpriteEffects.FlipVertically;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawScale = DrawScale;

            spriteBatch.Draw(heldTexture.Value, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
        }

        public void DrawArrow(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId, dontConsume: true);
            Main.instance.LoadProjectile(projToShoot);
            Asset<Texture2D> arrowTexture = TextureAssets.Projectile[projToShoot];
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = arrowTexture.Size() / 2f;
            drawPos += DrawOriginOffset;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float drawRotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            if (Projectile.spriteDirection == -1)
            {
                drawRotation += MathHelper.ToRadians(180);
            }
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawScale = DrawScale;
            spriteBatch.Draw(arrowTexture.Value, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);

        }
        public virtual void DrawSpriteFlash(ref Color lightColor)
        {
            //Don't even need to run the code lol
            Item heldItem = Owner.HeldItem;
            if (GlowProgress <= 0)
                return;
            if (heldItem.ModItem == null)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Asset<Texture2D> heldTexture = ModContent.Request<Texture2D>(heldItem.ModItem.Texture);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = heldTexture.Size() / 2f;
            drawPos += DrawOriginOffset;

            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawScale = DrawScale;
            SpriteEffects spriteEffects = SpriteEffects.None;
            float drawRotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
                drawRotation += MathHelper.ToRadians(90);
            if (Owner.direction == -1)
                spriteEffects |= SpriteEffects.FlipVertically;
            Color glowColor = drawColor * GlowProgress;
            glowColor.A = 0;
            spriteBatch.Draw(heldTexture.Value, drawPos, null, glowColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawAimingLines(ref lightColor);
            DrawSprite(ref lightColor);
            if (State == AIState.Take_Aim || State == AIState.Aim)
                DrawArrow(ref lightColor);
            DrawSpriteFlash(ref lightColor);
            DrawCrosshair(ref lightColor);
            return false;
        }
    }
}
