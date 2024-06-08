using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Stellamod.Projectiles.Gun;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfRealityPlayer : ModPlayer
    {
        public bool hasRuneOfReality;
        public override void ResetEffects()
        {
            hasRuneOfReality = false;
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (Main.rand.NextBool(7) && hasRuneOfReality)
            {
                var EntitySource = Player.GetSource_FromThis();
                Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0, 
                    ModContent.ProjectileType<RealityBolt>(), Player.HeldItem.damage / 4, 1, Player.whoAmI, 0, 0);
            }
        }
    }

    internal class RuneOfReality : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune of Reality");
            // Tooltip.SetDefault("When you hit an enemy, you will release homing magic bolts that summon mana stars when they hit ");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 20);
            recipe.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<RuneOfRealityPlayer>().hasRuneOfReality = true;
        }
    }
}