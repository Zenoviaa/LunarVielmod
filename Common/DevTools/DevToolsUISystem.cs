using Microsoft.Xna.Framework.Input;
using Stellamod.Core.TriggersSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.DevTools;

[Autoload(Side = ModSide.Client)]
public class DevToolsUISystem : ModSystem
{
    bool _pressed;
    bool _rightDown;
    bool _rightPressed;
    private UserInterface _userInterface;
    private UserInterface _npcUserInterface;
    private GameTime _lastUpdateUiGameTime;
    public DevToolHotbarUIState uiState;
    public NPCViewerUIState viewerUIState;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        _npcUserInterface = new();
        uiState = new();
        uiState.Activate();
        viewerUIState = new((NPC npc) => DevTools.spawnNPCType = npc.type);
    }

    public void ToggleNPCMenuUI()
    {
        if(_npcUserInterface.CurrentState == null)
            _npcUserInterface.SetState(viewerUIState);
        else
            _npcUserInterface.SetState(null);
    }
    public void CloseNPCMenuUI()
    {
        _npcUserInterface.SetState(null);
    }
    public bool IsNPCMenuOpen()
    {
        return _npcUserInterface.CurrentState != null;
    }
    public void OpenUI()
    {
        //Create a new editing context
        //Set the state of the interface.
        _userInterface.SetState(uiState);
    }

    public void CloseUI()
    {
        _userInterface.SetState(null);
        _npcUserInterface.SetState(null);
    }

    public void ToggleUI()
    {
        if (_userInterface.CurrentState == null)
            OpenUI();
        else
            CloseUI();
    }

    public override void UpdateUI(GameTime gameTime)
    {

        DevTools.UpdateFreeCameraPosition();
        if (InputHelper.KeyDown(Keys.OemOpenBrackets))
        {
            _pressed = true;
        }
        else if (_pressed && InputHelper.KeyUp(Keys.OemOpenBrackets))
        {
            ToggleUI();
            _pressed = false;
        }
        _lastUpdateUiGameTime = gameTime;
        if (_userInterface?.CurrentState != null)
        {
            _userInterface.Update(gameTime);
        }
        if (_npcUserInterface?.CurrentState != null)
        {
            _npcUserInterface.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Scarlet Sun: Dev Tools Hotbar",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    if (_lastUpdateUiGameTime != null && _npcUserInterface?.CurrentState != null)
                    {
                        _npcUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }

    public override void PostDrawFullscreenMap(ref string mouseText)
    {
        base.PostDrawFullscreenMap(ref mouseText);

        _rightPressed = false;
        var state = Mouse.GetState();
        if (_rightDown && state.RightButton == ButtonState.Pressed)
        {
            _rightPressed = true;

        }
        _rightDown = state.RightButton == ButtonState.Pressed;
        if (_rightPressed)
        {
            PlayerInput.SetZoom_Unscaled();
            int mapWidth = Main.maxTilesX * 16;
            int mapHeight = Main.maxTilesY * 16;
            Vector2 cursorPosition = new Vector2(Main.mouseX, Main.mouseY);

            cursorPosition.X -= Main.screenWidth / 2f;
            cursorPosition.Y -= Main.screenHeight / 2f;

            Vector2 mapPosition = Main.mapFullscreenPos;
            Vector2 cursorWorldPosition = mapPosition;

            cursorPosition /= 16;
            cursorPosition *= 16 / Main.mapFullscreenScale;
            cursorWorldPosition += cursorPosition;
            cursorWorldPosition *= 16;

            Player player = Main.player[Main.myPlayer];
            cursorWorldPosition.Y -= player.height;
            cursorWorldPosition.X = MathHelper.Clamp(cursorWorldPosition.X, 0, mapWidth - player.width);
            cursorWorldPosition.Y = MathHelper.Clamp(cursorWorldPosition.Y, 0, mapHeight - player.height);
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                player.Teleport(cursorWorldPosition, 1, 0);
                player.position = cursorWorldPosition;
                player.velocity = Vector2.Zero;
                player.fallStart = (int)(player.position.Y / 16f);
            }
            else
            {
                player.Teleport(cursorWorldPosition, 6);
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, cursorWorldPosition.X, cursorWorldPosition.Y, 1);
            }
            PlayerInput.SetZoom_UI();
        }
    }
    public void PostDrawFullScreenMap(ref string mouseText)
    {
    }


}
