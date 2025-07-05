using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal class NPCSelectButton : UIPanel
    {
        private readonly Action<ModNPC> _changeFunc;
        internal event Action<int> OnEmptyMouseover;
        private readonly float _scale = 1f;
        public NPCSelectButton(string pointName, Action<ModNPC> changeFunc)
        {
            _changeFunc = changeFunc;
            Width.Pixels = 256;
            Height.Pixels = 36;
            BackgroundColor = Color.Blue * 0.5f;
            BorderColor = Color.Transparent;
            Text = new UIText(pointName);
            Text.HAlign = 0.5f;
            OnLeftClick += OnButtonClick;
        }

        public UIText Text { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Append(Text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
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
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            NPCViewerUIState viewer = new NPCViewerUIState(OnSelectNPC);
            EditorPopupSystem popupUISystem = ModContent.GetInstance<EditorPopupSystem>();
            popupUISystem.OpenUI(viewer);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
        private void OnSelectNPC(ModNPC npc)
        {
            EditorPopupSystem popupUISystem = ModContent.GetInstance<EditorPopupSystem>();
            popupUISystem.CloseUI();
            _changeFunc(npc);
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }
    }
}
