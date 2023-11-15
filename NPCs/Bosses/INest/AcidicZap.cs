using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.INest
{
    public class AcidicZap : ModProjectile
    {
        Vector2 MissilePos;
        bool OnGround;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Zap");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.penetrate = 1;
			Projectile.tileCollide = true;
			Projectile.hostile = false;
			Projectile.friendly = false;
			Projectile.timeLeft = 1000;
            Projectile.hide = false;
			Projectile.alpha = 0;
			Projectile.damage = 0;
			base.Projectile.penetrate = -1;
			base.Projectile.scale = 1f;
			Projectile.width = Projectile.height = 32;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        float alphaCounter = 0;
		public override void AI()
		{
			if (Projectile.timeLeft <= 99)
			{
				OnGround = true;
            }
            if (OnGround)
            {
                if (alphaCounter < 2)
                {
                    alphaCounter += 0.1f;
                }

            }
            Player player;
            if ((player = VectorHelper.GetNearestPlayerDirect(base.Projectile.position, Alive: true)) != null)
            {
                if (Projectile.position.Y + 10 <= player.position.Y)
                {

                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }


            MissilePos.X = Projectile.Center.X;
            MissilePos.Y = Projectile.Center.Y + -1000;

            if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.timeLeft == 50 )
            {
                var entitySource = Projectile.GetSource_FromThis();
                Projectile.NewProjectile(entitySource, MissilePos, new Vector2(0, 28), Mod.Find<ModProjectile>("ToxicMissile").Type, 29, 0);
			}
		}
        public override bool OnTileCollide(Vector2 oldVelocity)
		{

			Projectile.timeLeft = 100;
			base.Projectile.tileCollide = false;
			base.Projectile.velocity = oldVelocity * 0.001f;
			base.Projectile.Center += oldVelocity;
			base.Projectile.rotation = (float)Math.PI;
			return false;
		}
		public override bool PreDraw(ref Color lightColor)
        {
            Vector2 WarnPos;
			WarnPos.X = Projectile.Center.X;
			WarnPos.Y = Projectile.Center.Y;
			if (OnGround)
            {
                Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_56").Value;
                Main.spriteBatch.Draw(texture2D4, (Projectile.Center - Main.screenPosition), null, new Color((int)(30f * alphaCounter), (int)(30f * alphaCounter), (int)(30f * alphaCounter), 0), Projectile.rotation, new Vector2(171, 51), 0.4f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
                Texture2D texture2D5 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_48").Value;
                Main.spriteBatch.Draw(texture2D5, (Projectile.Center - Main.screenPosition), null, new Color((int)(10.5f * alphaCounter), (int)(30.5f * alphaCounter), (int)(22f * alphaCounter), 0), Projectile.rotation, new Vector2(15, 514), 0.6f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
            }

			return true;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = (height = 8);
            fallThrough = base.Projectile.position.Y <= base.Projectile.ai[1];
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}