using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Stellamod.Common.UI;

public class CommonBackButton : UIPanel
{
    private float _scale;
    private Action _closeFunction;
    private UIText _backText;
    public CommonBackButton(Action closeFunction) : base()
    {
        _closeFunction = closeFunction;
        _backText = new UIText("Back", large: true);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 160;
        Height.Pixels = 54;

        Append(_backText);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        _closeFunction();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        _backText.Width.Pixels = Width.Pixels;
        _backText.Height.Pixels = Height.Pixels;
        _backText.HAlign = 0.5f;
        _backText.SetText(LangText.Common("Back"));
        BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0.5f;
        BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0.5f;
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
        if (IsMouseHovering)
        {
            _scale = MathHelper.Lerp(_scale, 1.25f, 0.3f);
        }
        else
        {
            _scale = MathHelper.Lerp(_scale, 1f, 0.3f);
        }
        _backText.SetText(LangText.Common("Back"), _scale, true);
    }
}
