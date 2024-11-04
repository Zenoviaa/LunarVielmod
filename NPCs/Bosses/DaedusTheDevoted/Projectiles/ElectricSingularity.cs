using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class ElectricSingularity : ModProjectile
    {
        private float _scale;
        private Vector2[] _lightningZaps;
        private ref float Timer => ref Projectile.ai[0];
        private ref float AttackTimer => ref Projectile.ai[1];
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningZaps = new Vector2[4];
            Projectile.width = 49;
            Projectile.height = 49;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.timeLeft = 420;
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
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 16; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(8, 8);
                float rot = Main.rand.NextFloat(0f, 3.14f);

                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor * 0.5f, drawRotation + rot, Projectile.Frame().Size() / 2f,
                    drawScale * VectorHelper.Osc(0.95f, 2f, speed: 6, offset: i), SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 32; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                float rot = Main.rand.NextFloat(0f, 3.14f);

                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), Color.Black, drawRotation + rot, Projectile.Frame().Size() / 2f,
                    drawScale * VectorHelper.Osc(0.85f, 0.95f, speed: 2, offset: i), SpriteEffects.None, 0);
            }


            Lightning.WidthMultiplier = 0.35f;
            for(int i = 0; i < Lightning.Trails.Length; i++)
            {
                var trail = Lightning.Trails[i];
                trail.PrimaryColor = Color.Black;
                trail.NoiseColor = Color.Black;
            }
            Lightning.DrawAlpha(spriteBatch, _lightningZaps, null);
            return false;
        }

        public override void AI()
        {
            base.AI();
            AttackTimer++;
            if (AttackTimer % 4 == 0)
            {
                Vector2 dustSpawnPoint = Projectile.Center + Main.rand.NextVector2CircularEdge(64, 64);
                Vector2 dustVelocity = (Projectile.Center - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                dustVelocity *= 4;
                float progress = AttackTimer / 80f;

                Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                d.noGravity = true;
            }

            if (AttackTimer >= 60)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
                if(player != null)
                {
                    Vector2 velToPlayer = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    velToPlayer *= 16;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velToPlayer,
                        ModContent.ProjectileType<ElectricSingularityBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                if (Main.rand.NextBool(4))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                       ModContent.ProjectileType<ElectricSingularityBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                AttackTimer = 0;
            }

            //Some interesting movement code for the singularity
            Player playerToTarget = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
            if(playerToTarget != null)
            {
                float diffX = playerToTarget.Center.X - Projectile.Center.X;
                Projectile.velocity.X = diffX * 0.03f;
            }
       
            Projectile.velocity.Y = MathF.Sin(Timer * 0.05f) * 2;

            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Enrage");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                //Spawn Dust Circle
                for(int i = 0; i < 32; i++)
                {
                    float progress = (float)i / 32f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 8;
                    Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel);
                }
            }

            if (Timer % 3 == 0)
            {
                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    float progress = (float)i / (float)_lightningZaps.Length;
                    float rot = progress * MathHelper.TwoPi * 1 + (Timer * 0.05f);
                    Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(64, 80, speed: 3);
                    _lightningZaps[i] = Projectile.Center + offset;
                }

                Lightning.RandomPositions(_lightningZaps);
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

            if (Timer <= 15)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(1f, 1.4f), Easing.InCubic(Timer / 15f));
            }

            if(Timer > 400)
            {
                _scale *= 0.98f;
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
                float progress = (float)i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }
        }
    }
}
