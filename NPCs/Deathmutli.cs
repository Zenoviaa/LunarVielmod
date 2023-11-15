using Stellamod.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs
{
    public class Deathmulti : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			if (npc.HasBuff(ModContent.BuffType<DeathMultiplier>()) && projectile.CountsAsClass(DamageClass.Melee))
			{
				projectile.damage = (int)(projectile.damage * 1.7f);
			}
		}
	}
}
