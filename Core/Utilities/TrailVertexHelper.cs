using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using static System.Collections.Specialized.BitVector32;

namespace Stellamod.Core.Utilities
{
    public struct VertexSection
    {
        public VertexSection(int startIndex, int vertexCount, int primitiveCount)
        {
            this.startIndex = startIndex;
            this.vertexCount = vertexCount;
            this.primitiveCount = primitiveCount;
        }
        public int startIndex;
        public int vertexCount;
        public int primitiveCount;
    }

    /// <summary>
    /// Creates a pool of vertex data so we are not constantly allocating new arrays to render trails
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class TrailVertexHelper : ModSystem
    {
        private int _index;

        private int[] _trailIndexBuffer;
        private VertexPositionColorTexture[] _trailVertexBuffer;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _trailVertexBuffer = new VertexPositionColorTexture[6 * 2000];
            _trailIndexBuffer = new int[6 * 2000];
            int connectIndex = 0;
            for (int i = 0; i < _trailIndexBuffer.Length; i += 6)
            {
                _trailIndexBuffer[i] = connectIndex + 0;
                _trailIndexBuffer[i + 1] = connectIndex + 1;
                _trailIndexBuffer[i + 2] = connectIndex + 2;
                _trailIndexBuffer[i + 3] = connectIndex + 2;
                _trailIndexBuffer[i + 4] = connectIndex + 3;
                _trailIndexBuffer[i + 5] = connectIndex + 1;
                connectIndex += 4;
            }
        }

        public void Clear()
        {
            _index = 0;
        }

        public Vector2 GetTrailRotation(Vector2[] oldPos, int index)
        {
            if (oldPos.Length == 1)
                return oldPos[0];

            if (index == 0)
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.PiOver2);

            return (index == oldPos.Length - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.PiOver2);
        }

        private void Add(VertexPositionColorTexture vertex)
        {
            if (_index >= _trailVertexBuffer.Length)
                return;
            _trailVertexBuffer[_index++] = vertex;
        }
        public void CreateCircleVertices(Vector2 center, float radius, int numPoints, Func<float, Color> colorFunction,
    out VertexPositionColorTexture[] vertices, out int[] indices)
        {
            //So what I want to do is create a primtive circle
            //Before anything we need the center vertex
            Color centerColor = colorFunction(0);
            Vector2 centerTextureCoordinate = Vector2.Zero;

            int numVertices = numPoints + 1;
            vertices = new VertexPositionColorTexture[numVertices];

            VertexPositionColorTexture centerVertex = new VertexPositionColorTexture(new Vector3(center, 0), centerColor, centerTextureCoordinate);
            vertices[0] = centerVertex;

            //First let's get evenly spaced points
            for (int n = 0; n < numPoints; n++)
            {
                float ratio = (float)n / (float)numPoints;
                float radians = ratio * MathHelper.TwoPi;
                Vector2 offset = radians.ToRotationVector2();
                offset *= radius;
                Vector2 edgePoint = center + offset;

                Color color = colorFunction(1);
                Vector2 edgeTextureCoordinate = new Vector2(1, 1);
                VertexPositionColorTexture edgeVertex = new VertexPositionColorTexture(new Vector3(edgePoint, 0), color, edgeTextureCoordinate);
                vertices[n + 1] = edgeVertex;
            }

            //The index pattern for this is going to go back to vertex 0 every every vertex so
            //Example:
            //1 0 2
            //2 0 3
            //3 0 4
            //4 0 1/
            //etc, so let's create the index buffer
            indices = new int[3 * numPoints];
            int connectToIndex = 1;
            for (int n = 0; n < indices.Length; n += 3)
            {
                indices[n] = connectToIndex;
                indices[n + 1] = 0;
                indices[n + 2] = connectToIndex + 1;

                connectToIndex += 1;
            }
            indices[indices.Length - 1] = 1;
        }

        public void CreateCircleVertices(Vector2 center, float radius, int numPoints,
            out VertexPositionColorTexture[] vertices, out int[] indices)
        {
            //So what I want to do is create a primtive circle
            //Before anything we need the center vertex
            Color centerColor = Color.White;
            Vector2 centerTextureCoordinate = Vector2.Zero;

            int numVertices = numPoints * 4;
            vertices = new VertexPositionColorTexture[numVertices];
            //First let's get evenly spaced points
            float range = (float)numPoints / MathHelper.TwoPi * ExtraMath.Osc(0.85f, 1f, 1) * 0.1f;
            radius *= ExtraMath.Osc(0.6f, 1f, 4f);
            for(int n = 0; n < numPoints; n++)
            {
                float ratio = (float)n / (float)numPoints;
                float radians = ratio * MathHelper.TwoPi;
                Vector2 direction = radians.ToRotationVector2();
           


                direction *= radius * ExtraMath.Osc(0.95f, 1f, 3f, n * 16f);

                Vector2 leftOffset = direction.RotatedBy(-range);
                Vector2 rightOffset = direction.RotatedBy(range);

                Color color = Color.White;
                Vector2 edgeTextureCoordinate = new Vector2(1, 1);

                //Now we have a left and a right, ok.
                //This hsould get us our texture back
                //If not we'll introduce some more points
                VertexPositionColorTexture topleft = new VertexPositionColorTexture(new Vector3(center + leftOffset, 0), color, new Vector2(0, 1));
                VertexPositionColorTexture topRight = new VertexPositionColorTexture(new Vector3(center + rightOffset, 0), color, new Vector2(1, 1));
                VertexPositionColorTexture bottomRight = new VertexPositionColorTexture(new Vector3(center, 0), color, new Vector2(0, 0));
                VertexPositionColorTexture bottomLeft = new VertexPositionColorTexture(new Vector3(center, 0), color, new Vector2(1, 0));




                int startIndex = n * 4;
                vertices[startIndex] = topleft;
                vertices[startIndex + 1] = bottomRight;
                vertices[startIndex + 2] = topRight;
                vertices[startIndex + 3] = bottomLeft;
            }
            indices = _trailIndexBuffer;
        }
        public VertexSection FillVertexArrayNonAlloc(Vector2[] trailingPoints, Func<float, Color> colorFunc, Func<float, float> widthFunc, Vector2 offset)
        {
            const float coord1 = 0;
            const float coord2 = 1;

            int numVertices = (trailingPoints.Length - 1) * 4;
            int startIndex = _index;
            int vertexCount = 0;
            int primitiveCount = 0;


            //Keep track of what we calculated for the trail so we don't have to calculate the same point multiple times
            //Decent optimization
            Vector2 lastWidth = widthFunc(0) * Vector2.One;
            Vector2 lastOffset = GetTrailRotation(trailingPoints, 0) * lastWidth;
            Color lastColor = colorFunc(0);

            float pointCount = (float)trailingPoints.Length;
            for (int i = 0; i < trailingPoints.Length - 1; i++)
            {
                Vector2 pos1 = trailingPoints[i];
                Vector2 pos2 = trailingPoints[i + 1];

                //Cull trail points that are too far away from each other
                Vector2 diff = pos2 - pos1;
                if (diff.Length() > 1000)
                    continue;

                float uv = i / pointCount;
                float uv2 = (i + 1) / pointCount;

                Vector2 width = lastWidth;
                lastWidth = widthFunc(uv2) * Vector2.One;

                //Apply global trail offset
                pos2 += offset;
                pos1 += offset;

                Vector2 off1 = lastOffset;
                lastOffset = GetTrailRotation(trailingPoints, i + 1) * lastWidth;
          

                Color col1 = lastColor;
                lastColor = colorFunc(uv2);

                VertexPositionColorTexture topLeft = new VertexPositionColorTexture(new Vector3(pos1 + off1, 0f), col1, new Vector2(uv, coord1));
                VertexPositionColorTexture bottomRight = new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2(uv, coord2));
                VertexPositionColorTexture topRight = new VertexPositionColorTexture(new Vector3(pos2 + lastOffset, 0f), lastColor, new Vector2(uv2, coord1));
                VertexPositionColorTexture bottomLeft = new VertexPositionColorTexture(new Vector3(pos2 - lastOffset, 0f), lastColor, new Vector2(uv2, coord2));

                //0
                Add(topLeft);

                //1
                Add(bottomRight);

                //2
                Add(topRight);

                //3
                Add(bottomLeft);

                //Because of the if statement above, we can't calculate these at the end, so just do some simple math if the addition is successful
                vertexCount += 4;
                primitiveCount += 2;
            }

            VertexSection section = new VertexSection(startIndex, vertexCount, primitiveCount);
            return section;
        }


        /// <summary>
        /// Creates a new array and fills it with the vertices for the trail
        /// </summary>
        /// <param name="trailingPoints"></param>
        /// <param name="colorFunc"></param>
        /// <param name="widthFunc"></param>
        /// <returns></returns>
        public VertexPositionColorTexture[] FillVertexArray(Vector2[] trailingPoints, Func<float, Color> colorFunc, Func<float, float> widthFunc)
        {
            const float coord1 = 0;
            const float coord2 = 1;

            int numVertices = (trailingPoints.Length - 1) * 4;
            int index = 0;
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[numVertices];
            for (int i = 0; i < trailingPoints.Length - 1; i++)
            {
                float uv = i / (float)trailingPoints.Length;
                float uv2 = (i + 1) / (float)trailingPoints.Length;
                Vector2 width = widthFunc(uv) * Vector2.One;
                Vector2 width2 = widthFunc(uv2) * Vector2.One;
                Vector2 pos1 = trailingPoints[i];
                Vector2 pos2 = trailingPoints[i + 1];
                

                Vector2 off1 = MathUtil.GetRotation(trailingPoints, i) * width;
                Vector2 off2 = MathUtil.GetRotation(trailingPoints, i + 1) * width2;

                Color col1 = colorFunc(uv);
                Color col2 = colorFunc(uv2);

                VertexPositionColorTexture topLeft = new VertexPositionColorTexture(new Vector3(pos1 + off1, 0f), col1, new Vector2(uv, coord1));
                VertexPositionColorTexture bottomRight = new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2(uv, coord2));
                VertexPositionColorTexture topRight = new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2(uv2, coord1));
                VertexPositionColorTexture bottomLeft = new VertexPositionColorTexture(new Vector3(pos2 - off2, 0f), col2, new Vector2(uv2, coord2));

                //0
                vertices[index++] = topLeft;

                //1
                vertices[index++] = bottomRight;

                //2
                vertices[index++] = topRight;

                //3
                vertices[index++] = bottomLeft;

            }

            return vertices;
        }


        /// <summary>
        /// Draws primitives to the screen
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="shader"></param>
        public void DrawPrimitives(VertexPositionColorTexture[] vertices, BaseShader shader)
        {
            if (vertices.Length <= 0)
                return;

      
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            BlendState originalBlendState = graphicsDevice.BlendState;
            CullMode oldCullMode = graphicsDevice.RasterizerState.CullMode;
            SamplerState originalSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.RasterizerState.CullMode = CullMode.None;

            if (shader != null)
            {
                graphicsDevice.BlendState = shader.BlendState;
                graphicsDevice.SamplerStates[0] = shader.SamplerState;
            }


            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
              PrimitiveType.TriangleList, vertices, 0, vertices.Length, _trailIndexBuffer, 0, vertices.Length / 3);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
        public void DrawPrimitives(VertexPositionColorTexture[] vertices, int[] indices, BaseShader shader)
        {
            if (vertices.Length <= 0)
                return;


            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            BlendState originalBlendState = graphicsDevice.BlendState;
            CullMode oldCullMode = graphicsDevice.RasterizerState.CullMode;
            SamplerState originalSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.RasterizerState.CullMode = CullMode.None;

            if (shader != null)
            {
                graphicsDevice.BlendState = shader.BlendState;
                graphicsDevice.SamplerStates[0] = shader.SamplerState;
            }

            shader.ApplyPasses();
            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
              PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, vertices.Length / 2);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
        public void DrawPrimitives(VertexSection section, BaseShader shader)
        {
            if (section.primitiveCount <= 0)
                return;


            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            BlendState originalBlendState = graphicsDevice.BlendState;
            CullMode oldCullMode = graphicsDevice.RasterizerState.CullMode;
            SamplerState originalSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.RasterizerState.CullMode = CullMode.None;

            if (shader != null)
            {
                graphicsDevice.BlendState = shader.BlendState;
                graphicsDevice.SamplerStates[0] = shader.SamplerState;
            }

            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
              PrimitiveType.TriangleList, _trailVertexBuffer, 0, section.vertexCount, _trailIndexBuffer, 0, section.primitiveCount);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
        public void DrawPrimitives(VertexSection section, MiscShaderData shader)
        {
            if (section.primitiveCount <= 0)
                return;


            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            BlendState originalBlendState = graphicsDevice.BlendState;
            CullMode oldCullMode = graphicsDevice.RasterizerState.CullMode;
            SamplerState originalSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.RasterizerState.CullMode = CullMode.None;
            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
              PrimitiveType.TriangleList, _trailVertexBuffer, 0, section.vertexCount, _trailIndexBuffer, 0, section.primitiveCount);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
        public void DrawPrimitives(VertexPositionColorTexture[] vertices)
        {
            if (vertices.Length <= 0)
                return;

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
              PrimitiveType.TriangleList, vertices, 0, vertices.Length, _trailIndexBuffer, 0, vertices.Length / 2);
        }
        public void DrawPrimitives(VertexSection section)
        {
            if (section.primitiveCount <= 0)
                return;


            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
              PrimitiveType.TriangleList, _trailVertexBuffer, 0, section.vertexCount, _trailIndexBuffer, 0, section.primitiveCount);

        }
    }
}
