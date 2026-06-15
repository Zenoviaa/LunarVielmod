using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    public class IrradiaLaserBoxProj : ModProjectile
    {

        public const float LaserLength = 2400f;
        private float LifeTime => 1080;
        private ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
            if (Projectile.timeLeft > 60)
            {
                Timer++;
                if (Timer >= 60)
                {
                    Timer = 60;
                }
            }
            else
            {
                Timer--;
            }
            CreateDustAtBeginning();
        }

        public void CreateDustAtBeginning()
        {
            for (int i = 0; i < 6; i++)
            {
                Dust fire = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), DustID.SpectreStaff);
                fire.velocity = -Vector2.UnitY * Main.rand.NextFloat(1.5f, 3.25f);
                fire.velocity *= Main.rand.NextBool(2).ToDirectionInt();
                fire.scale = 1f + fire.velocity.Length() * 0.1f;
                fire.color = Color.Lerp(Color.White, Color.OrangeRed, Main.rand.NextFloat());
                fire.noGravity = false;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center - new Vector2(0, LaserLength / 2);
            Vector2 end = start + Vector2.UnitY * (LaserLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }
    }
}
