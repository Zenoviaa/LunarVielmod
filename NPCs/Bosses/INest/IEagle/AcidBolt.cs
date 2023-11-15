using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.INest.IEagle
{
    public class AcidBolt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Acid Bolt");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 25;
			Projectile.height = 25;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
			Projectile.damage = 15;
			Projectile.aiStyle = -1;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 20; i++) {
				Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height,
					DustID.Dirt, 0, 60, 133);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * ((255 - Projectile.alpha) / 255f);
		}
		
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<Buffs.AcidFlame>(), 200);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[base.Projectile.type].Value;
			Vector2 drawOrigin = new Vector2(22f, 22f) * 0.5f;
            Main.spriteBatch.BeginBlendState(BlendState.Additive);
			for (int i = 0; i < base.Projectile.oldPos.Length; i++)
			{
				Vector2 drawPos = base.Projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, base.Projectile.gfxOffY);
				Color color = Color.FromNonPremultiplied(152, 208, 113, 255 - base.Projectile.alpha) * ((base.Projectile.oldPos.Length - i) / (float)base.Projectile.oldPos.Length);
                Main.spriteBatch.Draw(tex, drawPos, new Rectangle(0, 0, 22, 70), color, base.Projectile.rotation, drawOrigin, base.Projectile.scale, SpriteEffects.None, 0f);
			}
            Main.spriteBatch.Draw(tex, base.Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, base.Projectile.gfxOffY), new Rectangle(0, 0, 22, 70), Color.FromNonPremultiplied(255, 255, 255, 255 - base.Projectile.alpha), base.Projectile.rotation, drawOrigin, base.Projectile.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.EndBlendState();
			return false;
		}

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            Projectile.ai[1] += 1f;
			if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 30)
			{
				Projectile.velocity *= 0.79f;
			}

			if (Projectile.ai[1] >= 30 && Projectile.ai[1] <= 160)
            {
				Projectile.velocity /= 0.89f;
			}
			if (Projectile.ai[1] == 30)
			{
				SoundEngine.PlaySound(SoundID.Item63, Projectile.Center);
			}
		}
	}
}