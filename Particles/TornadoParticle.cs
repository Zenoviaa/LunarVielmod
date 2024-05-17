using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Particles
{
    internal class TornadoParticle : Particle
    {
        public override void SetDefaults()
        {
            width = 32;
            height = 32;
            Scale = 1f;
            timeLeft = 90;
            layer = Layer.BeforeNPCs;
        }

        public override void AI()
        {
            velocity = velocity.RotatedBy(MathHelper.PiOver4 / 4);
            position.Y -= 10 * Scale * 0.25f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 origin = this.OriginCenter();
            spriteBatch.Draw(texture, screenPos,
                null, Color.White,
                velocity.ToRotation() + 180,
                origin,
                1.35f * scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
