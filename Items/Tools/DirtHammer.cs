using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Tools
{
    public class DirtHammer : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Dirt Hammer"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Why would you craft this?");
        }

		public override void SetDefaults() 
		{
			Item.damage = 2;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 7;
			Item.useAnimation = 7;
			Item.useStyle = ItemUseStyleID.Thrust;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.hammer = 30;	
		}

		public override void AddRecipes() 
		{
			Recipe recipe = CreateRecipe();

			recipe.AddIngredient(ItemID.DirtBlock, 8);
			recipe.Register();
		}
	}
}