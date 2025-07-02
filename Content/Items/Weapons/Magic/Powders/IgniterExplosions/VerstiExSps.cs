using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ID;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    internal class VerstiExSps : BaseIgniterExplosion
    {
        public override int FrameCount => 15;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Green);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Ichor, 120);
            }
        }
    }
}
