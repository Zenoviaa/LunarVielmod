using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
	public class Ripper : ModItem
    {
        public override void SetDefaults()
        {
			Item.damage = 8;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 62;
			Item.height = 54;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			
			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;

			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ProjectileType<PotOfGreedMinion>();
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			//Spawn at the mouse cursor position
			position = Main.MouseWorld;
			Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
			return false;
		}
    }

	public class RipperMinion : ModProjectile
	{
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
			Projectile.width = 18;
			Projectile.height = 28;
			// Makes the minion go through tiles freely
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;

			// Only determines the damage type
		//	Projectile.minion = true;
			Projectile.sentry = true;
			Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 1f;

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
			DrawHelper.AnimateTopToBottom(Projectile, 5);
			Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
		}
	}
}
