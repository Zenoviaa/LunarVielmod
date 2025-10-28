using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak.Projectiles
{
    public class WindStormDebris : ModProjectile,
        IDrawOutlines
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float FallDownTime => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                FallDownTime = Main.rand.NextFloat(80, 120);
                Projectile.netUpdate = true;
            }
            Projectile.velocity.Y += MathF.Sin(Timer * 0.2f) * 0.1f;
            Projectile.rotation += 0.02f;
            Projectile.rotation -= Projectile.velocity.Length() * 0.025f;
            if (Timer > FallDownTime)
            {
                Projectile.tileCollide = true;
                Projectile.velocity.Y += 1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 12; f++)
            {
                float rot = f / 12f * MathHelper.TwoPi;
                Vector2 velOffset = rot.ToRotationVector2() * 4;
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, velOffset, Scale: 1f);
            }
            for (float f = 0; f < 4; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(64, 64);
                FXUtil.GlowStretch(Projectile.Center, velocity);
            }
        }


        protected virtual void DrawWindTrail(ref Color lightColor)
        {

        }


        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            this.OutlineNoRestart(Color.Red, ref lightColor, Vector2.One);
        }
        public override bool PreDraw(ref Color lightColor)
        {
           
            this.DrawCentered(ref lightColor);
            return false;
        }

    }
}
