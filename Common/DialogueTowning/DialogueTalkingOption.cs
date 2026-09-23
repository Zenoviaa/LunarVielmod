using Stellamod.Core.DialogueSystem;
using Terraria.ModLoader;

namespace Stellamod.Common.DialogueTowning;

public class DialogueTalkingOption : ITalkingOption
{
    public DialogueTalkingOption(string localizedText, BaseDialogue dialogue)
    {
        LocalizedText = localizedText;
        Dialogue = dialogue;
    }

    public readonly string LocalizedText;
    public readonly BaseDialogue Dialogue;

    public string GetDisplayName()
    {
        return LocalizedText;
    }

    public void Talk()
    {
        DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
        dialogueSystem.StartDialogueSequence(Dialogue);
    }

    public void Show(SpriteBatch spriteBatch)
    {

    }
}
