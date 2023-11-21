using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Backgrounds;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Skies;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
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
        public override void Load()
        {

            Ref<Effect> BasicTrailRef = new(Assets.Request<Effect>("Effects/Primitives/BasicTrailShader", AssetRequestMode.ImmediateLoad).Value);
            Ref<Effect> LightningTrailRef = new(Assets.Request<Effect>("Effects/Primitives/LightningTrailShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["VampKnives:BasicTrail"] = new MiscShaderData(BasicTrailRef, "TrailPass");
            GameShaders.Misc["VampKnives:LightningTrail"] = new MiscShaderData(LightningTrailRef, "TrailPass");



            Ref<Effect> genericLaserShader = new(Assets.Request<Effect>("Effects/Primitives/GenericLaserShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["VampKnives:GenericLaserShader"] = new MiscShaderData(genericLaserShader, "TrailPass");


            Ref<Effect> LightBeamVertexShader = new(Assets.Request<Effect>("Effects/Primitives/LightBeamVertexShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["VampKnives:LightBeamVertexShader"] = new MiscShaderData(LightBeamVertexShader, "TrailPass");


            Ref<Effect> ArtemisLaserShader = new(Assets.Request<Effect>("Effects/Primitives/ArtemisLaserShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["VampKnives:ArtemisLaserShader"] = new MiscShaderData(ArtemisLaserShader, "TrailPass");


            Ref<Effect> shadowflameShader = new(Assets.Request<Effect>("Effects/Primitives/Shadowflame", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["VampKnives:Fire"] = new MiscShaderData(shadowflameShader, "TrailPass");

         //   Ref<Effect> NaxtrinShader = new(Assets.Request<Effect>("Effects/Primitives/NaxtrinSky", AssetRequestMode.ImmediateLoad).Value);
         //   Filters.Scene["Stellamod:NaxtrinSky"] = new Filter(new ScreenShaderData(NaxtrinShader, "ShaderPass"), EffectPriority.VeryHigh);
          //  Filters.Scene["Stellamod:NaxtrinSky"].Load();
            SkyManager.Instance["Stellamod:NaxtrinSky"] = new NaxtrinSky();
            SkyManager.Instance["Stellamod:NaxtrinSky"].Load();

            SkyManager.Instance["Stellamod:NaxtrinSky2"] = new NaxtrinSky2();
            SkyManager.Instance["Stellamod:NaxtrinSky2"].Load();

            SkyManager.Instance["Stellamod:AlcadSky"] = new NaxtrinSky3();
            SkyManager.Instance["Stellamod:AlcadSky"].Load();
         


            // ...other Load stuff goes here
            MedalCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<Medal>(), 999L, "Ruin medals"));
            Filters.Scene["Stellamod:Daedussss"] = new Filter(new DaedusScreenShaderData("FilterMiniTower").UseColor(-0.3f, -0.3f, -0.3f).UseOpacity(0.375f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Aurelus"] = new Filter(new AbyssScreenShaderData("FilterMiniTower").UseColor(0.2f, 0.0f, 1f).UseOpacity(0.375f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Verlia"] = new Filter(new VerliaScreenShaderData("FilterMiniTower").UseColor(0.3f, 0.0f, 1f).UseOpacity(0.375f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Acid"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(0f, 1f, 0.3f).UseOpacity(0.275f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Starbloom"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(1f, 0.3f, 0.8f).UseOpacity(0.375f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Govheil"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(1f, 0.7f, 0f).UseOpacity(0.275f), EffectPriority.Medium);
            Filters.Scene["Stellamod:AuroreanStars"] = new Filter(new AuroreanStarsScreenShaderData("FilterMiniTower").UseColor(1.3f, 0.2f, 0.2f).UseOpacity(0.275f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Gintzing"] = new Filter(new GintzeScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.6f).UseOpacity(0.275f), EffectPriority.Medium);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("Stellamod/Effects/Shockwave", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();



                Filters.Scene["Stellamod:Starbloom"] = new Filter(new StellaScreenShader("FilterMiniTower").UseColor(0.1f, 0, 0.3f).UseOpacity(0.9f), EffectPriority.VeryHigh);
                Filters.Scene["Stellamod:Starbloom"] = new Filter(new StellaScreenShader("FilterMiniTower").UseColor(0.5f, 0.2f, 0.7f).UseOpacity(0.65f), EffectPriority.VeryHigh);
                SkyManager.Instance["Stellamod:Starbloom"] = new StarbloomSky();



            }

            Ref<Effect> GenericLaserShader = new(Assets.Request<Effect>("Effects/LaserShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["Stellamod:LaserShader"] = new MiscShaderData(GenericLaserShader, "TrailPass");




            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


            //``````````````````````````````````````````````````````````````````````````````````````

            Filters.Scene["Stellamod:GreenMoonSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.1f, 0.2f, 0.5f).UseOpacity(0.53f), EffectPriority.High);
            SkyManager.Instance["Stellamod:GreenMoonSky"] = new GreenMoonSky();

            SkyManager.Instance["Stellamod:GovheilSky"] = new GovheilSky();
            Filters.Scene["Stellamod:GovheilSky"] = new Filter((new ScreenShaderData("FilterMiniTower")).UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);

            //`````````````````````````````````````````````````````````````````````````````





            if (!Main.dedServ)
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
                TextureAssets.Tile[TileID.GrayBrick] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/PearlstoneRE");

                Main.instance.LoadTiles(TileID.GraniteBlock);
                TextureAssets.Tile[TileID.GrayBrick] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/GraniteRE");

                Main.instance.LoadTiles(TileID.MarbleBlock);
                TextureAssets.Tile[TileID.GrayBrick] = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/MarbRE");
            }
            Instance = this;



        }

        public override void Unload()
        {



            StellaMultiplayer.Unload();

            if (!Main.dedServ)
            {
                /*  Main.tileFrame[TileID.Dirt] = 0;
                  Main.tilesLoaded(TileID.Dirt) = false; 
                  Main.wallFrame[WallID.Dirt] = 0;
                  Main.wallLoaded[WallID.Dirt] = false;
                  Main.tileFrame[TileID.Stone] = 0;
                  Main.tileSetsLoaded[TileID.Stone] = false;
                  Main.tileFrame[TileID.Grass] = 0;
                  Main.tileSetsLoaded[TileID.Grass] = false;
                  Main.tileFrame[TileID.ClayBlock] = 0;
                  Main.instance.LoadTiles(TileID.ClayBlock) = false;
                  Main.tileFrame[TileID.Sand] = 0;
                  Main.tileSetsLoaded[TileID.Sand] = false;
                  Main.tileFrame[TileID.Sandstone] = 0;
                  Main.tileSetsLoaded[TileID.Sandstone] = false;
                  Main.tileFrame[TileID.HardenedSand] = 0;
                  Main.tileSetsLoaded[TileID.HardenedSand] = false;
                  Main.tileFrame[TileID.Mud] = 0;
                  Main.tileSetsLoaded[TileID.Mud] = false;
                  Main.tileFrame[TileID.CrimsonGrass] = 0;
                  Main.tileSetsLoaded[TileID.CrimsonGrass] = false;
                  Main.tileFrame[TileID.CorruptGrass] = 0;
                  Main.tileSetsLoaded[TileID.CorruptGrass] = false;
                  Main.tileFrame[TileID.Crimstone] = 0;
                  Main.tileSetsLoaded[TileID.Crimstone] = false;
                  Main.tileFrame[TileID.JungleGrass] = 0;
                  Main.tileSetsLoaded[TileID.JungleGrass] = false;
                  Main.tileFrame[TileID.SnowBlock] = 0;
                  Main.tileSetsLoaded[TileID.SnowBlock] = false;
                  Main.tileFrame[TileID.IceBlock] = 0;
                  Main.tileSetsLoaded[TileID.IceBlock] = false;
                  Main.tileFrame[TileID.WoodBlock] = 0;
                  Main.tileSetsLoaded[TileID.WoodBlock] = false;
                  Main.tileFrame[TileID.GrayBrick] = 0;
                 Main.tileSetsLoaded[TileID.GrayBrick] = false;
                */
            }
        }

        internal class NPCs
        {
        }
    }

   
    public class Stellamenu : ModMenu
    {


        private const string menuAssetPath = "Stellamod/Assets/Textures/Menu"; // Creates a constant variable representing the texture path, so we don't have to write it out multiple times

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{menuAssetPath}/Logo2");

        //  public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/TheSun");

        //   public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/TheMoon");


        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Menutheme");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<StarbloomBackgroundStyle>();
       
        public override string DisplayName => "The Lunar Veil ModMenu";

        public override void OnSelected()
        {
            SoundEngine.PlaySound(SoundID.Tink); // Plays a thunder sound when this ModMenu is selected

        }
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            drawColor = Main.DiscoColor * 2f ; // Changes the draw color of the logo
            return true;



        }

    }
}
