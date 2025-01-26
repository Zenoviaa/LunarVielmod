using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.ToolsSystem
{
    internal class TilePainterSlot : UIElement
    {
        private Color DrawColor;
        public ModTile ModTile { get; private set; }

        public ModWall ModWall { get; private set; }
        public TilePainterSlot(int tileType = -1, int wallType = -1) : base()
        {
            ModTile = ModContent.GetModTile(tileType);
            ModWall = ModContent.GetModWall(wallType);
        }
        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Pixels = 64;
            Height.Pixels = 64;
            OnLeftClick += OnButtonClick;
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // We can do stuff in here!
            if (ModTile != null)
            {
                Main.LocalPlayer.GetModPlayer<TilePainterPlayer>().SelectTile(ModTile);
            }
            else if (ModWall != null)
            {
                Main.LocalPlayer.GetModPlayer<TilePainterPlayer>().SelectWall(ModWall);
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Asset<Texture2D> texture = null;
            if (ModTile != null)
            {
                texture = ModContent.Request<Texture2D>(ModTile.Texture);

            }
            else if (ModWall != null)
            {
                texture = ModContent.Request<Texture2D>(ModWall.Texture);

            }
            int w = (int)Width.Pixels;
            int h = (int)Height.Pixels;
            Rectangle sourceRect = new Rectangle(0, 0, w, h);

            if (IsMouseHovering)
            {
                DrawColor = Color.Lerp(DrawColor, Color.Goldenrod, 0.1f);
                string name = ModWall != null ? ModWall.Name : string.Empty;
                if (string.IsNullOrEmpty(name))
                    name = ModTile != null ? ModTile.Name : string.Empty;
                Player player = Main.LocalPlayer;
                player.cursorItemIconID = -1;
                player.cursorItemIconText = name;
                player.cursorItemIconEnabled = true;
            }
            else
            {
                DrawColor = Color.Lerp(DrawColor, Color.White, 0.1f);
            }
            spriteBatch.Draw(texture.Value, new Rectangle(point.X, point.Y, w, h), sourceRect, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
