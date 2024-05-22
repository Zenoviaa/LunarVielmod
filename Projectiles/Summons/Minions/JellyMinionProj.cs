using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    /*
     * This minion shows a few mandatory things that make it behave properly. 
     * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
     * If the player targets a certain NPC with right-click, it will fly through tiles to it
     * If it isn't attacking, it will float near the player with minimal movement
     */
    public class JellyMinionProj : ModProjectile
    {
        private static float _orbitingOffset;
        Player Owner => Main.player[Projectile.owner];
        ref float Timer => ref Projectile.ai[0];
        ref float TimerOffset => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Minion");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<JellyMinionBuff>(Owner, Projectile))
                return;

            _orbitingOffset += 0.03f;
            Projectile.Center = CalculateCirclePosition(Owner);
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (foundTarget)
            {
                Timer++;
                if (Timer < 120)
                {
                    ChargeVisuals(Timer, 80);
                }

                if (Timer >= 120 + TimerOffset)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        TimerOffset = Main.rand.Next(0, 30);
                        Projectile.netUpdate = true;
                    }

                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
                    Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
                    Vector2 velocityToTarget = directionToTarget * 1;
                    int numProjectiles = Main.rand.Next(1, 3);
                    for (int p = 0; p < numProjectiles; p++)
                    {
                        // Rotate the velocity randomly by 30 degrees at max.
                        Vector2 newVelocity = velocityToTarget.RotatedByRandom(MathHelper.ToRadians(6));
                        newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity,
                            ModContent.ProjectileType<JellyStaffLightningProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    Timer = 0;
                }
            }
            else
            {

            }
            Visuals();
        }
        private void ChargeVisuals(float timer, float maxTimer)
        {

            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(64, 64);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    Dust d = Dust.NewDustPerfect(pos, DustID.Electric, vel, 0, Color.White);
                    d.noGravity = true;
                }
            }
        }
        private Vector2 CalculateCirclePosition(Player owner)
        {
            //Get the index of this minion
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int fireflyCount = owner.ownedProjectileCounts[Type];
            float degreesBetween = 360 / (float)fireflyCount;
            float degrees = degreesBetween * minionIndex;
            float circleDistance = 64;
            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitingOffset));
            return circlePosition;
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

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

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
