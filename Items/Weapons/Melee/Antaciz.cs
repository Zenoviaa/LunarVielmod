using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
    public class Antaciz : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 16;
        }

        public override void SetDefaults()
        {

            Item.damage = 11; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Melee;
            Item.width = 40; // hitbox width of the Item
            Item.height = 20; // hitbox height of the Item
            Item.useTime = 100; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 11; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Blue; // the color that the Item's name will be in-game
            Item.UseSound = SoundID.Item1; // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<AntacizProj>();
            Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 10);
            recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 10);
            recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 15);
            recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 9);
            recipe.Register();
        }
    }
}