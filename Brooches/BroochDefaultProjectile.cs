using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Brooches
{
    /// <summary>
    /// Provides default implementation for brooch type follower, so you don't have to copy & paste code!
    /// <br>Override functions if you want it to work a bit differently</br>
    /// </summary>
    public abstract class BroochDefaultProjectile : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch Of speedy fish");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 1;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public override void SetDefaults()
		{
			Projectile.originalDamage = (int)0f;
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely
											// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
			Projectile.scale = 0.6f;
			Projectile.timeLeft = 1000;
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

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			Projectile.timeLeft = 2;
			SummonHelper.CalculateIdleValues(owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		public virtual void Visuals()
		{
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.05f;

			// This is a simple "loop through all frames from top to bottom" animation
			DrawHelper.AnimateTopToBottom(Projectile, 5);

			// Some visuals here
			
		}
	}
}
