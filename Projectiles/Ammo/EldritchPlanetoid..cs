using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Ammo
{
    internal class EldritchPlanetoid : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.penetrate = 2;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.03f);
            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.friendly && p.type != Projectile.type)
                {
                    Rectangle otherRect = p.getRect();
                    if (Projectile.Colliding(myRect, otherRect) && p.active)
                    {
                        for (int t = 0; t < 3; t++)
                        {
                            Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                            ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<IceyParticle>(),
                                default(Color), 1 / 3f);
                        }

                        //Shoot the projectile
                        NPC npc = FindClosestNPC(float.MaxValue);
                        if (npc != null)
                        {
                            Vector2 velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                                ModContent.ProjectileType<EldritchBolt>(), (int)((float)Projectile.damage * 1.25f), Projectile.knockBack, Projectile.owner);
                        }

                        for (int j = 0; j < 8; j++)
                        {
                            Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                            var d = Dust.NewDustPerfect(Projectile.Center, DustID.UnusedWhiteBluePurple, speed, Scale: 3f);
                            d.noGravity = true;
                        }

                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon") { PitchVariance = 0.15f }, Projectile.position);
                        Projectile.Kill();
                        break;
                    }
                }
            }

            //Animate It
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(93, 203, 243), Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }


        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}
