using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Backgrounds;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Skies;
using Stellamod.WorldG;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Stellamod
{


    public class Stellamod : Mod
    {
        public const string EMPTY_TEXTURE = "Stellamod/Empty";
        public static Texture2D EmptyTexture
        {
            get;
            private set;
        }
        public int GlobalTimer { get; private set; }

        public Stellamod()
        {
            Instance = this;

        }
      
        public ModPacket GetPacket(MessageType type, int capacity)
        {
            ModPacket packet = GetPacket(capacity + 1);
            packet.Write((byte)type);
            return packet;
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

        public override void HandlePacket(BinaryReader reader, int whoAmI) => StellaMultiplayer.HandlePacket(reader, whoAmI);


        public static Player PlayerExists(int whoAmI)
        {
            return whoAmI > -1 && whoAmI < Main.maxPlayers && Main.player[whoAmI].active && !Main.player[whoAmI].dead && !Main.player[whoAmI].ghost ? Main.player[whoAmI] : null;
        }


       


        public static Stellamod Instance;
        public static int MedalCurrencyID;
 
      
        
        public static int MOKCurrencyID;
        public static int MOPCurrencyID;

        public static int MOBCurrencyID;
        public static int MOACurrencyID;
        public static int MOCCurrencyID;
        public static int MOHCurrencyID;
        public static int MOLCurrencyID;
        public override void Load()
        {
           
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            if (Main.netMode != NetmodeID.Server)
            {
                ShaderRegistry.LoadShaders();
                MedalCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<Medal>(), 999L, "Ruin medals"));


                //Anxuety Dreadmire
                MOACurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Manifestments.ManifestmentOfAnxiety(ModContent.ItemType<ManifestedAnxiety>(), 999L, "Manifestation Of Anxiety"));

                //Bravery Verlia
                MOBCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Manifestments.ManifestmentOfBravery(ModContent.ItemType<ManifestedBravery>(), 999L, "Manifestation Of Bravery"));

                //Committment Irradia
                MOCCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Manifestments.ManifestmentOfCommittment(ModContent.ItemType<ManifestedCommitment>(), 999L, "Manifestation Of Committment"));

                //Humility Azurerin
                MOHCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Manifestments.ManifestmentOfLove(ModContent.ItemType<ManifestedHumility>(), 999L, "Manifestation Of Humility"));

                //Love Gothivia
                MOLCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Manifestments.ManifestmentOfLove(ModContent.ItemType<ManifestedLove>(), 999L, "Manifestation Of Love"));

                //----------------------------------------------- Shaders
                Filters.Scene["Stellamod:Daedussss"] = new Filter(new DaedusScreenShaderData("FilterMiniTower").UseColor(-0.3f, -0.3f, -0.3f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Jellyfish1"] = new Filter(new DaedusScreenShaderData("FilterMiniTower").UseColor(-0.3f, -0.3f, -0.3f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Ishtar"] = new Filter(new DaedusScreenShaderData("FilterMiniTower").UseColor(-0.6f, -0.6f, -0.6f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Jellyfish2"] = new Filter(new DaedusScreenShaderData("FilterMiniTower").UseColor(-0.3f, -0.3f, -0.3f).UseOpacity(0.475f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Mechanics"] = new Filter(new DaedusScreenShaderData("FilterMiniTower").UseColor(-0.3f, -0.3f, -0.3f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Aurelus"] = new Filter(new AbyssScreenShaderData("FilterMiniTower").UseColor(0.2f, 0.0f, 1f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Verlia"] = new Filter(new VerliaScreenShaderData("FilterMiniTower").UseColor(0.3f, 0.0f, 1f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Acid"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(0f, 1f, 0.3f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Lab"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(0f, 1f, 0.3f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Veriplant"] = new Filter(new VeriplantScreenShaderData("FilterMiniTower").UseColor(0f, 1f, 0.3f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Starbloom"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(1f, 0.3f, 0.8f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Govheil"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(1f, 0.7f, 0f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:AuroreanStars"] = new Filter(new AuroreanStarsScreenShaderData("FilterMiniTower").UseColor(1.3f, 0.2f, 0.2f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Gintzing"] = new Filter(new GintzeScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.6f).UseOpacity(0.275f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Caeva"] = new Filter(new CaevaScreenShaderData("FilterMiniTower").UseColor(0.1f, 0.6f, 0.65f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Illuria"] = new Filter(new AuroreanStarsScreenShaderData("FilterMiniTower").UseColor(0.4f, -0.3f, 1.3f).UseOpacity(0.275f), EffectPriority.Medium);

                Filters.Scene["Stellamod:ChaosD"] = new Filter(new ChaosDScreenShaderData("FilterMiniTower").UseColor(0.1f, 0.7f, 0.1f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:ChaosT"] = new Filter(new ChaosTScreenShaderData("FilterMiniTower").UseColor(0.6f, 0.2f, 0.75f).UseOpacity(0.375f), EffectPriority.Medium);
                Filters.Scene["Stellamod:Veil"] = new Filter(new ChaosPScreenShaderData("FilterMiniTower").UseColor(0.7f, 0.1f, 0.2f).UseOpacity(0.275f), EffectPriority.VeryHigh);

                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("Stellamod/Effects/Shockwave", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();

                Filters.Scene["Stellamod:GreenMoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.1f, 0.2f, 0.5f).UseOpacity(0.53f), EffectPriority.High);
                SkyManager.Instance["Stellamod:GreenMoonSky"] = new GreenMoonSky();

                SkyManager.Instance["Stellamod:GovheilSky"] = new GovheilSky();
                Filters.Scene["Stellamod:GovheilSky"] = new Filter((new ScreenShaderData("FilterMiniTower")).UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);


                Filters.Scene["Stellamod:Starbloom"] = new Filter(new StellaScreenShader("FilterMiniTower").UseColor(0.1f, 0, 0.3f).UseOpacity(0.9f), EffectPriority.VeryHigh);
                Filters.Scene["Stellamod:Starbloom"] = new Filter(new StellaScreenShader("FilterMiniTower").UseColor(0.5f, 0.2f, 0.7f).UseOpacity(0.65f), EffectPriority.VeryHigh);
                SkyManager.Instance["Stellamod:Starbloom"] = new StarbloomSky();
                SkyManager.Instance["Stellamod:NiiviSky"] = new NiiviSky();


                Ref<Effect> GenericLaserShader = new(Assets.Request<Effect>("Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value);
                GameShaders.Misc["Stellamod:LaserShader"] = new MiscShaderData(GenericLaserShader, "TrailPass");
            }




            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


            //``````````````````````````````````````````````````````````````````````````````````````


            //`````````````````````````````````````````````````````````````````````````````





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
            
            
            On_UIWorldListItem.DrawSelf += (orig, self, spriteBatch) =>
            {
                orig(self, spriteBatch);
                DrawWorldSelectItemOverlay(self, spriteBatch);
            };


            Instance = this;
        }

        private void UnloadTile(int tileID)
        {
            TextureAssets.Tile[tileID] = ModContent.Request<Texture2D>($"Terraria/Images/Tiles_{tileID}");
        }

        private void UnloadWall(int wallID)
        {
            TextureAssets.Wall[wallID] = ModContent.Request<Texture2D>($"Terraria/Images/Wall_{wallID}");
        }

        public override void Unload()
        {
            StellaMultiplayer.Unload();
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

        private void DrawWorldSelectItemOverlay(UIWorldListItem uiItem, SpriteBatch spriteBatch)
        {
            //    bool data = uiItem.Data.TryGetHeaderData(ModContent.GetInstance<WorldLoadGen>(), out var _data);
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
    #region UnopenedWorldIcon


    #endregion;

    public class Stellamenu : ModMenu
    {


        private const string menuAssetPath = "Stellamod/Assets/Textures/Menu"; // Creates a constant variable representing the texture path, so we don't have to write it out multiple times

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{menuAssetPath}/Logo");

        //  public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/TheSun");

        //   public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/TheMoon");


        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Menutheme");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<StarbloomBackgroundStyle>();
       
        public override string DisplayName => "Lunar Veil Legacy";
        public override void OnSelected()
        {
            SoundEngine.PlaySound(SoundID.Tink); // Plays a thunder sound when this ModMenu is selected

        }
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
           // drawColor = Main.DiscoColor * 2f ; // Changes the draw color of the logo
            return true;



        }

    }
}

