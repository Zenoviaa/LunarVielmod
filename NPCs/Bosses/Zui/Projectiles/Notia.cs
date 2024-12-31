using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Zui.Projectiles
{
    public class Notina : ModProjectile
	{
		public int timer = 0;
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 150;
			Projectile.ignoreWater = true;
			Projectile.hostile = true;
		}

		public override bool PreAI()
		{
			timer++; 
			Projectile.tileCollide = false;
			if (timer == 5)
            {
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworkFountain_Yellow, 0f, 0f);
				Main.dust[dust].scale = 0.5f;
				timer = 0;
			}
			return true;
		}


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
            Vector2 frameOrigin = frame.Size() / 2;
            Vector2 offset = new Vector2(Projectile.width - frameOrigin.X);
            Vector2 drawPos = Projectile.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            if (Projectile.timeLeft >= 180)
            {
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(209, 209, 0, 80), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + timer) * MathHelper.TwoPi;

                    Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(209, 109, 10, 77), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);  
        }

    }
}