using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    public class AbyssWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("AbyssWave");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 1000;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
			Projectile.damage = 60;
			AIType = ProjectileID.Bullet;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        int timer;
		int colortimer;
		public override bool PreAI()
		{
			timer++;
			if (timer <= 50)
			{
				colortimer++;
			}
			if (timer > 50)
			{
				colortimer--;
			}
			if (timer >= 100)
			{
				timer = 0;
			}
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.Blue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
		}
        
        public override void AI()
        {

            Projectile.localAI[0] += 1f;
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;


            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 256f, 16f);
            if (Main.netMode != NetmodeID.Server)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.BlueTorch);
                dust.velocity *= -1f;
                dust.scale *= .8f;
                dust.noGravity = true;
                Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                vector2_1.Normalize();
                Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                dust.velocity = vector2_2;
                vector2_2.Normalize();
                Vector2 vector2_3 = vector2_2 * 34f;
                dust.position = Projectile.Center - vector2_3;
                Projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BlueTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BlueTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }

        }

        public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, 0f, -2f, 0, default(Color), .8f);
				Main.dust[num1].noGravity = true;
				Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num1].position != Projectile.Center)
					Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, 0f, -2f, 0, default(Color), .8f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num].position != Projectile.Center)
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}
		}
	}

}