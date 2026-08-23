using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class FrostBringer : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 12;
            Item.shoot = ModContent.ProjectileType<FrostBringerSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<FrostBringerStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
            staminaCost = 1;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }

    public class FrostBringerSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.LightSkyBlue;
            blackFireShader.OuterColor = Color.DarkBlue;
            blackFireShader.BackColor = Color.Violet;
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 80;
                },
                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.LightSkyBlue, Color.Violet, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }
            };

            Trailer = devilsPeak;
            useAfterImage = true;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.LightSkyBlue * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                Vector2 position = target.Center;
                Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.White,
                        outerColor: Color.LightBlue,
                        fadeToColor: Color.Violet,
                        distortOut: true);
                }

                _hit = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }

    public class FrostBringerStaminaSlash : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float HitstopTimer => ref Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }


        public override void AI()
        {
            base.AI();
            if(HitstopTimer > 0)
            {
                Projectile.velocity *= 1.1f;
                HitstopTimer--;
                return;
            }
            Timer++;
            if (Timer == 1)
            {
                SoundStyle throwSound = AssetManager.GetSound("FrostBringer");
                throwSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(throwSound, Projectile.position);

                var tp = ThrustParticle.Spawn(Projectile.Center, Projectile.velocity, Color.LightSkyBlue);
                tp.innerColor = Color.White;
                tp.bloomColor = Color.LightSkyBlue;
                Owner.velocity += -Projectile.velocity;
            }

            if (Timer % 7 == 0)
            {
                FlakeParticleSpawnParams spawnParams = new FlakeParticleSpawnParams
                {
                    gravity = 0,
                    scaleRange = new Vector2(0.2f, 0.5f)
                };
                FlakeParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
            }

            if(Timer > 60)
            {
                Projectile.velocity *= 0.98f;
                if (Projectile.velocity.Length() <= 1f)
                    Projectile.Kill();

            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            float outScale = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
            Projectile.scale = outScale;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundStyle hitSound = SoundID.Item28;
            hitSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(hitSound, target.Center);

            target.AddBuff(BuffID.Frostburn, 60);

            for (float i = 0; i < 2; i++)
            {
                var donutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 4 * MathHelper.Lerp(15, 1f, i / 3f));
                donutParticle.Scale *= MathHelper.Lerp(0.3f, 1f, i / 3f);
                donutParticle.Velocity *= 0.1f;
                donutParticle.color = Color.LightSkyBlue;
                donutParticle.innerColor = Color.LightSkyBlue;
                donutParticle.outerColor = Color.Violet;
            }

            HitstopTimer = 4;
            float nuMDust = 8;
            for (float n = 0; n < nuMDust; n++)
            {
                FlakeParticleSpawnParams spawnParams = new FlakeParticleSpawnParams
                {
                    gravity = 0,
                    scaleRange = new Vector2(0.2f, 0.5f)
                };
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                FlakeParticle.Spawn(target.Center, velocity, spawnParams);
            }

            nuMDust = 4;
            for (float n = 0; n < nuMDust; n++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Cyan,
                    outerColor = Color.Violet,
                    gravity = 0,
                    scaleRange = new Vector2(0.5f, 1f)
                };
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                DustParticle dp = DustParticle.Spawn(target.Center, velocity, spawnParams);
                dp.dampening = 0.1f;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
            ModContent.ProjectileType<BaseHitEffect>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
            FXUtil.GlowCircleBoom(target.Center, Color.Cyan, Color.DarkBlue, Color.Violet);
            FXUtil.ShakeCamera(target.Center, 1024, 4);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float nuMDust = 6;
            for (int n = 0; n < nuMDust; n++)
            {
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = -Vector2.UnitY;

                SmokeParticle sp = SmokeParticle.SpawnInAlphaLayer(spawnPoint, vel, Color.DarkGray);
                sp.initialColor = Color.Lerp(Color.LightSkyBlue, Color.Black, 0.8f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 drawCenter = oldPos + Projectile.Size * 0.5f - Main.screenPosition;
                float rotation = Projectile.oldRot[i];
                float ratio = (float)i / (float)Projectile.oldRot.Length;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0f, 1f, ratio));
                afterImageColor *= 0.4f;
                spriteBatch.Draw(texture, drawCenter, null, afterImageColor, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            texture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            drawOrigin = texture.Size() * 0.5f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (i % 2 == 0)
                    continue;

                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 drawCenter = oldPos + Projectile.Size * 0.5f - Main.screenPosition;
                float rotation = Projectile.oldRot[i];
                float ratio = (float)i / (float)Projectile.oldRot.Length;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0f, 1f, ratio));
                afterImageColor *= 0.2f;
                afterImageColor.A = 0;
                spriteBatch.Draw(texture, drawCenter, null, afterImageColor, rotation, drawOrigin, Projectile.scale * 0.2f, SpriteEffects.None, 0);
            }
            this.DrawCentered(ref lightColor);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}