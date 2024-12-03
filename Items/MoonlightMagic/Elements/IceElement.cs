using CrystalMoon.Systems.MiscellaneousMath;
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
    internal class IceElement : BaseElement
    {
        int trailingMode = 0;
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<WindElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            SoundStyle castStyle = SoundID.Item28;
            castStyle.PitchVariance = 0.25f;
            CastSound = castStyle;

            SoundStyle hitStyle = SoundID.Item50;
            hitStyle.PitchVariance = 0.25f;
            HitSound = hitStyle;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.IceLightBlue,
                primaryColor: Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f),
                noiseColor: Color.Blue);
            return true;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.IceLightBlue;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.IceLightBlue);
        }

        public override void AI()
        {
            base.AI();
            AI_Particles();
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

                if (Main.rand.NextBool(2))
                {
                    Dust.NewDustPerfect(spawnPoint, DustID.Snow, velocity);
                }

                if (Main.rand.NextBool(2))
                {
                    Color color = Color.White;
                    color.A = 0;
                    Particle.NewBlackParticle<SnowflakeParticle>(spawnPoint, velocity, color);
                }
                else
                {
                    Color color = ColorFunctions.IceLightBlue;
                    color.A = 0;
                    Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
                    Particle.NewBlackParticle<SnowflakeParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                }
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                if (Main.rand.NextBool(2))
                {
                    Color color = ColorFunctions.IceLightBlue;
                    color.A = 0;
                    Particle.NewBlackParticle<SnowflakeParticle>(spawnPoint, velocity, color);
                }
                else
                {

                    Color color = Color.White;
                    color.A = 0;
                    Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
                    Particle.NewBlackParticle<SnowflakeParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                }
            }
        }

        private void AI_Particles()
        {
            if (MagicProj.GlobalTimer % 8 == 0)
            {
                for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
                {
                    if (!Main.rand.NextBool(2))
                        continue;
                    Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                    Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                    velocity = velocity.SafeNormalize(Vector2.Zero) * -8;

                    Color color = Color.White;
                    color.A = 0;


                    if (Main.rand.NextBool(7))
                    {
                        Dust.NewDustPerfect(spawnPoint, DustID.Snow, velocity);
                    }

                    if (Main.rand.NextBool(7))
                    {
                        Particle.NewBlackParticle<SnowflakeParticle>(spawnPoint, velocity, color);
                    }
                }
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            Color endColor = Color.Lerp(Color.SeaGreen, Color.LightBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
            endColor = Color.Lerp(Color.White, endColor, VectorHelper.Osc(0f, 1f, speed: 3f, offset: 4));
             Color c = Color.Lerp(Color.White, endColor, completionRatio);
            
            return c;
        }
        private float WidthFunction(float completionRatio)
        {
            float width = 16 * 4f * MagicProj.ScaleMultiplier;
            completionRatio = Easing.SpikeOutCirc(completionRatio);
            return MathHelper.Lerp(0, width, completionRatio);
        }


        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
            spriteBatch.Restart(blendState: BlendState.Additive);
         
            for(float f = 0f; f <1f; f+= 0.1f)
            {
                float rot = f * MathHelper.ToRadians(360);
                rot += Main.GlobalTimeWrappedHourly * 0.05f;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(4f, 7f);
                base.DrawForm(spriteBatch, formTexture, drawPos  + offset, drawColor * 0.3f, lightColor, drawRotation, drawScale);
            }
          
            spriteBatch.RestartDefaults();
        }

        private void DrawMainShader()
        {
            var shader = MagicIceShader.Instance;
            shader.TrailTexture = TrailRegistry.IceTrailFlat;
            shader.MorphTexture = TrailRegistry.IceTrailSpiked;
            shader.DistortingTexture = TrailRegistry.CrystalNoise;
            shader.BlendState = BlendState.AlphaBlend;
            shader.TrailColor = Color.White;
            shader.Speed = Main.GlobalTimeWrappedHourly * 0.001f;

            shader.Distortion = 0.015f;


            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, MagicProj.OldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
            shader = MagicIceShader.Instance;
            shader.TrailTexture = TrailRegistry.IceTrailFlat;
            shader.MorphTexture = TrailRegistry.IceTrailSpiked;
            shader.DistortingTexture = TrailRegistry.CrystalNoise;
            shader.BlendState = BlendState.Additive;
            shader.TrailColor = Color.White;
            shader.GlowColor = Color.Blue;
            shader.Speed = Main.GlobalTimeWrappedHourly * 0.001f;

            shader.Distortion = 0.015f;


            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, MagicProj.OldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
       
        }


        public override void DrawTrail()
        {
            base.DrawTrail();
            DrawMainShader();
        }
    }
}
