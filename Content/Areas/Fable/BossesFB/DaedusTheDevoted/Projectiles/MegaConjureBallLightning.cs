using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles
{
    public class MegaConjureBallLightning : ModProjectile
    {
        private float _scale;
        private float _width;
        private Vector2[] _lightningZaps;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Charge => ref Projectile.ai[1];

        private ref float Parent => ref Projectile.ai[2];

        public CoreLightning Lightning { get; set; } = new CoreLightning();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _width = 1;
            _lightningZaps = new Vector2[7];
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.timeLeft = 600;
            Projectile.light = 0.48f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;

            SpriteBatch spriteBatch = Main.spriteBatch;

            Lightning.WidthMultiplier = 2;
            Lightning.SetBoltDefaults();
            Lightning.Draw(spriteBatch, _lightningZaps, null);

            Vector2 scale = Vector2.One * drawScale * 0.4f;
   
            var shader = TeslaOrbShader.Instance;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);

            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation += 0.2f;

            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Wave");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

            }

            if (Timer % 3 == 0)
            {
                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    float progress = i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + Timer * 0.05f;

                    float osc = VectorHelper.Osc(256, 384, speed: 3);
                    float p = Timer / 300f;
                    osc *= MathHelper.Lerp(1f, 0.5f, p);
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * osc;
                    _lightningZaps[i] = Projectile.Center + offset;
                }
                Lightning.RandomPositions(_lightningZaps);
            }
            if (Timer % 6 == 0)
            {
                for (float f = 0; f < 1; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<SparkParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }
            }
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer <= 300f)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(4.5f, 6), Timer / 300f);
            }

            if (Timer < 300)
            {
                if (Parent != -1)
                {
                    NPC parentNpc = Main.npc[(int)Parent];
                    Projectile.Center = parentNpc.Center - new Vector2(0, 128);
                    Projectile.velocity = Vector2.Zero;
                }

            }

            if (Timer > 300)
            {

                Projectile.velocity = Vector2.UnitY * 3;
            }

            if (Timer > 360)
            {
                Projectile.tileCollide = true;
            }

            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            SoundStyle zapSound = SoundID.DD2_LightningBugZap;
            zapSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(zapSound, target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 16; i++)
            {
                float progress = i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            //EXPLODE
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<ConjureBallExplosionBig>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
