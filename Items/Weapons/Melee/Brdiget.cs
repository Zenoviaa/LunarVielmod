using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
    public class Bridget : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Throwing;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.damage = 10;
        }
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Spinny Winny damage the binny");

        }


        public override void SetDefaults()
        {

            Item.damage = 4; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
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
            Item.shoot = ModContent.ProjectileType<BridgetProj>();
            Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = false;


        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.DirtBlock, 99);
            recipe.AddIngredient(ItemID.Arkhalis, 1);

            recipe.Register();
        }
    }
}