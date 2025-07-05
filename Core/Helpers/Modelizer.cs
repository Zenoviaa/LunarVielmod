using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Helpers
{
    public class Modelizer : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            if (player.HeldItem.type == Type)
            {
                int x = (int)Main.MouseWorld.X / 16;
                int y = (int)Main.MouseWorld.Y / 16;

                Rectangle rectangle = Structurizer.ReadSavedRectangle(Structurizer.SelectedStructure);
                Vector2 bottomRight = new Vector2(x + 1, y + 1) * 16;
                Vector2 topLeft = new Vector2(x + rectangle.Width, y - rectangle.Height) * 16;
                Dust.QuickBox(topLeft, bottomRight, 2, Color.YellowGreen, null);
                Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.Red, null);


            }
            else
            {

            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Structurizer.FlipStructure = !Structurizer.FlipStructure;
                if (Structurizer.FlipStructure)
                {
                    Main.NewText("Flip Structure ON");
                }
                else
                {
                    Main.NewText("Flip Structure OFF");
                }

            }
            else
            {

                if (!string.IsNullOrEmpty(Structurizer.SelectedStructure))
                {
                    Rectangle rectangle = Structurizer.ReadSavedRectangle(Structurizer.SelectedStructure);
                    Point bottomLeft = Main.MouseWorld.ToTileCoordinates();
                    Point topRight = bottomLeft + new Point(rectangle.Width, -rectangle.Height);
                    SnapshotSystem snapshotSystem = ModContent.GetInstance<SnapshotSystem>();
                    snapshotSystem.Save(bottomLeft, topRight);
                    Structurizer.ReadSavedStruct(Structurizer.SelectedStructure, bottomLeft);
                    TileEntityStructurizer.ReadSavedStruct(Structurizer.SelectedStructure, bottomLeft);
                }

            }

            //ModelingPreviewer.texturePreview = null;
            return true;
        }
    }
}

