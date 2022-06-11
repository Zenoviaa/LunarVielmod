using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.Projectiles.PocketProj;
using Stellamod.Projectiles.Powders;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.PocketDust
{
	internal class PocketSand : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pocket Sand");
			Tooltip.SetDefault("Throw magical dust on them!" +
				"\nDust that can be used with and for combos in igniters" +
				"\n Can penetrate armored enemies like nothing!");
		}
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HiddenAnimation;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<PocketSandProj>();
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.ArmorPenetration = 300;
			
			Item.crit = 12;
			Item.UseSound = SoundID.Grass;
		}

	
	}
}