using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class Alcarish : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSafunai();
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<AlcarishProj>();

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.damage = 17;
            Item.value = 10000;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankSafunai>(),
                material: ModContent.ItemType<GintzlMetal>());
        }
    }
    public class AlcarishProj : BaseSafunaiProjectile
    {
        public SlashEffect SlashEffect { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            //Define shader, set the shader
            SlashEffect = new()
            {
                BaseColor = Color.Gray,
                WindColor = Color.DarkGray,
                LightColor = Color.LightGray,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };

            Trailer.Shader = SlashEffect;
            Trailer.TrailColorFunction = GetTrailColor;
            Trailer.TrailWidthFunction = GetTrailWidth;
        }
        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.InOutCubic(interpolant) * 24;
        }
        private Color GetTrailColor(float interpolant)
        {
            return Color.Lerp(Color.LightGray, Color.Transparent, interpolant) * 0.3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;

            if (Slam)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    target.Center.X + speedX,
                    target.Center.Y + speedY, speedX, speedY, ProjectileID.ThrowingKnife, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    target.Center.X + speedX,
                    target.Center.Y + speedY, speedX * 2, speedY, ProjectileID.ThrowingKnife, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

                SoundStyle explosionSound = AssetRegistry.Sounds.Melee.MorrowExp;
                explosionSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound, target.position);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.outerColor = Color.DarkGray;
                    spawnParams.scaleRange *= 0.5f;
                    spawnParams.innerColor = Color.White;
                    DustParticle.Spawn(target.Center, velocity, spawnParams);
                }

                var tp = ThrustParticle.Spawn(target.Center, -Vector2.UnitY, Scale: 0.5f);
                tp.bloomColor = Color.White;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 pos = target.Bottom;
                    pos += Main.rand.NextVector2Circular(32, 32);
                    Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(5f, 10f);
                    var d = Dust.NewDustPerfect(pos, DustID.Cloud, vel, Scale: 0.66f);
                    d.noGravity = true;
                }
                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.outerColor = Color.DarkGray;
                    spawnParams.scaleRange *= 0.5f;
                    spawnParams.innerColor = Color.White;
                    DustParticle.Spawn(target.Center, velocity, spawnParams);
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(target.Center,
                        innerColor: Color.White,
                        glowColor: Color.Gray,
                        outerGlowColor: Color.DarkGray,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
                FXUtil.ShakeCamera(target.Center, 1024, 32);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.outerColor = Color.DarkGray;
                    spawnParams.scaleRange *= 0.25f;
                    spawnParams.innerColor = Color.White;
                    DustParticle.Spawn(target.Center, velocity, spawnParams);
                }
                FXUtil.GlowCircleBoom(target.Center,
                   innerColor: Color.White,
                   glowColor: Color.Black,
                   outerGlowColor: Color.Black, duration: 25, baseSize: 0.12f);

                var fx2 = FXUtil.GlowCircleBoom(target.Center,
                   innerColor: Color.White,
                   glowColor: Color.Black,
                   outerGlowColor: Color.Black, duration: 25, baseSize: 0.12f);
                fx2.Scale *= 1.5f;
                fx2.InnerColor *= 0.5f;
                fx2.OuterGlowColor *= 0.5f;
                fx2.GlowColor *= 0.5f;
                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
            }
        }
    }
}
