using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.QuestSystem;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.DialogueTowning
{
    [Autoload(Side = ModSide.Client)]
    public class DialogueTowningUISystem : BaseUISystem
    {
        private BaseDialogue[] _oldDialogues;
        private float _dialogueTimer;
        public enum Animation
        {
            Open,
            Close
        }

        private Animation _animation;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        private Vector2 _talkWorld;

        private Vector2 StartDrawOffset => new Vector2(-200, 0);
        private Vector2 EndDrawOffset => new Vector2(0, 0);


        public DialogueTowningUIState dialogueTowningUIState;

        public float Duration { get; set; }

        public int WhosTalking { get; set; }
        public static string RootTexturePath => "Stellamod/UI/DialogueTowning/";
        public static string RootPortraitTexturePath => typeof(BaseDialogue).DirectoryHere() + "/";
        public override int uiSlot => -1;
        public override void OnModLoad()
        {
            base.OnModLoad();

            _userInterface = new UserInterface();
            dialogueTowningUIState = new DialogueTowningUIState();
            dialogueTowningUIState.Activate();
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
        }

        public override void CloseThis()
        {
            base.CloseThis();
            CloseUI();
        }

        public void Interact(VeilTownNPC townNPC)
        {

            if (WhosTalking == townNPC.Type)
                return;
            SoundEngine.PlaySound(SoundID.Chat);
            string text = string.Empty;
            string portrait = "FenixPortrait";
            float timeBetweenTexts = 0.05f;
            SoundStyle? talkingSound = null;
            dialogueTowningUIState.dialogueTownButtonsUI.ClearButtons();

            //Create buttons and open dialogue
            List<Tuple<string, Action>> buttons = new List<Tuple<string, Action>>();
            townNPC.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);

            //Some goofballs you can only interact with no dialogue
            if (townNPC.OnlyInteract)
                return;

            //Check if quest giver
            if (townNPC.HasQuestAvailable())
            {
                buttons.Add(new Tuple<string, Action>("Quest", townNPC.GiveQuest));
            }
            buttons.Add(new Tuple<string, Action>("Close", townNPC.CloseTownDialogue));
            foreach (var pair in buttons)
            {
                dialogueTowningUIState.dialogueTownButtonsUI.AddButton(pair.Item1, pair.Item2);
            }

            OpenUI();
            dialogueTowningUIState.dialogueTownUI.ResetText();
            dialogueTowningUIState.dialogueTownUI.LocalizedText = LangText.TownDialogue(text);
            dialogueTowningUIState.dialogueTownUI.TalkingSound = talkingSound;


            SetPortrait(portrait);
            _talkWorld = Main.LocalPlayer.position;
            WhosTalking = townNPC.NPC.type;
        }

        public void ChatWith(Quest quest)
        {
            string text = string.Empty;
            string portrait = "FenixPortrait";
            float timeBetweenTexts = 0.05f;
            SoundStyle? talkingSound = SoundID.Item1;
            quest.QuestIntroDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            dialogueTowningUIState.dialogueTownUI.ResetText();
            dialogueTowningUIState.dialogueTownUI.LocalizedText = LangText.TownDialogue(text);
            dialogueTowningUIState.dialogueTownUI.TalkingSound = talkingSound;
            SetPortrait(portrait);
        }

        public void ChatWith(BaseDialogue dialogue, int lineNumber)
        {
            DialogueTowningUI ui = dialogueTowningUIState.dialogueTownUI;
            ui.ClearText();
            ui.PrepareForTalking();
            _talkWorld = Main.LocalPlayer.position;
            OpenUI();
            SoundStyle? talkingSound = SoundID.Item1;
            dialogueTowningUIState.dialogueTownUI.ResetText();
            dialogueTowningUIState.dialogueTownUI.LocalizedText = dialogue.GetLine(lineNumber);
            dialogueTowningUIState.dialogueTownUI.TalkingSound = talkingSound;
        }

        public void ChatWith(VeilTownNPC veilTownNPC)
        {



            /*
            string text = string.Empty;
            string portrait = "FenixPortrait";
            float timeBetweenTexts = 0.05f;
            SoundStyle? talkingSound = null;
            veilTownNPC.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            dialogueTowningUIState.dialogueTownUI.ResetText();
            dialogueTowningUIState.dialogueTownUI.LocalizedText = LangText.TownDialogue(text);
            dialogueTowningUIState.dialogueTownUI.TalkingSound = talkingSound;
            SetPortrait(portrait);*/
        }

        public void OpenTalkOptions(BaseDialogue[] dialogues)
        {
            DialogueTowningUI ui = dialogueTowningUIState.dialogueTownUI;
            ui.ClearText();
            ui.PrepareForTalkingOptions();

            TalkingOptionsButtonGroupUI options = dialogueTowningUIState.talkingOptionsUI;
            options.ClearButtons();
            foreach(BaseDialogue dialogue in dialogues)
            {
                DialogueTalkingOption talkingoption = new DialogueTalkingOption(dialogue.DisplayName, dialogue);
                options.AddButton(talkingoption);
          
            }
            _oldDialogues = dialogues;
        }

        public void RefreshTalkOptions()
        {
            OpenTalkOptions(_oldDialogues);
        }
        public void ClearOptions()
        {
            TalkingOptionsButtonGroupUI options = dialogueTowningUIState.talkingOptionsUI;
            options.ClearButtons();
        }

        public void SetPortrait(string portrait)
        {
            portrait = portrait.Replace("Portrait", string.Empty);
            dialogueTowningUIState.dialogueTownUI.Portrait = ModContent.Request<Texture2D>(RootPortraitTexturePath + $"{portrait}");
        }
        public override void UpdateUI(GameTime gameTime)
        {
            Duration = 1f;
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
            float dist = Vector2.Distance(Main.LocalPlayer.position, _talkWorld);
            if (dist > 160)
            {
                CloseUI();
            }

            switch (_animation)
            {
                case Animation.Open:
                    Update_Open(gameTime);
                    break;
                case Animation.Close:
                    Update_Close(gameTime);
                    break;
            }
        }


        public void SwitchState(Animation animation)
        {
            _animation = animation;
        }

        private void Update_Open(GameTime gameTime)
        {
            _dialogueTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_dialogueTimer >= Duration)
            {
                _dialogueTimer = Duration;
            }

            float progress = _dialogueTimer / Duration;
            float easedProgress = Easing.OutExpo(progress);
            dialogueTowningUIState.dialogueTownUI.Alpha = easedProgress;
            dialogueTowningUIState.dialogueTownButtonsUI.Alpha = easedProgress;
            dialogueTowningUIState.dialogueTownUI.Offset = Vector2.Lerp(StartDrawOffset, EndDrawOffset, easedProgress);
            dialogueTowningUIState.dialogueTownButtonsUI.Offset = dialogueTowningUIState.dialogueTownUI.Offset;
        }

        private void Update_Close(GameTime gameTime)
        {
            _dialogueTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_dialogueTimer <= 0f)
            {
                _dialogueTimer = 0f;
                if (_userInterface.CurrentState != null)
                {
                    WhosTalking = -1;
                    _userInterface.SetState(null);
                }
            }

            float progress = _dialogueTimer / Duration;
            float easedProgress = Easing.InOutSine(progress);
            dialogueTowningUIState.dialogueTownUI.Alpha = easedProgress;
            dialogueTowningUIState.dialogueTownButtonsUI.Alpha = easedProgress;
            dialogueTowningUIState.dialogueTownUI.Offset = Vector2.Lerp(StartDrawOffset, EndDrawOffset, easedProgress);
            dialogueTowningUIState.dialogueTownButtonsUI.Offset = dialogueTowningUIState.dialogueTownUI.Offset;
        }

        public void OpenUI()
        {
            if (_animation != Animation.Open)
            {
                TakeSlot();
                SwitchState(Animation.Open);
                if (_userInterface.CurrentState == null)
                {

                    SoundStyle soundStyle = SoundID.MenuClose;
                    SoundEngine.PlaySound(soundStyle);
                    _userInterface.SetState(dialogueTowningUIState);
                }
            }
            //Set State;

        }

        public void CloseUI()
        {

            if (_animation != Animation.Close)
            {
                Main.CloseNPCChatOrSign();
                ClearSlot();
                SoundStyle soundStyle = SoundID.MenuClose;
                SoundEngine.PlaySound(soundStyle);
                SwitchState(Animation.Close);
            }

        }
        public void OnlyCloseWindow()
        {

            if (_animation != Animation.Close)
            {
                ClearSlot();
                SoundStyle soundStyle = SoundID.MenuClose;
                SoundEngine.PlaySound(soundStyle);
                SwitchState(Animation.Close);
            }

        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                _userInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {

            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Dialogue Towning UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
