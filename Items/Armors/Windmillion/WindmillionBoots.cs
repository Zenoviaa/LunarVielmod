using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Armors.Windmillion
{
    [AutoloadEquip(EquipType.Legs)]
    public class WindmillionBoots : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Astrasilk Boots");
			// Tooltip.SetDefault("Increases movement speed by 20%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.GetDamage(DamageClass.Throwing) *= 1.10f;
        }

       
    }
}
