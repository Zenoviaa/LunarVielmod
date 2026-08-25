using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;

using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class ForwardRune : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToRune(ModContent.ProjectileType<ForwardRuneShaper>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MiracleThread, BlankRune>();
        }
    }

    public class ForwardRuneShaper : AbstractRuneProjectile
    {

        public override bool MatchShapeCheck(Vector2[] shapePoints)
        {
            if (shapePoints.Length <= 2)
                return false;
            //First let's estimate where the lines are at
            //We can do this with a dot product, comparing it with the last line to see if it changes position
            List<Line> lines = ShapeUtilities.GetLines(shapePoints, detectLineChangeThreshold: 0.5f);

            //There may be cases where it detects an extra line, those are fine
            //But if there's too many extra lines just say it's a fail, this will prevent scribbling from doing anything
            if (lines.Count >= 7)
                return false;

            //A square has 4 90 degree angles
            //We'll add a margin of error though so you can still mess up a little bit and get credit for it
            float marginOfError = 32;
            float targetAngle = 72;
            float numMatches = ShapeUtilities.CountAngles(lines, targetAngle, marginOfError);

            //Finally check that this is roughly a closed shape
            //Pretty sure that a simple distance check can't realistically be cheated
            float distanceToEndPointThreshold = 180;
            bool startAndEndPointsMeet = Vector2.Distance(lines[0].a, lines[lines.Count - 1].b) <= distanceToEndPointThreshold;
            return numMatches >= 5 && startAndEndPointsMeet;
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Main.rand.NextBool(3))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.LightGoldenrodYellow,
                    outerColor = Color.Goldenrod,
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
            minion.Teleport(Main.MouseWorld + Main.rand.NextVector2Circular(48, 48));
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
            return Color.Lerp(Color.LightGoldenrodYellow, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }


        private void DrawDottedLine(GraphicsDevice graphicsDevice)
        {
            Vector2 startPosition = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            startPosition.Y -= 200;

            List<Vector2> points = new List<Vector2>();
            float numPoints = 72;
            for(int n = 0; n < 5; n++)
            {
                float angleRadians = MathHelper.TwoPi * (float)n / 5f - MathF.PI / 2f;
                float nextRadians = MathHelper.TwoPi * (float)(n+1) / 5f - MathF.PI / 2f;
                Vector2 offset = new Vector2(MathF.Cos(angleRadians), MathF.Sin(angleRadians)) * 100;
                Vector2 nextOffset = new Vector2(MathF.Cos(nextRadians), MathF.Sin(nextRadians)) * 100;

                float pointsPerSide = numPoints / 5;
                for(float i = 0; i < pointsPerSide; i++)
                {
                    Vector2 interp = Vector2.Lerp(offset, nextOffset, i / pointsPerSide);
                    points.Add(startPosition + interp);
                }
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
            shader.InnerColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 32));
            shader.OuterColor = Color.Goldenrod;
            shader.LaserTexture = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction, shader, Main.screenPosition);


            shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.LaserColor = Color.Goldenrod * 0.2f;
            shader.InnerColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 32)) * 0.2f;
            shader.OuterColor = Color.Goldenrod * 0.2f;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction2, shader, Main.screenPosition);
        }

        private void DrawPointer(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D pointerTexture = AssetManager.GlowMask.Shine.Value;
            Vector2 drawOrigin = pointerTexture.Size() / 2f;
            Vector2 drawCenter = DrawingPosition;

            Color glowColor2 = Color.Goldenrod;
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
                    outerColor = Color.Goldenrod,
                    scaleRange = new Vector2(0.1f, 1f)

                };
                var dp = DustParticle.Spawn(trailPoint + Main.screenPosition, Vector2.Zero, spawnParams);
                dp.gravity = 0;
            }
        }
    }
}
