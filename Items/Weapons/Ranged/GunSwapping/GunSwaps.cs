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
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
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
        public override LeftGunHolsterState LeftHand => LeftGunHolsterState.Eagle;
    }

    internal class BurnBlast : MiniGun
    {
        public override RightGunHolsterState RightHand => RightGunHolsterState.Burn_Blast;
    }

    internal class PoisonPistol : MiniGun
    {
        public override RightGunHolsterState RightHand => RightGunHolsterState.Poison_Pistol;
    }
}
