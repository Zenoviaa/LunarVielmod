using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using System;
using Terraria;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class KaBoomKaev : BaseIgniterExplosion
    {
        public override int FrameCount => 8;
        public override void SetExplosionDefaults()
        {
            base.SetExplosionDefaults();
            FrameSpeed = 0.5f;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Red, endRadius: 70);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                //Life steal for % of the damage
                float healFactor = damageDone * 0.08f;
                int healthToHeal = (int)healFactor;
                healthToHeal = Math.Clamp(healthToHeal, 1, 20);
                Player owner = Main.player[Projectile.owner];
                owner.Heal(healthToHeal);
            }
        }
    }
}