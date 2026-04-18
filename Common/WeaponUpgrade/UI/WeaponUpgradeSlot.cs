using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.WeaponUpgrade.UI;

public class WeaponUpgradeSlot : UIElement
{
    private readonly int _context;
    private readonly float _scale;

    public Item Item;
    public WeaponUpgradeSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _context = context;
        _scale = scale;
        Item = new Item();
        Item.SetDefaults(ItemID.None);

        var asset = ModContent.Request<Texture2D>(
            $"{WeaponUpgradeUISystem.RootTexturePath}UpgradeSlot", ReLogic.Content.AssetRequestMode.ImmediateLoad);

        Width.Set(asset.Width() * scale, 0f);
        Height.Set(asset.Height() * scale, 0f);
    }

    /// <summary>
    /// Returns true if this item can be placed into the slot (either empty or a pet item)
    /// </summary>
    public bool Valid(Item item)
    {
        return true;
    }

    public void HandleMouseItem()
    {
        if (Valid(Main.mouseItem))
        {
            ItemSlot.Handle(ref Item, _context);
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();
        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
            HandleMouseItem();
        }

        //Draw Backing
        Color color2 = Main.inventoryBack;
        Vector2 pos = rectangle.TopLeft();

        Texture2D backingTexture = ModContent.Request<Texture2D>($"{WeaponUpgradeUISystem.RootTexturePath}UpgradeSlot").Value;

        Vector2 centerPos = pos + rectangle.Size() / 2f;
        spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

        float scale = 1.25f;
        Vector2 drawPos = centerPos + new Vector2(0, 3);

        /*
        for (float f = 0; f <= MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            Vector2 offset = f.ToRotationVector2();
            offset *= _scale * scale;
            Vector2 outlineDrawPos = drawPos + offset;
            Color outlineColor = Color.White;
            outlineColor.A = 0;
            for(int i = 0; i < 2; i++)
                ItemSlot.DrawItemIcon(Item, _context, spriteBatch, outlineDrawPos, _scale * scale, 32 * scale, outlineColor);
        }

        */
        ItemSlot.DrawItemIcon(Item, _context, spriteBatch, drawPos, _scale * scale, 32 * scale, Color.White);


        if (Item.stack > 1)
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Item.stack.ToString(),
                centerPos + new Vector2(10f, 26f) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);

        Main.inventoryScale = oldScale;
    }
}
