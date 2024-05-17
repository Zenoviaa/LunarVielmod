using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviTornadoProj : ModProjectile
    {
        internal PrimitiveTrail BeamDrawer;

        public override string Texture => TextureRegistry.EmptyTexture;

        private ref float Timer => ref Projectile.ai[0];
        private float LifeTime => 600;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Timer++;
            Vector2 velocity = Projectile.velocity;
            float points = 8;
            float rotation = velocity.ToRotation();
            float xRadius = 24;
            float yRadius = 384;
            float angle = MathHelper.TwoPi * 4;

            for (float i = 0; i < points; i++)
            {
                float progress = i / points;
                float xOffset = xRadius * MathF.Cos(progress * angle) * progress;
                float yOffset = yRadius * MathF.Sin(progress * angle) * progress;
                Vector2 pointOnOval = Projectile.Center + new Vector2(xOffset, yOffset).RotatedBy(rotation);
                pointOnOval += velocity * 512 * progress;

                progress = (i + 1f) / points;
                xOffset = xRadius * MathF.Cos(progress * angle) * progress;
                yOffset = yRadius * MathF.Sin(progress * angle) * progress;
                Vector2 nextPointOnOval = Projectile.Center + new Vector2(xOffset, yOffset).RotatedBy(rotation);
                nextPointOnOval += velocity * 512 * progress;

                Vector2 tornadoVelocity = pointOnOval.DirectionTo(nextPointOnOval) * 32 * progress;
              //  Dust.NewDustPerfect(nextPointOnOval, ModContent.DustType<TSmokeTornadoDust>(), tornadoVelocity, 0, Color.Lerp(Color.WhiteSmoke, Color.DarkGray, progress), MathHelper.Lerp(0.4f, 0.7f, progress)).noGravity = true;
                ParticleManager.NewParticle<TornadoParticle>(nextPointOnOval, tornadoVelocity, Color.Lerp(Color.WhiteSmoke, Color.DarkGray, progress), MathHelper.Lerp(0.4f, 0.7f, progress));
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
