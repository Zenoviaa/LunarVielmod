using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Players
{
    public class DashPlayer3 : ModPlayer
	{
		// These indicate what direction is what in the timer arrays used
		public const int DashDown = 0;
		public const int DashUp = 1;
		public const int DashRight = 2;
		public const int DashLeft = 3;

		public const int DashCooldown = 37; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
		public const int DashDuration = 30; // Duration of the dash afterimage effect in frames

		// The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
		public const float DashVelocity = 23f;

		// The direction the player has double tapped.  Defaults to -1 for no dash double tap
		public int DashDir = -1;

		// The fields related to the dash accessory
		public bool DashAccessoryEquipped;
		public int DashDelay = 0; // frames remaining till we can dash again
		public int DashTimer = 0; // frames remaining in the dash

		public override void ResetEffects()
		{
			// Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
			DashAccessoryEquipped = false;

			// ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
			// When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
			// If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
			if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
			{
				DashDir = DashDown;
			}
			else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
			{
				DashDir = DashRight;
			}
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
			{
				DashDir = DashLeft;
			}
			else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
			{
				DashDir = DashUp;
			}
			else
			{
				DashDir = -1;
			}
		}

		// This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
		// If they double tapped this frame, they'll move fast this frame
		public override void PreUpdateMovement()
		{
			// if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
			if (CanUseDash() && DashDir != -1 && DashDelay == 0)
			{
				Vector2 newVelocity = Player.velocity;

				switch (DashDir)
				{
					// Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
					case DashUp when Player.velocity.Y > -DashVelocity:
					case DashDown when Player.velocity.Y < DashVelocity:
						{
							// Y-velocity is set here
							// If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
							// This adjustment is roughly 1.3x the intended dash velocity
							float dashDirection = DashDir == DashDown ? 1 : -1.3f;
							newVelocity.Y = dashDirection * DashVelocity;
							break;
						}

			
					case DashLeft when Player.velocity.X > -DashVelocity:
					case DashRight when Player.velocity.X < DashVelocity:
						{
							// X-velocity is set here
							float dashDirection = DashDir == DashRight ? 1 : -1;
							newVelocity.X = dashDirection * DashVelocity;
							break;
						}
					default:
						return; // not moving fast enough, so don't start our dash
				}

				// start our dash
				DashDelay = DashCooldown;
				DashTimer = DashDuration;
				Player.velocity = newVelocity;
				// Here you'd be able to set an effect that happens when the dash first activates
				// Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
			}

			if (DashDelay > 0)
            {
				DashDelay--;
			}
				

			if (DashTimer > 0)
			{ // dash is active
				Vector2 newVelocity = Player.velocity;
				Player.GetModPlayer<ImmunityPlayer3>().HasStealiImmunityAcccc = true;
				Player.armorEffectDrawShadowEOCShield = true;
				for (int j = 0; j < 3; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Player.Center, speed * 4, ParticleManager.NewInstance<StarParticle>(), Color.DarkGoldenrod, Main.rand.NextFloat(0.2f, 0.8f));
				}

				for (int j = 0; j < 1; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Player.Center, speed * 6, ParticleManager.NewInstance<StarParticle>(), Main.DiscoColor, Main.rand.NextFloat(0.2f, 0.8f));
				}
				Player.AddBuff(BuffID.Cursed, 1);

				// count down frames remaining
				
				Player.velocity *= 0.98f;
				DashTimer--;
			}

		
			if (DashTimer == 0)
            {
				Player.GetModPlayer<ImmunityPlayer3>().HasStealiImmunityAcccc = false;
				

			}
		}

		private bool CanUseDash()
		{
			return DashAccessoryEquipped
				&& !Player.setSolar // player isn't wearing solar armor
				
				&& !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
		}
	}
}
