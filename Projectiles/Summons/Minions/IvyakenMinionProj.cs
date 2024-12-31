using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;

namespace Stellamod.Projectiles.Summons.Minions
{
    /*
		 * This minion shows a few mandatory things that make it behave properly. 
		 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
		 * If the player targets a certain NPC with right-click, it will fly through tiles to it
		 * If it isn't attacking, it will float near the player with minimal movement
		 */
    public class IvyakenMinionProj : ModProjectile
    {
        Player Owner => Main.player[Projectile.owner];
        ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ivyaken");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 14;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Projectile.width = 18;
            Projectile.height = 28;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Poisoned, 180);
        }

        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<IvyakenMinionBuff>(Owner, Projectile))
                return;

            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);
            if (foundTarget)
            {
                Timer++;
                if (Timer == 1)
                {
                    Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
                    Projectile.velocity = directionToTarget * 24;

                }
                else
                {   // So it will lean slightly towards the direction it's moving
                    Projectile.rotation += Projectile.velocity.Length() * 0.05f;
                    Projectile.velocity *= 0.96f;
                }

                if (Timer == 30)
                {
                    Timer = 0;
                }
            }
            else
            {
                Timer = 0;
                SummonHelper.CalculateIdleValues(Owner, Projectile,
                    out Vector2 vectorToIdlePosition,
                    out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }

            Visuals();
        }

        private void Visuals()
        {

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Dirt);
                int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.JunglePlants);
                Main.dust[dust].noGravity = true;
                Main.dust[dust1].noGravity = true;
            }
        }
    }
}
