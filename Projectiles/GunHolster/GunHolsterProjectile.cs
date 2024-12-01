using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class GunHolsterProjectile : ModProjectile
    {
        private enum ActionState
        {
            Holster,
            Prepare,
            Fire,
            Hide
        }

        public MiniGun MiniGun
        {
            get
            {
                GunPlayer gunPlayer = Owner.GetModPlayer<GunPlayer>();
                if (IsRightHand)
                {
                    return gunPlayer.HeldRightHandGun;
                }
                else
                {
                    return gunPlayer.HeldLeftHandGun;
                }
            }
        }
        public Texture2D HeldTexture
        {
            get => ModContent.Request<Texture2D>(MiniGun.HeldTexture).Value;
        }

        public override string Texture => TextureRegistry.EmptyTexture;
        private float PrepTime => 4 / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float ExecTime => MiniGun.AttackSpeed / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float HideTime => MiniGun.AttackSpeed / Owner.GetTotalAttackSpeed(Projectile.DamageType) / 2;
        private Player Owner => Main.player[Projectile.owner];

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private ActionState State
        {
            get => (ActionState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private bool IsRightHand
        {
            get => Projectile.ai[2] == 1;
        }

        private float IdleRotation
        {
            get
            {
                Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                float rotation = direction.ToRotation();
                return rotation;
            }
        }

        private float StartRotation;
        private float FireStartRotation;
        private float HideStartRotation;
        private float SpriteDirection;
        private float HoldRotation;
        private float Recoil;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StartRotation);
            writer.Write(FireStartRotation);
            writer.Write(HideStartRotation);
            writer.Write(SpriteDirection);
            writer.Write(HoldRotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StartRotation = reader.ReadSingle();
            FireStartRotation = reader.ReadSingle();
            HideStartRotation = reader.ReadSingle();
            SpriteDirection = reader.ReadSingle();
            HoldRotation = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (!Owner.HasBuff(ModContent.BuffType<MarksMan>()))
                Projectile.Kill();

            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();
            if (MiniGun == null)
            {
                Projectile.Kill();
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                SpriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
                Projectile.netUpdate = true;
            }

            Projectile.spriteDirection = (int)SpriteDirection;
            switch (State)
            {
                case ActionState.Holster:
                    AI_Holster();
                    break;
                case ActionState.Prepare:
                    AI_Prepare();
                    break;
                case ActionState.Fire:
                    AI_Fire();
                    break;
                case ActionState.Hide:
                    AI_Hide();
                    break;
            }
        }

        private void AI_Holster()
        {
            if(Projectile.owner == Main.myPlayer)
            {
                Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                HoldRotation = direction.ToRotation();
                Projectile.netUpdate = true;
            }
 
            Projectile.rotation = HoldRotation;
            bool mouseInput = IsRightHand ? Main.mouseRight : Main.mouseLeft;
            if (Projectile.owner == Main.myPlayer 
                && mouseInput 
                && Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId))
            {
                if(Projectile.spriteDirection == -1)
                {
                    StartRotation = Projectile.rotation - MiniGun.RecoilRotationMini;
                    FireStartRotation = StartRotation + MiniGun.RecoilRotationMini;
                    HideStartRotation = StartRotation + MiniGun.RecoilRotation;
                }
                else
                {
                    StartRotation = Projectile.rotation + MiniGun.RecoilRotationMini;
                    FireStartRotation = StartRotation - MiniGun.RecoilRotationMini;
                    HideStartRotation = StartRotation - MiniGun.RecoilRotation;
                }

                Projectile.netUpdate = true;
                State = ActionState.Prepare;
                Timer = 0;
            }

            SetGunPosition();    
        }

        private void AI_Prepare()
        {
            Timer++;
            float endRotation = FireStartRotation;
            float progress = Timer / PrepTime;
            float easedProgress = Easing.OutCubic(progress);
            Projectile.rotation = MathHelper.Lerp(StartRotation, endRotation, easedProgress);

            SetGunPosition();
            if (Timer >= PrepTime)
            {
                State = ActionState.Fire;
                Timer = 0;
            }
        }

        private void AI_Fire()
        {
            Timer++;
            float endRotation = HideStartRotation;
            float progress = Timer / ExecTime;
            float easedProgress = Easing.OutExpo(progress);

            Projectile.rotation = MathHelper.Lerp(StartRotation, endRotation, easedProgress);
            SetGunPosition();

            if(MiniGun.ShootCount > 1)
            {
                float shootTime = ExecTime / (float)MiniGun.ShootCount;
                if (Timer % shootTime == 0)
                {
                    Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                    Vector2 position = Projectile.Center + direction * Projectile.width / 2;
                    MiniGun.Fire(Owner, position, direction, Projectile.damage, Projectile.knockBack);
                } 
            }
            else
            {
                if(Timer == 1)
                {
                    Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                    Vector2 position = Projectile.Center + direction * Projectile.width / 2;
                    MiniGun.Fire(Owner, position, direction, Projectile.damage, Projectile.knockBack);
                }  
            }

            Recoil = MathHelper.Lerp(0, MiniGun.RecoilDistance, Easing.SpikeInOutExpo(progress));
            if (Timer >= ExecTime)
            {
                State = ActionState.Hide;
                Timer = 0;
            }
        }

        private void AI_Hide()
        {
            Timer++;
            float progress = Timer / HideTime;
            float easedProgress = Easing.OutCubic(progress);
            Projectile.rotation = MathHelper.Lerp(FireStartRotation, IdleRotation, easedProgress);
            SetGunPosition();

            if (Timer >= HideTime)
            {
                State = ActionState.Holster;
                Timer = 0;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public void SetGunPosition()
        {
            if (MiniGun == null)
                return;

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            if (IsRightHand)
            {
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = Owner.GetBackHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += Owner.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += MiniGun.HolsterOffset.RotatedBy(Projectile.rotation);
                Projectile.position -= new Vector2(0, 4);
            }
            else
            {
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += Owner.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += MiniGun.HolsterOffset.RotatedBy(Projectile.rotation);
            }

            Projectile.position += new Vector2(-Recoil, 0).RotatedBy(Projectile.rotation);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.position += new Vector2(0, 12).RotatedBy(Projectile.rotation);
                Projectile.rotation -= MathHelper.ToRadians(180);
            }
        
            if(Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }
          
            if (!IsRightHand)
            {
                Owner.heldProj = Projectile.whoAmI;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = HeldTexture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = 1f;
            float drawRotation = Projectile.rotation;
            SpriteEffects spriteEffects = SpriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(HeldTexture, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            return false;
        }
    }
}
