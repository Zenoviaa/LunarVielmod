using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class IllurianBroochPlayer : ModPlayer
    {
        public bool IllurianBroochActive => Player.GetModPlayer<BroochSpawnerPlayer>().BroochActive(ModContent.ItemType<IllurianBroochA>());
        public override void PostUpdate()
        {
            base.PostUpdate();
            if (IllurianBroochActive)
            {
                Player.moveSpeed += 0.05f;
                Player.jumpSpeedBoost *= 1.2f;
                Player.accRunSpeed *= 1.5f;
                Player.maxRunSpeed *= 1.5f;
                Player.hasMagiluminescence = true;
                Player.statDefense -= 10;
            }
        }
    }

    public class IllurianBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Lime;
            Item.buffType = ModContent.BuffType<IllurianB>();
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBrooch>(), material: ModContent.ItemType<IllurineScale>());
        }
    }
}