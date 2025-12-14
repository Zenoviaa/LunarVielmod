using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public class DescendingRisingTornado : ScarletProjectile
    {
        private Vector2 InitialVelocity;
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[2];
        public Vector2 ReTargetPosition;
        public override string Texture => TextureRegistry.EmptyTexture;


        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 64;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(OldCenterPos, projHitbox, targetHitbox);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(ReTargetPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            ReTargetPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
            }

            if(Timer > 120f)
            {
                float completionRatio = (Timer - 120f) / 120f;
                float ease = EasingFunction.InOutSine(completionRatio);
                Vector2 targetVelocity = (ReTargetPosition - Projectile.Center);
                Projectile.velocity = Vector2.Lerp(InitialVelocity, targetVelocity, ease);
            }

            if(Timer == 240)
            {
                if(Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                        ModContent.ProjectileType<DescendingTornadoBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Variant);
                    Projectile.Kill();
                }
            }
            float numFlamePos = OldCenterPos.Length;
            for (int n = 0; n < numFlamePos; n++)
            {
                if (Main.rand.NextBool(32))
                {
                    SpawnFlameDust(OldCenterPos[n]);
                }
            }
        }

        private Color GetTwinColor() => DescendingTwins.GetTwinColor(Variant);
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
            return MathHelper.SmoothStep(32, 0, completionRatio) * ease * inScale * 2 * MathHelper.Lerp(8, 1f, EasingFunction.InOutSine(Timer / 30f));
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
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, flameTrailShader);

            flameTrailShader.BlendState = BlendState.Additive;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, flameTrailShader);
            return false;
        }

    }
}
