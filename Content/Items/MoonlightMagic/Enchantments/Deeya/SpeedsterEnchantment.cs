using Microsoft.Xna.Framework;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Deeya
{
    public class SpeedsterEnchantment : BaseEnchantment
    {
        private Vector2 _originalVelocity;
        private float _incrementPer;
        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 30;
        }
        public override void AI()
        {
            base.AI();
            Countertimer++;
            if(Countertimer == 1)
            {
                _originalVelocity = Projectile.velocity;
                Projectile.velocity *= 3;

                Vector2 diff = Projectile.velocity - _originalVelocity;
                _incrementPer = diff.Length() / (float)time;
            }

            float ratio = Countertimer / (float)time;
            if(Countertimer < time)
            {
                Vector2 velocityMod = _incrementPer * -Projectile.velocity.SafeNormalize(Vector2.Zero);
                Projectile.velocity += velocityMod;
            }
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
        }
    }
}
