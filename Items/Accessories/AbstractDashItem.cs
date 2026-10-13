using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public abstract class AbstractDashItem : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            //This class should be inherited to prevent multiple dash augments from being equipped, 
            //These things are gonna be powerful so
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashAugmentEquipped = true;
            dashPlayer.DashItems.Add(this);
        }

        public virtual void BeginDash(Player player)
        {

        }
        /// <summary>
        /// Called every tick that you are dashing basically
        /// </summary>
        /// <param name="player"></param>
        public virtual void UpdateDash(Player player)
        {

        }

        public virtual void EndDash(Player player)
        {

        }
    }
}
