using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Stellamod.NPCs.RoyalCapital;
using Stellamod.NPCs.Bosses.Fenix;
using System.IO;
using Stellamod.NPCs.Bosses.Sylia;
using Terraria.ID;

namespace Stellamod.NPCs.Town
{
    internal class AlcadSpawnSystem : ModSystem
    {
        public static Point AlcadTile;
        public static Point UnderworldRuinsTile;
        public static Point MerenaSpawnTileOffset => new Point(174, -119);
        public static Point LonelySorceressTileOffset => new Point(189, -129);
        public static Point UnderworldRiftTileOffset => new Point(70, -21);

        public static Vector2 AlcadWorld => AlcadTile.ToWorldCoordinates();
        public static Vector2 MerenaSpawnWorld => AlcadTile.ToWorldCoordinates() + MerenaSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 LonelySorceressSpawnWorld => AlcadTile.ToWorldCoordinates() + LonelySorceressTileOffset.ToWorldCoordinates();
        public static Vector2 UnderworldRiftSpawnWorld => UnderworldRuinsTile.ToWorldCoordinates() + UnderworldRiftTileOffset.ToWorldCoordinates();
      
        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(AlcadTile.ToVector2());
            writer.WriteVector2(UnderworldRuinsTile.ToVector2());
        }

        public override void NetReceive(BinaryReader reader)
        {
            AlcadTile = reader.ReadVector2().ToPoint();
            UnderworldRuinsTile = reader.ReadVector2().ToPoint();
        }

        public override void PostUpdateWorld()
        {
            base.PostUpdateWorld();
            for(int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                float distanceToUnderworldRuins = Vector2.Distance(player.Center, UnderworldRiftSpawnWorld);
                if (player.active && player.GetModPlayer<MyPlayer>().ZoneAlcadzia)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<Merena>()))
                    {
                        NPC.NewNPC(player.GetSource_FromThis(), 
                            (int)MerenaSpawnWorld.X, (int)MerenaSpawnWorld.Y, 
                            ModContent.NPCType<Merena>());
                        NetMessage.SendData(MessageID.SyncNPC);
                    }

                    if (!NPC.AnyNPCs(ModContent.NPCType<LonelySorceress>()) &&
                        !NPC.AnyNPCs(ModContent.NPCType<Fenix>()))
                    {
                        NPC.NewNPC(player.GetSource_FromThis(), 
                            (int)LonelySorceressSpawnWorld.X, (int)LonelySorceressSpawnWorld.Y, 
                            ModContent.NPCType<LonelySorceress>());
                        NetMessage.SendData(MessageID.SyncNPC);
                    }
                } 
                else if (player.active && distanceToUnderworldRuins < 600 && !NPC.AnyNPCs(ModContent.NPCType<UnderworldRift>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(), 
                        (int)UnderworldRiftSpawnWorld.X, (int)UnderworldRiftSpawnWorld.Y, 
                        ModContent.NPCType<UnderworldRift>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["AlcadTile"] = AlcadTile;
            tag["UnderworldRuinsTile"] = UnderworldRuinsTile;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            AlcadTile = tag.Get<Point>("AlcadTile");
            UnderworldRuinsTile = tag.Get<Point>("UnderworldRuinsTile");
        }
    }
}
