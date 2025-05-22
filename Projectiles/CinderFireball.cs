using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class CinderFireball : ModProjectile
    {
        private ref float ai_Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 16;
            Projectile.width = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            ai_Timer++;
            Projectile.velocity.Y += 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }


        private void Visuals()
        {
            float radius = 1 / 6f;
            for (int i = 0; i < 2; i++)
            {
                float speedX = Main.rand.NextFloat(-radius, radius);
                float speedY = Main.rand.NextFloat(-radius, radius);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork,
                    speedX, speedY, Scale: scale);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }
    }
}
