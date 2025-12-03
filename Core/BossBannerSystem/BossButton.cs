using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Opens a boss page
    /// </summary>
    public class BossButton : UIPanel
    {
        private BossPage _bossPage;
        private BossPageUI _ui;
        public BossButton(BossPageUI ui, BossPage bossPage)
        {
            _ui = ui;
            _bossPage = bossPage;
            OnLeftClick += OpenBossPage;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48;
            Height.Pixels = 48;
        }

        private void OpenBossPage(UIMouseEvent evt, UIElement listeningElement)
        {
            _ui.SetBossPage(_bossPage);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            Vector2 topLeft = rectangle.TopLeft();
            Asset<Texture2D> bossIcon = _bossPage.RequestBossIcon();
            if (IsMouseHovering)
            {
                UIHelper.QuickOutline(spriteBatch, bossIcon.Value, topLeft, Color.Yellow);
            }
          
            spriteBatch.Draw(bossIcon.Value, topLeft, Color.White);
 
        }
    }
}
