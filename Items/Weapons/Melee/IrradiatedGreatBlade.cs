using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Bow;
using Terraria.Audio;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Weapons.Melee
{
    public class IrradiatedGreatBlade : ModItem
    {
        public int WinterboundArrow;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Irradiated Great Blade"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ProjectileType<AcidBlast2>();
            Item.shootSpeed = 15f;
            Item.autoReuse = true;
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<VirulentPlating>(), 10);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 3);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

	}
}