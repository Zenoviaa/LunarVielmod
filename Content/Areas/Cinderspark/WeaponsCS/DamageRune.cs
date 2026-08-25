using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{

    public class DamageRune : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToRune(ModContent.ProjectileType<DamageRuneShaper>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankRune>();
        }
    }
    public class DamageRuneShaper : AbstractRuneProjectile
    {

        public override bool MatchShapeCheck(Vector2[] shapePoints)
        {
            if (shapePoints.Length <= 2)
                return false;

            //To detect a circle, we don't even have to do any of this
            //We can just loop over the entire shape points array and check that the previous movement doesn't go over acertain threshold
            //and the last point is close to the starting point
            Vector2 lastMovement = shapePoints[1] - shapePoints[0];
            lastMovement = lastMovement.SafeNormalize(Vector2.Zero);
            for (int n = 2; n < shapePoints.Length; n++)
            {
                Vector2 prevPoint = shapePoints[n - 1];
                Vector2 point = shapePoints[n];
                Vector2 movement = (point - prevPoint).SafeNormalize(Vector2.Zero);
                float dp = Vector2.Dot(lastMovement, movement);
                if (dp < 0.75f)
                {
                    return false;
                }
                lastMovement = movement;
            }
            float distanceToEndPointThreshold = 120;
            bool startAndEndPointsMeet = Vector2.Distance(shapePoints[0], shapePoints[shapePoints.Length - 1]) <= distanceToEndPointThreshold;
            return startAndEndPointsMeet;
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Main.rand.NextBool(3))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Red,
                    outerColor = Color.DarkRed,
                    scaleRange = new Vector2(0.3f, 0.6f)
                };
                var dp = DustParticle.Spawn(DrawingPosition + Main.screenPosition, Main.rand.NextVector2Circular(3, 3), spawnParams);
                dp.gravity = 0.02f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundStyle soundStyle = AssetRegistry.Sounds.Runes.RuneTeleport;
            soundStyle.PitchVariance = 0.2f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        public override void ApplyMagic(AbstractBellSummon minion)
        {
            minion.DamageBuff();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPointer);
            PixelationManager.QueuePrimitivesDrawAction(DrawRunePixelPrimitives);
            return false;
        }
        public float PreviewWidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0f, 3, EasingFunction.InOutSine(EaseInRatio)) * MathF.Sin(completionRatio * 8 + Main.GlobalTimeWrappedHourly * 4);
        }

        public Color PreviewColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, EasingFunction.QuadraticBump(completionRatio));
        }

        public float WidthFunction(float completionRatio)
        {
            float osc = MathF.Sin(completionRatio * 384) * 0.5f + 0.5f;
            return MathHelper.SmoothStep(7, 2, completionRatio) * MathHelper.Lerp(1f, 0f, osc);
        }

        public float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 3;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.IndianRed, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }


        private void DrawDottedLine(GraphicsDevice graphicsDevice)
        {
            Vector2 startPosition = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            startPosition.Y -= 200;
            List<Vector2> points = new List<Vector2>();
            float numPoints = 36;
            for (float n = 0; n < numPoints; n++)
            {
                float ratio = n / numPoints;
                float radians = ratio * MathHelper.TwoPi;
                Vector2 offset = new Vector2(MathF.Cos(radians), MathF.Sin(radians)) * 100;
                points.Add(startPosition + offset);
            }

            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.White;
            TrailDrawer.Draw(Main.spriteBatch, points.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
        }

        private void DrawRunePixelPrimitives(GraphicsDevice graphicsDevice)
        {
            DrawDottedLine(graphicsDevice);
            //DrawInnerSquare();
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(Color.IndianRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 32));
            shader.OuterColor = Color.Red;
            shader.LaserTexture = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction, shader, Main.screenPosition);


            shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.LaserColor = Color.Red * 0.2f;
            shader.InnerColor = Color.Lerp(Color.GreenYellow, Color.Green, ExtraMath.Osc(0f, 1f, speed: 32)) * 0.2f;
            shader.OuterColor = Color.Red * 0.2f;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction2, shader, Main.screenPosition);
        }

        private void DrawPointer(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D pointerTexture = AssetManager.GlowMask.Shine.Value;
            Vector2 drawOrigin = pointerTexture.Size() / 2f;
            Vector2 drawCenter = DrawingPosition;

            Color glowColor2 = Color.Red;
            glowColor2 = Color.Lerp(glowColor2, Color.Black, ExtraMath.Osc(0.1f, 0.3f, 4));
            glowColor2.A = 0;
            spriteBatch.Draw(pointerTexture, drawCenter, null, glowColor2, 0, drawOrigin, ExtraMath.Osc(0.8f, 1f, speed: 32) * 0.2f, SpriteEffects.None, 0);

            Color glowColor = Color.White;
            glowColor = Color.Lerp(glowColor, Color.Black, ExtraMath.Osc(0.1f, 0.3f, 4));
            glowColor.A = 0;
            spriteBatch.Draw(pointerTexture, drawCenter, null, glowColor, 0, drawOrigin, ExtraMath.Osc(0.8f, 1f, speed: 32) * 0.12f, SpriteEffects.None, 0);

        }
        public override void OnDissipate(Vector2 trailPoint)
        {
            base.OnDissipate(trailPoint);
            if (Main.rand.NextBool(2))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.White,
                    outerColor = Color.Red,
                    scaleRange = new Vector2(0.1f, 1f)

                };
                var dp = DustParticle.Spawn(trailPoint + Main.screenPosition, Vector2.Zero, spawnParams);
                dp.gravity = 0;
            }
        }
    }
}
