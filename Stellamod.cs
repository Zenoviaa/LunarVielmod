global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Backgrounds;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas;
using Stellamod.Content.Areas.Terror;
using Stellamod.Content.Areas.WorldsEnd;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Currencies;
using Stellamod.Core.UI;
using Stellamod.Helpers;
using Stellamod.Skies;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Stellamod
{
    public class Stellamod : Mod
    {
        public Stellamod()
        {
#if DEBUG
            MusicAutoloadingEnabled = false;
#endif
            //     Music
        }

        // this is alright, and i'll expand it so it can still be used, but really this shouldn't be used
        public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
        {
            packet.Write(msg);

            for (int m = 0; m < param.Length; m++)
            {
                object obj = param[m];
                if (obj is bool) packet.Write((bool)obj);
                else if (obj is byte) packet.Write((byte)obj);
                else if (obj is int) packet.Write((int)obj);
                else if (obj is float) packet.Write((float)obj);
                else if (obj is double) packet.Write((double)obj);
                else if (obj is short) packet.Write((short)obj);
                else if (obj is ushort) packet.Write((ushort)obj);
                else if (obj is sbyte) packet.Write((sbyte)obj);
                else if (obj is uint) packet.Write((uint)obj);
                else if (obj is decimal) packet.Write((decimal)obj);
                else if (obj is long) packet.Write((long)obj);
                else if (obj is string) packet.Write((string)obj);
            }
            return packet;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) => MultiplayerHelper.HandlePacket(reader, whoAmI);

        public static Stellamod Instance => ModContent.GetInstance<Stellamod>();
        public static int MedalCurrencyID;
        public static int EreshstylCurrencyID;
        public static int NoHitCrystalCurrencyID;
        public static int DragonShardCurrencyID;
        public override void PostSetupContent()
        {
            base.PostSetupContent();
        }
        public override void Load()
        {

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //   Instance = this;
            if (Main.netMode != NetmodeID.Server)
            {
                ShaderLoader.LoadShaders(this);
                ShaderRegistry.LoadShaders();
                CrystalShaderRegistry.LoadShaders();
                MedalCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<RuinMedal>(), 999L, "Ruin Medals"));
                EreshstylCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<Ereshstyl>(), 999L, "Ereshstyl"));
                NoHitCrystalCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<NoHitCrystal>(), 999L, "No Hit Crystal"));
                DragonShardCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<DragonShard>(), 999L, "Dragon Shard"));

                //----------------------------------------------- Shaders
                Filters.Scene["Stellamod:Illuria"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, -0.3f, 1.3f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Marsh"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.4f, 0f, 0f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Aegislav"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.6f, 0f, 0f).UseOpacity(0.35f), EffectPriority.Medium);

                Asset<Effect> screenRef = ModContent.Request<Effect>("Stellamod/Effects/Shockwave"); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();

                SkyManager.Instance["Stellamod:Starbloom"] = new StarbloomSky();
                SkyManager.Instance["Stellamod:NiiviSky"] = new NiiviSky();

                SkyManager.Instance["Stellamod:WorldsEndSky"] = new WorldsEndSky();
                SkyManager.Instance["Stellamod:WorldsEndSky"].Load();


                SkyManager.Instance["Stellamod:AegislavSky"] = new AegislavSky();
                SkyManager.Instance["Stellamod:AegislavSky"].Load();

                Asset<Effect> GenericLaserShader = Assets.Request<Effect>("Effects/LaserShader");
                GameShaders.Misc["Stellamod:LaserShader"] = new MiscShaderData(GenericLaserShader, "TrailPass");
            }


            if (!Main.dedServ && Main.netMode != NetmodeID.Server && ModContent.GetInstance<LunarVeilClientConfig>().VanillaTexturesToggle == true)
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

            var config = ModContent.GetInstance<LunarVeilClientConfig>();

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

            On_UIPanel.DrawPanel += SetPanelColors;
            On_UIWorldListItem.DrawSelf += DrawWorldIconHook;

            // Instance = this;
        }

        private void SetPanelColors(On_UIPanel.orig_DrawPanel orig, UIPanel self, SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            if (ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive)
            {
                if (texture == self._borderTexture.Value)
                    color = Color.White;
                else
                {
                    float brightness = MathF.Max(color.R, color.B);
                    brightness = MathF.Max(brightness, color.G);
                    //   Console.WriteLine(color);
                    float alpha = brightness / 255f;
                    Color baseColor = Color.Black * 0.95f;
                    Color litColor = Color.White;
                    color = Color.Lerp(baseColor, litColor, alpha / 6f);
                }

            }

            orig(self, spriteBatch, texture, color);
        }

        private void DrawWorldIconHook(On_UIWorldListItem.orig_DrawSelf orig, UIWorldListItem self, SpriteBatch spriteBatch)
        {
            orig(self, spriteBatch);
            DrawWorldSelectItemOverlay(self, spriteBatch);
        }

        private void UnloadTile(int tileID)
        {
            TextureAssets.Tile[tileID] = ModContent.Request<Texture2D>($"Terraria/Images/Tiles_{tileID}");
        }

        private void UnloadWall(int wallID)
        {
            TextureAssets.Wall[wallID] = ModContent.Request<Texture2D>($"Terraria/Images/Wall_{wallID}");
        }

        private string InventoryBackPath(int tileID)
        {
            if (tileID == 0)
                return $"Terraria/Images/Inventory_Back";
            return $"Terraria/Images/Inventory_Back{tileID}";
        }


        public override void Unload()
        {
            //Instance = null;
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
            On_UIWorldListItem.DrawSelf -= DrawWorldIconHook;
        }


        private void DrawWorldSelectItemOverlay(UIWorldListItem uiItem, SpriteBatch spriteBatch)
        {

            UIElement WorldIcon = (UIElement)typeof(UIWorldListItem).GetField("_worldIcon", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(uiItem);
            WorldFileData Data = (WorldFileData)typeof(AWorldListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(uiItem);
            WorldIcon.RemoveAllChildren();

            UIElement worldIcon = WorldIcon;
            UIImage element = new UIImage(ModContent.Request<Texture2D>("Stellamod/Assets/Textures/Menu/LunarTree"))
            {
                Top = new StyleDimension(-10f, 0f),
                Left = new StyleDimension(-6f, 0f),
                IgnoresMouseInteraction = true
            };
            worldIcon.Append(element);
        }
    }

    public class Stellamenu : ModMenu
    {
        private const string menuAssetPath = "Stellamod/Assets/Textures/Menu";
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{menuAssetPath}/Logo");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/BeforeTheFlames");
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override string DisplayName => "Lunar Veil";
        public override void OnSelected()
        {
            SoundEngine.PlaySound(SoundID.Tink);
        }

     
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Texture2D logo = MenuLoader.CurrentMenu.Logo.Value;
            Vector2 logoDrawPos = new Vector2(275, 100f);
            float scale = logoScale;
            scale *= 0.28f;
            drawColor = Color.White;
            drawColor.A = 0;
            spriteBatch.Draw(logo, logoDrawPos, new Rectangle(0, 0, logo.Width, logo.Height), drawColor, logoRotation, new Vector2(logo.Width * 0.5f, logo.Height * 0.5f), scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

