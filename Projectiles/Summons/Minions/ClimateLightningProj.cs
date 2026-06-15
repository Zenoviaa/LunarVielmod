using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class ClimateLightningProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private int Seed
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private float Lifetime => 24;
        private Vector2[] LightningPos;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void AI()
        {
            if (Seed != 0)
            {
                //Calculate
                UnifiedRandom random = new UnifiedRandom(Seed);
                List<Vector2> points = new List<Vector2>();
                Vector2 currentPoint = Projectile.Center;
                points.Add(currentPoint);

                int numPoints = 24;
                for (int i = 0; i < numPoints; i++)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    direction = direction.RotatedByRandom(MathHelper.ToRadians(30));
                    float distance = random.NextFloat(2, Projectile.velocity.Length());
                    currentPoint = currentPoint + direction * distance;
                    points.Add(currentPoint);
                }

                LightningPos = points.ToArray();
                Seed = 0;
            }

            Timer++;
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                Seed = Main.rand.Next(1, int.MaxValue);
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = LightningPos;
            if (positions == null)
                return false;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 1, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
