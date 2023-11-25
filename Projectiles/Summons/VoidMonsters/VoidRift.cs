using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.VoidMonsters
{
    public class VoidRift : ModProjectile
    {
        private int _particleCounter;
        private const int Body_Particle_Count = 4;

        //Lower number = faster
        private const int Body_Particle_Rate = 2;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 150;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            //Local NPC hit time so it doesn't interfere with other piercing weps
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.PostDrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, new Color(60, 0, 118), lightColor, 1);
            DrawHelper.PreDrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            Visuals();
        }

        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {
                Rectangle rectangle = Projectile.getRect();
                for (int i = 0; i < Body_Particle_Count; i++)
                {

                    float x = Main.rand.Next(0, rectangle.Width);
                    float y = Main.rand.Next(0, rectangle.Height);
                    Vector2 position = Projectile.position + new Vector2(x, y);
                    position += new Vector2(-8, -16);
                    Particle p = ParticleManager.NewParticle(position, new Vector2(0, -2f), ParticleManager.NewInstance<VoidParticle>(),
                        default(Color), Main.rand.NextFloat(0.1f, 0.2f));
                    p.layer = Particle.Layer.BeforeProjectiles;
                }
                _particleCounter = 0;
            }

            DrawHelper.AnimateTopToBottom(Projectile, 3);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }
    }
}
