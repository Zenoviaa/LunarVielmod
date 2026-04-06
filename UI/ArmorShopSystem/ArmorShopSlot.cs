using Stellamod.Common.ArmorShop;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.ArmorShopSystem;

public class ArmorShopSlot : UIElement
{
    private readonly int _context;
    private readonly float _scale;
    public Item Item;
    public ArmorShopSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
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

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();
        bool contains = ContainsPoint(Main.MouseScreen);


        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.HoverItem = Item;
            Main.hoverItemName = Item.Name;
        }

        //Draw Backing
        Color color2 = Main.inventoryBack;
        Vector2 pos = rectangle.TopLeft();

        Texture2D backingTexture = ModContent.Request<Texture2D>($"{ArmorShopUISystem.RootTexturePath}ArmorShopSlot").Value;
        Vector2 centerPos = pos + backingTexture.Size() / 2f;
        spriteBatch.Draw(backingTexture, pos, null, color2, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);



        spriteBatch.End();
        spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);
        for (int i = 0; i < 4; i++)
        {
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(0, 3) + new Vector2(-2, 0), _scale, 32, Color.White);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(0, 3) + new Vector2(2, 0), _scale, 32, Color.White);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(0, 3) + new Vector2(0, -2), _scale, 32, Color.White);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(0, 3) + new Vector2(0, 2), _scale, 32, Color.White);
        }

        spriteBatch.End();
        spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

        ArmorShopPlayer shopPlayer = Main.LocalPlayer.GetModPlayer<ArmorShopPlayer>();
        if (shopPlayer.HasPurchased(Item.type))
        {
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(0, 3), _scale, 32, Color.White);
        }
        else
        {
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(0, 3), _scale, 32, Color.Black);
        }




        Main.inventoryScale = oldScale;
    }
}
