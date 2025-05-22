using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{


        internal class BucketScrapperMinionProj : ModProjectile
        {
            private static float _orbitingOffset;
            public override void SetStaticDefaults()
            {
                ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
                ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
                ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
                // These below are needed for a minion
                // Denotes that this projectile is a pet or minion
                Main.projPet[Projectile.type] = true;
                // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
                ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
                // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
                ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            }

            public override void SetDefaults()
            {
                Projectile.width = 68;
                Projectile.height = 70;

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
                Projectile.localNPCHitCooldown = 30;
            }

            public override bool? CanCutTiles()
            {
                return true;
            }

            public override bool MinionContactDamage()
            {
                return true;
            }

            private Vector2 CalculateCirclePosition(Player owner)
            {
                //Get the index of this minion
                int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

                //Now we can calculate the circle position	
                int minionCount = owner.ownedProjectileCounts[Type];
                float degreesBetweenFirefly = 360 / (float)minionCount;
                float degrees = degreesBetweenFirefly * minionIndex;
                float circleDistance = 96f;
                Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitingOffset));
                return circlePosition;
            }


            public override void AI()
            {
                Player owner = Main.player[Projectile.owner];
                if (!SummonHelper.CheckMinionActive<BucketScrapperMinionBuff>(owner, Projectile))
                    return;

                SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
                if (!foundTarget)
                {
                    Vector2 circlePosition = CalculateCirclePosition(owner);
                    float speed = 48;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, VectorHelper.VelocitySlowdownTo(Projectile.Center, circlePosition, speed), 0.1f);
                }
                else
                {
                    float speed = 20;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter, speed), 0.1f);
                }

                Visuals();
            }

            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                direction = direction.RotatedByRandom(MathHelper.ToRadians(30));
                Projectile.velocity = -direction * 71;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2") with { PitchVariance = 0.1f });
            }

            public override bool PreDraw(ref Color lightColor)
            {
                DrawHelper.DrawAdditiveAfterImage(Projectile, Color.DarkCyan, Color.Transparent, ref lightColor);
                return base.PreDraw(ref lightColor);
            }

            private void Visuals()
            {
                Player owner = Main.player[Projectile.owner];
                int minionCount = owner.ownedProjectileCounts[Type];

                _orbitingOffset += 0.3f;
                Projectile.rotation += MathHelper.ToRadians(2 + minionCount);
                Projectile.rotation += Projectile.velocity.Length() * 0.02f;
                // Some visuals here
                Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
            }
        }
    
}
