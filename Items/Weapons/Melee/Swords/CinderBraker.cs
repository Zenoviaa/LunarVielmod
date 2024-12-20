using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Swords;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Swords
{
    public class CinderBraker : BaseSwingItem
    {

        public int dir;

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 12;
            Item.mana = 3;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cinder Braker");
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 126;
            Item.useAnimation = 126;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<CinderBreakerSlash>();
            Item.autoReuse = true;
            meleeWeaponType = MeleeWeaponType.Sword;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 60;
            //Set max combo
            maxCombo = 5;


            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 1;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<CinderBreakerStaminaSlash>();
        }
    }


    public class CinderBreakerSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/CinderBraker";
        public bool Hit;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            holdOffset = 40;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 38;
            Projectile.width = 38;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);
            SoundStyle swingSound1 = SoundRegistry.NSwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.NSwordSlash2;
            swingSound2.PitchVariance = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 80,
                swingYRadius = 48,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 80,
                swingYRadius = 48,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 80,
                swingYRadius = 64,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 16,
                swingXRadius = 80,
                swingYRadius = 64,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            float circleRange = MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.TwoPi;
            swings.Add(new CircleSwingStyle
            {
                swingTime = 64,
                startSwingRotOffset = -circleRange,
                endSwingRotOffset = circleRange,
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound3
            });
        }


        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            if (ComboIndex == 5)
            {
                Projectile.localNPCHitCooldown = 2 * ExtraUpdateMult;
            }
        }

        public override void AI()
        {
            base.AI();

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(18));
                vel *= Main.rand.NextFloat(0.25f, 1.0f);
                Dust.NewDustPerfect(target.Center, DustID.Torch, vel);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + Projectile.velocity, Projectile.velocity, ModContent.ProjectileType<CinderBrakerSword>(),
                Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (Main.rand.NextBool(4))
                target.AddBuff(BuffID.OnFire, 180);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);


            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }

        //TRAIL VISUALS
        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 32, Easing.InExpo(completionRatio) * t) * Easing.SpikeOutCirc(uneasedLerpValue);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.Yellow, Color.Red, completionRatio), completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 252);

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.CrystalTrail);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 155);
        }
    }

    public class CinderBreakerStaminaSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/CinderBraker";

        public bool Hit;
        public bool AuroraProj2;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            holdOffset = 40;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 38;
            Projectile.width = 38;
            Projectile.friendly = true;
            Projectile.scale = 1.3f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        private bool _thrust;
        public float thrustSpeed = 5;
        public float stabRange;
        public override void AI()
        {
            base.AI();

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (_smoothedLerpValue > 0.5f && !AuroraProj2)
            {
                SoundStyle soundStyle = SoundRegistry.IceyWind;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.Yellow,
                   outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.Yellow,
                   outerGlowColor: Color.Red, duration: 25, baseSize: 0.2f);

                for (float f = 0; f < 12; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel *= Main.rand.NextFloat(0.5f, 1.5f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                    Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.Red);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, shootVelocity,
                        ModContent.ProjectileType<CinderBreakerEruptor>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                }
                AuroraProj2 = true;
            }

            if (_smoothedLerpValue > 0.5f)
            {

                if (!_thrust)
                {
                    Owner.velocity += swingDirection * thrustSpeed;
                    _thrust = true;
                }
            }



        }
        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;


            base.SetComboDefaults(swings);
            swings.Add(new OvalSwingStyle
            {
                swingTime = 72,
                swingXRadius = 160 / 1.5f,
                swingYRadius = 80 / 1.5f,
                swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.PiOver4 + MathHelper.Pi,
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f

            });
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Hit = true;
                hitstopTimer = 4 * ExtraUpdateMult;
            }
            target.AddBuff(BuffID.OnFire, 180);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit = SoundRegistry.CrystalHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }

        //TRAIL VISUALS
        //TRAIL VISUALS
        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 32, completionRatio * t);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.Yellow, Color.Red, completionRatio), Easing.SpikeOutCirc(completionRatio) * Timer / 60f);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 155);

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.CrystalTrail);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 155);

        }
    }

    public class CinderBreakerEruptor : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float EruptionCount => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer >= 5)
            {
                Timer = 0;
            }
            if (Timer == 1 && EruptionCount < 15 && Main.myPlayer == Projectile.owner)
            {

                EruptionCount++;

                float offset = ProjectileHelper.PerformBeamHitscan(Projectile.Top, Vector2.UnitY, 1200);
                Vector2 spawnPoint = Projectile.Top + new Vector2(0, offset);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint - new Vector2(0, 384 / 2), Vector2.Zero,
                    ModContent.ProjectileType<CinderBreakerEruption>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }

    public class CinderBreakerEruption : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 384;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                ShakeModSystem.Shake = 4;
                FXUtil.ShakeCamera(Projectile.position, 1024, 8);
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/MorrowExp"), Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger2"), Projectile.position);
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(Projectile.Bottom, ModContent.DustType<GlowDust>(), (-Vector2.UnitY * Main.rand.Next(1, 15)).RotatedByRandom(MathHelper.ToRadians(60)), 0, Color.Yellow, 2f).noGravity = true;
                }
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(Projectile.Bottom, ModContent.DustType<TSmokeDust>(), (-Vector2.UnitY * Main.rand.Next(1, 15)).RotatedByRandom(MathHelper.ToRadians(60)), 0, Color.Orange, 1f).noGravity = true;
                }

                /*
                FXUtil.GlowCircleBoom(Projectile.Bottom,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);*/
                var part = FXUtil.GlowCircleBoom(Projectile.Bottom,
                   innerColor: Color.White,
                   glowColor: Color.Yellow,
                   outerGlowColor: Color.Red, duration: 25, baseSize: 0.15f);
                part.Pixelation = 0.03f;

                part = FXUtil.GlowCircleBoom(Projectile.Bottom,
                   innerColor: Color.White,
                   glowColor: Color.Yellow,
                   outerGlowColor: Color.Red, duration: 25, baseSize: 0.1f);
                part.Pixelation = 0.03f;
                var part2 = FXUtil.GlowSpikeBoom(Projectile.Center,
                    innerColor: Color.Yellow,
                    glowColor: Color.OrangeRed,
                    outerGlowColor: Color.DarkRed, duration: 25, baseSize: 0.24f);
                part2.Pixelation = 0.03f;
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Bottom,
                        innerColor: Color.DarkRed,
                        glowColor: Color.DarkRed,
                        outerGlowColor: Color.Black, baseSize: 0.12f, duration: 25);
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Pixelation = 0.03f;
                }
            }
        }
    }
}