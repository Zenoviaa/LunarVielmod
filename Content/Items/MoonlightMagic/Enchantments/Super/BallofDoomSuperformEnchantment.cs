using Microsoft.Xna.Framework;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Super
{
    public class BallofDoomSuperformEnchantment : BaseEnchantment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            isSuper = true;
        }
        public override void SetMagicDefaults()
        {
            base.SetMagicDefaults();
            MagicProj.orb = true;
            Projectile.penetrate += 5;
            Projectile.velocity *= 0.8f;
            MagicProj.hitboxSize += 100;
        }
        public override float GetStaffManaModifier()
        {
            return 0.1f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<BasicElement>();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}
