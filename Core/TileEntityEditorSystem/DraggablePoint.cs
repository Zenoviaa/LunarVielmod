using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal class DraggablePoint : ModProjectile
    {
        private static bool _capturedMouse;
        private bool _isDragging;
        private Rectangle Rectangle
        {
            get
            {
                Rectangle rectangle = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                return rectangle;
            }
        }
        private bool IsMouseHovering
        {
            get
            {
                if (Rectangle.Contains(Main.MouseWorld.ToPoint()))
                {
                    return true;
                }
                return false;
            }
        }
        private bool IsDragging
        {
            get
            {
                return IsMouseHovering && Main.mouseLeft;
            }
        }
        private ref float Timer => ref Projectile.ai[1];

        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            base.AI();
            Player owner = Main.player[Projectile.owner];
            Timer++;
            if (Timer == 1)
            {
                _capturedMouse = false;
                _isDragging = false;
            }

            if (IsMouseHovering && Main.mouseLeft && !_capturedMouse)
            {
                _isDragging = true;
                _capturedMouse = true;
            }
            if (_isDragging && !Main.mouseLeft)
            {
                _isDragging = false;
                _capturedMouse = false;
            }



            if (TileEntitySelector.DraggablePoint != null)
            {
                Projectile.timeLeft = 8;
            }
            else
            {
                return;
            }

            if (_isDragging)
            {
                int x = (int)Main.MouseWorld.X / 16;
                int y = (int)Main.MouseWorld.Y / 16;
                Vector2 roundedPoint = new Vector2(x, y) * 16;
                Projectile.position = roundedPoint;
                TileEntitySelector.DraggablePoint.X = (int)roundedPoint.X / 16;
                TileEntitySelector.DraggablePoint.Y = (int)roundedPoint.Y / 16;
            }
            else
            {
                Projectile.position.X = TileEntitySelector.DraggablePoint.X * 16;
                Projectile.position.Y = TileEntitySelector.DraggablePoint.Y * 16;
            }

            if (IsMouseHovering)
            {
                Main.LocalPlayer.itemTime = 12;
                Main.LocalPlayer.itemAnimation = 12;
                Main.LocalPlayer.heldProj = Projectile.whoAmI;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 scale = Vector2.One;
            Color drawColor = Color.White;
            if (IsMouseHovering)
            {
                scale *= 1.5f;
                drawColor = Color.LightGoldenrodYellow;
            }
            spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
