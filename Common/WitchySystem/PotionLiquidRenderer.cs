using System;
using Terraria;

namespace Stellamod.Common.WitchySystem;

public class PotionLiquidRenderer
{
    float BackLiquidTimeOffset => 2.4f;
    float BaseFrequency => 0.6f;
    float BaseSpeed => 2f;
    float Size => 528;
    float IncrementDivisor => 18F;
    float StartY => 232;

    public Vector2[] metaballs = new Vector2[32];
    public Vector2[] backMetaballs = new Vector2[32];
    public float waveAmplitude;
    public float waveFrequency;


    public Color BloodColor => Main.LocalPlayer.GetModPlayer<PotionHealthPlayer>().bloodColor;
    public Color DarkBloodColor => Main.LocalPlayer.GetModPlayer<PotionHealthPlayer>().darkBloodColor;

    public void SimulateBackWithInitialPotions(Vector2[] referencePoints, Vector2 topLeft, Vector2 size)
    {
        if (backMetaballs.Length != referencePoints.Length)
            backMetaballs = new Vector2[referencePoints.Length];
        // Main.NewText(size);
        float t = Main.GlobalTimeWrappedHourly;
        for (var i = 0; i < referencePoints.Length && i < backMetaballs.Length; i++)
        {
            ref var rp = ref referencePoints[i];
            var offset = (rp - topLeft);

            ref var metaball = ref backMetaballs[i];
            var targetX = (offset.X / Size) + (MathF.Cos(BackLiquidTimeOffset + t * BaseSpeed + offset.Y * BaseFrequency * waveFrequency) * waveAmplitude);
            var targetY = (offset.Y / Size) + (MathF.Sin(BackLiquidTimeOffset + t * BaseSpeed + offset.X * BaseFrequency * waveFrequency) * waveAmplitude);

            metaball.X = MathHelper.Lerp(metaball.X, targetX, 0.1F);
            metaball.Y = MathHelper.Lerp(metaball.Y, targetY, 0.1F);
        }
    }
    public void SimulateWithInitialPotions(Vector2[] referencePoints, Vector2 topLeft, Vector2 size)
    {
        if (metaballs.Length != referencePoints.Length)
            metaballs = new Vector2[referencePoints.Length];
        for (var i = 0; i < referencePoints.Length && i < metaballs.Length; i++)
        {
            ref var rp = ref referencePoints[i];
            var offset = (rp - topLeft);

            ref var metaball = ref metaballs[i];
            var targetX = (offset.X / Size) + (MathF.Cos(Main.GlobalTimeWrappedHourly * BaseSpeed + offset.Y * BaseFrequency * waveFrequency) * waveAmplitude);
            var targetY = (offset.Y / Size) + (MathF.Sin(Main.GlobalTimeWrappedHourly * BaseSpeed + offset.X * BaseFrequency * waveFrequency) * waveAmplitude);
            metaball.X = MathHelper.Lerp(metaball.X, targetX, 0.1F);
            metaball.Y = MathHelper.Lerp(metaball.Y, targetY, 0.1F);
        }
    }
    public void Simulate()
    {
        //      SimulateBack();
        //SimulateFront();
    }


    public void Render(SpriteBatch spriteBatch, Vector2 pos)
    {
        var noiseTex = AssetReferences.Assets.Textures.UI.Witchy.PotionLiquid.Asset;
        var ditherTex = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset;
        var manaColor = new Color(48, 64, 236);

        var drawer = SpritebatchDrawer.FromTextureAsset(noiseTex, pos + Main.screenPosition) with
        {
            drawOrigin = Vector2.Zero,
            color = Color.Lerp(DarkBloodColor, Color.Black, 0.3f)
        };

        var noMaskPass = AssetReferences.Effects.Generic.LiquidHealth.NoMaskPotionLiquid.CreateNoMaskPass();
        noMaskPass.Parameters.time = Main.GlobalTimeWrappedHourly + BackLiquidTimeOffset;
        noMaskPass.Parameters.outsideColor = Color.White.ToVector4();
        noMaskPass.Parameters.insideColor = Color.Lerp(DarkBloodColor, Color.Black, 0.25f).ToVector4();
        noMaskPass.Parameters.metaballRadius = 0.15F;
        noMaskPass.Parameters.metaballStrength = 0.05f;
        noMaskPass.Parameters.metaballs = backMetaballs;
        noMaskPass.Parameters.outlineColor = Color.Lerp(BloodColor, Color.White, 0.3f).ToVector4();
        noMaskPass.Parameters.spriteSize = noiseTex.Size();
        noMaskPass.Parameters.noiseTexelSize = ditherTex.GetTexelSize();
        noMaskPass.Parameters.noiseSampler = new()
        {
            Texture = ditherTex.Value,
            Sampler = SamplerState.LinearWrap
        };
        noMaskPass.Apply();

        var d2 = drawer with { spriteEffects = SpriteEffects.None };
        drawer.worldPosition.Y -= 16;

        var startParameters = SpritebatchParams.UI with
        {
            sortMode = SpriteSortMode.Deferred,
            effect = noMaskPass.Shader,
            samplerState = SamplerState.LinearWrap,
        };
        spriteBatch.Begin(startParameters);
        spriteBatch.Draw(drawer);
        spriteBatch.End();

        var pass = AssetReferences.Effects.Generic.LiquidHealth.PotionLiquid.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly + BackLiquidTimeOffset;
        pass.Parameters.outsideColor = Color.White.ToVector4();
        pass.Parameters.insideColor = Color.Lerp(DarkBloodColor, Color.Black, 0.25f).ToVector4();
        pass.Parameters.metaballRadius = 0.15F;
        pass.Parameters.metaballStrength = 0.05f;
        pass.Parameters.metaballs = backMetaballs;
        pass.Parameters.outlineColor = Color.Lerp(BloodColor, Color.White, 0.3f).ToVector4();

        pass.Parameters.spriteSize = noiseTex.Size();
        pass.Parameters.noiseTexelSize = ditherTex.GetTexelSize();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.metaballs = metaballs;
        pass.Parameters.noiseSampler = new()
        {
            Texture = ditherTex.Value,
            Sampler = SamplerState.LinearWrap
        };
        pass.Apply();
        startParameters.effect = pass.Shader;
        spriteBatch.Begin(startParameters);
        spriteBatch.Draw(d2);
        spriteBatch.End();
    }
}
