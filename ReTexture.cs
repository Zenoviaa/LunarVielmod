using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod
{
    internal static class ReTexture
    {
        public static void Load()
        {

            if (!Main.dedServ && Main.netMode != NetmodeID.Server && ModContent.GetInstance<StellamodClientConfig>().VanillaTexturesToggle == true)
            {
                Main.instance.LoadTiles(TileID.Dirt);
                TextureAssets.Tile[TileID.Dirt] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/DirtRE");

                Main.instance.LoadTiles(TileID.IceBlock);
                TextureAssets.Tile[TileID.IceBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/IceRE");

                Main.instance.LoadTiles(TileID.SnowBlock);
                TextureAssets.Tile[TileID.SnowBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/SnowRE");

                Main.instance.LoadWall(WallID.Dirt);
                TextureAssets.Wall[WallID.Dirt] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/DirtWallRE");

                Main.instance.LoadTiles(TileID.Stone);
                TextureAssets.Tile[TileID.Stone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/StoneRE");

                Main.instance.LoadTiles(TileID.Grass);
                TextureAssets.Tile[TileID.Grass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/GrassRE");

                Main.instance.LoadTiles(TileID.ClayBlock);
                TextureAssets.Tile[TileID.ClayBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/ClayRE");

                Main.instance.LoadTiles(TileID.Sand);
                TextureAssets.Tile[TileID.Sand] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/SandRE");

                Main.instance.LoadTiles(TileID.HardenedSand);
                TextureAssets.Tile[TileID.HardenedSand] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/HardSandRE");

                Main.instance.LoadTiles(TileID.Sandstone);
                TextureAssets.Tile[TileID.Sandstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/StoneSandRE");

                Main.instance.LoadTiles(TileID.Mud);
                TextureAssets.Tile[TileID.Mud] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MudRE");

                Main.instance.LoadTiles(TileID.CrimsonGrass);
                TextureAssets.Tile[TileID.CrimsonGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrimGrassRE");

                Main.instance.LoadTiles(TileID.JungleGrass);
                TextureAssets.Tile[TileID.JungleGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MudGrassRE");

                Main.instance.LoadTiles(TileID.CorruptGrass);
                TextureAssets.Tile[TileID.CorruptGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrorpGrassRE");

                Main.instance.LoadTiles(TileID.Crimstone);
                TextureAssets.Tile[TileID.Crimstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrimStoneRE");

                Main.instance.LoadTiles(TileID.WoodBlock);
                TextureAssets.Tile[TileID.WoodBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/WoodRE");

                Main.instance.LoadTiles(TileID.GrayBrick);
                TextureAssets.Tile[TileID.GrayBrick] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/StoneBrickRE");

                Main.instance.LoadTiles(TileID.Pearlstone);
                TextureAssets.Tile[TileID.Pearlstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/PearlstoneRE");

                Main.instance.LoadTiles(TileID.GraniteBlock);
                TextureAssets.Tile[TileID.GraniteBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/GraniteRE");

                Main.instance.LoadTiles(TileID.Granite);
                TextureAssets.Tile[TileID.Granite] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/GraniteRE");

                Main.instance.LoadTiles(TileID.MarbleBlock);
                TextureAssets.Tile[TileID.MarbleBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MarbRE");

                Main.instance.LoadTiles(TileID.Marble);
                TextureAssets.Tile[TileID.Marble] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MarbRE");

                Main.instance.LoadTiles(TileID.MushroomGrass);
                TextureAssets.Tile[TileID.MushroomGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MushGrassRE");

                Main.instance.LoadTiles(TileID.Ebonstone);
                TextureAssets.Tile[TileID.Ebonstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrorpStoneRE");

                Main.instance.LoadTiles(TileID.Ash);
                TextureAssets.Tile[TileID.Ash] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/AshingRE");

                Main.instance.LoadTiles(TileID.ObsidianBrick);
                TextureAssets.Tile[TileID.ObsidianBrick] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/AshedRE");

                Main.instance.LoadTiles(TileID.Cloud);
                TextureAssets.Tile[TileID.Cloud] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CloudRE");

                Main.instance.LoadTiles(TileID.Pearlsand);
                TextureAssets.Tile[TileID.Pearlsand] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/PearlSandRE");

                Main.instance.LoadTiles(TileID.SnowCloud);
                TextureAssets.Tile[TileID.SnowCloud] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/SnowCloudRE");
            }

            var config = ModContent.GetInstance<StellamodClientConfig>();

            if (!Main.dedServ && Main.netMode != NetmodeID.Server && config.VanillaUIRespritesToggle)
            {
                //Replace UI
                string categoryPanel = "Stellamod/Assets/Textures/UI/CategoryPanel";
                string categoryPanelHot = "Stellamod/Assets/Textures/UI/CategoryPanelHot";

                TextureAssets.InventoryBack = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack2 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack3 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack4 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack5 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack6 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack7 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack8 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack9 = ModContent.Request<Texture2D>(categoryPanel);
                TextureAssets.InventoryBack10 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack11 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack12 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack13 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack14 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack15 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack16 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack17 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack18 = ModContent.Request<Texture2D>(categoryPanelHot);
                TextureAssets.InventoryBack19 = ModContent.Request<Texture2D>(categoryPanelHot);


                TextureAssets.ScrollLeftButton = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/BackButton");
                TextureAssets.ScrollRightButton = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/ForwardButton");
            }

        }

        private static void UnloadTile(int tileID)
        {
            TextureAssets.Tile[tileID] = ModContent.Request<Texture2D>($"Terraria/Images/Tiles_{tileID}");
        }

        private static void UnloadWall(int wallID)
        {
            TextureAssets.Wall[wallID] = ModContent.Request<Texture2D>($"Terraria/Images/Wall_{wallID}");
        }

        private static string InventoryBackPath(int tileID)
        {
            if (tileID == 0)
                return $"Terraria/Images/Inventory_Back";
            return $"Terraria/Images/Inventory_Back{tileID}";
        }

        public static void Unload()
        {
            if (!Main.dedServ)
            {
                string backButton = "Terraria/Images/UI/Bestiary/Button_Back";
                string forwardButton = "Terraria/Images/UI/Bestiary/Button_Forward";

                TextureAssets.InventoryBack = ModContent.Request<Texture2D>(InventoryBackPath(0));
                TextureAssets.InventoryBack2 = ModContent.Request<Texture2D>(InventoryBackPath(2));
                TextureAssets.InventoryBack3 = ModContent.Request<Texture2D>(InventoryBackPath(3));
                TextureAssets.InventoryBack4 = ModContent.Request<Texture2D>(InventoryBackPath(4));
                TextureAssets.InventoryBack5 = ModContent.Request<Texture2D>(InventoryBackPath(5));
                TextureAssets.InventoryBack6 = ModContent.Request<Texture2D>(InventoryBackPath(6));
                TextureAssets.InventoryBack7 = ModContent.Request<Texture2D>(InventoryBackPath(7));
                TextureAssets.InventoryBack8 = ModContent.Request<Texture2D>(InventoryBackPath(8));
                TextureAssets.InventoryBack9 = ModContent.Request<Texture2D>(InventoryBackPath(9));
                TextureAssets.InventoryBack10 = ModContent.Request<Texture2D>(InventoryBackPath(10));
                TextureAssets.InventoryBack11 = ModContent.Request<Texture2D>(InventoryBackPath(11));
                TextureAssets.InventoryBack12 = ModContent.Request<Texture2D>(InventoryBackPath(12));
                TextureAssets.InventoryBack13 = ModContent.Request<Texture2D>(InventoryBackPath(13));
                TextureAssets.InventoryBack14 = ModContent.Request<Texture2D>(InventoryBackPath(14));
                TextureAssets.InventoryBack15 = ModContent.Request<Texture2D>(InventoryBackPath(15));
                TextureAssets.InventoryBack16 = ModContent.Request<Texture2D>(InventoryBackPath(16));
                TextureAssets.InventoryBack17 = ModContent.Request<Texture2D>(InventoryBackPath(17));
                TextureAssets.InventoryBack18 = ModContent.Request<Texture2D>(InventoryBackPath(18));
                TextureAssets.InventoryBack19 = ModContent.Request<Texture2D>(InventoryBackPath(19));
                TextureAssets.ScrollLeftButton = ModContent.Request<Texture2D>(backButton);
                TextureAssets.ScrollRightButton = ModContent.Request<Texture2D>(forwardButton);
            }

            if (!Main.dedServ)
            {
                UnloadTile(TileID.Dirt);
                UnloadTile(TileID.IceBlock);
                UnloadTile(TileID.SnowBlock);
                UnloadWall(WallID.Dirt);
                UnloadTile(TileID.Stone);
                UnloadTile(TileID.Grass);
                UnloadTile(TileID.ClayBlock);
                UnloadTile(TileID.Sand);
                UnloadTile(TileID.HardenedSand);
                UnloadTile(TileID.Sandstone);
                UnloadTile(TileID.Mud);
                UnloadTile(TileID.CrimsonGrass);
                UnloadTile(TileID.JungleGrass);
                UnloadTile(TileID.CorruptGrass);
                UnloadTile(TileID.Crimstone);
                UnloadTile(TileID.WoodBlock);
                UnloadTile(TileID.GrayBrick);
                UnloadTile(TileID.Pearlstone);
                UnloadTile(TileID.GraniteBlock);
                UnloadTile(TileID.Granite);
                UnloadTile(TileID.MarbleBlock);
                UnloadTile(TileID.Marble);
                UnloadTile(TileID.MushroomGrass);
                UnloadTile(TileID.Ebonstone);
                UnloadTile(TileID.Ash);
                UnloadTile(TileID.ObsidianBrick);
                UnloadTile(TileID.Cloud);
                UnloadTile(TileID.Pearlsand);
                UnloadTile(TileID.SnowCloud);
            }
        }
    }
}
