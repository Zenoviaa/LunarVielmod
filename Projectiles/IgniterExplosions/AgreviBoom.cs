using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class AgreviBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 15;
        public override void SetDefaults()
        {
            base.SetDefaults();
            DrawScale = 1f;
            Projectile.width = 132;
            Projectile.height = 132;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightGoldenrodYellow, endRadius: 96);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
        }
    }
}