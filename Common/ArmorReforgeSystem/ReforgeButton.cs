using ReLogic.Content;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.ArmorReforgeSystem;

public class ReforgeButton : UIPanel
{
    private readonly Asset<Texture2D> _buttonTextureAsset;
    private ReforgeUISystem ReforgeSystem => ModContent.GetInstance<ReforgeUISystem>();
    public ReforgeButton()
    {
        _buttonTextureAsset = ModContent.Request<Texture2D>($"{ReforgeUISystem.RootTexturePath}ReforgeButton", AssetRequestMode.ImmediateLoad);
        Width.Set(48, 0f);
        Height.Set(32, 0f);
        OnLeftClick += OnButtonClick;
    }

    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        if (ReforgeSystem.CanReforge())
        {
            ReforgeSystem.Reforge();
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        this.QuickMouseInteraction();
        Color drawColor = Color.White;
        //Grey out when crafting won't make anything
        if (!ReforgeSystem.CanReforge())
            drawColor = drawColor.MultiplyRGB(Color.Gray);

        Rectangle frame = _buttonTextureAsset.Value.GetFrame(IsMouseHovering ? 1 : 0, 2);
        Rectangle rect = GetDimensions().ToRectangle();
        rect.Location += new Point(0, (int)VectorHelper.Osc(-8f, 8f, 1f));
        float rotation = 0;
        spriteBatch.Draw(_buttonTextureAsset.Value, rect, frame, drawColor, rotation, Vector2.Zero, SpriteEffects.None, 0);
    }
}