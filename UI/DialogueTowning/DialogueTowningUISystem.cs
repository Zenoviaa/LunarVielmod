using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellamod.Common;
using Stellamod.Common.QuestSystem;
using Stellamod.Helpers;
using Stellamod.UI.CauldronSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.UI.DialogueTowning
{
    [Autoload(Side = ModSide.Client)]
    public class DialogueTowningUISystem : ModSystem
    {
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
        private bool _killUi;
        private Vector2 StartDrawOffset => new Vector2(0, 400);
        private Vector2 EndDrawOffset => new Vector2(0, 0);


        public DialogueTowningUIState dialogueTowningUIState;

        public float Duration { get; set; }

        public int WhosTalking { get; set; }
        public static string RootTexturePath => "Stellamod/UI/DialogueTowning/";

        public override void OnModLoad()
        {
            base.OnModLoad();
            Duration = 1f;
            _userInterface = new UserInterface();
            dialogueTowningUIState = new DialogueTowningUIState();
            dialogueTowningUIState.Activate();
            On_Player.SetTalkNPC += SetTalker;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Player.SetTalkNPC -= SetTalker;
        }

        private void SetTalker(On_Player.orig_SetTalkNPC orig, Player self, int npcIndex, bool fromNet)
        {
            if(Main.netMode != 1 && npcIndex >= 0 && npcIndex < 200)
            {
                NPC npc = Main.npc[npcIndex];
                if (npc.ModNPC is VeilTownNPC veilTownNPC && veilTownNPC.HasTownDialogue)
                {
                    if(WhosTalking != npc.ModNPC.Type)
                    {
                        string text = string.Empty;
                        string portrait = "FenixPortrait";
                        float timeBetweenTexts = 0.05f;
                        SoundStyle? talkingSound = null;
                        dialogueTowningUIState.dialogueTownButtonsUI.ClearButtons();
                        List<Tuple<string, Action>> buttons = new List<Tuple<string, Action>>();
                        veilTownNPC.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
                        if (veilTownNPC.HasQuestAvailable())
                        {
                            buttons.Add(new Tuple<string, Action>("Quest", veilTownNPC.GiveQuest));
                        }
                        foreach (var pair in buttons)
                        {
                            dialogueTowningUIState.dialogueTownButtonsUI.AddButton(pair.Item1, pair.Item2);
                        }

                        _killUi = true;
                        OpenUI();
                        dialogueTowningUIState.dialogueTownUI.ResetText();
                        dialogueTowningUIState.dialogueTownUI.LocalizedText = LangText.TownDialogue(text);
                        dialogueTowningUIState.dialogueTownUI.TalkingSound = talkingSound;
                        dialogueTowningUIState.dialogueTownUI.Portrait = ModContent.Request<Texture2D>(RootTexturePath + $"{portrait}");
                        _talkWorld = Main.LocalPlayer.position;
                        WhosTalking = veilTownNPC.NPC.type;
                        orig(self, npcIndex, fromNet);
                    } 
                    return;
                } 
            }
            _killUi = false;
            orig(self, npcIndex, fromNet);
        }
        public void ChatWith(Quest quest)
        {
            string text = string.Empty;
            string portrait = "FenixPortrait";
            float timeBetweenTexts = 0.05f;
            SoundStyle? talkingSound = null;
            quest.QuestIntroDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            dialogueTowningUIState.dialogueTownUI.ResetText();
            dialogueTowningUIState.dialogueTownUI.LocalizedText = LangText.TownDialogue(text);
            dialogueTowningUIState.dialogueTownUI.TalkingSound = talkingSound;
            dialogueTowningUIState.dialogueTownUI.Portrait = ModContent.Request<Texture2D>(RootTexturePath + $"{portrait}");
        }


        public void ChatWith(VeilTownNPC veilTownNPC)
        {
            string text = string.Empty;
            string portrait = "FenixPortrait";
            float timeBetweenTexts = 0.05f;
            SoundStyle? talkingSound = null;
            veilTownNPC.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            dialogueTowningUIState.dialogueTownUI.ResetText();
            dialogueTowningUIState.dialogueTownUI.LocalizedText = LangText.TownDialogue(text);
            dialogueTowningUIState.dialogueTownUI.TalkingSound = talkingSound;
            dialogueTowningUIState.dialogueTownUI.Portrait = ModContent.Request<Texture2D>(RootTexturePath + $"{portrait}");
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
            float easedProgress = Easing.InOutCubic(progress);
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
            float easedProgress = Easing.InOutCubic(progress);
            dialogueTowningUIState.dialogueTownUI.Offset = Vector2.Lerp(StartDrawOffset, EndDrawOffset, easedProgress);
            dialogueTowningUIState.dialogueTownButtonsUI.Offset = dialogueTowningUIState.dialogueTownUI.Offset;
        }

        internal void OpenUI()
        {
            if (_animation != Animation.Open)
            {
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

        internal void CloseUI()
        {
            if(_animation != Animation.Close)
            {
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

            GameInterfaceLayer layer = layers.FirstOrDefault(x => x.Name == "Vanilla: NPC / Sign Dialog");
            if (layer is not null && _killUi)
            {
                layers.Remove(layer);
            }
        }
    }
}
