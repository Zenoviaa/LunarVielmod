using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using static ReLogic.Peripherals.RGB.Corsair.CorsairDeviceGroup;
using static Stellamod.Core.AssetReferences.Assets.NoiseTextures;

namespace Stellamod.Core.WindLineSystem
{
    public class WindLine
    {
        public Vector2 position;
        public Vector2 velocity;
        public Vector2[] oldPosition;
        public float lifetime;
        public bool active;
        public float width;
        public WindLine(int trailCacheLength)
        {
            oldPosition = new Vector2[trailCacheLength];
        }
    }

    [Autoload(Side =ModSide.Client)]
    public class WindLineRenderer : ModSystem
    {
        private WindLine[] _windLines;
        int _length;
        VertexPositionColorTexture[] _vertexBuffer;
        public const int TRAIL_CACHE_LENGTH = 24;
        public const int MAX_WINDLINE_COUNT = 50;
        public const int MAX_VERTEX_COUNT = 6 * MAX_WINDLINE_COUNT * TRAIL_CACHE_LENGTH;
        public override void Load()
        {
            base.Load();
            _windLines = new WindLine[MAX_WINDLINE_COUNT];
            for (int i = 0; i < MAX_WINDLINE_COUNT; i++)
            {
                _windLines[i] = new WindLine(trailCacheLength: TRAIL_CACHE_LENGTH);
            }
            _vertexBuffer = new VertexPositionColorTexture[MAX_VERTEX_COUNT];
            On_Main.CheckMonoliths += PrepareRendering;
        }

        private void PrepareRendering(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (Main.gameMenu)
                return;
            if (!ShouldRender())
                return;
            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedWindlines, DrawLayer.OverWater);
        }

        private bool ShouldRender()
        {
            return _length != 0;
        }


        private void RenderPixelatedWindlines(GraphicsDevice graphicsDevice)
        {
            var windLineShader = BasicLaserAlphaShader.Instance;
            windLineShader.LaserTexture = AssetReferences.Assets.NoiseTextures.GlowTrailNoBlack.Asset;
            windLineShader.Time = 0;
            windLineShader.ApplyPasses();

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(_vertexBuffer, DrawUtilities.Indices, _length / 2, windLineShader.Effect);
        }


        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            UpdateWindLines();
            float windSpeed = MathHelper.Clamp(MathF.Abs(Main.windSpeedCurrent), 0f, 1f);
            int denom = (int)MathHelper.Lerp(60, 30, windSpeed);

            int yPosition = Main.LocalPlayer.position.ToTileCoordinates().Y;
            bool aboveSurface = yPosition < (int)Main.worldSurface;
            if (MoonUtils.Rand.NextBool(denom) && windSpeed > 0.3f && aboveSurface)
            {
                Vector2 pos = Main.Camera.Center;
                float height = Main.screenHeight / 2f;
                pos.Y += Main.rand.NextFloat(-height, height);


                float edgeOffset = Main.screenWidth / 2f;
                Vector2 initialWindVelocity = Main.windSpeedCurrent * Vector2.UnitX * 40;
                edgeOffset *= -MathF.Sign(Main.windSpeedCurrent);
                pos.X += edgeOffset;

                NewWindLine(pos, initialWindVelocity);
            }
        }

        private void UpdateWindLines()
        {
            for (int i = 0; i < _windLines.Length; i++)
            {
                WindLine line = _windLines[i];
                //Don't update lines that aren't doing anything
                if (!line.active)
                    continue;


                for (int j = line.oldPosition.Length - 1; j > 0; j--)
                {
                    line.oldPosition[j] = line.oldPosition[j - 1];
                }

                line.oldPosition[0] = line.position;
                line.position += line.velocity;
                line.velocity.Y += MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + i) * 0.05f;
                line.lifetime--;
                if (line.lifetime <= 0)
                {
                    line.active = false;
                }
            }

            PrepareWindLinesForDrawing();
        }

        public Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio)) * 0.3f;
        }

        public float GetTrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0f, 4, EasingFunction.QuadraticBump(completionRatio));
        }
        public float GetTrailWidth(float completionRatio, float windWidth)
        {
            return MathHelper.Lerp(0f, 3, EasingFunction.QuadraticBump(completionRatio)) * windWidth;
        }

        public void PrepareWindLinesForDrawing()
        {
            _length = 0;
            for (var i = 0; i < _windLines.Length; i++)
            {
                if (_length >= _vertexBuffer.Length)
                    break;
                WindLine line = _windLines[i];
                if (!line.active)
                    continue;

                float outScale = line.lifetime / 30f;
                outScale = EasingFunction.InOutSine(outScale);
                float w = line.width * outScale;
                float WindGetTrailWidth(float ratio)
                {
                    return GetTrailWidth(ratio, w);
                }

                var points = DrawUtilities.PrepareSimpleTrailingNoSmoothing(line.oldPosition, GetTrailColor, WindGetTrailWidth);
                foreach (var p in points)
                {
                    if (_length >= _vertexBuffer.Length)
                        break;
                    _vertexBuffer[_length++] = p;
                }
            }
        }

        private WindLine GetInactiveWindLine()
        {
            for (int i = 0; i < _windLines.Length; i++)
            {
                WindLine windLine = _windLines[i];
                if (!windLine.active)
                    return windLine;
            }
            return null;
        }

        private void NewWindLine(Vector2 initialPosition, Vector2 initialVelocity)
        {
            WindLine windLine = GetInactiveWindLine();
            if (windLine == null)
                return;
            windLine.position = initialPosition;
            for (int i = 0; i < windLine.oldPosition.Length; i++)
            {
                windLine.oldPosition[i] = windLine.position;
            }
            windLine.velocity = initialVelocity;
            windLine.active = true;
            windLine.lifetime = 180;
            windLine.width = MoonUtils.Rand.NextFloat(0.95f, 3f);
        }
    }
}

