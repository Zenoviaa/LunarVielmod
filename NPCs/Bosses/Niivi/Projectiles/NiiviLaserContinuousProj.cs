using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviLaserContinuousProj : ModProjectile
    {
        internal PrimitiveTrail BeamDrawer;

        public override string Texture => TextureRegistry.EmptyTexture;

        private float LaserLength => 4800;
        private float LifeTime => 600;
        private ref float Timer => ref Projectile.ai[0];
        private NPC Owner => Main.npc[(int)Projectile.ai[1]];
        private ref float ExtraWidth => ref Projectile.ai[2];
        private float Blend = 0.05f;
        private float DrawMode = 0;
        private Vector2 ScrollSpeed = new Vector2(0.0001f, 0.0001f);
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {

            ShakeModSystem.Shake = 3f;
            ExtraWidth += 0.03f;
            Projectile.Center = Owner.Center;
            Projectile.velocity = Owner.rotation.ToRotationVector2();
            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);

            Projectile.scale = MathF.Sin(Timer / 600f * MathHelper.Pi) * 3f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;


            // And create bright light.
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.5f);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player npc = Main.player[i];

                if (npc.active)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= 4000)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction * 0.15f;
                    }
                }
            }

            Timer++;
            if(Timer == 1)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_PrimRay, Projectile.position);
            }

            if(Timer % 4 == 0)
            {
                float starRadius = 1024;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(starRadius, starRadius);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 16;
                    ParticleManager.NewParticle<StarParticle>(pos, vel, Color.White, 1f);
                }
            }
            if (Timer % (int)(LifeTime / 4) == 0)
            {
                //Spawn the things
                Vector2 spikeVelocity = Projectile.velocity;
                float spawnNum = 8;
                for (int i = 0; i < spawnNum; i++)
                {
                    float progress = (float)i / spawnNum;
                    float rot = MathHelper.Lerp(0f, MathHelper.TwoPi, progress);
                    spikeVelocity = spikeVelocity.RotatedBy(rot);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, spikeVelocity * 1048,
                        ModContent.ProjectileType<NiiviLaserSpikeProj>(), Projectile.damage / 5, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * (LaserLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            float mult = 1;
            if (Projectile.timeLeft < 60)
            {
                mult = Projectile.timeLeft / (float)60;
            }

            float extraWidth = MathHelper.Lerp(1f, 1f + ExtraWidth, completionRatio);
            if (DrawMode == 1)
                extraWidth *= 0.33f;
            return Projectile.width * Projectile.scale * 1.3f * mult * extraWidth;
        }

        public override bool ShouldUpdatePosition() => false;
        public Color ColorFunction(float completionRatio)
        {
            return Main.DiscoColor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.White);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.WhispyTrail);
            DrawMode = 0;
            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
  
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, i / 8f));
            }

            BeamDrawer.Draw(points, -Main.screenPosition, 32);

      
            float numOrbPoints = 4;
            float orbRadius = 32;
            float numOrbTrails = 32;


            DrawMode = 1;
            for(float j = 0; j < numOrbTrails; j++)
            {
                points.Clear();
                for (float i = 0; i < numOrbPoints; i++)
                {
                    float progress = i / numOrbPoints;
                    Vector2 start = Projectile.Center;
                    Vector2 velocity = Projectile.velocity.RotatedBy(MathHelper.TwoPi * (j / numOrbTrails)) * orbRadius;
                    Vector2 end = Projectile.Center + velocity.RotatedBy(MathHelper.PiOver2 * progress);
                    points.Add(Vector2.Lerp(start, end, progress));
                }
                BeamDrawer.Draw(points, -Main.screenPosition, 32);

            }


            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
