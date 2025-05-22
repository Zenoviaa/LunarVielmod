using Stellamod.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Elements
{
    internal class VeilElement : BaseElement
    {
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private Trails.CommonLightning _lightning;
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<NaturalElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            SoundStyle castStyle = SoundID.DD2_LightningAuraZap;
            castStyle.PitchVariance = 0.15f;
            CastSound = castStyle;

            SoundStyle hitStyle = SoundID.DD2_LightningBugZap;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.VeilPink;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.VeilPink,
                primaryColor: Color.White,
                noiseColor: Color.DarkGray);
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.VeilPink);
        }

        public override void OnKill()
        {
            base.OnKill();
            SpawnDeathParticles();
        }

        public override void AI()
        {
            base.AI();
            if (MagicProj.GlobalTimer % 3 == 0)
            {
                _lightningTrail ??= new();
                _lightningTrail.RandomPositions(MagicProj.OldPos);
            }


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

                Color color = Color.White;
                color.A = 0;
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.White;
                color.A = 0;
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
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
            var shader = MagicVaellusShader.Instance;
            shader.PrimaryTexture = TrailRegistry.LightningTrail2;
            shader.NoiseTexture = TrailRegistry.LightningTrail3;
            shader.OutlineTexture = TrailRegistry.LightningTrail2Outline;
            shader.PrimaryColor = Color.Orange;
            shader.NoiseColor = new Color(169, 101, 255);
            shader.OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            shader.Alpha = 1f;

            _lightningTrail ??= new();
            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 5;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 12;
            _lightningTrail.Draw(spriteBatch, MagicProj.OldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2f);
       
            /*

            for (int i = 0; i < _lightning.Trails.Length; i++)
            {
                float progress = (float)i / (float)_lightning.Trails.Length;
                var trail = _lightning.Trails[i];
                trail.PrimaryTexture = TrailRegistry.LightningTrail2;
                trail.NoiseTexture = TrailRegistry.LightningTrail3;
                trail.LightningRandomOffsetRange = MathHelper.Lerp(8, 4, progress);
                trail.LightningRandomExpand = MathHelper.Lerp(8, 2, progress);
                trail.PrimaryColor = Color.Lerp(Color.Orange, Color.Purple, progress);
                trail.NoiseColor = Color.Lerp(Color.Purple, Color.Black, progress);
            }
            _lightning.WidthTrailFunction = WidthFunction;

            SpriteBatch spriteBatch = Main.spriteBatch;
            _lightning.DrawAlpha(spriteBatch, MagicProj.OldPos, Projectile.oldRot);*/
        }

        private float WidthFunction(float completionRatio)
        {
            float progress = completionRatio / 0.3f;
            float rounded = Easing.SpikeOutCirc(progress);
            float spikeProgress = Easing.SpikeOutExpo(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));
            float midWidth = 48 * MagicProj.ScaleMultiplier;
            return MathHelper.Lerp(0, midWidth, fireball);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.Orange, Color.Purple, p);
            return trailColor;
        }
        #endregion
    }
}
