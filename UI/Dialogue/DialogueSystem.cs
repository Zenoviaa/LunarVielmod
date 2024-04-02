using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Dialogue
{
    [Autoload(Side = ModSide.Client)]
    internal class DialogueSystem : ModSystem
    {
        private UserInterface _panel;
        private string _text;
        private string _fullText;

        private float _easeTimer;
        private float _textingCounter;
        private int _textingLength;
        private int _dialogueIndex;

        public DialoguePanel Panel { get; private set; }
        public Dialogue Dialogue { get; private set; }
        public float TextingSpeed;
        public int NumLines { get; private set; }
        public bool IsFinishedWithLine { get; private set; }
        public bool IsVisible { get; set; }
        public override void Load()
        {
            Panel = new DialoguePanel();
            Panel.OnLeftClick += OnButtonClick; 
            Panel.Activate();
            _panel = new UserInterface();
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            //If no dialogue then no
            if (Dialogue == null)
                return;

            // We can do stuff in here!
            if (!IsFinishedWithLine)
            {
                SkipLine();
                return;
            }

            _dialogueIndex++;
            if(_dialogueIndex >= Dialogue.Length)
            {
                //End
                ResetTexts();
                IsVisible = false;
                Dialogue.Complete();
            }
            else
            {
                Dialogue.Next(_dialogueIndex);
            }
        }

        private void NextCharacter()
        {
            //Call this every frame for type writer effect
            _textingCounter++;
            if (_textingCounter >= TextingSpeed && _textingLength < _fullText.Length)
            {
                _textingLength++;
                _text = _fullText.Substring(0, _textingLength);

                //Fill the rest of the line with blank spaces so yeah
                for (int i = 0; i < 200; i++)
                {
                    _text += " ";
                }


                _textingCounter = 0;
                Panel.Text.SetText(_text);
            }

            if (_textingLength >= _fullText.Length)
            {
                IsFinishedWithLine = true;
            }
        }


        private void SkipLine()
        {
            //Store the text in variable
            _textingCounter = 0;
            _textingLength = _fullText.Length;
            _text = _fullText.Substring(0, _textingLength);
            //Fill the rest of the line with blank spaces so yeah
            for (int i = 0; i < 200; i++)
            {
                _text += " ";
            }

            //Update the text
            Panel.Text.SetText(_text);
            IsFinishedWithLine = true;
        }

        private void UpdateVisibility()
        {
            float easeLength = 60;
            _easeTimer = MathHelper.Clamp(_easeTimer, 0, easeLength);

            if (IsVisible)
            {
                _panel?.SetState(Panel);
                _easeTimer++;
              
                float progress = _easeTimer / easeLength;
                float easedProgress = Easing.OutCubic(progress);
                Panel.UIPanel.VAlign = MathHelper.Lerp(2, Panel.PresetVAlign, easedProgress);
            }
            else
            {
                _easeTimer--;
                float progress = _easeTimer / easeLength;
                float easedProgress = Easing.OutCubic(progress);
                Panel.UIPanel.VAlign = MathHelper.Lerp(2, Panel.PresetVAlign, easedProgress);
                if(_easeTimer <= 0)
                {
                    _panel?.SetState(null);
                }
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _panel?.Update(gameTime);

            UpdateVisibility();
            if (_fullText != string.Empty && _text != _fullText)
            {
         
                NextCharacter();
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "YourMod: A Description",
                    delegate
                    {
                        _panel.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void StartDialogue(Dialogue dialogue)
        {
            //Reset our stuffs
            ResetTexts();
            if (!IsVisible)
            {
                _easeTimer = 0;
                Panel.UIPanel.VAlign = 3;
            }
       
            IsVisible = true;

            //Reset the dialogue index so we start from the beginning
            _dialogueIndex = 0;
            Dialogue = dialogue;

            //Let's goooo
            Dialogue?.Next(_dialogueIndex);
        }

        private void ResetTexts()
        {
            _text = string.Empty;
            _fullText = string.Empty;
            _textingCounter = 0;
            _textingLength = 0;
            Panel.Text.SetText(string.Empty);
            IsFinishedWithLine = false;
        }

        public void WriteText(string text)
        {
            ResetTexts();
            _text = string.Empty;
            _fullText = text;
        }

        public void SetPortrait(string texture)
        {
            Panel.Portrait.SetImage(ModContent.Request<Texture2D>(texture));
        }
    }
}
