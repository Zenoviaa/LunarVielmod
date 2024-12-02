using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Phantasmal
{
    internal class PhasingMoonEnchantment : BaseEnchantment
    {
        private float Countertimer;



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override float GetStaffManaModifier()
        {
            return 0.1f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<PhantasmalElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }
    }
}
