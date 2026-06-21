using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.IgnitersNPowders;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.PowderSystem
{
    public class PowderUI : UIPanel
    {
        private List<PowderSlot> _slots;
        private BaseIgniterCard _card;
        private UIGrid _grid;

        public const int width = 480;
        public const int height = 155;

        public int RelativeLeft => 64;
        public int RelativeTop => 0 + 256;

        public PowderUI() : base()
        {
            _grid = new UIGrid();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = width;
            Height.Pixels = height;
            SetPos();

            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;


            _grid.Left.Pixels = 0;
            _grid.Top.Pixels = 0;
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0f;
            _grid.ListPadding = 2f;
            Append(_grid);
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }


        public void OpenUI(BaseIgniterCard card)
        {
            _card = card;
            _grid.Clear();
            _slots = new List<PowderSlot>();
            for (int i = 0; i < card.Powders.Count; i++)
            {
                PowderSlot slot = new PowderSlot(card, _grid._items.Count);
                _grid.Add(slot);
                _slots.Add(slot);
            }
        }

        private void SetPos()
        {
   
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            SetPos();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            for(int i = 0; i < _slots.Count; i++)
            {
                var uiElement = _slots[i];
                Vector2 center = uiElement.GetDimensions().ToRectangle().Center();
                SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Main.screenPosition + center);
                glowDrawer.scale *= 0.25f;
                glowDrawer.color = Color.White * ExtraMath.Osc(0.4f, 0.6f, speed: 6);
                glowDrawer.color.A = 0;
                spriteBatch.Draw(glowDrawer);
            }
        }
    }
}