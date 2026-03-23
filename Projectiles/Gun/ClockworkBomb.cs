using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
 
    public class ClockworkBomb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float Speed => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 12;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            //Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1; 
            Timer++;
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Teal, Main.rand.NextFloat(1f, 1.5f));
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.CopperCoin, Projectile.velocity * 0.1f, 0, Color.White, Main.rand.NextFloat(1f, 1.5f));
                }
            }

            if (Timer <= 2 && Main.myPlayer == Projectile.owner)
            {
                Speed = Main.rand.NextFloat(0.92f, 0.98f);
                Projectile.netUpdate = true;
            }
            Projectile.velocity *= Speed;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Length() <= 0.1f && Projectile.active)
            {
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, Color.Teal.ToVector3() * 1.75f * Main.essScale);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            for (int i = 0; i < 6; i++)
            {
                Color glowColor = Color.Teal;
                glowColor.A = 0;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f) * VectorHelper.Osc(0f, 1f, offset: i), SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
      //      Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ClockworkBoomer>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
