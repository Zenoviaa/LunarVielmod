using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT
{
    public class BlunderofBlunders : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<GunHoldPlayer>().numberOfReloadsNeeded += 4;
            if (player.HeldItem.ModItem is BaseGun gun)
            {
                player.GetAttackSpeed(DamageClass.Ranged) += 0.5f;

            }
   
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MechanizedSoul, BlankAccessory>();
        }
    }
}
