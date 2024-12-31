using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Head)]
    public class FoxMark : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Hiker's Backpack");
            /* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +3% damage" +
				"\n Drops stumps on the ground as you walk! "); */
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 15, 10, 10);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}

		

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) *= 1.04f; // Increase ALL player damage by 100%
            player.moveSpeed += 0.3f;
            player.maxRunSpeed += 0.3f;
            player.statLifeMax2 += 30;

        }




	}
}