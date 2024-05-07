using Microsoft.Xna.Framework;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.Projectiles.Whips;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Whipfx
{
    public class ColdheartAnklebiterDebuff : ModBuff
    {
        public static readonly int TagDamage = 4;

        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
            BuffID.Sets.CanBeRemovedByNetMessage[Type] = true;
        }
    }


    public class ColdheartAnklebiterDebuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {           
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;

            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<ColdheartAnklebiterDebuff>())
            {
                // Apply a flat bonus to every hit
                modifiers.FlatBonusDamage += ColdheartAnklebiterDebuff.TagDamage * projTagMultiplier;
                npc.RequestBuffRemoval(ModContent.BuffType<ColdheartAnklebiterDebuff>());

                //spawn icicles
                for(int i = 0; i < Main.rand.Next(1, 3); i++)
                {
                    float speed = 12;
                    Vector2 velocity = -Vector2.UnitY * speed;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 * 1.5f);

                    int type = ModContent.ProjectileType<ColdheartAnklebiterIcicleProj>();
                    int damage = projectile.damage / 2;
                    float knockback = projectile.knockBack / 2;
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity,
                        type, damage, knockback, projectile.owner);
                }
            }
        }
    }
}
