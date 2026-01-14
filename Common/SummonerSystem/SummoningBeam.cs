using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    public class SummoningBeam : ModProjectile
    {
        private Vector2 _scale;
        private ref float Timer => ref Projectile.ai[0];
        private int MinionToSummon => (int)Projectile.ai[1];
        private ref float Lifetime => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];

        public bool isGuardian;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            float ticks = 30f;
            float lerp = Timer / ticks;
            float interp = EasingFunction.QuadraticBump(lerp);
            _scale = Vector2.Lerp(new Vector2(0f, 1f), Vector2.One, interp);
            if (Timer == 15)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        MinionToSummon, Projectile.damage, Projectile.knockBack, Projectile.owner);

                    AbstractBellSummon bellSummon = p.ModProjectile as AbstractBellSummon;
                    bellSummon.lifetime = Lifetime;
                    bellSummon.isGuardian = isGuardian;
                    p.netUpdate = true;
                    Owner.AddBuff(ModContent.BuffType<BellBlessing>(), 25);
                }
                SoundStyle cast = new SoundStyle("Stellamod/Assets/Sounds/Aurora");
                cast.PitchVariance = 0.2f;
                SoundEngine.PlaySound(cast, Projectile.position);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Black);
                for (float f = 0; f < Main.rand.Next(3, 7); f++)
                {
                    FXUtil.GlowStretch(Projectile.Center, -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(3f, 6f));
                }
                for (float f = 0; f < Main.rand.Next(3, 7); f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f, 6f),
                        newColor: Color.White,
                        Scale: Main.rand.NextFloat(0.5f, 1f));
                }
            }

            if (Timer >= ticks)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = AssetManager.GlowMask.GradientPillar.Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2f, texture.Height);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.Lerp(Color.White, Color.Black, Timer / 30f);
            drawColor = Color.Lerp(drawColor, Color.Black, 0.2f);
            drawColor.A = 0;
            Vector2 beamScael = Vector2.One;
            beamScael.X *= 0.35f;
            beamScael.X *= MathHelper.SmoothStep(0f, 1f, EasingFunction.QuadraticBump(Timer / 30f));
            beamScael.Y *= MathHelper.SmoothStep(0f, 1f, Timer / 30f);
            spriteBatch.Draw(texture, drawPosition, null, drawColor, 0, drawOrigin, beamScael, SpriteEffects.None, 0);
            return false;
        }
    }
}
