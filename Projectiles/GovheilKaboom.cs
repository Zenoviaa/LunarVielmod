using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;

namespace Stellamod.Projectiles
{
    public class GovheilKaboom : BaseIgniterExplosion
    {
        public override int FrameCount => 16;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.localNPCHitCooldown = Projectile.timeLeft / 3;
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
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightSeaGreen, endRadius: 70);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage *= 0.33f;
        }
    }
}