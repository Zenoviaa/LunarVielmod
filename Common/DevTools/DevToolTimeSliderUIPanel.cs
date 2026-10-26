using Stellamod.Core.LunarLightingSystem;
using Stellamod.WorldG;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Stellamod.Common.DevTools;

public class DevToolTimeSliderUIPanel : UIPanel
{
    bool _setting;
    bool _needsToSet;
    double _timeToSetTo;
    bool _dayToSetTo;
    Texture2D _dayTexture;
    Texture2D _nightTexture;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 264;
        Height.Pixels = 16;
        OnLeftMouseDown += SettingTime;
        OnLeftMouseUp += FinishSettingTime;
        Main.QueueMainThreadAction(CreateGradientTexture);
    }

    private void CreateGradientTexture()
    {
        var halfWidth = Width.Pixels / 2f;
        _dayTexture = new Texture2D(Main.graphics.GraphicsDevice, (int)halfWidth, (int)Height.Pixels);
        Color[] pixels = new Color[_dayTexture.Width * _dayTexture.Height];
        for (int y = 0; y < _dayTexture.Width; y++)
        {
            float ratio = (float)y / (float)_dayTexture.Width;
            for (int x = 0; x < _dayTexture.Height; x++)
            {
                int pixelIndex = TextureUtilities.GetPixelIndex(_dayTexture.Width, y, x);
                pixels[pixelIndex] = LunarLightingRenderer.GetSunColor(ratio);
            }
        }
        _dayTexture.SetData(pixels);
        _nightTexture = new Texture2D(Main.graphics.GraphicsDevice, (int)halfWidth, (int)Height.Pixels);
        for (int y = 0; y < _nightTexture.Width; y++)
        {
            float ratio = (float)y / (float)_nightTexture.Width;
            for (int x = 0; x < _nightTexture.Height; x++)
            {
                int pixelIndex = TextureUtilities.GetPixelIndex(_nightTexture.Width, y, x);
                pixels[pixelIndex] = LunarLightingRenderer.GetSunColor(1f);
            }
        }
        _nightTexture.SetData(pixels);
    }

    private void SettingTime(UIMouseEvent evt, UIElement listeningElement)
    {
        _setting = true;
    }

    private void FinishSettingTime(UIMouseEvent evt, UIElement listeningElement)
    {
        _setting = false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
        var rect = GetDimensions().ToRectangle();
        if (_setting)
        {
            var topLeft = rect.TopLeft();
            var xPos = Main.MouseScreen.X;
            var diff = Main.MouseScreen.X - topLeft.X;
            var midPoint = topLeft;
            var halfWidth = Width.Pixels * 0.5f;
            midPoint.X += halfWidth;

            var ratio = 0F;
            if (xPos < midPoint.X)
            {
                ratio = (Main.MouseScreen.X - topLeft.X) / halfWidth;
                ratio = MathHelper.Clamp(ratio, 0f, 1f);
                _timeToSetTo = ratio * Main.dayLength;
                _dayToSetTo = true;
            }
            if(xPos > midPoint.X)
            {
                ratio = (Main.MouseScreen.X - midPoint.X) / halfWidth;
                ratio = MathHelper.Clamp(ratio, 0f, 1f);
                _timeToSetTo = ratio * Main.nightLength;
                _dayToSetTo = false;

            }

            WorldDevTools.SetTimeNoSync(_timeToSetTo, _dayToSetTo);
            _needsToSet = true;
        }

        if (_needsToSet && !_setting)
        {
            WorldDevTools.SetTime(_timeToSetTo, _dayToSetTo);
            _needsToSet = false;
        }

        var t = (float)Main.time;
        var offset = 0F;
        if (Main.dayTime)
        {
            t /= (float)Main.dayLength;
        }
        if (!Main.dayTime)
        {
            t /= (float)Main.nightLength;
            offset = Width.Pixels * 0.5f;
        }

        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        var drawer = SpritebatchDrawer.FromTextureAsset(_dayTexture, rect.TopLeft() + Main.screenPosition);
        drawer.color = Color.White;
        drawer.drawOrigin = Vector2.Zero;

        spriteBatch.Draw(drawer);

        var drawer2 = SpritebatchDrawer.FromTextureAsset(_nightTexture, rect.TopLeft() + Main.screenPosition + new Vector2(_dayTexture.Width, 0));
        drawer2.color = Color.White;
        drawer2.drawOrigin = Vector2.Zero;

        spriteBatch.Draw(drawer2);


        var pointToPlaceAt = rect.TopLeft() + new Vector2(t * Width.Pixels * 0.5f + offset, 0);
        var iconDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.UI.FancyScrollbarInner.Asset, pointToPlaceAt + Main.screenPosition);
        iconDrawer.drawOrigin = Vector2.Zero;
        iconDrawer.color = Color.White;
        spriteBatch.Draw(iconDrawer);

    }
}
