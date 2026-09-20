using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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




public class GoldenLeafTree : ZTile
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





public class GrandGoldenLeafTree : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {

   
        Rectangle srcRect = _tileTextureAsset.Value.GetFrame(0, 2);
        Vector2 drawOrigin = new Vector2(srcRect.Width / 2f, srcRect.Height);
        Vector2 offset = new Vector2(0, srcRect.Height / 2);
        spriteBatch.Draw(_tileTextureAsset.Value, drawPosition + offset, srcRect, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);

        srcRect = _tileTextureAsset.Value.GetFrame(1, 2);
        spriteBatch.Draw(_tileTextureAsset.Value, drawPosition + offset, srcRect, drawParams.lightColor, GetLeafSway(drawParams.tilePosition.x, 0.01f, 0.02f), drawOrigin, 1, SpriteEffects.None, 0);
        return false;
    }
}






































































