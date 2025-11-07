

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
using Terraria;
using Terraria.ID;

namespace Stellamod.Core.LunarLightingSystem
{
    public class LightCaster
    {
        public int lightingIndex;
        public int radius;
        public Texture2D texture;
        private int IndexOf(int x, int y)
        {
            int index = x + y * radius;
            return index;
        }

        private static Vector3 RayTrace(Vector2 position, Vector2 lightPosition,
            Vector3 lightColor, float lightRadius, float lightIntensity)
        {
            Vector2 lightVector = (lightPosition - position);
            Vector2 normalizedDirection = lightVector.SafeNormalize(Vector2.Zero);
            if (normalizedDirection == Vector2.Zero)
                return Vector3.One;

            float distance = lightVector.Length();
            //Too far, skip the calculation
            if (distance > lightRadius)
                return Vector3.Zero;

            //Calculate how much to move in a single step
            float stepLength = 2;
            Vector2 stepDirection = normalizedDirection * stepLength;

            Vector2 rayPosition = position;
            float maxSteps = distance / stepLength;
            float fallOff = 0f;
            for (int i = 0; i < maxSteps; i++)
            {
                rayPosition += stepDirection;

                int x = (int)rayPosition.X / 16;
                int y = (int)rayPosition.Y / 16;
                if (!WorldGen.InWorld(x, y))
                    return LunarLighting.AmbientLight;

                Tile tile = Main.tile[x, y];
                bool hasCollision = Main.tileSolid[tile.TileType] && tile.HasTile;
                bool openToSun = tile.WallType == WallID.None;
                if (hasCollision)
                {
                    fallOff += 0.1f;
                    if (fallOff >= 1f)
                    {
                        break;
                    }
                }
                else
                {
                    fallOff += 0.01f;
                }
            }

            //Return the light
            //Calculating how much attenuation to give it
            float attenuation = 1.0f - (distance / (lightRadius / 2f));
            Vector3 pixelRGB = LunarLighting.AmbientLight * (lightColor * lightIntensity * attenuation * (1.0f - fallOff));
            return pixelRGB;
        }

        private static Vector3 RayTrace(Vector2 position, PointLight pointLight)
        {
            Vector3 lightColor = pointLight.color;
            float lightRadius = pointLight.radius;
            float lightIntensity = pointLight.intensity;
            return RayTrace(position, pointLight.position, lightColor, lightRadius, lightIntensity);
        }

        public void CastLight(int pixelRadius, PointLight pointLight)
        {
            return;
            radius = pixelRadius;
            lightingIndex = LunarLighting.UseLightingIndex();
            if (lightingIndex == -1)
                return;

            Color[] lightingData = LunarLighting.GetLightingData(lightingIndex);

            Vector2 offset = (new Vector2(pixelRadius, pixelRadius) / 2) * LunarLighting.DownSamples;
            Vector2 topLeftPixelWorld = pointLight.position - offset;
            Vector2 bottomRightPixelWorld = pointLight.position + offset;
            FastParallel.For(0, radius, delegate (int start, int end, object context)
            {
                for (int x = start; x < end; x++)
                {
                    for (int y = 0; y < radius; y++)
                    {
                        int index = IndexOf(x, y);
                        lightingData[index] = Color.Black;

                        float xInterpolant = ((float)x / (float)radius);
                        float yInterpolant = ((float)y / (float)radius);
                        float worldX = MathHelper.Lerp(topLeftPixelWorld.X, bottomRightPixelWorld.X, xInterpolant);
                        float worldY = MathHelper.Lerp(topLeftPixelWorld.Y, bottomRightPixelWorld.Y, yInterpolant);
                        Vector2 position = new Vector2(worldX, worldY);// + topLeftPixelWorld;
                        lightingData[index] = RayTrace(position, pointLight).ToColor();
                    }
                }
            });

            texture = LunarLighting.GetLightCastTexture(lightingIndex);
            texture.SetData(lightingData);
        }

        public void ReleaseLight()
        {
            LunarLighting.ReleaseLightingIndex(lightingIndex);
        }
    }
}
