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
    public class DescendingNodeBeam : ScarletProjectile,
        IDrawPixelated
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
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
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
            if(Timer % 5 == 0)
            {
                var p = Particle.NewParticle<GlowFragmentParticle>(Projectile.Center, Vector2.Zero, Color.White, Scale: 4f);
                Color twinColor = Color.Green;
                p.innerColor = twinColor;
                p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
                p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
            }
        }
        private void SpawnFlameDonut()
        {
            //movement donut particles
            var donut = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.White);
            Color twinColor = Color.Green;
            donut.innerColor = twinColor;
            donut.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            donut.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }

        public void DrawPixelated()
        {
            float outScale = (float)Projectile.timeLeft / 10f;
            float outScaleEase = EasingFunction.InOutSine(outScale);

            Texture2D drawTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Backglow").Value;
            Vector2 drawOrigin = drawTexture.Size() / 2f;
            float numAfterImages = TrailCacheLength;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 centerPos = OldCenterPos[i] - Main.screenPosition;
                float f = i;
                float completionRatio = f / numAfterImages;

                Color drawColor = Color.Lerp(Color.White, Color.Green, completionRatio);
                drawColor.A = 0;
                drawColor *= MathHelper.Lerp(1f, 0f, completionRatio);

                float scale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                scale *= outScaleEase;
                scale *= 0.5f;
                spriteBatch.Draw(drawTexture, centerPos, null, drawColor, OldCenterRot[i], drawOrigin, scale, SpriteEffects.None, 0f);
            }
        }

    }
}
