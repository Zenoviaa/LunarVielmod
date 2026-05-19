using ReLogic.Content;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.ArmorReforgeSystem;

public class ReforgeSlot : UIElement
{
    private readonly Asset<Texture2D> _reforgeSlotTextureAsset;
    public Item Item;
    public int slotType;
    public ReforgeSlot()
    {
        _reforgeSlotTextureAsset = ModContent.Request<Texture2D>($"{ReforgeUISystem.RootTexturePath}ReforgeSlot", AssetRequestMode.AsyncLoad);
        Item = new Item();
        Item.SetDefaults(ItemID.None);


        Width.Set(52, 0f);
        Height.Set(52, 0f);
    }

    /// <summary>
    /// Returns true if this item can be placed into the slot (either empty or a pet item)
    /// </summary>
    public bool Valid(Item item)
    {
        bool rightItemType = false;
        switch (slotType)
        {
            default:
            case 0:
                rightItemType = item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0;
                break;
            case 1:
                rightItemType = item.accessory;
                break;
        }

        return rightItemType || item.IsAir;
    }

    public void HandleMouseItem()
    {
        if (Valid(Main.mouseItem))
        {
            if (Main.mouseLeftRelease && Main.mouseLeft)
            {
                ItemSlot.Handle(ref Item, ItemSlot.Context.InventoryItem);
            }
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Rectangle rectangle = GetDimensions().ToRectangle();
        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
          
            Main.LocalPlayer.mouseInterface = true;
            HandleMouseItem();
            Main.HoverItem = Item;
            Main.hoverItemName = Item.HoverName;


            if (Item.IsAir)
            {
                List<TooltipLine> tooltipLines = new List<TooltipLine>();
                switch (slotType)
                {
                    case 0:
                        {
                            TooltipLine helpLine = new TooltipLine(Stellamod.Instance, "SlotHelp", LangText.Common("ArmorSlotHelp"));
                            helpLine.OverrideColor = Color.Goldenrod;
                            tooltipLines.Add(helpLine);

                            ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
                            renderer.SetTooltipsToDraw(tooltipLines, 64, 16);
                        }

                        break;
                    case 1:
                        {
                            TooltipLine helpLine = new TooltipLine(Stellamod.Instance, "SlotHelp", LangText.Common("AccessorySlotHelp"));
                            helpLine.OverrideColor = Color.Goldenrod;
                            tooltipLines.Add(helpLine);

                            ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
                            renderer.SetTooltipsToDraw(tooltipLines, 64, 16);
                        }
                        break;
                }
            }

        }

        //Draw Backing
        Color color2 = Main.inventoryBack;
        Vector2 pos = rectangle.TopLeft();

        int frameNumber = 0;
        if (slotType == 1)
            frameNumber = 1;

        Rectangle frame = _reforgeSlotTextureAsset.Value.GetFrame(frameNumber, 2);
        Vector2 centerPos = pos + rectangle.Size() / 2f;
        spriteBatch.Draw(_reforgeSlotTextureAsset.Value, rectangle.TopLeft(), frame, color2, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

        ItemSlot.DrawItemIcon(Item, ItemSlot.Context.InventoryItem, spriteBatch, centerPos + new Vector2(0, 3), 1f, 32, Color.White);
        if (Item.stack > 1)
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Item.stack.ToString(),
                centerPos + new Vector2(10f, 26f), Color.White, 0f, Vector2.Zero, new Vector2(1f), -1f, 1f);
    }
}
