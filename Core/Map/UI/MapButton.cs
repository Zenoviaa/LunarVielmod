using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.TriggerTiles;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.Map.UI
{
    internal class MapButton : UIPanel
    {
        private static MapButton _hoverTarget;
        private static bool _hasDecreasedHoverTimer;
        private readonly string _localizationKey;
        private readonly MapUI _mapUI;
        private readonly BaseMapMarker _marker;
        public string Texture => (GetType().Namespace + "." + "MapPin").Replace('.', '/');
        public Asset<Texture2D> TextureAsset;
        public MapButton(string localizationKey, MapUI mapUI, ModWall marker) : base()
        {
            _marker = marker as BaseMapMarker;
            _mapUI = mapUI;
            _localizationKey = localizationKey;
            TextureAsset = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad);
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
            OnMouseOut += OnMouseHoverOut;
        }

        public float HoverProgress { get; set; }
        public Color HoverColor { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            BackgroundColor = Color.Blue * 0.5f;
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // We can do stuff in here!
            if (_marker.CanTeleport())
            {
                _marker.Teleport();
                MapUISystem uiSystem = ModContent.GetInstance<MapUISystem>();
                uiSystem.CloseThis();
                Main.playerInventory = false;
            }
        }
        private void OnMouseHoverOut(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!_marker.CanTeleport())
                return;
            _hoverTarget = this;
            _mapUI.MapMarker.Left.Pixels = Left.Pixels + Width.Pixels / 2 - TextureAsset.Width() / 2 - 2f;
            _mapUI.MapMarker.Top.Pixels = Top.Pixels + Height.Pixels / 2 - TextureAsset.Height() / 2 - 16;
            _mapUI.AreaPreview.Title.SetText(LangText.Map(_localizationKey, "DisplayName"));
            _mapUI.AreaPreview.Text.SetText(LangText.Map(_localizationKey, "Description"));
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_marker.CanTeleport())
            {
                if (_hoverTarget == this && IsMouseHovering)
                {
                    _mapUI.HoverTimer = 15;
                }
            }


            if (IsMouseHovering)
            {
                HoverColor = Color.Lerp(HoverColor, Color.Goldenrod, 0.1f);
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Yellow * 0.5f, 0.1f);
            }
            else
            {
                HoverColor = Color.Lerp(HoverColor, Color.Transparent, 0.1f);
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Blue * 0.5f, 0.1f);
            }

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            //Comment this out if you wanna see the hitbox locations
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (!_marker.CanTeleport())
                return;

            Rectangle rectangle = GetDimensions().ToRectangle();

            //Draw Backing
            Color color2 = Color.White;
            Vector2 pos = rectangle.TopLeft();
            spriteBatch.Draw(TextureAsset.Value, rectangle.TopLeft() + new Vector2(Width.Pixels / 2, Height.Pixels / 2) - new Vector2(TextureAsset.Value.Width, TextureAsset.Value.Height) / 2, null, color2, 0f, default, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            Color glowColor = HoverColor;
            spriteBatch.Draw(TextureAsset.Value, rectangle.TopLeft() + new Vector2(Width.Pixels / 2, Height.Pixels / 2) - new Vector2(TextureAsset.Value.Width, TextureAsset.Value.Height) / 2, null, color2, 0f, default, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
        }
    }
}
