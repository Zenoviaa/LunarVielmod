using Microsoft.Xna.Framework;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Moon
{
    internal class ReverseMoonEnchantment : BaseEnchantment
    {

        public override void AI()
        {
            base.AI();
        }
        public override void SetMagicDefaults()
        {
            Projectile.penetrate += 1;
        }

        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<MoonElement>();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Player player = Main.player[Projectile.owner];
            Vector2 diff = (player.Center - Projectile.Center);
            Projectile.Center += -diff;
            Projectile.velocity = -Projectile.velocity;
        }
    }
}
