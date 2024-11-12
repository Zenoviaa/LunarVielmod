using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Visual;
using Terraria;
using Stellamod.Helpers;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class IlluredBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 32;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightSkyBlue, endRadius: 80);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Vector2 upwardVelocity = -Vector2.UnitY * Projectile.knockBack * 8.5f;
            upwardVelocity *= target.knockBackResist;
            target.velocity += upwardVelocity;
        }
    }
}