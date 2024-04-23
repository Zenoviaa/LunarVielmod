
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Thrown
{
    public class ComicalTrident : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Melee;

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 12;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.crit = 33;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ComicalTridentP>();
            Item.shootSpeed = 19f;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = false;
          
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<GintzlMetal>(), 100);
            recipe.AddIngredient(ItemType<GraftedSoul>(), 100);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}