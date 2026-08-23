using ReLogic.Content;
using Stellamod.Core.ZTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.TilesTR;

public class AegislavFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 4;
        drawOrigin = TileDrawOrigin.BottomUp;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}

public class AegislavHangingCage : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.08f;
        windSwaySpeed = 0.02f;
    }
}

public class BloodyCauldron : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class HangingBloodyCauldron : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}
public class DeadShrub : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.1f;
        windSwaySpeed = 0.02f;
    }
}
public class BloodCathedralWindow : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class TheDreadmire : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.Center;
    }
}
public class GrimmingPainting : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.Center;
    }
}
public class AegislavBlade : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class AegislavBookcase : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class AegislavSmallChain : AbstractZTileChain { }
public class AegislavLargeChain : AbstractZTileChain { }

public abstract class HangingGlowBottle : ZTile
{
    private Asset<Texture2D> _glowMaskTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

        _glowMaskTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        //Calculate frame;
        int frameHeight = _glowMaskTextureAsset.Height() / frameCount;
        int frameWidth = _glowMaskTextureAsset.Width();
        int yOffset = frameHeight * drawParams.tileData.frameNumber;
        Rectangle frame = new Rectangle(0, yOffset, frameWidth, frameHeight);

        //Calculate hte draworigin
        Vector2 drawOrigin = new Vector2(frame.Width / 2, frame.Height / 2);
        Vector2 drawOffset = Vector2.Zero;
        switch (this.drawOrigin)
        {
            default:
            case TileDrawOrigin.BottomUp:
                drawOffset = new Vector2(0, -frameHeight / 2f);
                break;
            case TileDrawOrigin.Center:
                drawOffset = Vector2.Zero;
                break;
            case TileDrawOrigin.TopDown:
                drawOffset = new Vector2(0, 0);
                drawOrigin = new Vector2(frame.Width / 2, 0);
                break;

        }


        //Since it's gonne default to 0 on old worlds
        //We'll make 255 be black
        Color valueColor = Color.Lerp(Color.White, Color.Black, drawParams.tileData.value / 255f);
        Color drawColor = drawParams.lightColor.MultiplyRGB(valueColor);

        float drawRotation;
        switch (drawParams.tileData.rotation)
        {
            default:
            case Rotation.Degrees_0:
                drawRotation = 0;
                break;
            case Rotation.Degrees_90:
                drawRotation = MathHelper.PiOver2;
                break;
            case Rotation.Degrees_180:
                drawRotation = MathHelper.Pi;
                break;
            case Rotation.Degrees_270:
                drawRotation = MathHelper.Pi + MathHelper.PiOver2;
                break;
        }

        if (rotateSpeed > 0)
        {
            drawRotation += Main.GlobalTimeWrappedHourly * rotateSpeed * 24;
        }

        //Calculate wind if any
        if (windSwayMagnitude > 0)
        {
            drawRotation += GetLeafSway(windSwayOffset + drawParams.tilePosition.x, windSwayMagnitude, windSwaySpeed);
        }

        SpriteEffects spriteEffects = drawParams.tileData.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (drawParams.tileData.flipX)
            drawRotation *= -1;

        Color glowColor = Color.Lerp(Color.White * 0.05f, Color.White * 0.15f, ExtraMath.Osc(0f, 1f, speed: 1));
        glowColor.A = 0;
        spriteBatch.Draw(_glowMaskTextureAsset.Value, drawPosition, frame, glowColor, drawRotation, drawOrigin, drawParams.tileData.scale, spriteEffects, 0);
    }
}


public class HangingSmallBottle : HangingGlowBottle
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 4;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}

public class HangingMediumBottle : HangingGlowBottle
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}

public class HangingBigBottle : HangingGlowBottle
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;
    }
}


public class HangingBloodyPot : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}



public class BloodAltar : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class BloodBath : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}