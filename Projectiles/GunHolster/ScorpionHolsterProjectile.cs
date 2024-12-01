using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Common.ScorpionMountSystem;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class ScorpionHolsterProjectile : ModProjectile
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
                ScorpionPlayer scorpionPlayer = Owner.GetModPlayer<ScorpionPlayer>();
                return scorpionPlayer.miniGuns[ScorpionHolsterIndex];
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

        private float ShootTimer;
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

        private int ScorpionMountType = -1;
        private int ScorpionHolsterIndex
        {
            get => (int)Projectile.ai[2];
        }

        private bool IsRightHand
        {
            get
            {
                ScorpionPlayer scorpionPlayer = Owner.GetModPlayer<ScorpionPlayer>();
                return scorpionPlayer.onRight[ScorpionHolsterIndex];
            }
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
            Projectile.minionSlots = 1;
            Projectile.minion = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if(ScorpionMountType == -1)
            {
                ScorpionMountType = Owner.mount.Type;
            }

            if(Owner.mount.Type != ScorpionMountType || !Owner.mount.Active)
            {
                Projectile.Kill();
            }

            if (Owner.HeldItem.ModItem is not BaseScorpionItem)
            {
                Projectile.Kill();
            }

            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
            {
                Projectile.Kill();
            }
             
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
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 direction = Owner.Center.DirectionTo(Main.MouseWorld);
                HoldRotation = direction.ToRotation();
                Projectile.netUpdate = true;
            }

            Projectile.rotation = HoldRotation;
            bool mouseInput = Owner.controlUseItem;
            ShootTimer++;
            if(ShootTimer < 1 + (24 * ScorpionHolsterIndex))
            {
                mouseInput = false;
            }
            else
            {
                ShootTimer = 0f;
            }
            if (Projectile.owner == Main.myPlayer
                && mouseInput
                && Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId))
            {
                if (Projectile.spriteDirection == -1)
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

            SetGunPositionFromScorpion();
        }

        private void AI_Prepare()
        {
            Timer++;
            float endRotation = FireStartRotation;
            float progress = Timer / PrepTime;
            float easedProgress = Easing.OutCubic(progress);
            Projectile.rotation = MathHelper.Lerp(StartRotation, endRotation, easedProgress);

            SetGunPositionFromScorpion();
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
            SetGunPositionFromScorpion();

            if (MiniGun.ShootCount > 1)
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
                if (Timer == 1)
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
            SetGunPositionFromScorpion();

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

        public void DecideGunPosition()
        {
            SetGunPositionFromScorpion();
        }

        public void SetGunPositionFromScorpion()
        {
            if (MiniGun == null)
                return;
   
            ScorpionPlayer scorpionPlayer = Owner.GetModPlayer<ScorpionPlayer>();
            Vector2 holsterPosition = scorpionPlayer.holsterPositions[ScorpionHolsterIndex];
            Projectile.Center = holsterPosition;
            Projectile.Center += MiniGun.HolsterOffset.RotatedBy(Projectile.rotation);
            Projectile.position += new Vector2(-Recoil, 0).RotatedBy(Projectile.rotation);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.position += new Vector2(0, 12).RotatedBy(Projectile.rotation);
                Projectile.rotation -= MathHelper.ToRadians(180);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
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