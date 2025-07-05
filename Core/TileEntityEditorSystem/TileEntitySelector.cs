using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TileEntityEditorSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class TileEntitySelector : ModSystem
    {
        private BaseModTileEditorUIState _state;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public static bool SaveNClose;
        public static bool IsEditing => TargetTileEntity != null;
        public static DraggablePointField DraggablePoint = null;
        public static Point16 TargetTileEntityPoint;
        public static ModTileEntity TargetTileEntity
        {
            get
            {
                if (TileEntity.ByPosition.ContainsKey(TargetTileEntityPoint))
                {
                    return TileEntity.ByPosition[TargetTileEntityPoint] as ModTileEntity;
                }
                return null;
            }
        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
            if (SaveNClose)
            {
                CloseUI();
            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            EditModTileEntity();
            EditDraggablePos();
        }

        private void EditDraggablePos()
        {
            if (DraggablePoint == null)
                return;
            Player player = Main.LocalPlayer;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DraggablePoint>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), DraggablePoint.Point.ToWorldCoordinates(), Vector2.Zero,
                    ModContent.ProjectileType<DraggablePoint>(), 1, 1, player.whoAmI);
            }
        }

        private void EditModTileEntity()
        {
            if (!IsEditing)
                return;

        }

        internal void OpenUI(BaseModTileEditorUIState uiState)
        {
            //Set State
            _state = uiState;
            _state.Open();
            _userInterface.SetState(uiState);
        }

        internal void CloseUI()
        {
            _state.Close();
            _state = null;
            _userInterface.SetState(null);
            SaveNClose = false;
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
                    "Urdveil: Tile Entity Editor UI",
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
