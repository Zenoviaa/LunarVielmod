using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostBreathProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 0.99f;
            if(Timer % 4 == 0)
            {
                Vector2 speed2 = Projectile.velocity / 2 + Main.rand.NextVector2Circular(0.5f, 0.5f);
                ParticleManager.NewParticle(Projectile.Center, speed2, ParticleManager.NewInstance<ForParticle>(), Color.Violet, Main.rand.NextFloat(0.2f, 0.8f));
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Frostburn, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Frostburn, 60);
                    break;
            }
        }
    }
}
