using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Systems.MiscellaneousMath;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseCrossbowProjectile : ModProjectile
    {
        internal enum AIState
        {
            Take_Aim,
            Aim,
            Fire
        }


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
        protected ref float Timer => ref Projectile.ai[0];
        protected AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        protected Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
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
                SoundStyle crossbowPull = new SoundStyle("Stellamod/Assets/Sounds/CrossbowPull");
                crossbowPull.PitchVariance = 0.1f;
                SoundEngine.PlaySound(crossbowPull, Projectile.position);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Owner.Center.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
            }

            AimProgress = Timer / AimTime;
            CrosshairProgress = AimProgress;
            float easedScaleInProgress = Easing.InOutCubic(AimProgress);
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
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }

            if (Timer >= AimTime)
            {
                SwitchState(AIState.Aim);
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

            if (Main.myPlayer == Projectile.owner && Owner.controlUseItem)
            {
                SwitchState(AIState.Fire);
            }
        }

        public virtual void AI_AimFire()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/MorrowSalfi"), Projectile.position);

            }

            if (BurstCount > 1)
            {
                int fireDivisor = (int)(FireTime / BurstCount);
                if (Timer % fireDivisor == 0)
                {
                    Shoot(Owner.Center, Projectile.velocity);
                }
            }
            else
            {
                if (Timer == 1)
                {
                    Shoot(Owner.Center, Projectile.velocity);
                }
            }


            float scaleOutProgress = Timer / FireTime;
            float easedScaleOutProgress = Easing.OutExpo(scaleOutProgress);
            DrawScale = Vector2.Lerp(AimedDrawScale, Vector2.One, easedScaleOutProgress);

            float originEasedProgress = Easing.SpikeOutBounce(scaleOutProgress);
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
                Projectile.rotation -= MathHelper.ToRadians(90);
            }


            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            float handOffset = 45;
            if (Projectile.spriteDirection == 1)
            {
                handOffset = 135;
            }
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(handOffset)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position

            float or = Projectile.spriteDirection == -1 ? MathHelper.ToRadians(90) : MathHelper.ToRadians(-90);
            Projectile.Center += HeldOffset.RotatedBy(Projectile.rotation + or);

            Owner.heldProj = Projectile.whoAmI;
        }

        public void DrawAimingLines(ref Color lightColor)
        {
            Item heldItem = Owner.HeldItem;
            if (heldItem.ModItem == null)
                return;
            Asset<Texture2D> heldTexture = ModContent.Request<Texture2D>("Stellamod/Common/Bases/CrossbowCrosshairLineParticle");
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 centerPos = Owner.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;
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


            shader.InnerColor = Color.Lerp(shader.InnerColor, Color.Black, AimProgress);
            shader.GlowColor = Color.Lerp(shader.GlowColor, Color.Black, AimProgress);
            shader.OuterGlowColor = Color.Lerp(shader.OuterGlowColor, Color.Black, AimProgress);
            shader.Pixelation = 0.0005f;

            shader.Apply();

            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);

            Vector2 aimLineOrigin = new Vector2(0, heldTexture.Size().Y / 2);
            for (int i = 0; i < 3; i++)
            {
                float off = 64;
                float rot = Projectile.velocity.ToRotation();
                rot += MathHelper.ToRadians(45 * Projectile.spriteDirection);

                float aimlineRot = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), AimProgress);
                Vector2 upPos = centerPos + rot.ToRotationVector2() * off * (1f - AimProgress);
                upPos = Vector2.Lerp(upPos, centerPos, AimProgress);
                upPos += Projectile.velocity * -off;
                spriteBatch.Draw(heldTexture.Value, upPos, null, Color.White, aimlineRot,
                   aimLineOrigin, 1f, SpriteEffects.None, 0);


                float aimlineRot2 = MathHelper.Lerp(Projectile.rotation - MathHelper.ToRadians(90 * Projectile.spriteDirection), Projectile.velocity.ToRotation(), AimProgress);
                upPos = centerPos + (rot - MathHelper.ToRadians(90 * Projectile.spriteDirection)).ToRotationVector2() * off * (1f - AimProgress);
                upPos = Vector2.Lerp(upPos, centerPos, AimProgress);
                upPos += Projectile.velocity * -off;
                spriteBatch.Draw(heldTexture.Value, upPos, null, Color.White, aimlineRot2,
      aimLineOrigin, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();

        }

        public void DrawCrosshair(ref Color lightColor)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("Stellamod/Common/Bases/CrossbowCrosshair");
            Vector2 drawPos = Main.MouseWorld - Main.screenPosition;
            Vector2 drawOrigin = tex.Size() / 2f;
            float drawScale = 1f * CrosshairProgress * MathUtil.Osc(0.95f, 1f, speed: 12);
            float drawRotation = Main.GlobalTimeWrappedHourly;
            Color drawColor = Color.Red * CrosshairProgress;

            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < 1; i++)
            {
                spriteBatch.Draw(tex.Value, drawPos, null, drawColor * 0.73f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();

        }

        public virtual void DrawSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Item heldItem = Owner.HeldItem;
            if (heldItem.ModItem == null)
                return;

            Asset<Texture2D> heldTexture = ModContent.Request<Texture2D>(heldItem.ModItem.Texture);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = heldTexture.Size() / 2f;
            drawPos += DrawOriginOffset;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
            {
                drawOrigin.Y = heldTexture.Size().Y - drawOrigin.Y;
            }
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                drawOrigin.X = heldTexture.Size().X - drawOrigin.X;
            }
            float drawRotation = Projectile.rotation;
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
            float drawRotation = Projectile.rotation + MathHelper.ToRadians(45);
            if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
            {
                drawOrigin.Y = arrowTexture.Size().Y - drawOrigin.Y;
                drawRotation -= MathHelper.ToRadians(90);
                drawPos += ArrowOffset.RotatedBy(Projectile.rotation + MathHelper.ToRadians(45));
            }
            else
            {
                drawPos += ArrowOffset.RotatedBy(Projectile.rotation - MathHelper.ToRadians(45));
            }
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                drawOrigin.X = arrowTexture.Size().X - drawOrigin.X;

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

            float drawRotation = Projectile.rotation;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawScale = DrawScale;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
            {
                drawOrigin.Y = heldTexture.Size().Y - drawOrigin.Y;
            }
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                drawOrigin.X = heldTexture.Size().X - drawOrigin.X;
            }
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < 3; i++)
            {
                Color glowColor = drawColor * GlowProgress;
                spriteBatch.Draw(heldTexture.Value, drawPos, null, glowColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            }

            spriteBatch.RestartDefaults();
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
