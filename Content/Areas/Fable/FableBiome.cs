using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Backgrounds;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas;
using Stellamod.Content.Areas.Ishtar;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable;

public class FableSky : CustomSky
{
    private bool _isActive;
    private float _drawOpacity;
    public override void Update(GameTime gameTime)
    {

        if (_isActive && _drawOpacity < 1f)
        {
            _drawOpacity += 0.01f;
        }
        else if (!_isActive && _drawOpacity > 0f)
        {
            _drawOpacity -= 0.1f;
        }
    }

    public override Color OnTileColor(Color inColor)
    {
        return inColor;
        //  return Color.White * 0.5f * _drawOpacity;
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        //draw the sky itself
        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {
            //Draw Sky visual here
            var pass = AssetReferences.Effects.Fable.FableSky.CreatePixelPass();

            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.noiseSampler = new()
            {
                Texture = AssetReferences.Assets.NoiseTextures.CloudNoise2.Asset.Value,
                Sampler = SamplerState.LinearWrap
            };
            pass.Parameters.ditherSampler = new()
            {
                Texture = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value,
                Sampler = SamplerState.LinearWrap
            };
            pass.Parameters.spriteSize = new Vector2(Main.screenWidth, Main.screenHeight);
            pass.Parameters.ditherTexelSize = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value.GetTexelSize();
            pass.Parameters.parallax = Main.screenPosition * 0.001f;
            pass.Apply();
            using(spriteBatch.Ctx(spriteBatch.Parameters with {  effect = pass.Shader }))
            {
                Rectangle targetRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Main.ColorOfTheSkies * _drawOpacity);
            }
        }
    }

    public override float GetCloudAlpha()
    {
        return (1f - _drawOpacity);
    }

    public override void Activate(Vector2 position, params object[] args)
    {
        _drawOpacity = 0.002f;
        _isActive = true;
    }


    public override void Deactivate(params object[] args)
    {
        _isActive = false;
    }

    public override void Reset()
    {
        _isActive = false;
    }

    public override bool IsActive()
    {
        return (_isActive || _drawOpacity > 0.001f) && !Main.gameMenu;
    }
}

public class FableBiome : BaseUrdveilBiome,
    ISunColor
{
    public static Color[] FableSunColors
    {
        get
        {
            field = new Color[]
            {
                  Color.SkyBlue,

                    new Color(255, 173, 63),
                       new Color(255, 173, 63),
                                new Color(255, 173, 63),
                                         new Color(255, 173, 63),
                                            new Color(255, 173, 63),



                    Color.White,
                   Color.White,
                        Color.White,
                             Color.White,
                    Color.White,
                    Color.White,
                   Color.White,
                        Color.White,
                    Color.White,
                    Color.White,

                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    Color.SkyBlue
            };
            return field;
        }
    }
    // Select all the scenery
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IshtarWaterStyle>(); // Sets a water style for when inside this biome
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>(); //ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/FabledBackgroundStyle");
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    public override int Music
    {
        get
        {
            //Put your if statement here


            //Normal music
            if (Main.dayTime)
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/GetSpookyYaWitch");
            }
            else
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/LightedFable");
            }
        }
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

    // Populate the Bestiary Filter
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InFable;
    public override void SpecialVisuals(Player player, bool isActive)
    {
        string name = "LunarVeil:FableSky";
        if (!SkyManager.Instance[name].IsActive() && isActive)
            SkyManager.Instance.Activate(name, player.Center);
        if (SkyManager.Instance[name].IsActive() && !isActive)
            SkyManager.Instance.Deactivate(name);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.ZoneWitchesFable = true;
        if (Main.netMode == NetmodeID.Server)
            return;
        LunarLightingRenderer.AddSunColor(this);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.ZoneWitchesFable = false;
        if (Main.netMode == NetmodeID.Server)
            return;
        LunarLightingRenderer.RemoveSunColor(this);
    }

    public void ModifySunColor(ref SkySettings skySettings, float sunInterpolant)
    {
        if (!Main.dayTime)
            skySettings.TargetTintSunColor = FableSunColors[0];
        else
            skySettings.TargetTintSunColor = DrawUtilities.InterpolateColorArray(sunInterpolant, FableSunColors);

        var endDayColorNightColor = DrawUtilities.InterpolateColorArray(0.96f, FableSunColors);
        var endColor = new Color(97, 93, 110);
        var endTintSunColor = new Color(135, 206, 235);
        if (!Main.dayTime)
        {
            skySettings.TargetColorOfTheSkies = endColor;
            skySettings.TargetTintSunColor = endTintSunColor;
        }
        else
        {
            skySettings.TargetColorOfTheSkies = Color.Lerp(skySettings.TargetColorOfTheSkies, skySettings.TargetTintSunColor, 0.25f);
        }

    }
}
