using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Moon
{
    internal class MoonlightSwirlEnchantment : BaseEnchantment
    {
        private float Countertimer;

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;
            float oscSpeed = 0.01f;
            float xAmp = 15f;
            float yAmp = 15f;

            Vector2 circleVelocity = new Vector2(
                MathF.Sin(Countertimer * oscSpeed) * xAmp,
                MathF.Cos(Countertimer * oscSpeed) * yAmp);
            circleVelocity = circleVelocity.RotatedBy(Projectile.velocity.ToRotation());
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, circleVelocity, 0.5f);
            //If greater than time then start homing, we'll just swap the movement type of the projectile

        }

        public override float GetStaffManaModifier()
        {
            return 0.3f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<MoonElement>();
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
