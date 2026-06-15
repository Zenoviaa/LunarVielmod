using ReLogic.Content;
using Stellamod.Common.ClassReworkSystem.AmmoRework;
using Stellamod.Common.XixianFlaskSystem.UI;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

public class AmmoToolSlot : UIElement
{
    private readonly int _context;
    private readonly float _scale;
    private Item _item;
    private Asset<Texture2D> _slotTextureAsset;
    public AmmoToolSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _context = context;
        _scale = scale;

        _slotTextureAsset = ModContent.Request<Texture2D>(
            $"{XixianFlaskUISystem.RootTexturePath}FlaskSlot", AssetRequestMode.ImmediateLoad);

        Width.Set(_slotTextureAsset.Width() * scale, 0f);
        Height.Set(_slotTextureAsset.Height() * scale, 0f);
        OnLeftClick += OpenUI;
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

    }

    private void OpenUI(UIMouseEvent evt, UIElement listeningElement)
    {
        AmmoToolUISystem uiSystem = ModContent.GetInstance<AmmoToolUISystem>();
        uiSystem.ToggleUI();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        _item ??= ModContent.GetModItem(ModContent.ItemType<ElementalQuiver>()).Item.Clone();
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();
        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.hoverItemName = _item.HoverName;
            Main.HoverItem = _item;
        }

        //Draw Backing
        Color color2 = Main.inventoryBack;
        Vector2 pos = rectangle.TopLeft();

        Texture2D backingTexture = _slotTextureAsset.Value;
        int offset = (int)(backingTexture.Size().Y / 2);
        Vector2 centerPos = pos + rectangle.Size() / 2f;
        spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);

        ItemSlot.DrawItemIcon(_item, _context, spriteBatch, centerPos, _scale, 32, Color.White);


        Main.inventoryScale = oldScale;
    }
}
