using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.WorldG.StructureManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.NPCs
{
    public class StructureSerializer : TagSerializer<Structure, TagCompound>
    {
        public override TagCompound Serialize(Structure value) => new TagCompound
        {
            ["name"] = value.name,
            ["tile"] = value.tile,
        };

        public override Structure Deserialize(TagCompound tag)
        {
            Structure structure = new Structure();
            structure.name = tag.GetString("name");
            structure.tile = tag.Get<Point>("tile");
            return structure;
        }
    }

    [Serializable]
    public struct Structure
    {
        public string name;
        public Point tile;
    }

    public struct NPCPointSpawner
    {
        public int npcType;
        public string structureToSpawnIn;
        public Point spawnTileOffset;
        public bool always;
    }

    public abstract class PointSpawnNPC : ModNPC
    {
        public abstract void SetPointSpawnerDefaults(ref NPCPointSpawner spawner);
    }

    public class NPCPointSpawnSystem : ModSystem
    {
        private List<NPCPointSpawner> _npcPointSpawners = new List<NPCPointSpawner>();

        public List<Structure> Structures = new List<Structure>();
        public PointSpawnNPC[] npcs;

        public override void OnModLoad()
        {
            base.OnModLoad();
            npcs = Stellamod.Instance.GetContent<PointSpawnNPC>().ToArray();
            LoadNPCPointSpawners();
            Structurizer.OnStructPlace += AddStructure;
            StructureLoader.OnStructPlace += AddStructure;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            Structurizer.OnStructPlace -= AddStructure;
            StructureLoader.OnStructPlace -= AddStructure;
        }

        public override void ClearWorld()
        {
            base.ClearWorld();
            Structures.Clear();
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
        }

        private void AddStructure(Point bottomLeft, string path)
        {
            AddStructureTile(path, bottomLeft);
        }

        private void LoadNPCPointSpawners()
        {
            _npcPointSpawners.Clear();
            for (int i = 0; i < npcs.Length; i++)
            {
                PointSpawnNPC npc = npcs[i];
                NPCPointSpawner pointSpawner = new NPCPointSpawner();
                pointSpawner.npcType = npc.Type;
                npc.SetPointSpawnerDefaults(ref pointSpawner);

                _npcPointSpawners.Add(pointSpawner);
            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            LoadNPCPointSpawners();
            if (StellaMultiplayer.IsHost)
            {
                if (NPC.AnyDanger(ignorePillarsAndMoonlordCountdown: true))
                    return;

                for (int i = 0; i < _npcPointSpawners.Count; i++)
                {
                    NPCPointSpawner pointSpawner = _npcPointSpawners[i];
                    if (NPC.AnyNPCs(pointSpawner.npcType))
                        continue;

                    Structure structuresToSpawnIn = Structures.Find(x => x.name == pointSpawner.structureToSpawnIn);
                    Point spawnTile = structuresToSpawnIn.tile + pointSpawner.spawnTileOffset;
                    Vector2 spawnWorld = spawnTile.ToWorldCoordinates();
                    Player activePlayer = GetClosestActivePlayer(spawnWorld);
                    if (activePlayer == null)
                        return;
                    float d = Vector2.Distance(activePlayer.position, spawnWorld);
                    if (d <= 960 || pointSpawner.always)
                    {
                        NPC.NewNPC(activePlayer.GetSource_FromThis(), (int)spawnWorld.X, (int)spawnWorld.Y, pointSpawner.npcType);
                    }
                }
            }

        }
        private Player GetClosestActivePlayer(Vector2 position)
        {
            Player closest = null;
            float closestDistance = float.MaxValue;
            foreach (var player in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(position, player.position);
                if (distance < closestDistance)
                {
                    closest = player;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        public void AddStructureTile(string name, Point tile)
        {
            Structure structure = new Structure();
            structure.name = name;
            structure.tile = tile;
            Structures.Add(structure);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["structures"] = Structures;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            Structures = tag.Get<List<Structure>>("structures");
        }
    }
}
