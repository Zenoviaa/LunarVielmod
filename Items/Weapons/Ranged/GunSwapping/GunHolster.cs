using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.GunHolster;
using System;
using System.Collections.Generic;
using Terraria;
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
    }

    public enum RightGunHolsterState
    {
        None,
        Burn_Blast,
        Poison_Pistol,
        Minty_Blast,
        Rocket_Launcher,
        Ravest_Blast
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

                    break;
                case RightGunHolsterState.Minty_Blast:
                    break;
                case RightGunHolsterState.Rocket_Launcher:
                    break;
                case RightGunHolsterState.Ravest_Blast:
                    baseDamage = RavestBlast.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterRavestBlastRightProj>(), baseDamage, knockback);
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

                    break;
                case LeftGunHolsterState.Ravest_Blast:
                    baseDamage = RavestBlast.Base_Damage;
                    knockback = 1;
                    HolsterGun(Player, ModContent.ProjectileType<GunHolsterRavestBlastLeftProj>(), baseDamage, knockback);
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

            Texture2D leftHandTexture = null;
            Texture2D rightHandTexture = null;
            Vector2 leftHandTextureSize = Vector2.Zero;
            Vector2 rightHandTextureSize = Vector2.Zero;
            const string Base_Path = "Stellamod/Items/Weapons/Ranged/GunSwapping/";





            if(gunPlayer.LeftHand != LeftGunHolsterState.None)
            {
                string textureName = gunPlayer.LeftHand.ToString().Replace("_", "");
                leftHandTexture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}_Held").Value;
                leftHandTextureSize = leftHandTexture.Size();
                Vector2 leftHandDrawOrigin = leftHandTextureSize / 2;

                Vector2 drawPosition = position;
                spriteBatch.Draw(leftHandTexture, drawPosition, null, drawColor, 0f, leftHandDrawOrigin, scale, SpriteEffects.None, 0);
            }

            if(gunPlayer.RightHand != RightGunHolsterState.None)
            {

                string textureName = gunPlayer.RightHand.ToString().Replace("_", "");
                rightHandTexture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}_Held").Value;
                rightHandTextureSize = rightHandTexture.Size();
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
