using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace Stellamod.Common.DevTools;

public class DevToolHotbarButton : UIPanel
{
    public DevToolHotbarButton(
        Asset<Texture2D> buttonIconTexture,
        Func<(int frame, int frameCount)> getFrameFunc,
        Action toggleFunction,
        Func<Color> getDrawColor = null)
    {
        ButtonIconTexture = buttonIconTexture;
        GetFrameFunction = getFrameFunc;
        ToggleFunction = toggleFunction;
        GetDrawColorFunction = getDrawColor;
    }
    public readonly Asset<Texture2D> ButtonIconTexture;
    public readonly Func<(int frame, int frameCount)> GetFrameFunction;
    public readonly Action ToggleFunction;
    public readonly Func<Color> GetDrawColorFunction;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 32;
        Height.Pixels = 32;
        OnLeftClick += Toggle;
    }

    private void Toggle(UIMouseEvent evt, UIElement listeningElement)
    {
        ToggleFunction();
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        this.QuickMouseInteraction();
        var rect = GetDimensions().ToRectangle();
        var drawer = SpritebatchDrawer.FromTextureAsset(ButtonIconTexture, Vector2.Zero);
        drawer.color = Color.White;
        if (GetDrawColorFunction != null)
            drawer.color = GetDrawColorFunction();
        var framing = GetFrameFunction();
        if (framing.frame == 0)
            drawer.color = Color.Lerp(drawer.color, Color.Black, 0.5f);
        drawer.VerticalFrame(framing.frame, framing.frameCount);
        drawer.CenterOrigin();

        drawer.worldPosition = rect.Center() + Main.screenPosition;
        spriteBatch.Draw(drawer);
    }
}
