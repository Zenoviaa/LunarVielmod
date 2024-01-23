using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Stellamod.NPCs.RoyalCapital;
using Stellamod.NPCs.Bosses.Fenix;

namespace Stellamod.NPCs.Town
{
    internal class AlcadSpawnSystem : ModSystem
    {
        public static Point AlcadTile;
        public static Point MerenaSpawnTileOffset => new Point(174, -119);
        public static Point LonelySorceressTileOffset = new Point(189, -129);

        public static Vector2 AlcadWorld => AlcadTile.ToWorldCoordinates();
        public static Vector2 MerenaSpawnWorld => AlcadTile.ToWorldCoordinates() + MerenaSpawnTileOffset.ToWorldCoordinates();
        public static Vector2 LonelySorceressSpawnWorld => AlcadTile.ToWorldCoordinates() + LonelySorceressTileOffset.ToWorldCoordinates();

        public override void ClearWorld()
        {
            base.ClearWorld();

        }

        public override void PostUpdateWorld()
        {
            base.PostUpdateWorld();
            if (!NPC.AnyNPCs(ModContent.NPCType<Merena>()) && StellaMultiplayer.IsHost)
            {
                NPC.NewNPC(null, (int)MerenaSpawnWorld.X, (int)MerenaSpawnWorld.Y, ModContent.NPCType<Merena>());
            }

            if (!NPC.AnyNPCs(ModContent.NPCType<LonelySorceress>()) && 
                !NPC.AnyNPCs(ModContent.NPCType<Fenix>()) && StellaMultiplayer.IsHost)
            {
                NPC.NewNPC(null, (int)LonelySorceressSpawnWorld.X, (int)LonelySorceressSpawnWorld.Y, ModContent.NPCType<LonelySorceress>());
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["AlcadTile"] = AlcadTile;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            AlcadTile = tag.Get<Point>("AlcadTile");
        }
    }
}
