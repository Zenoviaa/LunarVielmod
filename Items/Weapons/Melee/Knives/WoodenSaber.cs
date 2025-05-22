
using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Common.Players;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Knives
{
    public class WoodenSaber : BaseSwingItem
    {
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 4;
            Item.mana = 0;
        }

        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<WoodenSaberSwordSlash>();
            Item.autoReuse = true;

            //Combo variables
            //Set combo wait time
            comboWaitTime = 60;
            //Set max combo
            maxCombo = 9;





            //Set stamina to use
            staminaToUse = 1;

            //set staminacombo
            maxStaminaCombo = 3;

            //Set stamina projectile
            staminaProjectileShoot = ModContent.ProjectileType<WoodenSaberStaminaSlash>();
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<Ivythorn>());
        }
    }

    public class WoodenSaberSwordSlash2 : WoodenSaberSwordSlash
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            ShouldSpawnKnife = false;
        }
    }
    public class WoodenSaberSwordSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/WoodenSaber";
        public bool Hit;
        private bool _hasDashed;
        private bool _hasSpawnedSecondKnife;
        public bool ShouldSpawnKnife;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            holdOffset = 40;
            trailStartOffset = 0.2f;
            ShouldSpawnKnife = true;
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

            SoundStyle swingSound1 = SoundRegistry.NSwordSlash2;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.NSwordSlash2;
            swingSound2.PitchVariance = 0.5f;
            swingSound2.Pitch = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSlash1;
            swingSound3.PitchVariance = 0.5f;


            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 84,
                swingYRadius = 42,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 72,
                swingYRadius = 36,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 84,
                swingYRadius = 56,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 96,
                swingYRadius = 32,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 76,
                swingYRadius = 24,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 84,
                swingYRadius = 32,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 80,
                swingYRadius = 16,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound1,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 8,
                swingXRadius = 76,
                swingYRadius = 24,
                swingRange = MathHelper.ToRadians(315),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });

            if (ShouldSpawnKnife)
            {
                swings.Add(new KnifeThrowStyle
                {
                    swingTime = 36,
                    throwRange = 200,
                    spinRotationRange = MathHelper.ToRadians(720),
                    thrustSpeed = 5,
                    easingFunc = (float lerpValue) => Easing.OutCirc(lerpValue),
                    //dashEasingFunc = (float lerpValue) => Easing.InCirc(lerpValue),
                    swingSound = swingSound3
                });
            }

            if (!ShouldSpawnKnife)
            {
                swings.Add(new OvalSwingStyle
                {
                    swingTime = 6,
                    swingXRadius = 76,
                    swingYRadius = 24,
                    swingRange = MathHelper.ToRadians(315),
                    easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                    swingSound = swingSound2,
                    swingSoundLerpValue = 0.5f
                });
            }
        }


        protected override void InitSwingAI()
        {
            base.InitSwingAI();
            if (ComboIndex == 8 && ShouldSpawnKnife)
            {
                Owner.SetImmuneTimeForAllTypes(60);
            }
        }


        public override void AI()
        {
            base.AI();
            if (!_hasSpawnedSecondKnife && ComboIndex != 8 && ShouldSpawnKnife && _smoothedLerpValue >= 0.9f)
            {
                ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
                int combo = (int)(ComboIndex + 1);
                int dir = comboPlayer.ComboDirection;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, ModContent.ProjectileType<WoodenSaberSwordSlash2>(), Projectile.damage, Projectile.knockBack,
                               Owner.whoAmI, ai2: combo, ai1: dir);
                _hasSpawnedSecondKnife = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 2f);
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
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Poisoned, 120);
            }
            if (ComboIndex == 8)
            {
                modifiers.FinalDamage *= 5;
            }
        }

        //TRAIL VISUALS
        //TRAIL VISUALS
        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 32, Easing.InExpo(completionRatio) * t) * Easing.SpikeOutCirc(uneasedLerpValue);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.Goldenrod, Color.DarkGreen, completionRatio), completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 252);
        }
    }

    public class WoodenSaberStaminaSlash2 : WoodenSaberStaminaSlash
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            ShouldSpawnKnife = false;
        }
    }
    public class WoodenSaberStaminaSlash : BaseSwingProjectile
    {
        private LightningTrail _lightningTrail;
        private float _lightningTrailTimer;
        private bool _hasSpawnedSecondKnife;
        private bool _thrust;
        public override string Texture => this.PathHere() + "/WoodenSaber";
        public bool Hit;
        public bool ShouldSpawnKnife;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            //This is the actual trail itself
            _lightningTrail = new LightningTrail();

            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 1;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 2;


            holdOffset = 40;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            ShouldSpawnKnife = true;
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

        public override void AI()
        {
            base.AI();
            //Spawn the other knife
            if (!_hasSpawnedSecondKnife && ComboIndex < 2 && ShouldSpawnKnife && _smoothedLerpValue >= 0.9f)
            {
                ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
                int combo = (int)(ComboIndex + 1);
                int dir = comboPlayer.ComboDirection;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, ModContent.ProjectileType<WoodenSaberStaminaSlash2>(), Projectile.damage, Projectile.knockBack,
                               Owner.whoAmI, ai2: combo, ai1: dir);
                _hasSpawnedSecondKnife = true;
            }

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (!_thrust && _smoothedLerpValue >= 0.5f)
            {
                float thrustSpeed = 5;
                Owner.velocity += swingDirection * thrustSpeed;
                _thrust = true;
            }
            _lightningTrailTimer++;

            //How often it recalculates the lightning trail
            float randomizeRate = 8;
            if (_lightningTrailTimer % randomizeRate == 0)
            {
                _lightningTrail.RandomPositions(_trailPoints);
            }
        }

        public override void SetComboDefaults(List<BaseSwingStyle> swings)
        {
            base.SetComboDefaults(swings);

            SoundStyle swingSound1 = SoundRegistry.NSwordSlash2;
            swingSound1.PitchVariance = 0.5f;

            SoundStyle swingSound2 = SoundRegistry.NSwordSlash2;
            swingSound2.PitchVariance = 0.5f;
            swingSound2.Pitch = 0.5f;

            SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
            swingSound3.PitchVariance = 0.5f;

            swings.Add(new OvalSwingStyle
            {
                swingTime = 60,
                swingXRadius = 84,
                swingYRadius = 42,
                swingRange = MathHelper.ToRadians(720),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound3,
                swingSoundLerpValue = 0.5f
            });

            swings.Add(new OvalSwingStyle
            {
                swingTime = 60,
                swingXRadius = 72,
                swingYRadius = 36,
                swingRange = MathHelper.ToRadians(720),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound2,
                swingSoundLerpValue = 0.5f
            });


            swings.Add(new OvalSwingStyle
            {
                swingTime = 60,
                swingXRadius = 72,
                swingYRadius = 36,
                swingRange = MathHelper.ToRadians(720),
                easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue),
                swingSound = swingSound2,
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
            int combo = (int)(ComboIndex + 1);
            int dir = comboPlayer.ComboDirection;
            if (ComboIndex < 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 32, Easing.InExpo(completionRatio) * t) * Easing.SpikeOutCirc(uneasedLerpValue);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.Goldenrod, Color.DarkGreen, completionRatio), completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 252);
        }
    }
}