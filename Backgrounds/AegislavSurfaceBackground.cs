using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Backgrounds;

public class AegislavSurfaceBackground : CustomBG
{
    private Asset<Texture2D> _farTextureAsset;
    private Asset<Texture2D> _midTextureAsset;
    private Asset<Texture2D> _closeTextureAsset;
    private Asset<Texture2D> _undergroundTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _farTextureAsset = AssetManager.LoadBackground("AegislavSurface_Far");
        _midTextureAsset = AssetManager.LoadBackground("AegislavSurface_Mid");
        _closeTextureAsset = AssetManager.LoadBackground("AegislavSurface_Close");
        _undergroundTextureAsset = AssetManager.LoadBackground("AegislavSurface_UndergroundLoop");
    }
    public override void Unload()
    {
        base.Unload();
        _farTextureAsset = null;
        _midTextureAsset = null;
        _closeTextureAsset = null;
        _undergroundTextureAsset = null;
    }

    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        LunarBackgroundShader backgroundShader = LunarBackgroundShader.Instance;
        Color fadeToColor = new Color(124, 87, 94);
        fadeToColor *= 0.86f;
        backgroundShader.FadeToColor = fadeToColor;
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        
        //Prepare sampler states
        gDevice.Textures[1] = _midTextureAsset.Value;
        gDevice.SamplerStates[1] = SamplerState.PointClamp;
        
        gDevice.Textures[2] = _farTextureAsset.Value;
        gDevice.SamplerStates[2] = SamplerState.PointClamp;

        gDevice.Textures[3] = _undergroundTextureAsset.Value;
        gDevice.SamplerStates[3] = SamplerState.PointClamp;


        Vector2 closeParallax = new Vector2();
        closeParallax.X = Main.screenPosition.X * LocalParallaxSpeed * 0.0002f;



        int worldSurfaceY = GetParallaxYStartHeight();
        worldSurfaceY -= 3000;
        int diffY = (int)(worldSurfaceY - Main.screenPosition.Y);
        closeParallax.Y = -diffY * 0.0001f ;
        closeParallax.Y += 0.2f;

        Vector2 midParallax = new Vector2();
        midParallax.X = Main.screenPosition.X * LocalParallaxSpeed * 0.0002f * 0.5f;
        midParallax.Y = closeParallax.Y;

        Vector2 farParallax = new Vector2();
        farParallax.X = Main.screenPosition.X * LocalParallaxSpeed * 0.0002f * 0.25f + 0.25f;
        farParallax.Y = closeParallax.Y;

        //Set up parallax
        Vector2[] parallax = new Vector2[3];
        parallax[0] = farParallax;
        parallax[1] = midParallax;
        parallax[2] = closeParallax;
        backgroundShader.Parallax = parallax;

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend, 
            SamplerState.PointClamp, 
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color drawColor = Color.White * Alpha;
        Vector2 drawScale = Vector2.One * 2;
        spriteBatch.Draw(
            _closeTextureAsset.Value,
            Vector2.Zero,
            null,
            drawColor,
            0f,
            default,
            scale: drawScale,
            SpriteEffects.None,
            0f
        );

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            null);
    }
    public override bool IsActive()
    {
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneAegislavSurface;
    }
}
