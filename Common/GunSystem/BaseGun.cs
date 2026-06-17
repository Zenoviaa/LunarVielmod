using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellamod.Assets;
using Stellamod.Buffs;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Effects.Generic;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Common.GunSystem
{
    public class ReloadBar : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float FailTimer => ref Projectile.ai[1];
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
            if (GunHoldPlayer.doFailAnimation)
            {
                FailTimer = 30;
                GunHoldPlayer.doFailAnimation = false;
            }

            Vector2 offset = Vector2.Zero;
            if(FailTimer > 0)
            {
                offset = Main.rand.NextVector2Circular(16, 16);
                FailTimer--;
            }
            Projectile.Center = Owner.Center + new Vector2(0, 64) + offset;
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
    public struct GunReloadParams
    {
        public int maxAmmo;
        public float reloadWindow;
        public GunReloadParams()
        {
            maxAmmo = 6;
            reloadWindow = 30;
        }
    }

    public abstract class BaseGun : ModItem
    {
        public int remainingAmmo = 6;
        public Vector2 muzzleOrigin;
        public override void SetDefaults()
        {
            base.SetDefaults();
            GunReloadParams reloadParams = new GunReloadParams();
            SetMagazine(ref reloadParams);
            remainingAmmo = reloadParams.maxAmmo;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Bullet;
            Item.noUseGraphic = true;
        }

        public virtual bool UseDefaultHoldAnimation() => true;

        /// <summary>
        /// Set the max ammo and reload window counts for this weapon
        /// If none is set, defaults to 6 max ammo and 30 reload window
        /// </summary>
        /// <param name="fireParams"></param>
        public virtual void SetMagazine(ref GunReloadParams fireParams)
        {

        }


        public virtual bool UseHeatShader() => true;
        public virtual void ModifyMuzzleFlashColors(ref Color hottestColor, ref Color coldestColor)
        {
            hottestColor = Color.Yellow;
            coldestColor = Color.DarkRed;
        }
        public int GetMaxAmmo(Player player)
        {
            //We can use local player here can reloading is never checked over clients, I think
            //ehh
            GunReloadParams reloadParams = new GunReloadParams();
            SetMagazine(ref reloadParams);
            float maxAmmo = reloadParams.maxAmmo + player.GetModPlayer<ArmorStatsPlayer>().rangedGunAmmoAmount;
            maxAmmo *= 1.0f + player.GetModPlayer<ArmorStatsPlayer>().rangedGunAmmoAmountPct;
            return (int)maxAmmo;
        }

        public float GetReloadWindow()
        {
            GunReloadParams reloadParams = new GunReloadParams();
            SetMagazine(ref reloadParams);
            return reloadParams.reloadWindow;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "AmmoCapacity", LangText.Common("MagazineHelp", GetMaxAmmo(Main.LocalPlayer)))
            {
                OverrideColor = Color.White
            };
            tooltips.Add(line);
        }

        public override bool CanUseItem(Player player)
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
            remainingAmmo = GetMaxAmmo(Main.LocalPlayer);
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

        public virtual bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public virtual void GunCasingEffects(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 1; i++)
            {
                Vector2 vel = velocity.SafeNormalize(Vector2.Zero);
                vel *= -4;
                vel.Y -= 4;
                Gore.NewGore(player.GetSource_FromThis(), position, vel,
                    ModContent.GoreType<BulletCasing>());
            }
        }

        public Vector2 GetMuzzlePosition(Player player, Vector2 velocity)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Vector2? holdOutOffset = HoldoutOffset();
            Vector2 offset = holdOutOffset.HasValue ? holdOutOffset.Value : Vector2.Zero;
            offset = offset.RotatedBy(velocity.ToRotation());

            SpriteEffects spriteEffects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (player.direction == -1)
                spriteEffects |= SpriteEffects.FlipVertically;

            Vector2 muzzleOffset = muzzleOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
                muzzleOffset.Y = TextureAssets.Item[Type].Height() - muzzleOffset.Y;
            muzzleOffset -= new Vector2(texture.Width, texture.Height) * 0.5f;
            muzzleOffset = muzzleOffset.RotatedBy(velocity.ToRotation());
            Vector2 muzzlePosition = player.MountedCenter - new Vector2(0, 7) + offset + muzzleOffset;
            return muzzlePosition;
        }

        public virtual bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            GunCasingEffects(player, source, position, velocity, type, damage, knockback);
        
            Vector2 muzzlePosition = GetMuzzlePosition(player, velocity);

            //ctor2 muzzlePosition = player.MountedCenter + velocity.SafeNormalize(Vector2.Zero) * texture.Width / 2;
            ShootEffects(muzzlePosition, velocity);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GunHold>()] > 0)
            {
                foreach(var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != player.whoAmI)
                        continue;
                    if(proj.type != ModContent.ProjectileType<GunHold>())
                    {
                        continue;
                    }
                    proj.ai[0] = 0;
                    proj.ai[2] = 1;
                    proj.netUpdate = true;
                    break;
                }
            }
            return ShootProjectile(player, source, muzzlePosition, velocity, type, damage, knockback);
        }

        public void BasicMuzzleFlash(Vector2 position, Vector2 velocity, Color innerColor, Color outerColor)
        {
            var p = FXUtil.GlowCircleBoom(position, innerColor, outerColor, Color.Black);
            p.Scale *= Main.rand.NextFloat(0.4f, 0.65f);

            var sp = SmokeParticle.SpawnInAlphaLayer(position, velocity * 0.2f, Color.DarkGray);
            sp.initialColor = Color.Lerp(Color.Red, Color.Black, 0.6f);
            sp.fast = true;
            sp.dampening = 0.08f;

            MuzzleFlashParticle flashParticle = MuzzleFlashParticle.Spawn(position + velocity.SafeNormalize(Vector2.Zero) * 16, velocity.SafeNormalize(Vector2.Zero), innerColor);
            flashParticle.innerColor = innerColor;
            flashParticle.bloomColor = outerColor;
            flashParticle.Scale *= Main.rand.NextFloat(0.3f, 0.6f);
           // flashParticle.Scale *= Main.rand.NextFloat(0.15f, 0.3f);


  
            for (float f = 0; f < 3; f++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    gravity = 0f,
                    innerColor = innerColor,
                    outerColor = outerColor,
                    scaleRange = new Vector2(0.3f, 1f)
                };
                var dp = DustParticle.Spawn(position, velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f, 0.6f), spawnParams);
                dp.dampening = 0.15f;
                dp.fast = true;
            }
        }


        public virtual void ShootEffects(Vector2 position, Vector2 velocity)
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.05f;
            SoundEngine.PlaySound(shootSound, position);
            BasicMuzzleFlash(position, velocity, Color.Yellow, Color.Red);
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
        public bool doFailAnimation;
        public int numberOfReloadsNeeded;
        public int successfulReloads;
        public float reloadRatio => reloadTimer / reloadTime;
        public BaseGun HeldGun
        {
            get
            {
                BaseGun mouseGun = Main.mouseItem.ModItem as BaseGun;

                if (mouseGun != null)
                    return mouseGun;
                BaseGun myGun = Player.HeldItem.ModItem as BaseGun;
                return myGun;
                
            }
        }
        public static event Action<Player, BaseGun> OnReload;
        public override void ResetEffects()
        {
            base.ResetEffects();
            isReloading = false;
            numberOfReloadsNeeded = 1;
              marginOfError = 10;
            var heldGun = HeldGun;
            if (heldGun == null)
                reloadTime = 60;
            else
                reloadTime = heldGun.GetReloadWindow();
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
                reloadTimer = 0;
                doFailAnimation = true;
                successfulReloads--;
                if (successfulReloads <= 0)
                    successfulReloads = 0;
                return false;
            }
            return true;
  
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            HandleReloading();
        }
        

        private void HandleReloading()
        {
            if (reloadFireDelay > 0)
                reloadFireDelay--;

            var heldGun = HeldGun;
            if (heldGun == null)
            {
                reloadTimer = 0;
                return;
            }

            if (Main.myPlayer == Player.whoAmI &&
                Player.ownedProjectileCounts[ModContent.ProjectileType<GunHold>()] == 0 && (Player.channel || Player.controlUseItem) && HeldGun.UseDefaultHoldAnimation())
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<GunHold>(), 1, 1, Player.whoAmI);
            }
            if (heldGun.NeedsReloading() && !Player.channel)
            {
                Player.AddBuff(ModContent.BuffType<Reloading>(), 2);
                isReloading = true;
                reloadTimer++;
                if (reloadTimer >= reloadTime)
                {
                    reloadTimer = 0;
                }


                if (Main.myPlayer == Player.whoAmI)
                {
                    if (TimedReload())
                    {
                        successfulReloads++;
                        if (successfulReloads >= numberOfReloadsNeeded)
                        {
                            successfulReloads = 0;
                            heldGun.Reload();
                            reloadFireDelay = 60;
                            OnReload?.Invoke(Player, heldGun);
                        }
                        else
                        {
                            SoundStyle gunReloadSound = AssetRegistry.Sounds.Gun.GunReload;
                            gunReloadSound.PitchVariance = 0.2f;
                            gunReloadSound.Pitch = MathHelper.Lerp(0f, 1f, successfulReloads / numberOfReloadsNeeded);
                            gunReloadSound.Volume = 0.4f;
                            SoundEngine.PlaySound(gunReloadSound);

                            int combatText = CombatText.NewText(Player.getRect(), Color.White, $"{successfulReloads} / {numberOfReloadsNeeded}", true);
                            CombatText numText = Main.combatText[combatText];
                            numText.lifeTime = 60;
                        }

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
        private float _heatTimer;
        private float _oldItemTime;
        private float _startRotation;
        private Vector2 _recoilOffset;
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
        private Vector2 HoldDirection => HoldRotation.ToRotationVector2();
        private Player Owner => Main.player[Projectile.owner];
        private GunHoldPlayer GunHoldPlayer => Owner.GetModPlayer<GunHoldPlayer>(); 

        public override void SetDefaults()
        {
            base.SetDefaults();
         //   Projectile.hide = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }

        /*
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overPlayers.Add(index);
        }*/
        public override void AI()
        {
            base.AI();
            if (Owner.HeldItem.ModItem is BaseGun && (Owner.channel || Owner.controlUseItem))
            {
                Projectile.timeLeft = 120;
            }

            if(Owner.HeldItem.ModItem is BaseGun gun && !gun.UseDefaultHoldAnimation())
            {
                Projectile.Kill();
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 mousePos = Main.MouseWorld;
                Vector2 rotationVector = mousePos - Owner.Center;
                HoldRotation = rotationVector.ToRotation();
                Projectile.netUpdate = true;
            }
       
            if(Owner.HeldItem.ModItem != null)
            {
                Vector2? holdOutOffset = Owner.HeldItem.ModItem.HoldoutOffset();
                Vector2 offset = holdOutOffset.HasValue ? holdOutOffset.Value : Vector2.Zero;
                offset = offset.RotatedBy(HoldRotation);
                Projectile.Center = Owner.MountedCenter - new Vector2(0, 7) + offset + _recoilOffset;
                Projectile.rotation = HoldRotation;
            }
  

            if (State != AIState.Reload && GunHoldPlayer.doCoolReloadAnimation)
            {
                SwitchState(AIState.Reload);
                GunHoldPlayer.doCoolReloadAnimation = false;
            }

            _heatTimer *= 0.95f;
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

            //Owner.itemRotation = rotation * Owner.direction;

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90));// set arm position (90 degree offset since arm starts lowered)
            Owner.heldProj = Projectile.whoAmI;
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
            Timer++;
            if(Timer == 1)
            {
                _startRotation = Projectile.rotation;
            }
            float recoilTime = 10f;
            float ratio = Timer / recoilTime;
            float ease = EasingFunction.QuadraticBump(ratio);
            
            float shootRadians = MathHelper.ToRadians(-5 * Owner.direction);
            float offset = MathHelper.Lerp(0f, shootRadians, ease);
            Projectile.rotation = _startRotation + offset;
            _heatTimer = 1f;

            _recoilOffset = Vector2.Lerp(-HoldDirection * 8, Vector2.Zero, EasingFunction.InOutSine(ratio));
            if(Timer >= recoilTime)
            {
                SwitchState(AIState.Hold);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (Owner.HeldItem.ModItem == null)
                return false;

            Texture2D texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (Owner.direction == -1)
                spriteEffects |= SpriteEffects.FlipVertically;

            if (GunHoldPlayer.HeldGun == null)
                return false;

            bool useHeatShader = GunHoldPlayer.HeldGun.UseHeatShader();
            if (useHeatShader)
            {
                GunHeatShader gunHeatShader = ShaderContent.GetInstance<GunHeatShader>();
                gunHeatShader.Time = _heatTimer;

                Color hottestColor = Color.Yellow;
                Color coldestColor = Color.DarkRed;
                GunHoldPlayer.HeldGun.ModifyMuzzleFlashColors(ref hottestColor, ref coldestColor);
                gunHeatShader.HottestColor = hottestColor;
                gunHeatShader.ColdestColor = coldestColor;
                SpritebatchParams spritebatchParams = SpritebatchParams.InWorldAndZoomed() with { effect = gunHeatShader, sortMode = SpriteSortMode.Immediate };
                using (SpritebatchStarter.Begin(Main.spriteBatch, spritebatchParams))
                {
                    spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0);
                }
            }
            else
            {
                spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0);
            }

            /*
            Vector2? holdOutOffset = Owner.HeldItem.ModItem.HoldoutOffset();
            Vector2 offset = holdOutOffset.HasValue ? holdOutOffset.Value : Vector2.Zero;
            offset = offset.RotatedBy(Projectile.rotation);


            Vector2 muzzleOffset = (Owner.HeldItem.ModItem as BaseGun).muzzleOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
                muzzleOffset.Y = TextureAssets.Item[Type].Height() - muzzleOffset.Y;
            muzzleOffset -= new Vector2(texture.Width, texture.Height) * 0.5f;
            muzzleOffset = muzzleOffset.RotatedBy(Projectile.rotation);
            Vector2 muzzlePosition = Owner.MountedCenter - new Vector2(0, 7) + offset + muzzleOffset;// + muzzleOffset;



            SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.JumbledGlowCircle.Asset, muzzlePosition);
            flareDrawer.color = Color.OrangeRed * _heatTimer * 0.4f;
            flareDrawer.worldPosition += new Vector2(0, 0);
            flareDrawer.color.A = 0;
            flareDrawer.scale *= 0.15f;
            spriteBatch.Draw(flareDrawer);
    */
            return false;
        }
    }
}
