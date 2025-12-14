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

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles
{
    public class DescendingLaser : ModProjectile
    {
        private Vector2[] FlamePos = new Vector2[64];
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }
        private int Variant => (int)Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(FlamePos, projHitbox, targetHitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            ShakeModSystem.Shake = 1;
            Projectile.Center = Parent.Center;
            Projectile.velocity = Parent.rotation.ToRotationVector2() * Projectile.velocity.Length();
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
            if (!Parent.active && Projectile.timeLeft > 10)
                Projectile.timeLeft = 10;
        }

        private Color GetTwinColor() => DescendingTwins.GetTwinColor(Variant);
        private Color GetSecondaryTwinColor() => DescendingTwins.GetSecondaryTwinColor(Variant);
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
            return MathHelper.SmoothStep(16, 24, completionRatio) * ease * inScale * 2 * MathHelper.Lerp(8, 1f, EasingFunction.InOutSine(Timer / 30f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DescendingFlameTrailShader flameTrailShader = DescendingFlameTrailShader.Instance;
            flameTrailShader.LaserTexture = AssetRegistry.Textures.Noise.JungleWaterCaustics;
            flameTrailShader.Tiling = Vector2.One * new Vector2(4, 0.85f);

            Color innerColor = GetTwinColor();
            Color outerColor = GetSecondaryTwinColor();

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
