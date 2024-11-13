using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.ID;

namespace Stellamod.Projectiles
{
    public class FrostbiteProj : BaseIgniterExplosion
    {
        public override int FrameCount => 30;
        public override bool BlackIsTransparency => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            DrawScale = 1f;
        }

        public override void AI()
        {
            base.AI();
            DrawScale *= 0.98f;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightCyan, endRadius: 70);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }
    }
}