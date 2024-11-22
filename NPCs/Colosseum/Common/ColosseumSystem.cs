using Microsoft.Xna.Framework;
using Stellamod.NPCs.Bosses.EliteCommander;
using Stellamod.NPCs.Bosses.Gustbeak;
using Stellamod.WorldG.StructureManager;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace Stellamod.NPCs.Colosseum.Common
{
    public class ColosseumSystem : ModSystem
    {
        private Rectangle _colosseumRectangle;
        private bool _active;

        public int colosseumIndex;
        public int waveIndex;
        public int maxWave;
        public int enemyCount;


        public bool completedBronzeColosseum;
        public bool completedSilverColosseum;
        public bool completedGoldColosseum;
        public bool completedTrueColosseum;
        public Point colosseumTile;

        private Vector2 GongSpawnWorld
        {
            get
            {
                NPCPointSpawnSystem npcPointSpawnSystem = ModContent.GetInstance<NPCPointSpawnSystem>();
                Point colosseumOriginTile = npcPointSpawnSystem.GetStructureTile("Struct/Colosseum/TheColosseum");
                Point centerOffset = (_colosseumRectangle.Size() / 2).ToPoint();
                Point colosseumCenterTile = colosseumOriginTile + new Point(centerOffset.X, -centerOffset.Y);
                Vector2 gongSpawnWorld = colosseumCenterTile.ToWorldCoordinates();
                return gongSpawnWorld;
            }
        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            _colosseumRectangle = Structurizer.ReadRectangle("Struct/Colosseum/TheColosseum");
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(colosseumIndex);
            writer.Write(waveIndex);
            writer.Write(maxWave);
            writer.Write(enemyCount);
            writer.Write(completedBronzeColosseum);
            writer.Write(completedSilverColosseum);
            writer.Write(completedGoldColosseum);
            writer.Write(completedTrueColosseum);
            writer.Write(_active);
            writer.WriteVector2(colosseumTile.ToVector2());
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            colosseumIndex = reader.ReadInt32();
            waveIndex = reader.ReadInt32();
            maxWave = reader.ReadInt32();
            enemyCount = reader.ReadInt32();
            completedBronzeColosseum = reader.ReadBoolean();
            completedSilverColosseum = reader.ReadBoolean();
            completedGoldColosseum = reader.ReadBoolean();
            completedTrueColosseum = reader.ReadBoolean();
            _active = reader.ReadBoolean();
            colosseumTile = reader.ReadVector2().ToPoint();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["bronze"] = completedBronzeColosseum;
            tag["silver"] = completedSilverColosseum;
            tag["gold"] = completedGoldColosseum;
            tag["true"] = completedTrueColosseum;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            completedBronzeColosseum = tag.GetBool("bronze");
            completedSilverColosseum = tag.GetBool("silver");
            completedGoldColosseum = tag.GetBool("gold");
            completedTrueColosseum = tag.GetBool("true");
        }

        private bool AllPlayersDead()
        {
            foreach(var player in Main.ActivePlayers)
            {
                if (!player.dead)
                    return false;
            }
            return true;
        }
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (_active)
            {
                if (AllPlayersDead())
                {
                    _active = false;
                    NetMessage.SendData(MessageID.WorldData);
                }
                    
                return;
            }
      
            if (!StellaMultiplayer.IsHost)
                return;


            if (!completedBronzeColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<BronzeGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<BronzeGong>());
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            else if (!completedSilverColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<SilverGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<SilverGong>());
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
            else if (!completedGoldColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<GoldGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<GoldGong>());
                    NetMessage.SendData(MessageID.WorldData);
                }
            }
        }
        public void Spawn(Point tileOffset, int npcType)
        {
            enemyCount++;
            Point spawnPoint = colosseumTile + tileOffset;
            Vector2 spawnWorld = spawnPoint.ToWorldCoordinates();
            NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnWorld.X, (int)spawnWorld.Y,
                ModContent.NPCType<SpawnerNPC>(), ai0: npcType);
        }

        public void SpawnWave(int index)
        {
            if (!StellaMultiplayer.IsHost)
                return;
            switch (colosseumIndex)
            {
                case 0:
                    SpawnBronzeWave(index);
                    break;
                case 1:
                    SpawnSilverWave(index);
                    break;
                case 2:
                    SpawnGoldWave(index);
                    break;
            }
        }

        private void SpawnBronzeWave(int index)
        {
            switch (index)
            {
                case 0:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSolider>());
                    break;
                case 1:
                    Spawn(new Point(-37, 0), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(-27, 0), ModContent.NPCType<GintzeSolider>());
                    break;
                case 2:
                    Spawn(new Point(37, 0), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(27, 0), ModContent.NPCType<Gintzling>());
                    break;
                case 3:
                    Spawn(new Point(-33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-15, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 0), ModContent.NPCType<Gintzling>());
                    break;
                case 4:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(-15, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(15, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    break;
                case 5:
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSolider>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());                   
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    break;
                case 6:
                    Spawn(new Point(27, 0), ModContent.NPCType<EliteCommander>());
                    break;
            }
        }

        private void SpawnSilverWave(int index)
        {
            switch (index)
            {
                case 0:
                    Spawn(new Point(-33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    break;
                case 1:
                    Spawn(new Point(-33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 2:
                    Spawn(new Point(-33, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(-15, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(15, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(33, -10), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 3:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeSpearman>());
                    break;
                case 4:
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(33, -10), ModContent.NPCType<GintzeWindRider>());
                    break;

                case 5:
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(27, -13), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 6:
                    Spawn(new Point(33, -10), ModContent.NPCType<Gustbeak>());
                    break;
            }
        }

        private void SpawnGoldWave(int index)
        {
            switch (index)
            {
                case 0:
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(35, 0), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(35, 0), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 1:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    break;
                case 2:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(27, -13), ModContent.NPCType<GintzeWindRider>());

                    break;
                case 3:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    break;
                case 4:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, -10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, -10), ModContent.NPCType<GintzeTumbleWeed>());
                    break;
                case 5:
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(35, 10), ModContent.NPCType<EliteCommander>());
                    Spawn(new Point(-35, 10), ModContent.NPCType<EliteCommander>());
                    break;
                case 6:
                    break;
            }
        }

        private void SpawnTrueWave(int index)
        {

        }

        private void Spawn()
        {
            SpawnWave(waveIndex);
            if (waveIndex >= maxWave)
            {
                CompleteColosseum();
            }
        }

        public void Progress()
        {
            enemyCount--;
            if (enemyCount < 0)
            {
                waveIndex++;
                Spawn();
            }
            NetMessage.SendData(MessageID.WorldData);
        }

        public bool IsActive()
        {
            return _active;
        }

        public void StartColosseum(int colosseumIndex, Point tile)
        {
            this.colosseumIndex = colosseumIndex;
            this.enemyCount = 0;
            waveIndex = 0;
            maxWave = 7;
            colosseumTile = tile;
            _active = true;
            Progress();
            NetMessage.SendData(MessageID.WorldData);
        }

        public void Reset()
        {
            this.enemyCount = 0;
            this.colosseumIndex = 0;
            maxWave = 7;
            waveIndex = 0;
            _active = false;
            completedBronzeColosseum = false;
            completedSilverColosseum = false;
            completedGoldColosseum = false;
            completedTrueColosseum = false;
        }

        public void CompleteColosseum()
        {
            _active = false;
            switch (colosseumIndex)
            {
                case 0:
                    completedBronzeColosseum = true;
                    break;
                case 1:
                    completedSilverColosseum = true;
                    break;
                case 2:
                    completedGoldColosseum = true;
                    break;
                case 3:
                    completedTrueColosseum = true;
                    break;
            }
            NetMessage.SendData(MessageID.WorldData);
        }
    }
}
