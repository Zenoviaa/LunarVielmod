using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;

using Microsoft.Xna.Framework;

using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;

namespace Stellamod.Items.Weapons.Mage
{
	public class JellyTome : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jelly Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;

			Item.autoReuse = true;
			Item.shoot = ProjectileType<Gelatin>();
			Item.shootSpeed = 6f;
			Item.mana = 15;


		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}