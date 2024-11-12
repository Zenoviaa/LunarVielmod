using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class MushyBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 60;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Blue, endRadius: 70);
            }
        }

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