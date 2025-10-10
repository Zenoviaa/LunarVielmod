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

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Deeya
{
    public class ShadowStepEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.12f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
        }

        public override void SetMagicDefaults()
        {
            float range = MagicProj.Size * 8;
            Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4/2);
            Projectile.position.X += Main.rand.NextFloat(-range, range);
            Projectile.position.Y += Main.rand.NextFloat(-range, range);
        }
    }
}
