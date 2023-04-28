using Stellamod.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs
{
	public class DeathmultiBL : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (npc.HasBuff(ModContent.BuffType<DeathMultiplierBloodLamp>()))
			{
				damage = (int)(damage * 2.1f);
			}
		}
	}
}