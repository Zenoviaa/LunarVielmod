using Microsoft.Xna.Framework;
using Stellamod.NPCs.Illuria;
using Stellamod.NPCs.RoyalCapital;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers
{
    internal class IllurianGuardSpawnSystem : ModSystem
    {
        public static Point WorshipingTower1TileOffset => new Point(31, -54);
        public static Point WorshipingTower2TileOffset => new Point(24, -58);
        public static Point WorshipingTower3TileOffset => new Point(34, -44);

        public static Dictionary<Point, string> PlacedStructures;
        public static Dictionary<Point, bool> HasActiveGuard;
        public override void ClearWorld()
        {
            PlacedStructures = new();
            HasActiveGuard = new();
        }

        public override void PostUpdateWorld()
        {
            base.PostUpdateWorld();

            //NOT UNTIL POST PLANT
            if (!NPC.downedPlantBoss)
                return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;

                float maxTilesAway = 100;
                float maxDistanceAway = maxTilesAway.TilesToDistance();
                foreach(var kvp in PlacedStructures)
                {
                    Point tilePlacedOn = kvp.Key;
                    Vector2 tileWorld = tilePlacedOn.ToWorldCoordinates() + GetTileOffset(kvp.Value).ToWorldCoordinates();
                    float distanceToPlayer = Vector2.Distance(player.Center, tileWorld);
                    if (!HasActiveGuard.ContainsKey(tilePlacedOn))
                    {
                        HasActiveGuard.Add(tilePlacedOn, false);
                    }

                    if(distanceToPlayer <= maxDistanceAway && !HasActiveGuard[tilePlacedOn])
                    {
                        int npcIndex = NPC.NewNPC(player.GetSource_FromThis(), (int)tileWorld.X, (int)tileWorld.Y,
                            ModContent.NPCType<IllurianMage>());
                        NetMessage.SendData(MessageID.SyncNPC);
                        HasActiveGuard[tilePlacedOn] = true;
                    }
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["Tiles"] = PlacedStructures.Keys.ToList();
            tag["Structures"] = PlacedStructures.Values.ToList();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var names = tag.Get<List<Point>>("Tiles");
            var values = tag.Get<List<string>>("Structures");
            PlacedStructures = names.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
        }

        private Point GetTileOffset(string structure)
        {
            if(structure == "Struct/Jungle/WorshipingTower1")
            {
                return WorshipingTower1TileOffset;
            } 
            else if (structure == "Struct/Jungle/WorshipingTower2")
            {
                return WorshipingTower2TileOffset;
            } 
            else if (structure == "Struct/Jungle/WorshipingTower3")
            {
                return WorshipingTower3TileOffset;
            }

            return Point.Zero;
        }

        public static void Add(Point tile, string structure)
        {
            if (PlacedStructures.ContainsKey(tile))
                return;
            PlacedStructures.Add(tile, structure);
 
        }
    }
}
