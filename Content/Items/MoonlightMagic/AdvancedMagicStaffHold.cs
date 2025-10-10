using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class AdvancedMagicStaffHold : ModProjectile
    {
        private enum AIState
        {
            Charge,
            Release
        }
        private AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private int _ringFrame;
        private int _targetRingFrame;
        private float _ringTransitionTimer;
        private Vector2 _ringScale;
        private float _ringAlpha;
        private float MaxChargeTime => 60;

        private ref float Timer => ref Projectile.ai[1];
        private ref float ChargeProgress => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 EndPoint => Projectile.Center + -Vector2.UnitY * 40;
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

        public override void AI()
        {
            base.AI();
            _ringTransitionTimer++;
            if(_ringTransitionTimer >= 15)
            {
                _ringFrame = _targetRingFrame;
            }
            _ringScale = Vector2.Lerp(Vector2.One, Vector2.One * 0.75f, EasingFunction.QuadraticBump(_ringTransitionTimer / 30f));
            _ringAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.QuadraticBump(_ringTransitionTimer / 30f));
            
            switch (State)
            {
                case AIState.Charge:
                    AI_Charge();
                    break;
                case AIState.Release:
                    AI_Release();
                    break;
            }


            Owner.heldProj = Projectile.whoAmI;
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
            float drawRotation = -Vector2.UnitY.ToRotation() + MathHelper.PiOver4 * Owner.direction;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,
               drawRotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full,
             drawRotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            Owner.heldProj = Projectile.whoAmI;
            if (Projectile.spriteDirection == -1)
            {
                // Projectile.rotation += MathHelper.ToRadians(90);
            }


        }

        private void AI_Charge()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle mySound = new SoundStyle("Stellamod/Assets/Sounds/StormKnight_Rechage");
                mySound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mySound, Projectile.position);

            }
            if (Main.myPlayer == Projectile.owner)
            {

                Projectile.velocity = Owner.Center.DirectionTo(Main.MouseWorld);
                Projectile.netUpdate = true;
            }
            if (Timer == MaxChargeTime)
            {
                for (float f = 0; f < 7; f++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Dust.NewDustPerfect(EndPoint, ModContent.DustType<GlowSparkleDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
                    }
                    else
                    {
                        Dust.NewDustPerfect(EndPoint, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 0.4f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
                    }
                }
            }
            else if (Timer < MaxChargeTime)
            {
                if (Timer % 5 == 0)
                {
                    Vector2 spawnPos = EndPoint + Main.rand.NextVector2CircularEdge(64, 64);
                    Vector2 vel = (EndPoint - spawnPos).SafeNormalize(Vector2.Zero) * 4;
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlyphDust>(), vel, newColor: Color.White, Scale: Main.rand.NextFloat(0.25f, 1f));
                }
            }
            ChargeProgress = Timer / MaxChargeTime;
            ChargeProgress = MathHelper.Clamp(ChargeProgress, 0, 1);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!Owner.channel)
                {
                    SwitchState(AIState.Release);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Lighting.AddLight((Projectile.Center + Projectile.velocity * 64), Color.LightCyan.ToVector3() * 1.5f);
            SetHoldPosition();
        }

        private void AI_Release()
        {
            Timer++;
            if (Timer == 1)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    int damage = (int)MathHelper.Lerp(0, Projectile.damage, ChargeProgress);
                    float knockback = Projectile.knockBack;
                    Vector2 fireVelocity = Projectile.velocity * 15;
                    BaseStaff staff = Owner.HeldItem.ModItem as BaseStaff;
                    AdvancedMagicUtil.NewMagicProjectile(staff, Owner, new Terraria.DataStructures.EntitySource_ItemUse_WithAmmo(Owner, staff.Item, -1), Owner.Center, fireVelocity,
                        ModContent.ProjectileType<AdvancedMagicStaffProjectile>(), damage, knockback);
                }
            }
            if (Timer >= 4)
            {
                Projectile.Kill();
            }
            SetHoldPosition();
        }

        private void SwitchRingFrame(int frameNumber)
        {
            _targetRingFrame = frameNumber;
            _ringTransitionTimer = 0;
        }
        private void DrawRing(ref Color lightColor)
        {
            BaseStaff staff = Owner.HeldItem.ModItem as BaseStaff;
            Item elementItem = staff.GetElement();
            BaseElement element = elementItem.ModItem as BaseElement;
            if (element == null)
                return;

            Texture2D ringTexture = element.GetRingTexture();
            if (ringTexture == null)
                return;
            Vector2 drawPos = Owner.Center - Main.screenPosition;
            int frameNumber = (int)MathHelper.Lerp(0, 2, ChargeProgress);
            if(_targetRingFrame != frameNumber)
            {
                SwitchRingFrame(frameNumber);
            }
            Rectangle frame = ringTexture.GetFrame(_ringFrame, 3);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = new Color(255, 255, 255, 0) * 0.2f * _ringAlpha;
            float drawRotation = Timer * 0.01f;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawScale = new Vector2(1f, 1f) * _ringScale;

            spriteBatch.Draw(ringTexture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private void DrawMagicTrailRing(ref Color lightColor)
        {
            BaseStaff staff = Owner.HeldItem.ModItem as BaseStaff;
            Item elementItem = staff.GetElement();
            BaseElement element = elementItem.ModItem as BaseElement;
            if (element == null)
                return;


            List<Vector2> trailPoints = new List<Vector2>();
            List<float> trailRot = new List<float>();
            float xRadius = MathHelper.Lerp(64, 80, ChargeProgress);
            float yRadius = MathHelper.Lerp(16, 18, ChargeProgress);
            for(int i = 0; i < 48; i++)
            {
                float rads = Timer * 0.15f + i * 0.05f;
                rads += MathHelper.PiOver2;
                float xOffset = xRadius * MathF.Sin(rads);
                float yOffset = yRadius * MathF.Cos(rads);
                Vector2 point = Owner.Center + new Vector2(xOffset, yOffset);
                trailPoints.Add(point);
                trailRot.Add(0);
            }
            element.DrawRingTrail(trailPoints.ToArray(), trailRot.ToArray(), Vector2.Zero);

        }
        private void DrawStaff(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = -Vector2.UnitY.ToRotation() + MathHelper.PiOver4;
            float drawScale = 1f;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos + -Vector2.UnitY * 24, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, drawPos + -Vector2.UnitY * 32, null, new Color(255, 128, 125, 0) * ChargeProgress * 0.5f, drawRotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            DrawRing(ref lightColor);
            DrawMagicTrailRing(ref lightColor);
            DrawStaff(ref lightColor);
            return false;
        }
    }
}
