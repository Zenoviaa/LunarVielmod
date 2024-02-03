using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Bow;
using Stellamod.Projectiles.Slashers.ArchariliteRaysword;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Tools
{
    public class ArcharilitePickaxe : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Archarilite Pickaxe");
		}

		public override void SetDefaults() 
		{
			Item.damage = 6;
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
			Item.pick = 30;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 12);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().ArchariliteSC)
            {
                Item.useTime = 5;
                Item.useAnimation = 5;
                Item.pick = 35;
            }
            else
            {
                Item.useTime = 10;
                Item.useAnimation = 10;
                Item.pick = 30;
            }
            return false;
        }

    }
}