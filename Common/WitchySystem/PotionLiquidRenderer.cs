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
    float Size => 256;
    float IncrementDivisor => 18F;
    float StartY => 232;
    public Vector2[] metaballs = new Vector2[32];
    public Vector2[] backMetaballs = new Vector2[32];
    public float waveAmplitude;
    public float waveFrequency;

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
            var ratio = f / (float)backMetaballs.Length;

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
            var ratio = f / (float)metaballs.Length;

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

    public void Simulate()
    {
        SimulateBack();
        SimulateFront();
    }

    public void Render(SpriteBatch spriteBatch, Vector2 pos)
    {
        var manaColor = new Color(48, 64, 236);
        var pass = AssetReferences.Effects.Generic.PotionLiquid.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly + BackLiquidTimeOffset;
        pass.Parameters.outsideColor = Color.White.ToVector4();
        pass.Parameters.insideColor = Color.Lerp(Color.DarkRed, Color.Black, 0.25f).ToVector4();
        pass.Parameters.metaballRadius = 0.65F;
        pass.Parameters.metaballStrength = 0.05f;
        pass.Parameters.metaballs = backMetaballs;
        pass.Parameters.outlineColor = Color.Lerp(Color.Red, Color.White, 0.3f).ToVector4();
        var noiseTex = AssetReferences.Assets.LaserTextures.Aura.Asset;
        var ditherTex = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset;
        pass.Parameters.spriteSize = noiseTex.Size();
        pass.Parameters.noiseTexelSize = ditherTex.GetTexelSize();
        pass.Apply();

        //idk why the above other sampler no work tbh
        //but this does!
        //might need hepler functions for this later.
        Main.graphics.GraphicsDevice.Textures[2] = ditherTex.Value;
        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.LinearWrap;

        var drawer = SpritebatchDrawer.FromTextureAsset(noiseTex, pos + Main.screenPosition) with
        { 
            drawOrigin = Vector2.Zero,
            color = Color.Lerp(Color.DarkRed, Color.Black, 0.3f) 
        };

        var d2 = drawer with { spriteEffects = SpriteEffects.FlipHorizontally };
        drawer.worldPosition.Y -= 16;
        using (spriteBatch.Ctx(SpritebatchParams.UI with { effect = pass.Shader }))
        {
            spriteBatch.Draw(drawer);
        }
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.metaballs = metaballs;
        pass.Apply();
        using (spriteBatch.Ctx(SpritebatchParams.UI with { effect = pass.Shader }))
        {
            spriteBatch.Draw(d2);
        }

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value,
            Main.LocalPlayer.statLife.ToString(), pos + new Vector2(128), Color.White, 0, new Vector2(32, -32), Vector2.One);
    }
}
