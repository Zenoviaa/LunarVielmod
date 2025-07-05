using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Tiles;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.ToolsSystem
{
    internal class TilePainterPreview : ModProjectile
    {
        public override string Texture => AssetHelper.EmptyTexture;
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            base.AI();
            TilePainterPlayer painter = Owner.GetModPlayer<TilePainterPlayer>();
            if (!painter.ShouldDraw)
                Projectile.Kill();


            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;

            Rectangle rectangle = Structurizer.ReadSavedRectangle(Structurizer.SelectedStructure);
            Vector2 mousePos = new Vector2(x, y) * 16;
            Projectile.position = mousePos;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.White * 0.5f;
            Vector2 drawPos = Projectile.position - Main.screenPosition;
            TilePainterPlayer painter = Owner.GetModPlayer<TilePainterPlayer>();
            if (painter.SelectedTile != null)
            {
                if (painter.SelectedTile is BaseSpecialTile specialTile)
                {
                    int i = (int)(Projectile.position.X / 16);
                    int j = (int)(Projectile.position.Y / 16);
                    specialTile.DrawPreview(i, j);
                }
            }
            else if (painter.SelectedWall != null)
            {
                if (painter.SelectedWall is BaseSpecialWall specialTile)
                {
                    int i = (int)(Projectile.position.X / 16);
                    int j = (int)(Projectile.position.Y / 16);
                    specialTile.DrawPreview(i, j);
                }
            }
            return base.PreDraw(ref lightColor);
        }
    }
    internal class TilePainterPlayer : ModPlayer
    {
        public ModTile SelectedTile { get; set; }
        public ModWall SelectedWall { get; set; }
        public bool ShouldDraw => SelectedTile != null || SelectedWall != null && ModContent.GetInstance<ToolsUISystem>().ShouldDraw;

        public override void PostUpdate()
        {
            base.PostUpdate();
            bool shouldDraw = SelectedTile != null || SelectedWall != null;
            if (!ShouldDraw)
                return;

            if (Main.LocalPlayer.controlUseItem && Main.mouseLeft)
            {
                if (SelectedTile != null)
                {
                    Player.itemAnimation = 12;
                    Player.itemTime = 12;
                    int i = (int)Main.MouseWorld.X / 16;
                    int j = (int)Main.MouseWorld.Y / 16;
                    SelectedTile.PlaceInWorld(i, j, new Item(0));
                }
                else if (SelectedWall != null)
                {
                    Player.itemAnimation = 12;
                    Player.itemTime = 12;
                    int i = (int)Main.MouseWorld.X / 16;
                    int j = (int)Main.MouseWorld.Y / 16;
                    WorldGen.PlaceWall(i, j, SelectedWall.Type);
                }
            }
            else if (Main.LocalPlayer.controlUseItem && Main.mouseRight)
            {

                if (SelectedTile != null)
                {
                    Player.itemAnimation = 12;
                    Player.itemTime = 12;
                    int i = (int)Main.MouseWorld.X / 16;
                    int j = (int)Main.MouseWorld.Y / 16;
                    WorldGen.KillTile(i, j);
                }
                else if (SelectedWall != null)
                {
                    Player.itemAnimation = 12;
                    Player.itemTime = 12;
                    int i = (int)Main.MouseWorld.X / 16;
                    int j = (int)Main.MouseWorld.Y / 16;
                    WorldGen.KillWall(i, j);
                }
            }

            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.Red, null);
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<TilePainterPreview>()] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<TilePainterPreview>(), 1, 1, Player.whoAmI);
            }
        }
        public void SelectTile(ModTile modTile)
        {
            SelectedTile = modTile;
            SelectedWall = null;
        }
        public void SelectWall(ModWall modWall)
        {
            SelectedWall = modWall;
            SelectedTile = null;
        }
    }
}
