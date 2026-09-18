using Stellamod.Common.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{
    public class CollectionBookUIState : UIState
    {
        public CollectionBookUI bookUI;
        public CommonBackButton backButton;
        public int RelativeLeft => Main.screenWidth / 2 - 64;
        public int RelativeTop => Main.screenHeight / 2 + 312 ;
        public CollectionBookUIState() : base()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
         //in.NewText("Guh");   Ma
            backButton.Left.Pixels = RelativeLeft;
            backButton.Top.Pixels = RelativeTop;
            backButton.alpha = bookUI.book.alpha;

         
        }
        public override void OnInitialize()
        {
            bookUI = new CollectionBookUI();
            Append(bookUI);

            backButton = new CommonBackButton(() => ModContent.GetInstance<CollectionBookUISystem>().CloseBookUI());
            Append(backButton);
        }
    }
}
