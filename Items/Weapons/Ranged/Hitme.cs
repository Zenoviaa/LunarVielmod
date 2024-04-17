using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Hitme : ClassSwapItem
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
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.DD2_FlameburstTowerShot;

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 8;
			Item.knockBack = 6f;
			Item.noMelee = true;
			Item.crit = 12;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<HitmeProj>();
			Item.shootSpeed = 6f;
			Item.value = Item.sellPrice(0, 5, 0, 0);
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}

	}
}












