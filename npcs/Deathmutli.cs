using Stellamod.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs
{
	public class Deathmulti : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (npc.HasBuff(ModContent.BuffType<DeathMultiplier>()) && projectile.CountsAsClass(DamageClass.Melee))
			{
				damage = (int)(damage * 1.7f);
			}
		}
	}
}
