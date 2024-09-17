using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Garbage
{
	

	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class GarbageMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 28; // Width of the item
			Item.height = 26; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = ItemRarityID.Pink; // The rarity of the item
			Item.defense = 17; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.GetAttackSpeed(DamageClass.Throwing) += 0.3f;
			player.GetDamage(DamageClass.Throwing) += 0.15f;
			player.GetDamage(DamageClass.Summon) += 0.15f;
			player.maxMinions += 2;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<GarbageChestplate>() && legs.type == ModContent.ItemType<GarbagePants>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			//Shadow Effect
			if (Main.rand.NextBool(10))
			{
				int count = Main.rand.Next(3);
				for (int iz = 0; iz < count; iz++)
				{
					for (int i = 0; i < 1; i++)
					{
						Dust.NewDustPerfect(player.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.PaleVioletRed, 0.5f).noGravity = true;
					}
					for (int i = 0; i < 1; i++)
					{
						Dust.NewDustPerfect(player.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.Green, 0.5f).noGravity = true;
					}
				}
			}



			player.setBonus = LangText.SetBonus(this);//"Grants immunity to knockback!\n" + "+2 Summons");
			player.noKnockback = true;
			player.maxMinions += 2;
		
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 15);
			recipe.AddIngredient(ModContent.ItemType<ArmorDrive>(), 10);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}