using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class CursedShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 40;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightRed;
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            //The below code makes this item hover up and down in the world
            //Don't forget to make the item have no gravity, otherwise there will be weird side effects
            float hoverSpeed = 5;
            float hoverRange = 0.2f;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
            Item.position = position;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrystalShard, 3);
            recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
