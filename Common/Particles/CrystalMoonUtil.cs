using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles
{
    internal static class ParticleUtils
    {



        public static int ParticleType<T>() where T : Particle => ModContent.GetInstance<T>()?.Type ?? 0;

        public static bool OnScreen(Vector2 pos) => pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;

        public static bool OnScreen(Rectangle rect) => rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));

        public static bool OnScreen(Vector2 pos, Vector2 size) => OnScreen(new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y));

        public static void DrawTrail(GraphicsDevice device, Action draw
        , BlendState blendState = null, SamplerState samplerState = null, RasterizerState rasterizerState = null)
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            BlendState originalBlendState = Main.graphics.GraphicsDevice.BlendState;
            SamplerState originalSamplerState = Main.graphics.GraphicsDevice.SamplerStates[0];

            device.BlendState = blendState ?? originalBlendState;
            device.SamplerStates[0] = samplerState ?? originalSamplerState;
            device.RasterizerState = rasterizerState ?? originalState;

            draw();

            device.RasterizerState = originalState;
            device.BlendState = originalBlendState;
            device.SamplerStates[0] = originalSamplerState;
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public static Matrix GetTransfromMaxrix()
        {
            Vector3 screenPosition = new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0);
            Matrix world = Matrix.CreateTranslation(-screenPosition);
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            return world * view * projection;
        }

        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.X, vector2.Y, 0);
        }

        public static float EllipticalEase(float rotation, float halfShortAxis, float halfLongAxis)
        {
            float halfFocalLength2 = (halfLongAxis * halfLongAxis) - (halfShortAxis * halfShortAxis);
            float cosX = MathF.Cos(rotation);
            return (halfLongAxis * halfShortAxis) / MathF.Sqrt(halfLongAxis * halfLongAxis - halfFocalLength2 * cosX * cosX);
        }
    }
}
