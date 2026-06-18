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
        public int RelativeLeft => Main.screenWidth / 2 + 450;
        public int RelativeTop => Main.screenHeight / 2 - 800 / 2 + 128;
        public CollectionBookUIState() : base()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
         //in.NewText("Guh");   Ma
            backButton.Left.Pixels = RelativeLeft;
            backButton.Top.Pixels = 128;
 
         
        }
        public override void OnInitialize()
        {
            bookUI = new CollectionBookUI();
            Append(bookUI);

            backButton = new CommonBackButton(() => ModContent.GetInstance<CollectionBookUISystem>().CloseBookUI());
//            backButton.Left.Set(0f, 1f);
            backButton.asXButton = true;
            backButton.axXBigButton = true;
            Append(backButton);
        }
    }
}
