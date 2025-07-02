using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Stellamod.Core.Helpers;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class JungleBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 10;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FrameSpeed = 0.5f;
        }

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
                target.AddBuff(BuffID.Poisoned, 120);
            }
        }
    }
}