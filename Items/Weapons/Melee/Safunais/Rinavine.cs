using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Safunai.Parendine;
using Stellamod.Projectiles.Safunai.Rinavine;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Safunais
{
	public class Rinavine : ModItem
	{
		public int combo;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Halhurish The Flamed"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Whip your opponents in the air" +
				"\nHitting enemies will explode"); */
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Parandinea",  Helpers.LangText.Common("Safunai"))
			{
				OverrideColor = new Color(308, 71, 99)

			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Parendinea", "(B) Medium Damage Scaling (Grail shot) On Hit!")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);



		}
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = Item.useAnimation = 18;
			Item.shootSpeed = 1f;
			Item.knockBack = 4f;
			Item.UseSound = SoundID.Item116;
			Item.shoot = ModContent.ProjectileType<RinavineProj>();
			Item.value = Item.sellPrice(gold: 10);
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 76;
			Item.rare = ItemRarityID.LightRed;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			combo++;

			float distanceMult = Main.rand.NextFloat(0.8f, 1.2f);
			float curvatureMult = 0.7f;

			bool slam = combo % 5 == 4;

			Vector2 direction = velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
			Projectile proj = Projectile.NewProjectileDirect(source, position, direction, type, damage, knockback, player.whoAmI);

			if (proj.ModProjectile is RinavineProj modProj)
			{
				modProj.SwingTime = (int)(Item.useTime * UseTimeMultiplier(player) * (slam ? 1.75f : 1));
				modProj.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult;
				modProj.Curvature = 0.33f * curvatureMult;
				modProj.Flip = combo % 2 == 1;
				modProj.Slam = slam;
				modProj.PreSlam = combo % 5 == 3;
			}

			return false;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);

			recipe.AddIngredient(ModContent.ItemType<BlankSafunai>(), 1);
			recipe.AddIngredient(ItemID.Chain, 15);
			recipe.AddIngredient(ModContent.ItemType<GrailBar>(), 5);
			recipe.AddIngredient(ItemID.FallenStar, 10);


			recipe.Register();
		}
		public override float UseTimeMultiplier(Player player) => player.GetAttackSpeed(DamageClass.Melee); //Scale with melee speed buffs, like whips
		public override void NetSend(BinaryWriter writer) => writer.Write(combo);
		public override void NetReceive(BinaryReader reader) => combo = reader.ReadInt32();
	}
}
