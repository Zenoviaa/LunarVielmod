using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Stellamod.Projectiles;

namespace Stellamod.Items.weapons.summon
{
	public class AlcadizDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alcadiz Dagger");
			Tooltip.SetDefault("Your summons will target focus enemies\nSummons will pop out of explosions and act as a temporary summon to give your other minions company!");
		}


		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 44;
			Item.rare = ItemRarityID.Green;
			Item.value = Terraria.Item.sellPrice(0, 5, 80, 0);
			Item.damage = 15;
			Item.knockBack = 2;
			Item.mana = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 60;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<PlantDagga>();
			Item.shootSpeed = 6;
			Item.UseSound = SoundID.Item1;
		}
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .52f);
			
		
		}
	}
}