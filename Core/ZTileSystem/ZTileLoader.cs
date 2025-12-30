using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria.ModLoader;

namespace Stellamod.Core.ZTileSystem;

/// <summary>
/// Maps the z tiles to a integer type for fast lookup
/// </summary>
public class ZTileLoader : ModSystem
{
    public ZTile[] Tiles { get; private set; }
    public override void OnModLoad()
    {
        base.OnModLoad();
        Tiles = ModContent.GetContent<ZTile>().ToArray();
        for(ushort i = 0; i < Tiles.Length; i++)
        {
            Tiles[i].type = i;
        }
    }

    public ZTile GetTile(ushort type)
    {
        return Tiles[type];
    }

    public ZTileInstanceData InstanceTileData<T>() where T : ZTile
    {
        ZTile instance = ModContent.GetInstance<T>();
        ZTileInstanceData tileData = new ZTileInstanceData();
        tileData.type = instance.type;
        tileData.rotation = Rotation.Degrees_0;
        tileData.flipX = false;
        tileData.scale = 1f;
        return tileData;
    }
    public ZTileInstanceData InstanceTileData(ZTile instance)
    {
        ZTileInstanceData tileData = new ZTileInstanceData();
        tileData.type = instance.type;
        tileData.rotation = Rotation.Degrees_0;
        tileData.flipX = false;
        tileData.scale = 1f;
        return tileData;
    }
}
