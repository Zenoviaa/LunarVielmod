using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.UI;

public class CommonBackButton : UIPanel
{
    private readonly Asset<Texture2D> _commonXBigTextureAsset;

    private readonly Asset<Texture2D> _commonXTextureAsset;
    private float _scale;
    private Action _closeFunction;
    private UIText _backText;
    public CommonBackButton(Action closeFunction, string titleKey = null) : base()
    {
        alpha = 1f;
        _commonXBigTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/CommonXBig");
        _commonXTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/CommonX");
        _closeFunction = closeFunction;
        _backText = new UIText("Back", large: true);
        if (!string.IsNullOrEmpty(titleKey))
        {
            this.titleKey = titleKey;
        }
        else
        {
            this.titleKey = "Back";
        }
    }

    public bool asXButton;
    public bool axXBigButton;
    public float alpha;
    public string titleKey;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 160;
        Height.Pixels = 54;
    
        _backText.Width.Pixels = Width.Pixels;
        _backText.Height.Pixels = Height.Pixels;
        _backText.HAlign = 0.5f;
        _backText.SetText(LangText.Common(titleKey));
        BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0.5f;
        BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0.5f;
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


    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0.5f * alpha;
        BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0.5f * alpha;
        _backText.TextColor = Color.White * alpha;
        _backText.ShadowColor = Color.Black * alpha;
        _backText.SetText(LangText.Common(titleKey), _scale, true);
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();

        if (asXButton)
        {
            Width.Pixels = 24;
            Height.Pixels = 24;
            if (axXBigButton)
            {
                Width.Pixels = 54;
                Height.Pixels = 54;
                //BackgroundColor = Color.Red;
            }
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            _backText.ShadowColor = Color.Transparent;
            _backText.TextColor = Color.Transparent;

            Asset<Texture2D> textureAsset = _commonXTextureAsset;
            if (axXBigButton)
                textureAsset = _commonXBigTextureAsset;

            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, Main.screenPosition + GetDimensions().ToRectangle().TopLeft());
            drawer.drawOrigin = Vector2.Zero;
            int frame = IsMouseHovering ? 1 : 0;
            drawer.VerticalFrame(frame, 2);
            drawer.color = Color.White * alpha;
            spriteBatch.Draw(drawer);
        }
        if (IsMouseHovering)
        {
            _scale = MathHelper.Lerp(_scale, 1.25f, 0.3f);
        }
        else
        {
            _scale = MathHelper.Lerp(_scale, 1f, 0.3f);
        }


    }
}
