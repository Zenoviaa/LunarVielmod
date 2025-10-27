using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class MushroomGreatbow : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 3;
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            CrossbowPlayer crossbowPlayer = player.GetModPlayer<CrossbowPlayer>();
            crossbowPlayer.BurstShot(5, 5, shootParams.velocity, shootParams.chargeStrength);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<Mushroom>());
        }
    }
}
