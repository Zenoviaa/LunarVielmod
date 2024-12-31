using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.PopupSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class PopupUISystem : ModSystem
    {
        private enum AIState
        {
            Open,
            Idle,
            Close
        }

        private float _timer;
        private float _idleTimer;
        private float _inDuration;
        private float _duration;
        private AIState _state;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public static string RootTexturePath => "Stellamod/UI/PopupSystem/";

        public PopupUIState popupUIState;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _inDuration = 0.5f;
            _duration = 2f;
            _userInterface = new UserInterface();
            popupUIState = new PopupUIState();
            popupUIState.Activate();
            _userInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            float curProgress = _timer / _inDuration;
            float easedProgress = Easing.InOutCirc(curProgress);
            Vector2 offset = Vector2.Lerp(new Vector2(0, -256), new Vector2(0, 64), easedProgress);
            popupUIState.popupUI.Offset = offset;
            switch (_state)
            {
                case AIState.Open:
                    _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_timer >= _inDuration)
                    {
                        SwitchState(AIState.Idle);
                    }
                    break;
                case AIState.Idle:
                    _idleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(_idleTimer >= _duration)
                    {
                        SwitchState(AIState.Close);
                    }
                    break;
                case AIState.Close:
                    _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_timer <= 0)
                    {
                        _timer = 0f;
                        CloseUI();
                    }
                    break;
            }
            //Close if inventory isn't open lol
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
        }

        private void SwitchState(AIState state)
        {
            _state = state;
        }
        internal void OpenUI(string text)
        {
            _idleTimer = 0f;
            popupUIState.popupUI.SetText(text);
            SwitchState(AIState.Open);
            _userInterface.SetState(popupUIState);
        }

        private void CloseUI()
        {
            _userInterface.SetState(null);
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
                    "LunarVeil: Popup UI",
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
