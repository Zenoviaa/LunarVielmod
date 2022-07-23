using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Powders;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PowdersItem
{
	internal class FlamePowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame Powder");
			Tooltip.SetDefault("Throw magical dust on them!" +
				"\nA flamey dust that explodes with your igniter!");
		}
		public override void SetDefaults()
		{
			Item.damage = 3;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = Item.buyPrice(0, 50, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType <FlamePowderProj>();
			Item.autoReuse = true;
			Item.shootSpeed = 16f;
			Item.crit = 7;
			Item.UseSound = SoundID.AbigailAttack;
		}

	
	}
}