using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Summons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class Valtotude : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
			Item.damage = 21;
        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.CloneDefaults(ItemID.Arkhalis);
			Item.damage = 7; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.DamageType = DamageClass.Summon;
			Item.mana = 20;
			Item.useTime = 90; // The Item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 90; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true; //so the Item's animation doesn't do damage
			Item.knockBack = 0; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.value = 10000; // how much the Item sells for (measured in copper)
			Item.UseSound = SoundID.Item11; // The sound that this Item plays when used.
			Item.autoReuse = false; // if you can hold click to automatically use it again
			Item.shoot = ModContent.ProjectileType<VoltuxPortal>();
			Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
			Item.channel = true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .52f);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
			position = Main.MouseWorld;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 2);
			recipe.AddIngredient(ItemID.AntlionMandible, 10);
			recipe.Register();
		}
	}
}