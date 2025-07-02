using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Stellamod.Core.Helpers;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class IshBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 61;

        public override bool BlackIsTransparency => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.White, endRadius: 128);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Confused, 120);
            }
        }
    }
}