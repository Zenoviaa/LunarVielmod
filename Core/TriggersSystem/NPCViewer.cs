using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.StructureSelector;
using Stellamod.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.Core.TriggersSystem
{
    public class NPCViewerSlot : UIPanel
    {
        private readonly ModNPC _npc;
        private readonly Action<ModNPC> _action;
        public NPCViewerSlot(ModNPC npc, Action<ModNPC> action)
        {
            _npc = npc;
            _action = action;
            Width.Pixels = 64;
            Height.Pixels = 64;
            BackgroundColor = Color.Blue * 0.5f;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.NewText(_npc.Name);
                Main.LocalPlayer.mouseInterface = true;
            }
            if (IsMouseHovering)
            {
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Yellow * 0.5f, 0.1f);
            }
            else
            {
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Blue * 0.5f, 0.1f);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (_npc == null)
                return;
            Texture2D texture = ModContent.Request<Texture2D>(_npc.Texture).Value;
            int frameCount = Main.npcFrameCount[_npc.Type];
            int frameHeight = texture.Height / frameCount;
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, frameHeight);

            Rectangle uiRectangle = GetDimensions().ToRectangle();
            Vector2 pos = uiRectangle.TopLeft();
            Vector2 centerPos = pos + uiRectangle.Size() / 2f;

            Vector2 size = rectangle.Size();
            Vector2 origin = size / 2f;
            Vector2 targetDimensions = uiRectangle.Size();
            Vector2 mult = targetDimensions / size;

            spriteBatch.Draw(texture,
                centerPos, rectangle, Color.White, 0, rectangle.Size() / 2, Vector2.One * mult, SpriteEffects.None, 0);

        }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            _action(_npc);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    public class NPCViewerUIState : UIState
    {
        private readonly Action<ModNPC> _onSelect;
        public NPCViewer ui;
        public NPCViewerUIState(Action<ModNPC> onSelect) : base()
        {
            _onSelect = onSelect;
        }

        public override void OnInitialize()
        {
            ui = new NPCViewer(_onSelect);
            Append(ui);
        }
    }

    public class NPCViewer : UIPanel
    {
        private readonly Action<ModNPC> _onSelect;
        private StructureBackground _background;
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _grid;
        private UIScrollbar _scrollbar;
        private readonly ModNPC[] _allNpcs;
        public NPCViewer(Action<ModNPC> onSelect)
        {
            _onSelect = onSelect;
            _background = new StructureBackground();
            _panel = new UIPanel();
            _grid = new UIGrid();
            _scrollbar = new FancyScrollbar();
            _uiList = new UIList();
            _allNpcs = GetNPCs();
        }


        public int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        public int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;

        public static ModNPC[] GetNPCs()
        {
            var npcs = Stellamod.Instance.GetContent<ModNPC>();
            IEnumerable<ModNPC> query = from npc in npcs orderby npc.Name select npc;
            return query.ToArray();
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 600;
            Height.Pixels = 400;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(_background);

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.ListPadding = 2f;
            _panel.Append(_grid);

            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.93f);
            _scrollbar.Top.Set(0, 0f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);

            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);

            _grid.Clear();
            foreach (var npc in _allNpcs)
            {
                NPCViewerSlot slot = new NPCViewerSlot(npc, _onSelect);
                _grid.Add(slot);
            }
        }

        public ModNPC SelectedModNpc { get; set; }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _panel.Height.Pixels = _grid.GetTotalHeight() + 32;

            _panel.BackgroundColor = Color.Transparent;
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
