using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
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

            if (IsSpecial)
            {
                line = new TooltipLine(Mod, "LeftHanded", "Use to equip to your gun holster's left hand!");
                tooltips.Add(line);
                line = new TooltipLine(Mod, "RightHanded", "OR right click to equip to your gun holster's right hand!");
                tooltips.Add(line);
            }
            else
            {
                if (LeftHand != LeftGunHolsterState.None)
                {
                    line = new TooltipLine(Mod, "LeftHanded", "Use to equip to your gun holster's left hand!");
                    tooltips.Add(line);
                }
                if (RightHand != RightGunHolsterState.None)
                {
                    line = new TooltipLine(Mod, "RightHanded", "Use to equip to your gun holster's right hand!");
                    tooltips.Add(line);
                }
            }

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

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = null;
            const string Base_Path = "Stellamod/Items/Weapons/Ranged/GunSwapping/";

            if (IsSpecial)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}LR").Value;
            } else if(LeftHand != LeftGunHolsterState.None)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}L").Value;
            } else if(RightHand != RightGunHolsterState.None)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}R").Value;
            }
            Vector2 drawOrigin = iconTexture.Size() / 2;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
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


    internal class Devolver : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 42;
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Devolver;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }


    internal class CinderNeedle : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 42;
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Cinder_Needle;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }

    internal class ShottyPitol : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 50;

        public override RightGunHolsterState RightHand => RightGunHolsterState.Shotty_Pitol;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }

    internal class BubbleBussy : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 50;

        public override RightGunHolsterState RightHand => RightGunHolsterState.Bubble_Bussy;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }

    internal class AssassinsRecharge : MiniGun
    {
        //Damage of this gun
        public const int Base_Damage = 50;

        public override RightGunHolsterState RightHand => RightGunHolsterState.Assassins_Recharge;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
        }
    }
}
