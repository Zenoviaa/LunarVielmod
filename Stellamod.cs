using Stellamod.WorldG;
using System;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;
using ReLogic.Content;
using Stellamod.Items.Materials;
using Stellamod.Helpers;
using Terraria.GameContent.UI;
using Stellamod.Skies;
using Terraria.Audio;
using System.Numerics;
using Microsoft.Xna.Framework;
using Stellamod.Backgrounds;
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
            // ...other Load stuff goes here
            MedalCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<Medal>(), 999L, "Ruin medals"));
            Filters.Scene["Stellamod:Daedussss"] = new Filter(new DaedusScreenShaderData("FilterMiniTower").UseColor(-0.3f, -0.3f, -0.3f).UseOpacity(0.375f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Aurelus"] = new Filter(new AbyssScreenShaderData("FilterMiniTower").UseColor(0.2f, 0.0f, 1f).UseOpacity(0.375f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Verlia"] = new Filter(new VerliaScreenShaderData("FilterMiniTower").UseColor(0.3f, 0.0f, 1f).UseOpacity(0.375f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Acid"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(0f, 1f, 0.3f).UseOpacity(0.275f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Govheil"] = new Filter(new AcidScreenShaderData("FilterMiniTower").UseColor(1f, 0.7f, 0f).UseOpacity(0.275f), EffectPriority.Medium);
            Filters.Scene["Stellamod:Gintzing"] = new Filter(new GintzeScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.4f, 0.6f).UseOpacity(0.275f), EffectPriority.Medium);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("Stellamod/Effects/Shockwave", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();



                Filters.Scene["Stellamod:Raveyard"] = new Filter(new StellaScreenShader("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Stellamod:Raveyard"] = new StarbloomSky();
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
                Main.tileTexture[TileID.Dirt] = GetTexture("Textures/DirtRE");

                Main.instance.LoadTiles(TileID.IceBlock);
                Main.tileTexture[TileID.IceBlock] = GetTexture("Textures/IceRE");

                Main.instance.LoadTiles(TileID.SnowBlock);
                Main.tileTexture[TileID.SnowBlock] = GetTexture("Textures/SnowRE");

                Main.instance.LoadWall(WallID.Dirt);
                Main.wallTexture[WallID.Dirt] = GetTexture("Textures/DirtWallRE");

                Main.instance.LoadTiles(TileID.Stone);
                Main.tileTexture[TileID.Stone] = GetTexture("Textures/StoneRE");

                Main.instance.LoadTiles(TileID.Grass);
                Main.tileTexture[TileID.Grass] = GetTexture("Textures/GrassRE");

                Main.instance.LoadTiles(TileID.ClayBlock);
                Main.tileTexture[TileID.ClayBlock] = GetTexture("Textures/ClayRE");

                Main.instance.LoadTiles(TileID.Sand);
                Main.tileTexture[TileID.Sand] = GetTexture("Textures/SandRE");

                Main.instance.LoadTiles(TileID.HardenedSand);
                Main.tileTexture[TileID.HardenedSand] = GetTexture("Textures/HardSandRE");

                Main.instance.LoadTiles(TileID.Sandstone);
                Main.tileTexture[TileID.Sandstone] = GetTexture("Textures/StoneSandRE");

                Main.instance.LoadTiles(TileID.Mud);
                Main.tileTexture[TileID.Mud] = GetTexture("Textures/MudRE");

                Main.instance.LoadTiles(TileID.FleshGrass);
                Main.tileTexture[TileID.FleshGrass] = GetTexture("Textures/CrimGrassRE");

                Main.instance.LoadTiles(TileID.JungleGrass);
                Main.tileTexture[TileID.JungleGrass] = GetTexture("Textures/MudGrassRE");

                Main.instance.LoadTiles(TileID.CorruptGrass);
                Main.tileTexture[TileID.CorruptGrass] = GetTexture("Textures/CrorpGrassRE");

                Main.instance.LoadTiles(TileID.Crimstone);
                Main.tileTexture[TileID.Crimstone] = GetTexture("Textures/CrimStoneRE");

                Main.instance.LoadTiles(TileID.WoodBlock);
                Main.tileTexture[TileID.WoodBlock] = GetTexture("Textures/WoodRE");

                Main.instance.LoadTiles(TileID.GrayBrick);
                Main.tileTexture[TileID.GrayBrick] = GetTexture("Textures/StoneBrickRE");

            }
            Instance = this;



        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                Main.tileFrame[TileID.Dirt] = 0;
                Main.tileSetsLoaded[TileID.Dirt] = false;
                Main.wallFrame[WallID.Dirt] = 0;
                Main.wallLoaded[WallID.Dirt] = false;
                Main.tileFrame[TileID.Stone] = 0;
                Main.tileSetsLoaded[TileID.Stone] = false;
                Main.tileFrame[TileID.Grass] = 0;
                Main.tileSetsLoaded[TileID.Grass] = false;
                Main.tileFrame[TileID.ClayBlock] = 0;
                Main.tileSetsLoaded[TileID.ClayBlock] = false;
                Main.tileFrame[TileID.Sand] = 0;
                Main.tileSetsLoaded[TileID.Sand] = false;
                Main.tileFrame[TileID.Sandstone] = 0;
                Main.tileSetsLoaded[TileID.Sandstone] = false;
                Main.tileFrame[TileID.HardenedSand] = 0;
                Main.tileSetsLoaded[TileID.HardenedSand] = false;
                Main.tileFrame[TileID.Mud] = 0;
                Main.tileSetsLoaded[TileID.Mud] = false;
                Main.tileFrame[TileID.FleshGrass] = 0;
                Main.tileSetsLoaded[TileID.FleshGrass] = false;
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
            }
        }
    }


    public class Stellamenu : ModMenu
    {


        private const string menuAssetPath = "Stellamod/Assets/Textures/Menu"; // Creates a constant variable representing the texture path, so we don't have to write it out multiple times

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{menuAssetPath}/Logo2");

        //  public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/TheSun");

        //   public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/TheMoon");


        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Menutheme");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<MarrowSurfaceBackgroundStyle>();

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
