using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Uvilis
{
    internal class TyphoonWaverEnchantment : BaseEnchantment
    {
        private Vector2 _velocity;
        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;
            if (Countertimer == 1)
            {
                _velocity = Projectile.velocity;
            }

            Vector2 newVelocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(25));
            Projectile.velocity = newVelocity;
            Projectile.Center += _velocity * 0.3f;

            // Projectile.a greater than time then start homing, we'll just swap the movement type of the projectile
        }


        public override float GetStaffManaModifier()
        {
            return 0.3f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<UvilisElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.MoonGreen);
        }
    }
}
