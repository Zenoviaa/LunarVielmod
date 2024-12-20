
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Knives
{
    public class LightKnives : BaseSwingItem
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.CrystalMoon.hjson' file.
        public override DamageClass AlternateClass => DamageClass.Throwing;
        public override void SetDefaults()
        {
            Item.damage = 12;
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
            Item.shoot = ModContent.ProjectileType<LightKnivesSwordSlash>();
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
            staminaProjectileShoot = ModContent.ProjectileType<LightKnivesStaminaSlash>();
        }
    }

    public class LightKnivesSwordSlash2 : LightKnivesSwordSlash
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            ShouldSpawnKnife = false;
        }
    }
    public class LightKnivesSwordSlash : BaseSwingProjectile
    {
        public override string Texture => this.PathHere() + "/LightKnivesHeld";
        ref float ComboAtt => ref Projectile.ai[0];
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
            if (!_hasSpawnedSecondKnife && ComboAtt != 8 && ShouldSpawnKnife && _smoothedLerpValue >= 0.9f)
            {
                ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
                int combo = (int)(ComboIndex + 1);
                int dir = comboPlayer.ComboDirection;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, ModContent.ProjectileType<LightKnivesSwordSlash2>(), Projectile.damage, Projectile.knockBack,
                               Owner.whoAmI, ai2: combo, ai1: dir);
                _hasSpawnedSecondKnife = true;
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
            if (ComboIndex == 8)
            {
                modifiers.FinalDamage *= 5;
            }
        }

        //TRAIL VISUALS
        #region Visuals
        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 40;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 106, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Orange, p);
            Color fadeColor = Color.Lerp(trailColor, Color.Red, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.GlowTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail2;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.Orange;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(Main.spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }

    public class LightKnivesStaminaSlash2 : LightKnivesStaminaSlash
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            ShouldSpawnKnife = false;
        }
    }
    public class LightKnivesStaminaSlash : BaseSwingProjectile
    {
        private Common.Shaders.MagicTrails.LightningTrail _lightningTrail;
        private float _lightningTrailTimer;
        private bool _hasSpawnedSecondKnife;
        private bool _thrust;
        public override string Texture => this.PathHere() + "/LightKnivesHeld";
        ref float ComboAtt => ref Projectile.ai[0];
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
            _lightningTrail = new();

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
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, ModContent.ProjectileType<LightKnivesStaminaSlash2>(), Projectile.damage, Projectile.knockBack,
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

            SoundStyle spearHit = SoundRegistry.FireHit1;
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
            int combo = (int)(ComboIndex + 1);
            int dir = comboPlayer.ComboDirection;
            if (ComboIndex < 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }

        //TRAIL VISUALS
        #region Visuals


        public override Vector2 GetTrailOffset()
        {
            //Moves the trail along the blade, negative goes towards the player, positive goes away the player
            return Vector2.One * 40;
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 106, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.Orange, Color.Purple, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DarkViolet, _smoothedLerpValue);
            //   fadeColor.A = 0;
            //This will make it fade out near the end
            return fadeColor;
        }
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = MagicVaellusShader.Instance;
            shader.PrimaryTexture = TrailRegistry.LightningTrail2;
            shader.NoiseTexture = TrailRegistry.LightningTrail3;
            shader.OutlineTexture = TrailRegistry.LightningTrail2Outline;
            shader.PrimaryColor = new Color(69, 70, 159);
            shader.NoiseColor = new Color(224, 107, 10);
            shader.OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            shader.Alpha = 1f;
            _lightningTrail.Draw(spriteBatch, trailPoints, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
        #endregion
    }
}
