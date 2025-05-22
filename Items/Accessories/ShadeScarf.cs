using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
	public class ShadeScarf : BaseDashItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<PearlescentScrap>());
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashVelocity += 7;
			player.moveSpeed *= 1.3f;
			player.maxRunSpeed *= 1.3f;
			player.statLifeMax2 += 10;
		}
	}
}