using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.WeaponUpgrade.UI
{
    [Autoload(Side = ModSide.Client)]
    internal class WeaponUpgradeUISystem : BaseUISystem
    {
        private Vector2 _worldPos;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public WeaponUpgradeUIState reforgeUIState;
        public static string RootTexturePath => "Stellamod/Core/WeaponUpgrade/UI/";

        public int RequiredAmount
        {
            get
            {
                if (ItemToUpgrade == null || ItemToUpgrade.IsAir)
                {
                    return 0;
                }
                else
                {
                    return ItemToUpgrade.GetGlobalItem<WeaponUpgradeGlobalItem>().GetUpgradeAmt();
                }
            }
        }
        public int RequiredMaterialType
        {
            get
            {
                if (ItemToUpgrade == null || ItemToUpgrade.IsAir)
                {
                    return ModContent.ItemType<LunarStone>();
                }
                else
                {
                    return ItemToUpgrade.GetGlobalItem<WeaponUpgradeGlobalItem>().GetMaterialType();
                }
            }
        }
        public Asset<Texture2D> RequiredMaterialTexture
        {
            get
            {
                if (ItemToUpgrade == null || ItemToUpgrade.IsAir)
                {
                    return ModContent.Request<Texture2D>(RootTexturePath + "NoMaterial");
                }
                else
                {
                    return TextureAssets.Item[RequiredMaterialType];
                }

            }
        }
        private Item ItemToUpgrade => reforgeUIState.ui.reforgeSlot.Item;
        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            reforgeUIState = new WeaponUpgradeUIState();
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
            if (ItemToUpgrade == null || ItemToUpgrade.IsAir)
                return false;

            Player player = Main.LocalPlayer;
            return ItemToUpgrade.GetGlobalItem<WeaponUpgradeGlobalItem>().CanUpgrade(ItemToUpgrade, player);
        }

        public void Reforge()
        {
            if (ItemToUpgrade == null || ItemToUpgrade.IsAir)
                return;

            Player player = Main.LocalPlayer;
            WeaponUpgradeGlobalItem upgradeGlobalItem = ItemToUpgrade.GetGlobalItem<WeaponUpgradeGlobalItem>();
            upgradeGlobalItem.Upgrade(ItemToUpgrade, player);
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
                    "Stellamod: Weapon Upgrade Damage UI",
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
