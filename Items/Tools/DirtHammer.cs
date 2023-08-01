using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;


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
			Item.useStyle = 3;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
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