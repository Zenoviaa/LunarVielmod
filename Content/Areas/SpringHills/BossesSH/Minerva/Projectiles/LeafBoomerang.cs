using Microsoft.Xna.Framework;
using Stellamod.Core;
using Stellamod.Core.Effects.Trails;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Minerva.Projectiles
{
    public class LeafBoomerang : ScarletProjectile
    {
        public SlashTrailer Trailer { get; set; }
        private ref float Timer => ref Projectile.ai[0];
        private ref float StartX => ref Projectile.ai[1];
        private ref float EndX => ref Projectile.ai[2];
        private Vector2 ShootVelocity;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 64;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                StartX = Projectile.velocity.X;
                EndX = -Projectile.velocity.X;
                Projectile.netUpdate = true;
            }

            if(Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
            }
            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture);
            }

            float interpolant = Timer / 120f;
            float ease = EasingFunction.InOutSine(interpolant);
            Projectile.velocity.X = MathHelper.Lerp(StartX, EndX, ease);
            Projectile.velocity.Y = 2;
            Projectile.rotation += 0.5f;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Trailer ??= new SlashTrailer();
            Trailer.DrawTrail(ref lightColor, OldCenterPos);
            this.Outline(Color.Red, ref lightColor);
            this.DrawCentered(ref lightColor);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
