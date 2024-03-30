using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class Steali : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Steali");
			/* Tooltip.SetDefault("A small fast dash that provides invincibility as you dash" +
				"\nIncreased regeneration" +
				"\nYou may not attack while this is in use" +
				"\nHollow Knight inspiried!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;


			Item.lifeRegen = 10;
		}


        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{

            player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped = true;
            player.GetModPlayer<DashPlayer>().DashAccessoryEquipped = true;
           
            //	player.GetDamage(DamageClass.Generic) *= 0.95f;

        }

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		
	}

}