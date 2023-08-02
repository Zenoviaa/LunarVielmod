using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Stellamod.Projectiles.Thrown;

namespace Stellamod.Items.Weapons.Thrown
{
	public class AcidicLancher : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Virulent Missiles"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 27;
			Item.DamageType = DamageClass.Throwing;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ProjectileType<AcidicLancherProj>();
			Item.shootSpeed = 25f;
            Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.consumable = true;
			Item.maxStack = 999;
		}

	}
}