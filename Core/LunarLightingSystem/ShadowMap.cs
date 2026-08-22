using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Core.LunarLightingSystem
{
    public class ShadowMap
    {
        private Vector2[] _shadowCoordinates;
        public ShadowMap(int maxShadowCasters, int resolution)
        {
            MaxShadowCasters = maxShadowCasters;
            Resolution = resolution;
            _shadowCoordinates = new Vector2[maxShadowCasters * Resolution];
        }
        public Texture2D Texture { get; private set; }
        public readonly int Resolution;
        public readonly int MaxShadowCasters;
        public void Clear()
        {
            for(int i = 0; i < _shadowCoordinates.Length; i++)
            {
                _shadowCoordinates[i] = new Vector2( 0, 10000);
            }
        }
        public void Dispose()
        {
            Texture?.Dispose();
        }

        public Texture2D Output()
        {
            if(Texture == null || Texture.Width != Resolution)
            {
                Texture?.Dispose();
                Texture = new Texture2D(Main.graphics.GraphicsDevice, Resolution, MaxShadowCasters, false, SurfaceFormat.Vector2);
            }

            Texture.SetData(_shadowCoordinates);
            return Texture;
        }

        public void RayMarch(int lightIndex, Vector2 lightPosition, float distance)
        {
           // Stopwatch sw = Stopwatch.StartNew();
            //What we're gonna do is raycast in all directions and output the geometry
            //TODO: remove in world check
            float d = 16;
            float maxSteps = distance / d; 
            for(int i = 0; i < Resolution; i++)
            {
                float radians = (float)i / (float)Resolution;
                Vector2 rayDirection = (radians*MathHelper.TwoPi).ToRotationVector2();
                Vector2 rayPos = lightPosition;
                int steps = 0;
                while (steps < maxSteps)
                {
                    Point tilePoint = rayPos.ToTileCoordinates();
                    if(!WorldGen.InWorld(tilePoint.X, tilePoint.Y))
                    {
                        break;
                    }
                    Tile tile = Main.tile[tilePoint];
                    if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                    {
                        //Submit for shadows
                        break;
                    }
                    rayPos += rayDirection * d;
                    steps++;
                }

                //Normalize to 1
                Vector2 shadowCoordinate = new Vector2(radians, d * steps / distance);
                _shadowCoordinates[lightIndex * Resolution + i] = shadowCoordinate;
            }
           //sw.Stop();
        //    Main.NewText(sw.ElapsedTicks);

        }

        /*
        public void MapLight(int lightIndex, Vector2 lightPosition, float distance)
        {
            Vector2 topLeftWorld = lightPosition - new Vector2(distance / 2f);
            Vector2 bottomRightWorld = lightPosition + new Vector2(distance / 2f);
            Point topLeft = topLeftWorld.ToTileCoordinates();
            Point bottomRight = bottomRightWorld.ToTileCoordinates();

            topLeft = TileUtilities.Clamp(topLeft);
            bottomRight = TileUtilities.Clamp(bottomRight); 
            for(int x = topLeft.X; x < bottomRight.X; x++)
            {
                for(int y =topLeft.Y; y < bottomRight.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile || !Main.tileSolid[tile.TileType])
                        continue;

                    Vector2 worldCoordinates = new Point(x, y).ToWorldCoordinates();
                    Vector2 lightVector = worldCoordinates - lightPosition;
                    float angle = lightVector.ToRotation();
                    float distToSolidTile = lightVector.Length();

                    int indexOffset = (int)(angle / MathHelper.TwoPi * (Resolution - 1));
                    int index = lightIndex * Resolution + indexOffset;
                    ref Vector2 currentShadowCoordinate = ref _shadowCoordinates[index];
                    if (currentShadowCoordinate.Y < distToSolidTile)
                        continue;
                    currentShadowCoordinate.X = angle;
                    currentShadowCoordinate.Y = distToSolidTile;
                }
            }
        }
        */
        public int IndexOf(int lightIndex, int i)
        {
            return lightIndex * Resolution + i; 
        }

        public void PreviewCoordinates(SpriteBatch spriteBatch, int lightIndex, Vector2 lightPosition)
        {
            for(int i = 0; i < Resolution; i++)
            {
                Vector2 coordinate = _shadowCoordinates[IndexOf(lightIndex, i)];
                Vector2 offset = (coordinate.X*MathHelper.TwoPi).ToRotationVector2() * coordinate.Y * 400;
                Primitives2D.DrawCircle(spriteBatch, lightPosition + offset - Main.screenPosition, 16, 8, Color.Red);
            }
        }
    }
}
