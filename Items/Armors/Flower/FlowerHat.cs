using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Chains;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria.Audio;
using Stellamod.Projectiles.Arrows;
using Stellamod.Helpers;

namespace Stellamod.Items.Armors.Flower
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
 
	[AutoloadEquip(EquipType.Head)]
	public class FlowerHat : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lovestruck Mask");
			/* Tooltip.SetDefault("Magical essence of an Lusting Goddess"
				+ "\n+7% increased damage" +
				"\n+40 Health"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.LightRed; // The rarity of the item
			Item.defense = 12; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Melee) += 0.16f;
			player.GetDamage(DamageClass.Ranged) += 0.16f;
			player.hasAngelHalo = true;
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FlowerRobe>() && legs.type == ModContent.ItemType<FlowerPants>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Increases life regen by decent amount!" + "\nThe armor makes a flowery circle that heals players in it for a large amount!" + "\nTurns all your wooden arrows into flower arrows! Which when hitting a target, " + "\nsplits into little golden shots that hit back!"); // This is the setbonus tooltip


			player.lifeRegen += 1;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlowerLeafAura>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<FlowerLeafAura>(), 0, 0, player.whoAmI);
            }

            player.GetModPlayer<FlowerPlayer>().hasQuiver = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 5);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 100);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}


		
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}

	internal class FlowerPlayer : ModPlayer
	{
		public bool hasQuiver;
		public override void ResetEffects()
		{
			base.ResetEffects();
			hasQuiver = false;
		}

		public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.WoodenArrowFriendly && hasQuiver)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordThrow"), position);
				type = ModContent.ProjectileType<FlowerArrow>();
				damage += 2;
				velocity *= 2f;
			}
		}
	}
}