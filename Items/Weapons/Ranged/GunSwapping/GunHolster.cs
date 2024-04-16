using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.GunHolster;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.GunSwapping
{
    public enum LeftGunHolsterState
    {
        None,
        Pulsing,
        Eagle,
        Ms_Freeze,
        Ravest_Blast,
        Electrifying,
        STARBUST,
        Cinder_Needle,
        Devolver
    }

    public enum RightGunHolsterState
    {
        None,
        Burn_Blast,
        Poison_Pistol,
        Minty_Blast,
        Rocket_Launcher,
        Ravest_Blast,
        Pulsing,
        Bubble_Bussy,
        Shotty_Pitol,
        Assassins_Recharge
    }

    internal class GunPlayer : ModPlayer
    {
        public LeftGunHolsterState LeftHand;
        public RightGunHolsterState RightHand;

        private void HolsterGun(Player player, int projectileType, int baseDamage, int knockBack)
        {
            //   player.damage
            int newDamage = (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo((float)baseDamage);
            if (player.HeldItem.type == ModContent.ItemType<GunHolster>()
                && player.ownedProjectileCounts[projectileType] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    projectileType, newDamage, knockBack, player.whoAmI);
            }
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            int baseDamage;
            int knockback;

            //This is where you actually spawn the gun holsters, so that the player holds them
            //We spawn the guns here cause otherwise the damage won't scale from your modifiers
            switch (RightHand)
            {
                //Do nothing, maybe shoot puff of smoke lmao
                default:
                case RightGunHolsterState.None:
                    break;

                case RightGunHolsterState.Burn_Blast:

                    //Don't forget to set the damage
                    baseDamage = BurnBlast.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterBurnBlastProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Poison_Pistol:
                    baseDamage = PoisonPistol.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterPoisonPistolProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Minty_Blast:
                    baseDamage = MintyBlast.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterMsFreezeProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Rocket_Launcher:
                    baseDamage = RocketLauncher.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterRocketLauncherProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Ravest_Blast:
                    baseDamage = RavestBlast.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterRavestBlastRightProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Pulsing:
                    baseDamage = Pulsing.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterPulsingProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Bubble_Bussy:
                    baseDamage = BubbleBussy.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterBubbleBussyProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Shotty_Pitol:
                    baseDamage = ShottyPitol.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterShottyPitolProj>(), baseDamage, knockback);
                    break;

                case RightGunHolsterState.Assassins_Recharge:
                    baseDamage = AssassinsRecharge.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterAssassinsRechargeProj>(), baseDamage, knockback);
                    break;
            }

            switch (LeftHand)
            {
                default:
                case LeftGunHolsterState.None:
                    break;

                case LeftGunHolsterState.Pulsing:
                    baseDamage = Pulsing.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterPulsingProj>(), baseDamage, knockback);
                    break;

                case LeftGunHolsterState.Eagle:
                    baseDamage = Eagle.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterEagleProj>(), baseDamage, knockback);
                    break;

                case LeftGunHolsterState.Ms_Freeze:
                    baseDamage = MsFreeze.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterMintyBlastProj>(), baseDamage, knockback);
                    break;

                case LeftGunHolsterState.Ravest_Blast:
                    baseDamage = RavestBlast.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterRavestBlastLeftProj>(), baseDamage, knockback);
                    break;

                case LeftGunHolsterState.Electrifying:
                    baseDamage = Electrifying.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterElectrifyingProj>(), baseDamage, knockback);
                    break;

                case LeftGunHolsterState.STARBUST:
                    baseDamage = STARBUST.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterSTARBUSTProj>(), baseDamage, knockback);
                    break;

                case LeftGunHolsterState.Cinder_Needle:
                    baseDamage = CinderNeedle.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterCinderNeedleProj>(), baseDamage, knockback);
                    break;

                case LeftGunHolsterState.Devolver:
                    baseDamage = Devolver.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterDevolverProj>(), baseDamage, knockback);
                    break;
            }
        }
    }

    internal class GunHolster : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 36;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;

            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = null;
            Item.noUseGraphic = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            GunPlayer gunPlayer = Main.LocalPlayer.GetModPlayer<GunPlayer>();
            var leftHand = new TooltipLine(Mod, "left", "");
            var rightHand = new TooltipLine(Mod, "right", "");
            switch (gunPlayer.LeftHand)
            {
                default:
                    leftHand.Text = "Left Hand: [None]";
                    leftHand.OverrideColor = Color.DarkGray;
                    break;
                case LeftGunHolsterState.Pulsing:
                    leftHand.Text = "Left Hand: [Pulsing]";
                    leftHand.OverrideColor = Color.LightGray;
                    break;
                case LeftGunHolsterState.Eagle:
                    leftHand.Text = "Left Hand: [Eagle]";
                    leftHand.OverrideColor = Color.DarkOrange;
                    break;
                case LeftGunHolsterState.Ms_Freeze:
                    leftHand.Text = "Left Hand: [Ms Freeze]";
                    leftHand.OverrideColor = Color.LightCyan;
                    break;
                case LeftGunHolsterState.Ravest_Blast:
                    leftHand.Text = "Left Hand: [Ravest Blast]";
                    leftHand.OverrideColor = Color.LightPink;
                    break;
            }

            switch (gunPlayer.RightHand)
            {
                default:
                    rightHand.Text = "Right Hand: [None]";
                    rightHand.OverrideColor = Color.DarkGray;
                    break;
                case RightGunHolsterState.Burn_Blast:
                    rightHand.Text = "Right Hand: [Burn Blast]";
                    rightHand.OverrideColor = Color.Orange;
                    break;
                case RightGunHolsterState.Poison_Pistol:
                    rightHand.Text = "Right Hand: [Poison Pistol]";
                    rightHand.OverrideColor = Color.Green;
                    break;
                case RightGunHolsterState.Minty_Blast:
                    rightHand.Text = "Right Hand: [Minty Blast]";
                    rightHand.OverrideColor = Color.LightCyan;
                    break;
                case RightGunHolsterState.Rocket_Launcher:
                    rightHand.Text = "Right Hand: [Rocket Launcher]";
                    rightHand.OverrideColor = Color.LightGray;
                    break;
                case RightGunHolsterState.Ravest_Blast:
                    rightHand.Text = "Right Hand: [Ravest Blast]";
                    rightHand.OverrideColor = Color.LightPink;
                    break;
            }
        

            tooltips.Add(leftHand);
            tooltips.Add(rightHand);

            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "WeaponType", "Gun Holster Weapon Type")
            {
                OverrideColor = ColorFunctions.GunHolsterWeaponType
            };
            tooltips.Add(line);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            GunPlayer gunPlayer = Main.LocalPlayer.GetModPlayer<GunPlayer>();
            const string Base_Path = "Stellamod/Items/Weapons/Ranged/GunSwapping/";

            if(gunPlayer.LeftHand != LeftGunHolsterState.None)
            {
                string textureName = gunPlayer.LeftHand.ToString().Replace("_", "");
                Texture2D leftHandTexture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}_Held").Value;
                Vector2 leftHandTextureSize = leftHandTexture.Size();
                Vector2 leftHandDrawOrigin = leftHandTextureSize / 2;

                Vector2 drawPosition = position;
                spriteBatch.Draw(leftHandTexture, drawPosition, null, drawColor, 0f, leftHandDrawOrigin, scale, SpriteEffects.None, 0);
            }

            if(gunPlayer.RightHand != RightGunHolsterState.None)
            {

                string textureName = gunPlayer.RightHand.ToString().Replace("_", "");
                Texture2D rightHandTexture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}_Held").Value;
                Vector2 rightHandTextureSize = rightHandTexture.Size();
                Vector2 rightHandDrawOrigin = rightHandTextureSize / 2;

                //Offset it a little
                Vector2 drawPosition = position + new Vector2(8, 8);
                spriteBatch.Draw(rightHandTexture, drawPosition, null, drawColor, 0f, rightHandDrawOrigin, scale, SpriteEffects.None, 0);
            }

            if (gunPlayer.LeftHand != LeftGunHolsterState.None || gunPlayer.RightHand != RightGunHolsterState.None)
                return false;
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }


}
