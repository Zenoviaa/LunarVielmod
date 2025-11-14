using Stellamod.UI.DialogueTowning;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.DialogueSystem
{
    [Autoload(Side = ModSide.Client)]
    public class DialogueSystemV2 : ModSystem
    {
        private DialogueActor _dialogueActor;
        private bool _hasCompleted;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (_dialogueActor == null)
                return;
            if(Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (_dialogueActor.IsFinished() && !_hasCompleted)
                {
                    _dialogueActor.dialogue.OnComplete();
                    _hasCompleted = true;
                    _dialogueActor = null;
                }
                else
                {
                    _dialogueActor.ProgressLine();
                }
                  
 
                Main.mouseLeftRelease = false;
            }
        }
        public void StartDialogueSequence(BaseDialogue dialogue)
        {
            _hasCompleted = false;
            dialogue.OnStart();
            _dialogueActor = new DialogueActor(dialogue);
            _dialogueActor.ProgressLine();
        }
    }
    public class DialogueActor
    {
        public DialogueActor(BaseDialogue dialogue)
        {
            this.dialogue = dialogue;
        }
        public readonly BaseDialogue dialogue;
        public int currentLineIndex;

        public void ProgressLine()
        {
            DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
            uiSystem.ChatWith(dialogue, currentLineIndex);
            currentLineIndex++;
        }

        public bool IsFinished()
        {
            return currentLineIndex >= dialogue.GetLength();
        }
    }
    public abstract class BaseDialogue : ModType,
        ILocalizedModType
    {
        public int Type { get; internal set; }
        public string LocalizationCategory => "TownDialogue";
        protected sealed override void Register()
        {
            ModTypeLookup<BaseDialogue>.Register(this);
            DialogueLoader.RegisterQuest(this);
        }

        public virtual int GetLength()
        {
            return 1;
        }

        public virtual void OnStart()
        {

        }
        public virtual void OnComplete()
        {

        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
            for (int i = 0; i < GetLength(); i++)
            {
                this.GetLocalization($"Line{i}", () => "");
            }
        }

        public string GetLine(int lineNumber)
        {
            return this.GetLocalization($"Line{lineNumber}").Value;
        }
    }
}
