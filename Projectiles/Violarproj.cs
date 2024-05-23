using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Violarproj : ModProjectile
	{
		public float ExplodingTimer;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Violarproj");
			Main.projFrames[Projectile.type] = 7;
		}
		public override void SetDefaults()
		{
			Projectile.damage = 0;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = 300;
			Projectile.light = 0.78f;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override bool PreAI()
		{
			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 7)
				{
					Projectile.frame = 0;
				}
			}
			return true;
		}
		public override void OnKill(int timeLeft)
		{

			for (int i = 0; i < 14; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
			}
			for (int i = 0; i < 14; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
			Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
			for (int i = 0; i < Main.rand.Next(3, 7); i++)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
				int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
					ProjectileID.GreekFire3, Projectile.damage, 0f, Projectile.owner);
				Main.projectile[index].friendly = true;
				Main.projectile[index].hostile = false;
			}
		}



		public override void PostDraw(Color lightColor)
		{
			string glowTexture = Texture + "_White";
			Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

			Vector2 textureSize = new Vector2(70, 74);
			Vector2 drawOrigin = textureSize / 2;

			//Lerping
			float progress = ExplodingTimer;
			Color drawColor = Color.Lerp(Color.Transparent, Color.Orange, progress);
			Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
			Main.spriteBatch.Draw(whiteTexture, drawPosition, Projectile.Frame(), drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
		}

		public override void AI()
		{
			Projectile.velocity *= 0.98f;
			{
				Timer++;
				if (Timer == 150)
                {
					int S1 = Main.rand.Next(0, 3);
					if(S1 == 0)
					{
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong"), Projectile.position);
                    }
                    if (S1 == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong2"), Projectile.position);
                    }
                    if (S1 == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong3"), Projectile.position);
                    }
                    int S2 = Main.rand.Next(0, 3);
                    if (S2 == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong"), Projectile.position);
                    }
                    if (S2 == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong2"), Projectile.position);
                    }
                    if (S2 == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowSong3"), Projectile.position);
                    }


                    var entitySource = Projectile.GetSource_FromThis();
                    Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music1").Type, 26, 0, Projectile.owner);
                    Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music2").Type, 26, 0, Projectile.owner);
                    Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music1").Type, 26, 0, Projectile.owner);
					Projectile.NewProjectile(entitySource, Projectile.position, new Vector2(Main.rand.Next(-6, 6), Main.rand.Next(-6, 6)), Mod.Find<ModProjectile>("Music2").Type, 26, 0, Projectile.owner);

					float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
					float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;
					Projectile.Kill();
					SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), Projectile.position);
					Timer = 0;
                }
                if (Timer >= 100)
                {
                    Projectile.scale += 0.002f;
                    ExplodingTimer += 0.005f;
                }

            }
		}
	}
}