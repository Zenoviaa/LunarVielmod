using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.RoyalMagic
{
    internal class RadioSilenceEnchantment : BaseEnchantment
    {
        private float Countertimer;

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;
            float oscSpeed = 0.05f;
            float xAmp = 5;
            float yAmp = 5;
            Projectile.velocity += new Vector2(
                MathF.Sin(Projectile.position.X * oscSpeed) * xAmp,
                MathF.Cos(Projectile.position.Y * oscSpeed) * yAmp);


            /*float p = MathF.Sin(Countertimer * 0.05f);
            float ep = Easing.SpikeOutExpo(p);
            float radiansRange = 3;
            float radiansToRotateBy = MathHelper.Lerp(-radiansRange, radiansRange, ep);
            Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(radiansToRotateBy);
            //If greater than time then start homing, we'll just swap the movement type of the projectile

            */

        }

        public override float GetStaffManaModifier()
        {
            return 0.1f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RoyalMagicElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.RoyalMagicPink);
        }
    }
}
