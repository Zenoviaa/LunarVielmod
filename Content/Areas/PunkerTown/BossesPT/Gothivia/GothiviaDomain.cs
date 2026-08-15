using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

[Autoload(Side = ModSide.Client)]
public class GothiviaDomain : ModSystem
{
    private float _darkenAlpha;
    private RenderTargetProvider _domainSwapRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private RenderTargetProvider _domainRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public bool drawGothivia;
    public bool darken;

    public override void OnModLoad()
    {
        On_Main.DrawNPCs += DrawBlack;

    }
    public override void Load()
    {

        base.Load();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += DrawClouds;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
  
    }
    private bool ShouldRender() => drawGothivia;

    private void DrawWind(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var shader = ShaderContent.GetInstance<FlameWindsShader>();
        shader.Time = Main.GlobalTimeWrappedHourly * 10;
        spriteBatch.Restart(effect: shader);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>(
            "Stellamod/Assets/NoiseTextures/BlurryPerlinNoise"), Vector2.Zero);
        drawer.dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        drawer.color = Color.White * 0.4f;
        drawer.drawOrigin = Vector2.Zero;
        spriteBatch.Draw(drawer);
        spriteBatch.RestartDefaults();
    }

    private void DrawClouds()
    {
        if (!ShouldRender())
            return;
        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (config.FocusMode)
            return;
        _darkenAlpha += darken ? 0.05f : -0.05f;
        _darkenAlpha = MathHelper.Clamp(_darkenAlpha, 0, 0.25f);
        PixelationManager.QueueSpritebatchDrawAction(DrawWind, DrawLayer.OverPlayers);
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_domainRT);
        graphicsDevice.Clear(Color.Lerp(Color.Red, Color.Black, 0.9f));

        FireVortexShader fireShader = ShaderContent.GetInstance<FireVortexShader>();
        fireShader.Time = Main.GlobalTimeWrappedHourly * 0.1f;
        fireShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        fireShader.GradientTopColor = new Color(224, 187, 122);
        fireShader.GradientBottomColor = new Color(59, 19, 13);
        fireShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, fireShader.Effect);

        Rectangle targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        spriteBatch.Draw(AssetManager.Noise.FlameVortexNoise, targetRect, Color.Lerp(Color.White, Color.Black, 0.3f));

        spriteBatch.End();


        //Draw the smokee
        FireVortexSmokeShader smokeShader = ShaderContent.GetInstance<FireVortexSmokeShader>();
        smokeShader.GradientTopColor = new Color(125, 125, 125);
        smokeShader.GradientBottomColor = new Color(22, 22, 22);
        smokeShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        smokeShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        smokeShader.Time = 1.5f + Main.GlobalTimeWrappedHourly * 0.1f;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, smokeShader.Effect);
        targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

        Color c = Color.Lerp(Color.White, Color.Black, 0.5f);
        spriteBatch.Draw(AssetManager.Noise.FlameVortexNoise, targetRect, c);

        spriteBatch.End();


        /*

        WildfireShader wildfireShader = ShaderContent.GetInstance<WildfireShader>();
        wildfireShader.GradientTopColor = new Color(125, 125, 125);
        wildfireShader.GradientBottomColor = new Color(22, 22, 22);
        wildfireShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        wildfireShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        wildfireShader.Time = 1.5f + Main.GlobalTimeWrappedHourly * 0.1f;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, wildfireShader.Effect);
        targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

   
        spriteBatch.Draw(AssetManager.Noise.FlameVortexNoise, targetRect, Color.White);

        spriteBatch.End();

        */

        graphicsDevice.SetRenderTarget(_domainSwapRT);
        graphicsDevice.Clear(Color.Lerp(Color.Red, Color.Black, 0.9f));


        PalettizerShader palettizerShader = PalettizerShader.Instance;
        palettizerShader.PaletteTexture = PaletteHelper.GetColorSpectrum("Hell.pal");
        palettizerShader.Progress = 1f;
        palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
        palettizerShader.ImageSize = new Vector2(131, 312) * 4f;
        palettizerShader.DitherAlpha = 0.125f;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, palettizerShader.Effect);
        spriteBatch.Draw(_domainRT, Vector2.Zero, Color.White);
        spriteBatch.End();


        graphicsDevice.SetRenderTarget(_domainRT);
        graphicsDevice.Clear(Color.Lerp(Color.Red, Color.Black, 0.9f));

        spriteBatch.Begin();
        spriteBatch.Draw(_domainSwapRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
        spriteBatch.End();
    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= DrawClouds;
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        On_Main.DrawNPCs -= DrawBlack;
    }

    private void DrawBlack(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        if (ShouldRender())
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.Clear(Color.Transparent);
            Color drawColor2 = Color.Lerp(Color.White, Color.Black, 0f);
            drawColor2 *= 1f;
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.FocusMode)
            {
                spriteBatch.Draw(_domainRT, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), drawColor2);
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (0.5f + _darkenAlpha));

                SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar.Value, Vector2.Zero);
                Rectangle rect = new Rectangle(-Main.screenWidth/2, 384, Main.screenWidth * 2, Main.screenHeight);
                drawer.dstRect = rect;
                drawer.drawOrigin = Vector2.Zero;
                drawer.color = Color.Yellow;
                drawer.color.A = 0;
                spriteBatch.Draw(drawer);
            }


            //  spriteBatch.Draw(TextureAssets.BlackTile.Value, targetRect, Color.White);
            DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (singularityFallSystem.hoveringPlatform)
            {
                Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, singularityFallSystem.hoverPlatformY);
                SpritebatchDrawer blackDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Vector2.Zero);
                blackDrawer.dstRect = new Rectangle(0, (int)(drawPosition.Y - Main.screenPosition.Y) + 48, Main.screenWidth, Main.screenHeight);
                blackDrawer.drawOrigin = Vector2.Zero;
                blackDrawer.color = Color.White * 0.15f;
                var bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine");

                //drawPosition -= Main.screenPosition;
                drawPosition.Y += 48;
                SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bloomLine, drawPosition);
                drawer.rotation += MathHelper.PiOver2;
                drawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 12);
                drawer.color.A = 0;
                drawer.scale.Y *= 8;
                spriteBatch.Draw(drawer);
            }


            drawGothivia = false;
            darken = false;
        }

        orig(self, behindTiles);
    }
}
