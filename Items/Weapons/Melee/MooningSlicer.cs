using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Slashers.Voyager;
using Stellamod.Projectiles.Swords.Altride;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
	public class MooningSlicer : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Magic;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.mana = 5;
            Item.damage = 18;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Swing");
			/* Tooltip.SetDefault("Shoots one bone bolt to swirl and kill your enemies after attacking!" +
			"\nHitting foes with the melee swing builds damage towards the swing of the weapon"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 10;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.value = 100000;
			Item.shoot = ModContent.ProjectileType<MooningProj>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);

			recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 10);
			recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 9);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 12);

			recipe.Register();
		}
	}
}