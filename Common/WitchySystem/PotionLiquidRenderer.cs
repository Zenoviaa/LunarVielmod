using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.WitchySystem;

public class WitchyUIManaPotion : UIPanel
{
    public PotionLiquidRenderer potionLiquidRenderer;
    public int RelativeLeft => 640;
    public int RelativeTop => 252;
    public WitchyUIManaPotion()
    {
        potionLiquidRenderer = new();

        potionLiquidRenderer.waveFrequency = 8;
        potionLiquidRenderer.waveAmplitude = 32;
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        SetPos();
    }

    private void SetPos()
    {
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        potionLiquidRenderer.waveAmplitude = 0.1f;
        potionLiquidRenderer.waveFrequency = 0.03f;
        potionLiquidRenderer.Simulate();

        //Constantly lock the UI in the position regardless of resolution changes
        SetPos();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        var tl = GetDimensions().ToRectangle();
        potionLiquidRenderer.Render(spriteBatch, tl.TopLeft());
    //    var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, tl.TopLeft() + Main.screenPosition);
  //      spriteBatch.Draw(drawer);
        //Main.NewText("oh yeah");
    }
}
public class WitchyUIState : UIState
{
    public WitchyUIManaPotion manaPotion;
    public WitchyUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        manaPotion = new();
        Append(manaPotion);
    }
}


[Autoload(Side = ModSide.Client)]
public class WitchyUI : ModSystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    WitchyUIState witchyUIState;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        witchyUIState = new();
    }

    public override void UpdateUI(GameTime gameTime)
    {

        if (_userInterface.CurrentState == null)
        {
         //   _userInterface.SetState(witchyUIState);
        }

        _lastUpdateUiGameTime = gameTime;
        if (_userInterface?.CurrentState != null)
        {

            _userInterface.Update(gameTime);
        }
    }


    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "LunarVeil: Witchy UI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}

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
    public Color bloodColor;
    public Color darkBloodColor;

    public Color BloodColor => Main.LocalPlayer.GetModPlayer<PotionHealthPlayer>().bloodColor;
    public Color DarkBloodColor => Main.LocalPlayer.GetModPlayer<PotionHealthPlayer>().darkBloodColor;

    void SimulateBack()
    {
       
        var x = 0F;
        var y = 0F;
        y = StartY;
        var increment = Size / IncrementDivisor;
        float t = Main.GlobalTimeWrappedHourly;
        for (var i = 0; i < backMetaballs.Length; i++)
        {
            var f = (float)i;
            ref var metaball = ref backMetaballs[i];

            metaball.X = (x / Size) + (MathF.Cos(BackLiquidTimeOffset + t * BaseSpeed + y * BaseFrequency * waveFrequency) * waveAmplitude);
            metaball.Y = (y / Size) + (MathF.Sin(BackLiquidTimeOffset + t * BaseSpeed + x * BaseFrequency * waveFrequency) * waveAmplitude);
            x += increment;
            if (x >= Size)
            {
                x = 0;
                y += increment;
            }
        }
    }
    void SimulateFront()
    {
        var x = 0F;
        var y = 0F;
        y = StartY;
        var increment = Size / IncrementDivisor;
        for (var i = 0; i < metaballs.Length; i++)
        {
            var f = (float)i;
            ref var metaball = ref metaballs[i];

            metaball.X = (x / Size) + (MathF.Cos(Main.GlobalTimeWrappedHourly * BaseSpeed + y * BaseFrequency * waveFrequency) * waveAmplitude);
            metaball.Y = (y / Size) + (MathF.Sin(Main.GlobalTimeWrappedHourly * BaseSpeed + x * BaseFrequency * waveFrequency) * waveAmplitude);
            x += increment;
            if(x >= Size)
            {
                x = 0;
                y += increment;
            }
        }
    }

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
            var targetX = (offset.X / Size ) + (MathF.Cos(Main.GlobalTimeWrappedHourly * BaseSpeed + offset.Y * BaseFrequency * waveFrequency) * waveAmplitude);
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
