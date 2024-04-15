using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Bow;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.Paint;
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
    }

    public enum RightGunHolsterState
    {
        None,
        Burn_Blast,
        Poison_Pistol,
        Cannon,
        Rocket_Launcher,
        Ravest_Blast
    }

    internal class GunPlayer : ModPlayer
    {
        public LeftGunHolsterState LeftHand;
        public RightGunHolsterState RightHand;
    }

    internal class GunHolster : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Broken Wrath");
        }

        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 36;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Bullet;
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
                    leftHand.OverrideColor = Color.LightCyan;
                    break;
                case LeftGunHolsterState.Ravest_Blast:
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
                case RightGunHolsterState.Cannon:
                    rightHand.OverrideColor = Color.RosyBrown;
                    break;
                case RightGunHolsterState.Rocket_Launcher:
                    rightHand.OverrideColor = Color.LightGray;
                    break;
            }

            tooltips.Add(leftHand);
            tooltips.Add(rightHand);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse == 2)
            {
                Shoot_SecondaryFire(player, source, position, velocity, type, damage, knockback);
            }
            else
            {
                Shoot_PrimaryFire(player, source, position, velocity, type, damage, knockback);
            }

            return false;
        }

        private void Shoot_PrimaryFire(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
          
            LeftGunHolsterState state = gunPlayer.LeftHand;
            switch (state)
            {
                //Do nothing, maybe shoot puff of smoke lmao
                default:
                case LeftGunHolsterState.None:
                    break;
                case LeftGunHolsterState.Pulsing:
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<ClockworkBomb>(), damage, knockback, player.whoAmI);
                    break;
                case LeftGunHolsterState.Eagle:
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<CrysalizerArrow1>(), damage, knockback, player.whoAmI);
                    break;
                case LeftGunHolsterState.Ms_Freeze:
                    break;
                case LeftGunHolsterState.Ravest_Blast:
                    break;
            }
        }

        private void Shoot_SecondaryFire(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            RightGunHolsterState state = gunPlayer.RightHand;
            switch (state)
            {               
                //Do nothing, maybe shoot puff of smoke lmao
                default:
                case RightGunHolsterState.None:
                    break;
                case RightGunHolsterState.Burn_Blast:
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<HeatedShot>(), damage, knockback, player.whoAmI);
                    break;
                case RightGunHolsterState.Poison_Pistol:
                    Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<PhotobombProj>(), damage, knockback, player.whoAmI);
                    break;
                case RightGunHolsterState.Cannon:
                    break;
                case RightGunHolsterState.Rocket_Launcher:
                    break;
                case RightGunHolsterState.Ravest_Blast:
                    break;
            }
        }
    }
}
