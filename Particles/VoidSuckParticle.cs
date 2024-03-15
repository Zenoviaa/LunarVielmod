

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class VoidSuckParticle : Particle
    {
        private int _frameCount;
        private int _frameTick;
        private const int Frame_Count = 10;
        private const int Frame_Duration = 1;
        public override void SetDefaults()
        {
            width = 92;
            height = 24;
            Scale = 1f;
            timeLeft = Frame_Count * Frame_Duration;
            layer = Layer.BeforeNPCs;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D tex3 = texture;
            Vector2 origin = this.OriginCenter();
            spriteBatch.Draw(tex3, screenPos, 
                tex3.AnimationFrame(ref _frameCount, ref _frameTick, Frame_Duration, Frame_Count, true), Color.White, 
                velocity.ToRotation(),
                origin, 
                scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}