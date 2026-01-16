using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Winterborn
{
    public class WinterbornIcicleProj : ModProjectile
    {
        private float Health
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            Timer++;
            Projectile.scale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                Timer += Main.rand.NextFloat(0, 240);
                Projectile.netUpdate = true;
            }

            if (Main.rand.NextBool(32))
            {
                FlakeParticle fp = FlakeParticle.Spawn(Projectile.Center, Vector2.Zero);
                fp.Scale *= 0.15f;
                fp.gravity = 0;
            }
            AI_RotateAroundOwner();
        }

        private void AI_RotateAroundOwner()
        {
            float offsetProgress = Timer / 240;
            float degrees = offsetProgress * MathHelper.TwoPi;
            float circleDistance = 64;
            Vector2 circleCenter = Owner.Center;
            Vector2 circleOffset = new Vector2(circleDistance, 0);
            Vector2 rotatedCirclePosition = circleCenter + circleOffset.RotatedBy(degrees);
            Projectile.Center = rotatedCirclePosition;

            float osc = MathF.Sin(offsetProgress) * 16;
            Projectile.Center += new Vector2(osc, 0).RotatedBy(degrees);
            //Projectile.rotation = Owner.Center.DirectionTo(Projectile.Center).ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var projectile = Projectile;
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            int projFrames = Main.projFrames[projectile.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            //drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Color startColor = Color.White;
                Color endColor = Color.Black;
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.Lerp(startColor, endColor, 1f / projectile.oldPos.Length * k) * (1f - 1f / projectile.oldPos.Length * k));
                color = Color.Lerp(color, Color.Black, 0.85f);
                color.A = 0;
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, projectile.oldRot[k], drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int t = 0; t < 8; t++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                float scale = Main.rand.NextFloat(0.5f, 0.75f);
                Dust.NewDustPerfect(Projectile.Center, DustID.Ice, speed, newColor: Color.White, Scale: scale);
            }

       
            Health -= damageDone;
            if (Health <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
                Projectile.Kill();
            }
        }
    }
}
