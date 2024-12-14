
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Stellamod.Common.Bases;
using Stellamod.Common.Players;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Magic;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Swords
{
    public class Auroran : BaseSwingItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 5;

        }
        public int AttackCounter = 1;
        public int combowombo = 0;

        public override void SetDefaults()
        {
            Item.damage = 10;
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
            Item.shoot = ModContent.ProjectileType<AuroranSlash>();
            Item.autoReuse = true;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 60;
            //Set max combo
            maxCombo = 5;


            //Set stamina to use
            staminaToUse = 1;
            //set staminacombo
            maxStaminaCombo = 2;
            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<AuroranStaminaSlash>();
        }
    }


    public class AuroranSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Auroran";
        public bool Hit;
        public bool AuroraProj1;
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
                swingTime = 24,
                swingXRadius = 80,
                swingYRadius = 48,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
                swingXRadius = 80,
                swingYRadius = 48,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
                swingXRadius = 80,
                swingYRadius = 64,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 24,
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
            if (!AuroraProj1 && _smoothedLerpValue > 0.5f)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity, ModContent.ProjectileType<Aurora>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                AuroraProj1 = true;
            }
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
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.Blue, Color.BlueViolet, completionRatio), completionRatio);
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

    public class AuroranStaminaSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/Auroran";
        ref float ComboAtt => ref Projectile.ai[0];
        float ProjTimer;
        public bool Hit;
        public bool AuroraProj1;
        public bool AuroraProj2;
        public bool AuroraProj3;

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
                   glowColor: Color.LightCoral,
                   outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.3f);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.LightCoral,
                   outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.2f);

                for (float f = 0; f < 12; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel *= Main.rand.NextFloat(0.5f, 1.5f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                    Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightBlue);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity, ModContent.ProjectileType<AuroranBullet>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity * 0.8f, ModContent.ProjectileType<AuroranBullet2>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity * 1.2f, ModContent.ProjectileType<AuroranBullet3>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
           
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
                swingTime = 68,
                swingXRadius = 160 / 1.5f,
                swingYRadius = 80 / 1.5f,
                swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4 + MathHelper.PiOver4 + MathHelper.Pi,
                easingFunc = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f

            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 68,
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

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = ComboIndex + 1;
            int dir = comboPlayer.ComboDirection;


            if (ComboIndex < 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }


        }
        //TRAIL VISUALS
        //TRAIL VISUALS
        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 48, completionRatio * t);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.Blue, Color.BlueViolet, completionRatio), Easing.SpikeOutCirc(completionRatio) * Timer / 60f);
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
}