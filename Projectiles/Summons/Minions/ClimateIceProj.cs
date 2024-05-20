using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    internal class ClimateIceProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 595;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            float distanceFromTarget = 128;
            bool foundTarget = false;
            Vector2 targetCenter = Vector2.Zero;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;

                float between = Vector2.Distance(npc.Center, Projectile.Center);
                bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                bool inRange = between < distanceFromTarget;

                if (closest && inRange || !foundTarget)
                {
                    foundTarget = true;
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                }
            }

            if (foundTarget)
                Projectile.velocity = VectorHelper.VelocityHomingTo(Projectile.position, Projectile.velocity, targetCenter, 0.5f);
            for (int j = 0; j < 5; j++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                var particle = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<IceyParticle>(), Color.White, Main.rand.NextFloat(.2f, .4f));
                particle.timeLeft = 12;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            target.AddBuff(BuffID.Frozen, 120);
            target.AddBuff(BuffID.Frostburn, 120);
        }


        public override void OnKill(int timeLeft)
        {
            //Explosion
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer3"), Projectile.position);

            int count = Main.rand.Next(10, 30);
            for (int j = 0; j < count; j++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                ParticleManager.NewParticle(Projectile.Center, speed * 8, ParticleManager.NewInstance<IceyParticle>(), Color.White, Main.rand.NextFloat(.3f, .6f));
            }

            //Explosion?
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<ClimateIceProjExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
