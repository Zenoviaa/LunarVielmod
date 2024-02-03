using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Tools
{
    public class ArchariliteAxe : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Archarilite Axe");
		}

		public override void SetDefaults() 
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.axe = 9;

        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (player.GetModPlayer<MyPlayer>().ArchariliteSC)
			{
				Item.useTime = 5;
				Item.useAnimation = 5;
				Item.axe = 12;
			}
			else
			{
				Item.useTime = 10;
				Item.useAnimation = 10;
				Item.axe = 9;
			}
       
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 8);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}