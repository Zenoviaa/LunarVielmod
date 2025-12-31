using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Grass
{

    [Autoload(Side = ModSide.Client)]
    public class GrassRenderer : ModSystem
    {

        public struct GrassBlade
        {
            public Color color;
            public Vector2 position;
            public Vector2 direction;
            public float length;
            public float width;
        }

        public const int Max_Blade_Count = 1000;
        private FastNoiseLite _fastNoise;
        private GrassBlade[] _grassBlades;
        private VertexPositionColor[] _grassVertices;
        private int _grassIndex;
        private int _grassVertexIndex;
        private float _windTimer;
        private float _noiseTimer;

        private Color _darkSkyColor;
        private float _backLayerInterp;
        private Vector2 _offset;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _fastNoise = new FastNoiseLite();
            _grassBlades = new GrassBlade[Max_Blade_Count];
            _grassVertices = new VertexPositionColor[Max_Blade_Count * 3];
            On_Main.CheckMonoliths += Monoliths_Hook;
        }


        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= Monoliths_Hook;
        }


        private void Monoliths_Hook(On_Main.orig_CheckMonoliths orig)
        {
            ClearGrass();
            orig();
        }

        public void ClearGrass()
        {
            _grassIndex = 0;
        }

        public override void PostDrawTiles()
        {
            base.PostDrawTiles();
            PixelationManager.QueuePrimitivesDrawAction(RenderGrassBack, DrawLayer.BackGrassTarget);
            PixelationManager.QueuePrimitivesDrawAction(RenderGrass, DrawLayer.FrontGrassTarget);
        }

        public void AddGrassPatch(Color color, Vector2 position, Vector2 direction, float length, float width, int numGrasses)
        {
            float maxOffset = 8;
            float maxLengthOffset = length / 4f;
            float maxWidthOffset = width / 4f;
            for(int g = 0; g < numGrasses; g++)
            {
                float rand = _fastNoise.GetNoise(position.X + g * 8, 0) * 0.5f + 0.5f;
                Vector2 newGrassPosition = position + Vector2.UnitX * MathHelper.Lerp(-maxOffset, maxOffset, rand);// * rand;
                float newLength = length + MathHelper.Lerp(-maxLengthOffset, maxLengthOffset, rand);
                float newWidth = width + MathHelper.Lerp(-maxWidthOffset, maxWidthOffset, rand);
                AddGrass(color, newGrassPosition, direction, newLength, newWidth);
            }
        }

        public void AddGrass(Color color, Vector2 position, Vector2 direction, float length, float width)
        {
            //HIt the maximum number of grasses
            if (_grassIndex >= _grassBlades.Length)
                return;
            ref GrassBlade blade = ref _grassBlades[_grassIndex];
            blade.color = color;
            blade.position = position;
            blade.direction = direction;
            blade.length = length;
            blade.width = width;
            _grassIndex++;
        }


        public void AddGrassVertices(int bladeIndex, Vector2 direction)
        {
            //HIt the maximum number of grasses
            ref GrassBlade blade = ref _grassBlades[bladeIndex];
            Color color = blade.color;
            color = Color.Lerp(color, _darkSkyColor, _backLayerInterp);

            int startIndex = bladeIndex * 3;
            ref VertexPositionColor bottomLeft = ref _grassVertices[startIndex];
            ref VertexPositionColor bottomRight = ref _grassVertices[startIndex + 1];
            ref VertexPositionColor top = ref _grassVertices[startIndex + 2];

            Vector2 perpDirection = direction.RotatedBy(MathHelper.PiOver2);
            Vector2 perpOffset = perpDirection * blade.width * 0.5f;

            //Calculate vertice positions
            bottomLeft.Position = new Vector3(blade.position - perpOffset + _offset, 0);
            bottomRight.Position = new Vector3(blade.position + perpOffset + _offset, 0);

            Vector2 topBladePosition = blade.position + direction * blade.length;


            //Apply offset based on if something is interacting with this grass
            //For now we can just check the player
            float dist = Vector2.Distance(Main.LocalPlayer.Center, blade.position);
            float interactDistance = 64;
            float interp = dist / interactDistance;
            interp = 1f - MathHelper.Clamp(interp, 0, 1);
            float x = MathF.Sign(Main.LocalPlayer.Center.X - blade.position.X);
            topBladePosition += Vector2.UnitX * -x * interp * blade.length * 0.05f;

            //Round the x position to prevent blades from fading out of existence
            topBladePosition.X = MathF.Floor(topBladePosition.X);
            topBladePosition.Y = MathF.Floor(topBladePosition.Y);
            top.Position = new Vector3(topBladePosition + _offset, 0);

            Color topColor = Color.Lerp(color, Main.ColorOfTheSkies, 0.5f);

            //Apply noise to the top color
            float noiseSample = _fastNoise.GetNoise(blade.position.X + _noiseTimer, 0) * 0.5f + 0.5f;
            float bladeOsc = ExtraMath.Osc(0f, 1f, 0f, blade.position.X) * 0.3f;
            Color bottomColor = Color.Lerp(color, Color.Black, bladeOsc + noiseSample * 0.4f);
            bottomLeft.Color = bottomColor;
            bottomRight.Color = bottomColor;
            top.Color = topColor;
        }

        public void DarkenVertex(int bladeIndex)
        {
            //HIt the maximum number of grasses
            int startIndex = bladeIndex * 3;
            ref VertexPositionColor bottomLeft = ref _grassVertices[startIndex];
            ref VertexPositionColor bottomRight = ref _grassVertices[startIndex + 1];
            ref VertexPositionColor top = ref _grassVertices[startIndex + 2];

            bottomLeft.Color = Color.Lerp(bottomLeft.Color, Color.Black, _backLayerInterp);
            bottomRight.Color = Color.Lerp(bottomRight.Color, Color.Black, _backLayerInterp);
            top.Color = Color.Lerp(top.Color, Color.Black, _backLayerInterp);

            top.Position += new Vector3(_offset, 0);
            bottomLeft.Position += new Vector3(_offset, 0);
            bottomRight.Position += new Vector3(_offset, 0);
        }

        private void PrepareGrassVertices()
        {
            float windRange = MathHelper.ToRadians(Main.windSpeedCurrent * 5);
            _windTimer += Main.windSpeedCurrent * 0.15f;
            _noiseTimer = _windTimer * 2;

            _grassVertexIndex = 0;
            _darkSkyColor = Color.Lerp(Main.ColorOfTheSkies, Color.Black, 0.75f);
            _fastNoise.SetFrequency(0.05f);

            //Simulate wind and populate the draw buffer with the grass vertex data
            FastParallel.For(0, _grassIndex, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    ref GrassBlade blade = ref _grassBlades[i];
                    float bladeOffset = _windTimer + blade.position.X;
                    float osc = ExtraMath.Osc(0f, 1f, 0, offset: bladeOffset);
                    float windRadians = MathHelper.Lerp(-windRange, windRange, osc);
                    Vector2 newDirection = blade.direction.RotatedBy(windRadians);
                    AddGrassVertices(i, newDirection);
                }
            });
            _grassVertexIndex = _grassIndex * 3;
        }
        private void PrepareDarkGrassVertices()
        {
            //Simulate wind and populate the draw buffer with the grass vertex data
            FastParallel.For(0, _grassIndex, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    DarkenVertex(i);
                }
            });
            _grassVertexIndex = _grassIndex * 3;
        }

        private void RenderGrass(GraphicsDevice graphicsDevice)
        {
            //Yuh
            _offset = Vector2.Zero;
            _backLayerInterp = 0;
            PrepareGrassVertices();
            if (_grassVertexIndex <= 0)
                return;

            //Prepare the graphics device

            var shader = GrassShader.Instance;
            shader.ApplyPasses();

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleList, _grassVertices, 0, _grassVertexIndex / 3);

        }
        private void RenderGrassBack(GraphicsDevice graphicsDevice)
        {
            //Yuh
            _backLayerInterp = 0.55f;
            _offset = new Vector2(24, 0);
            PrepareDarkGrassVertices();
            if (_grassVertexIndex <= 0)
                return;

            //Prepare the graphics device

            var shader = GrassShader.Instance;
            shader.ApplyPasses();

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleList, _grassVertices, 0, _grassVertexIndex / 3);

        }
    }
}
