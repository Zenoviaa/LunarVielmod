using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    public class SummoningBeam : ModProjectile
    {
        private Vector2 _scale;
        private ref float Timer => ref Projectile.ai[0];
        private int MinionToSummon => (int)Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
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
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        MinionToSummon, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Console.WriteLine(MinionToSummon);
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
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Time = Timer * 0.3f;
            sparkyShader.Distortion = -0.15f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(effect: sparkyShader.Effect, blendState: BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, null,
                Color.White,
                0, texture.Size() / 2f,
            _scale, SpriteEffects.None, 0);

            spriteBatch.Restart(blendState: BlendState.Additive, effect: null);
            return false;
        }
    }
}
