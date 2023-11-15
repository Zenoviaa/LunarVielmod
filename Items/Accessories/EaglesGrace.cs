
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
	public class EaglesGrace : ModItem
	{
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(20, 5f, 2);
        }

        public override void SetDefaults()
		{
			Item.Size = new Vector2(25);
			Item.accessory = true;
			Item.value = Item.sellPrice(gold: 4);
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 55;
			if (Main.rand.NextBool(4))
			{
				Dust.NewDust(player.position, player.width, player.height, DustID.GreenFairy);
			}
		}

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.11f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.6f;
            constantAscend = 0.135f;
        }
    }
}