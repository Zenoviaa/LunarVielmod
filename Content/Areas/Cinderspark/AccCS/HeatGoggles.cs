using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;




[Autoload(Side = ModSide.Client)]
public class InfraredRenderer : ModSystem
{
    private RenderTargetProvider _tileRenderTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public bool IsActive => !Main.gameMenu && Main.LocalPlayer.GetModPlayer<HeatGogglesPlayer>().hasHeatGoggles;
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderMask;
        On_Main.DrawPlayers_AfterProjectiles += DrawToScreen;
    }

    private Type[] _invokeTypes;
    private object[] _invokeParams;
    private MethodInfo _drawWatersMethod;
    private void RenderMask(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (!IsActive)
        {
            return;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

        spriteBatch.GraphicsDevice.SetRenderTarget(_tileRenderTarget);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


        //TODO:
        try
        {

            _invokeTypes ??= new Type[]
            {
                    typeof(bool)
            };
            _invokeParams ??= new object[]
            {
                false
            };

            //Cache the method info so we're not spamming reflection calls
            _drawWatersMethod ??= typeof(Main).GetMethod("DrawNPCs", BindingFlags.NonPublic | BindingFlags.Instance, _invokeTypes);
            _drawWatersMethod.Invoke(Main.instance, _invokeParams);
        }
        catch
        {
        }

//        Main.instance.DrawNPCs();
     
        spriteBatch.End();
    }

    private void DrawToScreen(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (!IsActive)
            return;

        var target = Main.instance.tileTarget;
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        SpriteBatch spriteBatch = Main.spriteBatch;
        var outlineShader = ShaderContent.GetInstance<WhiteOutlineShader>();
        outlineShader.TexelSize = Vector2.One / target.Size();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, effect: outlineShader.Effect);
        spriteBatch.Draw(target, Main.sceneTilePos - Main.screenPosition, null, Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, effect: whiteShader.Effect);
        spriteBatch.Draw(_tileRenderTarget, Vector2.Zero, null, Color.Orange * ExtraMath.Osc(0.5f, 1f, speed: 3), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        spriteBatch.End();
    }
}
public class HeatGogglesPlayer : ModPlayer
{
    public bool hasHeatGoggles;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasHeatGoggles = false;
    }
}

public class HeatGoggles : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.rare = ModContent.RarityType<CinderscrapRarity>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<HeatGogglesPlayer>().hasHeatGoggles = true;
       
    }
}


