using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Projectiles.IgniterExplosions
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

            if (Main.rand.NextBool(2))
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DeathShotBomb"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DeathShotBomb2"), Projectile.position);
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