using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.GunSystem
{
    public class ReloadBar : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        private GunHoldPlayer GunHoldPlayer => Owner.GetModPlayer<GunHoldPlayer>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle gunTossSound = AssetRegistry.Sounds.Gun.GunToss;
                gunTossSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(gunTossSound);
            }
            if (GunHoldPlayer.isReloading)
            {
                Projectile.timeLeft = 2;
            }
            Projectile.Center = Owner.Center + new Vector2(0, 64);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D reloadBar = ModContent.Request<Texture2D>(Texture).Value;


            Texture2D reloadHandle = ModContent.Request<Texture2D>(Texture + "_Handle").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = reloadBar.Size() / 2f;


            float width = reloadBar.Width;
            float offset = MathHelper.Lerp(-width / 2f, width / 2f, GunHoldPlayer.reloadRatio);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(effect: SpriteWhiteShader.Instance.Effect);
            spriteBatch.Draw(reloadBar, drawPos - Vector2.UnitX * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadBar, drawPos + Vector2.UnitX * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadBar, drawPos - Vector2.UnitY * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadBar, drawPos + Vector2.UnitY * 2, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);



            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) - Vector2.UnitX * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) + Vector2.UnitX * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) - Vector2.UnitY * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0) + Vector2.UnitY * 2, null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(reloadBar, drawPos, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Vector2 drawScale = Vector2.One;
            drawScale.X = GunHoldPlayer.marginOfError / GunHoldPlayer.reloadTime;
            spriteBatch.Draw(reloadBar, drawPos, null, Color.Green, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);

    
            spriteBatch.Draw(reloadHandle, drawPos + new Vector2(offset, 0), null, Color.White, Projectile.rotation, reloadHandle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public abstract class BaseGun : ModItem
    {
        public int remainingAmmo = 6;
        public int maxAmmo = 6;
        public float reloadWindow = 30;
        public override void SetDefaults()
        {
            base.SetDefaults();
            remainingAmmo = 6;
            maxAmmo = 6;
            reloadWindow = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Bullet;
            Item.noUseGraphic = true;
        }

        public override bool CanShoot(Player player)
        {
            GunHoldPlayer gunHoldPlayer = player.GetModPlayer<GunHoldPlayer>();
            return remainingAmmo > 0 && !player.HasBuff<Reloading>() && gunHoldPlayer.reloadFireDelay <= 0;
        }

        public bool NeedsReloading()
        {
            return remainingAmmo <= 0;
        }

        public virtual void Reload()
        {
            remainingAmmo = maxAmmo;
            SoundStyle gunReloadSound = AssetRegistry.Sounds.Gun.GunReload;
            gunReloadSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(gunReloadSound);
        }

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (remainingAmmo > 0)
            {
                remainingAmmo--;
                return GunShot(player, source, position, velocity, type, damage, knockback);
            }
            else
            {
                return false;
            }
        }

        public virtual bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 1; i++)
            {
                Gore.NewGore(player.GetSource_FromThis(), position, velocity * -1,
                    ModContent.GoreType<BulletCasing>());
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 muzzlePosition = player.MountedCenter + velocity.SafeNormalize(Vector2.Zero) * texture.Width / 2;
            ShootEffects(muzzlePosition, velocity);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public virtual void ShootEffects(Vector2 position, Vector2 velocity)
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.5f;
            SoundEngine.PlaySound(shootSound, position);

            FXUtil.GlowCircleBoom(position, Color.White, Color.Yellow, Color.Red, baseSize: 0.03f, duration: 15);

            for (float f = 0; f < 3; f++)
            {
                float rot = f / 8f;
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                var p = Particle.NewParticle<ImpactParticle>(position, velocity.RotatedByRandom(0.7f));
                p.fast = true;
            }
            for (float f = 0; f < 3; f++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), velocity.RotatedByRandom(0.3f), Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }
    }
    public class Reloading : ModBuff
    {

    }
    public class GunHoldPlayer : ModPlayer
    {
        public bool isReloading;
        public float reloadTimer;
        public float reloadTime;
        public float marginOfError;
        public float reloadFireDelay;

        public bool doCoolReloadAnimation;
        
        public float reloadRatio => reloadTimer / reloadTime;
        public BaseGun HeldGun => Player.HeldItem.ModItem as BaseGun;
        public override void ResetEffects()
        {
            base.ResetEffects();
            isReloading = false;
          
            marginOfError = 10;
            var heldGun = HeldGun;
            if (heldGun == null)
                reloadTime = 60;
            else
                reloadTime = heldGun.reloadWindow;
        }

        public bool TimedReload()
        {
            if (!Main.mouseLeft)
                return false;
            if (!Main.mouseLeftRelease)
                return false;

            float center = reloadTime / 2f;
            float diff = MathF.Abs(center - reloadTimer);
            bool hasTimed = diff <= marginOfError;
            if (!hasTimed)
            {
                SoundStyle jamSound = AssetRegistry.Sounds.Gun.GunJam;
                jamSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(jamSound, Player.position);
                return false;
            }
            return true;
  
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if(reloadFireDelay > 0)
                reloadFireDelay--;

            var heldGun = HeldGun;
            if (heldGun == null)
            {
                reloadTimer = 0;
                return;
            }

            if (Main.myPlayer == Player.whoAmI &&
                Player.ownedProjectileCounts[ModContent.ProjectileType<GunHold>()] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<GunHold>(), 1, 1, Player.whoAmI);
            }
            if (heldGun.NeedsReloading())
            {
                Player.AddBuff(ModContent.BuffType<Reloading>(), 2);
                isReloading = true;
                reloadTimer++;
                if (reloadTimer >= reloadTime)
                {
                    reloadTimer = 0;
                }

                Main.player
                if (Main.myPlayer == Player.whoAmI)
                {
                    if (TimedReload())
                    {
                        heldGun.Reload();
                        reloadFireDelay = 60;
                        doCoolReloadAnimation = true;
                    }
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<ReloadBar>()] == 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                            ModContent.ProjectileType<ReloadBar>(), 1, 1, Player.whoAmI);
                    }
                }

            }
            else
            {
                reloadTimer = 0;
            }

        }
    }

    public class GunHold : ModProjectile
    {
        private float _startRotation;
        public override string Texture => TextureRegistry.EmptyTexture;
        private enum AIState
        {
            Hold,
            Shoot,
            Reload
        }

        private ref float Timer => ref Projectile.ai[0];
        private ref float HoldRotation => ref Projectile.ai[1];

        private AIState State
        {
            get => (AIState)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        private Player Owner => Main.player[Projectile.owner];
        private GunHoldPlayer GunHoldPlayer => Owner.GetModPlayer<GunHoldPlayer>(); 
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overPlayers.Add(index);
        }
        public override void AI()
        {
            base.AI();
            if (Owner.HeldItem.ModItem is BaseGun)
            {
                Projectile.timeLeft = 2;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 mousePos = Main.MouseWorld;
                Vector2 rotationVector = mousePos - Owner.Center;
                HoldRotation = rotationVector.ToRotation();
                Projectile.netUpdate = true;

            }
            Projectile.Center = Owner.MountedCenter - new Vector2(0, 7);
            Projectile.rotation = HoldRotation;

            if (State != AIState.Reload && GunHoldPlayer.doCoolReloadAnimation)
            {
                SwitchState(AIState.Reload);
                GunHoldPlayer.doCoolReloadAnimation = false;
            }

            switch (State)
            {
                case AIState.Hold:
                    AI_Hold();
                    break;
                case AIState.Shoot:
                    AI_Shoot();
                    break;
                case AIState.Reload:
                    AI_Reload();
                    break;
            }
            AI_OrientHand();
        }

        private void SwitchState(AIState state)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }

        }
        private void AI_OrientHand()
        {

            float rotation = Projectile.rotation;
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Owner.direction;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            Owner.itemRotation = rotation * Owner.direction;
            //Owner.itemTime = 2;
            //Owner.itemAnimation = 2;

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90));// set arm position (90 degree offset since arm starts lowered)
        }

        private void AI_Reload()
        {
            Timer++;
            if(Timer == 1)
            {
                _startRotation = Projectile.rotation;
            }
            float interp = Timer / 60f;
            float ease = EasingFunction.InOutExpo7(interp);
            Projectile.rotation = MathHelper.Lerp(_startRotation, _startRotation + MathHelper.TwoPi * 2, ease);
            if(Timer >= 60f)
            {
                SwitchState(AIState.Hold);
            }
        }
        private void AI_Hold()
        {

        }
        private void AI_Shoot()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
