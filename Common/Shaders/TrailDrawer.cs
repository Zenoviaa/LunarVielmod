using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders
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


        public static void DrawWithMiscShader(SpriteBatch spriteBatch,
            Vector2[] oldPos,
            float[] oldRot,
            Func<float, Color> colorFunc,
            Func<float, float> widthFunc,
            MiscShaderData shader,
            Vector2? offset = null)
        {
            shader.Apply();
            Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;
            float numPoints = oldPos.Length * 2;
            Vector2[] trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);
            TrailVertexHelper trailVertexCache = ModContent.GetInstance<TrailVertexHelper>();
            trailVertexCache.Clear();
            VertexSection section = trailVertexCache.FillVertexArrayNonAlloc(trailingPoints, colorFunc, widthFunc, trailOffset);
            trailVertexCache.DrawPrimitives(section, shader);
        }

        public static void ClearPrimitives()
        {
            TrailVertexHelper trailVertexCache = ModContent.GetInstance<TrailVertexHelper>();
            trailVertexCache.Clear();
        }

        public static void PreparePrimitives(Vector2[] oldPos,
            Func<float, Color> colorFunc,
            Func<float, float> widthFunc,
            Vector2? offset = null)
        {
            TrailVertexHelper trailVertexCache = ModContent.GetInstance<TrailVertexHelper>();
            Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;
            float numPoints = oldPos.Length * 2;
            Vector2[] trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);
            VertexSection section = trailVertexCache.FillVertexArrayNonAlloc(trailingPoints, colorFunc, widthFunc, trailOffset);
        }
        public static VertexPositionColorTexture[] PrepareVertices(Vector2[] oldPos,
            Func<float, Color> colorFunc,
            Func<float, float> widthFunc,
            bool useSmoothing= true,
            Vector2? offset = null)
        {
            TrailVertexHelper trailVertexCache = ModContent.GetInstance<TrailVertexHelper>();
            Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;
            float numPoints = oldPos.Length * 2;
            Vector2[] trailingPoints;
            if (useSmoothing)
            {
                trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);
            }
            else
            {
                trailingPoints = oldPos;
            }
               
            return trailVertexCache.FillVertexArray(trailingPoints, colorFunc, widthFunc, trailOffset);
        }

        public static void DrawCached(BaseShader shader)
        {
            shader.Apply();
            ApplyPasses(shader.Effect);
            ModContent.GetInstance<TrailVertexHelper>().DrawCachedPrimitives();
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
            Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;
            float numPoints = oldPos.Length * 2;

            Vector2[] trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);

            TrailVertexHelper trailVertexCache = ModContent.GetInstance<TrailVertexHelper>();
            trailVertexCache.Clear();
            VertexSection section = trailVertexCache.FillVertexArrayNonAlloc(trailingPoints, colorFunc, widthFunc, trailOffset);
            trailVertexCache.DrawPrimitives(section, shader);
            if (shader != null)
            {
                shader.FillShape = false;
            }
        }
        public static void Draw(
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
            Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;
            float numPoints = oldPos.Length * 2;

            Vector2[] trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);

            TrailVertexHelper trailVertexCache = ModContent.GetInstance<TrailVertexHelper>();
            trailVertexCache.Clear();
            VertexSection section = trailVertexCache.FillVertexArrayNonAlloc(trailingPoints, colorFunc, widthFunc, trailOffset);
            trailVertexCache.DrawPrimitives(section, shader);
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
            Draw(spriteBatch, oldPos, null, colorFunc, widthFunc, shader, offset);
        }
    }
}
