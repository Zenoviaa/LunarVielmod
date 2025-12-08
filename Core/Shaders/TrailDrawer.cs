using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Systems.MiscellaneousMath;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Core.Shaders
{
    //DEV NOTES / Thinking:
    //Ok
    //So, this is actually a pretty unoptimized system because we're allocating new arrays every single frame just to draw primitives
    //What we should do instead is preallocate a big buffer of vertices, and then just use parts of that buffer for our calculations
    //This will be much more efficient

    //We could probably have a trail drawing interface too? Might not be a good idea since some of them are on the pixelation draw
    //But we can have both the old system and the new system
    //Now there's twoo ways to do this

    //Either we initialize a buffer when they are needed, the same lengthof the trail
    //Or we just have 1 massive buffer and projectiles index into them
    //The latter is probably more performant?
    

    //BUT ALSO, if we use interfaces, then projectiles don't need to know much about how their trailing works, and it'll be loosely coupled

    //Ok but what if we have a lot of primitives on the screen? We need to batch the draw calls
    //In that case a bigger buffer probably is a good idea
    //I don't want zemmie to lag

    //ALRIGHT, new plan
    //We just create a new renderer for each new spammable effect that we make
    //They'll have their own buffers and that'll allow us to easily batch all calls to it, since it'll just draw once at the end of the frame
    //Yeah, that's a good idea.
    //We should have a base class then, and probably use a generic?

    //In terms of how projectiles will actually use it, we can just do a simple SetDefaults/ClearBuffer at the beginning of the frame
    //Then each projectile will just use it
    //This also makes the code easier to reuse since if we want to swap out an effect it'd be pretty easy

    //The only downside to this approach is that they're all going to share the same shader settings
    //So we'd need to either make custom vertex data to get around that or really make a LOT of different subvariations for this optimization to be worth it
    //That said though, it's not like we're deleting the old system, we're just making an alternative system for optimization purposes, the old system will eventually not be used anymore
    //But for like bosses and things that spam similar effects this is necessary
    //This will get a lot of draw code all into a single pass :) 

    public interface ITrail
    {
        void Update(Vector2[] oldPos);
        void Draw();
    }

    public class PrimitiveTrailManager : ITrail
    {
        public delegate Color GetTrailColorFunction(float completionRatio);
        public delegate float GetTrailWidthFunction(float completionRatio);
        public PrimitiveTrailManager(int trailCacheLength)
        {

        }

        public GetTrailColorFunction GetTrailColor;
        public GetTrailWidthFunction GetTrailWidth;
        
        public void Update(Vector2[] oldPos)
        {

        }

        public void Draw()
        {

        }
    }

    public class TrailDrawer
    {
        public static Matrix WorldViewPoint2
        {
            get
            {
                Vector3 screenPosition = new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0);
                Matrix world = Matrix.CreateTranslation(-screenPosition);
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
                return world * view * projection;
            }
        }

        private static void ApplyPasses(Effect effect)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
        }


        public static void CalculateVerticesTris(Vector2[] trailingPoints, Func<float, Color> colorFunc,
            Func<float, float> widthFunc, List<VertexPositionColorTexture> vertices)
        {

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
                float uvAdd = 0;
                float uvMultiplier = 1;
                float coord1 = 0;
                float coord2 = 1;
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 + off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 - off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
            }
        }


        private static List<VertexPositionColorTexture> CalculateVertices(Vector2[] oldPos,
            float[] oldRot,
            Func<float, Color> colorFunc,
            Func<float, float> widthFunc,
            Vector2? offset = null)
        {
            Vector2 o = offset == null ? Vector2.Zero : (Vector2)offset;
            var vertices = new List<VertexPositionColorTexture>();
            oldPos = MathUtil.RemoveZeros(oldPos, o);
            MathUtil.LerpTrailPoints(oldPos, out Vector2[] trailingPoints);
            CalculateVerticesTris(trailingPoints, colorFunc, widthFunc, vertices);
            return vertices;
        }


        public static void DrawWithMiscShader(SpriteBatch spriteBatch,
            Vector2[] oldPos,
            float[] oldRot,
            Func<float, Color> colorFunc,
            Func<float, float> widthFunc,
            MiscShaderData shader,
            Vector2? offset = null)
        {
            spriteBatch.End();
            spriteBatch.Begin();
            shader.Apply();
            var vertices = CalculateVertices(
                oldPos, oldRot, colorFunc, widthFunc, offset);
            DrawPrimsTriangles(vertices, null);
            spriteBatch.End();
            spriteBatch.Begin();
        }

        public static void Draw(SpriteBatch spriteBatch,
            Vector2[] oldPos,
            float[] oldRot,
            Func<float, Color> colorFunc,
            Func<float, float> widthFunc,
            BaseShader shader,
            Vector2? offset = null)
        {
            //Apply passes
            if (shader != null)
            {
                shader.Apply();
                ApplyPasses(shader.Effect);
                if (shader.FillShape)
                {
                    Vector2[] filledPos = new Vector2[oldPos.Length + 1];
                    for (int i = 0; i < oldPos.Length; i++)
                    {
                        filledPos[i] = oldPos[i];
                    }
                    filledPos[filledPos.Length - 1] = oldPos[0];
                    oldPos = filledPos;
                }
            }

            //
            var vertices = CalculateVertices(oldPos, oldRot, colorFunc, widthFunc, offset);
            DrawPrimsTriangles(vertices, shader);

            if (shader != null)
            {
                shader.FillShape = false;

            }

        }
        public static void Draw(SpriteBatch spriteBatch,
     Vector2[] oldPos,
     Func<float, Color> colorFunc,
     Func<float, float> widthFunc,
     BaseShader shader,
     Vector2? offset = null)
        {
            //Apply passes
            if (shader != null)
            {
                shader.Apply();
                ApplyPasses(shader.Effect);
                if (shader.FillShape)
                {
                    Vector2[] filledPos = new Vector2[oldPos.Length + 1];
                    for (int i = 0; i < oldPos.Length; i++)
                    {
                        filledPos[i] = oldPos[i];
                    }
                    filledPos[filledPos.Length - 1] = oldPos[0];
                    oldPos = filledPos;
                }
            }

            //
            var vertices = CalculateVertices(oldPos, null, colorFunc, widthFunc, offset);
            DrawPrimsTriangles(vertices, shader);

            if (shader != null)
            {
                shader.FillShape = false;

            }

        }


        private static void DrawPrimsTriangles(List<VertexPositionColorTexture> vertices, BaseShader shader)
        {
            if (vertices.Count % 6 != 0 || vertices.Count <= 3)
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

            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
    }
}
