using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Main;

namespace Stellamod.Content.Areas.TheFalling;

public class GoldenSpiralCloudsShader : CrystalShader<GoldenSpiralCloudsShader>
{
    public Texture2D ColorationTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public Color GlowColor
    {
        set
        {
            Effect.Parameters["glowColor"].SetValue(value.ToVector4());
        }
    }

    public float Threshold
    {
        set
        {
            Effect.Parameters["threshold"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
    public Vector2 Parallax
    {
        set
        {
            Effect.Parameters["parallax"].SetValue(value);
        }
    }
}
public class EdgeyCloudsShader : CrystalShader<EdgeyCloudsShader>
{
    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public Vector2 Parallax
    {
        set
        {
            Effect.Parameters["parallax"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
    public float DistortionStrength
    {
        set
        {
            Effect.Parameters["distortionStrength"].SetValue(value);
        }
    }
}
public class CrystalSkyMixShader : 
    CrystalShader<CrystalSkyMixShader>
{
    public Texture2D MaskTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }

    public Texture2D CloudTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
        }
    }

    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[3] = value;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.PointClamp;
        }
    }
    public Vector2 Parallax
    {
        set
        {
            Effect.Parameters["parallax"].SetValue(value);
        }
    }

    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float DistortionStrength
    {
        set
        {
            Effect.Parameters["distortionStrength"].SetValue(value);
        }
    }
}

[Autoload(Side = ModSide.Client)]
public class EdgeofTheMoonRenderer : ModSystem
{
    
    public ManagedRenderTarget MaskRT { get; private set; }
    public ManagedRenderTarget CloudsRT { get; private set; }
    public override void Load()
    {
        base.Load(); On_OverlayManager.Draw += DrawBackgrounds;
        On_Main.CheckMonoliths += RenderBackground;
    }

    private void RenderBackground(On_Main.orig_CheckMonoliths orig)
    {

        orig();
        if (Main.gameMenu)
            return;

        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        SpriteBatch spriteBatch = Main.spriteBatch;
        gDevice.SetRenderTarget(MaskRT);
        gDevice.Clear(Color.Transparent);

        SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds6"), Main.screenPosition);
        maskDrawer.color = Color.White;
        maskDrawer.color.A = 0;
      //  maskDrawer.VerticalFrame(1, 4);
        maskDrawer.CenterOrigin();


        EdgeyCloudsShader fadeShader = ShaderContent.GetInstance<EdgeyCloudsShader>();
        fadeShader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        fadeShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        fadeShader.DistortionStrength = 0.015f;
        fadeShader.Parallax = new Vector2(Main.screenPosition.X * 0.00005f, 0f);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, fadeShader.Effect); 

        maskDrawer.scale = Vector2.One * 2;
        maskDrawer.scale.X *= 2;
        maskDrawer.scale.Y *= 0.3f;
        maskDrawer.dstRect = new Rectangle(-215, 0, Main.screenWidth + 450, Main.screenHeight);
        maskDrawer.drawOrigin = Vector2.Zero;
        maskDrawer.worldPosition = Main.screenPosition + new Vector2(1250, 950);
        spriteBatch.Draw(maskDrawer);
        spriteBatch.Draw(maskDrawer);

        spriteBatch.End();


        gDevice.SetRenderTarget(CloudsRT);
        gDevice.Clear(Color.Transparent);


        var mixShader = ShaderContent.GetInstance<CrystalSkyMixShader>();
        mixShader.MaskTexture = MaskRT;
        mixShader.Time = Main.GlobalTimeWrappedHourly * 0.25f;
        mixShader.DistortionStrength = 0.05f;
        mixShader.NoiseTexture = AssetManager.Noise.PainterlyNoise.Value;
        mixShader.CloudTexture = TextureRegistry.CloudNoise2.Value;//AssetManager.Noise.PainterlyNoise.Value;
        mixShader.Parallax = Vector2.Zero;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
            mixShader.Effect);


        SpritebatchDrawer cloudDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.Clouds, Main.screenPosition);
        
        cloudDrawer.dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        
        cloudDrawer.drawOrigin = Vector2.Zero;
        cloudDrawer.color = Color.Lerp(Color.White, Color.Black, 0f) * 0.9f;
        spriteBatch.Draw(cloudDrawer);


        spriteBatch.End();


        gDevice.SetRenderTarget(MaskRT);
        gDevice.Clear(Color.Transparent);
        GoldenSpiralCloudsShader outlineShader = ShaderContent.GetInstance<GoldenSpiralCloudsShader>();
        outlineShader.GlowColor = Color.Orange * 0.4f ;
        outlineShader.Threshold = 0.1f;
        outlineShader.ColorationTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/LavaDepths").Value;
        outlineShader.Time = Main.GlobalTimeWrappedHourly;
        outlineShader.Parallax = Main.screenPosition * 0.005f;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
            outlineShader.Effect);

        spriteBatch.Draw(CloudsRT, Vector2.Zero, Color.White);
        spriteBatch.End();

    }
    private void DrawBackgrounds(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (layer == RenderLayers.Background)
        {
            if(!Main.gameMenu && Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneEdgeoftheMoon)
            {
                spriteBatch.Draw(MaskRT, Vector2.Zero, Color.DarkBlue);
         //       spriteBatch.Draw(MaskRT, Vector2.Zero, Color.Blue);

                spriteBatch.Draw(MaskRT, Vector2.Zero, Color.White);
                //spriteBatch.Draw(MaskRT, Vector2.Zero, Color.White);
            }

        }
        orig(self, spriteBatch, layer, beginSpriteBatch);

    }


    public override void Unload()
    {
        base.Unload();
    }
    public override void OnModLoad()
    {
        base.OnModLoad();
        MaskRT = ManagedRenderTarget.New();
        CloudsRT = ManagedRenderTarget.New();
    }
}


public class EdgeofTheMoonBiome : ModBiome
{
 //   public override ModWaterStyle WaterStyle => nul
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    // Select Music
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music
    {
        get
        {
            return MusicLoader.GetMusicSlot(Mod, "Assets/Music/MoonskapeReflection");
        }
    }


    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player)
    {
        return player.Bottom.ToTileCoordinates().Y < 1500;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<BiomePlayer>().ZoneEdgeoftheMoon = true;
        if (Main.netMode == NetmodeID.Server)
            return;

        SkyManager.Instance.Activate("Stellamod:EdgeofTheMoonSky", player.Center);
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.GetModPlayer<BiomePlayer>().ZoneEdgeoftheMoon = false;
        if (Main.netMode == NetmodeID.Server)
            return;

        SkyManager.Instance.Deactivate("Stellamod:EdgeofTheMoonSky", player.Center);
    }
}

public class EdgeofTheMoonSky : 
    CustomSky
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
        _drawOpacity = MathHelper.Clamp(_drawOpacity, 0f, 1f);
    }

    public override Color OnTileColor(Color inColor)
    {
        Color targetColor = inColor * 0.5f;
        Color inbetweenColor = Color.Lerp(inColor, targetColor, _drawOpacity);
        return inbetweenColor;
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {

            SkyGradientShader skyGradientShader = SkyGradientShader.Instance;
            skyGradientShader.H = 0.15f;
            skyGradientShader.Bend = -0.25f;
            skyGradientShader.StartColor = Color.Black;
            skyGradientShader.MidColor = Color.Lerp(Color.DarkBlue, Color.Black, 0.85f);
            skyGradientShader.EndColor = Color.Lerp(Color.White, Color.Black, 0.99f);
            spriteBatch.Restart(effect: skyGradientShader.Effect);
            Rectangle targetRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Color.White * _drawOpacity);
            spriteBatch.RestartDefaults();
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