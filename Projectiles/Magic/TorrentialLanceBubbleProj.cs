using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class TorrentialLanceBubbleProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.scale = 0.001f;
        }

        public override void AI()
        {
            Timer++;
            float progress = Timer / 30;
            Projectile.scale = Easing.InExpo(progress);
            Projectile.velocity.Y -= 0.05f;
            Projectile.velocity.X *= 0.98f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                float scale = Main.rand.NextFloat(0.2f, 0.4f);
                ParticleManager.NewParticle(Projectile.Center, velocity, ParticleManager.NewInstance<BubbleParticle>(),
                    Color.White, scale);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / 2;
            spriteBatch.Draw(texture, position, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
