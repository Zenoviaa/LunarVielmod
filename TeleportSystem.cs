
using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Catacombs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod;

internal class TeleportSystem : ModSystem
{
    private static bool _findTeleportTiles;
    private static bool _refreshPortals;
    private static int _refreshPortalsCounter;
    public static Vector2[] FireDungeonAltarWorld;
    public static Vector2[] WaterDungeonAltarWorld;
    public static Vector2[] TrapDungeonAltarWorld;
    public static Vector2 StoneGolemAltarWorld;
    public static Vector2 RalladWorld;

    public static Dictionary<Point, Vector2> FocalPortals;
    public override void ClearWorld()
    {
        FocalPortals = new Dictionary<Point, Vector2>();
        _findTeleportTiles = false;
        _refreshPortals = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["FFocalTiles"] = FocalPortals.Keys.ToList();
        tag["FFocalTargets"] = FocalPortals.Values.ToList();
    }

    public override void LoadWorldData(TagCompound tag)
    {
        var names = tag.Get<List<Point>>("FFocalTiles");
        var values = tag.Get<List<Vector2>>("FFocalTargets");
        FocalPortals = names.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
    }

    public override void NetSend(BinaryWriter writer)
    {
        int count = FireDungeonAltarWorld.Length;
        writer.Write(count);
        for (int i = 0; i < count; i++)
        {
            writer.WriteVector2(FireDungeonAltarWorld[i]);
        }

        count = WaterDungeonAltarWorld.Length;
        writer.Write(count);
        for (int i = 0; i < count; i++)
        {
            writer.WriteVector2(WaterDungeonAltarWorld[i]);
        }

        count = TrapDungeonAltarWorld.Length;
        writer.Write(count);
        for (int i = 0; i < count; i++)
        {
            writer.WriteVector2(TrapDungeonAltarWorld[i]);
        }

        writer.WriteVector2(StoneGolemAltarWorld);
        writer.WriteVector2(RalladWorld);
    }

    public override void NetReceive(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        FireDungeonAltarWorld = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            FireDungeonAltarWorld[i] = reader.ReadVector2();
        }

        count = reader.ReadInt32();
        WaterDungeonAltarWorld = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            WaterDungeonAltarWorld[i] = reader.ReadVector2();
        }

        count = reader.ReadInt32();
        TrapDungeonAltarWorld = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            TrapDungeonAltarWorld[i] = reader.ReadVector2();
        }

        StoneGolemAltarWorld = reader.ReadVector2();
        RalladWorld = reader.ReadVector2();
    }

    public override void PostUpdateWorld()
    {
        if (!_findTeleportTiles)
        {
            FindDungeonAltarTiles();
            FindTile(ModContent.TileType<FlowerSummon>(), ref StoneGolemAltarWorld);
            FindTile(ModContent.TileType<Rallad>(), ref RalladWorld);
            NetMessage.SendData(MessageID.WorldData);
            _findTeleportTiles = true;
        }

        if (!_refreshPortals)
        {
            bool count = false;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    count = true;
                    break;
                }
            }

            if (count)
            {
                _refreshPortalsCounter++;
                if (_refreshPortalsCounter >= 60)
                {
                    RefreshPortals();
                    _refreshPortals = true;
                }
            }

        }
    }


    private int AddAltarToList(int x, int y, List<Vector2> fireList, List<Vector2> waterList, List<Vector2> trapList)
    {
        int left = x - 10;
        int right = x + 10;
        int top = y - 10;
        int bottom = y + 10;

        left = (int)MathHelper.Clamp(left, 0, Main.maxTilesX - 1);
        right = (int)MathHelper.Clamp(right, 0, Main.maxTilesX - 1);
        top = (int)MathHelper.Clamp(top, 0, Main.maxTilesY - 1);
        bottom = (int)MathHelper.Clamp(bottom, 0, Main.maxTilesY - 1);


        int fireType = ModContent.TileType<CatacombStoneFire>();
        int waterType = ModContent.TileType<CatacombStoneWater>();
        int trapType = ModContent.TileType<CatacombStoneTrap>();
        for (int i = left; i < right; i++)
        {
            for (int j = top; j < bottom; j++)
            {
                Tile tile = Main.tile[i, j];
                if (tile.HasTile)
                {
                    if (tile.TileType == fireType)
                    {
                        Vector2 worldPoint = new Vector2(x, y).ToWorldCoordinates();
                        fireList.Add(worldPoint);
                        return fireType;
                    }
                    else if (tile.TileType == waterType)
                    {
                        Vector2 worldPoint = new Vector2(x, y).ToWorldCoordinates();
                        waterList.Add(worldPoint);
                        return waterType;
                    }
                    else if (tile.TileType == trapType)
                    {
                        Vector2 worldPoint = new Vector2(x, y).ToWorldCoordinates();
                        trapList.Add(worldPoint);
                        return trapType;
                    }
                }
            }
        }
        return -1;
    }
    private void FindDungeonAltarTiles()
    {
        int fireType = ModContent.TileType<CatacombStoneFire>();
        int waterType = ModContent.TileType<CatacombStoneWater>();
        int trapType = ModContent.TileType<CatacombStoneTrap>();
        int tileType = ModContent.TileType<CatacombsSummon>();

        List<Vector2> fireDungeonWorldList = new List<Vector2>();
        List<Vector2> waterDungeonWorldList = new List<Vector2>();
        List<Vector2> trapDungeonWorldList = new List<Vector2>();

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (tile.TileType == tileType)
                {
                    AddAltarToList(x, y, fireDungeonWorldList, waterDungeonWorldList, trapDungeonWorldList);
                }
            }
        }

        FireDungeonAltarWorld = fireDungeonWorldList.ToArray();
        WaterDungeonAltarWorld = waterDungeonWorldList.ToArray();
        TrapDungeonAltarWorld = trapDungeonWorldList.ToArray();
    }


    private void FindTile(int tileType, ref Vector2 world)
    {
        world = Vector2.Zero;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                if (Main.tile[x, y].TileType == tileType)
                {
                    Vector2 worldPoint = new Vector2(x, y).ToWorldCoordinates();
                    world = worldPoint;
                    break;
                }
            }
        }
    }


    public static void RefreshPortals()
    {
        //Kill all the portals
        for (int p = 0; p < Main.maxProjectiles; p++)
        {
            Projectile projectile = Main.projectile[p];
            if (!projectile.active)
                continue;

            if (projectile.type != ModContent.ProjectileType<FocalPortal>())
                continue;

            projectile.Kill();
        }

        Point altarTile;
        //Spawn all the portals again
        foreach (var kvp in FocalPortals)
        {
            altarTile = kvp.Key;
            Vector2 portalPosition = new Vector2((altarTile.X + 1) * 16, (altarTile.Y - 6) * 16);
            Vector2 catacombsPortalPosition = new Vector2(kvp.Value.X, kvp.Value.Y) + new Vector2(0, -16 * 160);

            int p = Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), portalPosition, Vector2.Zero,
               ModContent.ProjectileType<FocalPortal>(), 0, 0, Main.myPlayer,
               ai0: catacombsPortalPosition.X,
               ai1: catacombsPortalPosition.Y);

            Vector2 portalPosition2 = new Vector2(kvp.Value.X, kvp.Value.Y);
            int p2 = Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), catacombsPortalPosition, Vector2.Zero,
               ModContent.ProjectileType<FocalPortal>(), 0, 0, Main.myPlayer,
               ai0: portalPosition.X,
               ai1: portalPosition.Y);
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p2);
        }
    }

    public static void CreatePortal(Vector2 catacombsAltar, int altarTileX, int altarTileY)
    {
        //Save it
        Point altarTile = new Point(altarTileX, altarTileY);
        foreach (var kvp in FocalPortals)
        {
            Point otherPortal = kvp.Key;
            float dist = Vector2.Distance(new Vector2(altarTileX, altarTileY), new Vector2(otherPortal.X, otherPortal.Y));
            if (dist <= 6)
            {
                altarTile = otherPortal;
                break;
            }
        }

        if (!FocalPortals.ContainsKey(altarTile))
            FocalPortals.Add(altarTile, catacombsAltar);
        else
            FocalPortals[altarTile] = catacombsAltar;
    }
}
