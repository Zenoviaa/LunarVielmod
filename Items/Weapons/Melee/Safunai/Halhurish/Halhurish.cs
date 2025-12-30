using Stellamod.Core.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Safunai.Halhurish
{
    public class Halhurish : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSafunai();
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 30;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<HalhurishProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.damage = 14;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
