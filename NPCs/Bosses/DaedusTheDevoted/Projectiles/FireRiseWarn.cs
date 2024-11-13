using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DaedusRework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class FireRiseWarn : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            int explosion = ModContent.ProjectileType<DaedusBombExplosion>();
            int damage = 0;
            int knockback = 2;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - new Vector2(32), Vector2.Zero, explosion, damage, knockback);

            //Dust Particles
            for (int k = 0; k < 3; k++)
            {
                Vector2 newVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(7));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Dust.NewDust(Projectile.Center, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                Dust.NewDust(Projectile.Center, 0, 0, DustID.InfernoFork, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, 
                ModContent.ProjectileType<FireRise>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Color drawColor = new Color(55, 45, 5);
            drawColor *= MathHelper.Lerp(0f, 1f, Easing.SpikeOutCirc(Timer / 90f));
            drawColor.A = 0;

            Vector2 drawScale = new Vector2(1f, 1f);
            Vector2 drawOrigin = new Vector2(texture.Width / 2f, texture.Height);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
         
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPos, null, drawColor,
                            Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
