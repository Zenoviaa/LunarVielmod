using ReLogic.Content;
using Stellamod.Common.UI;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TabletSystem;

[Autoload(Side = ModSide.Client)]
public class TabletUISystem : BaseUISystem
{
    public enum AIState
    {
        Close,
        Open
    }

    private Vector2 _talkWorld;
    private AIState _state;
    private float _timer;
    private float _alpha;
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    public TabletUIState tabletUIState;
    public float Duration { get; set; }

    public RenderTarget2D UITarget => ModContent.GetInstance<UIRenderTargets>().uiTarget;
    public override int uiSlot => Slot_MinorUI;
    public override void OnModLoad()
    {
        base.OnModLoad();
        Duration = 1f;
        _state = AIState.Close;
        _userInterface = new UserInterface();
        tabletUIState = new TabletUIState();
        tabletUIState.Activate();

        On_Main.CheckMonoliths += RenderUI;
    }

    private void RenderUI(On_Main.orig_CheckMonoliths orig)
    {
        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
        {
            PlayerInput.SetZoom_UI();
            Main.spriteBatch.GraphicsDevice.SetRenderTarget(UITarget);
            Main.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null,
                    Main.UIScaleMatrix);

            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

            Main.spriteBatch.End();
            PlayerInput.SetZoom_World();
        }

        orig();
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

        _alpha = _timer / Duration;
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

        _alpha = _timer / Duration;
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
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }

    public void OpenUI(Asset<Texture2D> innerTexture, string title, string text)
    {
        if (_state != AIState.Open)
        {
            var ui = tabletUIState.tabletUI;
            ui.InnerTexture = innerTexture;
            ui.Title.SetText(title);
            ui.helpText = text;
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

    public void OpenUI()
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

    public void CloseUI()
    {
        if (_state != AIState.Close)
        {
            ClearSlot();
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
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Player Chat"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Stellamod: Tablet UI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {

                        SpriteBatch spriteBatch = Main.spriteBatch;
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);

                        Vector2 offset = Vector2.Lerp(-Vector2.UnitX * 100, Vector2.Zero, EasingFunction.OutSine(_alpha));
                        Color color = Color.Lerp(Color.Transparent, Color.White, _alpha);
                        spriteBatch.Draw(UITarget, offset, color);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
