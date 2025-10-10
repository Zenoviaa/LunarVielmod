using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Helpers;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Bloodlet
{
    public class BloodVesselEnchantment : BaseEnchantment
    {
        
        public Vector2 FindBottomTile(Vector2 position)
        {
            int i = (int)(position.X / 16f);
            int upSide = (int)(position.Y / 16f);
            for(int j = upSide; j < Main.maxTilesY; j++)
            {
                if (WorldGen.SolidTile(i, j))
                {
                    return new Vector2(i * 16, j * 16);
                }
            }

            return position;
        }


        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;
            if (Countertimer == 1 && Main.myPlayer == Projectile.owner)
            {
                MagicProj.OldPos[0] = Vector2.Zero;
                Vector2 center = Main.MouseWorld;

                Vector2 col = FindBottomTile(center);
                Projectile.Center = col - Vector2.UnitY * (MagicProj.Size + 2);
                Projectile.velocity = -Vector2.UnitY * Projectile.velocity.Length();
                Projectile.netUpdate = true;
            }
        }

        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<BloodletElement>();
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.DreadRed);
        }
    }
}
