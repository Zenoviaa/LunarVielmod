using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Hex
{
    internal class TrailerEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 15;
        }

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;

            //If greater than time then start homing, we'll just swap the movement type of the projectile


            if (Countertimer < time)
            {
                MagicProj.TrailLength += 1;
            }
        }

        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<HexElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }
    }
}
