using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;




[Autoload(Side = ModSide.Client)]
public class InfraredRenderer : ModSystem
{
    private ManagedRenderTarget _infraredMask;
    private ManagedRenderTarget _tileRenderTarget;
    //private float infraredTimer;
    public bool IsActive => !Main.gameMenu && Main.LocalPlayer.GetModPlayer<HeatGogglesPlayer>().hasHeatGoggles;
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderMask;
        On_Main.DrawPlayers_AfterProjectiles += DrawToScreen;
    }


    public override void OnModLoad()
    {
        base.OnModLoad();
        _infraredMask = ManagedRenderTarget.New();
        _tileRenderTarget = ManagedRenderTarget.New();
    }

    private void RenderMask(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (!IsActive)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        TileDrawing tilesRenderer = Main.instance.TilesRenderer;

        //Capture NPCs
        spriteBatch.GraphicsDevice.SetRenderTarget(_infraredMask);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.DrawNPCs();
        spriteBatch.End();


        graphicsDevice.SetRenderTarget(_tileRenderTarget);
        graphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        LightingPreDrawEdit.DontRenderPreDraw = true;
        tilesRenderer.PreDrawTiles(true, true, true);
        tilesRenderer.Draw(true, true, true);
        spriteBatch.End();
        LightingPreDrawEdit.DontRenderPreDraw = false;
    }

    private void DrawToScreen(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (!IsActive)
            return;

        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, effect : whiteShader.Effect);
        spriteBatch.Draw(_infraredMask, Vector2.Zero, null, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        spriteBatch.End();

        OutlineShader outlineShader = ShaderContent.GetInstance<OutlineShader>();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, effect: outlineShader.Effect);
        spriteBatch.Draw(_tileRenderTarget, Vector2.Zero - new Vector2(Main.offScreenRange), null, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
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


