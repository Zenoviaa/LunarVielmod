using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Stellamod.Core.Helpers;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class Skullboom : BaseIgniterExplosion
    {
        public override int FrameCount => 30;
        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple, endRadius: 80);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.ShadowFlame, 120);
            }
        }
    }
}