using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class JellyBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jelly Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.shootSpeed = 15f;
			Item.useAmmo = AmmoID.Arrow;


		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}