using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.INest
{
    public class AEgg : ModProjectile
	{
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acid Egg");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 32;

            Projectile.penetrate = 2;
			Projectile.tileCollide = true;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.FrostDaggerfish;
			Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = false;
			Projectile.hostile = true;
		}

        public override void OnKill(int timeLeft)
        {
            var entitySource = Projectile.GetSource_FromThis();
            NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<IrradiatedCreeper>());

            for (int i = 0; i < 15; i++)
			{
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<Dusts.GunFlash>(), newColor: ColorFunctions.AcidFlame);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<Dusts.GunFlash>(), newColor: ColorFunctions.AcidFlame);
            }

			for (int i = 0; i < 30; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, 74, (Vector2.One * Main.rand.Next(1, 4)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
			}

			Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Egg_Land"));
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			width = (height = 8);
			fallThrough = base.Projectile.position.Y <= base.Projectile.ai[1];
			return true;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(2))
				target.AddBuff(BuffID.Frostburn, 180);
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.GreenYellow, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SpikyTrail1);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            return false;
        }
	}
}
