
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class Kilvier : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Throwing;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.damage = 67;

			Item.knockBack = 15;
		}
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("This sword feels warm, what is this Materials?"); // The (English) text shown below your weapon's name.

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.width = 40; // The item texture's width.
			Item.height = 40; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = 60; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 60; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.shoot = ModContent.ProjectileType<IrradiatedGreatBladeThrow>();
			Item.shootSpeed = 10f;

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 52; // The damage your item deals.
			Item.knockBack = 12; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 62; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

			Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1; // The sound when the weapon is being used.
			Item.noUseGraphic = true;
		}
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.NextBool(1))
			{
				// Emit dusts when the sword is swung
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.SilverCoin);
			}
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}
}