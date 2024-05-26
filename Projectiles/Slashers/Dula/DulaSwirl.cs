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
using Stellamod.Dusts;

namespace Stellamod.Projectiles.Slashers.Dula
{
    internal class DulaSwirl : ModProjectile
    {
        public override string Texture => TextureRegistry.VoxTexture4;
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
            Projectile.rotation += 0.3f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.CursedInferno, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.OnFire, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.CursedInferno, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.OnFire3, 60);
                    break;
            }


            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(target.position, Projectile.width, Projectile.height, ModContent.DustType<GunFlash>(), Scale: 0.8f);
                Dust.NewDustPerfect(target.position, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.GreenYellow, 1f).noGravity = true;
            }


            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire3, 180);


        
    }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.LightGoldenrodYellow.R,
                Color.LightGoldenrodYellow.G,
                Color.LightGoldenrodYellow.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            float scale = 1.5f;
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
