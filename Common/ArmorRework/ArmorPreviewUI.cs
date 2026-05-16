using Stellamod.Common.Shaders;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.ArmorRework
{
    /// <summary>
    /// Creates a cute little preview of the character for the armor UI
    /// </summary>
    public class ArmorPreviewUI : UIPanel
    {
        private Item _item;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 128;
            Height.Pixels = 128;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }
        public float alpha;
        public void SetArmorSet(Item item)
        {
            _item = item;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (_item == null)
                return;
            if (_item.IsAir)
                return;

            Vector2 position = GetDimensions().ToRectangle().TopLeft();
            ArmorSet set = ArmorSetSystem.FindArmorSet(_item);
            ArmorSetSystem.GetArmorSet(set, out Item helm, out Item armor, out Item leggings);

            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels);
            Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);


            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.graphics.GraphicsDevice.SamplerStates[0], default, Main.Rasterizer, SpriteWhiteShader.Instance.Effect, Main.UIScaleMatrix);


            Vector2 size = FontAssets.MouseText.Value.MeasureString(_item.Name);
            float xOffset = (rectangle.Width / 2f) - size.X / 2f;

            Vector2 armorIconPosition = position + new Vector2(-24, 0);// + new Vector2(-size.X / 2f, 0);
            //Step 3. Draw item icon of the current item
            Vector2 topRight = position;
            topRight.X += Width.Pixels * 1f; 

            for (float f = 0; f < 4f; f++)
            {
                Color outlineColor = Color.White;
                outlineColor *= (int)ExtraMath.Osc(0f, 2f, speed: 3);
                ItemSlot.DrawItemIcon(_item, 0, spriteBatch, armorIconPosition + (Vector2.UnitY * 2).RotatedBy(f / 4f * MathHelper.TwoPi), 1, 32, outlineColor * alpha);
            }
   
                
            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.graphics.GraphicsDevice.SamplerStates[0], default, Main.Rasterizer, null, Main.UIScaleMatrix);



         
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, _item.Name,
               rectangle.TopLeft() + new Vector2(xOffset, 0), Color.White * alpha, 0, Vector2.Zero, Vector2.One);
            ItemSlot.DrawItemIcon(_item, 0, spriteBatch, armorIconPosition, 1, 32, Color.White * alpha);


            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, Main.Rasterizer, default, Main.UIScaleMatrix);

            Vector2 playerPosition = position + new Vector2(Width.Pixels, Height.Pixels) * 0.5f;
            playerPosition.Y -= Main.LocalPlayer.height / 2;
            playerPosition.Y += 20;
            ExpandableTooltip.DrawArmorPreview(playerPosition, helm, armor, leggings);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, Main.Rasterizer, default, Main.UIScaleMatrix);
        }
    }
}
