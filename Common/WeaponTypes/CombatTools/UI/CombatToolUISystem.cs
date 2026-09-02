using Stellamod.Common.ArmorShop.UI;
using Stellamod.Common.ClassReworkSystem;
using Stellamod.Common.UI;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.WeaponTypes.CombatTools.UI;

[Autoload(Side = ModSide.Client)]
public class CombatToolUISystem : BaseUISystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    private UserInterface _hudUserInterface;
    public BannerShop bannerShop;
    public CombatToolUIState slotUIState;
    public override int uiSlot => Slot_MajorUI;

    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        _hudUserInterface = new UserInterface();
        slotUIState = new();
        _hudUserInterface.SetState(null);
        BannerShopParameters bannerShopParameters = new BannerShopParameters();
        bannerShopParameters.AvailableItemsFunction = () =>
        {
            List<Item> itemList = new List<Item>();
            var items = ModContent.GetContent<ModItem>();
            foreach (var item in items)
            {
                if (item.Item.TryGetGlobalItem<CombatTool>(out var t))
                {
                    if (t.isCombatTool)
                    {
                        itemList.Add(new Item(item.Type));
                    }
                }
            }
            return itemList.ToArray();
        };

      //  armorBannerShopParameters.SlotTextureOverride = ModContent.Request<Texture2D>("Stellamod/Common/UI/Banner_ArmorShop");
        bannerShopParameters.SelectItemFunction = (Item item) =>
        {
            CombatToolPlayer combatToolPlayer = Main.LocalPlayer.GetModPlayer<CombatToolPlayer>();
            if (!combatToolPlayer.HasUnlocked(item))
                return;

            combatToolPlayer.SelectedTool = item;
            combatToolPlayer.SelectedTool.GetGlobalItem<CombatTool>().ammoCount = (int)(
            (float)combatToolPlayer.carryingCapacity * (float)combatToolPlayer.SelectedTool.GetGlobalItem<CombatTool>().maxAmmoCount);
        };

        bannerShopParameters.SelectedItemFunction = (Item item) =>
        {
            return Main.LocalPlayer.GetModPlayer<CombatToolPlayer>().SelectedTool.type == item.type;
        };

        bannerShopParameters.ViewItemFunction = (Item item) => Main.LocalPlayer.GetModPlayer<CombatToolPlayer>().HasUnlocked(item);
        bannerShopParameters.TitleKey = "CombatTool";
        bannerShopParameters.TooltipKey = "CombatToolHelp";
        bannerShop = new BannerShop(bannerShopParameters, CloseThis);
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
            bannerShop.shopMenuUIState.isOpen = false;
            /*
            SoundStyle soundStyle = SoundID.MenuClose;
            SoundEngine.PlaySound(soundStyle);
            CloseUI();*/
        }
        else
        {
            bannerShop.shopMenuUIState.isOpen = true;
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
        _userInterface.SetState(bannerShop.shopMenuUIState);
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
                "Stellamod: Combat Tool UI",
                delegate
                {
                    Player localPlayer = Main.LocalPlayer;
                    ClassReworkPlayer reworkPlayer = localPlayer.GetModPlayer<ClassReworkPlayer>();
                    if (reworkPlayer.playerClass != PlayerClass.Ranger && reworkPlayer.playerClass != PlayerClass.God && reworkPlayer.playerClass != PlayerClass.Omni)
                        return true;
                    if (_lastUpdateUiGameTime != null && _hudUserInterface?.CurrentState != null)
                    {
                        _hudUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                    }
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

