using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{
    internal class CollectionItemTabCraft : UIElement
    {
        internal Item Item;
        private readonly float _scale;
        private readonly int _context;
        internal CollectionItemTabCraft(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _scale = scale;
            _context = context;
            Item = new Item();
            Item.SetDefaults(0);

            var value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot",
                ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(value.Width() * scale, 0f);
            Height.Set(value.Height() * scale, 0f);
        }

        public float Glow { get; set; }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            Vector2 centerPos = pos + rectangle.Size() / 2f;

            var cauldronPlayer = Main.LocalPlayer.GetModPlayer<CauldronPlayer>();
            var cauldron = ModContent.GetInstance<Cauldron>();
            Color drawColor = Color.White;
            if (!cauldronPlayer.HasMadeItem(Item))
            {
                drawColor = Color.Black;
                if (contains)
                {
                    MoldTooltipItem t = ModContent.GetModItem(ModContent.ItemType<MoldTooltipItem>()) as MoldTooltipItem;
                    if (t.MoldNeeded == null)
                    {
                        t.MoldNeeded ??= new Item();
                        t.MoldNeeded.SetDefaults(0);
                    }

                    t.MoldNeeded = cauldron.FindMold(Item);
                    Main.hoverItemName = "Testing Testing 123";
                    Main.HoverItem = t.Item;
                }
            }
            else
            {
                if (contains)
                {
                    Main.hoverItemName = Item.Name;
                    Main.HoverItem = Item;
                }
            }

            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, drawColor);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            for (int i = 0; i < 8f; i++)
            {
                Color glowColor = Color.White * Glow;
                float progress = (float)i / 8f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2() * 8 * Glow;
                ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + offset, _scale, 32, glowColor);
            }

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            Main.inventoryScale = oldScale;
        }
    }
    internal class CollectionItemTabSlot : UIElement
    {
        internal Item Item;
        private readonly int _context;
        private readonly float _scale;
        internal CollectionItemTabSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            Item = new Item();
            Item.SetDefaults(0);

            var value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot",
                ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(value.Width() * scale, 0f);
            Height.Set(value.Height() * scale, 0f);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public float Glow { get; set; }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
            uiSystem.OpenRecipesInfoUI(Item);
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
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
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            Texture2D value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot").Value;
            Vector2 centerPos = pos + rectangle.Size() / 2f;

            spriteBatch.Draw(value, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, Color.White);


            if (contains)
            {
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item;
            }


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            for (int i = 0; i < 8f; i++)
            {
                Color glowColor = Color.White * Glow;
                float progress = (float)i / 8f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2() * 8 * Glow;
                ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + offset, _scale, 32, glowColor);
            }

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            Main.inventoryScale = oldScale;
        }
    }
    internal class CollectionItemRecipesUI : UIPanel
    {
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _slotGrid;
        private FancyScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 + 280;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;

        public CollectionItemRecipesUI() : base()
        {
            //Set to air
            Material = new Item();
            Material.SetDefaults(0);
            Glow = 1f;
        }

        public Item Material { get; set; }
        public float Glow { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 6f;
            Height.Pixels = 48 * 9;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _panel = new UIPanel();
            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _slotGrid = new UIGrid();
            _slotGrid.Width.Set(0, 1f);
            _slotGrid.Height.Set(0, 1f);
            _slotGrid.ListPadding = 2f;
            _panel.Append(_slotGrid);

            _scrollbar = new FancyScrollbar();
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.93f);
            _scrollbar.Top.Set(0, 0.05f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);


            _uiList = new UIList();
            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);
        }

        public override void Recalculate()
        {
            //Recalculate the UI when there is some sort of update
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _slotGrid?.Clear();
            if (Main.gameMenu)
                return;

            //We just need to get the number of unique materials since that's how we're sorting things

            var cauldron = ModContent.GetInstance<Cauldron>();
            Item[] crafts = cauldron.GetCraftsFromMaterial(Material.type);
            for (int i = 0; i < crafts.Length; i++)
            {
                Item craft = crafts[i];
                CollectionItemTabCraft slot = new CollectionItemTabCraft();
                slot.Item = craft;
                slot.Glow = Glow;
                _slotGrid.Add(slot);
            }

            _slotGrid.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            Glow *= 0.95f;

            _panel.Height.Pixels = _slotGrid.GetTotalHeight() + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);

            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0.05f, 0f);
            }
        }
    }
    internal class CollectionItemTabUI : UIPanel
    {
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _slotGrid;
        private FancyScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 - 64;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;
        public float Glow { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 6f;
            Height.Pixels = 48 * 9;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _panel = new UIPanel();
            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _slotGrid = new UIGrid();
            _slotGrid.Width.Set(0, 1f);
            _slotGrid.Height.Set(0, 1f);
            _slotGrid.ListPadding = 2f;

            _panel.Append(_slotGrid);

            _scrollbar = new FancyScrollbar();
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.9f);
            _scrollbar.Top.Set(0, 0.05f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);


            _uiList = new UIList();
            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);
            Glow = 1;
        }

        public override void Recalculate()
        {
            //Recalculate the UI when there is some sort of update
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _slotGrid?.Clear();
            if (Main.gameMenu)
                return;

            //We just need to get the number of unique materials since that's how we're sorting things

            var cauldron = ModContent.GetInstance<Cauldron>();
            Item[] materialsYouCanCraftWith = cauldron.GetMaterials();
            for (int i = 0; i < materialsYouCanCraftWith.Length; i++)
            {
                Item mat = materialsYouCanCraftWith[i];
                CollectionItemTabSlot slot = new CollectionItemTabSlot();
                slot.Item = mat;
                slot.Glow = Glow;
                _slotGrid.Add(slot);
            }

            _slotGrid.Recalculate();
            base.Recalculate();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            Glow *= 0.985f;

            _panel.Height.Pixels = _slotGrid.GetTotalHeight() + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);

            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0f);
            }
        }
    }
}
