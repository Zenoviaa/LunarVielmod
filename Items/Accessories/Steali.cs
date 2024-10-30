using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class Steali : BaseDashItem
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
		}


        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
			return true;
          //  return !player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
			dashPlayer.DashVelocity += 7;
        }
	}

}