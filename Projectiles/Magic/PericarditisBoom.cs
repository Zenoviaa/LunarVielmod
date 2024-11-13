using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Projectiles.Magic
{
    internal class PericarditisBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 30;
        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Red, endRadius: 80);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Bleeding, 120);
            }
        }
    }
}
