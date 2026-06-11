using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Skies;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

[Autoload(Side = ModSide.Client)]
public class FenixDomain : ModSystem
{
    private ManagedRenderTarget _domainRTSwap;
    private ManagedRenderTarget _domainRT;
    public bool drawFenix;
    public override void OnModLoad()
    {
        _domainRT = ManagedRenderTarget.New();
        _domainRTSwap = ManagedRenderTarget.New();
        On_Main.DrawNPCs += DrawBlack;

    }
    public override void Load()
    {

        base.Load();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += DrawClouds;
    }

    private bool ShouldRenderClouds() => drawFenix;
    private void DrawClouds()
    {
        if (!ShouldRenderClouds())
            return;
        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (config.FocusMode)
        {
            return;
        }
           
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_domainRT);
        graphicsDevice.Clear(Color.Transparent);

        FenixDomainShader fenixDomainShader = ShaderContent.GetInstance<FenixDomainShader>();
        fenixDomainShader.GradientMap = TextureRegistry.CloudNoise3.Value;
        fenixDomainShader.Time = Main.GlobalTimeWrappedHourly;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, fenixDomainShader.Effect);

        Rectangle targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        spriteBatch.Draw(TextureAssets.BlackTile.Value, targetRect, Color.White);

        spriteBatch.End();
        //    spriteBatch.Restart(effect: fenixBackCloudsShader.Effect);

        FenixBackClouds fenixBackCloudsShader = ShaderContent.GetInstance<FenixBackClouds>();
        fenixBackCloudsShader.Time = Main.GlobalTimeWrappedHourly * 0.4f;
        fenixBackCloudsShader.SwirlTexture = AssetManager.Noise.AuroraRays.Value;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, fenixBackCloudsShader.Effect);


        Asset<Texture2D> cloudTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds6");
        spriteBatch.Draw(cloudTextureAsset.Value, targetRect, Color.White);

        spriteBatch.End();

        graphicsDevice.SetRenderTarget(_domainRTSwap);
        graphicsDevice.Clear(Color.Transparent);

        Color outlineColor = new Color(150, 150, 235) * 0.5f;
        Color outlineColor2 = new Color(235, 150, 235) * 0.5f;
        Color outlineColor3 = Color.Lerp(outlineColor, outlineColor2, ExtraMath.Osc(0f, 1f));
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        RoyalOutlineShader mixerShader2 = ShaderContent.GetInstance<RoyalOutlineShader>();
        mixerShader2.TexelSize = texelSize;
        mixerShader2.OutlineColor = outlineColor3;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, mixerShader2.Effect);
        spriteBatch.Draw(_domainRT, targetRect, Color.White);
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
        if (ShouldRenderClouds())
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.Clear(Color.Transparent);
            Color drawColor2 = Color.SkyBlue;
            drawColor2 *= ExtraMath.Osc(0.25f, 0.35f, speed: 0.3f);

            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.FocusMode)
            {
                spriteBatch.Draw(_domainRTSwap, Vector2.Zero, drawColor2);
                spriteBatch.Draw(_domainRTSwap, Vector2.Zero, null, Color.White * 0.2f, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);

            }


            //  spriteBatch.Draw(TextureAssets.BlackTile.Value, targetRect, Color.White);
            DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (singularityFallSystem.hoveringPlatform)
            {
                Texture2D bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
                Vector2 drawOrigin = new Vector2(bloomLine.Size().X / 2, 0);
                float rotation = MathHelper.PiOver2;
                Color drawColor = Color.White;
                drawColor.A = 0;
                drawColor *= 0.5f;
                drawColor *= ExtraMath.Osc(0.5f, 1f);
                Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, singularityFallSystem.hoverPlatformY);
                drawPosition -= Main.screenPosition;
                drawPosition.Y += 48;
                Vector2 drawScale = new Vector2(1, 2);
                spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, -rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }


            drawFenix = false;
        }

        orig(self, behindTiles);
    }
}
