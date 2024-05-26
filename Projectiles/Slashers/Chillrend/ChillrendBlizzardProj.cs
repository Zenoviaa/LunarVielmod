using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Projectiles.Slashers.Chillrend
{
    internal class ChillrendBlizzardProj : ModProjectile
    {
        public override string Texture => TextureRegistry.VoxTexture3;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 512;
          
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            
        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation += 0.05f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Chilled, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Frozen, 60);
                    break;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.LightCyan.R,
                Color.LightCyan.G,
                Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            float scale = 2f;
            float progress = Timer / 60;
            float easedProgress = Easing.InOutExpo(progress);
            scale *= easedProgress;

            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor *= Easing.SpikeInOutCirc((1 - progress)) * 0.3f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation,
                drawOrigin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
