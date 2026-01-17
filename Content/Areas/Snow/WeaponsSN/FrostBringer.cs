using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
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

namespace Stellamod.Content.Areas.Snow.WeaponsSN
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
        public override string Texture => ModContent.GetInstance<FrostBringer>().Texture;
        private ref float Timer => ref Projectile.ai[0];
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

        }


        public override void AI()
        {
            base.AI();
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

            Projectile.velocity *= 0.98f;
            if (Projectile.velocity.Length() <= 0.5f)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            float outScale = MathHelper.SmoothStep(1f, 0f, (float)Projectile.timeLeft / 30f);
            Projectile.scale = outScale;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Frostburn, 60);
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
                afterImageColor *= 0.2f;
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