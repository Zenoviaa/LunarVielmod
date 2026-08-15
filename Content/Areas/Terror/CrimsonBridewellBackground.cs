using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Effects.Aegislav;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror;

public class CrimsonBridewellBackground : CustomBG
{
    private Vector2 _parallax;
    private Vector2 _lastCameraPos;
    private Vector2 _movementDiff;
    private void Parallax()
    {
        Vector2 parallaxAmt = new Vector2(0.5f, 0.5f);
        Vector2 refPosition = Main.Camera.UnscaledPosition;
        Vector2 diff = _lastCameraPos - refPosition;
        _parallax += diff * parallaxAmt;
        _movementDiff = diff * parallaxAmt;
        _lastCameraPos = refPosition;
    }
    private Vector2 GetScreenOffset(float scale)
    {
        //Apply an offset so the texture doesn't move when you're moving
        //This will wrap inside the shader
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }
    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        Parallax();
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            effect: null);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.GetInstance<AegisCloudsRenderer>().BackgroundTexture, Main.screenPosition);
        drawer.drawOrigin = Vector2.Zero;
        drawer.color = Color.White * Alpha;
        spriteBatch.Draw(drawer);
        spriteBatch.End();


        var starsTexture = TextureRegistry.StarNoise2;
        var noiseTexture = TextureRegistry.BlurryPerlinNoise2;
        MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

        eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
        eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
        eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        eff.Shader.Parameters["screenOffset"].SetValue(GetScreenOffset(scale: 1));
        eff.UseImage2(noiseTexture);
        eff.Shader.Parameters["parallax"].SetValue(-_parallax * 0.00005f);
        eff.Shader.Parameters["gradientFade"].SetValue(0f);
        eff.UseOpacity(1f);
        eff.Apply();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, eff.Shader);
        spriteBatch.Draw(starsTexture.Value,
           new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
            null, Color.White * 0.3f);
        spriteBatch.End();
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneCrimsonBridewell;
    }
}
