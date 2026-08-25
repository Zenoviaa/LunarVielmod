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
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT
{
    public class ReturnRune : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToRune(ModContent.ProjectileType<ReturnRuneShaper>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MechanizedSoul, BlankRune>();
        }
    }

    public class ReturnRuneShaper : AbstractRuneProjectile
    {
        private void GetDrawBounds(out Vector2 outerLeftBound, out Vector2 outerRightBound, out Vector2 innerLeftBound, out Vector2 innerRightBound)
        {
            outerLeftBound = StartDrawingPosition - new Vector2(32);

            //Size of the square you gotta make
            outerRightBound = outerLeftBound + new Vector2(256, 256);


            float padding = 32;
            innerLeftBound = StartDrawingPosition + new Vector2(32);
            innerRightBound = outerRightBound - new Vector2(padding) - new Vector2(32);
        }


        public override bool MatchShapeCheck(Vector2[] shapePoints)
        {
            if (shapePoints.Length <= 3)
                return false;
            //First let's estimate where the lines are at
            //We can do this with a dot product, comparing it with the last line to see if it changes position
            List<Line> lines = ShapeUtilities.GetLines(shapePoints, detectLineChangeThreshold: 0.5f);

            //There may be cases where it detects an extra line, those are fine
            //But if there's too many extra lines just say it's a fail, this will prevent scribbling from doing anything
            if (lines.Count >= 6)
                return false;

            //A square has 4 90 degree angles
            //We'll add a margin of error though so you can still mess up a little bit and get credit for it
            float marginOfError = 25;
            float targetAngle = 90;
            float numMatches = ShapeUtilities.CountAngles(lines, targetAngle, marginOfError);

            //Finally check that this is roughly a closed shape
            //Pretty sure that a simple distance check can't realistically be cheated
            float distanceToEndPointThreshold = 120;
            bool startAndEndPointsMeet = Vector2.Distance(lines[0].a, lines[lines.Count - 1].b) <= distanceToEndPointThreshold;
            return numMatches >= 4 && startAndEndPointsMeet;
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Main.rand.NextBool(3))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.White,
                    outerColor = Color.Blue,
                    scaleRange = new Vector2(0.3f, 0.6f)
                };
                DustParticle.Spawn(DrawingPosition + Main.screenPosition, Main.rand.NextVector2Circular(8, 8), spawnParams);
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
            //  throw new System.NotImplementedException();
            minion.Teleport(Owner.Center);
        }

        private void DrawOuterSquare()
        {
            GetDrawBounds(out Vector2 outerLeftBound, out Vector2 outerRightBound, out Vector2 innerLeftBound, out Vector2 innerRightBound);
            List<Vector2> line1 = new List<Vector2>();
            List<Vector2> line2 = new List<Vector2>();
            List<Vector2> line3 = new List<Vector2>();
            List<Vector2> line4 = new List<Vector2>();
            float numPoints = 32;
            for (float i = 0; i < numPoints; i++)
            {
                line1.Add(Vector2.Lerp(outerLeftBound, new Vector2(outerLeftBound.X, outerRightBound.Y), i / numPoints));
                line2.Add(Vector2.Lerp(new Vector2(outerLeftBound.X, outerRightBound.Y), outerRightBound, i / numPoints));
                line3.Add(Vector2.Lerp(outerRightBound, new Vector2(outerRightBound.X, outerLeftBound.Y), i / numPoints));
                line4.Add(Vector2.Lerp(new Vector2(outerRightBound.X, outerLeftBound.Y), outerLeftBound, i / numPoints));
            }
            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.White;

            TrailDrawer.Draw(Main.spriteBatch, line1.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
            TrailDrawer.Draw(Main.spriteBatch, line2.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
            TrailDrawer.Draw(Main.spriteBatch, line3.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
            TrailDrawer.Draw(Main.spriteBatch, line4.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
        }
        private void DrawInnerSquare()
        {
            GetDrawBounds(out Vector2 outerLeftBound, out Vector2 outerRightBound, out Vector2 innerLeftBound, out Vector2 innerRightBound);
            List<Vector2> line1 = new List<Vector2>();
            List<Vector2> line2 = new List<Vector2>();
            List<Vector2> line3 = new List<Vector2>();
            List<Vector2> line4 = new List<Vector2>();
            float numPoints = 32;
            for (float i = 0; i < numPoints; i++)
            {
                line1.Add(Vector2.Lerp(innerLeftBound, new Vector2(innerLeftBound.X, innerRightBound.Y), i / numPoints));
                line2.Add(Vector2.Lerp(new Vector2(innerLeftBound.X, innerRightBound.Y), innerRightBound, i / numPoints));
                line3.Add(Vector2.Lerp(innerRightBound, new Vector2(innerRightBound.X, innerLeftBound.Y), i / numPoints));
                line4.Add(Vector2.Lerp(new Vector2(innerRightBound.X, innerLeftBound.Y), innerLeftBound, i / numPoints));
            }
            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.White;

            TrailDrawer.Draw(Main.spriteBatch, line1.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
            TrailDrawer.Draw(Main.spriteBatch, line2.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
            TrailDrawer.Draw(Main.spriteBatch, line3.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
            TrailDrawer.Draw(Main.spriteBatch, line4.ToArray(), PreviewColorFunction, PreviewWidthFunction, shader, Main.screenPosition);
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
            return Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }

        private void DrawRunePixelPrimitives(GraphicsDevice graphicsDevice)
        {
            DrawOuterSquare();
            //DrawInnerSquare();
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(Color.Cyan, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 32));
            shader.OuterColor = Color.Blue;
            shader.LaserTexture = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction, shader, Main.screenPosition);


            shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.LaserColor = Color.Blue * 0.2f;
            shader.InnerColor = Color.Lerp(Color.Cyan, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 32)) * 0.2f;
            shader.OuterColor = Color.Blue * 0.2f;
            TrailDrawer.Draw(Main.spriteBatch, OldDrawingCache, ColorFunction, WidthFunction2, shader, Main.screenPosition);
        }

        private void DrawPointer(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D pointerTexture = AssetManager.GlowMask.Shine.Value;
            Vector2 drawOrigin = pointerTexture.Size() / 2f;
            Vector2 drawCenter = DrawingPosition;

            Color glowColor2 = Color.Blue;
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
            if (Main.rand.NextBool(3))
            {
                var spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.White,
                    outerColor = Color.Blue
                };
                DustParticle dp = DustParticle.Spawn(trailPoint + Main.screenPosition, Vector2.Zero, spawnParams);
                dp.gravity = 0.06f;
                dp.fast = true;
            }
        }
    }

}
