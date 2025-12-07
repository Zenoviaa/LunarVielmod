using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.RibbonSystem
{
    public class RibbonWand : ModItem
    {
        public RibbonWandType style;
        public static Vector2? startPosition;
        public static Vector2? endPosition;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item42;
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int frameHeight = frame.Height / 5;
            int y = (int)style * frameHeight;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle sourceFrame = new Rectangle(0, y, texture.Width, texture.Height / 5);


            spriteBatch.Draw(texture, position, sourceFrame, Color.White, 0, sourceFrame.Size() / 2f, 0.75f, SpriteEffects.None, 0);
            return false;
        }
        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int mouseX = (int)(Main.MouseWorld.X / 16);
                int mouseY = (int)(Main.MouseWorld.Y / 16);

                int tileMouseX = mouseX;
                int tileMouseY = mouseY;

                //Just some position clamping so it's not connecting floating points and it looks a bit better
                mouseX *= 16;
                mouseY *= 16;
                if (player.altFunctionUse == 2)
                {
                    int ribbonType = (int)style;
                    ribbonType++;
                    if (ribbonType >= 5)
                    {
                        ribbonType = 0;
                    }
                    style = (RibbonWandType)ribbonType;
                }
                else
                {

                    Tile tile = Main.tile[tileMouseX, tileMouseY];
                    Vector2 proposedPosition = new Vector2(mouseX, mouseY);

                    if (WorldGen.SolidTile(tile))
                    {
                        if (startPosition == null)
                        {
                            startPosition = proposedPosition;

                        }
                        else if (endPosition == null)
                        {
                            endPosition = proposedPosition;
                        }

                        if (startPosition != null && endPosition != null)
                        {
                            Vector2 start = startPosition.Value;
                            Vector2 end = endPosition.Value;
                            Vector2 temp = start;



                            RibbonRenderer ribbonRenderer = ModContent.GetInstance<RibbonRenderer>();
                            ribbonRenderer.PlaceRibbon(start, end, style);
                            startPosition = null;
                            endPosition = null;
                        }
                    }
                }

            }


            return true;
        }
    }
}
