using ReLogic.Content;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;

[Autoload(Side = ModSide.Client)]
public class FlyingSoilSystem : ModSystem
{
    private struct SoilBlock
    {
        public int topLeftVariant;
        public int topRightVariant;
        public int bottomLeftVariant;
        public int bottomRightVariant;
    }

    private struct Soil
    {
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float direction;
        public float timer;
        public int tileType;
        public bool active;
    }

    private int _soilIndex;
    private Soil[] _soils;
    public override void Load()
    {
        base.Load();
        _soils = new Soil[100];
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        for (int i = 0; i < _soils.Length; i++)
        {
            ref Soil soil = ref _soils[i];
            if (!soil.active)
                continue;

            soil.timer++;
            soil.position += soil.velocity;
            soil.rotation += soil.direction * 0.05f;
            soil.velocity.Y += 0.25f;
            if (soil.timer > 90)
            {
                soil.active = false;
            }
        }
    }

    public void NewSoil(Vector2 worldPosition, Vector2 initialVelocity)
    {
        Point point = worldPosition.ToTileCoordinates();
        while (!WorldGen.SolidTile(point))
            point.Y++;
        Tile tile = Main.tile[point];

        for (int i = 0; i < _soils.Length; i++)
        {
            _soilIndex++;
            _soilIndex %= _soils.Length;
            if (!_soils[_soilIndex].active)
            {
                break;
            }
        }

        Vector2 startPosition = point.ToWorldCoordinates();
        ref Soil soil = ref (_soils[_soilIndex]);
        soil.timer = 0;
        soil.active = true;
        soil.position = startPosition;
        soil.velocity = initialVelocity;
        soil.direction = Main.rand.NextBool(2) ? -1 : 1;
        soil.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        soil.tileType = tile.TileType;
    }
    private void DrawSoil(SpriteBatch sb, Vector2 screenPos)
    {
        for (int i = 0; i < _soils.Length; i++)
        {
            ref Soil soil = ref _soils[i];
            if (!soil.active)
                continue;
            Vector2 scale = Vector2.Lerp(Vector2.One, Vector2.Zero, soil.timer / 90f);
            Vector2 x = Vector2.UnitX * 8;
            x = x.RotatedBy(soil.rotation);
            x *= scale;
            Vector2 y = Vector2.UnitY * 8;
            y = y.RotatedBy(soil.rotation);
            y *= scale;
            Vector2 center = soil.position;

            Vector2 topLeft = center - x - y;
            Vector2 topRight = center + x - y;
            Vector2 bottomLeft = center - x + y;
            Vector2 bottomRight = center + x + y;

            Vector2 origin = Vector2.One * 8;
            Asset<Texture2D> texture = TextureAssets.Tile[soil.tileType];
            SpritebatchDrawer topLeftDrawer = SpritebatchDrawer.FromTextureAsset(texture, topLeft);
            topLeftDrawer.sourceRect = new Rectangle(0, 54, 16, 16);
            topLeftDrawer.rotation = soil.rotation;
            topLeftDrawer.drawOrigin = origin;
            topLeftDrawer.scale = scale;
            sb.Draw(topLeftDrawer);

            SpritebatchDrawer topRightDrawer = SpritebatchDrawer.FromTextureAsset(texture, topRight);
            topRightDrawer.sourceRect = new Rectangle(18, 54, 16, 16);
            topRightDrawer.rotation = soil.rotation;
            topRightDrawer.drawOrigin = origin;
            topRightDrawer.scale = scale;
            sb.Draw(topRightDrawer);

            SpritebatchDrawer bottomLeftDrawer = SpritebatchDrawer.FromTextureAsset(texture, bottomLeft);
            bottomLeftDrawer.sourceRect = new Rectangle(0, 72, 16, 16);
            bottomLeftDrawer.rotation = soil.rotation;
            bottomLeftDrawer.drawOrigin = origin;
            bottomLeftDrawer.scale = scale;
            sb.Draw(bottomLeftDrawer);

            SpritebatchDrawer bottomRightDrawer = SpritebatchDrawer.FromTextureAsset(texture, bottomRight);
            bottomRightDrawer.sourceRect = new Rectangle(18, 72, 16, 16);
            bottomRightDrawer.rotation = soil.rotation;
            bottomRightDrawer.drawOrigin = origin;
            bottomRightDrawer.scale = scale;
            sb.Draw(bottomRightDrawer);
        }
    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        PixelationManager.QueueSpritebatchDrawAction(DrawSoil);
    }
}
