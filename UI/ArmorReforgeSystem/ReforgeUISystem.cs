using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorReforge;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.ArmorReforgeSystem
{
    [Autoload(Side = ModSide.Client)]
    public class ReforgeUISystem : BaseUISystem
    {
        private Vector2 _worldPos;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public ReforgeUIState reforgeUIState;
        public static string RootTexturePath => "Stellamod/UI/ArmorReforgeSystem/";


        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            reforgeUIState = new ReforgeUIState();
            reforgeUIState.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            float dist = Vector2.Distance(Main.LocalPlayer.position, _worldPos);
            if (dist > 160)
            {
                CloseUI();
            }

            if ((!Main.playerInventory && _userInterface.CurrentState != null) || (Main.npcShop == 1))
            {
                CloseUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
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

        public bool CanReforge()
        {
            Player player = Main.LocalPlayer;
            if (player.HasItem(ModContent.ItemType<GlisteningPearl>()))
            {
                return true;
            }
            return false;
        }

        public void ReforgeArmor(Player player, Item item)
        {
            List<ArmorReforgeType> armorReforges = GeneralHelpers.GetEnumList<ArmorReforgeType>();
            ArmorReforgeType chosenReforge = armorReforges[Main.rand.Next(0, armorReforges.Count)];
            //Don't ever reforge to none
            while (chosenReforge == ArmorReforgeType.None)
                chosenReforge = armorReforges[Main.rand.Next(0, armorReforges.Count)];

            player.RemoveItem(ModContent.ItemType<GlisteningPearl>(), 1);
            ArmorReforgeGlobalItem armorReforgeGlobalItem = item.GetGlobalItem<ArmorReforgeGlobalItem>();
            armorReforgeGlobalItem.reforgeType = chosenReforge;
            item.NetStateChanged();
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Converted"));

            FXUtil.ShakeCamera(Main.LocalPlayer.Center, 1024, 16f);
            string text = LangText.ArmorReforge(chosenReforge, "DisplayName") + " " + item.Name;
            int combatText = CombatText.NewText(player.getRect(), Color.White, text);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 60;
        }


        public void ReforgeAccessory(Player player , Item item)
        {
            List<AccessoryReforgeType> reforges = GeneralHelpers.GetEnumList<AccessoryReforgeType>();
            AccessoryReforgeType chosenReforge = reforges[Main.rand.Next(0, reforges.Count)];
            //Don't ever reforge to none
            while (chosenReforge == AccessoryReforgeType.None)
                chosenReforge = reforges[Main.rand.Next(0, reforges.Count)];

            player.RemoveItem(ModContent.ItemType<GlisteningPearl>(), 1);
            AccessoryReforgeGlobalItem armorReforgeGlobalItem = item.GetGlobalItem<AccessoryReforgeGlobalItem>();
            armorReforgeGlobalItem.accessoryReforgeType = chosenReforge;
            item.NetStateChanged();
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Converted"));

            FXUtil.ShakeCamera(Main.LocalPlayer.Center, 1024, 16f);
            string text = LangText.AccessoryReforge(chosenReforge, "DisplayName") + " " + item.Name;
            int combatText = CombatText.NewText(player.getRect(), Color.White, text);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 60;
        }
        public void Reforge()
        {
            Player player = Main.LocalPlayer;

            Item item = reforgeUIState.ui.reforgeSlot.Item;

            //Can't reforge nothing
            if (item == null)
                return;
            //Can't reforge air
            if (item.IsAir)
                return;

            if (item.accessory)
            {
                ReforgeAccessory(player, item);
            }
            else
            {
                ReforgeArmor(player, item); 
            }
        }

        public void OpenUI()
        {
            //Set State
            _worldPos = Main.LocalPlayer.position;
            _userInterface.SetState(reforgeUIState);
        }

        public void CloseUI()
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
                    "LunarVeil: Reforge UI",
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
