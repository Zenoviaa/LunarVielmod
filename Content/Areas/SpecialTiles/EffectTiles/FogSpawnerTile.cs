using Stellamod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles;

[Autoload(Side = ModSide.Client)]
public class FogTileRenderer : ModSystem
{
    public static readonly List<Point> FogTilePoints = new List<Point>();
    public override void Load()
    {
        base.Load();
        On_Main.DrawPlayers_AfterProjectiles += RenderFogOverPlayers;
        On_Main.RenderTiles += ResetDustPoints;
    }

    private void RenderFogOverPlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (FogTilePoints.Count <= 0)
            return;

        SpritebatchParams worldParams = SpritebatchParams.InWorldAndZoomed();

        HlslSampler spriteSampler = new();
        spriteSampler.Texture = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Value;
        spriteSampler.Sampler = SamplerState.LinearWrap;

        var pass = AssetReferences.Effects.Generic.BigFog.CreatePixelPass();
        pass.Parameters.spriteSampler = spriteSampler;
        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 1.5f;
        pass.Apply();

        worldParams = worldParams with { effect = pass.Shader };
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(worldParams);
        foreach (Point fogTilePoint in FogTilePoints)
        {
            Vector2 worldPos = fogTilePoint.ToWorldCoordinates();
            SpritebatchDrawer fogDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.Clouds.Asset, worldPos);
            fogDrawer.color = Color.White;
            float a = ExtraMath.Osc(0f, 1f, speed: 0, offset: fogTilePoint.X * fogTilePoint.Y);
            fogDrawer.color *= a;
            fogDrawer.scale *= 0.6f;
            a += 0.1f;
            
            spriteBatch.Draw(fogDrawer);
        }
        spriteBatch.End();
    }

    private void ResetDustPoints(On_Main.orig_RenderTiles orig, Main self)
    {
        if (!Main.drawToScreen)
        {
            FogTilePoints.Clear();
        }
        orig(self);
    }

    public override void Unload()
    {
        base.Unload();
        FogTilePoints.Clear();
    }
}

public class FogSpawnerTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(178, 163, 190), name);
        MineResist = 1f;
        MinPick = 145;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, spriteBatch, ref drawData);
        FogTileRenderer.FogTilePoints.Add(new Point(i, j));
    }
    
    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        return base.PreDraw(i, j, spriteBatch);
    }

    /*
    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        LunarLightingRenderer fogSystem = ModContent.GetInstance<LunarLightingRenderer>();
        Point point = new Point(i, j);
        Fog fog = fogSystem.SetupFog(point, FogCreateFunction);
        fog.updateFunc = FogUpdateFunction;
        fog.shaderFunc = FogShaderFunction;
        FogTileRenderer.FogTilePoints.Add(new Point(i, j));
        return base.PreDraw(i, j, spriteBatch);
    }
    public virtual BaseShader FogShaderFunction()
    {
        var fogShader = FogShader.Instance;
        fogShader.FogTexture = TextureRegistry.Clouds6;
        fogShader.ProgressPower = 0.75f;
        fogShader.EdgePower = 1f;
        fogShader.Speed = 1f;
        fogShader.Apply();
        return fogShader;
    }
    private void FogUpdateFunction(Fog fog)
    {

    }

    private void FogCreateFunction(Fog fog)
    {
        fog.startColor = Color.White;
        fog.startScale = new Vector2(Main.rand.NextFloat(0.75f, 1.0f), Main.rand.NextFloat(0.7f, 0.9f)) * 0.9f;
        fog.pulseWidth = Main.rand.NextFloat(0.96f, 0.98f);
        fog.texture = TextureRegistry.Clouds6;
        fog.rotation = Main.rand.NextFloat(-1f, 1f);
        fog.offset = Main.rand.NextVector2Circular(16, 16);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Framing.GetTileSafely(i, j);
        Tile tileBelow = Framing.GetTileSafely(i, j + 1);
        Tile tileAbove = Framing.GetTileSafely(i, j - 1);

        if (!tileAbove.HasTile || !tileBelow.HasTile)
        {
            r = 0.05f;
            g = 0.15f;
            b = 0.25f;
        }
    }*/
}


public class FogSpawnerBlock : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<FogSpawnerTile>());
    }
}