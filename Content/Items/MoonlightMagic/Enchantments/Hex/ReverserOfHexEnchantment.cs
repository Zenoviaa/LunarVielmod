using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Helpers;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.ItemTemplates;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Visual.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Hex
{
    public class ReverserOfHexEnchantment : BaseEnchantment
    {
        
        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 60;
        }
        
        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;
            if (Countertimer >= time)
            {

                //If greater than time then start homing, we'll just swap the movement type of the projectile

                foreach (var enchantment in MagicProj.Enchantments)
                {
                    //do a thing here
                    if (enchantment.Countertimer > enchantment.time)
                    {
                        enchantment.Countertimer = 0;
                    }

                }
            }

        }

        public override float GetStaffManaModifier()
        {
            return 1f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<HexElement>();
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.HexPurple);
        }
    }
}
