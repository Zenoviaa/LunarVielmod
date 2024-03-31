using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Stellamod.NPCs.RoyalCapital;
using Stellamod.NPCs.Bosses.Fenix;
using System.IO;
using Stellamod.NPCs.Bosses.Sylia;
using Terraria.ID;
using Stellamod.NPCs.Bosses.Zui;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.NPCs.Bosses.Azzuria;

namespace Stellamod.NPCs.Town
{
    internal class AlcadSpawnSystem : ModSystem
    {
        public static int SpawnDelay = 10;
        public static Point AlcadTile;
        public static Point UnderworldRuinsTile;
        public static Point LittleWitchTownTile;
        public static Point MechanicsTownTile;
        public static Point LabTile;
        public static Point GiaTile;
        public static Point VelTile;
        public static Point IlluriaTile;
        public static Point FableTile;
        public static Point SireTile;

        public static Point MerenaSpawnTileOffset => new Point(174, -119);
        public static Point LonelySorceressTileOffset => new Point(189, -129);
        public static Point UnderworldRiftTileOffset => new Point(70, -21);
        public static Point ZuiSpawnTileOffset => new Point(15, -15);
        public static Point CellConverterSpawnTileOffset => new Point(83, -8);
        public static Point DelgrimSpawnTileOffset => new Point(39, -7);
        public static Point LabSpawnTileOffset => new Point(39, -20);
        public static Point GiaSpawnTileOffset => new Point(14, -7);
        public static Point AzzuriaSpawnTileOffset => new Point(134, -224);
        public static Point BORDOCSpawnTileOffset => new Point(94, -380);

        public static Point SirestiasSpawnTileOffset => new Point(24, -21);
        public static Point VelSpawnTileOffset => new Point(18, -23);
        public static Vector2 AlcadWorld => AlcadTile.ToWorldCoordinates();
        public static Vector2 MerenaSpawnWorld => AlcadTile.ToWorldCoordinates() + MerenaSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 LonelySorceressSpawnWorld => AlcadTile.ToWorldCoordinates() + LonelySorceressTileOffset.ToWorldCoordinates();
        public static Vector2 UnderworldRiftSpawnWorld => UnderworldRuinsTile.ToWorldCoordinates() + UnderworldRiftTileOffset.ToWorldCoordinates();
        public static Vector2 LittleWitchSpawnWorld => LittleWitchTownTile.ToWorldCoordinates() + ZuiSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 DelgrimSpawnWorld => MechanicsTownTile.ToWorldCoordinates() + DelgrimSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 CellConverterSpawnWorld => MechanicsTownTile.ToWorldCoordinates() + CellConverterSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 LabSpawnWorld => LabTile.ToWorldCoordinates() + LabSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 GiaSpawnWorld => GiaTile.ToWorldCoordinates() + GiaSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 AzzuriaSpawnWorld => IlluriaTile.ToWorldCoordinates() + AzzuriaSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 BORDOCSpawnWorld => FableTile.ToWorldCoordinates() + BORDOCSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 SireSpawnWorld => SireTile.ToWorldCoordinates() + SirestiasSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 VelSpawnWorld => VelTile.ToWorldCoordinates() + VelSpawnTileOffset.ToWorldCoordinates();

        public static bool TownedGia;
        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["AlcadTile"] = AlcadTile;
            tag["UnderworldRuinsTile"] = UnderworldRuinsTile;
            tag["LittleWitchTownTile"] = LittleWitchTownTile;
            tag["MechanicsTownTile"] = MechanicsTownTile;
            tag["LabTile"] = LabTile;
            tag["GiaTile"] = GiaTile;
            tag["TownedGia"] = TownedGia;
            tag["IlluriaTile"] = IlluriaTile;
            tag["VelTile"] = VelTile;
            tag["FableTile"] = FableTile;
            tag["SireTile"] = SireTile;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            AlcadTile = tag.Get<Point>("AlcadTile");
            UnderworldRuinsTile = tag.Get<Point>("UnderworldRuinsTile");
            LittleWitchTownTile = tag.Get<Point>("LittleWitchTownTile");
            MechanicsTownTile = tag.Get<Point>("MechanicsTownTile");
            LabTile = tag.Get<Point>("LabTile");
            GiaTile = tag.Get<Point>("GiaTile");
            TownedGia = tag.GetBool("TownedGia");
            IlluriaTile = tag.Get<Point>("IlluriaTile");
            VelTile = tag.Get<Point>("VelTile");
            FableTile = tag.Get<Point>("FableTile");
            SireTile = tag.Get<Point>("SireTile");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(AlcadTile.ToVector2());
            writer.WriteVector2(UnderworldRuinsTile.ToVector2());
            writer.WriteVector2(LittleWitchTownTile.ToVector2());
            writer.WriteVector2(MechanicsTownTile.ToVector2());
            writer.WriteVector2(LabTile.ToVector2());
            writer.Write(TownedGia);
            writer.WriteVector2(GiaTile.ToVector2());
            writer.WriteVector2(IlluriaTile.ToVector2());
            writer.WriteVector2(FableTile.ToVector2());
            writer.WriteVector2(VelTile.ToVector2());
            writer.WriteVector2(SireTile.ToVector2());
        }

        public override void NetReceive(BinaryReader reader)
        {
            AlcadTile = reader.ReadVector2().ToPoint();
            UnderworldRuinsTile = reader.ReadVector2().ToPoint();
            LittleWitchTownTile = reader.ReadVector2().ToPoint();
            MechanicsTownTile = reader.ReadVector2().ToPoint();
            LabTile = reader.ReadVector2().ToPoint();
            TownedGia = reader.ReadBoolean();
            GiaTile = reader.ReadVector2().ToPoint();
            IlluriaTile = reader.ReadVector2().ToPoint();
            FableTile = reader.ReadVector2().ToPoint();
            VelTile = reader.ReadVector2().ToPoint();
            SireTile = reader.ReadVector2().ToPoint();
        }



        public override void PostUpdateWorld()
        {
            base.PostUpdateWorld();
            if (TargetBossAlive())
            {
                SpawnDelay = 10;
            }

            SpawnDelay--;
            if(SpawnDelay <= 0)
            {
                Spawn();
            }
        }

        private bool TargetBossAlive()
        {
            return 
                NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Sylia>()) ||
                NPC.AnyNPCs(ModContent.NPCType<IrradiatedNest>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Fenix>()) ||
                NPC.AnyNPCs(NPCID.WallofFlesh);

        }

        private void Spawn()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;

                if (!NPC.AnyNPCs(ModContent.NPCType<Merena>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)MerenaSpawnWorld.X, (int)MerenaSpawnWorld.Y,
                        ModContent.NPCType<Merena>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<LonelySorceress>()) &&
                    !NPC.AnyNPCs(ModContent.NPCType<Fenix>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)LonelySorceressSpawnWorld.X, (int)LonelySorceressSpawnWorld.Y,
                        ModContent.NPCType<LonelySorceress>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Zui>()) && !NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                            (int)LittleWitchSpawnWorld.X, (int)LittleWitchSpawnWorld.Y,
                            ModContent.NPCType<Zui>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<UnderworldRift>()) && !NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)UnderworldRiftSpawnWorld.X, (int)UnderworldRiftSpawnWorld.Y,
                        ModContent.NPCType<UnderworldRift>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Delgrim>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)DelgrimSpawnWorld.X, (int)DelgrimSpawnWorld.Y,
                        ModContent.NPCType<Delgrim>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<CellConverter>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)CellConverterSpawnWorld.X, (int)CellConverterSpawnWorld.Y,
                        ModContent.NPCType<CellConverter>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<UnknownSignal>()) && Main.hardMode &&
                    !NPC.AnyNPCs(ModContent.NPCType<IrradiatedNest>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)LabSpawnWorld.X, (int)LabSpawnWorld.Y,
                        ModContent.NPCType<UnknownSignal>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Gia>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)GiaSpawnWorld.X, (int)GiaSpawnWorld.Y,
                        ModContent.NPCType<Gia>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Azzuria>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)AzzuriaSpawnWorld.X, (int)AzzuriaSpawnWorld.Y,
                        ModContent.NPCType<Azzuria>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Veldris>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)VelSpawnWorld.X, (int)VelSpawnWorld.Y,
                        ModContent.NPCType<Veldris>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Bordoc>()) && Main.hardMode)
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)BORDOCSpawnWorld.X, (int)BORDOCSpawnWorld.Y,
                        ModContent.NPCType<Bordoc>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Sirestias>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)SireSpawnWorld.X, (int)SireSpawnWorld.Y,
                        ModContent.NPCType<Sirestias>());
                    NetMessage.SendData(MessageID.SyncNPC);
                }
            }
        }
        public override void PostUpdateNPCs()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Gia>()))
            {
                TownedGia = true;
            }
        }


    }
}
