using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Backgrounds;
using Stellamod.Effects.Aegislav;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Backgrounds;

public partial class AegislavUndergroundBackground : CustomBG
{
    private Asset<Texture2D> _cloudTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _cloudTextureAsset = AssetManager.LoadBackground("AegislavUnderground_Clouds");
    }
    public override void Unload()
    {
        base.Unload();
        _cloudTextureAsset = null;
    }

    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);


        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        var backgroundShader = AegisUndercloudsShader.Instance;
        backgroundShader.Time = Main.GlobalTimeWrappedHourly * 4;
        backgroundShader.CloudDetailTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise2").Value;
        backgroundShader.DistortionStrength = 0.005f;
        backgroundShader.SpriteSize = _cloudTextureAsset.Size();
        backgroundShader.Resolution = drawRect.Size();
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.Additive,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_cloudTextureAsset, Main.screenPosition);
        drawer.drawOrigin = Vector2.Zero;
//        drawer.scale = Vector2.One * 2;
        drawer.color = Color.White;
        drawer.dstRect = drawRect;
        spriteBatch.Draw(drawer);

        spriteBatch.End();
    }
    public override bool IsActive()
    {
        return false;
      //  return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneAegislavSurface;
    }

}
