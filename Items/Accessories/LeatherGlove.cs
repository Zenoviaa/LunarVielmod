
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class LeatherGlove : ModItem
	{
		public override void SetDefaults()
		{
			Item.value = Item.sellPrice(gold: 1);
            Item.width = 26;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.value = 1200;
            Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Throwing).Flat += 3;
            player.ThrownVelocity += 2;
        }
	}
}
