using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Dread
{
    internal class HeartBurnBlastEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 75;
        }

        public override void AI()
        {
            base.AI();
            if (!MagicProj.IsClone)
            {
                //Count up
                Countertimer++;

                //If greater than time then start homing, we'll just swap the movement type of the projectile
                if (Countertimer == time)
                {
                    int count = 4;
                    for (float i = 0; i < count; i++)
                    {
                        float progress = i / count;
                        float angle = MathHelper.TwoPi;
                        float fireRot = progress * angle;
                        Vector2 fireDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        Vector2 firePoint = Projectile.Center;
                        Vector2 fireVelocity = fireDirection.RotatedBy(fireRot - angle / 2f) * Projectile.velocity.Length() * 0.5f;
                        AdvancedMagicUtil.CloneMagicProjectile(MagicProj, firePoint, fireVelocity * 2.5f, Projectile.damage / 2, Projectile.knockBack / count,
                            MagicProj.TrailLength * 2, MagicProj.Size / count);
                    }

                    Projectile.Kill();
                }
            }
        }

        public override float GetStaffManaModifier()
        {
            return 0.4f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<DreadElement>();
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.DreadRed);
        }
    }
}
