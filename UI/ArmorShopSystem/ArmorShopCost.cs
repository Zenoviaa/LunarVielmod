using Stellamod.Common.ArmorShop;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.UI.ArmorShopSystem;

public class ArmorShopCost : UIElement
{
    private readonly int _context;
    private readonly float _scale;

    public Item Item;
    public ArmorShopCost(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _context = context;
        _scale = scale;
        Item = new Item();
        Item.SetDefaults(ItemID.None);

        var asset = ModContent.Request<Texture2D>(
            $"{ArmorShopUISystem.RootTexturePath}ArmorShopSlot", ReLogic.Content.AssetRequestMode.ImmediateLoad);

        Width.Set(asset.Width() * scale, 0f);
        Height.Set(asset.Height() * scale, 0f);
    }

    public ArmorShopSet armorSet;
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();
        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains)
        {
            Main.HoverItem = Item;
            Main.hoverItemName = Item.Name;
        }

        //Draw Backing
        Color color2 = Main.inventoryBack;
        Vector2 pos = rectangle.TopLeft();
        Vector2 centerPos = pos + rectangle.Size() / 2f;

        ItemSlot.DrawItemIcon(Item, _context, spriteBatch, pos, _scale, 32, Color.White);
        if (Item.stack > 1 && !armorSet.HasPurchased())
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Item.stack.ToString(),
                pos + new Vector2(8, 0) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
        Main.inventoryScale = oldScale;
    }
}
