using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusRework
{
    internal class DaedusDeath : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        private bool _focus;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 60;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = 120;
            Projectile.height = 74;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.scale = 1f;
        }

        public override bool PreAI()
        {
            if (++_frameTick >= 2)
            {
                _frameTick = 0;
                if (++_frameCounter >= 60)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }

        private ref float ai_Direction => ref Projectile.ai[0];
        public override void AI()
        {
            if (!_focus)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(Projectile.Center, 7f);
                _focus = true;
            }

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 120;
            float height = 74;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 60;

            SpriteEffects flip = SpriteEffects.None;
            if(ai_Direction == 1)
            {
                flip = SpriteEffects.FlipHorizontally;
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                lightColor, 0f, origin, 2f, flip, 0f);
            return false;
        }
    }
}
