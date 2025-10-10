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

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Radiance
{
    public class SparksSplitEnchantment : BaseEnchantment
    {
        
        public override void SetDefaults()
        {
            base.SetDefaults();
          
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Spawn the explosion
            if (!MagicProj.IsClone)
            {
                //Count up
               

                //If greater than time then start homing, we'll just swap the movement type of the projectile
               
                    int count = 6;
                    for (float i = 0; i < count; i++)
                    {
                        float progress = i / count;
                        float angle = MathHelper.PiOver4;
                        float fireRot = progress * angle;
                        Vector2 fireDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        Vector2 firePoint = Projectile.Center;
                        Vector2 fireVelocity = fireDirection.RotatedBy(fireRot - angle / 2f) * Projectile.velocity.Length() * 0.5f;
                        AdvancedMagicUtil.CloneMagicProjectile(MagicProj, firePoint, -fireVelocity, Projectile.damage / count, Projectile.knockBack / count,
                            MagicProj.TrailLength / count, MagicProj.Size / count);
                    }

                    Projectile.Kill();
                
            }
            return true;
        }

        public override void AI()
        {
            base.AI();
           
        }

        public override float GetStaffManaModifier()
        {
            return 0.3f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.RadianceYellow);
        }
    }
}
