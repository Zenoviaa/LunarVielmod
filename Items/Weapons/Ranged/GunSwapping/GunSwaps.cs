using Stellamod.Helpers;
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
        }

        public override bool? UseItem(Player player)
        {
            if(LeftHand != LeftGunHolsterState.None)
            {
                player.GetModPlayer<GunPlayer>().LeftHand = LeftHand;
            }

            if(RightHand != RightGunHolsterState.None)
            {
                player.GetModPlayer<GunPlayer>().RightHand = RightHand;
            }
     
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
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Pulsing;
    }

    internal class Eagle : MiniGun
    {
        public const int Base_Damage = 24;
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Eagle;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
            Item.width = 56;
            Item.height = 30;
        }
    }

    internal class BurnBlast : MiniGun
    {
        public const int Base_Damage = 50;
        public override RightGunHolsterState RightHand => RightGunHolsterState.Burn_Blast;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = Base_Damage;
            Item.width = 62;
            Item.height = 38;
        }
    }

    internal class PoisonPistol : MiniGun
    {
        public override RightGunHolsterState RightHand => RightGunHolsterState.Poison_Pistol;
    }
}
