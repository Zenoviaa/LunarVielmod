using Stellamod.Content.Areas.SpecialTiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ambient;

public abstract class ABreakableSpawningNPCTileEntity<ModNPCType, ModTileType> : ModTileEntity
    where ModNPCType : ModNPC
    where ModTileType : ModTile
{
    int _activeNpc = -1;
    float TestDistance => 1024 * 1024;
    bool _canSpawn=true;
    float TooCloseDistance => 1024 * 1024;
    float TooFarDistance => 2000 * 2000;
    public override void Update()
    {
        base.Update();
        if (_activeNpc != -1)
        {

            var npc = Main.npc[_activeNpc];
            if (!npc.active || npc.type != ModContent.NPCType<ModNPCType>())
                _activeNpc = -1;
            return;
        }

        if (Main.GameUpdateCount % 30 != 0)
            return;
 
        var worldCoordinates = Position.ToWorldCoordinates();
        var nearestPlayer = PlayerHelper.FindClosestPlayer(worldCoordinates, 999999);
        if (nearestPlayer == null)
            return;
        var sqrDistToPlayer = Vector2.DistanceSquared(worldCoordinates, nearestPlayer.Center);
        if (!_canSpawn)
        {
            _canSpawn = sqrDistToPlayer > TooFarDistance;
        }


        if (sqrDistToPlayer <= TooCloseDistance && _canSpawn)
        {
            _canSpawn = false;
            _activeNpc = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)worldCoordinates.X, (int)worldCoordinates.Y, ModContent.NPCType<ModNPCType>());
        }
         
       
    }

    public override void OnNetPlace()
    {
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
        }
    }

    public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
            int width = 4;
            int height = 4;
            NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

            // Sync the placement of the tile entity with other clients
            // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
            NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
            return -1;
        }

        // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
        int placedEntity = Place(i, j);

        return placedEntity;
    }


    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<ModTileType>();
    }
}