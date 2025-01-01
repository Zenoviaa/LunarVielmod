using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Pearlinator : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Pealet", "(D) Really low scaling damage")
			{
				OverrideColor = new Color(108, 271, 99)

			};
			tooltips.Add(line);

			base.ModifyTooltips(tooltips);
		}

        public override void SetClassSwappedDefaults()
        {
			Item.damage = 4;
			Item.mana = 4;
        }

        public override void SetDefaults()
		{
			Item.width = 62;
			Item.height = 32;
			Item.scale = 0.9f;
			Item.rare = ItemRarityID.Blue;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item34;

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 8;
			Item.knockBack = 4;
			Item.noMelee = true;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<Flameball>();
			Item.shootSpeed = 8f;
			// Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
			Item.value = 5000;
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(0f, -2f);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<Flameball2>() });
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }
}