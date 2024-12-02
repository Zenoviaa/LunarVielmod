using Stellamod.Items.MoonlightMagic.Elements;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Phantasmal
{
    internal class MagicMissileEnchantment : BaseEnchantment
    {
        public override int GetElementType()
        {
            return ModContent.ItemType<PhantasmalElement>();
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            Vector2 value = Main.MouseWorld;
            if (Vector2.Distance(Projectile.Center, Main.MouseWorld) >= 64f)
            {
                Vector2 v = value - Projectile.Center;
                Vector2 vector2 = v.SafeNormalize(Vector2.Zero);
                float num8 = Math.Min(32, v.Length());
                Vector2 value2 = vector2 * num8;
                if (Projectile.velocity.Length() < 4f)
                {
                    Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.7853981852531433).SafeNormalize(Vector2.Zero) * 4f;
                }
                if (Projectile.velocity.HasNaNs())
                {
                    Projectile.Kill();
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, value2, 0.1f);
            }
            else
            {
                Projectile.velocity *= 0.3f;
                Projectile.velocity += (value - Projectile.Center) * 0.3f;
            }

        }
    }
}
