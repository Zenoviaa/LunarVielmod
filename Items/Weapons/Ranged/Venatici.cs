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
using Stellamod.Projectiles.Gun;
using Terraria.Audio;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Venatici : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Divine Sharpshooter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 41;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 43;
			Item.height = 10;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = 5;
			Item.knockBack = 2;
			Item.value = 100000;
			Item.rare = 2;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/TON618");
			Item.autoReuse = false;
			Item.shoot = ProjectileType<Venbullet>();
			Item.shootSpeed = 20f;


		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, 4);
		}

	
	}
}
