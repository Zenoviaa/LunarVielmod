using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class Parendine : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSafunai();
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;

            Item.shoot = ModContent.ProjectileType<ParendineProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.damage = 16;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<WinterbornShard, BlankSafunai>();
        }
    }

    public class ParendineProj : BaseSafunaiProjectile
    {

        public SlashEffect SlashEffect { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();

            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.LightCyan;
            blackFireShader.OuterColor = Color.Cyan;
            blackFireShader.BackColor = Color.DarkBlue;
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 80;
                },
                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.Cyan, Color.Violet, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }

            };

            Trailer = devilsPeak;
            Trailer.TrailColorFunction = GetTrailColor;
            Trailer.TrailWidthFunction = GetTrailWidth;
        }
        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.InOutCubic(interpolant) * 64;
        }
        private Color GetTrailColor(float interpolant)
        {
            return Color.Lerp(Color.White, Color.Transparent, interpolant);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath with { Volume = 0.124f }, target.Center);
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.NextFloat(.2f, .3f) * 0.01f;
            if (Slam)
            {
                //Hit Sound
                SoundStyle parendineHitSound = AssetRegistry.Sounds.Melee.Parendine;
                parendineHitSound.Volume = 0.3f;
                parendineHitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(parendineHitSound, target.Center);

                FXUtil.ShakeCamera(target.Center, 1024, 32);
                float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);

                for (float n = 0; n < 4; n++)
                {
                    DustParticle dp = Particle<DustParticle>.Spawn(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4f, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                    dp.outerColor = Color.Blue;
                }
                for (float f = 0; f < 4; f++)
                {
                    var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    smoke.initialColor = Color.DarkGray;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(target.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightBlue,
                        outerGlowColor: Color.DarkBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            else
            {
                float boomSize = Main.rand.NextFloat(0.04f, 0.08f);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.Cyan,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);
                for (float n = 0; n < 2; n++)
                {
                    DustParticle dp = Particle<DustParticle>.Spawn(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4f, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                    dp.outerColor = Color.Blue;
                }
                SoundStyle parendineHitSound = AssetRegistry.Sounds.Melee.Parendine2;
                parendineHitSound.PitchVariance = 0.2f;
                parendineHitSound.Volume = 0.2f;
                SoundEngine.PlaySound(parendineHitSound, target.Center);
            }
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }
    }
}
