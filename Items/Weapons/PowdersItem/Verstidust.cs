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
	internal class Verstidust : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Versti Powder");
			Tooltip.SetDefault("Throw magical dust on them!" +
				"\nA musical dust that has calming flowers");
		}
		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<FlowerPowder>();
			Item.autoReuse = true;
			Item.shootSpeed = 28f;
			Item.crit = 26;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Lenabee");
		}


	}
}