using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorShop;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Audio;

namespace Stellamod.UI.ArmorShopSystem
{
    internal class ArmorShopUISystem : ModSystem
    {
        private Vector2 _worldPos;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public ArmorShopUIState armorShopUIState;
        public static string RootTexturePath => "Stellamod/UI/ArmorShopSystem/";

        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            armorShopUIState = new ArmorShopUIState();
            armorShopUIState.Activate();
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

        public bool CanPurchase(ArmorShopSet armorSet)
        {


            Player player = Main.LocalPlayer;
            ArmorShopPlayer shopPlayer = player.GetModPlayer<ArmorShopPlayer>();
            if (armorSet.HasPurchased())
            {
                return true;
            }
            else
            {
                if (player.CountItem(armorSet.material.type) >= armorSet.material.stack)
                    return true;
            }
  
            return false;
        }

        public void Purchase(ArmorShopSet armorSet)
        {
            Player player = Main.LocalPlayer;
            if (!armorSet.HasPurchased())
            {
                player.RemoveItem(armorSet.material.type, armorSet.material.stack);
            }
  
            armorSet.QuickSpawn(player);
            SoundEngine.PlaySound(SoundID.Coins);
        }

        internal void OpenUI()
        {
            //Set State
            _worldPos = Main.LocalPlayer.position;
            _userInterface.SetState(armorShopUIState);
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
                    "LunarVeil: Armor Shop UI",
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