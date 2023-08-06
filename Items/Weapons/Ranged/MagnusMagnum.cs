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

namespace Stellamod.Items.Weapons.Ranged
{
	public class MagnusMagnum : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Divine Sharpshooter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 27;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 100000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item36;
			Item.autoReuse = false;
			Item.shoot = ProjectileID.Bullet;
			Item.shootSpeed = 35f;
			Item.useAmmo = AmmoID.Bullet;


		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

	}
}
