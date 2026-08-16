using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.TilesNew.RainforestTiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.TilesTR;

public class AegislavSand : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<AegislavSandTile>());
    }
}

public class AegislavSandTile : ModTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[TileID.Dirt][Type] = true;
        Main.tileMerge[TileID.Grass][Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;
        TileSets.AegisMisty[Type] = true;
        RegisterItemDrop(ModContent.ItemType<AegislavSand>());
        AddMapEntry(new Color(40, 40, 40));
    }
    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {

    }
    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.SpecialDraw(i, j, spriteBatch);
      
    }
}

public class AegislavDustGlobalTile : GlobalTile
{
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, type, spriteBatch, ref drawData);
        if (TileSets.AegisMisty[type] &&
            ExtraMath.Osc(0, 1, 0, offset: i + j) <= 0.1f &&
            WorldGen.TileIsExposedToAir(i, j))
        {
            AegislavDustRenderer.DustPoints.Add(new Point(i, j));
        }
    }
}

[Autoload(Side = ModSide.Client)]
public class AegislavDustRenderer : ModSystem
{
    private Asset<Texture2D> _maskTexture;
    private Asset<Texture2D> _cloudTexture;
    public static readonly HashSet<Point> DustPoints = new();
    private RenderTargetProvider _maskRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private RenderTargetProvider _cloudRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);

    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderDustMask;
        On_Main.RenderTiles += ResetDustPoints;
    }

    private void ResetDustPoints(On_Main.orig_RenderTiles orig, Main self)
    {
        if (!Main.drawToScreen)
        {
            DustPoints.Clear();
        }
        orig(self);
    }


    private void RenderDustClouds(SpriteBatch sb, Vector2 screenPos)
    {
        _cloudTexture ??= ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds2");
        AegislavDustShader dustShader = AegislavDustShader.Instance;
        dustShader.Tiling = new Vector2(1f, 1f);

      
        dustShader.Parallax = Vector2.Zero;
        sb.GraphicsDevice.Textures[1] = _cloudRT;
        sb.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
        sb.Restart(effect: dustShader.Effect, samplerState: SamplerState.LinearWrap);

        Color fogColor = Color.Pink * 0.9f;
        fogColor.A = 0;
        sb.Draw(_maskRT, Vector2.Zero, fogColor);
        sb.RestartDefaults();
    }

  

    private void RenderDustMask(On_Main.orig_CheckMonoliths orig)
    {
        if (!Main.gameMenu && DustPoints.Count > 0)
        {
            GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
            gDevice.SetRenderTarget(_maskRT);
            gDevice.Clear(Color.Transparent);
            SpriteBatch spriteBatch = Main.spriteBatch;

            bool renderClouds = true;
            _maskTexture = AssetManager.GlowMask.SimpleGlowCircle;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix); ;

            SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(_maskTexture, Vector2.Zero);
            sbDrawer.color = Color.White;
            sbDrawer.color.A = 0;
            sbDrawer.scale *= 0.5f;
            foreach(Point point in DustPoints)
            {
                Vector2 worldCoordinates = point.ToWorldCoordinates();
                sbDrawer.worldPosition = worldCoordinates;
                spriteBatch.Draw(sbDrawer);
            }

            spriteBatch.End();
            PixelationManager.QueueSpritebatchDrawAction(RenderDustClouds, DrawLayer.OverPlayers);

            gDevice.SetRenderTarget(_cloudRT);
            gDevice.Clear(Color.Transparent);

            if (renderClouds)
            {
                _cloudTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds2");
                BackgroundParallaxShader pShader = BackgroundParallaxShader.Instance;
                Vector2 parallax = Main.screenPosition * 0.0001f + new Vector2(Main.GlobalTimeWrappedHourly * -0.015f, 0.0f);
                

                Vector2 texelSize = Vector2.One / new Vector2(_maskTexture.Width(), _maskTexture.Height());
                Vector2 screenoffset = Main.screenPosition * texelSize;
                screenoffset *= (1f / 4f);
                screenoffset.Y *= 2f;

                pShader.Parallax = parallax + screenoffset;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
                    pShader.Effect); ;

                Rectangle dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(_cloudTexture.Value, dstRect, null, Color.White);
                spriteBatch.End();
            }
        }

        orig();
        //throw new NotImplementedException();
    }

    public override void Unload()
    {
        base.Unload();
    }
}