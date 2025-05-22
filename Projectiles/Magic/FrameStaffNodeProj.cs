using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class FrameStaffNodeProj : ModProjectile
    {
        private const float Whiten_Time = 60;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float WhiteTimer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 720;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Oscillate movement
            Timer++;
            if(Timer < 60)
            {
                float ySpeed = Timer / 60;
                ySpeed = Easing.SpikeInOutCirc(ySpeed);
                Projectile.velocity = new Vector2(0, -ySpeed);
            } else if (Timer < 120)
            {
                //Inverse
                float ySpeed = 1f - ((Timer - 60) / 60);
                ySpeed = Easing.SpikeInOutCirc(ySpeed);
                Projectile.velocity = new Vector2(0, ySpeed);
            }
            if(Timer == 120)
            {
                Timer = 0;
            }

            if (IsActivated())
            {
                WhiteTimer++;
                WhiteTimer = MathHelper.Clamp(WhiteTimer, 0, Whiten_Time);
            }
            else
            {
                WhiteTimer--;
                WhiteTimer = MathHelper.Clamp(WhiteTimer, 0, Whiten_Time);
            }
        }

        private bool IsActivated()
        {
            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active)
                    continue;
                if (p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<FrameStaffConnectorProj>())
                    continue;

                return true;
            }

            return false;
        } 

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Aquamarine, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new Vector2(38, 40);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = WhiteTimer / Whiten_Time;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
