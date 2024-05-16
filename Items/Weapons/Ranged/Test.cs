using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.NPCs.Catacombs.Trap.Sparn;
using Stellamod.NPCs.Catacombs.Water.WaterCogwork;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Chains;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.Test;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Test : ModItem
	{
		private int _dir = 1;
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
			// DisplayName.SetDefault("Teraciz");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 62;
			Item.height = 32;
			Item.scale = 0.9f;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 19;
			Item.knockBack = 0;
			Item.noMelee = true;
			Item.noUseGraphic = true;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<GothDarkBlastProj>();
			Item.shootSpeed = 5;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            _dir = -_dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: _dir);
            return false;
        }
    }
}