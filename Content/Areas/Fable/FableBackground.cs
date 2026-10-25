using ReLogic.Content;
using Stellamod.Common.BackgroundRendering;
using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.BackupIO;

namespace Stellamod.Content.Areas.Fable;

public class FableBackgroundReal : ABackground
{
    float CalculateReferenceY()
    {
        var surfaceWorld = (float)Main.worldSurface;
        surfaceWorld *= 16;
        var SnapParallaxY = surfaceWorld - 2430;
        var ParallaxReferenceY = SnapParallaxY - 384;
        return ParallaxReferenceY;
    }

    float CalculateYParallaxOffset(float screenYStart, float parallaxYFactor, float referenceY)
    {
        float y = -screenYStart * 512;
        var diffParallaxY = referenceY - Main.Camera.Center.Y;
        var dist = MathF.Abs(diffParallaxY);
        var factor = EasingFunction.InOutSine(dist / 666F);
        var diffDrawY = diffParallaxY * factor;
        if (diffDrawY > 0)
            diffDrawY *= 1f;
        else
            diffDrawY *= 0;

        var snappingParallaxY = (float)(diffDrawY * parallaxYFactor);
        var newY = snappingParallaxY;
        y += newY;
        return y;
    }

    float Offset => 20;
    void DrawBackgroundLayer(SpriteBatch spriteBatch, in BackgroundDrawing draw, Asset<Texture2D> background, int frame, int frameCount, float scale, float parallaxX, float parallaxY)
    {
        var drawPosition = Main.screenPosition;
        var xLoops = draw.CalculateXLoops(background.Width());
        var offset = CalculateYParallaxOffset(0.7f, parallaxY, CalculateReferenceY());

        for (var x = -1; x < xLoops; x++)
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(background, drawPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale *= scale;
            drawer.worldPosition += new Vector2((-draw.cameraMovement.X * parallaxX - drawer.Width * 1.25F) % (drawer.Width * scale), 0);
            drawer.worldPosition.X += drawer.Width * scale * x;
            drawer.worldPosition.Y += offset;
            drawer.worldPosition.Y += Offset;
            drawer.color = draw.lightColor * draw.opacity;
            drawer.VerticalFrame(frame, frameCount);
            Main.spriteBatch.Draw(drawer);
        }
    }
    void DrawBackgroundLayer(SpriteBatch spriteBatch, in BackgroundDrawing draw, Asset<Texture2D> background, int frame, int frameCount, float scale, float parallaxX, float parallaxY, Vector2 drawOffset)
    {
        var drawPosition = Main.screenPosition;
        var xLoops = draw.CalculateXLoops(background.Width());
        var offset = CalculateYParallaxOffset(0.7f, parallaxY, CalculateReferenceY());

        for (var x = -1; x < xLoops; x++)
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(background, drawPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale *= scale;
            drawer.worldPosition += new Vector2(-draw.cameraMovement.X * parallaxX % (drawer.Width * scale), 0);
            drawer.worldPosition.X += drawer.Width * scale * x;
            drawer.worldPosition.Y += offset;
            drawer.worldPosition.Y += Offset;
            drawer.worldPosition += drawOffset;
            drawer.color = draw.lightColor * draw.opacity;
            drawer.VerticalFrame(frame, frameCount);
            Main.spriteBatch.Draw(drawer);
        }
    }

    public override void Draw(SpriteBatch spriteBatch, in BackgroundDrawing draw)
    {
        DrawBackgroundLayer(spriteBatch, draw, AssetReferences.Assets.Textures.Backgrounds.Fable.Asset, 3, 4, scale: 2, parallaxX: 0.1f, parallaxY: 0.1f, drawOffset: new Vector2(0, 533 * 2));
        DrawBackgroundLayer(spriteBatch, draw, AssetReferences.Assets.Textures.Backgrounds.Fable.Asset, 2, 4, scale: 2, parallaxX: 0.06f, parallaxY: 0.1f);

        DrawBackgroundLayer(spriteBatch, draw, AssetReferences.Assets.Textures.Backgrounds.Fable.Asset, 3, 4, scale: 2, parallaxX: 0.1f, parallaxY: 0.165f, drawOffset: new Vector2(0, 533 * 2));
        DrawBackgroundLayer(spriteBatch, draw, AssetReferences.Assets.Textures.Backgrounds.Fable.Asset, 1, 4, scale: 2, parallaxX: 0.09f, parallaxY: 0.165f);
        FableParticles.MiniWitch.Draw(spriteBatch, Main.screenPosition);
        FableParticles.MiniLantern.Draw(spriteBatch, Main.screenPosition);

        DrawBackgroundLayer(spriteBatch, draw, AssetReferences.Assets.Textures.Backgrounds.Fable.Asset, 0, 4, scale: 2, parallaxX: 0.12f, parallaxY: 0.25f);
        DrawBackgroundLayer(spriteBatch, draw, AssetReferences.Assets.Textures.Backgrounds.Fable.Asset, 3, 4, scale: 2, parallaxX: 0.1f, parallaxY: 0.25f, drawOffset: new Vector2(0, 533*2));
        FableParticles.NyoomSparkle.Draw(spriteBatch, Main.screenPosition);
        FableParticles.Witch.Draw(spriteBatch, Main.screenPosition);
        FableParticles.Lantern.Draw(spriteBatch, Main.screenPosition);

        var noiseSprite = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Value;
        var ditherSprite = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value;
        var pass = AssetReferences.Effects.Abyss.AbyssFog.CreateBlackPass();
        HlslSampler sampler = new HlslSampler();
        sampler.Sampler = SamplerState.PointWrap;
        sampler.Texture = noiseSprite;
        pass.Parameters.spriteSampler = sampler;

        HlslSampler ditherSampler = new HlslSampler();
        ditherSampler.Sampler = SamplerState.PointWrap;
        ditherSampler.Texture = ditherSprite;
        pass.Parameters.ditherSampler = ditherSampler;

        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.4f;
        pass.Parameters.ditherTexelSize = ditherSprite.GetTexelSize();
        pass.Parameters.spriteSize = noiseSprite.Size();
        pass.Parameters.screenOffset = DrawUtilities.CalculateScreenOffset(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight)) * 0.3f;
        pass.Apply();
        using (spriteBatch.Ctx(spriteBatch.Parameters with { effect = pass.Shader }))
        {
            Color fogColor = Color.Lerp(Color.White, Color.Blue, 0.3f);
            spriteBatch.Draw(noiseSprite, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), fogColor * 0.36f);
        }
        //    FableParticles.BigLantern.Draw(spriteBatch, Main.screenPosition);
    }

    public override bool IsActive()
    {
      // return false;
        return Main.LocalPlayer.ZoneWitchesFable;
    }
}
public class FableBackground : CustomBG
{
    public CustomBGLayer bleedLayer;
    public CustomBGLayer farLayer;
    public CustomBGLayer midLayer;
    public CustomBGLayer closeLayer;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1.5f;
        DrawOffset = new Vector2(0, -100);

        float startParallax = 0.15f;

        farLayer = new CustomBGLayer();
        farLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableFar.Asset;
        farLayer.Parallax = startParallax;
        farLayer.DrawOffset = Vector2.Zero;
        AddLayer(farLayer);


        midLayer = new CustomBGLayer();
        midLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableMid.Asset;
        midLayer.Parallax = startParallax + 0.02f;
        midLayer.DrawOffset = Vector2.Zero;
        AddLayer(midLayer);

        closeLayer = new CustomBGLayer();
        closeLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableClose.Asset;
        closeLayer.Parallax = startParallax + 0.05f;
        closeLayer.DrawOffset = Vector2.Zero;
        AddLayer(closeLayer);


        bleedLayer = new CustomBGLayer();
        bleedLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableUnder.Asset;
        bleedLayer.Parallax = startParallax;
        bleedLayer.DrawOffset = Vector2.Zero;
        AddLayer(bleedLayer);

    }
    public override void SetDrawDefaults()
    {
        base.SetDrawDefaults();
        NoSurfaceOffset = true;
        DrawScale = 1f;
        LocalParallaxSpeed = 1 / 1.5f;
        farLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableFar.Asset;
        farLayer.Parallax = 0.06f;
        midLayer.Parallax = 0.09f;
        closeLayer.Parallax = 0.12f;

        var p = (float)Main.worldSurface;
        p *= 16;
        SnapParallaxY = p - 2430;
        ParallaxReferenceY = SnapParallaxY - 384;

        closeLayer.SnapParallaxYFactor = 0.25f;

        bleedLayer.SnapParallaxYFactor = closeLayer.SnapParallaxYFactor;
        midLayer.SnapParallaxYFactor = 0.165f;
        farLayer.SnapParallaxYFactor = 0.1f;
        bleedLayer.DrawOffset = new Vector2(0, bleedLayer.Texture.Size().Y * DrawScale * 1.9f);

        DrawScreenPositionYStart = 0.7f;
        DrawOffset = new Vector2(0, 150);
    }

    public override bool IsActive()
    {
        return false;
        return Main.LocalPlayer.ZoneWitchesFable;
    }
}
