using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class BurningAngel : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Throwing;

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "BurningAngel", "(A) Great Damage scaling for explosions!")
			{
				OverrideColor = new Color(108, 271, 99)

			};
			tooltips.Add(line);


			line = new TooltipLine(Mod, "BurningAngel", "(Special) The farther your cursor is, the faster the axe goes!")
			{
				OverrideColor = new Color(235, 52, 97)

			};
			tooltips.Add(line);
			base.ModifyTooltips(tooltips);
		}

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 10;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 51;
			Item.useAnimation = 51;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.DD2_FlameburstTowerShot;

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 4;
			Item.knockBack = 5f;
			Item.noMelee = true;
			Item.crit = 26;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<BurningAngelProj>();
			Item.shootSpeed = 4f;
			Item.value = 10000;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}

	}
}












