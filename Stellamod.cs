global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Content.Sources;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Assets.ContentReader.Pal;
using Stellamod.Assets.Videos;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas;
using Stellamod.Content.Areas.Terror;
using Stellamod.Content.Areas.TheFalling;
using Stellamod.Content.Areas.WorldsEnd;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Currencies;
using Stellamod.Content.Scrolls;
using Stellamod.Core.Bases;
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
using Terraria.ModLoader;
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
                Filters.Scene["Stellamod:HeatedDepths"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.3f, 0f, 0f).UseOpacity(0.35f), EffectPriority.Medium);

                Asset<Effect> screenRef = ModContent.Request<Effect>("Stellamod/Effects/Shockwave"); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();

                SkyManager.Instance["Stellamod:Starbloom"] = new StarbloomSky();
                SkyManager.Instance["Stellamod:NiiviSky"] = new NiiviSky();

                SkyManager.Instance["Stellamod:WorldsEndSky"] = new WorldsEndSky();
                SkyManager.Instance["Stellamod:WorldsEndSky"].Load();


                SkyManager.Instance["Stellamod:AegislavSky"] = new AegislavSky();
                SkyManager.Instance["Stellamod:AegislavSky"].Load();


                SkyManager.Instance["Stellamod:EdgeofTheMoonSky"] = new EdgeofTheMoonSky();
                SkyManager.Instance["Stellamod:EdgeofTheMoonSky"].Load();

                Asset<Effect> GenericLaserShader = Assets.Request<Effect>("Effects/LaserShader");
                GameShaders.Misc["Stellamod:LaserShader"] = new MiscShaderData(GenericLaserShader, "TrailPass");
            }


            if (!Main.dedServ && Main.netMode != NetmodeID.Server && ModContent.GetInstance<LunarVeilClientConfig>().VanillaTexturesToggle == true)
            {
                TextureAssets.Tile[TileID.Dirt] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/DirtRE");
                TextureAssets.Tile[TileID.IceBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/IceRE");
                TextureAssets.Tile[TileID.SnowBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/SnowRE");
                TextureAssets.Wall[WallID.Dirt] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/DirtWallRE");
                TextureAssets.Tile[TileID.Stone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/StoneRE");
                TextureAssets.Tile[TileID.Grass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/GrassRE");
                TextureAssets.Tile[TileID.ClayBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/ClayRE");
                TextureAssets.Tile[TileID.Sand] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/SandRE");
                TextureAssets.Tile[TileID.HardenedSand] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/HardSandRE");
                TextureAssets.Tile[TileID.Sandstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/StoneSandRE");
                TextureAssets.Tile[TileID.Mud] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MudRE");
                TextureAssets.Tile[TileID.CrimsonGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrimGrassRE");
                TextureAssets.Tile[TileID.JungleGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MudGrassRE");
                TextureAssets.Tile[TileID.CorruptGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrorpGrassRE");
                TextureAssets.Tile[TileID.Crimstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrimStoneRE");
                TextureAssets.Tile[TileID.WoodBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/WoodRE");
                TextureAssets.Tile[TileID.GrayBrick] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/StoneBrickRE");
                TextureAssets.Tile[TileID.Pearlstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/PearlstoneRE");
             //   TextureAssets.Tile[TileID.GraniteBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/GraniteRE");
                TextureAssets.Tile[TileID.Granite] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/GraniteRE");
            //    TextureAssets.Tile[TileID.MarbleBlock] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MarbRE");
                TextureAssets.Tile[TileID.Marble] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MarbRE");
                TextureAssets.Tile[TileID.MushroomGrass] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MushGrassRE");
                TextureAssets.Tile[TileID.Ebonstone] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CrorpStoneRE");
                TextureAssets.Tile[TileID.Ash] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/AshingRE");
                TextureAssets.Tile[TileID.ObsidianBrick] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/AshedRE");
                TextureAssets.Tile[TileID.Cloud] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/CloudRE");
                TextureAssets.Tile[TileID.Pearlsand] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/PearlSandRE");
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
            On_UICharacterSelect.OnInitialize += ChangeColor;
            // On_Main.UpdateMenu += EditCharacterSelectColor;
            On_UIPanel.DrawPanel += SetPanelColors;
        }


        private void ChangeColor(On_UICharacterSelect.orig_OnInitialize orig, UICharacterSelect self)
        {
            orig(self);
            var containerPanel = (UIPanel)typeof(UICharacterSelect).GetField("_containerPanel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
            containerPanel.BackgroundColor = Color.Black * 0.8f;
        }

        private void SetPanelColors(On_UIPanel.orig_DrawPanel orig, UIPanel self, SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            var borderTexture = (Asset<Texture2D>)typeof(UIPanel).GetField("_borderTexture", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
            if (ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive && Main.gameMenu)
            {
                if (texture == borderTexture.Value)
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
        }

        private void CreateDefaultPaletteValue()
        {
            Texture3D tex3d = new Texture3D(Main.graphics.GraphicsDevice, 1, 1, 1, false, SurfaceFormat.Color);
            Asset<Palette>.DefaultValue = new Palette(new Vector3[1], tex3d);
        }
        public override IContentSource CreateDefaultContentSource()
        {
            if (!Main.dedServ)
            {
                AddContent(new VideoReader());
                AddContent(new AseFileReader());
                AddContent(new PalFileReader());
                Main.QueueMainThreadAction(CreateDefaultPaletteValue);
   
            }

            return base.CreateDefaultContentSource();
        }
        //   override co
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
            Vector2 logoDrawPos = new Vector2(215, 150f);
            float scale = logoScale;
            scale *= 0.26f;
            drawColor = Color.White;
            drawColor.A = 0;
            spriteBatch.Draw(logo, logoDrawPos, new Rectangle(0, 0, logo.Width, logo.Height), drawColor, logoRotation, new Vector2(logo.Width * 0.5f, logo.Height * 0.5f), scale, SpriteEffects.None, 0f);
            return false;

        }
    }
}

