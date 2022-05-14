﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.UI.systems;
using Stellamod.Buffs;

namespace Stellamod.UI.systems
{
	// Here is a class dedicated to showcasing projectile modifications
	public class BloodLampProjectileModifications : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		
		public bool applyBuffOnHit;
		public bool sayTimesHitOnThirdHit;
		// These are set when the user specifies that they want a trail.
		private Color trailColor;
		private bool trailActive;

		// Here I have a method for setting the above fields.
		public void SetTrail(Color color)
		{
			trailColor = color;
			trailActive = true;
		}
		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
			if (sayTimesHitOnThirdHit)
			{
				ProjectileModificationGlobalNPC globalNPC = target.GetGlobalNPC<ProjectileModificationGlobalNPC>();
				if (globalNPC.timesHitByModifiedProjectiles % 3 == 0)
				{
					Main.NewText($"This NPC has been hit with a modified projectile {globalNPC.timesHitByModifiedProjectiles} times.");
				}
				target.GetGlobalNPC<BLProjectileModificationGlobalNPC>().timesHitByModifiedProjectiles += 1;
			}

			if (applyBuffOnHit)
			{
				target.AddBuff(ModContent.BuffType<DeathmultiplierBloodLamp>(), 480);
			}
		}

	}
}