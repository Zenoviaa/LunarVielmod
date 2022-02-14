using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.weapons.ranged
{
	public class Violar : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Let them burn in harmony!" +
				"\nSimple weapon forged from Stellean bricks and the heat from plants of the morrow...");
			DisplayName.SetDefault("Violiar");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{

			Item.width = 40;
			Item.height = 10;
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Green;


			Item.useTime = 100;
			Item.useAnimation = 100;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.autoReuse = true;

			Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/violar");

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 18;
			Item.knockBack = 5f;
			Item.noMelee = true;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<violarproj>();
			Item.shootSpeed = 4f;
		}


		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}
		
	}
}












