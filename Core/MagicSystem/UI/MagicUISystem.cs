using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.MagicSystem.UI
{
    [Autoload(Side = ModSide.Client)]
    internal class MagicUISystem : ModSystem
    {
        private UserInterface _staffInterface;
        private GameTime _lastUpdateUiGameTime;
        public EnchantmentMenuUIState enchantmentMenuUI;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _staffInterface = new UserInterface();
            enchantmentMenuUI = new EnchantmentMenuUIState();
            enchantmentMenuUI.Activate();
        }

        internal void OpenUI(Staff staff)
        {
            //Create a new editing context
            StaffEditingContext ctx = new StaffEditingContext(staff);
            EnchantmentMenu menu = enchantmentMenuUI.enchantmentMenu;
            menu.UseContext(ctx);

            //Set the state of the interface.
            _staffInterface.SetState(enchantmentMenuUI);
        }
        internal void EmptyUI()
        {
            //throw new NotImplementedException();
            Item item = new Item(ModContent.ItemType<NoStaff>());
            var noStaff = item.ModItem as Staff;
            StaffEditingContext ctx = new StaffEditingContext(noStaff);
            EnchantmentMenu menu = enchantmentMenuUI.enchantmentMenu;
            menu.UseContext(ctx);

        }

        internal void CloseUI()
        {
            _staffInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Main.playerInventory && _staffInterface?.CurrentState != null)
            {
                CloseUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_staffInterface?.CurrentState != null)
            {
                _staffInterface.Update(gameTime);
            }
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_staffInterface.CurrentState != null)
            {
                //   RenamePetUI.saveItemInUI = true;
                _staffInterface.SetState(null);
            }
        }


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Magic UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _staffInterface?.CurrentState != null)
                        {
                            _staffInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

    }
}
