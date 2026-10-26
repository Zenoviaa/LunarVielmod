using Microsoft.Xna.Framework;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.PowderSystem;

[Autoload(Side = ModSide.Client)]
public class PowderUISystem : BaseUISystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    public PowderUIState powderUIState;
    public BaseIgniterCard Card { get; set; }
    public override int uiSlot => Slot_MinorUI;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        powderUIState = new PowderUIState();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (!Main.playerInventory && _userInterface.CurrentState != null)
        {
            CloseUI();
        }

        _lastUpdateUiGameTime = gameTime;
        if (_userInterface?.CurrentState != null)
        {

            _userInterface.Update(gameTime);
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
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }

    public void OpenUI()
    {
        //Set State
        TakeSlot();
        powderUIState.powderUI.OpenUI(Card);
        _userInterface.SetState(powderUIState);
    }

    public void CloseUI()
    {
        ClearSlot();
        _userInterface.SetState(null);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "LunarVeil: Powder UI",
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