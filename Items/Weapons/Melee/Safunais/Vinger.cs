using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Projectiles.Safunai.Parendine;
using Stellamod.Projectiles.Safunai.Vinger;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Safunais
{
    public class Vinger : BaseSafunaiItem
    {

        public override DamageClass AlternateClass => DamageClass.Generic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 17;
            Item.mana = 0;
        }
      
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Vinger",  Helpers.LangText.Common("Safunai"))
			{
				OverrideColor = new Color(308, 71, 99)

			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Vinger", "(C) Medium Damage Scaling (spikyballs) On Hit!")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);
		}

		public override void SetDefaults()
		{
            Item.width = 32;
            Item.height = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.shoot = ModContent.ProjectileType<VingerProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 35;
            Item.rare = ItemRarityID.Blue;
        }
	}
}
	