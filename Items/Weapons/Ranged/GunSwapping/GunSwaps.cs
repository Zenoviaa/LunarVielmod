using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.GunSwapping
{
    internal abstract class MiniGun : ModItem
    {
        public virtual LeftGunHolsterState LeftHand { get; }
        public virtual RightGunHolsterState RightHand { get; }
        public bool IsSpecial => LeftHand != LeftGunHolsterState.None && RightHand != RightGunHolsterState.None;
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "WeaponType", "Gun Holster Weapon Type")
            {
                OverrideColor = ColorFunctions.GunHolsterWeaponType
            };
            tooltips.Add(line);

            if (!Main.LocalPlayer.HasItemInAnyInventory(ModContent.ItemType<GunHolster>()))
            {
                line = new TooltipLine(Mod, "WompWomp", "You do not have a Gun Holster...")
                {
                    OverrideColor = Color.Gray
                };
                tooltips.Add(line);

                line = new TooltipLine(Mod, "WompWomp2", "Buy one from Delgrim!")
                {
                    OverrideColor = Color.Gray
                };
                tooltips.Add(line);
            }
           
        }

        public override bool AltFunctionUse(Player player)
        {
            return IsSpecial;
        }

        public override bool? UseItem(Player player)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            if (IsSpecial)
            {
                if(player.altFunctionUse == 2)
                {
                    if (gunPlayer.LeftHand == LeftHand)
                        gunPlayer.LeftHand = LeftGunHolsterState.None;
                    gunPlayer.RightHand = RightHand;
                }
                else
                {
                    if (gunPlayer.RightHand == RightHand)
                        gunPlayer.RightHand = RightGunHolsterState.None;
                    gunPlayer.LeftHand = LeftHand;
                }
            }
            else
            {
                if (LeftHand != LeftGunHolsterState.None)
                {
                    gunPlayer.LeftHand = LeftHand;
                }

                if (RightHand != RightGunHolsterState.None)
                {
                    gunPlayer.RightHand = RightHand;
                }
            }

            //Remember to code this slightly differently for the special sirestias one that can go on both hands
            //Actually you could just make it right clickable lol, left click for primary hand, right click for secondary hand
            //Don't allow more than one of course

            //Left-Handed Pulsing
            //Left-Handed Eagle
            //L
            //Right-Handed Burn Blast
            //Right-Handed Poison Pistol
            //Right-Handed Cannon
            //Right-Handed Rocket Launcher
            return base.UseItem(player);
        }
    }

    internal class Pulsing : MiniGun
    {        
        //Damage of this gun holster
        public const int Base_Damage = 18;
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Pulsing;
        public override RightGunHolsterState RightHand => RightGunHolsterState.Pulsing;
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = Base_Damage;
            Item.width = 56;
            Item.height = 30;
            Item.value = Item.buyPrice(gold: 5);
        }
    }

    internal class Eagle : MiniGun
    {
        //Damage of this gun holster
        public const int Base_Damage = 24;

        //Which thing it sets
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Eagle;

        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = Base_Damage;
            Item.width = 56;
            Item.height = 30;
        }
    }

    internal class BurnBlast : MiniGun
    {       
        //Damage of this gun holster
        public const int Base_Damage = 50;

        //Which thing it sets
        public override RightGunHolsterState RightHand => RightGunHolsterState.Burn_Blast;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;

            //Setting this to width and height of the texture cause idk
            Item.width = 62;
            Item.height = 38;
        }       
    }

    internal class PoisonPistol : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 90;
        public override RightGunHolsterState RightHand => RightGunHolsterState.Poison_Pistol;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }

    internal class RocketLauncher : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 50;

        public override RightGunHolsterState RightHand => RightGunHolsterState.Rocket_Launcher;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
            Item.value = Item.buyPrice(gold: 15);
        }
    }

    internal class MintyBlast : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 10;

        public override RightGunHolsterState RightHand => RightGunHolsterState.Minty_Blast;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 9);

            recipe.Register();
        }
    }

    internal class MsFreeze : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 20;
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Ms_Freeze;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }

    internal class Electrifying : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 20;
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Electrifying;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }

    internal class  RavestBlast : MiniGun
    {        
        //Damage of this gun holster
        public const int Base_Damage = 262;

        //Which thing it sets
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Ravest_Blast;
        public override RightGunHolsterState RightHand => RightGunHolsterState.Ravest_Blast;

        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = Base_Damage;
            Item.width = 56;
            Item.height = 30;
        }
    }

    internal class STARBUST : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 42;
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.STARBUST;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }
}
