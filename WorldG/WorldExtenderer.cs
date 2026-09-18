using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;

namespace Stellamod.WorldG
{
    public class WorldExtenderer : ModSystem
    {

        //We can set the world size to anything, 8400x2400 is large world
        public int NewMaxTilesX => 13000;
        public int NewMaxTilesY => 4800;

        public static int? XSizeOverride;
        public static int? YSizeOverride;
        //Original 8400x 2400y
        public override void Load()
        {

            IL_WorldGen.CreateNewWorld += WorldGen_EditWorldSize;
            IL_WorldFile.LoadWorld += WorldGen_EditWorldSize;
        }
        public override void Unload()
        {
            base.Unload();
            IL_WorldGen.CreateNewWorld -= WorldGen_EditWorldSize;
            IL_WorldFile.LoadWorld -= WorldGen_EditWorldSize;
        }
        private void WorldGen_EditWorldSize(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.EmitDelegate(EditWorldSize);
        }

        private void EditWorldSize()
        {
            Main.maxTilesX = NewMaxTilesX;
            Main.maxTilesY = NewMaxTilesY;
            if (XSizeOverride.HasValue)
            {
                Main.maxTilesX = XSizeOverride.Value;
                XSizeOverride = null;
            }

            if (YSizeOverride.HasValue)
            {
                Main.maxTilesY = YSizeOverride.Value;
                YSizeOverride = null;
            }

            SetWorldSize();
        }

        public override void ClearWorld()
        {
            base.ClearWorld();

            //Server does need to edit minimap drawing code im pretty sure
            if (Main.netMode == NetmodeID.Server)
                return;
            ResizeMapTarget();
        }

        private void SetWorldSize()
        {

            ResizeMapTarget();
        }
        private void ResizeMapTarget()
        {
            if (8400 < Main.maxTilesX || 2400 < Main.maxTilesY)
            {
                int chunkX = (Main.maxTilesX - 1) / Main.sectionWidth + 1;
                int chunkY = (Main.maxTilesY - 1) / Main.sectionHeight + 1;
                int newSizeX = Math.Max(chunkX * Main.sectionWidth, 8400);
                int newSizeY = Math.Max(chunkY * Main.sectionHeight, 2400);

                Main.Map = new WorldMap(newSizeX, newSizeY);

                ConstructorInfo constructorInfo = typeof(Tilemap).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(ushort), typeof(ushort) })!;
                Main.tile = (Tilemap)constructorInfo.Invoke(new object?[] { (ushort)newSizeX, (ushort)newSizeY });
            }

            int newWidth = Main.maxTilesX / Main.textureMaxWidth + 2;
            int newHeight = Main.maxTilesY / Main.textureMaxHeight + 2;
            if (newWidth > Main.mapTargetX || newHeight > Main.mapTargetY)
            {
                Main.mapTargetX = Math.Max(5, newWidth);
                Main.mapTargetY = Math.Max(3, newHeight);
                Main.instance.mapTarget = new RenderTarget2D[Main.mapTargetX, Main.mapTargetY];
                Main.initMap = new bool[Main.mapTargetX, Main.mapTargetY];
                Main.mapWasContentLost = new bool[Main.mapTargetX, Main.mapTargetY];
            }
        }
    }
}
