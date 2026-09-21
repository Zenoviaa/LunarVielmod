using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Godrays;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.ZTileSystem;
using Stellamod.Gores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class GoldenPottedTree : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 4;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class GoldenLeafPile : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FrozenCrate : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FrozenBarrel : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class HangingGong : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;
    }
}
public class FurnaceBricks : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FurnaceIngots : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class Anvil : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class BigTent : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class HangingSmallMedallion : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 4;
        drawOrigin = TileDrawOrigin.TopDown;
    }
}

public class HangingChimes : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;
    }
}

public class HangingChimesAlt : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;
    }
}

public class Tent : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class BigGong : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class Gong : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class BigStoneFurnace : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class AbyssFlowerTable : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class AbyssFlowerChair : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class AbyssFlowerBed : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class ZuiPoster : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.Center;
    }
}

public class AbyssStatue : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 8;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}




public class AbyssEreshStatue : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}






public class BigBigTent : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}




public class GoldenSign : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}





public class RoyalTable : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}

public class RoyalTablePlant : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}






public class RoyalChair : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}




public class GrandCurtains : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}


public class GrandWindow : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}




public class GrandPillar : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}



public class GrandWall : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}



public class GrandPanel : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}









public class GrandAcademySigil : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.Center;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}







public class GrandMiniBanner : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        //TODO: Don't spam ModContent.Request
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

        Vector2 flagPosition = drawPosition;
        flagPosition.X += ExtraMath.Osc(0f, 4, speed: 3);
        //  flagPosition.Y -= texture.Height() * 0.5f;
        Vector2 drawOrigin = new Vector2(texture.Width() / 2f, 0f);
        BannerWavingShader wavingShader = BannerWavingShader.Instance;
        wavingShader.OscStrength = 0.1f;
        wavingShader.XOffset = 4;
        wavingShader.Time = Main.GlobalTimeWrappedHourly * 2 + drawParams.tilePosition.x;

        using(new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = wavingShader }))
        {
            spriteBatch.Draw(texture.Value, flagPosition, null, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
        }

        return false;
    }
}


public class GrandBigBanner : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        //TODO: Don't spam ModContent.Request
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

        Vector2 flagPosition = drawPosition;
        flagPosition.X += ExtraMath.Osc(0f, 4, speed: 3);
        //  flagPosition.Y -= texture.Height() * 0.5f;
        Vector2 drawOrigin = new Vector2(texture.Width() / 2f, 0f);
        BannerWavingShader wavingShader = BannerWavingShader.Instance;
        wavingShader.OscStrength = 0.1f;
        wavingShader.XOffset = 4;
        wavingShader.Time = Main.GlobalTimeWrappedHourly * 2 + drawParams.tilePosition.x;

        using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = wavingShader }))
        {
            spriteBatch.Draw(texture.Value, flagPosition, null, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
        }

        return false;
    }
}


file static class GoldenLeafTreeUtility
{
    public static void UpdateGoldenLeafTree(int i, int j)
    {
        Point spawnPoint = new Point(i, j);
        Vector2 worldCoordinates = spawnPoint.ToWorldCoordinates();
        if (Main.hasFocus && Main.netMode != NetmodeID.Server)
        {
            if (Main.rand.NextBool(64))
            {
                Vector2 leafSpawnPoint = worldCoordinates + Main.rand.NextVector2Circular(164, 64);
                leafSpawnPoint.Y -= 128;
                Gore.NewGore(leafSpawnPoint, Vector2.Zero, ModContent.GoreType<FallingLeafGold>(), Main.rand.NextFloat(0.6f, 1f));
            }

            if (Main.rand.NextBool(128))
            {
                GodrayRenderer godrayRenderer = ModContent.GetInstance<GodrayRenderer>();
                Vector2 centerPos = worldCoordinates;
                centerPos.Y -= 196;
                godrayRenderer.AddGodrayParticle(centerPos + Main.rand.NextVector2Circular(256, 64));
            }

        }
    }
    public static void UpdateBigGoldenLeafTree(int i, int j)
    {
        Point spawnPoint = new Point(i, j);
        Vector2 worldCoordinates = spawnPoint.ToWorldCoordinates();
        if (Main.hasFocus && Main.netMode != NetmodeID.Server)
        {
            if (Main.rand.NextBool(32))
            {
                Vector2 leafSpawnPoint = worldCoordinates + Main.rand.NextVector2Circular(164, 64);
                leafSpawnPoint.Y -= 256;
                Particles.FallingBigGoldenLeaf.Spawn(new()
                {
                    position = leafSpawnPoint,
                    timeLeft = 600
                });

                //       Gore.NewGore(leafSpawnPoint, Vector2.Zero, ModContent.GoreType<FallingLeafGold>(), Main.rand.NextFloat(0.6f, 1f));
            }
            if (Main.rand.NextBool(32))
            {
                GodrayRenderer godrayRenderer = ModContent.GetInstance<GodrayRenderer>();
                Vector2 centerPos = worldCoordinates;
                godrayRenderer.AddGodrayParticle(centerPos + Main.rand.NextVector2Circular(256, 16));
            }
        }

    }

}


public class GoldenLeafTree : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 6;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
        GoldenLeafTreeUtility.UpdateGoldenLeafTree(i, j);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        Vector2 worldCoordinates = new Point(drawParams.tilePosition.x, drawParams.tilePosition.y).ToWorldCoordinates();
        Lighting.AddLight(worldCoordinates + new Vector2(0, -16), TorchID.Torch);

        Rectangle srcRect = _tileTextureAsset.Value.GetFrame(drawParams.tileData.frameNumber, 6);
        Vector2 drawOrigin = new Vector2(srcRect.Width / 2f, srcRect.Height);
        Vector2 offset = new Vector2(0, srcRect.Height / 2);
        spriteBatch.Draw(_tileTextureAsset.Value, drawPosition + offset, srcRect, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);

        srcRect = _tileTextureAsset.Value.GetFrame(drawParams.tileData.frameNumber + 1, 6);
        spriteBatch.Draw(_tileTextureAsset.Value, drawPosition + offset, srcRect, drawParams.lightColor, GetLeafSway(drawParams.tilePosition.x, 0.01f, 0.02f), drawOrigin, 1, SpriteEffects.None, 0);
        return false;
    }
}

public class GrandGoldenLeafTree : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    
    public override void Update(int i, int j)
    {
        base.Update(i, j);
        GoldenLeafTreeUtility.UpdateBigGoldenLeafTree(i, j);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        Vector2 worldCoordinates = new Point(drawParams.tilePosition.x, drawParams.tilePosition.y).ToWorldCoordinates();
        Lighting.AddLight(worldCoordinates + new Vector2(0, -16), TorchID.Torch);

        Rectangle srcRect = _tileTextureAsset.Value.GetFrame(0, 2);
        Vector2 drawOrigin = new Vector2(srcRect.Width / 2f, srcRect.Height);
        Vector2 offset = new Vector2(0, srcRect.Height / 2);
        spriteBatch.Draw(_tileTextureAsset.Value, drawPosition + offset, srcRect, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);

        srcRect = _tileTextureAsset.Value.GetFrame(1, 2);
        spriteBatch.Draw(_tileTextureAsset.Value, drawPosition + offset, srcRect, drawParams.lightColor, GetLeafSway(drawParams.tilePosition.x, 0.01f, 0.02f), drawOrigin, 1, SpriteEffects.None, 0);

        //Godrays here
        Vector2 wp = new Point(drawParams.tilePosition.x, drawParams.tilePosition.y).ToWorldCoordinates();
        Asset<Texture2D> godrayTexture = AssetManager.GlowMask.SimpleGlowCircle;
        Vector2 origin = godrayTexture.Size() * 0.5f;
        Vector2 godrayScale = new Vector2(0.35f, 2f);

        float godrayOffset = 150;
        SpritebatchDrawer sbDrawer2 = SpritebatchDrawer.FromTextureAsset(godrayTexture, wp + new Vector2(144, -128));
        sbDrawer2.color = Color.White * 0.26f * ExtraMath.Osc(0f, 1f, speed: 1);
        sbDrawer2.color.A = 0;
        sbDrawer2.rotation -= MathHelper.ToRadians(-0);
        sbDrawer2.scale *= godrayScale;
        sbDrawer2.worldPosition.Y -= godrayOffset;
        sbDrawer2.worldPosition.X -= 64;
        Main.spriteBatch.Draw(sbDrawer2);

        sbDrawer2.color = Color.Gold * 0.26f * ExtraMath.Osc(0f, 1f, speed: 1, offset: 1);
        sbDrawer2.color.A = 0;
        sbDrawer2.worldPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 1) * 64;
        sbDrawer2.worldPosition.Y -= godrayOffset;
        sbDrawer2.worldPosition.X -= 64;
        Main.spriteBatch.Draw(sbDrawer2);
        return false;
    }
}




public class GrandChand : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;

        //idk
        windSwayOffset = 0f;

        //The max it can sway
        windSwayMagnitude = 0.05f;

        //How fast it sways
        windSwaySpeed = 0.02f;
    }

    public override void Update(int i, int j)
    {
        base.Update(i, j);
        Vector2 worldCoordinates = new Point(i, j).ToWorldCoordinates();
        Lighting.AddLight(worldCoordinates + new Vector2(0, -16), TorchID.Torch);
    }
}




public class GoldenArch : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }

    public override void Update(int i, int j)
    {
        base.Update(i, j);
    }
}
































































