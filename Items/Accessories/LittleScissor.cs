using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Projectiles.Summons.VoidMonsters;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
	public class LittleScissorDashPlayer : ModPlayer
	{
		private int _riftDurationCounter;
		private int _riftCounter;
		// These indicate what direction is what in the timer arrays used
		public const int DashDown = 0;
		public const int DashUp = 1;
		public const int DashRight = 2;
		public const int DashLeft = 3;

		public const int DashCooldown = 25; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
		public const int DashDuration = 5; // Duration of the dash afterimage effect in frames
		public const int RiftDuration = 30;
		// The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
		public const float DashVelocity = 12f;

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
			else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
			{
				DashDir = DashUp;
			}
			else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
			{
				DashDir = DashRight;
			}
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
			{
				DashDir = DashLeft;
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
				_riftDurationCounter = RiftDuration;
				DashDelay = DashCooldown;
				DashTimer = DashDuration;
				Player.velocity = newVelocity;

				//X Slash Visuals
				var xSlashPart1 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
					ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0, owner: Player.whoAmI);
				var xSlashPart2 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
					ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0, owner: Player.whoAmI);

				xSlashPart1.timeLeft = 1500;
				xSlashPart2.timeLeft = 1500;
				xSlashPart2.rotation = MathHelper.ToRadians(90);

				//Actual Attack Here
				var voidRiftProjectile1 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
					ModContent.ProjectileType<VoidRift>(), 120, 1, owner: Player.whoAmI);

				voidRiftProjectile1.timeLeft = 180;
				voidRiftProjectile1.rotation = MathHelper.ToRadians(-45);

				var voidRiftProjectile2 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
					ModContent.ProjectileType<VoidRift>(), 120, 1, owner: Player.whoAmI);

				voidRiftProjectile2.timeLeft = 180;
				voidRiftProjectile2.rotation = MathHelper.ToRadians(45);

				// Here you'd be able to set an effect that happens when the dash first activates
				// Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
			}

			if (DashDelay > 0)
				DashDelay--;

			if (DashTimer > 0)
			{ // dash is active
			  // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
			  // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
			  // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
				Player.eocDash = DashTimer;
				Player.armorEffectDrawShadowEOCShield = true;

				// count down frames remaining
				DashTimer--;

			}

			_riftCounter--;
			_riftDurationCounter--;
			if (_riftDurationCounter > 0 && _riftCounter <= 0)
			{
				Vector2 velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
				var proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, velocity,
					ModContent.ProjectileType<LittleScissorVoidBolt>(), 54, 1, owner: Player.whoAmI);

				//Scale with all damage classe
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaRiftClose"), Player.position);
				_riftCounter = 10;
			}
		}

		private bool CanUseDash()
		{
			return DashAccessoryEquipped
				&& Player.dashType == 0 // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
				&& !Player.setSolar // player isn't wearing solar armor
				&& !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird

		}
	}

	public class LittleScissor : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 46;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Expert;
			Item.accessory = true;
			Item.expert = true;
		}

		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
		}

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped; 
        }
    
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<DashPlayer>().OneDashAccessoryEquipped = true;
            player.GetModPlayer<LittleScissorDashPlayer>().DashAccessoryEquipped = true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
			return true;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			//The below code makes this item hover up and down in the world
			//Don't forget to make the item have no gravity, otherwise there will be weird side effects
			float hoverSpeed = 5;
			float hoverRange = 0.2f;
			float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
			Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
			Item.position = position;
		}
	}
}