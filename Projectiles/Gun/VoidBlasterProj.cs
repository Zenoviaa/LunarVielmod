

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class VoidBlasterProj : ModProjectile
	{
		public bool OptionallySomeCondition { get; private set; }

		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Granite MagmumProj");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Bullet);
			AIType = ProjectileID.Bullet;
			Projectile.penetrate = 1;
			Projectile.width = 15;
			Projectile.height = 15;
		}
		public override void AI()
		{
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 15)
            {
                Projectile.penetrate = 1;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.alpha = Math.Max(0, Projectile.alpha - 25);

                bool flag25 = false;
                int jim = 1;
                for (int index1 = 0; index1 < 200; index1++)
                {
					if (Main.npc[index1].CanBeChasedBy(Projectile, false)
						&& Projectile.Distance(Main.npc[index1].Center) < 500
                        && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
                    {
                        flag25 = true;
                        jim = index1;
                    }
                }

                if (flag25)
                {
                    float num1 = 10f;
                    Vector2 vector2 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                    float num2 = Main.npc[jim].Center.X - vector2.X;
                    float num3 = Main.npc[jim].Center.Y - vector2.Y;
                    float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
                    float num5 = num1 / num4;
                    float num6 = num2 * num5;
                    float num7 = num3 * num5;
                    int num8 = 10;
                    Projectile.velocity.X = (Projectile.velocity.X * (num8 - 1) + num6) / num8;
                    Projectile.velocity.Y = (Projectile.velocity.Y * (num8 - 1) + num7) / num8;
                }
            }
        }
		public override bool PreAI()
		{
			int num1222 = 74;
			for (int k = 0; k < 2; k++)
			{
				int index2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[index2].position = Projectile.Center - Projectile.velocity / num1222 * k;
				Main.dust[index2].scale = .95f;
				Main.dust[index2].velocity *= 0f;
				Main.dust[index2].noGravity = true;
				Main.dust[index2].noLight = false;
			}
			return true;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.timeLeft = 1;
			return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterNPC == null)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHitsTime = 0;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterNPC = target;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHits++;
            }
            else if(target != Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterNPC)
            {
                if (Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHits < 6)
                {
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHitsTime = 0;
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHits = 0;
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterNPC = target;
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHits++;
                }

            }
            else
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHits++;
                if (Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHits >= 7)
                {
                    var EntitySource = Projectile.GetSource_FromThis();
                    Projectile.NewProjectile(EntitySource, target.Center.X, target.Center.Y, 0, 0, 
                        ModContent.ProjectileType<VoidBlasterExsplosion>(), Projectile.damage, 1, Projectile.owner, 0, ai1: target.whoAmI);
                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidBlasterExplosionBomb"), Projectile.position);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidBlasterExplosionBomb2"), Projectile.position);
                    }
                }
                else
                {
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHitsTime = 0;
                }
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidBlaster2"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidBlaster1"), Projectile.position);
            }
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Lerp(new Color(1, 244, 255), new Color(67, 37, 172), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DodgerBlue, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 240, Color.DarkGray, 1f).noGravity = true;
            }
        }
	}
}