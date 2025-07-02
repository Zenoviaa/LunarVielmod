using Microsoft.Xna.Framework;
using Terraria;
using Stellamod.Core.Helpers;

namespace Stellamod.Content.Items.Weapons.Magic.Powders.IgniterExplosions
{
    public class EldritchBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 8;


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 190;
            Projectile.height = 190;
        }

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
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightBlue, endRadius: 78);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.Knockback += 8;
        }
    }
}