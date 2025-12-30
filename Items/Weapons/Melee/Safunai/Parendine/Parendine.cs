using Stellamod.Core.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Melee.Safunai.Parendine
{
    public class Parendine : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSafunai();
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<ParendineProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.damage = 16;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
