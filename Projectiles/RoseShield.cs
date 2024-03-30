using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Brooches;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class RoseShield : ModProjectile
    {
        private float _counter;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 54;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            BroochPlayer broochPlayer = owner.GetModPlayer<BroochPlayer>();
            if (!broochPlayer.hasRoseBrooch)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            Rectangle myRect = Projectile.getRect();
            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.hostile)
                {
                    Rectangle otherRect = p.getRect();
                    if(Projectile.Colliding(myRect, otherRect) && p.active)
                    {
                        for (int t = 0; t < 32; t++)
                        {
                            Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                            ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<FabledParticle5>(),
                                default(Color), 1 / 3f);
                        }

                        SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.position);
                        p.Kill();
                    }
                }
            }

            Visuals();
        }

        private void Visuals()
        {
            _counter++;
            Player owner = Main.player[Projectile.owner];
            float circleDistance = 64f * VectorHelper.Osc(0.75f, 1f, 5f);
            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(_counter*0.5f));
            Vector2 circleDirection = (circlePosition - owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.Center = circlePosition;
            Projectile.rotation = circleDirection.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
