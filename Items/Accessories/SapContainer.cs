using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod;
using Terraria.Audio;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Accessories
{
	public class SapContainer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sap Container");
			Tooltip.SetDefault("Every 10 seconds eat stardew and MAJORLY increase your magic damage" +
				"\n+7% Magic damage...");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public float Timer2;

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 3);
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

			if (Timer2 == 601)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 7; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(player.Center, speed * 3, ParticleManager.NewInstance<ArcanalParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					
				}
			}
			if (Timer2 > 600)
            {
				player.GetDamage(DamageClass.Magic) *= 4f;
				
			}

			if (Timer2 < 720)
			{
				Timer2 = 0;
			}
			player.GetDamage(DamageClass.Magic) *= 1.07f; // Increase ALL player damage by 100%
			player.GetModPlayer<MyPlayer>().ArcaneM = true;
			player.GetModPlayer<MyPlayer>().ArcaneMCooldown++;
		


		}




	}
}