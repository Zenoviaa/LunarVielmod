using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.BackpackSystem
{
    public class BackpackMenu : UIPanel
    {
        private UIGrid _grid;
        private UIPanel _panel;
        public BackpackMenu()
        {
            _panel = new UIPanel();
            _grid = new UIGrid();
        }

        public int RelativeLeft => 555;
        public int RelativeTop => 100;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 200;
            Height.Pixels = 256;

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _grid.Left.Pixels = 10;
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.VAlign = 0.5f;
            _grid.ListPadding = 2;
            _panel.Append(_grid);
        }

        public void SetBackpack()
        {
            _grid.Clear();
            var player = Main.LocalPlayer.GetModPlayer<BackpackPlayer>();
            for (int i = 0; i < player.MaxCapacity; i++)
            {
                BackpackSlot slot = new BackpackSlot(i);
                _grid.Add(slot);
            }
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            _grid.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }
    }
    public class BackpackInventoryUIState : UIState
    {
        public BackpackMenu backpackMenu;

        public BackpackInventoryUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            backpackMenu = new BackpackMenu();
            Append(backpackMenu);
        }

        public override void Recalculate()
        {
            base.Recalculate();
            if (backpackMenu == null)
                return;

            //Resize the main panels height based on resolution
            //Recalculate size of the UI based on the resolution, so it's dynamic
            const float size = 706;
            float height = Main.graphics.GraphicsDevice.Viewport.Height;
            float subHeight = height - 32;
            float targetSize = Math.Min(subHeight, size);
            backpackMenu.Height.Pixels = targetSize;
            backpackMenu.Width.Pixels = targetSize;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
 
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class BackpackUISystem : ModSystem
    {
        private UserInterface _backpackInterface;
        private GameTime _lastUpdateUiGameTime;
        private BackpackInventoryUIState _backpackInventory;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _backpackInterface = new UserInterface();
            _backpackInventory = new BackpackInventoryUIState();
            _backpackInventory.Activate();
        }

        private void OpenUI()
        {
            _backpackInterface.SetState(_backpackInventory);
        }
        private void CloseUI()
        {
            _backpackInterface.SetState(null);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if(Main.playerInventory && Main.LocalPlayer.GetModPlayer<BackpackPlayer>().hasBackpack && _backpackInterface?.CurrentState == null)
            {
                _backpackInventory.backpackMenu.SetBackpack();
                OpenUI();
            }
            if (_backpackInterface?.CurrentState != null)
            {
                if(!Main.playerInventory || !Main.LocalPlayer.GetModPlayer<BackpackPlayer>().hasBackpack)
                    CloseUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_backpackInterface?.CurrentState != null)
            {
                _backpackInterface.Update(gameTime);
            }
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_backpackInterface.CurrentState != null)
            {
                //   RenamePetUI.saveItemInUI = true;
                _backpackInterface.SetState(null);
            }
        }


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Magic UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _backpackInterface?.CurrentState != null)
                        {
                            _backpackInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

    }
    /// <summary>
    /// Represents a slot in the backpack
    /// </summary>
    public class BackpackSlot : UIElement
    {
        private readonly int _index;
        private readonly bool _isTimedSlot;
        private readonly int _context;
        private readonly float _scale;
        public Item Item;
        public Asset<Texture2D> SlotTextureAsset;
        public BackpackSlot(int index, int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            _index = index;

            Item = new Item();
            Item.SetDefaults(0);
            BackpackPlayer backpackPlayer = Main.LocalPlayer.GetModPlayer<BackpackPlayer>();
            Item = backpackPlayer.GetItem(_index);

            string texturePath = this.GetType().DirectoryHere() + "/BackpackSlot";
            SlotTextureAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(SlotTextureAsset.Width() * scale, 0f);
            Height.Set(SlotTextureAsset.Height() * scale, 0f);
        }


        public bool IsHidden()
        {
            var player = Main.LocalPlayer.GetModPlayer<ArmorStatsPlayer>();
            if (player.inventorySlots <= _index)
                return true;
            return false;
        }
        public override int CompareTo(object obj)
        {
            if(obj is BackpackSlot slot)
            {
                return _index.CompareTo(slot._index);
            }
            return base.CompareTo(obj);
        }
        public void HandleMouseItem()
        {
            ItemSlot.Handle(ref Item, _context);

            //Save Item 
            if (Main.mouseLeftRelease && Main.mouseLeft)
            {
                BackpackPlayer backpackPlayer = Main.LocalPlayer.GetModPlayer<BackpackPlayer>();
                backpackPlayer.SetItem(_index, Item);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsHidden())
                return;

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                HandleMouseItem();
            }

            Vector2 pos = rectangle.TopLeft();

            //Enchantment Card
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            Color color2 = Main.inventoryBack;
            Texture2D slotTexture = SlotTextureAsset.Value;
            Vector2 drawOrigin = slotTexture.Size() / 2;
            Vector2 iconCenterPos = rectangle.TopLeft() + slotTexture.Size() / 2;
            spriteBatch.Draw(slotTexture, iconCenterPos, null, color2, 0f, drawOrigin, _scale, SpriteEffects.None, 0f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale , 32, Color.White);
            if (Item.stack > 1)
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Item.stack.ToString(),
                    centerPos + new Vector2(0, 2) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
            Main.inventoryScale = oldScale;
        }
    }
    public class BackpackPlayer : ModPlayer
    {
        private List<Item> _backpackItems;
        public List<Item> BackpackItems
        {
            get
            {
                if (_backpackItems == null)
                {
                    _backpackItems = new List<Item>();
                }
                while(_backpackItems.Count < MaxCapacity)
                {
                    Item item = new Item();
                    item.SetDefaults(0);
                    _backpackItems.Add(item);
                }
              
                return _backpackItems;
            }
            set
            {
                _backpackItems = value;
            }
        }
        public override void Load()
        {
            base.Load();
            MaxCapacity = 0;
        }

        public bool hasBackpack;
        public override void ResetEffects()
        {
            base.ResetEffects();
            MaxCapacity = 0;// + Player.GetModPlayer<ArmorStatsPlayer>().inventorySlots;
            hasBackpack = false;

        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            ArmorStatsPlayer statsPlayer = Player.GetModPlayer<ArmorStatsPlayer>();
            MaxCapacity += statsPlayer.inventorySlots;
            if (statsPlayer.inventorySlots > 0)
            {
                hasBackpack = true;
            }
        }


        public int MaxCapacity;
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["backpackitems"] = BackpackItems;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            BackpackItems.Clear();
            BackpackItems = tag.Get<List<Item>>("backpackitems");
        }
        public Item[] GetBackpackArray()
        {
            return BackpackItems.ToArray();
        }
        public void SetItem(int index, Item item)
        {
            BackpackItems[index] = item;
        }
        public Item GetItem(int index)
        {
            return BackpackItems[index];
        }
    }
}
