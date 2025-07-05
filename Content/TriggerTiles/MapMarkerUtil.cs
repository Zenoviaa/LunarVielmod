using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using Stellamod.Core.Map;

namespace Stellamod.Content.TriggerTiles
{
    internal static class MapMarkerUtil
    {
        public static Point FindMarkerTile(Player player, int wallType)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.WallType == wallType)
                    {
                        Point point = new Point(i, j);
                        return point;
                    }
                }
            }

            Point markerPoint = new Point((int)player.position.X / 16, (int)player.position.Y / 16);
            return markerPoint;
        }

        public static void TeleportToMarker(Player player, int wallType)
        {
            Point markerPoint = FindMarkerTile(player, wallType);
            Vector2 teleportPosition = player.position;
            for (int j = markerPoint.Y - 4; j > 0; j--)
            {
                Tile tile = Main.tile[markerPoint.X, j];
                if (!tile.HasTile)
                {
                    teleportPosition = new Point(markerPoint.X, j).ToWorldCoordinates();
                    break;
                }
            }

            player.GetModPlayer<MapPlayer>().spotToTeleportTo = teleportPosition;
            /*
            player.Teleport(teleportPosition);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
            SoundEngine.PlaySound(SoundID.Item6, player.position);*/
        }
    }
}
