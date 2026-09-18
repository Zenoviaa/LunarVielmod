using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

[Autoload(Side = ModSide.Client)]
public class AmmoToolUISystem : BaseUISystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    private UserInterface _hudUserInterface;
    public static string RootTexturePath => typeof(AmmoToolUISystem).DirectoryHere() + "/";

    public AmmoToolMenuUIState menuUIState;
    public AmmoToolUIState slotUIState;
    public override int uiSlot => Slot_MajorUI;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        _hudUserInterface = new UserInterface();
        menuUIState = new();
        menuUIState.Activate();
        slotUIState = new();
        slotUIState.Activate();

        _hudUserInterface.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        //Close if inventory isn't open lol
        if (_hudUserInterface.CurrentState == null)
        {
            OpenHudUI();
        }

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
            menuUIState.isOpen = false;
            /*
            SoundStyle soundStyle = SoundID.MenuClose;
            SoundEngine.PlaySound(soundStyle);
            CloseUI();*/
        }
        else
        {
            menuUIState.isOpen = true;
            SoundStyle soundStyle = SoundID.MenuOpen;
            SoundEngine.PlaySound(soundStyle);
            OpenUI();
        }
    }
    public void OpenHudUI()
    {
        _hudUserInterface.SetState(slotUIState);
    }

    public void CloseHudUI()
    {
        _hudUserInterface.SetState(null);
    }
    public void OpenUI()
    {
        //Set State
        TakeSlot();
        _userInterface.SetState(menuUIState);
    }

    public void CloseUI()
    {
        ClearSlot();
        _userInterface.SetState(null);
    }

    public override void PreSaveAndQuit()
    {
        //Calls Deactivate and drops the item
        if (_userInterface.CurrentState != null)
        {
            CloseUI();
            _userInterface.SetState(null);
        }
        if (_hudUserInterface.CurrentState != null)
        {
            CloseHudUI();
            _hudUserInterface.SetState(null);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Stellamod: Ammo Tool UI",
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
        //Interface Logic 3
        mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Stellamod: Ammo Tool UI",
                delegate
                {
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
