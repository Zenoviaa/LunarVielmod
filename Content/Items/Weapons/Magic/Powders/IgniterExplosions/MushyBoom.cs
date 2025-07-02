using Terraria;
using Terraria.ID;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class MushyBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 60;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Poisoned, 120);
            }
        }
    }
}