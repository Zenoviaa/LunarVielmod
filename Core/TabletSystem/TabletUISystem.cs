using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TabletSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class TabletUISystem : BaseUISystem
    {
        public enum AIState
        {
            Close,
            Open
        }

        private Vector2 _talkWorld;
        private AIState _state;
        private float _timer;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public TabletUIState tabletUIState;
        public float Duration { get; set; }
        private Vector2 StartDrawOffset => new Vector2(0, 400);
        private Vector2 EndDrawOffset => new Vector2(0, 0);
        public override int uiSlot => Slot_MinorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            Duration = 1f;
            _state = AIState.Close;
            _userInterface = new UserInterface();
            tabletUIState = new TabletUIState();
            tabletUIState.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            float dist = Vector2.Distance(Main.LocalPlayer.position, _talkWorld);
            if (dist > 160)
            {
                CloseUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }

            switch (_state)
            {
                case AIState.Open:
                    Update_Open(gameTime);
                    break;
                case AIState.Close:
                    Update_Close(gameTime);
                    break;
            }
        }

        public void SwitchState(AIState state)
        {
            _state = state;
        }

        private void Update_Open(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer >= Duration)
            {
                _timer = Duration;
            }

            float progress = _timer / Duration;
            float easedProgress = EasingFunction.InOutCubic(progress);
            var ui = tabletUIState.tabletUI;
            ui.DrawOffset = Vector2.Lerp(StartDrawOffset, EndDrawOffset, easedProgress);
            ui.TabletColor = Color.Lerp(Color.Transparent, Color.White, easedProgress);
        }

        private void Update_Close(GameTime gameTime)
        {
            _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer <= 0f)
            {
                _timer = 0f;
                if (_userInterface.CurrentState != null)
                {
                    _userInterface.SetState(null);
                }
            }

            float progress = _timer / Duration;
            float easedProgress = EasingFunction.InOutCubic(progress);
            var ui = tabletUIState.tabletUI;
            ui.DrawOffset = Vector2.Lerp(StartDrawOffset, EndDrawOffset, easedProgress);
            ui.TabletColor = Color.Lerp(Color.Transparent, Color.White, easedProgress);
        }

        public override void CloseThis()
        {
            base.CloseThis();
            CloseUI();
        }

        internal void ToggleUI()
        {
            if (_userInterface.CurrentState != null)
            {
                CloseUI();
            }
            else
            {
                OpenUI();
            }
        }

        internal void OpenUI(Asset<Texture2D> innerTexture, string title, string text)
        {
            if (_state != AIState.Open)
            {
                var ui = tabletUIState.tabletUI;
                ui.InnerTexture = innerTexture;
                ui.Title.SetText(title);
                ui.Text.SetText(text);
                _talkWorld = Main.LocalPlayer.position;
                TakeSlot();
                SwitchState(AIState.Open);
                if (_userInterface.CurrentState == null)
                {

                    SoundStyle soundStyle = SoundID.MenuOpen;
                    SoundEngine.PlaySound(soundStyle);
                    _userInterface.SetState(tabletUIState);
                }
            }
        }

        internal void OpenUI()
        {
            if (_state != AIState.Open)
            {
                _talkWorld = Main.LocalPlayer.position;
                TakeSlot();
                SwitchState(AIState.Open);
                if (_userInterface.CurrentState == null)
                {

                    SoundStyle soundStyle = SoundID.MenuOpen;
                    SoundEngine.PlaySound(soundStyle);
                    _userInterface.SetState(tabletUIState);
                }
            }
        }

        internal void CloseUI()
        {
            if (_state != AIState.Close)
            {
                ClearSlot();
                SoundStyle soundStyle = SoundID.MenuClose;
                SoundEngine.PlaySound(soundStyle);
                SwitchState(AIState.Close);
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
                    "Urdveil: Tablet UI",
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
