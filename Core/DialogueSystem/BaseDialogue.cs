using Stellamod.Helpers;
using Stellamod.UI.Dialogue;
using Stellamod.UI.DialogueTowning;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.DialogueSystem
{
    public class Speaking : ModBuff
    {

    }

    public class DialogueCutscenePlayer : ModPlayer
    {
        public override bool CanUseItem(Item item)
        {
            return base.CanUseItem(item) && !Player.HasBuff<Speaking>();
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class DialogueSystemV2 : ModSystem
    {
        private DialogueActor _dialogueActor;
        private bool _hasCompleted;
        public bool inDialogue => _dialogueActor != null;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (_dialogueActor == null)
                return;
            Main.LocalPlayer.AddBuff(ModContent.BuffType<Speaking>(), 2);

            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (_dialogueActor.IsFinished() && !_hasCompleted)
                {
                    _dialogueActor.dialogue.OnComplete();
                    _hasCompleted = true;

                    DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
                    if (_dialogueActor.dialogue.CloseOnComplete)
                    {
                        uiSystem.CloseUI();
                    }
                    else
                    {
                        uiSystem.RefreshTalkOptions();
                    }

                        _dialogueActor = null;
           
                }
                else
                {
                    _dialogueActor.ProgressLine();
                }
                  
 
                Main.mouseLeftRelease = false;
            }
        }
        public DialogueActor StartDialogueSequence(BaseDialogue dialogue)
        {
            _hasCompleted = false;
            dialogue.OnStart();
            _dialogueActor = new DialogueActor(dialogue);
            _dialogueActor.ProgressLine();
            DialogueTowningUISystem uiSystem = ModContent.GetInstance<DialogueTowningUISystem>();
            uiSystem.OpenUI();
            uiSystem.ClearOptions();
            uiSystem.ClearButtons();
            return _dialogueActor;

        }
        public bool HasFinishedDialogue()
        {
            return _hasCompleted;
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
        public bool CloseOnComplete { get; set; }
        public string DisplayName
        {
            get
            {
                return LangText.Dialogue(this, "DisplayName");
            }
        }

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
            this.GetLocalization($"DisplayName", () => "");
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
