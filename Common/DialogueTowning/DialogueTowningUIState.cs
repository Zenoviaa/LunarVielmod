using Terraria.UI;

namespace Stellamod.Common.DialogueTowning
{
    public class DialogueTowningUIState : UIState
    {
        public DialogueTowningUI dialogueTownUI;
        public DialogueTowningButtonGroupUI dialogueTownButtonsUI;
        public TalkingOptionsButtonGroupUI talkingOptionsUI;
        public DialogueTowningUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            dialogueTownUI = new DialogueTowningUI();
            Append(dialogueTownUI);

            dialogueTownButtonsUI = new DialogueTowningButtonGroupUI();
            Append(dialogueTownButtonsUI);


            talkingOptionsUI = new TalkingOptionsButtonGroupUI();
            Append(talkingOptionsUI);
        }
    }
}
