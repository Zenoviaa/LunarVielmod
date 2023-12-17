using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    // ExampleCustomSwingSword is an example of a sword with a custom swing using a held projectile
    // This is great if you want to make melee weapons with complex swing behaviour
    // Note that this projectile only covers 2 relatively simple swings, everything else is up to you
    // Aside from the custom animation, the custom collision code in Colliding is very important to this weapon
    public class IronHookCustomSwingProjectile : ModProjectile
	{
		// We define some constants that determine the swing range of the sword
		// Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
		// You could change the values or even replace them entirely, but they are tweaked with looks in mind
		private const float SWINGRANGE = 2f * (float)Math.PI; // The angle a swing attack covers (300 deg)
		private const float FIRSTHALFSWING = 0.05f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)

		// Variables to keep track of during runtime
		private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
		private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
		private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
		private ref float Size => ref Projectile.localAI[2]; // Size of sword

		// We define timing functions for each stage, taking into account melee attack speed
		// Note that you can change this to suit the need of your projectile
		private float execTime => 16f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		public override string Texture => "Stellamod/Items/Weapons/Melee/IronHook"; // Use texture of item as projectile texture
		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 54; // Hitbox width of projectile
			Projectile.height = 48; // Hitbox height of projectile
			Projectile.friendly = true; // Projectile hits enemies
			Projectile.timeLeft = 10000; // Time it takes for projectile to expire
			Projectile.penetrate = -1; // Projectile pierces infinitely
			Projectile.tileCollide = false; // Projectile does not collide with tiles
			Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
			Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
			Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
			Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
		}


		public override void OnSpawn(IEntitySource source)
		{
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
			float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
			if (Projectile.spriteDirection == 1)
			{
				// However, we limit the rangle of possible directions so it does not look too ridiculous
				targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
			}
			else
			{
				if (targetAngle < 0)
				{
					targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
				}

				targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
			}

			InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			// Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Projectile.spriteDirection = reader.ReadSByte();
		}

		public override void AI()
		{
			// Extend use animation until projectile is killed
			Owner.itemAnimation = 2;
			Owner.itemTime = 2;

			// Kill the projectile if the player dies or gets crowd controlled
			if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
			{
				Projectile.Kill();
				return;
			}

			// AI depends on stage and attack
			// Note that these stages are to facilitate the scaling effect at the beginning and end
			// If this is not desireable for you, feel free to simplify
			ExecuteStrike();
			SetSwordPosition();
			Timer++;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			// Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
			Vector2 origin;
			float rotationOffset;
			SpriteEffects effects;

			if (Projectile.spriteDirection > 0)
			{
				origin = new Vector2(0, Projectile.height);
				rotationOffset = MathHelper.ToRadians(45f);
				effects = SpriteEffects.None;
			}
			else
			{
				origin = new Vector2(Projectile.width, Projectile.height);
				rotationOffset = MathHelper.ToRadians(135f);
				effects = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

			// Since we are doing a custom draw, prevent it from normally drawing
			return false;
		}

		// Find the start and end of the sword and use a line collider to check for collision with enemies
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
		}

		// Do a similar collision check for tiles
		public override void CutTiles()
		{
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// Make knockback go away from player
			modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
		}

		// Function to easily set projectile and arm position
		public void SetSwordPosition()
		{
			Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // Set projectile rotation

			// Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
			Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

			armPosition.Y += Owner.gfxOffY;
			Projectile.Center = armPosition; // Set projectile to arm position
			Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers
			Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
		}


		// Function facilitating the first half of the swing
		private void ExecuteStrike()
		{
			float time = Timer / execTime;
			float easedTime = EaseFunction.EaseQuadInOut.Ease(time);
			Progress = MathHelper.Lerp(0, SWINGRANGE, easedTime);
			Size = 1f;
			if (Timer >= execTime)
			{
				Projectile.Kill();
			}
		}
	}
}