using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Paint;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
	internal class JacksonPollock : ModItem
    {
		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.knockBack = 3f;
			Item.mana = 20;
			Item.width = 76;
			Item.height = 80;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
		//	Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;

			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<JacksonPollockMinion>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Spawn at the mouse cursor position
			position = Main.MouseWorld;

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			player.UpdateMaxTurrets();
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20)
				.AddIngredient(ModContent.ItemType<ArtisanBar>(), 5)
				.AddIngredient(ItemID.WaterBucket, 1)
				.AddIngredient(ItemID.LavaBucket, 1)
				.AddIngredient(ItemID.HoneyBucket, 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}

    public class JacksonPollockMinion : ModProjectile
    {
		private int _counter;
		private const int Time_Between_Spills = 15;
		private const int Spill_Count = 3;
		public override void SetStaticDefaults()
		{
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 46;
			// Makes the minion go through tiles freely
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;

			// Only determines the damage type
			//Projectile.minion = true;
			Projectile.sentry = true;
			Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 0f;

			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			return false;
		}

		public override void AI()
		{
			_counter++;
			if(_counter > Time_Between_Spills)
			{
				Player owner = Main.player[Projectile.owner];
				for (int i = 0; i < Spill_Count; i++)
                {
					float x = Main.rand.NextFloat(-32f, 32f);
					float y = 16;

					Vector2 randOffset = new Vector2(x, y);
					Vector2 velocity = VectorHelper.VelocityDirectTo(
						Projectile.Center, 
						Projectile.Center + randOffset, 4);

					Projectile projectile = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.Center, velocity,
						ModContent.ProjectileType<JacksonPollockProj>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
					projectile.DamageType = DamageClass.Summon;
				}

				_counter = 0;
            }

			Visuals();
		}

		public override bool PreDraw(ref Color lightColor)
		{
			DrawHelper.DrawAdditiveAfterImage(Projectile, Main.DiscoColor, Color.Black, ref lightColor);
			return true;
		}

		private void Visuals()
		{
			float hoverSpeed = 5;
			float rotationSpeed = 2.5f;
			float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
			float rotation = VectorHelper.Osc(MathHelper.ToRadians(-5), MathHelper.ToRadians(5), rotationSpeed);
			Projectile.velocity = new Vector2(0, yVelocity);
			Projectile.rotation = rotation + MathHelper.ToRadians(180);

			//It needs to make two of those particles
			//Then have a delay before actually enabling the AI and void rift particle
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.28f);
		}
	}
}
