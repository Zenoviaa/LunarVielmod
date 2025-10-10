using Terraria.UI;

namespace Stellamod.UI.CollectionSystem.Quests
{

    public class ActiveQuestUIState : UIState
    {
        public ActiveQuestUI ui;
        public ActiveQuestUIState() : base()
        {

        }
        public override void OnInitialize()
        {
            ui = new ActiveQuestUI();
            Append(ui);
        }
    }
}
