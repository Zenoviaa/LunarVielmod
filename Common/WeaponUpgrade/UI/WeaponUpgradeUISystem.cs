using ReLogic.Content;
using Stellamod.Common.UI;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.WeaponUpgrade.UI
{
    [Autoload(Side = ModSide.Client)]
    public class WeaponUpgradeUISystem : BaseUISystem
    {
        private float _inRatio;
        private float _inTimer;
        private bool _isClosing;

        private Vector2 _worldPos;
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public WeaponUpgradeUIState reforgeUIState;
        public static string RootTexturePath => typeof(WeaponUpgradeUISystem).DirectoryHere() + "/";
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
                return ModContent.ItemType<DragonShard>();
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
        public static float ForgeGlow;
        public float easeInTime => 30f;
        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            reforgeUIState = new WeaponUpgradeUIState();
            reforgeUIState.Activate();
            On_Main.CheckMonoliths += RenderUI;
        }

        private void RenderUI(On_Main.orig_CheckMonoliths orig)
        {
            if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
            {
                PlayerInput.SetZoom_UI();
                Main.spriteBatch.GraphicsDevice.SetRenderTarget(UITarget);
                Main.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                        Main.UIScaleMatrix);

                _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                Main.spriteBatch.End();
                PlayerInput.SetZoom_World();
            }

            orig();
        }

        public RenderTarget2D UITarget => ModContent.GetInstance<UIRenderTargets>().uiTarget;
        public override void UpdateUI(GameTime gameTime)
        {
            ForgeGlow = MathHelper.Lerp(ForgeGlow, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * 3);
            if (_isClosing)
            {
                _inTimer--;
                if (_inTimer <= 0)
                {
                    _inTimer = 0;
                    CloseUI();
                    _isClosing = false;
                }
            }
            else if (_userInterface.CurrentState != null)
            {
                _inTimer++;

            }
          

            _inTimer = MathHelper.Clamp(_inTimer, 0f, easeInTime);
            _inRatio = _inTimer / easeInTime;


            if(!_isClosing && _userInterface.CurrentState != null)
            {
                float dist = Vector2.Distance(Main.LocalPlayer.position, _worldPos);
                if (dist > 160)
                {
                    _isClosing = true;
                }

                if ((!Main.playerInventory) || (Main.npcShop == 1))
                {
                    _isClosing = true;
                }
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

        public void OpenUI()
        {
            //Set State
            _isClosing = false;
            _worldPos = Main.LocalPlayer.position;
            _userInterface.SetState(reforgeUIState);
        }

        public void CloseUI()
        {
            if (!ItemToUpgrade.IsAir)
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ItemToUpgrade);
                ItemToUpgrade.TurnToAir();
            }

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

                            SpriteBatch spriteBatch = Main.spriteBatch;
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);

     
                            Vector2 offset = Vector2.Lerp(-Vector2.UnitX * 100, Vector2.Zero, EasingFunction.OutCirc(_inRatio));
                            Color color = Color.Lerp(Color.Transparent, Color.White, _inRatio);
                            spriteBatch.Draw(UITarget, offset + UITarget.Size() * 0.5f, null, color, 0, UITarget.Size() * 0.5f, MathHelper.Lerp(0f, 1f, EasingFunction.OutCirc(_inRatio)), SpriteEffects.None, 0);

                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}



/// <summary>
/// Creates a scale and offset for UI to create cool animations :) 
/// </summary>
public struct UIScaler
{
    public Vector2 adjustedUIScale;
    public Vector2 scalePivot;
    public UIScaler()
    {
        adjustedUIScale = Vector2.One;
        scalePivot = Vector2.Zero;
    }

    public Matrix UIScaleMatrix()
    {
        Vector2 uiScale = Vector2.One * Main.UIScale;
        uiScale *= adjustedUIScale;
        Matrix scaleMatrix = Matrix.CreateScale(uiScale.X, uiScale.X, 1f);
        Vector2 offset = scalePivot;
        Matrix translationMatrix = Matrix.CreateTranslation(-offset.X, -offset.Y, 0f);
        Matrix finalMatrix = translationMatrix * scaleMatrix * Matrix.CreateTranslation(offset.X, offset.Y, 0f);
        return finalMatrix;
    }
}