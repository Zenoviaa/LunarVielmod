using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Items.Materials;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;


namespace Stellamod.Items.weapons.melee
{
	public class VerstiDance : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Verstidance");
			Tooltip.SetDefault("An Arcanal Weapon!" +
				"\nSwirl!" +
			"\nHitting foes with the melee swing builds damage towards the swing of the weapon");
		}


		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.mana = 2;
			Item.height = 32;
			Item.useTime = 500;
			Item.useAnimation = 500;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.autoReuse = false;
			Item.value = Item.sellPrice(0, 0, 0, 20);
			Item.shoot = ModContent.ProjectileType<PetalDance>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}
		


	}
}