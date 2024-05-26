using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal abstract class GunHolsterProjectile : ModProjectile
    {
        private enum ActionState
        {
            Holster,
            Prepare,
            Fire,
            Hide
        }

        protected float AttackSpeed = 10;
        protected Vector2 HolsterOffset;
        protected bool IsRightHand;

        protected float RecoilRotation = MathHelper.PiOver4;
        protected float RecoilDistance = 5;
        protected float RecoilRotationMini = MathHelper.ToRadians(15);
        protected float ShootCount;


        private float PrepTime => 4 / Owner.GetTotalAttackSpeed(DamageClass.Ranged);
        private float ExecTime => AttackSpeed / Owner.GetTotalAttackSpeed(DamageClass.Ranged);
        private float HideTime => AttackSpeed / Owner.GetTotalAttackSpeed(DamageClass.Ranged) / 2;
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

  

            if(Main.myPlayer == Projectile.owner)
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

        protected abstract void Shoot(Vector2 position, Vector2 direction);

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
                    StartRotation = Projectile.rotation - RecoilRotationMini;
                    FireStartRotation = StartRotation + RecoilRotationMini;
                    HideStartRotation = StartRotation + RecoilRotation;
                }
                else
                {
                    StartRotation = Projectile.rotation + RecoilRotationMini;
                    FireStartRotation = StartRotation - RecoilRotationMini;
                    HideStartRotation = StartRotation - RecoilRotation;
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

            if(ShootCount > 1)
            {
                float shootTime = ExecTime / (float)ShootCount;
                if (Timer % shootTime == 0)
                {
                    Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                    Vector2 position = Projectile.Center + direction * Projectile.width / 2;
                    Shoot(position, direction);
                } 
            }
            else
            {
                if(Timer == 1)
                {
                    Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                    Vector2 position = Projectile.Center + direction * Projectile.width / 2;
                    Shoot(position, direction);
                }
  
            }


            Recoil = MathHelper.Lerp(0, RecoilDistance, Easing.SpikeInOutExpo(progress));
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
            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            if (IsRightHand)
            {
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = Owner.GetBackHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += Owner.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += HolsterOffset.RotatedBy(Projectile.rotation);
                Projectile.position -= new Vector2(0, 4);
            }
            else
            {
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += Owner.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += HolsterOffset.RotatedBy(Projectile.rotation);
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
    }
}
