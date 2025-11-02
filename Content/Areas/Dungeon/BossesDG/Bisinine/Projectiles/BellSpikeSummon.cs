using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BellSpikeSummon : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    float attackNum = 16;
                    float dist = 1500;
                    for(float i = 0; i < attackNum; i++)
                    {
                        float interpolant = i / attackNum;
                        Vector2 offset = Vector2.Lerp(-Vector2.UnitX * dist, Vector2.UnitX * dist, interpolant);
                        Vector2 position = Projectile.Center + offset;
                        Vector2 velocity = -Vector2.UnitY;
                        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
                        velocity *= Main.rand.NextFloat(500, 700);
                        int projType = ModContent.ProjectileType<BellSpike>();
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, projType, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

    }
}
