using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Special.DeadRomancesExcalibur;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders;

[Autoload(Side = ModSide.Client)]
public class GoldenAuroraEffectRenderer : ModSystem
{
    private float _auroraTimer;
    public override void Load()
    {
        base.Load();
        On_Main.Draw += DrawGoldenAuroraHook;
    }

    public override void Unload()
    {
        base.Unload();
        On_Main.Draw -= DrawGoldenAuroraHook;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Main.LocalPlayer.HasBuff<HeavenlyLove>())
            _auroraTimer++;
        else
            _auroraTimer--;
        _auroraTimer = MathHelper.Clamp(_auroraTimer, 0f, 60f);
    }
    private void DrawGoldenAuroraHook(On_Main.orig_Draw orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (Main.gameMenu)
            return;
        if (_auroraTimer <= 0)
            return;

        DrawGoldenAurora();
    }

    private void DrawGoldenAurora()
    {
        float alpha = _auroraTimer / 60f;
        SpriteBatch sb = Main.spriteBatch;
        GoldenAuroraShader shader = GoldenAuroraShader.Instance;
        shader.DistortionTexture = AssetManager.Noise.AuroraRays.Value;
        shader.DistortionAmt = 0.05f;
        shader.Tiling = Vector2.One * 1;
        Asset<Texture2D> textureAsset = AssetManager.Noise.Whirly; ;
        Vector2 drawOrigin = textureAsset.Size() * 0.5f;
        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        Color drawColor = Color.Goldenrod;
        drawColor *= alpha * 0.7f;
        drawColor *= ExtraMath.Osc(0.5f, 1f, speed: 2);
        //shader.Time = 0;
        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, shader.Effect);
        sb.Draw(textureAsset.Value, drawRect, drawColor);
        sb.End();
    }
}
