using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.ArmorRework
{
    [Autoload(Side = ModSide.Client)]
    public class ArmorTooltipSystem : ModSystem
    {
        private float _timer;
        private float _alpha;
        private bool _active;
        private UserInterface _userInterface;
        private GameTime _lastUpdateUiGameTime;
        private ArmorTooltipUIState _uiState;
        private float EaseTime => 0.4f;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _uiState = new();
            _uiState.Activate();
            On_Main.DrawInterface_33_MouseText += DisableMouseText;
        }

        private void DisableMouseText(On_Main.orig_DrawInterface_33_MouseText orig, Main self)
        {
            if (_timer > 0)
                return;
            orig(self);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawInterface_33_MouseText -= DisableMouseText;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            _timer += (float)(gameTime.ElapsedGameTime.TotalSeconds * (_active ? 1 : -1));
            if(_timer >= EaseTime)
            {
                _timer = EaseTime;
            }
            if (_timer <= 0f)
                _timer = 0f;
            _active = false;
            _alpha = EasingFunction.InOutSine(_timer / EaseTime);


            _uiState.inspectorUI.alpha = _alpha;
            _uiState.inspectorUI.previewUI.alpha = _alpha;
            _uiState.inspectorUI.loreUI.alpha = _alpha;
            _uiState.inspectorUI.summaryUI.alpha = _alpha;
            _userInterface?.Update(gameTime);
            _lastUpdateUiGameTime = gameTime;
        }

        public void InspectArmor(Item item, string lore, string setBonus, List<TooltipLine> stats)
        {
            _active = true;



            _uiState.inspectorUI.previewUI.SetArmorSet(item);
            _uiState.inspectorUI.loreUI.SetText(lore);

            ArmorSet set = ArmorSetSystem.FindArmorSet(item.type);
            ArmorSetSystem.GetArmorSet(set, out Item helm, out Item armor, out Item leggings);
            Player player = Main.LocalPlayer;
            bool isActive = player.armor[0].type == helm.type && player.armor[1].type == armor.type && player.armor[2].type == leggings.type;
            bool isActive2 = player.armor[0].type == helm.type && player.armor[1].type == armor.type && player.armor[2].IsAir;
            _uiState.inspectorUI.summaryUI.SetTooltips(stats, setBonus, isActive || isActive2);
            if (_userInterface.CurrentState == null)
                OpenUI();
        }

        public void OpenUI()
        {
            _userInterface.SetState(_uiState);
        }

        public void CloseUI()
        {
            _userInterface.SetState(null);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Item / NPC Head"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Armor Rework",
                    delegate
                    {
                        if (_timer <= 0.1f)
                            return true;
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
