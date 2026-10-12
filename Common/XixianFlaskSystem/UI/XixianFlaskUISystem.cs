using Microsoft.Xna.Framework;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.XixianFlaskSystem.UI
{

    [Autoload(Side = ModSide.Client)]
    public class XixianFlaskUISystem : BaseUISystem
    {
        bool _isOpen;
        float _elaspedOpenTime;
        float OpenTime => 0.5f;
        float OpenLerp => EasingFunction.InOutSine(_elaspedOpenTime / OpenTime);
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        private UserInterface _hudUserInterface;
        public static string RootTexturePath => typeof(XixianFlaskUISystem).DirectoryHere() + "/";
        public static string RootItemTexturePath => typeof(XixianFlask).DirectoryHere() + "/";

        public XixianFlaskUIState xixianFlaskUIState;
        public FlaskSlotUIState flaskSlotUIState;
        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();

            _hudUserInterface = new UserInterface();
            xixianFlaskUIState = new XixianFlaskUIState();
            xixianFlaskUIState.Activate();
            flaskSlotUIState = new();
            flaskSlotUIState.Activate();

            _hudUserInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Close if inventory isn't open lol
            if (_hudUserInterface.CurrentState == null)
            {
                OpenHudUI();
            }

            if (_isOpen)
            {
                _elaspedOpenTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                _elaspedOpenTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(_userInterface?.CurrentState == null && _elaspedOpenTime > 0)
            {
                TakeSlot();
                _userInterface.SetState(xixianFlaskUIState);
            } else if(_userInterface?.CurrentState != null && _elaspedOpenTime <= 0)
            {
                ClearSlot();
                _userInterface.SetState(null);
            }
            _elaspedOpenTime = MathHelper.Clamp(_elaspedOpenTime, 0f, OpenTime);
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }


            if (_hudUserInterface?.CurrentState != null)
            {
                _hudUserInterface.Update(gameTime);
            }
        }

        public override void CloseThis()
        {
            base.CloseThis();
            CloseUI();
        }

        public void ToggleUI()
        {
            if (_userInterface.CurrentState != null)
            {
                SoundStyle soundStyle = SoundID.MenuClose;
                SoundEngine.PlaySound(soundStyle);
                CloseUI();
            }
            else
            {
                SoundStyle soundStyle = SoundID.MenuOpen;
                SoundEngine.PlaySound(soundStyle);
                OpenUI();
            }
        }
        public void OpenHudUI()
        {
            _hudUserInterface.SetState(flaskSlotUIState);
        }

        public void CloseHudUI()
        {
            _hudUserInterface.SetState(null);
        }
        public void OpenUI()
        {
            //Set State
            _isOpen = true;
        }

        public void CloseUI()
        {
            _isOpen = false;
        }

        public override void PreSaveAndQuit()
        {

        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Stellamod: Xixian Flask UI",
                    delegate
                    {

                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            if(_elaspedOpenTime < OpenTime)
                            {
                                var sb = Main.spriteBatch;
                                sb.EndOut(out var oldParameters);
                                using var target = RT.Context(RenderTargets.ScreenTarget);
                                using(RT.Clear(target, Color.Transparent))
                                {
                                    using (sb.Ctx(oldParameters))
                                    {
                                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                                    }
                                }

                                using(sb.Ctx(oldParameters with { matrix = Matrix.identity }))
                                {
                                    sb.Draw(target, 
                                        Vector2.Lerp(new Vector2(0, -80), Vector2.Zero, EasingFunction.OutSine(OpenLerp)), 
                                        null,
                                        Color.Lerp(Color.Transparent, Color.White, OpenLerp),
                                        0,
                                        Vector2.Zero,
                                        1f,
                                        SpriteEffects.None,
                                        0);
                                }
                                sb.Begin(oldParameters);
                            
                            }
                            else
                            {
                                _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                            }
                        }

                        if (_lastUpdateUiGameTime != null && _hudUserInterface?.CurrentState != null)
                        {
                            _hudUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }

                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
