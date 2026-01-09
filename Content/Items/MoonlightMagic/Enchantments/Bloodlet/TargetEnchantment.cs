using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Bloodlet
{
    public class TargetEnchantment : BaseEnchantment
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }
    }
}
