using ReLogic.Content;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponUpgrade.UI;

public struct AnimationFramer
{
    public float time;
    public int frame;
    public int maxFrame;
    public float frameSpeed;
    private void IncreaseFrame()
    {
        while (time >= frameSpeed)
        {
            frame++;
            frame %= maxFrame;
            time -= frameSpeed;
        }
    }
    private void IncreaseFrameDelta()
    {
        float deltaFrameTime = (frameSpeed / 60f);
        if (time >= deltaFrameTime)
        {
            frame++;
            frame %= maxFrame;
            time -= deltaFrameTime;
        }
    }
    public void Update(GameTime gameTime)
    {
        time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        IncreaseFrameDelta();
    }

    public void UpdateTick()
    {
        time++;
        IncreaseFrame();
    }
}
public class FurnaceBackground : UIPanel
{
    private Asset<Texture2D> _backgroundTextureAsset;
    private Asset<Texture2D> _furnaceTextureAsset;
    private Asset<Texture2D> _furnaceBurningTextureAsset;
    private Asset<Texture2D> _furnaceBurningAmbientTextureAsset;

    private AnimationFramer _burningAnimationFrame;
    private AnimationFramer _burningAmbientAnimationFrame;
    public FurnaceBackground() : base()
    {
        _backgroundTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "UpgradeBackground");
        _furnaceTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Furnace");
        _furnaceBurningTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Furnace_Burning");
        _furnaceBurningAmbientTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Furnace_BackBurning");
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 232;
        Height.Pixels = 232;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _burningAnimationFrame.frameSpeed = 2;
        _burningAnimationFrame.maxFrame = 60;
        _burningAnimationFrame.Update(gameTime);

        _burningAmbientAnimationFrame.frameSpeed = 2;
        _burningAmbientAnimationFrame.maxFrame = 60;
        _burningAmbientAnimationFrame.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        Rectangle rectangle = GetDimensions().ToRectangle();
        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
        }


        //Draw Backing
        Vector2 pos = rectangle.TopLeft();
        spriteBatch.Draw(_backgroundTextureAsset.Value, rectangle.TopLeft(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        Vector2 bottomMiddleDrawPosition = rectangle.Bottom();

        //Draw Ambient Flames
        Rectangle frame2 = _furnaceBurningAmbientTextureAsset.Value.GetFrame(_burningAnimationFrame.frame, 6, 10);
        Vector2 origin2 = new Vector2(frame2.Width * 0.5f, frame2.Height);
        Color fireDrawColor = Color.White;
        fireDrawColor.A = 0;
        spriteBatch.Draw(_furnaceBurningAmbientTextureAsset.Value, bottomMiddleDrawPosition, frame2, fireDrawColor, 0f, origin2, 1.75f, SpriteEffects.None, 0f);

        //Draw Furnace
        Vector2 furnaceOrigin = new Vector2(_furnaceTextureAsset.Width() * 0.5f, _furnaceTextureAsset.Height());
        spriteBatch.Draw(_furnaceTextureAsset.Value, bottomMiddleDrawPosition, null, Color.White, 0f, furnaceOrigin, 1f, SpriteEffects.None, 0f);

        //Draw Burning Furnace Animation
        Rectangle frame = _furnaceBurningTextureAsset.Value.GetFrame(_burningAnimationFrame.frame, 6, 10);
        Vector2 origin = new Vector2(frame.Width * 0.5f, frame.Height);
        spriteBatch.Draw(_furnaceBurningTextureAsset.Value, bottomMiddleDrawPosition - Vector2.UnitY * 48 + Vector2.UnitX * 16, frame, fireDrawColor, 0f, origin, 2f, SpriteEffects.None, 0f);
    }
}
