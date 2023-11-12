using Stellamod.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.UI.Systems
{
    // Here is a class dedicated to showcasing projectile modifications
    public class BloodLampProjectileModifications : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		public bool applyBuffOnHit;
		public bool sayTimesHitOnThirdHit;
		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
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
				target.AddBuff(ModContent.BuffType<DeathMultiplierBloodLamp>(), 480);
			}
		}
	}
}