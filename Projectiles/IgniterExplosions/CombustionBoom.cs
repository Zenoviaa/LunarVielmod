using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
	public class CombustionBoom : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 30;
		}

		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.width = 1024;
			Projectile.height = 1024;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 30;
			Projectile.scale = 1f;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}


        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 30)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }


        public override void AI()
		{
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			for(int i = 0; i < 3; i++)
			{
                switch (Main.rand.Next(0, 4))
                {
                    case 0:
                        target.AddBuff(BuffID.OnFire3, 120);
                        break;
                    case 1:
                        target.AddBuff(BuffID.ShadowFlame, 120);
                        break;
                    case 2:
                        target.AddBuff(BuffID.CursedInferno, 120);
                        break;
                    case 3:
                        target.AddBuff(BuffID.Daybreak, 60);
                        break;
                }
            }
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

			float width = 214;
			float height = 214;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 30;
			SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
				(Color)GetAlpha(lightColor), 0f, origin, 4f, SpriteEffects.None, 0f);
			return false;
        }
    }
}