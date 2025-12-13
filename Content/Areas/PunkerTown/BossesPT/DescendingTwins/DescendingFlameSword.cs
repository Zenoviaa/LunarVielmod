using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public class DescendingFlameSword : ModProjectile
    {
        private Vector2[] FlamePos = new Vector2[64];
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent => Main.npc[(int)Projectile.ai[1]];
        private int Variant => (int)Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(FlamePos, projHitbox, targetHitbox);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                ShakeModSystem.Shake = 2;
                FXUtil.ShakeCamera(Projectile.position, 1024, 6);


            }

            float numFlamePos = FlamePos.Length;
            for (int n = 0; n < numFlamePos; n++)
            {
                float completionRatio = (float)n / numFlamePos;
                FlamePos[n] = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
                if (Main.rand.NextBool(32))
                {
                    SpawnFlameDust(FlamePos[n]);

                }

            }
            Projectile.Center = Parent.Center;
        }
        private Color GetTwinColor()
        {
            switch (Variant)
            {
                default:
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Red;
            }
        }
        private void SpawnFlameDust(Vector2 position)
        {
            var p = Particle.NewParticle<GlowFragmentParticle>(position, Projectile.velocity.SafeNormalize(Vector2.Zero) * 5f, Color.White);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }


        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.White, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
        }

        private float GetTrailWidth(float completionRatio)
        {
            float outScale = (float)Projectile.timeLeft / 30f;
            float inScale = EasingFunction.InOutSine(Timer / 30f);
            float ease = EasingFunction.InOutSine(outScale);
            return MathHelper.SmoothStep(0, 32, completionRatio) * ease * inScale * 2 * MathHelper.Lerp(8, 1f, EasingFunction.InOutSine(Timer / 30f));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DescendingFlameTrailShader flameTrailShader = DescendingFlameTrailShader.Instance;
            flameTrailShader.LaserTexture = AssetRegistry.Textures.Noise.JungleWaterCaustics;

            flameTrailShader.Tiling = Vector2.One * new Vector2(4, 0.85f);
            Color innerColor;
            Color outerColor;
            switch (Variant)
            {
                default:
                case 0:
                    innerColor = Color.GreenYellow;
                    outerColor = Color.Green;
                    break;
                case 1:
                    innerColor = Color.Yellow;
                    outerColor = Color.Red;
                    break;
            }


            float lerp = EasingFunction.InOutSine(Timer / 20f);
            flameTrailShader.InnerColor = Color.Lerp(Color.White, innerColor, lerp);
            flameTrailShader.OuterColor = Color.Lerp(Color.White, outerColor, lerp);
            flameTrailShader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, FlamePos, GetTrailColor, GetTrailWidth, flameTrailShader);

            flameTrailShader.BlendState = BlendState.Additive;
            TrailDrawer.Draw(Main.spriteBatch, FlamePos, GetTrailColor, GetTrailWidth, flameTrailShader);
            return false;
        }

    }
}
