using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

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

    public class WindLineRenderer : ModSystem
    {
        private WindLine[] _windLines;
        private VertexPositionColorTexture[] _vertexBuffer;
        private int _vertexCount;

        public const int Max_WindLine_Count = 50;
        public const int Max_Vertice_Count = 6 * Max_WindLine_Count * 20;
        public override void Load()
        {
            base.Load();
            _windLines = new WindLine[Max_WindLine_Count];
            for (int i = 0; i < Max_WindLine_Count; i++)
            {
                _windLines[i] = new WindLine(trailCacheLength: 24);
            }
            _vertexBuffer = new VertexPositionColorTexture[Max_Vertice_Count];
        }


        private bool ShouldRender()
        {
            return _vertexCount >= 3;
        }


        private void RenderPixelatedWindlines(GraphicsDevice graphicsDevice)
        {
            if (!ShouldRender())
                return;

            var windLineShader = BasicLaserAlphaShader.Instance;
            windLineShader.LaserTexture = TrailRegistry.LightningTrail2;
            windLineShader.ApplyPasses();

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            graphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList, _vertexBuffer, 0, _vertexCount / 3);
        }


        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            UpdateWindLines();

            float windSpeed = MathHelper.Clamp(MathF.Abs(Main.windSpeedCurrent), 0f, 1f);
            int denom = (int)MathHelper.Lerp(60, 30, windSpeed);


            int yPosition = Main.LocalPlayer.position.ToTileCoordinates().Y;
            bool aboveSurface = yPosition < (int)Main.worldSurface;
            if (Main.rand.NextBool(denom) && windSpeed > 0.3f && aboveSurface)
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

            if (!ShouldRender())
                return;

            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedWindlines, DrawLayer.OverPlayers);
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
            return MathHelper.Lerp(0f, 4, EasingFunction.QuadraticBump(completionRatio)) * windWidth;
        }
        public void AddVertex(VertexPositionColorTexture vertex)
        {
            if (_vertexCount < _vertexBuffer.Length)
            {
                _vertexBuffer[_vertexCount] = vertex;
                _vertexCount++;
            }
        }

        public void PrepareWindLinesForDrawing()
        {
            _vertexCount = 0;
            for (int i = 0; i < _windLines.Length; i++)
            {
                WindLine line = _windLines[i];
                //Don't update lines that aren't doing anything
                if (!line.active)
                    continue;

                for (int j = 0; j < line.oldPosition.Length - 1; j++)
                {
                    float uv = j / (float)line.oldPosition.Length;
                    float uv2 = (j + 1) / (float)line.oldPosition.Length;
                    float outScale = line.lifetime / 30f;
                    outScale = EasingFunction.InOutSine(outScale);

                    float w = line.width * outScale;

                    Vector2 width = GetTrailWidth(uv, w) * Vector2.One;
                    Vector2 width2 = GetTrailWidth(uv2, w) * Vector2.One;
                    Vector2 pos1 = line.oldPosition[j];
                    Vector2 pos2 = line.oldPosition[j + 1];

                    Vector2 off1 = MathUtil.GetRotation(line.oldPosition, j) * width;
                    Vector2 off2 = MathUtil.GetRotation(line.oldPosition, j + 1) * width2;

                    Color col1 = GetTrailColor(uv);
                    Color col2 = GetTrailColor(uv2);
                    float uvAdd = 0;
                    float uvMultiplier = 1;
                    float coord1 = 0;
                    float coord2 = 1;

                    AddVertex(new VertexPositionColorTexture(new Vector3(pos1 + off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord1)));
                    AddVertex(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
                    AddVertex(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                    AddVertex(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                    AddVertex(new VertexPositionColorTexture(new Vector3(pos2 - off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord2)));
                    AddVertex(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
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
            windLine.width = Main.rand.NextFloat(0.95f, 3f);


        }
    }
}

