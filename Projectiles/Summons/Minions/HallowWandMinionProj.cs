using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    // - ModProjectile - the minion itself

    // It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overwiew.
    // To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
    // This is NOT an in-depth guide to advanced minion AI

    // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement
    public class HallowWandMinionProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 9;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.damage = 15;

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.scale = 1f;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return true;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<HallowWandMinionBuff>(owner, Projectile))
                return;

            SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget)
            {
                float speed = 20f;
                float inertia = 20f;

                Vector2 targetCenterUP;
                targetCenterUP.Y = targetCenter.Y - 600;
                targetCenterUP.X = targetCenter.X;

                Vector2 Fdirection = targetCenterUP - Projectile.Center;
                Fdirection.Normalize();

                Projectile.ai[1]++;
                if (Projectile.ai[1] >= 14)
                {
                    var entitySource = Projectile.GetSource_FromThis();
                    float xVelocity = Main.rand.NextFloat(-2f, 2f);
                    float yVelocity = Main.rand.NextFloat(-12f, 0f);
                    Vector2 velocity = new Vector2(xVelocity, yVelocity);
                    Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, velocity.X, velocity.Y,
                        ModContent.ProjectileType<HallowRain>(), Projectile.damage, 1, Projectile.owner, 0, 0);

                    Projectile.ai[1] = 0;
                }

                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 100f)
                {
                    Fdirection.Normalize();
                    Fdirection *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + Fdirection) / inertia;
                }
            }
            else
            {
                //Return near player and idle.
                SummonHelper.CalculateIdleValues(owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }

            Visuals();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Yellow, Color.Transparent, ref lightColor);
            return true;
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 5);

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}