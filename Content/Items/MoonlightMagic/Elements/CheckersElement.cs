using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class CheckersElement : BaseElement
    {
        private int trailMode = 0;
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

        public override Color GetElementColor()
        {
            return Color.White;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: Color.OrangeRed,
                primaryColor: Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f),
                noiseColor: new Color(206, 101, 0));
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.White);
        }

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;
            drawPos -= Projectile.velocity * 1.5f;
            drawScale *= 0.8f;
            spriteBatch.Draw(formTexture, drawPos, null, Color.Lerp(Color.Black, Color.White, ExtraMath.Osc(0f, 1f, speed: 32)), drawRotation, drawOrigin, drawScale * 1.15f +
            ExtraMath.Osc(-0.1f, 0.1f, speed: 16), SpriteEffects.None, 0);
        }

        public override void AI()
        {
            AI_Particles();
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            var shader = MagicCheckersShader.Instance;
            shader.SetDefaults();
            shader.BlendState = BlendState.AlphaBlend;
            shader.Distortion = 0;
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void AI_Particles()
        {
            if (MagicProj.GlobalTimer % 16 == 0)
            {
                int oldPosIndex = Main.rand.Next(0, MagicProj.OldPos.Length - 1);
                float lerpValue = (float)oldPosIndex / (float)MagicProj.OldPos.Length;
                float scaleFactor = MathHelper.Lerp(1.0f, 0f, lerpValue);

                Vector2 spawnPoint = MagicProj.OldPos[oldPosIndex] + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[oldPosIndex + 1] - MagicProj.OldPos[oldPosIndex];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -8;

                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                offset *= scaleFactor;
                spawnPoint += offset;

                Color color = Color.White;
                //  color.A = 0;
                Particle.NewParticle<GlowParticle>(spawnPoint, velocity, color, Scale: MagicProj.ScaleMultiplier * scaleFactor);
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
                if (!Main.rand.NextBool(4))
                    continue;
                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                Color color = Color.White;
                Particle.NewParticle<GlowParticle>(spawnPoint, velocity, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.White;
                Particle.NewParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
            }
            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.White,
               glowColor: Color.LightGray,
               outerGlowColor: Color.DarkGray, duration: 15, baseSize: boomSize);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.White;
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 100;
            float ew = w / 10;
            float width = w * MagicProj.ScaleMultiplier;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w * MagicProj.ScaleMultiplier, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }
    }
}
