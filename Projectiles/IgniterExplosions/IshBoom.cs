using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;



namespace Stellamod.Projectiles.IgniterExplosions
{
    public class IshBoom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 61;
        }

        private int _frameCounter;
        private int _frameTick;
        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 61;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }



        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 61)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 100;
            float height = 100;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 61;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color.White), 0f, origin, 4f, SpriteEffects.None, 0f);

            //(Color.White)
            return false;
        }

       


    }

}