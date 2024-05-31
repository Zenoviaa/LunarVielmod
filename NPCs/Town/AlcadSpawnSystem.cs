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
using Stellamod.NPCs.Bosses.Niivi;
using Stellamod.NPCs.Bosses.IrradiaNHavoc;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DreadMire.Monolith;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Irradia;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS;
using Stellamod.WorldG;
using Stellamod.NPCs.Bosses.GothiviaTheSun.REK;
using Stellamod.NPCs.Bosses.Ereshkigal;

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
        public static Point IrrTile;
        public static Point IshPinTile;
        public static Point EreshTile;
        public static Point PULSETile;
        public static Point LiberatTile;
        public static Point JhoviaTile;
        public static Point DaedenTile;
        public static Point OrdinTile;
        public static Point GothTile;

        public static Point DreadMonolithTile1;
        public static Point DreadMonolithTile2;
        public static Point DreadMonolithTile3;

        public static Point MerenaSpawnTileOffset => new Point(174, -119);
        public static Point LonelySorceressTileOffset => new Point(189, -129);
        public static Point UnderworldRiftTileOffset => new Point(70, -21);
        public static Point ZuiSpawnTileOffset => new Point(15, -15);
        public static Point CellConverterSpawnTileOffset => new Point(83, -8);
        public static Point DelgrimSpawnTileOffset => new Point(39, -7);
        public static Point LabSpawnTileOffset => new Point(39, -20);
        public static Point GiaSpawnTileOffset => new Point(14, -7);
        public static Point NiiviSpawnTileOffset => new Point(134, -224);
        public static Point BORDOCSpawnTileOffset => new Point(94, -380);

        public static Point SirestiasSpawnTileOffset => new Point(24, -21);
        public static Point VelSpawnTileOffset => new Point(18, -23);

        public static Point IrrSpawnTileOffset => new Point(120, -22);

        public static Point GothSpawnTileOffset => new Point(120, -345);
        public static Point IshPinSpawnTileOffset => new Point(199, -286);
        public static Point EreshSpawnTileOffset => new Point(90, -31);
        public static Point PULSESpawnTileOffset => new Point(64, -23);

        public static Point LiberatSpawnTileOffset => new Point(176, -316);

        public static Point JhoviaSpawnTileOffset => new Point(156, -316);

        public static Point DaedenSpawnTileOffset => new Point(50, -20);

        public static Point OrdinSpawnTileOffset => new Point(176, -25);

        //Dread Monoliths
        public static Vector2 DreadMonolithWorld1 => DreadMonolithTile1.ToWorldCoordinates();
        public static Vector2 DreadMonolithWorld2 => DreadMonolithTile2.ToWorldCoordinates();
        public static Vector2 DreadMonolithWorld3 => DreadMonolithTile3.ToWorldCoordinates();

        public static Point DreadMonolithTileOffset => new Point(5, -9);



        public static Vector2 DreadMonolithSpawnWorld1 => DreadMonolithTile1.ToWorldCoordinates() + DreadMonolithTileOffset.ToWorldCoordinates();
        public static Vector2 DreadMonolithSpawnWorld2 => DreadMonolithTile2.ToWorldCoordinates() + DreadMonolithTileOffset.ToWorldCoordinates();
        public static Vector2 DreadMonolithSpawnWorld3 => DreadMonolithTile3.ToWorldCoordinates() + DreadMonolithTileOffset.ToWorldCoordinates();

        public static Vector2 AlcadWorld => AlcadTile.ToWorldCoordinates();
        public static Vector2 MerenaSpawnWorld => AlcadTile.ToWorldCoordinates() + MerenaSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 LonelySorceressSpawnWorld => AlcadTile.ToWorldCoordinates() + LonelySorceressTileOffset.ToWorldCoordinates();
        public static Vector2 UnderworldRiftSpawnWorld => UnderworldRuinsTile.ToWorldCoordinates() + UnderworldRiftTileOffset.ToWorldCoordinates();
        public static Vector2 LittleWitchSpawnWorld => LittleWitchTownTile.ToWorldCoordinates() + ZuiSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 DelgrimSpawnWorld => MechanicsTownTile.ToWorldCoordinates() + DelgrimSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 CellConverterSpawnWorld => MechanicsTownTile.ToWorldCoordinates() + CellConverterSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 LabSpawnWorld => LabTile.ToWorldCoordinates() + LabSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 GiaSpawnWorld => GiaTile.ToWorldCoordinates() + GiaSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 NiiviSpawnWorld => IlluriaTile.ToWorldCoordinates() + NiiviSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 BORDOCSpawnWorld => FableTile.ToWorldCoordinates() + BORDOCSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 SireSpawnWorld => SireTile.ToWorldCoordinates() + SirestiasSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 VelSpawnWorld => VelTile.ToWorldCoordinates() + VelSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 IrrSpawnWorld => IrrTile.ToWorldCoordinates() + IrrSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 GothSpawnWorld => GothTile.ToWorldCoordinates() + GothSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 IshPinSpawnWorld => IshPinTile.ToWorldCoordinates() + IshPinSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 EreshSpawnWorld => EreshTile.ToWorldCoordinates() + EreshSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 PULSESpawnWorld => PULSETile.ToWorldCoordinates() + PULSESpawnTileOffset.ToWorldCoordinates();

        public static Vector2 LiberatSpawnWorld => LiberatTile.ToWorldCoordinates() + LiberatSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 JhoviaSpawnWorld => JhoviaTile.ToWorldCoordinates() + JhoviaSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 DaedenSpawnWorld => DaedenTile.ToWorldCoordinates() + DaedenSpawnTileOffset.ToWorldCoordinates();

        public static Vector2 OrdinSpawnWorld => OrdinTile.ToWorldCoordinates() + OrdinSpawnTileOffset.ToWorldCoordinates();

        public static bool TownedGia;
        public static bool TownedMardenth;
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
            tag["TownedMar"] = TownedMardenth;
            tag["IlluriaTile"] = IlluriaTile;
            tag["VelTile"] = VelTile;
            tag["FableTile"] = FableTile;
            tag["SireTile"] = SireTile;
            tag["IrrTile"] = IrrTile;
            tag["GothTile"] = GothTile;
            tag["PULSETile"] = PULSETile;
            tag["EreshTile"] = EreshTile;
            tag["IshPinTile"] = IshPinTile;
            tag["LibTile"] = LiberatTile;
            tag["JTile"] = JhoviaTile;
            tag["DTile"] = DaedenTile;
            tag["OTile"] = OrdinTile;
            tag["DreadMonolithTile1"] = DreadMonolithTile1;
            tag["DreadMonolithTile2"] = DreadMonolithTile2;
            tag["DreadMonolithTile3"] = DreadMonolithTile3;
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
            TownedMardenth = tag.GetBool("TownedMardenth");
            IlluriaTile = tag.Get<Point>("IlluriaTile");
            VelTile = tag.Get<Point>("VelTile");
            FableTile = tag.Get<Point>("FableTile");
            SireTile = tag.Get<Point>("SireTile");
            IrrTile = tag.Get<Point>("IrrTile");
            GothTile = tag.Get<Point>("GothTile");
            IshPinTile = tag.Get<Point>("IshPinTile");
            EreshTile = tag.Get<Point>("EreshTile");
            PULSETile = tag.Get<Point>("PULSETile");
            LiberatTile = tag.Get<Point>("LibTile");
            JhoviaTile = tag.Get<Point>("JTile");
            DaedenTile = tag.Get<Point>("DTile");
            OrdinTile = tag.Get<Point>("OTile");
            DreadMonolithTile1 = tag.Get<Point>("DreadMonolithTile1");
            DreadMonolithTile2 = tag.Get<Point>("DreadMonolithTile2");
            DreadMonolithTile3 = tag.Get<Point>("DreadMonolithTile3");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(AlcadTile.ToVector2());
            writer.WriteVector2(UnderworldRuinsTile.ToVector2());
            writer.WriteVector2(LittleWitchTownTile.ToVector2());
            writer.WriteVector2(MechanicsTownTile.ToVector2());
            writer.WriteVector2(LabTile.ToVector2());
            writer.Write(TownedGia);
            writer.Write(TownedMardenth);
            writer.WriteVector2(GiaTile.ToVector2());
            writer.WriteVector2(IlluriaTile.ToVector2());
            writer.WriteVector2(FableTile.ToVector2());
            writer.WriteVector2(VelTile.ToVector2());
            writer.WriteVector2(SireTile.ToVector2());
            writer.WriteVector2(IrrTile.ToVector2());
            writer.WriteVector2(GothTile.ToVector2());
            writer.WriteVector2(PULSETile.ToVector2());
            writer.WriteVector2(IshPinTile.ToVector2());
            writer.WriteVector2(EreshTile.ToVector2());
            writer.WriteVector2(LiberatTile.ToVector2());
            writer.WriteVector2(DreadMonolithTile1.ToVector2());
            writer.WriteVector2(DreadMonolithTile2.ToVector2());
            writer.WriteVector2(DreadMonolithTile3.ToVector2());
            writer.WriteVector2(JhoviaTile.ToVector2());
            writer.WriteVector2(DaedenTile.ToVector2());
            writer.WriteVector2(OrdinTile.ToVector2());
        }

        public override void NetReceive(BinaryReader reader)
        {
            AlcadTile = reader.ReadVector2().ToPoint();
            UnderworldRuinsTile = reader.ReadVector2().ToPoint();
            LittleWitchTownTile = reader.ReadVector2().ToPoint();
            MechanicsTownTile = reader.ReadVector2().ToPoint();
            LabTile = reader.ReadVector2().ToPoint();
            TownedGia = reader.ReadBoolean();
            TownedMardenth = reader.ReadBoolean();
            GiaTile = reader.ReadVector2().ToPoint();
            IlluriaTile = reader.ReadVector2().ToPoint();
            FableTile = reader.ReadVector2().ToPoint();
            VelTile = reader.ReadVector2().ToPoint();
            SireTile = reader.ReadVector2().ToPoint();
            IrrTile = reader.ReadVector2().ToPoint();
            GothTile = reader.ReadVector2().ToPoint();
            IshPinTile = reader.ReadVector2().ToPoint();
            EreshTile = reader.ReadVector2().ToPoint();
            PULSETile = reader.ReadVector2().ToPoint();
            LiberatTile = reader.ReadVector2().ToPoint();
            JhoviaTile = reader.ReadVector2().ToPoint();
            DaedenTile = reader.ReadVector2().ToPoint();
            OrdinTile = reader.ReadVector2().ToPoint();
            DreadMonolithTile1 = reader.ReadVector2().ToPoint();
            DreadMonolithTile2 = reader.ReadVector2().ToPoint();
            DreadMonolithTile3 = reader.ReadVector2().ToPoint();
        }


        
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (StellaMultiplayer.IsHost)
            {
                if (TargetBossAlive())
                {
                    SpawnDelay = 10;
                }

                SpawnDelay--;
                if (SpawnDelay <= 0)
                {
                    Spawn();
                }
            }
        }

        private bool TargetBossAlive()
        {
            return 
                NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Sylia>()) ||
                NPC.AnyNPCs(ModContent.NPCType<IrradiatedNest>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Fenix>()) ||
                NPC.AnyNPCs(ModContent.NPCType<GothiviaIyx>()) ||
                NPC.AnyNPCs(ModContent.NPCType<RekSnake>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Irradia>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Niivi>())||
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
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<LonelySorceress>()) &&
                    !NPC.AnyNPCs(ModContent.NPCType<Fenix>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)LonelySorceressSpawnWorld.X, (int)LonelySorceressSpawnWorld.Y,
                        ModContent.NPCType<LonelySorceress>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Zui>()) && !NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                            (int)LittleWitchSpawnWorld.X, (int)LittleWitchSpawnWorld.Y,
                            ModContent.NPCType<Zui>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<UnderworldRift>()) && !NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)UnderworldRiftSpawnWorld.X, (int)UnderworldRiftSpawnWorld.Y,
                        ModContent.NPCType<UnderworldRift>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Delgrim>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)DelgrimSpawnWorld.X, (int)DelgrimSpawnWorld.Y,
                        ModContent.NPCType<Delgrim>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<CellConverter>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)CellConverterSpawnWorld.X, (int)CellConverterSpawnWorld.Y,
                        ModContent.NPCType<CellConverter>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<UnknownSignal>()) && Main.hardMode &&
                    !NPC.AnyNPCs(ModContent.NPCType<IrradiatedNest>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)LabSpawnWorld.X, (int)LabSpawnWorld.Y,
                        ModContent.NPCType<UnknownSignal>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Gia>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)GiaSpawnWorld.X, (int)GiaSpawnWorld.Y,
                        ModContent.NPCType<Gia>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<EreshkigalIdle>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)EreshSpawnWorld.X, (int)EreshSpawnWorld.Y,
                        ModContent.NPCType<EreshkigalIdle>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<NiiviRoaming>()) 
                    && !NPC.AnyNPCs(ModContent.NPCType<Niivi>()) 
                    && !DownedBossSystem.downedNiiviBoss)
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)NiiviSpawnWorld.X, (int)NiiviSpawnWorld.Y,
                        ModContent.NPCType<NiiviRoaming>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Veldris>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)VelSpawnWorld.X, (int)VelSpawnWorld.Y,
                        ModContent.NPCType<Veldris>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Bordoc>()) && Main.hardMode)
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)BORDOCSpawnWorld.X, (int)BORDOCSpawnWorld.Y,
                        ModContent.NPCType<Bordoc>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Sirestias>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)SireSpawnWorld.X, (int)SireSpawnWorld.Y,
                        ModContent.NPCType<Sirestias>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<IrradiaIdle>()) && !NPC.AnyNPCs(ModContent.NPCType<Irradia>()) && !NPC.AnyNPCs(ModContent.NPCType<StartIrradia>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)IrrSpawnWorld.X, (int)IrrSpawnWorld.Y,
                        ModContent.NPCType<IrradiaIdle>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<GothiviaIdle>()) && !NPC.AnyNPCs(ModContent.NPCType<GothiviaIyx>()) && !NPC.AnyNPCs(ModContent.NPCType<StartGoth>()) && EventWorld.GreenSun)
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)GothSpawnWorld.X, (int)GothSpawnWorld.Y,
                        ModContent.NPCType<GothiviaIdle>());
                }
                else if (NPC.AnyNPCs(ModContent.NPCType<GothiviaIdle>()) && !NPC.AnyNPCs(ModContent.NPCType<RekSnake>()) && !NPC.AnyNPCs(ModContent.NPCType<RekSnakeIdle>()) && !DownedBossSystem.downedRekBoss)
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)GothSpawnWorld.X, (int)GothSpawnWorld.Y,
                        ModContent.NPCType<RekSnakeIdle>());
                }
                else if (!NPC.AnyNPCs(ModContent.NPCType<Havoc>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)IrrSpawnWorld.X, (int)IrrSpawnWorld.Y,
                        ModContent.NPCType<Havoc>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Ordin>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)OrdinSpawnWorld.X, (int)OrdinSpawnWorld.Y,
                        ModContent.NPCType<Ordin>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Mardenth>()))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)LiberatSpawnWorld.X, (int)LiberatSpawnWorld.Y,
                        ModContent.NPCType<Mardenth>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<PULSARHOLE>()) && DownedBossSystem.downedZuiBoss)
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)PULSESpawnWorld.X, (int)PULSESpawnWorld.Y,
                        ModContent.NPCType<PULSARHOLE>());
                }

                else if (!NPC.AnyNPCs(ModContent.NPCType<Ishtar>()) && DownedBossSystem.downedZuiBoss)
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)IshPinSpawnWorld.X, (int)IshPinSpawnWorld.Y,
                        ModContent.NPCType<Ishtar>());
                }

                else  if (!DownedBossSystem.downedDreadMonolith1 && !IsDreadMonolithAlive(0))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)DreadMonolithSpawnWorld1.X, (int)DreadMonolithSpawnWorld1.Y,
                        ModContent.NPCType<DreadMonolith>(), ai1: 0);
                }

                else if (!DownedBossSystem.downedDreadMonolith2 && !IsDreadMonolithAlive(1))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)DreadMonolithSpawnWorld2.X, (int)DreadMonolithSpawnWorld2.Y,
                        ModContent.NPCType<DreadMonolith>(), ai1: 1);
                }

                else if (!DownedBossSystem.downedDreadMonolith3 && !IsDreadMonolithAlive(2))
                {
                    NPC.NewNPC(player.GetSource_FromThis(),
                        (int)DreadMonolithSpawnWorld3.X, (int)DreadMonolithSpawnWorld3.Y,
                        ModContent.NPCType<DreadMonolith>(), ai1: 2);
                }
            }
        }

        private bool IsDreadMonolithAlive(int number)
        {
            for(int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;
                if (npc.type != ModContent.NPCType<DreadMonolith>())
                    continue;
                if (npc.ai[1] == number)
                    return true;
            }
            return false;
        }

        public override void PostUpdateNPCs()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<Gia>()))
            {
                TownedGia = true;
            }

            if (NPC.AnyNPCs(ModContent.NPCType<Mardenth>()))
            {
                TownedMardenth = true;
            }
        }
    }
}
