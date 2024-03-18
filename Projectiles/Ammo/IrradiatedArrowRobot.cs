using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Ammo
{
    internal class IrradiatedArrowRobot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 14;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 180;
        }

        ref float Timer => ref Projectile.ai[0];
        public override void AI()
        {
            float hoverSpeed = 5;
            float hoverRange = 12f;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero + new Vector2(0, y), 0.04f);
            
            NPC npc = FindClosestNPC(600);
            if(npc != null)
            {
                Projectile.spriteDirection = npc.position.X < Projectile.position.X ? -1 : 1;
                Timer++;
                if(Timer > 92)
                {
                    Vector2 velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<IrradiatedBolt>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GraniteMagmum1") { PitchVariance=0.15f}, Projectile.position);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GraniteMagmum2") { PitchVariance = 0.15f }, Projectile.position);
                    }
                    Timer = 0;
                }
            }

            Lighting.AddLight(Projectile.Center, Color.Green.ToVector3() * 0.78f * Main.essScale);
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

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(48, 160, 94), Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int j = 0; j < 4; j++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = ParticleManager.NewParticle<morrowstar>(Projectile.Center, speed, Color.White, Scale: 1f);
            }
        }
    }
}
