using Stellamod.Content.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Guuts
{
    public class DoubleHitEnchantment : BaseEnchantment
    {
        public override void SetMagicDefaults()
        {
            base.SetMagicDefaults();
            Projectile.penetrate += 1;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            //Spawn the explosion
            Projectile.velocity *= 0.5f;
        }
    }
}
