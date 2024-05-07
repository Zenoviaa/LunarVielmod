using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class DaggerDaggerProj2 : ModProjectile
    {

        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Timer++;
            if(Timer > 60)
            {
                Projectile.velocity *= 0.98f;
            }
            else
            {
                Projectile.velocity.Y += 0.2f;
            }

            Projectile.velocity.Y += 0.05f;
            Projectile.rotation += 0.4f;

            if (Timer % 25 == 0)
            {
                NPC closestNPC = NPCHelper.FindClosestNPC(Projectile.position, 1500);
                Vector2 targetVelocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
                if (closestNPC != null)
                {
                    targetVelocity = Projectile.Center.DirectionTo(closestNPC.Center) * 12;
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetVelocity, ModContent.ProjectileType<DaggerDaggerKnifeProj>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Owner.Heal(10);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Red, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new Vector2(42, 46);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = VectorHelper.Osc(0f, 1f, 12);
            Color drawColor = Color.Lerp(Color.Transparent, Color.Red, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            Main.EntitySpriteDraw(whiteTexture, drawPosition, null, Color.Red, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        }
    }
}
