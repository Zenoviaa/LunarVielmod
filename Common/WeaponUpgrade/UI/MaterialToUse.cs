using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.WeaponUpgrade.UI
{
    /// <summary>
    /// This is the display for the material to use on the weapon upgrading ui, it also shows how much you need to upgrade the item.
    /// </summary>
    public class MaterialToUse : UIPanel
    {
        private UIText _text;
        private UIText _requiredText;
        private readonly Asset<Texture2D> _pearlTextureAsset;
        private readonly Asset<Texture2D> _materialBoxTextureAsset;
        public MaterialToUse()
        {
            //Load assets
            string pearlAssetPath = this.GetTypeDirectoryWithSlash() + "Pearl";
            string boxAssetPath = this.GetTypeDirectoryWithSlash() + "MaterialBox";

            _pearlTextureAsset = ModContent.Request<Texture2D>(pearlAssetPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            _materialBoxTextureAsset = ModContent.Request<Texture2D>(boxAssetPath, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            //Set width and height of the material to use
            Width.Set(_pearlTextureAsset.Width(), 0f);
            Height.Set(_pearlTextureAsset.Height(), 0f);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            _text = new UIText("0");
            _text.Left.Pixels = 32;
            _text.Top.Pixels = 32;
            _text.Width.Pixels = Width.Pixels;
            _text.Height.Pixels = Height.Pixels;
            _text.HAlign = 0.5f;
            Append(_text);

            _requiredText = new UIText("0");
            _requiredText.Left.Pixels = 0;
            _requiredText.Top.Pixels = 0;
            _requiredText.TextColor = Color.IndianRed;
            _requiredText.Width.Pixels = Width.Pixels;
            _requiredText.Height.Pixels = Height.Pixels;
            _requiredText.HAlign = 0.5f;
            Append(_requiredText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateUI();
        }


        /// <summary>
        /// Update the text UI of the weapon upgrade system
        /// </summary>
        private void UpdateUI()
        {
            //Unsure if there's a bettery way to do this but....
            //I'lll atleast make a helper method
            WeaponUpgradeUISystem weaponUpgradeSystem = ModContent.GetInstance<WeaponUpgradeUISystem>();
            long num6 = PlayerHelper.CountCurrencyInAllBanks(Main.LocalPlayer, Stellamod.DragonShardCurrencyID);
            _text.SetText(num6.ToString());

            int requiredAmount = weaponUpgradeSystem.RequiredAmount;
            _requiredText.SetText(requiredAmount.ToString());
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            //Mouse over raycasting
            this.QuickMouseInteraction();

            //Draw the background texture
            Rectangle rectangle = GetDimensions().ToRectangle();
            Texture2D background = _materialBoxTextureAsset.Value;
            Vector2 pos = rectangle.TopLeft();
            spriteBatch.Draw(background,pos, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            //Draw the required material texture
            WeaponUpgradeUISystem system = ModContent.GetInstance<WeaponUpgradeUISystem>();
            Asset<Texture2D> textureAsset = system.RequiredMaterialTexture;
            Texture2D textureToDraw = textureAsset.Value;
        

            spriteBatch.Draw(textureToDraw, pos + rectangle.Width * Vector2.UnitX * 2, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
    }
}