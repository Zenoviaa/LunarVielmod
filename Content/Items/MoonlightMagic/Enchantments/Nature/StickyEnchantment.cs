using Stellamod.Content.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Nature
{
    public class StickyEnchantment : BaseEnchantment
    {
        public override int GetElementType()
        {
            return ModContent.ItemType<NaturalElement>();
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (MagicProj.stickToTarget != -1)
                return;

            MagicProj.stickToTarget = target.whoAmI;
            MagicProj.stickyOffset = Projectile.Center - target.Center;
        }
    }

}
