using Terraria.UI;

namespace Stellamod.Core.CollectionSystem.Quests
{

    internal class ActiveQuestUIState : UIState
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
