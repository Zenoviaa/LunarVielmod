using Terraria.UI;

namespace Stellamod.UI.DialogueTowning
{
    public class DialogueTowningUIState : UIState
    {
        public DialogueTowningUI dialogueTownUI;
        public DialogueTowningButtonGroupUI dialogueTownButtonsUI;
        public DialogueTowningUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            dialogueTownUI = new DialogueTowningUI();
            Append(dialogueTownUI);

            dialogueTownButtonsUI = new DialogueTowningButtonGroupUI();
            Append(dialogueTownButtonsUI);
        }
    }
}
