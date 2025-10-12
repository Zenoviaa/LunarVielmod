using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class RadianceElement : BaseElement
    {
        private int trailMode = 0;

        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<HolinessElement>());
        }
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<PhantasmalElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.FireCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.FireChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;

            SoundStyle hitStyle = SoundRegistry.RadianceHit1;
            hitStyle.PitchVariance = 0.25f;
            HitSound = hitStyle;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.RadianceYellow;
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
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.RadianceYellow);
        }

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.LowDetailShadersToggle)
            {
                DrawHighDetailForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
            }
            else
            {
                DrawLowDetailForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
            }
        }

        private void DrawLowDetailForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }
        public MoonSparkleShader SparkleShader;
        private void DrawHighDetailForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;
            drawPos -= Projectile.velocity * 1.5f;
            drawScale *= 0.6f;
            SparkleShader ??= new MoonSparkleShader();
            SparkleShader.ApplyToEffect();
            spriteBatch.Restart(effect: SparkleShader.Effect, blendState: BlendState.Additive);
            spriteBatch.Draw(formTexture, drawPos, null, Color.White, drawRotation, drawOrigin, drawScale * 1.15f +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 16), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();


            spriteBatch.Draw(formTexture, drawPos, null, Color.Lerp(Color.Black, Color.Red, MathUtil.Osc(0f, 1f, speed: 12)),
               drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(formTexture, drawPos, null, Color.White * 0.3f, drawRotation, drawOrigin, drawScale +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 4), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
        }
        public override void AI()
        {
            AI_Particles();
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            DrawMainShader(oldPos);
            DrawOutlineShader(oldPos);
        }

        private void AI_Particles()
        {
            if (MagicProj.GlobalTimer % 8 == 0)
            {
                int oldPosIndex = Main.rand.Next(0, MagicProj.OldPos.Length - 1);
                float lerpValue = (float)oldPosIndex / (float)MagicProj.OldPos.Length;
                float scaleFactor = MathHelper.Lerp(1.0f, 0.8f, lerpValue);

                Vector2 spawnPoint = MagicProj.OldPos[oldPosIndex] + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[oldPosIndex + 1] - MagicProj.OldPos[oldPosIndex];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -8;

                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                offset *= scaleFactor;
                spawnPoint += offset;

                Color color = Color.RosyBrown;
                //  color.A = 0;
                Particle.NewParticle<FireSmokeParticle>(spawnPoint, velocity, color, Scale: MagicProj.ScaleMultiplier * scaleFactor);
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                //  target.AddBuff(ModContent.BuffType<RadianceFireDebuff>(), time: 360);
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

                if (Main.rand.NextBool(2))
                {
                    Color color = Color.RosyBrown;
                    color.A = 0;
                    Particle.NewBlackParticle<FireSmokeParticle>(spawnPoint, velocity, color);
                }
                else
                {
                    Color color = ColorFunctions.RadianceYellow;
                    color.A = 0;
                    Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
                    Particle.NewBlackParticle<FireHeatParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                }
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                if (Main.rand.NextBool(2))
                {
                    Color color = Color.RosyBrown;
                    color.A = 0;
                    Particle.NewParticle<FireSmokeParticle>(spawnPoint, velocity, color);
                }
                else
                {

                    Color color = ColorFunctions.RadianceYellow;
                    if (Main.rand.NextBool(2))
                        color = Color.OrangeRed;

                    Particle.NewParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
                    Particle.NewParticle<FireHeatParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                }
            }
            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.Yellow,
                glowColor: Color.Red,
                outerGlowColor: Color.DarkRed, duration: 25, baseSize: boomSize);
            FXUtil.GlowCircleBoom(Projectile.Center,
           innerColor: Color.Yellow,
           glowColor: Color.Red,
           outerGlowColor: Color.DarkRed, duration: 15, baseSize: boomSize * 2);
        }

        private Color ColorFunction(float completionRatio)
        {
            Color c;
            switch (trailMode)
            {
                default:
                case 0:
                    c = Color.Lerp(Color.White, new Color(147, 72, 11), completionRatio);
                    break;
                case 1:
                    c = Color.Lerp(Color.White, new Color(147, 72, 11) * 0f, completionRatio);
                    break;
                case 2:
                    c = Color.White;
                    c.A = 0;
                    break;
            }
            return c;
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 100;
            if (trailMode == 2)
            {
                w *= 1.3f;
            }
            float ew = w / 10;
            float width = w * MagicProj.ScaleMultiplier;


            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w * MagicProj.ScaleMultiplier, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }


        private void DrawMainShader(Vector2[] oldPos)
        {
            //Trail
            trailMode = 0;
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
            shader.NoiseColor = new Color(206, 101, 0);
            shader.OutlineColor = Color.Black;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.05f;
            shader.Power = 0.35f;

            //This just applis the shader changes

            //Main Fill
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);

            //Secondary fill
            trailMode = 0;
            shader.PrimaryColor = new Color(206, 101, 0);
            shader.NoiseColor = Color.Red;
            shader.OutlineColor = Color.Black;
            shader.Speed = 2.2f;
            shader.Distortion = 0.3f;
            shader.Power = 1.5f;
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void DrawOutlineShader(Vector2[] oldPos)
        {
            trailMode = 2;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = Color.DarkRed;
            shader.PrimaryColor = c;
            shader.NoiseColor = Color.DarkRed;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.05f;
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }
    }
}
