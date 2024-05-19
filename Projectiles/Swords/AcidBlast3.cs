
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Projectiles.Swords
{
    public class AcidBlast3 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Blast");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 5;
			Projectile.timeLeft = 1000;
            Projectile.tileCollide = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
		}

		int timer;

        public override void AI()
        {
            Timer++;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch);
			int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch);

			//No need to set the velocity and then multiply by zero when you can just pass in 0 to the speed.
			//Main.dust[dust].velocity *= 0f;
			//Main.dust[dust1].velocity *= 0f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust1].noGravity = true;

			//Returning true here kills the projectile, so no need to do it manually.
			return true;

			/*	
			Projectile.Kill();

			// This code just straight up doesn't execute
			// Main.rand.Next(1) == 1 will always be false, since rand is exclusive
			// The statement translates to if (0 == 1)

			if (Main.rand.Next(1) == 1)
			{
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[dust].velocity *= 0f;
				Main.dust[dust1].velocity *= 0f;
				Main.dust[dust].noGravity = true;
				Main.dust[dust1].noGravity = true;
			}

			return false;
			*/
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
		}










        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.RoyalBlue.R,
                Color.RoyalBlue.G,
                Color.RoyalBlue.B, 0);
        }
      
        private float LifeTime = 300;
        private float MaxScale = 0.4f;
        public override bool PreDraw(ref Color lightColor)
        {
            var textureAsset = ModContent.Request<Texture2D>(Texture);
            Texture2D texture = textureAsset.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            float progress = Timer / LifeTime;
            float easedProgress = Easing.OutCirc(progress * 1.5f);
            float scale = easedProgress * MaxScale;

            //This should make it fade in and then out
            float alpha = Easing.SpikeOrb(progress);
            alpha += 0.05f;
            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor = drawColor * alpha;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;
            float opacityProgress = Easing.SpikeCirc(progress + 0.25f);

            //You have to set the opacity/alpha here, alpha in the spritebatch won't do anything
            //Should be between 0-1
            float opacity = 0.75f;
            shader.UseOpacity(opacity * opacityProgress);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(0.15f);

            //How fast the extra texture animates
            float speed = 1.0f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(new Color(Color.OrangeRed.R, Color.OrangeRed.G, Color.OrangeRed.B, 0));

            //Texture itself
            shader.UseImage1(textureAsset);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            for (int i = 0; i < 4; i++)
            {
                float drawScale = scale * (i / 4f);
                float drawRotation = Projectile.rotation * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, (Color)GetAlpha(lightColor), drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin();
            //I think that one texture will work
            //The vortex looking one
            //And make it spin
            return false;
        }
    }
}