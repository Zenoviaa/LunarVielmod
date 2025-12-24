using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles
{
    public class DescendingNodeBeam : ScarletProjectile
    {

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle shootSound = AssetRegistry.Sounds.SteamPunking.DescendingRetinaBeam;
                shootSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shootSound, Projectile.position);
                SpawnFlameDonut();
            }
            float outScale = (float)Projectile.timeLeft / 10f;
            float outScaleEase = EasingFunction.InOutSine(outScale);

            if (Timer % 2 == 0)
            {
                var p = LegacyParticle.NewParticle<GlowFragmentParticle>(Projectile.Center, Vector2.Zero, Color.White, Scale: 4f);
                Color twinColor = Color.Yellow;
                p.innerColor = twinColor;
                p.outerColor = Color.Red;
                p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
                p.Rotation += Main.rand.NextFloat(-0.5f, 0.5f);
                p.Scale *= 1.5f * outScaleEase;
            }
            if(Projectile.velocity.Length() < 10f)
            {
                Projectile.velocity *= 1.1f;
            }
        }
        private void SpawnFlameDonut()
        {
            //movement donut particles
            var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.White);
            Color twinColor = Color.Red;
            donut.innerColor = twinColor;
            donut.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            donut.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);
            return false;
        }
        public void DrawPixelated(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            float outScale = (float)Projectile.timeLeft / 10f;
            float outScaleEase = EasingFunction.InOutSine(outScale);

            Texture2D drawTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Backglow").Value;
            Vector2 drawOrigin = drawTexture.Size() / 2f;
            float numAfterImages = TrailCacheLength;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 centerPos = OldCenterPos[i] - Main.screenPosition;
                float f = i;
                float completionRatio = f / numAfterImages;

                Color drawColor = Color.Lerp(Color.Red, Color.DarkRed, completionRatio);
                drawColor.A = 0;

                float scale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                scale *= outScaleEase;
                scale *= 0.5f;
                spriteBatch.Draw(drawTexture, centerPos, null, drawColor, OldCenterRot[i], drawOrigin, scale, SpriteEffects.None, 0f);
            }
        }

    }
}
