using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Terror.TilesTR;
using Stellamod.Core;
using Stellamod.Core.Foggy;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Rendering;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles;

public class BarrierBlockSystem : ModSystem
{
    public override void OnModLoad()
    {
        base.OnModLoad();
        On_Player.DryCollision += PreDryCollision;
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        On_Player.DryCollision -= PreDryCollision;
    }

    public static Vector2 BossArenaCenter;

    private bool GetNearestBarrierBlock(Player player, out Vector2 worldPoint)
    {
        Vector2 cameraCenterWorld = player.Center;
        Vector2 cameraTopLeft = cameraCenterWorld;// - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
        Vector2 cameraBottomRight = cameraCenterWorld; // + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

        const float range = 64;
        cameraTopLeft -= new Vector2(range);
        cameraBottomRight += new Vector2(range);

        Point topLeftTile = cameraTopLeft.ToTileCoordinates();
        Point bottomRightTile = cameraBottomRight.ToTileCoordinates();

        Vector2 nearest = Vector2.Zero;
        float nearestDistance = 9999f;
        bool success = false;

        topLeftTile.X = Math.Clamp(topLeftTile.X, 0, Main.maxTilesX - 1);
        bottomRightTile.X = Math.Clamp(bottomRightTile.X, 0, Main.maxTilesX - 1);
        topLeftTile.Y = Math.Clamp(topLeftTile.Y, 0, Main.maxTilesY - 1);
        bottomRightTile.Y = Math.Clamp(bottomRightTile.Y, 0, Main.maxTilesY - 1);
        for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
        {
            for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType != ModContent.TileType<BossBarrierBlock>())
                    continue;

                Point tilePoint = new Point(x, y);
                Vector2 position = tilePoint.ToWorldCoordinates();
                float distToPoint = Vector2.Distance(player.Center, position);
                if (distToPoint < nearestDistance)
                {
                    nearest = position;
                    nearestDistance = distToPoint;
                    success = true;
                }
            }
        }

        worldPoint = nearest;
        return success;
    }

    private void PreDryCollision(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
    {
        int barrierBlockType = ModContent.TileType<BossBarrierBlock>();
        Player player = Main.LocalPlayer;
        if (NPC.AnyDanger() && GetNearestBarrierBlock(player, out Vector2 worldPoint))
        {
            Vector2 tileDirectionToBoss = (BossArenaCenter - worldPoint).SafeNormalize(Vector2.Zero);
            Vector2 tileDirectionToPlayer = (player.Center - worldPoint).SafeNormalize(Vector2.Zero);
            //Need to check if the vectors are within 180 degrees of each other, if not then well you can walk through
            float dp = Vector2.Dot(tileDirectionToBoss, tileDirectionToPlayer);
            if (dp < 0)
            {
                Main.tileSolid[barrierBlockType] = false;
            }
        }
        orig(self, fallThrough, ignorePlats);
    }

    public override void PostUpdatePlayers()
    {
        base.PostUpdatePlayers();
        Main.tileSolid[ModContent.TileType<BossBarrierBlock>()] = NPC.AnyDanger();
        Main.tileSolid[ModContent.TileType<SingularityBarrierBlock>()] = !DownedBossTracker.IsDowned(DownedBossFlag.Verlian_Singularity) || !DownedBossTracker.IsDowned(DownedBossFlag.Cariya);
        Main.tileSolid[ModContent.TileType<StarrVeriplantBarrierBlock>()] = !DownedBossTracker.IsDowned(DownedBossFlag.StoneGolem);
        Main.tileSolid[ModContent.TileType<STARBOMBERBarrierBlock>()] = !DownedBossTracker.IsDowned(DownedBossFlag.StarBomber);
        Main.tileSolid[ModContent.TileType<RavagerBarrierBlock>()] = !DownedBossTracker.IsDowned(DownedBossFlag.Woodland_Ravager);
    }
}

public class BarrierFogGlobalTile : GlobalTile
{
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, type, spriteBatch, ref drawData);
        Tile tile = Main.tile[i, j];
        if (!tile.HasTile)
            return;
        if (TileID.Sets.BarrierFog[type] == 0)
            return;

        switch (TileID.Sets.BarrierFog[type])
        {
            case 1:
                BarrierFog.WhiteFogPoints.Add(new Point(i, j));
                break;
            case 2:
                BarrierFog.RedFogPoints.Add(new Point(i, j));
                break;
        }
    }
}
[Autoload(Side = ModSide.Client)]
public class BarrierFog : ModSystem
{
    private RenderTargetProvider _maskRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderBarrierFog;
        On_Main.DrawPlayers_AfterProjectiles += RenderFogOverPlayers;
        On_Main.RenderTiles += ResetDustPoints;
    }
    private void ResetDustPoints(On_Main.orig_RenderTiles orig, Main self)
    {
        if (!Main.drawToScreen)
        {
            WhiteFogPoints.Clear();
            RedFogPoints.Clear();
        }
        orig(self);
    }

    public static List<Point> WhiteFogPoints = new List<Point>();
    public static List<Point> RedFogPoints = new List<Point>();
    private void RenderFogOverPlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;

        if (WhiteFogPoints.Count > 0 || RedFogPoints.Count > 0)
        {
            //Draw clouds over the screen with a nice mask
            var noiseSprite = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Value;
            var ditherSprite = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value;
            var pass = AssetReferences.Effects.Generic.BarrierFog.CreatePixelPass();
            HlslSampler sampler = new HlslSampler();
            sampler.Sampler = SamplerState.PointWrap;
            sampler.Texture = _maskRT;
            pass.Parameters.maskTarget = sampler;


            HlslSampler cloudSampler = new HlslSampler();
            cloudSampler.Sampler = SamplerState.PointWrap;
            cloudSampler.Texture = noiseSprite;
            pass.Parameters.cloudSampler = cloudSampler;

            HlslSampler ditherSampler = new HlslSampler();
            ditherSampler.Sampler = SamplerState.PointWrap;
            ditherSampler.Texture = ditherSprite;
            pass.Parameters.ditherSampler = ditherSampler;

            pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.4f;
            pass.Parameters.ditherTexelSize = ditherSprite.GetTexelSize();
            pass.Parameters.cloudTexelSize = noiseSprite.GetTexelSize();
            pass.Parameters.spriteSize = new Vector2(Main.screenWidth, Main.screenHeight);
            pass.Parameters.screenOffset = DrawUtilities.CalculateScreenOffset(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight));
            pass.Apply();

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, pass.Shader);
            spriteBatch.Draw(_maskRT, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            spriteBatch.End();
        }
    }

    private void RenderBarrierFog(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        //Render out the mask where the clouds will be drawing
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        gDevice.SetRenderTarget(_maskRT);
        gDevice.Clear(Color.Transparent);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteCircle.Asset, Vector2.Zero);
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Max);


        drawer.color = Color.White * 0.7f;

        foreach (Point tilePoint in WhiteFogPoints)
        {
            drawer.worldPosition = tilePoint.ToWorldCoordinates();
            spriteBatch.Draw(drawer);
        }

        drawer.color = Color.Red  * 0.7f;

        foreach (Point tilePoint in RedFogPoints)
        {
            drawer.worldPosition = tilePoint.ToWorldCoordinates();
            spriteBatch.Draw(drawer);
        }
        spriteBatch.End();
    }
}
public abstract class BaseBarrierBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.BarrierFog[Type] = 2;
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[TileID.Mud][Type] = true;
        Main.tileMerge[TileID.ClayBlock][Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;

        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(178, 163, 190), name);

        MineResist = 1f;
        MinPick = 145;
    }
    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        return true;
    }
}

public class BossBarrierBlockItem : ModItem
{
    public override void SetStaticDefaults()
    {
        // Tooltip.SetDefault("Super silk!");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
    }

    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<BossBarrierBlock>();
    }
}

public class BossBarrierBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.BarrierFog[Type] = 1;
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[TileID.Mud][Type] = true;
        Main.tileMerge[TileID.ClayBlock][Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;
     
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(178, 163, 190), name);

        MineResist = 1f;
        MinPick = 145;
    }
    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        return true;
    }
}
public abstract class BarrierBlockItem<T> : ModItem where T : BaseBarrierBlock
{
    public override void SetStaticDefaults()
    {
        // Tooltip.SetDefault("Super silk!");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
    }

    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<T>();
    }

}

public class RavagerBarrierBlockItem : BarrierBlockItem<RavagerBarrierBlock>
{

}

public class RavagerBarrierBlock : BaseBarrierBlock
{

}

public class StarrVeriplantBarrierBlockItem : BarrierBlockItem<StarrVeriplantBarrierBlock>
{

}

public class StarrVeriplantBarrierBlock : BaseBarrierBlock
{

}

public class STARBOMBERBarrierBlockItem : BarrierBlockItem<STARBOMBERBarrierBlock>
{

}

public class STARBOMBERBarrierBlock : BaseBarrierBlock
{

}
public class SingularityBarrierBlockItem : BarrierBlockItem<SingularityBarrierBlock>
{

}

public class SingularityBarrierBlock : BaseBarrierBlock
{

}
