using Terraria.UI;

namespace Stellamod.Core.CollectionSystem.Quests
{
    internal class QuestTabUIState : UIState
    {
        public QuestTabUI ui;
        public QuestTabUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new QuestTabUI();
            Append(ui);
        }
    }
}
