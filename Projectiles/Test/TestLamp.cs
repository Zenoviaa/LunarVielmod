using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Lights;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Test
{
    internal class TestLamp : ModProjectile,
        IDrawLightCast
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner]; 
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
  
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Owner.ownedProjectileCounts[Type] > 1 || Owner.HeldItem.shoot != Type)
            {
                Projectile.Kill();
            }
            Projectile.Center = Owner.Center;
            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);
       //     Projectile.velocity *= 252;
        }

        public void DrawLightCast(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Beam").Value;
            float degree = 25;
            float range = MathHelper.ToRadians(degree);
            float steps = degree;
 
            for(float i = 0; i < steps; i++)
            {
                float progress = i / steps;
                float rot = MathHelper.Lerp(-range / 2f, range / 2f, progress);
                Vector2 vel = Projectile.velocity.RotatedBy(rot);
                float distance = ProjectileHelper.PerformBeamHitscan(Projectile.Center, vel, maxBeamLength: 512);
                float fallOff = 0.0015f;
                for(float j = 0; j < distance; j+= texture.Size().X * 3)
                {
                    Vector2 drawPos = Projectile.Center + vel.SafeNormalize(Vector2.Zero) * j;
          
                    Lighting.AddLight(drawPos, Color.White.ToVector3() * 0.6f);
                    drawPos -= Main.screenPosition;
                    Color drawColor = Color.White;

                    drawColor *= MathHelper.Lerp(0.5f, 0f, fallOff * j);
                    spriteBatch.Draw(texture, drawPos,null, drawColor, Projectile.rotation, texture.Size() / 2,16, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
