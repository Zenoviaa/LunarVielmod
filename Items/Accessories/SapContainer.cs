using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class SapContainer : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sap Container");
			/* Tooltip.SetDefault("Every 10 seconds eat stardew and MAJORLY increase your magic damage" +
				"\n+7% Magic damage..."); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public float Timer2;

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 10);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 3);
			recipe.AddIngredient(ModContent.ItemType<MorrowVine>(), 9);
			recipe.AddIngredient(ModContent.ItemType<Morrowshroom>(), 20);
			recipe.AddIngredient(ItemID.Bottle, 10);
			recipe.AddIngredient(ItemID.BottledHoney, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			Timer2++;

			
			player.GetDamage(DamageClass.Magic) *= 1.07f; // Increase ALL player damage by 100%
			player.GetModPlayer<MyPlayer>().ArcaneM = true;
			player.GetModPlayer<MyPlayer>().ArcaneMCooldown++;
		


		}




	}
}