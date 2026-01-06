using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.ZTileSystem.UI;

/// <summary>
/// Setups a view that lets you look over a massive grid of items
/// </summary>
public class ZTileBrowserView : UIPanel
{
    private float _scale;
    private int _context;
    private bool _oldModFilter;
    private string _oldSearchFilter;
    //Basically, instead of ceratgin 6800 slots or whatever
    //We have a single view that takes an array of items
    //Uses that to calculate draw offsets for each item and draws them
    public ZTileBrowserView(ZTile[] items)
    {
        _scale = 1f;
        _context = ItemSlot.Context.BankItem;
        ElementsPerRow = 9;

        //Set up the items we're going to iterate over
        Items = items;
        OnLeftClick += SpawnItem;

        //Setup drawing
        string texturePath = this.GetType().DirectoryHere() + "/TileBrowserSlot";
        SlotTextureAsset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad);
        Width.Set(32, 0f);
        Height.Set(32, 0f);
    }

    public ZTile[] Items;
    public ZTile[] SearchFilterItems;
    public ZTile HoveringItem;
    public Asset<Texture2D> SlotTextureAsset;
    public string SearchFilter;
    public float ViewPosition;
    public int ElementsPerRow;

    private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
    {
        if (HoveringItem == null)
            return;
        ZTileLoader tileLoader = ModContent.GetInstance<ZTileLoader>();
        DecorationBuilder.templateData = tileLoader.InstanceTileData(HoveringItem);
        DecorationBuilder.frame = 0;
    }

    private bool NeedsUpdateCollection()
    {
        return _oldSearchFilter != SearchFilter;
    }

    private void UpdateCollection()
    {
        IEnumerable<ZTile> collection = Items;
        string filter = string.Empty;
        if (!string.IsNullOrEmpty(SearchFilter))
        {
            filter = SearchFilter.TrimStart().ToLower();
            collection = collection.Where(x => x.Name.ToLower().Contains(filter));
        }

        SearchFilterItems = collection.ToArray();
        _oldSearchFilter = SearchFilter;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();
        if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        if (IsMouseHovering)
        {
          //  Main.HoverItem = HoveringItem;
           // Main.hoverItemName = HoveringItem.HoverName;
        }

        Vector2 topLeft = rectangle.TopLeft();
        float availableWidth = GetInnerDimensions().Width;
        float listPadding = 10;
        Rectangle outerDimensions = new Rectangle(0, 0, 32, 32);
        Point mousePoint = Main.MouseScreen.ToPoint();
        string filter = string.Empty;
        if (!string.IsNullOrEmpty(SearchFilter))
            filter = SearchFilter.TrimStart().ToLower();
        bool useFilter = !string.IsNullOrEmpty(filter);

        //We're basically just reusing the grid code here lol
        //There's currently 9 items per row
        //To optimize this, we can calculate the placement of an element with some simple math based on its index
        //Instead of using left and top variables
        //So let's do that


        //We only want to draw the items that are actually in view
        //So we should calculate a starting inde


        //Define our width variables

        if (NeedsUpdateCollection())
        {
            UpdateCollection();
        }
        ZTile[] itemArr = SearchFilterItems;
        int elementsPerRow = ElementsPerRow;
        float elementWidth = outerDimensions.Width;
        float viewWidth = availableWidth;
        float elementHeight = outerDimensions.Height;

        //Calculate the maximum height of the grid
        int itemRows = (itemArr.Length / elementsPerRow);
        float maximumHeight = itemRows * (elementHeight + listPadding);
        Height.Pixels = maximumHeight + 32;


        Texture2D slotTexture = SlotTextureAsset.Value;
        Color drawColor = Color.Lerp(Color.White, Color.Black, 0.75f);
        Vector2 drawOrigin = slotTexture.Size() / 2;

        //The view position is the y offset of the scrollbar
        //So to figure out where to start from
        //We just divide the offset by 
        //Caculate a starting and ending index for which items to draw
        int numRowsDownward = (int)(ViewPosition / (elementHeight + listPadding));
        int startIndex = numRowsDownward * elementsPerRow;
        int endIndex = startIndex + elementsPerRow * 6;


        //Now we're only loading the items that are in view! Yippee! Optimization!
        for (int i = startIndex; i < endIndex && i < itemArr.Length; i++)
        {
            ZTile item = itemArr[i];

            //Remmeber 9 elements per row
            //We can use the modulus operator to get this to keep looping, since all elements are the same size
            float leftOffset = i % elementsPerRow * (elementWidth + listPadding);
            float topOffset = i / elementsPerRow * (elementHeight + listPadding);

            //Enchantment Card
            Vector2 tl = topLeft;
            tl.X += leftOffset;
            tl.Y += topOffset;
            Vector2 centerPos = tl + new Vector2(16);


            //Check if hovering for tooltip
            bool isHovering = false;
            Rectangle hoverRectangle = new Rectangle((int)tl.X, (int)tl.Y, 32, 32);
            if (hoverRectangle.Contains(mousePoint))
            {
                isHovering = true;
                HoveringItem = item;
                List<TooltipLine> tooltipLines = new List<TooltipLine>();
                TooltipLine helpLine = new TooltipLine(Stellamod.Instance, "ZTileName", item.Name);
                helpLine.OverrideColor = Color.Goldenrod;
                tooltipLines.Add(helpLine);


                ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
                renderer.SetTooltipsToDraw(tooltipLines, 64, 16);

            }

            Vector2 iconCenterPos = tl + slotTexture.Size() / 2;
            spriteBatch.Draw(slotTexture, iconCenterPos, null, drawColor, 0f, drawOrigin, _scale, SpriteEffects.None, 0f);

            float maxSize = 32;
            if (isHovering)
                maxSize *= 1.5f;
            item.DrawIcon(spriteBatch, iconCenterPos, maxSize);


        }

        Main.inventoryScale = oldScale;
    }
}