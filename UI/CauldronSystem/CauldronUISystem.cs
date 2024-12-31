using Microsoft.Xna.Framework;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CauldronSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class CauldronUISystem : BaseUISystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _cauldronInterface;
        public CauldronUIState cauldronUIState;
        public static string RootTexturePath => "Stellamod/UI/CauldronSystem/";
        public int CauldronX;
        public int CauldronY;
        public Vector2 CauldronPos;

        public override int uiSlot => Slot_MinorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _cauldronInterface = new UserInterface();
            cauldronUIState = new CauldronUIState();
            cauldronUIState.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Vector2 worldPos = new Vector2(CauldronX * 16, CauldronY * 16);
            float dist = Vector2.Distance(Main.LocalPlayer.position, CauldronPos);
            if(dist > 160)
            {
                CloseUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_cauldronInterface?.CurrentState != null)
            {
               
                _cauldronInterface.Update(gameTime);
            }
        }

        public override void CloseThis()
        {
            base.CloseThis();
            CloseUI();
        }

        internal void ToggleUI()
        {
            if (_cauldronInterface.CurrentState != null)
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
            _cauldronInterface.SetState(cauldronUIState);
        }

        internal void CloseUI()
        {
            ClearSlot();
            _cauldronInterface.SetState(null);
            Item mold = cauldronUIState.cauldronUI.moldSlot.Item;
            Item material = cauldronUIState.cauldronUI.materialSlot.Item;
            if (!mold.IsAir)
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), mold, mold.stack);
                cauldronUIState.cauldronUI.moldSlot.Item = new Item();
                cauldronUIState.cauldronUI.moldSlot.Item.SetDefaults(0);
            }
            if (!material.IsAir)
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), material, material.stack);
                cauldronUIState.cauldronUI.materialSlot.Item = new Item();
                cauldronUIState.cauldronUI.materialSlot.Item.SetDefaults(0);
            }
        }

        public bool CanCraft()
        {
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();
            Item material = cauldronUIState.cauldronUI.materialSlot.Item;
            return material.stack >= 10;
        }

        public void Craft()
        {
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();

            Item mold = cauldronUIState.cauldronUI.moldSlot.Item;
            Item material = cauldronUIState.cauldronUI.materialSlot.Item;
            var result = cauldron.Craft(mold, material);
            if (result.result == -1)
                return;

            //Add the result to the inventory
            Player player = Main.LocalPlayer;
            int item = player.QuickSpawnItem(player.GetSource_FromThis(), result.result, result.yield);
            

            if (mold.IsAir)
            {
                cauldronUIState.cauldronUI.moldSlot.Item = new Item();
                cauldronUIState.cauldronUI.moldSlot.Item.SetDefaults(0);
            }
            if (material.IsAir)
            {
                cauldronUIState.cauldronUI.materialSlot.Item = new Item();
                cauldronUIState.cauldronUI.materialSlot.Item.SetDefaults(0);
            }
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_cauldronInterface.CurrentState != null)
            {
                _cauldronInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Cauldron UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _cauldronInterface?.CurrentState != null)
                        {
                            _cauldronInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
