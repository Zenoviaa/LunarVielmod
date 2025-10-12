using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Bloodlet
{
    public class WaveringFollowEnchantment : BaseEnchantment
    {


        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            Vector2 value = Main.MouseWorld;
            if (Vector2.Distance(Projectile.Center, Main.MouseWorld) >= 64f)
            {
                Vector2 v = value - Projectile.Center;
                Vector2 vector2 = v.SafeNormalize(Vector2.Zero);
                float num8 = System.Math.Min(32, v.Length());
                Vector2 value2 = vector2 * num8;
                if (Projectile.velocity.Length() < 4f)
                {
                    Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(0.7853981852531433).SafeNormalize(Vector2.Zero) * 4f;
                }
                if (Projectile.velocity.HasNaNs())
                {
                    Projectile.Kill();
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, value2, 0.01f);
            }
            else
            {
               // Projectile.velocity *= 0.3f;
               // Projectile.velocity += (value - Projectile.Center) * 0.03f;
            }     // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<BloodletElement>();
        }
    }
}
