using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace Stellamod.Content.Areas.Collosseum.Event.Common
{
    public class ColosseumSystem : ModSystem
    {
        public float spawnTimer;
        public bool completedBronzeColosseum;
        public bool completedSilverColosseum;
        public bool completedGoldColosseum;
        public bool completedTrueColosseum;
        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(completedBronzeColosseum);
            writer.Write(completedSilverColosseum);
            writer.Write(completedGoldColosseum);
            writer.Write(completedTrueColosseum);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            completedBronzeColosseum = reader.ReadBoolean();
            completedSilverColosseum = reader.ReadBoolean();
            completedGoldColosseum = reader.ReadBoolean();
            completedTrueColosseum = reader.ReadBoolean();
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

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (!MultiplayerHelper.IsHost)
                return;

            if (NPC.AnyNPCs(ModContent.NPCType<ColosseumWaveManager>()))
            {
                spawnTimer = 0;
                return;
            }

            spawnTimer++;
            if (spawnTimer < 120)
            {
                return;
            }

            Vector2 GongSpawnWorld = ColosseumWaveManager.GongSpawnWorld;
            if (!completedBronzeColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<BronzeGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<BronzeGong>());
                }
            }
            else if (!completedSilverColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<SilverGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<SilverGong>());
                }
            }
            else if (!completedGoldColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<GoldGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<GoldGong>());
                }
            }
        }

        public void Reset()
        {
            completedBronzeColosseum = false;
            completedSilverColosseum = false;
            completedGoldColosseum = false;
            completedTrueColosseum = false;
        }
    }
}
