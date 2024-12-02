using Stellamod.Common.Shaders.MagicTrails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Elements
{
    internal class AutomationElement : BaseElement
    {
        private int trailMode = 0;
        private LightningTrail _lightningTrail;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override Color GetElementColor()
        {
            return new Color(207, 150, 140);
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: new Color(207, 150, 140),
                primaryColor: Color.Lerp(Color.White, new Color(207, 150, 140), 0.5f),
                noiseColor: new Color(60, 107, 128));
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, new Color(207, 150, 140));
        }


        public override void AI()
        {
            AI_Particles();
        }

        private void AI_Particles()
        {
            if (MagicProj.GlobalTimer % 3 == 0)
            {
                _lightningTrail ??= new();
                _lightningTrail.RandomPositions(MagicProj.OldPos);
            }

            if (MagicProj.GlobalTimer % 8 == 0)
            {
                for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
                {
                    if (!Main.rand.NextBool(4))
                        continue;
                    Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                    Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                    velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                    Color color = new Color(207, 150, 140);
                    if (Main.rand.NextBool(2))
                        color = new Color(60, 107, 128);
                    color.A = 0;
                    Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color, Scale: 0.5f);
                }
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            SpawnDeathParticles();
        }

        private void SpawnDeathParticles()
        {
            //Kill Trail
            for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                Color color = new Color(207, 150, 140);
                if (Main.rand.NextBool(2))
                    color = new Color(60, 107, 128);
                color.A = 0;
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = new Color(207, 150, 140);
                if (Main.rand.NextBool(2))
                    color = new Color(60, 107, 128);
                color.A = 0;
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
            }
        }

        #region Visuals
        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }

        public override void DrawTrail()
        {
            //Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningBolt2Shader lightningShader = LightningBolt2Shader.Instance;
            lightningShader.PrimaryColor = new Color(207, 150, 140);
            lightningShader.NoiseColor = new Color(60, 107, 128);
            lightningShader.Speed = 5;
            lightningShader.BlendState = BlendState.Additive;

            _lightningTrail ??= new();
            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 32;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 24;
            _lightningTrail.Draw(spriteBatch, MagicProj.OldPos, Projectile.oldRot, ColorFunction, WidthFunction, lightningShader, offset: Projectile.Size / 2f);
        }

        private float WidthFunction(float completionRatio)
        {
            float progress = completionRatio / 0.3f;
            float rounded = Easing.SpikeOutCirc(progress);
            float spikeProgress = Easing.SpikeOutExpo(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));
            float midWidth = 47 * MagicProj.ScaleMultiplier;
            return MathHelper.Lerp(0, midWidth, fireball);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, new Color(207, 150, 140), p);
            return trailColor;
        }
        #endregion
    }
}
