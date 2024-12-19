using Microsoft.Xna.Framework;
using Stellamod.Items;
using Stellamod.NPCs.Town;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CellConverterSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class CellConverterUISystem : BaseUISystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public static string RootTexturePath => "Stellamod/UI/CellConverterSystem/";

        public ConverterUIState converterUIState;
        public int CellConverterX;
        public int CellConverterY;
        public Vector2 CellConverterPos;
        public override int uiSlot => Slot_MinorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            converterUIState = new ConverterUIState();
            converterUIState.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Vector2 worldPos = new Vector2(CellConverterX * 16, CellConverterY * 16);
            float dist = Vector2.Distance(Main.LocalPlayer.position, CellConverterPos);
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

        internal void OpenUI()
        {
            //Set State
            TakeSlot();
            _userInterface.SetState(converterUIState);
        }

        internal void CloseUI()
        {
            ClearSlot();
            Item mold = converterUIState.converterUI.convertSlot.Item;
            if (!mold.IsAir)
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), mold, mold.stack);
                converterUIState.converterUI.convertSlot.Item = new Item();
                converterUIState.converterUI.convertSlot.Item.SetDefaults(0);
            }
            _userInterface.SetState(null);
        }

        public bool CanSwap()
        {
            Item material = converterUIState.converterUI.convertSlot.Item;
            if (material.ModItem is ClassSwapItem item)
            {
                if (item.IsSwapped)
                    return false;
                else
                    return true;
            }

            if (material.IsAir)
            {
                return false;
            }

            return false;
        }

        public void CellConvert()
        {
            //Add the result to the inventory
            Player player = Main.LocalPlayer;
            Item item = converterUIState.converterUI.convertSlot.Item;
            if (item.ModItem is ClassSwapItem swapItem)
            {
                swapItem.SwapDamageType();
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Converted"));
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Main.LocalPlayer.Center, 1024f, 16f);
                item.NetStateChanged();
            }
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                CloseUI();
                _userInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Cell Converter UI",
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
