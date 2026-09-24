using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
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
    public bool drawFenix;
    public override void OnModLoad()
    {
        On_Main.DrawNPCs += DrawBlack;

    }

    private bool ShouldRenderPlatforms() => drawFenix;
    private bool ShouldRenderClouds() => drawFenix && !ModContent.GetInstance<LunarVeilClientConfig>().FocusMode;
    private void PrepareDomainContent(RenderTargetHandle domainTarget, RenderTargetHandle domainTargetSwap)
    {
        Rectangle targetRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.EndOut(out var oldParameters);
        using(RT.Clear(domainTarget, Color.Transparent))
        {
            FenixDomainShader fenixDomainShader = ShaderContent.GetInstance<FenixDomainShader>();
            fenixDomainShader.GradientMap = TextureRegistry.CloudNoise3.Value;
            fenixDomainShader.Time = Main.GlobalTimeWrappedHourly;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, fenixDomainShader.Effect);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, targetRect, Color.White);
            spriteBatch.End();

            //    spriteBatch.Restart(effect: fenixBackCloudsShader.Effect);

            FenixBackClouds fenixBackCloudsShader = ShaderContent.GetInstance<FenixBackClouds>();
            fenixBackCloudsShader.Time = Main.GlobalTimeWrappedHourly * 0.4f;
            fenixBackCloudsShader.SwirlTexture = AssetManager.Noise.AuroraRays.Value;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, fenixBackCloudsShader.Effect);

            Asset<Texture2D> cloudTextureAsset = AssetReferences.Assets.NoiseTextures.Clouds6.Asset;
            spriteBatch.Draw(cloudTextureAsset.Value, targetRect, Color.White);

            spriteBatch.End();
        }

        using(RT.Clear(domainTargetSwap, Color.Transparent))
        {

            Color outlineColor = new Color(150, 150, 235) * 0.5f;
            Color outlineColor2 = new Color(235, 150, 235) * 0.5f;
            Color outlineColor3 = Color.Lerp(outlineColor, outlineColor2, ExtraMath.Osc(0f, 1f));
            Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
            RoyalOutlineShader mixerShader2 = ShaderContent.GetInstance<RoyalOutlineShader>();
            mixerShader2.TexelSize = texelSize;
            mixerShader2.OutlineColor = outlineColor3;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, mixerShader2.Effect);
            spriteBatch.Draw(domainTarget, targetRect, Color.White);
            spriteBatch.End();
        }
        spriteBatch.Begin(oldParameters);
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

            using var domainTarget = RT.Context(RenderTargets.ScreenTarget);
            using var domainTargetSwap = RT.Context(RenderTargets.ScreenTarget);
            PrepareDomainContent(domainTarget, domainTargetSwap);

            Color drawColor2 = Color.SkyBlue;
            drawColor2 *= ExtraMath.Osc(0.25f, 0.35f, speed: 0.3f);

            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            spriteBatch.Draw(domainTargetSwap, Vector2.Zero, drawColor2);
            spriteBatch.Draw(domainTargetSwap, Vector2.Zero, null, Color.White * 0.2f, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
        }

        if (ShouldRenderPlatforms())
        {
            DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (singularityFallSystem.hoveringPlatform)
            {
                Texture2D bloomLine = AssetReferences.Assets.NoiseTextures.BloomLine.Asset.Value;
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
        }

        drawFenix = false;
        orig(self, behindTiles);
    }
}
