using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseFlyingSlashProjectile : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.FlyingSlashTexture;
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }


        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            AI_Dusts();
        }
        public virtual void AI_Dusts()
        {
            if (Timer % 4 == 0)
            {
                Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                Vector2 dustVelocity = Vector2.Zero;
                Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowDust>(), Velocity: dustVelocity, newColor: Color.White, Scale: 0.24f);
            }
        }

        public virtual float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.8f;
            return MathHelper.SmoothStep(baseWidth, baseWidth, completionRatio);
        }

        public virtual Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            return Color.Lerp(startColor, Color.Lerp(Color.LightSkyBlue, Color.Blue, completionRatio), completionRatio);
        }

        public Color TrailColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            return Color.Lerp(startColor, Color.Transparent, completionRatio);
        }



        public PrimDrawer TrailDrawer { get; private set; } = null;
        public virtual void DrawSlashTrail()
        {
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, TrailColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            }

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Vector2 trailOffset = -Main.screenPosition + Projectile.Size / 2;
            trailOffset += Projectile.velocity * 4;
            TrailDrawer.ColorFunc = TrailColorFunction;
            TrailDrawer.DrawPrims(Projectile.oldPos, trailOffset, 155);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlashTrail();
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color drawColor = Color.White;
            drawColor.A = 0;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation;
            Vector2 drawOrigin = texture.Size() / 2;
            Vector2 drawScale = Vector2.One;

            SpriteBatch spriteBatch = Main.spriteBatch;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = (float)i / (float)Projectile.oldPos.Length;
                float scaleMult = MathHelper.Lerp(1f, 0f, progress);
                Vector2 afterImagePos = Projectile.oldPos[i] + Projectile.Size / 2;
                afterImagePos -= Main.screenPosition;

                drawColor = ColorFunction(progress);
                drawColor.A = 0;

                drawScale = Vector2.One;
                drawScale.X *= 1.8f;
                drawScale.Y *= 0.5f;
                drawScale *= 0.75f;
                spriteBatch.Draw(texture, afterImagePos, null, drawColor * scaleMult * 0.4f, Projectile.oldRot[i], drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            drawScale = Vector2.One;
            drawScale.X *= 1.8f;
            drawScale.Y *= 0.5f;
            drawScale *= 0.75f;
            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 glowOffset = rot.ToRotationVector2() * 4 * VectorHelper.Osc(0.75f, 1f, speed: 3);
                Vector2 glowDrawPos = drawPos + glowOffset;
                spriteBatch.Draw(texture, glowDrawPos, null, drawColor * 0.4f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }


            spriteBatch.Draw(texture, drawPos, null, drawColor * 0.4f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
