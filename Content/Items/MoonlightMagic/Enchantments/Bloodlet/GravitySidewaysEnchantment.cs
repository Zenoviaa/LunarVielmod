using Stellamod.Content.Items.MoonlightMagic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Bloodlet
{
    public class GravitySidewaysEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<BloodletElement>();
        }

        public override void AI()
        {
            base.AI();
            float maxSpeed = 15;
            if(Owner.direction == 1)
            {
                if (Projectile.velocity.X > -maxSpeed)
                {
                    Projectile.velocity.X -= 0.4f;
                }
            } else if (Owner.direction == -1)
            {
                if (Projectile.velocity.X < maxSpeed)
                {
                    Projectile.velocity.X += 0.4f;
                }
            }
  
        }
    }
}
