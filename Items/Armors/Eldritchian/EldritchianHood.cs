using Microsoft.Xna.Framework;

using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Eldritchian
{
    public class EldritchianPlayer : ModPlayer
    {
		private float _attackSpeedBoostCounter;
		private float _attackSpeedBoost;

		public bool hasEldritchian;
		
		public const float Max_Damage = 200;
		public const float Max_Duration = 600;
		public const float Max_Speed = 3f;
        
		public override void ResetEffects()
        {
			hasEldritchian = false;

        }

        public override void UpdateDead()
        {
            base.UpdateDead();
			_attackSpeedBoostCounter = 0;
        }

        public override void PostUpdateEquips()
        {
            if (hasEldritchian && _attackSpeedBoostCounter > 0)
            {
				_attackSpeedBoostCounter--;
				float durationMultiplier = _attackSpeedBoostCounter / Max_Duration;
				float boost = durationMultiplier * _attackSpeedBoost;
				Player.GetAttackSpeed(DamageClass.Throwing) += boost;

				if (Main.rand.NextBool(2))
				{
					int count = Main.rand.Next(6);
				}
			}
		}

        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
			if (hasEldritchian)
			{
				float damage = info.Damage;
				float multiplier = damage / Max_Damage;
				_attackSpeedBoost = Max_Speed * multiplier;
				_attackSpeedBoostCounter = Max_Duration * multiplier;
				Player.AddBuff(ModContent.BuffType<ShadowBoost>(), (int)_attackSpeedBoostCounter);
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/OverGrowth_TP1"));
			}
		}
    }

    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class EldritchianHood : ModItem
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
			Item.defense = 14; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.GetAttackSpeed(DamageClass.Throwing) += 0.1f;
			player.GetDamage(DamageClass.Throwing) += 0.20f;
			player.nightVision = true;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<EldritchianCloak>() && legs.type == ModContent.ItemType<EldritchianLegs>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			//Shadow Effect
			if (Main.rand.NextBool(10))
			{
				int count = Main.rand.Next(6);
			}



			player.setBonus = LangText.SetBonus(this);//"Grants immunity to knockback!\n" + "When you take a hit, gain a temporary attack speed boost based on the amount of damage you took!");

			player.noKnockback = true;
			player.GetModPlayer<EldritchianPlayer>().hasEldritchian = true;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 15);
			recipe.AddIngredient(ItemID.SoulofSight, 10);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 8);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}