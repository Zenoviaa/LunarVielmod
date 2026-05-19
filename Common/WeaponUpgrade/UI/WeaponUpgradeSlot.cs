using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Stellamod.Assets;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
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


        Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Color glowColor2 = Color.Lerp(Color.Black, Color.Red, ExtraMath.Osc(0.5f, 1f, speed: 2));
        glowColor2.A = 0;
        spriteBatch.Draw(glowTexture, centerPos, null, glowColor2, 0f, glowTexture.Size() * 0.5f, _scale * 0.2f, SpriteEffects.None, 0f);


        Color glowColor = Color.Lerp(Color.Black, Color.Red, WeaponUpgradeUISystem.ForgeGlow);
        glowColor.A = 0;
        spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, glowColor, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);


        float scale = 1.25f;
        Vector2 drawPos = centerPos + new Vector2(0, 3);
        drawPos.Y -= MathHelper.Lerp(0, 10, WeaponUpgradeUISystem.ForgeGlow);
        ItemSlot.DrawItemIcon(Item, _context, spriteBatch, drawPos, _scale * scale, 32 * scale, Color.White);


        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null,
              Main.UIScaleMatrix);

        if (Item.stack > 1)
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Item.stack.ToString(),
                centerPos + new Vector2(10f, 26f) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);

        if(Item.TryGetGlobalItem<WeaponUpgradeGlobalItem>(out var weaponUpgradeGlobalItem))
        {
            string weaponUpgrade = $"+{weaponUpgradeGlobalItem.weaponLevel}";
            Color levelColor = Color.Lerp(Color.White, Color.Red, WeaponUpgradeUISystem.ForgeGlow);
            float s = MathHelper.Lerp(_scale, _scale * 1.5f, WeaponUpgradeUISystem.ForgeGlow);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, weaponUpgrade,
                 centerPos + new Vector2(10f, 26f) * _scale, levelColor, 0f, Vector2.Zero, Vector2.One * s, -1f, _scale);


            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            Vector2 position = topLeft;
            position.X += 196;
            position.Y += ExtraMath.Osc(0f, 4f, speed: 2);
            position.Y += Height.Pixels + 32;

            int width = (int)Width.Pixels;
            width *= 2;
            Rectangle backgroundRect = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, width, (int)32);
            Utils.DrawInvBG(spriteBatch, backgroundRect, new Color(23, 25, 81, 255) * 0.925f);


            float pct1 = weaponUpgradeGlobalItem.weaponLevel * 0.25f * 100;
            float pct2 = (weaponUpgradeGlobalItem.weaponLevel+1) * 0.25f * 100;

            string firstLevelPercent = pct1.ToString("0.00");
            string secondLevelPercent = pct2.ToString("0.00");
            string nextText = LangText.Common("ForgeChange", $"+{pct1}%", $"+{pct2}%");
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, nextText,
                backgroundRect.TopLeft() + new Vector2(4, 12), levelColor, 0f, Vector2.Zero, Vector2.One * s, -1f, _scale);


        }
        DrawHelp(spriteBatch);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
              Main.UIScaleMatrix);

        //   WeaponUpgradeGlobalItem weaponUpgradeGlobalItem = Item.GetGlobalItem<WeaponUpgradeGlobalItem>();

        Main.inventoryScale = oldScale;
    }

    private void DrawHelp(SpriteBatch spriteBatch)
    {

        Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
        Vector2 position = topLeft;
        position.X += 196;
        position.Y += ExtraMath.Osc(0f, 4f, speed: 2);

        int width = (int)Width.Pixels;
        width *= 2;
        Rectangle backgroundRect = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, width, (int)Height.Pixels);
        Utils.DrawInvBG(spriteBatch, backgroundRect, new Color(23, 25, 81, 255) * 0.925f);

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, LangText.Common("ForgeHelp"),
            backgroundRect.TopLeft() + new Vector2(4), Color.White, 0f, Vector2.Zero, new Vector2(_scale), backgroundRect.Width, _scale);
    }
}
