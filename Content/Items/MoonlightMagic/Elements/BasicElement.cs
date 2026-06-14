using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class BasicElement : BaseElement
    {
        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<CheckersElement>());
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            SoundStyle castStyle = SoundID.Item43;
            castStyle.PitchVariance = 0.15f;
            CastSound = castStyle;

            SoundStyle hitStyle = SoundRegistry.BasicMagicHit;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;


            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.BasicCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

        }

        public override void OnKill()
        {
            base.OnKill();
            SpawnDeathParticles();
        }

        private void SpawnDeathParticles()
        {
            //Kill Trail
            //Kill Trail
            for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
            {
                if (!Main.rand.NextBool(12))
                    continue;
                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                Color color = Color.Lerp(Color.Black, Color.White, 0.5f);

                LegacyParticle.NewParticle<GlowParticle>(spawnPoint, velocity, color, Scale: MagicProj.ScaleMultiplier);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                if (!Main.rand.NextBool(4))
                    continue;
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.Lerp(Color.Black, Color.White, 0.5f);

                LegacyParticle.NewParticle<GlowParticle>(spawnPoint, velocity, color, Scale: MagicProj.ScaleMultiplier);
            }
            float boomSize = Main.rand.NextFloat(0.05f, 0.07f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightCyan,
                outerGlowColor: Color.Black, duration: 25, baseSize: boomSize);
        }

        #region Visuals
        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: Color.White,
                primaryColor: Color.White,
                noiseColor: Color.DarkGray);
            return true;
        }
        public MoonSparkleShader SparkleShader;
        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;
            drawPos -= Projectile.velocity * 1.5f;
            drawScale *= 0.5f;
            SparkleShader ??= new MoonSparkleShader();
            SparkleShader.ApplyToEffect();
            spriteBatch.Restart(effect: SparkleShader.Effect, blendState: BlendState.Additive);
            spriteBatch.Draw(formTexture, drawPos, null, Color.White, drawRotation, drawOrigin, drawScale * 1.25f +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 8), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();


        }

        public override void DrawTrail(Vector2[] oldPos)
        {

            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.25f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 25;
            float ew = w / 10;
            float width = w * MagicProj.ScaleMultiplier;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w * MagicProj.ScaleMultiplier, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }

        #endregion
    }
}
