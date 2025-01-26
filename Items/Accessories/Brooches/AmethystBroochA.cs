using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class AmethystBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.buffType = ModContent.BuffType<AmethystBroo>();
            Item.accessory = true;
            BroochType = BroochType.Simple;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBrooch>(), material: ModContent.ItemType<Mushroom>());
        }
    }
}