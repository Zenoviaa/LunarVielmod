using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class StarRock : ScarletProjectile,
        IDrawOutlines
    {
        private Vector2 _squish;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _squish = new Vector2(0.5f, 1.5f);
                Projectile.frame = Main.rand.Next(0, Main.projFrames[Type]);
            }

            if (Timer % 5 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GlowSparkleDust>(), Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            _squish = Vector2.Lerp(_squish, Vector2.One, 0.1f);

            //Gravity
            if (Projectile.velocity.Y < 10)
            {
                Projectile.velocity.Y += 0.5f;
            }
            Projectile.velocity.X *= 0.9f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle drawFrame = Projectile.Frame();
            DrawAfterImageEffect(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, drawFrame, drawFrame.Size() / 2f, _squish, SpriteEffects.None, Color.White, 1f);
            this.DrawCentered(ref lightColor, _squish);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            this.OutlineNoRestart(Color.Red, ref lightColor, _squish);
        }
    }
}
