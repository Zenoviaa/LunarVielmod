using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class CarrotPatrolProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green);
          
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkGreen * 0.3f, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for(int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                Vector2 velocity = -Projectile.velocity;
                velocity *= Main.rand.NextFloat(0.2f, 1f);
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity,
                    ModContent.ProjectileType<CarrotPatrolShardProj>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
