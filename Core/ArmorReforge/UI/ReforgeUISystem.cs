using Microsoft.Xna.Framework;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Helpers;
using Stellamod.Core.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.ArmorReforge.UI
{
    [Autoload(Side = ModSide.Client)]
    internal class ReforgeUISystem : BaseUISystem
    {
        private Vector2 _worldPos;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public ReforgeUIState reforgeUIState;
        public static string RootTexturePath => "Stellamod/Core/ArmorReforge/UI/";


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

            if (!Main.playerInventory && _userInterface.CurrentState != null || Main.npcShop == 1)
            {
                CloseUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
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

        public bool CanReforge()
        {
            Player player = Main.LocalPlayer;
            if (player.HasItem(ModContent.ItemType<GlisteningPearl>()))
            {
                return true;
            }
            return false;
        }

        public void Reforge()
        {
            Player player = Main.LocalPlayer;
            List<ArmorReforgeType> armorReforges = GeneralHelpers.GetEnumList<ArmorReforgeType>();
            ArmorReforgeType chosenReforge = armorReforges[Main.rand.Next(0, armorReforges.Count)];
            Item item = reforgeUIState.ui.reforgeSlot.Item;

            //Can't reforge nothing
            if (item == null)
                return;


            player.RemoveItem(ModContent.ItemType<GlisteningPearl>(), 1);
            ArmorReforgeGlobalItem armorReforgeGlobalItem = item.GetGlobalItem<ArmorReforgeGlobalItem>();
            armorReforgeGlobalItem.reforgeType = chosenReforge;
            item.NetStateChanged();

            string text = LangText.ArmorReforge(chosenReforge, "DisplayName") + " " + item.Name;
            int combatText = CombatText.NewText(player.getRect(), Color.White, text);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 60;
        }

        internal void OpenUI()
        {
            //Set State
            _worldPos = Main.LocalPlayer.position;
            _userInterface.SetState(reforgeUIState);
        }

        internal void CloseUI()
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
                    "Urdveil: Reforge UI",
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
