using Stellamod.Core.Bases;
using Terraria;
using Terraria.DataStructures;


namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class IronBow : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 2;
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            CrossbowPlayer crossbowPlayer = player.GetModPlayer<CrossbowPlayer>();
            crossbowPlayer.BurstShot(3, 5, shootParams.velocity, shootParams.chargeStrength);
        }
    }
}