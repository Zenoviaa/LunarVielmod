using Stellamod.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs
{
    public class DeathmultiBL : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			if (npc.HasBuff(ModContent.BuffType<DeathMultiplierBloodLamp>()) && projectile.CountsAsClass(DamageClass.Summon))
			{
				projectile.damage = (int)(projectile.damage * 2.1f);
			}
		}
	}
}