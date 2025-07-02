using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ID;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class KaBoomTrick : BaseIgniterExplosion
    {
        public override int FrameCount => 20;
        public override bool BlackIsTransparency => false;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(12))
            {
                target.AddBuff(BuffID.Confused, 120);
            }
        }
    }
}