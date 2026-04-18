using ReLogic.Content;
using Stellamod.Content.Items.Materials;
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
        private UIScaler _uiScaler;
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
        public float easeInTime => 60f;
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
                            float scale = 1f / Main.UIScale;
                            Main.screenWidth = (int)((float)PlayerInput.RealScreenWidth * scale);
                            Main.screenHeight = (int)((float)PlayerInput.RealScreenHeight * scale);

                            Vector2 pivot = new Vector2();
                            pivot.X = reforgeUIState.ui.RelativeLeft;
                            pivot.Y = reforgeUIState.ui.RelativeTop;
                            _uiScaler.adjustedOffset =pivot + new Vector2(reforgeUIState.ui.Width.Pixels, reforgeUIState.ui.Height.Pixels) * 0.5f;
                            _uiScaler.adjustedUIScale = Vector2.One * EasingFunction.OutExpo(_inRatio);
                            Main.spriteBatch.End();
                            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                                _uiScaler.UIScaleMatrix2());
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                            Main.spriteBatch.End();
                            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer);

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
    public Vector2 adjustedOffset;
    public UIScaler()
    {
        adjustedUIScale = Vector2.One;
        adjustedOffset = Vector2.Zero;
    }

    public Matrix UIScaleMatrix()
    {
        Vector2 uiScale = Vector2.One * Main.UIScale;
        uiScale *= adjustedUIScale;
        Matrix scaleMatrix = Matrix.CreateScale(uiScale.X, uiScale.X, 1f);
        Vector2 offset = new Vector2();

        float adjustedScreenWidth = Main.screenWidth * Main.UIScale;
        float adjustedScreenHeight = Main.screenHeight * Main.UIScale;
        offset.X = adjustedScreenWidth * 0.5f;
        offset.Y = adjustedScreenHeight * 0.5f;

        offset.X *= MathF.Abs( adjustedUIScale.X-1f);
        offset.Y *= MathF.Abs(adjustedUIScale.Y-1f);

        if (adjustedUIScale.X > 1)
            offset.X *= -1f;
        if (adjustedUIScale.Y > 1)
            offset.Y *= -1f;
        Matrix translationMatrix = Matrix.CreateTranslation(offset.X, offset.Y, 0f);
        scaleMatrix *= translationMatrix;
        return scaleMatrix;
    }
    public Matrix UIScaleMatrix2()
    {
        Vector2 uiScale = Vector2.One * Main.UIScale;
        uiScale *= adjustedUIScale;
        Matrix scaleMatrix = Matrix.CreateScale(uiScale.X, uiScale.X, 1f);
        Vector2 offset = adjustedOffset;
        Matrix translationMatrix = Matrix.CreateTranslation(-offset.X, -offset.Y, 0f);
        Matrix finalMatrix = translationMatrix * scaleMatrix * Matrix.CreateTranslation(offset.X, offset.Y, 0f);
        return finalMatrix;
    }
}