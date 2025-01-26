using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class GovheilHolsterBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
            Item.buffType = ModContent.BuffType<GovheilB>();
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<VirulentPlating>());
        }

    }
}