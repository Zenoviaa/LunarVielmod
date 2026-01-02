using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class Halhurish : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSafunai();
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 30;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<HalhurishProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.damage = 13;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankSafunai>();
        }
    }

    public class HalhurishProj : BaseSafunaiProjectile
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
            };

            Trailer = devilsPeak;
            Trailer.TrailColorFunction = GetTrailColor;
            Trailer.TrailWidthFunction = GetTrailWidth;
        }

        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.InOutCubic(interpolant) * 32;
        }

        private Color GetTrailColor(float interpolant)
        {
            Color lerp1 = Color.Lerp(Color.OrangeRed, Color.RosyBrown, interpolant);
            return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Play Explosion SOund
            SoundStyle expSound = AssetRegistry.Sounds.Melee.MorrowExp;
            expSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(expSound, target.position);


            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;
            if (Slam)
            {
                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);

                FXUtil.ShakeCamera(target.Center, 1024, 32);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

                for (float f = 0; f < 4; f++)
                {
                    Particle<DustParticle>.Spawn(target.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                for (float f = 0; f < 4; f++)
                {
                    var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    smoke.initialColor = Color.DarkGray;
                }


                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(target.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.07f);

                for (float f = 0; f < 4; f++)
                {
                    Particle<DustParticle>.Spawn(target.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
            }
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
        }
    }
}
