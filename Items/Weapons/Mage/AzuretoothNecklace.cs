using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class AzuretoothNecklace : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.damage = 26;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.knockBack = 2;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 52;
            Item.mana = 8; 

            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.shoot = ModContent.ProjectileType<AzuretoothNecklaceHold>();
            Item.shootSpeed = 10;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<IllurineScale>(), 16);
            recipe.AddIngredient(ItemID.Ectoplasm, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
