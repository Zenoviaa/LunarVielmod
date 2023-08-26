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






        }

        public override void Unload()
        {
            // The Unload() methods can be used for unloading/disposing/clearing special objects, unsubscribing from events, or for undoing some of your mod's actions.
            // Be sure to always write unloading code when there is a chance of some of your mod's objects being kept present inside the vanilla assembly.
            // The most common reason for that to happen comes from using events, NOT counting On.* and IL.* code-injection namespaces.
            // If you subscribe to an event - be sure to eventually unsubscribe from it.

            // NOTE: When writing unload code - be sure use 'defensive programming'. Or, in other words, you should always assume that everything in the mod you're unloading might've not even been initialized yet.
            // NOTE: There is rarely a need to null-out values of static fields, since TML aims to completely dispose mod assemblies in-between mod reloads.
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
